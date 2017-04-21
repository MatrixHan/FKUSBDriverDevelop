using System;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    internal class FKDeviceIOCTL
    {
        #region Third DLL
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            int securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileOptions flags,
            IntPtr overlapped);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            [MarshalAs(UnmanagedType.U4)]uint dwIoControlCode,
            IntPtr lpInBuffer,
            [MarshalAs(UnmanagedType.U4)]uint nInBufferSize,
            IntPtr lpOutBuffer,
            [MarshalAs(UnmanagedType.U4)]uint nOutBufferSize,
            [MarshalAs(UnmanagedType.U4)]out uint lpBytesReturned,
            IntPtr lpOverlapped
        );

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CancelIo(
            SafeFileHandle hFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        // 信号量等待
        public const int WAIT_OBJECT_0 = 0;
        #endregion

        #region 变量
        SafeFileHandle hDevice              = null;
        FKDeviceIoOverlapped overlapped     = null;
        ManualResetEvent hEvent             = null;
        bool bAsync                         = false;
        #endregion

        #region IOCTL definitions
        static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method);
        }

        public static uint FILE_DEVICE_FKUSBTRACER = 0x0F59;

        public static uint METHOD_BUFFERED = 0;
        public static uint METHOD_IN_DIRECT = 1;
        public static uint METHOD_OUT_DIRECT = 2;
        public static uint METHOD_NEITHER = 3;
        public static uint FILE_ANY_ACCESS = 0;
        public static uint FILE_READ_ACCESS = 0x0001;
        public static uint FILE_WRITE_ACCESS = 0x0002;

        public static uint IOCTL_FKUSBTRACER_GET_BUFFER = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2049,METHOD_OUT_DIRECT,FILE_READ_ACCESS);
        public static uint IOCTL_FKUSBTRACER_START_FILTERING = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2050,METHOD_BUFFERED,FILE_READ_ACCESS);
        public static uint IOCTL_FKUSBTRACER_STOP_FILTERING = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2051,METHOD_BUFFERED,FILE_READ_ACCESS);
        public static uint IOCTL_FKUSBTRACER_SET_DEVICE_FILTER_ENABLED = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2053,METHOD_BUFFERED,FILE_WRITE_ACCESS);
        public static uint IOCTL_FKUSBTRACER_GET_DEVICE_LIST = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2054,METHOD_BUFFERED,FILE_READ_ACCESS);
        public static uint IOCTL_FKUSBTRACER_GET_AUTOTRACE = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2057,METHOD_BUFFERED,FILE_READ_ACCESS);
        public static uint IOCTL_FKUSBTRACER_SET_AUTOTRACE = CTL_CODE(FILE_DEVICE_FKUSBTRACER,2058,METHOD_BUFFERED,FILE_WRITE_ACCESS);
        #endregion

        public FKDeviceIOCTL(string devicePath, bool async)
        {
            this.bAsync = async;
            FileOptions fileOps = FileOptions.None;
            if (async){
                overlapped = new FKDeviceIoOverlapped();
                hEvent = new ManualResetEvent(false);
                fileOps = FileOptions.Asynchronous;
            }
            IntPtr pFileHandle = CreateFile(devicePath, FileAccess.ReadWrite, FileShare.ReadWrite, 
                0, FileMode.Open, fileOps, IntPtr.Zero);
            int nHandle = pFileHandle.ToInt32();
            if(nHandle < 0)
            {
                uint errorCode = GetLastError();
                Console.WriteLine("Create FKDeviceIOCTL failed, Error : " + errorCode);
            }
            hDevice = new SafeFileHandle(pFileHandle, true);
        }

        public bool DeviceIoControl(uint code, IntPtr lpInBuffer, uint nInBufferSize,
            IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned)
        {
            IntPtr ol = IntPtr.Zero;
            if (bAsync){
                overlapped.ClearAndSetEvent(hEvent.SafeWaitHandle.DangerousGetHandle());
                ol = overlapped.GlobalOverlapped;
            }
            return DeviceIoControl(hDevice, code, lpInBuffer, nInBufferSize,
                lpOutBuffer, nOutBufferSize, out lpBytesReturned, ol);
        }

        public bool WaitForOverlappedIo(UInt32 dwMilliseconds, out uint bytesRead)
        {
            bytesRead = 0;
            uint res = WaitForSingleObject(hEvent.SafeWaitHandle.DangerousGetHandle(), dwMilliseconds);
            if (res == WAIT_OBJECT_0)
            {
                bytesRead = (uint)overlapped.InternalHigh.ToInt32();
                return true;
            }
            return false;
        }

        public bool CancelIo()
        {
            return CancelIo(hDevice);
        }
    }
}
