﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.Logging
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class LogAttributeBase : Attribute
    {
        protected LogAttributeBase(LogLevel level, bool isActivity, bool isThread, bool isMethodCall)
        {
            this.Level = level;
            this.IsActivity = isActivity;
            this.IsThread = isThread;
            this.IsMethodCall = isMethodCall;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool MustFormatEarly { get; set; }
        public LogLevel Level { get; private set; }
        public bool IsActivity { get; private set; }
        public bool IsThread { get; private set; }
        public bool IsMethodCall { get; private set; }
        public ThreadTaskType TaskType { get; protected set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogDebugAttribute : LogAttributeBase
    {
        public LogDebugAttribute()
            : base(LogLevel.Debug, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogVerboseAttribute : LogAttributeBase
    {
        public LogVerboseAttribute()
            : base(LogLevel.Verbose, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogInfoAttribute : LogAttributeBase
    {
        public LogInfoAttribute()
            : base(LogLevel.Info, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogWarningAttribute : LogAttributeBase
    {
        public LogWarningAttribute()
            : base(LogLevel.Warning, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogErrorAttribute : LogAttributeBase
    {
        public LogErrorAttribute()
            : base(LogLevel.Error, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogCriticalAttribute : LogAttributeBase
    {
        public LogCriticalAttribute()
            : base(LogLevel.Critical, isActivity: false, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogActivityAttribute : LogAttributeBase
    {
        public LogActivityAttribute()
            : base(LogLevel.Info, isActivity: true, isThread: false, isMethodCall: false)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogMethodAttribute : LogAttributeBase
    {
        public LogMethodAttribute()
            : base(LogLevel.Info, isActivity: true, isThread: false, isMethodCall: true)
        {
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class LogThreadAttribute : LogAttributeBase
    {
        public LogThreadAttribute(ThreadTaskType taskType)
            : base(LogLevel.Info, isActivity: true, isThread: true, isMethodCall: false)
        {
            base.TaskType = taskType;
        }
    }
}
