﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.Logging
{
    public enum ThreadTaskType
    {
        Unspecified,
        StartUp,
        ShutDown,
        IncomingRequest,
        QueuedWorkItem,
        ScheduledJob,
        LogProcessing
    }
}
