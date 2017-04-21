using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    public enum FKUSBTRACER_REQUEST_TYPE
    {
        FKUSBTracerReadRequest = 0x2000,
        FKUSBTracerWriteRequest,
        FKUSBTracerDeviceControlRequest,
        FKUSBTracerInternalDeviceControlRequest,
        FKUSBTracerPnPRequest,
        FKUSBTracerMaxRequestType
    }

    public enum FKUSBTRACER_REQUEST_INTERNAL_DEVICE_CONTROL_TYPE
    {
        FKUSBTracerSubmitURB = 0x2000,
        FKUSBTracerResetPort
    };

    public enum FKUSBTRACER_REQUEST_USB_DIRECTION
    {
        FKUSBTracerIn = 0x0,
        FKUSBTracerOut
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FKUSBTRACER_REQUEST_PARAMS
    {
        public uint p1;
        public uint p2;
        public uint p3;
        public uint p4;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FKUSBTRACER_TIMESTAMP
    {
        public int Seconds;
        public int USec;

        public override string ToString()
        {
            return string.Format("{0}.{1:D6}", Seconds, USec);
        }
    }

    public struct FKFilterTrace
    {
        #region URB request codes
        const int URB_FUNCTION_SELECT_CONFIGURATION = 0x0000;
        const int URB_FUNCTION_SELECT_INTERFACE = 0x0001;
        const int URB_FUNCTION_ABORT_PIPE = 0x0002;
        const int URB_FUNCTION_TAKE_FRAME_LENGTH_CONTROL = 0x0003;
        const int URB_FUNCTION_RELEASE_FRAME_LENGTH_CONTROL = 0x0004;
        const int URB_FUNCTION_GET_FRAME_LENGTH = 0x0005;
        const int URB_FUNCTION_SET_FRAME_LENGTH = 0x0006;
        const int URB_FUNCTION_GET_CURRENT_FRAME_NUMBER = 0x0007;
        const int URB_FUNCTION_CONTROL_TRANSFER = 0x0008;
        const int URB_FUNCTION_BULK_OR_INTERRUPT_TRANSFER = 0x0009;
        const int URB_FUNCTION_ISOCH_TRANSFER = 0x000A;
        const int URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE = 0x000B;
        const int URB_FUNCTION_SET_DESCRIPTOR_TO_DEVICE = 0x000C;
        const int URB_FUNCTION_SET_FEATURE_TO_DEVICE = 0x000D;
        const int URB_FUNCTION_SET_FEATURE_TO_INTERFACE = 0x000E;
        const int URB_FUNCTION_SET_FEATURE_TO_ENDPOINT = 0x000F;
        const int URB_FUNCTION_CLEAR_FEATURE_TO_DEVICE = 0x0010;
        const int URB_FUNCTION_CLEAR_FEATURE_TO_INTERFACE = 0x0011;
        const int URB_FUNCTION_CLEAR_FEATURE_TO_ENDPOINT = 0x0012;
        const int URB_FUNCTION_GET_STATUS_FROM_DEVICE = 0x0013;
        const int URB_FUNCTION_GET_STATUS_FROM_INTERFACE = 0x0014;
        const int URB_FUNCTION_GET_STATUS_FROM_ENDPOINT = 0x0015;
        const int URB_FUNCTION_RESERVED_0X0016 = 0x0016;
        const int URB_FUNCTION_VENDOR_DEVICE = 0x0017;
        const int URB_FUNCTION_VENDOR_INTERFACE = 0x0018;
        const int URB_FUNCTION_VENDOR_ENDPOINT = 0x0019;
        const int URB_FUNCTION_CLASS_DEVICE = 0x001A;
        const int URB_FUNCTION_CLASS_INTERFACE = 0x001B;
        const int URB_FUNCTION_CLASS_ENDPOINT = 0x001C;
        const int URB_FUNCTION_RESERVE_0X001D = 0x001D;
        const int URB_FUNCTION_SYNC_RESET_PIPE_AND_CLEAR_STALL = 0x001E;
        const int URB_FUNCTION_CLASS_OTHER = 0x001F;
        const int URB_FUNCTION_VENDOR_OTHER = 0x0020;
        const int URB_FUNCTION_GET_STATUS_FROM_OTHER = 0x0021;
        const int URB_FUNCTION_CLEAR_FEATURE_TO_OTHER = 0x0022;
        const int URB_FUNCTION_SET_FEATURE_TO_OTHER = 0x0023;
        const int URB_FUNCTION_GET_DESCRIPTOR_FROM_ENDPOINT = 0x0024;
        const int URB_FUNCTION_SET_DESCRIPTOR_TO_ENDPOINT = 0x0025;
        const int URB_FUNCTION_GET_CONFIGURATION = 0x0026;
        const int URB_FUNCTION_GET_INTERFACE = 0x0027;
        const int URB_FUNCTION_GET_DESCRIPTOR_FROM_INTERFACE = 0x0028;
        const int URB_FUNCTION_SET_DESCRIPTOR_TO_INTERFACE = 0x0029;
        const int URB_FUNCTION_GET_MS_FEATURE_DESCRIPTOR = 0x002A;
        const int URB_FUNCTION_SYNC_RESET_PIPE = 0x0030;
        const int URB_FUNCTION_SYNC_CLEAR_STALL = 0x0031;
        #endregion

        #region 变量
        public uint DeviceId;
        public FKUSBTRACER_REQUEST_TYPE Type;
        public FKUSBTRACER_REQUEST_PARAMS Params;
        public FKUSBTRACER_TIMESTAMP Timestamp;
        public byte[] Buffer;
        #endregion

        public FKFilterTrace(uint devId, FKUSBTRACER_REQUEST_TYPE type, FKUSBTRACER_REQUEST_PARAMS params_,
            FKUSBTRACER_TIMESTAMP timestamp, byte[] buffer)
        {
            DeviceId = devId;
            Type = type;
            Params = params_;
            Timestamp = timestamp;
            Buffer = buffer;
        }

        public string TypeToStr()
        {
            switch (Type)
            {
                case FKUSBTRACER_REQUEST_TYPE.FKUSBTracerReadRequest:
                    return "R";
                case FKUSBTRACER_REQUEST_TYPE.FKUSBTracerWriteRequest:
                    return "W";
                case FKUSBTRACER_REQUEST_TYPE.FKUSBTracerInternalDeviceControlRequest:
                    switch ((FKUSBTRACER_REQUEST_INTERNAL_DEVICE_CONTROL_TYPE)Params.p1)
                    {
                        case FKUSBTRACER_REQUEST_INTERNAL_DEVICE_CONTROL_TYPE.FKUSBTracerSubmitURB:
                            switch (Params.p3)
                            {
                                case URB_FUNCTION_ABORT_PIPE:
                                    return "URB_FUNCTION_ABORT_PIPE";
                                case URB_FUNCTION_SYNC_RESET_PIPE_AND_CLEAR_STALL:
                                    return "URB_FUNCTION_SYNC_RESET_PIPE_AND_CLEAR_STALL";
                                case URB_FUNCTION_SYNC_RESET_PIPE:
                                    return "URB_FUNCTION_SYNC_RESET_PIPE";
                                case URB_FUNCTION_SYNC_CLEAR_STALL:
                                    return "URB_FUNCTION_SYNC_CLEAR_STALL";
                                case URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE:
                                    return "URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE";
                                case URB_FUNCTION_GET_DESCRIPTOR_FROM_ENDPOINT:
                                    return "URB_FUNCTION_GET_DESCRIPTOR_FROM_ENDPOINT";
                                case URB_FUNCTION_GET_DESCRIPTOR_FROM_INTERFACE:
                                    return "URB_FUNCTION_GET_DESCRIPTOR_FROM_INTERFACE";
                                case URB_FUNCTION_SET_DESCRIPTOR_TO_DEVICE:
                                    return "URB_FUNCTION_SET_DESCRIPTOR_TO_DEVICE";
                                case URB_FUNCTION_SET_DESCRIPTOR_TO_ENDPOINT:
                                    return "URB_FUNCTION_SET_DESCRIPTOR_TO_ENDPOINT";
                                case URB_FUNCTION_SET_DESCRIPTOR_TO_INTERFACE:
                                    return "URB_FUNCTION_SET_DESCRIPTOR_TO_INTERFACE";
                                default:
                                    return string.Format("{0}  (USB URB Function: {1})",
                                        (FKUSBTRACER_REQUEST_USB_DIRECTION)Params.p2 == FKUSBTRACER_REQUEST_USB_DIRECTION.FKUSBTracerIn ? "In" : "Out",
                                        Params.p3);
                            }
                        case FKUSBTRACER_REQUEST_INTERNAL_DEVICE_CONTROL_TYPE.FKUSBTracerResetPort:
                            return "Reset Port";
                    }
                    goto default;
                default:
                    return string.Format("? (Type: {0}, p1, {1}, p2: {2}, p3: {3}, p4: {4})", 
                        Type, Params.p1, Params.p2, Params.p3, Params.p4);
            }
        }

        public string BufToChars()
        {
            if (Buffer != null)
            {
                StringBuilder sb = new StringBuilder(Buffer.Length);
                sb.Length = Buffer.Length;
                for (int i = 0; i < Buffer.Length; i++)
                {
                    byte b = Buffer[i];
                    if (b > 31 && b < 128)
                        sb[i] = (char)b;
                    else
                        sb[i] = '.';
                }
                return sb.ToString();
            }
            else
                return "";
        }

        public string BufToHex()
        {
            if (Buffer != null)
            {
                StringBuilder sb = new StringBuilder(Buffer.Length * 3 - 1);
                sb.Length = Buffer.Length * 3 - 1;
                for (int i = 0; i < Buffer.Length; i++)
                {
                    string hex = String.Format("{0:x2}", Buffer[i]);
                    sb[i * 3] = hex[0];
                    sb[i * 3 + 1] = hex[1];
                    if (i < Buffer.Length - 1)
                        sb[i * 3 + 2] = ' ';
                }
                return sb.ToString();
            }
            else
                return "";
        }

        public override string ToString()
        {
            return string.Format("{0:D2}: {1}: {2}: {3}", DeviceId, TypeToStr(), Timestamp.ToString(), BufToChars());
        }

        public FKUSBTRACER_TIMESTAMP GetTimestampDelta(FKFilterTrace prevTrace)
        {
            FKUSBTRACER_TIMESTAMP delta = new FKUSBTRACER_TIMESTAMP();
            if (prevTrace.Timestamp.Seconds > 0 || prevTrace.Timestamp.USec > 0)
            {
                delta.Seconds = Timestamp.Seconds - prevTrace.Timestamp.Seconds;
                delta.USec = Timestamp.USec - prevTrace.Timestamp.USec;
                if (delta.USec < 0)
                    delta.USec = 1000000 + delta.USec;
            }
            return delta;
        }
    }

    public class FKFilterTraceArrivedEventArgs : EventArgs
    {
        public List<FKFilterTrace> Traces;

        public FKFilterTraceArrivedEventArgs(List<FKFilterTrace> traces)
        {
            Traces = traces;
        }
    }
}
