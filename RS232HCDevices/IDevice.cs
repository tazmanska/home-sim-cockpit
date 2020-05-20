using System;
using System.Collections.Generic;
using System.Text;

namespace RS232HCDevices
{
    interface IDevice
    {
        void Write(byte[] data);
        
        IRSReceiver Receiver
        {
            get;
            set;
        }
    }
}
