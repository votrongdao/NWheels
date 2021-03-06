﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.UI.Uidl
{
    [DataContract(Namespace = UidlDocument.DataContractNamespace)]
    public abstract class AbstractUidlNode
    {
        /// <summary>
        /// Make IdName equal to current type name
        /// </summary>
        public const string IdNameAsTypeMacro = "${TYPE}";

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected AbstractUidlNode(UidlNodeType nodeType, string idName, AbstractUidlNode parent)
        {
            this.NodeType = nodeType;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.IdName = (idName == IdNameAsTypeMacro ? GetIdNameFromType() : idName);
            this.QualifiedName = (parent != null ? parent.QualifiedName + ":" : "") + IdName;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public UidlNodeType NodeType { get; set; }
        [DataMember]
        public string IdName { get; set; }
        [DataMember]
        public string QualifiedName { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected virtual string GetIdNameFromType()
        {
            return this.GetType().Name;
        }
    }
}
