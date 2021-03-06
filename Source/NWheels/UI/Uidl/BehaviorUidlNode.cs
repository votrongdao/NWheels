﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.UI.Uidl
{
    [DataContract(Namespace = UidlDocument.DataContractNamespace)]
    public abstract class BehaviorUidlNode : AbstractUidlNode
    {
        protected BehaviorUidlNode(string idName, BehaviorType behaviorType, ControlledUidlNode parent)
            : base(UidlNodeType.Behavior, idName, parent)
        {
            this.BehaviorType = behaviorType;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public BehaviorType BehaviorType { get; set; }
        [DataMember]
        public BehaviorUidlNode OnSuccess { get; set; }
        [DataMember]
        public BehaviorUidlNode OnFailure { get; set; }
        [DataMember]
        public SubscriptionToNotification Subscription { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataContract(Namespace = UidlDocument.DataContractNamespace)]
        public class SubscriptionToNotification
        {
            [DataMember]
            public string NotificationQualifiedName { get; set; }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "NavigateBehavior", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlNavigateBehavior : BehaviorUidlNode
    {
        public UidlNavigateBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.Navigate, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public UidlNodeType TargetType { get; set; }
        [DataMember]
        public NavigationType NavigationType { get; set; }
        [DataMember]
        public string TargetQualifiedName { get; set; }
        [DataMember]
        public string TargetContainerQualifiedName { get; set; }
        [DataMember]
        public string InputExpression { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "CallApiBehavior", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlCallApiBehavior : BehaviorUidlNode
    {
        public UidlCallApiBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.CallApi, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public ApiCallType CallType { get; set; }
        [DataMember]
        public string ContractName { get; set; }
        [DataMember]
        public string OperationName { get; set; }
        [DataMember]
        public string[] ParameterNames { get; set; }
        [DataMember]
        public string[] ParameterExpressions { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "InvokeCommand", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlInvokeCommandBehavior : BehaviorUidlNode
    {
        public UidlInvokeCommandBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.InvokeCommand, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public string CommandQualifiedName { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "AlertUser", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlAlertUserBehavior : BehaviorUidlNode
    {
        public UidlAlertUserBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.AlertUser, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public string AlertQualifiedName { get; set; }
        [DataMember]
        public UserAlertDisplayMode DisplayMode { get; set; }
        [DataMember]
        public string[] ParameterExpressions { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "BroadcastBehavior", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlBroadcastBehavior : BehaviorUidlNode
    {
        public UidlBroadcastBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.Broadcast, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public string NotificationQualifiedName { get; set; }
        [DataMember]
        public BroadcastDirection Direction { get; set; }
        [DataMember]
        public string PayloadExpression { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "BranchByRuleBehavior", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlBranchByRuleBehavior : BehaviorUidlNode
    {
        public UidlBranchByRuleBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.BranchByRule, parent)
        {
            this.BranchRules = new List<BranchRule>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public List<BranchRule> BranchRules { get; set; }
        [DataMember]
        public BehaviorUidlNode Otherwise { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataContract(Name = "BranchRule", Namespace = UidlDocument.DataContractNamespace)]
        public class BranchRule
        {
            [DataMember]
            public string ConditionExpression { get; set; }
            [DataMember]
            public BehaviorUidlNode OnTrue { get; set; }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Name = "AlterModelBehavior", Namespace = UidlDocument.DataContractNamespace)]
    public class UidlAlterModelBehavior : BehaviorUidlNode
    {
        public UidlAlterModelBehavior(string idName, ControlledUidlNode parent)
            : base(idName, BehaviorType.AlterModel, parent)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public string ReadExpression { get; set; }
        [DataMember]
        public string WriteExpression { get; set; }
    }
}
