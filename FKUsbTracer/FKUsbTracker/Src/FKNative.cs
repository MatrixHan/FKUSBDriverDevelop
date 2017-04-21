using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    public class FKNative
    {
        #region 常量
        uint OUT_BUFFER_SIZE = 0x10000;
        uint IN_BUFFER_SIZE = 0x10000;
        const string FKUsbTracerPath = "\\\\.\\BusDogFilter";
        public const int ERROR_IO_PENDING = 997;                    // 重叠I/O操作
        #endregion

        #region 变量
        IntPtr m_OutBuffer = IntPtr.Zero;
        IntPtr m_InBuffer = IntPtr.Zero;
        FKDeviceIOCTL m_DevIOCTL;
        Thread m_TraceBufferThread;

        public event EventHandler<FKFilterTraceArrivedEventArgs> FilterTraceArrived;
        #endregion

        public FKNative()
        {
            m_OutBuffer = Marshal.AllocHGlobal((int)OUT_BUFFER_SIZE);
            m_InBuffer = Marshal.AllocHGlobal((int)IN_BUFFER_SIZE);
            m_DevIOCTL = new FKDeviceIOCTL(FKUsbTracerPath, false);
            m_TraceBufferThread = new Thread(Func_TraceBufReadThread);
        }
        ~FKNative()
        {
            Marshal.FreeHGlobal(m_OutBuffer);
            Marshal.FreeHGlobal(m_InBuffer);
        }

        // 开启TraceReader
        public void StartTraceReader()
        {
            try
            {
                if (m_TraceBufferThread.ThreadState != ThreadState.Running)
                    m_TraceBufferThread.Start();
            }
            catch
            { }
        }
        // 停止TraceReader
        public void StopTraceReader()
        {
            if (m_TraceBufferThread.ThreadState != ThreadState.Stopped &&
                m_TraceBufferThread.ThreadState != ThreadState.Unstarted)
            {
                m_TraceBufferThread.Abort();
                m_TraceBufferThread.Join();
                // 重新创建了新进程
                m_TraceBufferThread = new Thread(Func_TraceBufReadThread);
            }
        }
        // 开始Trace
        public bool StartTracing()
        {
            uint bytesReturned;
            return m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_START_FILTERING,
                    IntPtr.Zero, 0, IntPtr.Zero, 0, out bytesReturned);
        }
        // 停止Trace
        public bool StopTracing()
        {
            uint bytesReturned;
            return m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_STOP_FILTERING,
                    IntPtr.Zero, 0, IntPtr.Zero, 0, out bytesReturned);
        }
        // 获取USB设备列表信息
        public bool GetDeviceList(out List<FKDeviceID> deviceIds)
        {
            deviceIds = new List<FKDeviceID>();
            uint bytesReturned;
            bool result = m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_GET_DEVICE_LIST,
                                            IntPtr.Zero,0, m_OutBuffer,OUT_BUFFER_SIZE,out bytesReturned);
            if (result)
            {
                int index = 0;
                while (bytesReturned >= index + Marshal.SizeOf(typeof(FKUSBTRACER_DEVICE_ID)))
                {
                    FKUSBTRACER_DEVICE_ID devId = (FKUSBTRACER_DEVICE_ID)Marshal.PtrToStructure(new IntPtr(m_OutBuffer.ToInt64() + index),typeof(FKUSBTRACER_DEVICE_ID));
                    index += Marshal.SizeOf(typeof(FKUSBTRACER_DEVICE_ID));
                    string hardwareId = Marshal.PtrToStringUni(new IntPtr(m_OutBuffer.ToInt64() + index),devId.PhysicalDeviceObjectNameSize.ToInt32() / 2);
                    index += devId.PhysicalDeviceObjectNameSize.ToInt32();
                    deviceIds.Add(new FKDeviceID(devId.DeviceId, Convert.ToBoolean(devId.Enabled), hardwareId));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(Marshal.GetLastWin32Error());
            }
            return result;
        }
        // 设置指定设备是否可用
        public bool SetDeviceEnabled(uint deviceId, bool enabled)
        {
            GCHandle h = GCHandle.Alloc(new FKUSBTRACER_FILTER_ENABLED(deviceId, enabled), GCHandleType.Pinned);
            IntPtr p = h.AddrOfPinnedObject();
            uint bytesReturned;
            bool result = m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_SET_DEVICE_FILTER_ENABLED,
                    p, (uint)Marshal.SizeOf(typeof(FKUSBTRACER_FILTER_ENABLED)), IntPtr.Zero, 0, out bytesReturned);
            if (!result)
            {
                System.Diagnostics.Debug.WriteLine(Marshal.GetLastWin32Error());
            }
            h.Free();
            return result;
        }
        // 开启自动Trace
        public bool SetAutoTrace(bool value)
        {
            GCHandle h = GCHandle.Alloc(new FKUSBTRACER_AUTOTRACE(value), GCHandleType.Pinned);
            IntPtr p = h.AddrOfPinnedObject();
            uint bytesReturned;
            bool result = m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_SET_AUTOTRACE,
                    p, (uint)Marshal.SizeOf(typeof(FKUSBTRACER_AUTOTRACE)), IntPtr.Zero, 0, out bytesReturned);
            if (!result)
            {
                System.Diagnostics.Debug.WriteLine(Marshal.GetLastWin32Error());
            }
            h.Free();
            return result;
        }
        // 当前是否开启了自动Trace
        public bool GetAutoTrace(out bool value)
        {
            value = false;
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FKUSBTRACER_AUTOTRACE)));
            uint bytesReturned;
            bool result = m_DevIOCTL.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_GET_AUTOTRACE,
                    IntPtr.Zero, 0, p, (uint)Marshal.SizeOf(typeof(FKUSBTRACER_AUTOTRACE)), out bytesReturned);
            if (result)
            {
                if (bytesReturned >= Marshal.SizeOf(typeof(FKUSBTRACER_AUTOTRACE)))
                {
                    FKUSBTRACER_AUTOTRACE autoTrace = (FKUSBTRACER_AUTOTRACE)Marshal.PtrToStructure(p,typeof(FKUSBTRACER_AUTOTRACE));
                    value = Convert.ToBoolean(autoTrace.AutoTrace);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(Marshal.GetLastWin32Error());
            }
            Marshal.FreeHGlobal(p);
            return result;
        }


        // 获取跟踪数据列表信息
        private List<FKFilterTrace> GetTraceList(IntPtr buffer, uint bufferSize, uint bytesReturned)
        {
            List<FKFilterTrace> filterTraces = new List<FKFilterTrace>();
            int index = 0;
            while (bytesReturned >= index + Marshal.SizeOf(typeof(FKUSBTRACER_FILTER_TRACE)))
            {
                FKUSBTRACER_FILTER_TRACE filterTrace = (FKUSBTRACER_FILTER_TRACE)Marshal.PtrToStructure(new IntPtr(m_OutBuffer.ToInt64() + index),typeof(FKUSBTRACER_FILTER_TRACE));
                index += Marshal.SizeOf(typeof(FKUSBTRACER_FILTER_TRACE));
                if (filterTrace.BufferSize.ToInt32() > 0)
                {
                    if (bytesReturned >= index + filterTrace.BufferSize.ToInt32())
                    {
                        byte[] trace = new byte[filterTrace.BufferSize.ToInt32()];
                        Marshal.Copy(new IntPtr(m_OutBuffer.ToInt64() + index), trace, 0, (int)filterTrace.BufferSize);
                        filterTraces.Add(new FKFilterTrace(filterTrace.DeviceId, filterTrace.Type, filterTrace.Params, filterTrace.Timestamp, trace));
                    }
                }
                else
                {
                    filterTraces.Add(new FKFilterTrace(filterTrace.DeviceId, filterTrace.Type, filterTrace.Params, filterTrace.Timestamp, null));
                }
                index += (int)filterTrace.BufferSize;
            }
            return filterTraces;
        }
        // 新的TraceReader线程
        private void Func_TraceBufReadThread()
        {
            try
            {
                FKDeviceIOCTL devIO = new FKDeviceIOCTL(FKUsbTracerPath, true);

                while (true)
                {
                    uint bytesReturned;
                    // 向驱动发送 跟踪信息 的请求
                    bool result = devIO.DeviceIoControl(FKDeviceIOCTL.IOCTL_FKUSBTRACER_GET_BUFFER, IntPtr.Zero,
                         0, m_OutBuffer, OUT_BUFFER_SIZE, out bytesReturned);
                    if (!result)
                    {
                        int err = Marshal.GetLastWin32Error();
                        // 检查I/O状态
                        if (err == ERROR_IO_PENDING)
                        {
                            while (true)
                            {
                                // 检查 I/O 请求是否被填充( 500ms 后要退出一次，不然线程死锁无法杀掉了 )
                                if (devIO.WaitForOverlappedIo(500, out bytesReturned))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                        else
                            System.Diagnostics.Debug.WriteLine(err);
                    }

                    if (result)
                    {
                        // 收到数据了，我们回调通知
                        if (FilterTraceArrived != null)
                            FilterTraceArrived(this,
                                new FKFilterTraceArrivedEventArgs(GetTraceList(m_OutBuffer, OUT_BUFFER_SIZE, bytesReturned)));
                        // 休眠，让出时间给主线程更新下UI
                        Thread.Sleep(10);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // 在本线程退出前，必须关闭当前pending状态的 IO
                m_DevIOCTL.CancelIo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
