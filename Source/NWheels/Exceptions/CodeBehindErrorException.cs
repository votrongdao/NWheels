﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWheels.Extensions;

namespace NWheels.Exceptions
{
    [Serializable]
    public class CodeBehindErrorException : Exception
    {
        public CodeBehindErrorException(string message)
            : base(message)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public CodeBehindErrorException(string message, params object[] formatArgs)
            : base(message.FormatIf(formatArgs))
        {
        }
    }
}
