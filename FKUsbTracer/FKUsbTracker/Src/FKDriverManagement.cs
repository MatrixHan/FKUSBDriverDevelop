using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.Globalization;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    static class FKDriverManagement
    {
        #region Invoke

        [StructLayout(LayoutKind.Sequential)]
        public class QUERY_SERVICE_CONFIG
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwServiceType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwStartType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwErrorControl;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpBinaryPathName;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpLoadOrderGroup;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwTagID;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpDependencies;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpServiceStartName;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpDisplayName;
        };

        [DllImport("kernel32")]
        public extern static IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32")]
        public extern static bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public extern static IntPtr GetProcAddress(IntPtr hMod, string procedureName);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);

        delegate uint WdfCoinstallInvoker(
            [MarshalAs(UnmanagedType.LPWStr)]
                    string infPath,
            [MarshalAs(UnmanagedType.LPWStr)]
                    string infSectionName);

        #endregion

        #region Const
        // 需要被打包的驱动资源文件
        const string infFile = "FKUSBTracer.inf";
        const string sysFile = "FKUSBTracer.sys";
        const string dpinstFile = "dpinst.exe";
        const string coinstFile = "WdfCoInstaller01011.dll";

        const string wdfInfSection = "FKUSBTracer.NT.Wdf";

        // WdfCoInstaller中的函数名
        const string wdfPreDeviceInstall = "WdfPreDeviceInstall";
        const string wdfPostDeviceInstall = "WdfPostDeviceInstall";
        const string wdfPreDeviceRemove = "WdfPreDeviceRemove";
        const string wdfPostDeviceRemove = "WdfPostDeviceRemove";

        // 临时驱动解压路径
        const string tmpDriverPath = "FKUsbTracerDriver";

        // 服务名
        const string serviceName = "FKUsbTracer";

        // 资源管理器名
        const string resourceMgrName = "FKUsbTracer.FKResourcePack";
        #endregion

        #region Core function
        // 检查当前本软件驱动是否已经安装
        public static bool IsDriverInstalled(out FileVersionInfo versionInfo)
        {
            string tempdir = Path.GetTempPath();
            string mydir = Path.Combine(tempdir, tmpDriverPath);

            versionInfo = null;
            bool drvInstalled = false;
            const uint GENERIC_READ = 0x80000000;

            // 打开服务控制管理器
            IntPtr scmgr = OpenSCManager(null, null, GENERIC_READ);
            if (!scmgr.Equals(IntPtr.Zero))
            {
                // 查找服务
                const uint SERVICE_QUERY_CONFIG = 0x00000001;
                IntPtr service = OpenService(scmgr, "FKUsbTracer", SERVICE_QUERY_CONFIG);
                if (!service.Equals(IntPtr.Zero))
                {
                    // 获取文件版本号
                    uint dwBytesNeeded = 0;
                    QueryServiceConfig(service, IntPtr.Zero, dwBytesNeeded, out dwBytesNeeded);
                    IntPtr ptr = Marshal.AllocHGlobal((int)dwBytesNeeded);
                    if (QueryServiceConfig(service, ptr, dwBytesNeeded, out dwBytesNeeded))
                    {
                        // 成功找到服务路径
                        QUERY_SERVICE_CONFIG qsConfig = new QUERY_SERVICE_CONFIG();
                        Marshal.PtrToStructure(ptr, qsConfig);
                        var binarypath = qsConfig.lpBinaryPathName;
                        binarypath = Regex.Replace(binarypath, "\\\\SystemRoot\\\\", "", RegexOptions.IgnoreCase);
                        // 获取二进制数据版本号
                        string sysroot = Environment.ExpandEnvironmentVariables("%systemroot%");
                        versionInfo = FileVersionInfo.GetVersionInfo(sysroot + "\\" + binarypath);
                        // 驱动正常安装，并获得了完全信息
                        drvInstalled = true;
                    }
                    Marshal.FreeHGlobal(ptr);
                    CloseServiceHandle(service);
                }
                CloseServiceHandle(scmgr);
            }
            return drvInstalled;
        }

        // 安装本软件驱动
        public static bool InstallDriver(out bool needRestart, out string failureReason)
        {
            failureReason = null;
            bool result = true;
            needRestart = false;
            string mydir;
            if (ExtractDriverFiles(out mydir))
            {
                ulong wdfCallResult = 0;
                IntPtr wdfPreDeviceInstallPtr;
                IntPtr wdfPostDeviceInstallPtr;
                IntPtr wdfPreDeviceRemovePtr;
                IntPtr wdfPostDeviceRemovePtr;
                bool procAddressFailure;
                // 加载 WdfCoInstaller 函数
                IntPtr hModule = GetCoinstallerFuncs(mydir, out wdfPreDeviceInstallPtr, out wdfPostDeviceInstallPtr, out wdfPreDeviceRemovePtr, out wdfPostDeviceRemovePtr, out procAddressFailure);
                if (!procAddressFailure)
                {
                    // 调用 WdfPreDeviceInstall
                    WdfCoinstallInvoker preDevInst = (WdfCoinstallInvoker)Marshal.GetDelegateForFunctionPointer(wdfPreDeviceInstallPtr, typeof(WdfCoinstallInvoker));
                    wdfCallResult = preDevInst(Path.Combine(mydir, infFile), wdfInfSection);
                    if (wdfCallResult != 0)
                    {
                        result = false;
                        failureReason = string.Format("{0} 处理结果 = 0x{1:X}", wdfPreDeviceInstall, wdfCallResult);
                    }
                    else
                    {
                        // 运行 Dpinst.exe 并等待
                        Process p = Process.Start(Path.Combine(mydir, dpinstFile), "/lm /q");
                        p.WaitForExit();

                        if (((p.ExitCode >> 24) & 0x40) == 0x40)
                            needRestart = true;
                        if (((p.ExitCode >> 24) & 0x80) == 0x80)
                        {
                            result = false;
                            failureReason = string.Format("DPInst 处理结果 = 0x{0:X}", p.ExitCode);
                        }
                        else
                        {
                            // 调用 WdfPostDeviceInstall
                            WdfCoinstallInvoker postDevInst = (WdfCoinstallInvoker)Marshal.GetDelegateForFunctionPointer(wdfPostDeviceInstallPtr, typeof(WdfCoinstallInvoker));
                            wdfCallResult = postDevInst(Path.Combine(mydir, infFile), wdfInfSection);
                            if (wdfCallResult != 0)
                            {
                                result = false;
                                failureReason = string.Format("{0} 处理结果 = 0x{1:X}", wdfPostDeviceInstall, wdfCallResult);
                            }
                        }
                    }
                }
                else
                {
                    result = false;
                    failureReason = "获取 WdfCoInstaller 库文件函数路径失败，驱动未正常安装...";
                }
                // 释放 WdfCoInstaller 库
                FreeLibrary(hModule);
                // 删除临时文件
                if (Directory.Exists(mydir))
                    Directory.Delete(mydir, true);
            }
            else
            {
                result = false;
                failureReason = "解压驱动文件失败，驱动未能正常安装...";
            }
            return result;
        }

        // 卸载本软件驱动
        public static bool UninstallDriver(out bool needRestart, out string failureReason)
        {
            failureReason = null;
            ulong wdfCallResult = 0;
            bool result = true;
            needRestart = false;
            string mydir;
            if (ExtractDriverFiles(out mydir))
            {
                IntPtr wdfPreDeviceInstallPtr;
                IntPtr wdfPostDeviceInstallPtr;
                IntPtr wdfPreDeviceRemovePtr;
                IntPtr wdfPostDeviceRemovePtr;
                bool procAddressFailure;

                // 加载 WdfCoInstaller 函数
                IntPtr hModule = GetCoinstallerFuncs(mydir, out wdfPreDeviceInstallPtr, out wdfPostDeviceInstallPtr, out wdfPreDeviceRemovePtr, out wdfPostDeviceRemovePtr, out procAddressFailure);
                if (!procAddressFailure)
                {
                    // 调用 WdfPreDeviceRemove
                    WdfCoinstallInvoker preDevRemove = (WdfCoinstallInvoker)Marshal.GetDelegateForFunctionPointer(wdfPreDeviceRemovePtr, typeof(WdfCoinstallInvoker));
                    wdfCallResult = preDevRemove(Path.Combine(mydir, infFile), wdfInfSection);
                    if (wdfCallResult != 0)
                    {
                        result = false;
                        failureReason = string.Format("{0} 处理结果 = 0x{1:X}", wdfPreDeviceRemove, wdfCallResult);
                    }
                    else
                    {
                        // 运行 Dpinst.exe 并等待
                        string inffile = Path.Combine(mydir, infFile);
                        Process p = Process.Start(Path.Combine(mydir, dpinstFile), string.Format("/u \"{0}\" /d /q", inffile));
                        p.WaitForExit();

                        if (((p.ExitCode >> 24) & 0x40) == 0x40)
                            needRestart = true;
                        if (((p.ExitCode >> 24) & 0x80) == 0x80)
                        {
                            result = false;
                            failureReason = string.Format("DPInst 处理结果 = 0x{0:X}", p.ExitCode);
                        }
                        else
                        {
                            // 调用 WdfPostDeviceRemove
                            WdfCoinstallInvoker postDevRemove = (WdfCoinstallInvoker)Marshal.GetDelegateForFunctionPointer(wdfPostDeviceRemovePtr, typeof(WdfCoinstallInvoker));
                            wdfCallResult = postDevRemove(Path.Combine(mydir, infFile), wdfInfSection);
                            if (wdfCallResult != 0)
                            {
                                result = false;
                                failureReason = string.Format("{0} 处理结果 = 0x{1:X}", wdfPostDeviceRemove, wdfCallResult);
                            }
                        }
                    }
                }
                else
                {
                    result = false;
                    failureReason = "获取 WdfCoInstaller 库文件函数路径失败，驱动未正常卸载...";
                }
                // 释放 WdfCoInstaller 库
                FreeLibrary(hModule);
                // 删除临时文件
                if (Directory.Exists(mydir))
                    Directory.Delete(mydir, true);
            }
            else
            {
                result = false;
                failureReason = "解压驱动文件失败，驱动未能正常卸载...";
            }
            return result;
        }
        #endregion

        #region Help function
        // 解压缩资源中的驱动文件
        private static bool ExtractDriverFiles(out string mydir)
        {
            string tempdir = Path.GetTempPath();
            mydir = Path.Combine(tempdir, tmpDriverPath);
            try
            {
                // 逐个解压资源驱动问及那
                Directory.CreateDirectory(mydir);
                WriteDrverFile(sysFile, mydir);
                WriteDrverFile(infFile, mydir);
                WriteDrverFile(coinstFile, mydir);
                WriteDrverFile(dpinstFile, mydir);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
        // 将资源中的驱动文件写入到指定目录
        private static void WriteDrverFile(string resname, string dir)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            ResourceManager rm = new ResourceManager(resourceMgrName, ass);

            ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            byte[] buf = (byte[])set.GetObject(resname);

            Stream w = File.OpenWrite(Path.Combine(dir, resname));
            w.Write(buf, 0, buf.Length);
            w.Flush();
            w.Close();
        }
        // 获取 WdfCoInstaller 的函数地址
        private static IntPtr GetCoinstallerFuncs(string mydir, out IntPtr wdfPreDeviceInstallPtr, 
            out IntPtr wdfPostDeviceInstallPtr, out IntPtr wdfPreDeviceRemovePtr,
            out IntPtr wdfPostDeviceRemovePtr, out bool procAddressFailure)
        {
            procAddressFailure = false;
            // 获取WdfCoInstaller函数地址
            IntPtr hModule = LoadLibrary(Path.Combine(mydir, coinstFile));
            wdfPreDeviceInstallPtr = GetProcAddress(hModule, wdfPreDeviceInstall);
            wdfPostDeviceInstallPtr = GetProcAddress(hModule, wdfPostDeviceInstall);
            wdfPreDeviceRemovePtr = GetProcAddress(hModule, wdfPreDeviceRemove);
            wdfPostDeviceRemovePtr = GetProcAddress(hModule, wdfPostDeviceRemove);

            if (wdfPreDeviceInstallPtr.ToInt64() == 0 ||
                wdfPostDeviceInstallPtr.ToInt64() == 0 ||
                wdfPreDeviceRemovePtr.ToInt64() == 0 ||
                wdfPostDeviceRemovePtr.ToInt64() == 0)
            {
                procAddressFailure = true;
            }

            return hModule;
        }
        #endregion
    }
}
