﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Hapil;
using NWheels.Extensions;

namespace NWheels.Logging
{
    [DataContract(Namespace = "NWheels.Logging", Name = "Activity")]
    public class FormattedActivityLogNode : ActivityLogNode
    {
        private readonly string _singleLineText;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public FormattedActivityLogNode(string singleLineText)
            : this(FormattedLogNode.AdHocFormattedMessageId, singleLineText)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public FormattedActivityLogNode(string messageId, string singleLineText)
            : base(messageId)
        {
            _singleLineText = singleLineText;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override string FormatSingleLineText()
        {
            return _singleLineText;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override string FormatFullDetailsText()
        {
            if ( base.Exception != null )
            {
                return base.Exception.ToString();
            }
            else
            {
                return null;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override IEnumerable<ILogNameValuePair> ListNameValuePairs()
        {
            return base.ListNameValuePairs()
                .ConcatIf(true, new LogNameValuePair<string> { Name = "text", Value = _singleLineText })
                .ConcatIf(base.Exception != null, () => new LogNameValuePair<string> { Name = "exception", Value = base.Exception.ToString(), IsDetail = true });
        }
    }
}
