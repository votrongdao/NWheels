﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.Logging
{
    [Flags]
    public enum LogContentTypes
    {
        Text = 0x01,
        Exception = 0x02,
        DataEntity = 0x04,
        CommunicationMessage = 0x08,
        PerformanceMeasurement = 0x10
    }
}
