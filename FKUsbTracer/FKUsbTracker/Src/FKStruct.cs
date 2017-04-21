using System;
using System.Runtime.InteropServices;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FKUSBTRACER_DEVICE_ID
    {
        public uint DeviceId;
        public byte Enabled;
        public IntPtr PhysicalDeviceObjectNameSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FKUSBTRACER_FILTER_ENABLED
    {
        public uint DeviceId;
        public byte FilterEnabled;
        public FKUSBTRACER_FILTER_ENABLED(uint devId, bool enabled)
        {
            DeviceId = devId;
            FilterEnabled = Convert.ToByte(enabled);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FKUSBTRACER_FILTER_TRACE
    {
        public uint DeviceId;
        public FKUSBTRACER_REQUEST_TYPE Type;
        public FKUSBTRACER_REQUEST_PARAMS Params;
        public FKUSBTRACER_TIMESTAMP Timestamp;
        public IntPtr BufferSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FKUSBTRACER_AUTOTRACE
    {
        public byte AutoTrace;
        public FKUSBTRACER_AUTOTRACE(bool autoTrace)
        {
            AutoTrace = Convert.ToByte(autoTrace);
        }
    }
}
