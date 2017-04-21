using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    public class FKDeviceID
    {
        public uint DevId;
        public bool Enabled;
        public string PhysicalDeviceObjectName;
        public string HardwareId;
        public string Description;
        public string InstanceId;

        public FKDeviceID(uint devId, bool enabled, string pdoName)
        {
            DevId = devId;
            Enabled = enabled;
            PhysicalDeviceObjectName = pdoName;
            HardwareId = null;
            Description = null;
            InstanceId = null;
        }

        public override string ToString()
        {
            if (Description != null)
                return string.Format("{0:D2}: {1}", DevId, Description);
            if (HardwareId != null)
                return string.Format("{0:D2}: {1}", DevId, HardwareId);
            return string.Format("{0:D2}: {1}", DevId, PhysicalDeviceObjectName);
        }
    }
}
