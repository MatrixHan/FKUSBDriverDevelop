using System;
using System.Runtime.InteropServices;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    sealed internal partial class FKDeviceManagement
    {
        // from dbt.h
        internal const Int32 DBT_DEVICEARRIVAL = 0X8000;
        internal const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
        internal const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        internal const Int32 DBT_DEVTYP_HANDLE = 6;
        internal const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        internal const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        internal const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        internal const Int32 WM_DEVICECHANGE = 0X219;

        // from setupapi.h
        internal const Int32 DIGCF_PRESENT = 2;
        internal const Int32 DIGCF_ALLCLASSES = 0x4;
        internal const Int32 DIGCF_DEVICEINTERFACE = 0X10;

        /// <summary>
        /// 设备注册属性码
        /// </summary>
        internal enum SPDRP : int
        {
            SPDRP_DEVICEDESC                    = 0x00000000,   // 设备描述(R/W)
            SPDRP_HARDWAREID                    = 0x00000001,   // 硬件ID(R/W)
            SPDRP_COMPATIBLEIDS                 = 0x00000002,   // 兼容ID(R/W)
            SPDRP_UNUSED0                       = 0x00000003,   // 未使用
            SPDRP_SERVICE                       = 0x00000004,   // 服务(R/W)
            SPDRP_UNUSED1                       = 0x00000005,   // 未使用
            SPDRP_UNUSED2                       = 0x00000006,   // 未使用
            SPDRP_CLASS                         = 0x00000007,   // 类(R)
            SPDRP_CLASSGUID                     = 0x00000008,   // 类GUID(R/W)
            SPDRP_DRIVER                        = 0x00000009,   // 驱动(R/W)
            SPDRP_CONFIGFLAGS                   = 0x0000000A,   // 配置标示(R/W)
            SPDRP_MFG                           = 0x0000000B,   // mfg(R/W)
            SPDRP_FRIENDLYNAME                  = 0x0000000C,   // 友名(R/W)
            SPDRP_LOCATION_INFORMATION          = 0x0000000D,   // 本地信息(R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME   = 0x0000000E,   // 物理设备对象名(R)
            SPDRP_CAPABILITIES                  = 0x0000000F,   // 容量(R)
            SPDRP_UI_NUMBER                     = 0x00000010,   // Ui(R)
            SPDRP_UPPERFILTERS                  = 0x00000011,   // UpperFilters(R/W)
            SPDRP_LOWERFILTERS                  = 0x00000012,   // LowerFilters(R/W)
            SPDRP_BUSTYPEGUID                   = 0x00000013,   // 总线类型GUID(R)
            SPDRP_LEGACYBUSTYPE                 = 0x00000014,   // 总线类型(R)
            SPDRP_BUSNUMBER                     = 0x00000015,   // 总线个数(R)
            SPDRP_ENUMERATOR_NAME               = 0x00000016,   // 枚举名称(R)
            SPDRP_SECURITY                      = 0x00000017,   // 安全(R/W)
            SPDRP_SECURITY_SDS                  = 0x00000018,   // 安全SDS(W)
            SPDRP_DEVTYPE                       = 0x00000019,   // 设备类型(R/W)
            SPDRP_EXCLUSIVE                     = 0x0000001A,   // 设备高级访问权限(R/W)
            SPDRP_CHARACTERISTICS               = 0x0000001B,   // 设备特性(R/W)
            SPDRP_ADDRESS                       = 0x0000001C,   // 设备地址(R)
            SPDRP_UI_NUMBER_DESC_FORMAT         = 0X0000001D,   // Ui描述模式(R/W)
            SPDRP_DEVICE_POWER_DATA             = 0x0000001E,   // 设备功率描述(R)
            SPDRP_REMOVAL_POLICY                = 0x0000001F,   // 移除策略(R)
            SPDRP_REMOVAL_POLICY_HW_DEFAULT     = 0x00000020,   // 硬件移除策略(R)
            SPDRP_REMOVAL_POLICY_OVERRIDE       = 0x00000021,   // 移除策略重写(R/W)
            SPDRP_INSTALL_STATE                 = 0x00000022,   // 设备安装状态
            SPDRP_LOCATION_PATHS                = 0x00000023,   // 设备本地路径
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            internal Int32 dbcc_size;
            internal Int32 dbcc_devicetype;
            internal Int32 dbcc_reserved;
            internal Guid dbcc_classguid;
            internal Int16 dbcc_name;
        }

#pragma warning disable 0649
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal Int32 cbSize;
            internal System.Guid InterfaceClassGuid;
            internal Int32 Flags;
            internal IntPtr Reserved;
        }
#pragma warning restore 0649
        internal struct SP_DEVINFO_DATA
        {
            internal Int32 cbSize;
            internal System.Guid ClassGuid;
            internal Int32 DevInst;
            internal IntPtr Reserved;
        }



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern Int32 SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, Int32 hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, IntPtr DeviceInstanceId, uint DeviceInstanceIdSize, out uint RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, out UInt32 PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern Boolean UnregisterDeviceNotification(IntPtr Handle);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern int CM_Locate_DevNode(out IntPtr pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern int CM_Get_Child(out IntPtr pdnDevInst, int dnDevInst, int ulFlags);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern int CM_Get_Sibling(out IntPtr pdnDevInst, int DevInst, int ulFlags);
    }
}
