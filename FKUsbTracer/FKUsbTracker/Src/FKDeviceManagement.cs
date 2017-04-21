using System;
using System.Runtime.InteropServices;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    sealed internal partial class FKDeviceManagement
    {
        internal Boolean IsDeviceChild(string parentInstanceId, string childInstanceId)
        {
            IntPtr ptrParentDevNode, ptrChildDevNode;
            if (CM_Locate_DevNode(out ptrParentDevNode, parentInstanceId, 0) == 0)
            {
                if (CM_Locate_DevNode(out ptrChildDevNode, childInstanceId, 0) == 0)
                {
                    IntPtr ptrChildCompare;
                    if (CM_Get_Child(out ptrChildCompare, ptrParentDevNode.ToInt32(), 0) == 0)
                    {
                        while (ptrChildCompare.ToInt32() != ptrChildDevNode.ToInt32())
                        {
                            if (!(CM_Get_Sibling(out ptrChildCompare, ptrChildCompare.ToInt32(), 0) == 0))
                                break;
                        }
                        return ptrChildCompare.ToInt32() == ptrChildDevNode.ToInt32();
                    }
                }
            }
            return false;
        }

        Boolean GetRegProp(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfo, uint property, out string propertyVal)
        {
            uint RequiredSize = 0;
            uint RegType;
            propertyVal = null;
            SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devInfo, property, out RegType, IntPtr.Zero, 0, out RequiredSize);
            if (RequiredSize > 0)
            {
                IntPtr ptrBuf = Marshal.AllocHGlobal((int)RequiredSize);
                if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devInfo, property, out RegType, ptrBuf, RequiredSize, out RequiredSize))
                    propertyVal = Marshal.PtrToStringAuto(ptrBuf);
                Marshal.FreeHGlobal(ptrBuf);
            }
            return propertyVal != null;
        }

        Boolean GetInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfo, out string instanceId)
        {
            uint RequiredSize = 256;
            instanceId = null;
            IntPtr ptrBuf = Marshal.AllocHGlobal((int)RequiredSize);
            if (SetupDiGetDeviceInstanceId(deviceInfoSet, ref devInfo, ptrBuf, RequiredSize, out RequiredSize))
                instanceId = Marshal.PtrToStringAuto(ptrBuf);
            else if (RequiredSize > 0)
            {
                Marshal.ReAllocHGlobal(ptrBuf, new IntPtr(RequiredSize));
                if (SetupDiGetDeviceInstanceId(deviceInfoSet, ref devInfo, ptrBuf, RequiredSize, out RequiredSize))
                    instanceId = Marshal.PtrToStringAuto(ptrBuf);
            }
            Marshal.FreeHGlobal(ptrBuf);
            return instanceId != null;
        }

        internal Boolean FindDeviceProps(string physicalDeviceObjectName, out string hardwareId, out string description, out string instanceId)
        {
            IntPtr detailDataBuffer = IntPtr.Zero;
            Boolean deviceFound;
            IntPtr deviceInfoSet = new System.IntPtr();
            Boolean lastDevice = false;
            UInt32 memberIndex = 0;
            Boolean success;

            hardwareId = null;
            description = null;
            instanceId = null;

            try
            {
                deviceInfoSet = SetupDiGetClassDevs(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);

                deviceFound = false;
                memberIndex = 0;

                SP_DEVINFO_DATA devInfo = new SP_DEVINFO_DATA();
                devInfo.cbSize = Marshal.SizeOf(devInfo);

                do
                {
                    success = SetupDiEnumDeviceInfo
                        (deviceInfoSet,
                        memberIndex,
                        ref devInfo);

                    if (!success)
                    {
                        lastDevice = true;
                    }
                    else
                    {
                        string pdoName;
                        if (GetRegProp(deviceInfoSet, ref devInfo, (uint)SPDRP.SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, out pdoName))
                        {
                            if (physicalDeviceObjectName == pdoName)
                            {
                                GetRegProp(deviceInfoSet, ref devInfo, (uint)SPDRP.SPDRP_HARDWAREID, out hardwareId);
                                GetRegProp(deviceInfoSet, ref devInfo, (uint)SPDRP.SPDRP_DEVICEDESC, out description);
                                GetInstanceId(deviceInfoSet, ref devInfo, out instanceId);
                                deviceFound = true;
                                break;
                            }
                        }
                    }
                    memberIndex++;
                }
                while (!((lastDevice == true)));

                return deviceFound;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (detailDataBuffer != IntPtr.Zero)
                {
                    // 释放内存
                    Marshal.FreeHGlobal(detailDataBuffer);
                }

                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        // 注册USB设备 增/删 处理消息
        internal Boolean RegisterForDeviceNotifications(IntPtr formHandle, ref IntPtr deviceNotificationHandle)
        {
            DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
            IntPtr devBroadcastDeviceInterfaceBuffer = IntPtr.Zero;
            Int32 size = 0;

            try
            {
                size = Marshal.SizeOf(devBroadcastDeviceInterface);
                devBroadcastDeviceInterface.dbcc_size = size;
                devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
                devBroadcastDeviceInterface.dbcc_reserved = 0;
                //devBroadcastDeviceInterface.dbcc_classguid = classGuid;
                devBroadcastDeviceInterface.dbcc_classguid = Guid.Empty;
                devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);

                Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);
                deviceNotificationHandle = RegisterDeviceNotification(formHandle, devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);

                Marshal.PtrToStructure(devBroadcastDeviceInterfaceBuffer, devBroadcastDeviceInterface);

                if ((deviceNotificationHandle.ToInt64() == IntPtr.Zero.ToInt64()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (devBroadcastDeviceInterfaceBuffer != IntPtr.Zero)
                {
                    // 释放内存
                    Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer);
                }
            }
        }


        internal void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
        {
            try
            {
                FKDeviceManagement.UnregisterDeviceNotification(deviceNotificationHandle);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
