using System;
using System.Runtime.InteropServices;
using System.Threading;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    class FKDeviceIoOverlapped
    {
        private IntPtr mPtrOverlapped = IntPtr.Zero;

        private int mFieldOffset_InternalLow = 0;
        private int mFieldOffset_InternalHigh = 0;
        private int mFieldOffset_OffsetLow = 0;
        private int mFieldOffset_OffsetHigh = 0;
        private int mFieldOffset_EventHandle = 0;

        public FKDeviceIoOverlapped()
        {
            // 为重叠结构分配内存
            mPtrOverlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));

            // 找到 NativeOverlapped 结构中的各单元偏移
            mFieldOffset_InternalLow = Marshal.OffsetOf(typeof(NativeOverlapped), "InternalLow").ToInt32();
            mFieldOffset_InternalHigh = Marshal.OffsetOf(typeof(NativeOverlapped), "InternalHigh").ToInt32();
            mFieldOffset_OffsetLow = Marshal.OffsetOf(typeof(NativeOverlapped), "OffsetLow").ToInt32();
            mFieldOffset_OffsetHigh = Marshal.OffsetOf(typeof(NativeOverlapped), "OffsetHigh").ToInt32();
            mFieldOffset_EventHandle = Marshal.OffsetOf(typeof(NativeOverlapped), "EventHandle").ToInt32();
        }

        public IntPtr InternalLow
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, mFieldOffset_InternalLow); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, mFieldOffset_InternalLow, value); }
        }

        public IntPtr InternalHigh
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, mFieldOffset_InternalHigh); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, mFieldOffset_InternalHigh, value); }
        }

        public int OffsetLow
        {
            get { return Marshal.ReadInt32(mPtrOverlapped, mFieldOffset_OffsetLow); }
            set { Marshal.WriteInt32(mPtrOverlapped, mFieldOffset_OffsetLow, value); }
        }

        public int OffsetHigh
        {
            get { return Marshal.ReadInt32(mPtrOverlapped, mFieldOffset_OffsetHigh); }
            set { Marshal.WriteInt32(mPtrOverlapped, mFieldOffset_OffsetHigh, value); }
        }

        // 重叠事件处理函数指针
        public IntPtr EventHandle
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, mFieldOffset_EventHandle); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, mFieldOffset_EventHandle, value); }
        }

        public IntPtr GlobalOverlapped
        {
            get { return mPtrOverlapped; }
        }

        // 设置重叠事件回调函数指针，并清除数据结构信息
        public void ClearAndSetEvent(IntPtr hEventOverlapped)
        {
            EventHandle = hEventOverlapped;
            InternalLow = IntPtr.Zero;
            InternalHigh = IntPtr.Zero;
            OffsetLow = 0;
            OffsetHigh = 0;
        }

        // 清除已分配全局内存
        ~FKDeviceIoOverlapped()
        {
            if (mPtrOverlapped != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mPtrOverlapped);
                mPtrOverlapped = IntPtr.Zero;
            }
        }
    }
}
