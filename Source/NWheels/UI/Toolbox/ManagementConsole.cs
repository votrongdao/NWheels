﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NWheels.UI.Uidl;

namespace NWheels.UI.Toolbox
{
    [DataContract(Namespace = UidlDocument.DataContractNamespace)]
    public class ManagementConsole : WidgetUidlNode, UidlBuilder.IBuildableUidlNode
    {
        public ManagementConsole(string idName, ControlledUidlNode parent)
            : base(idName, parent)
        {
            this.MainMenu = new Menu("MainMenu", this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ManagementConsole Navigation(object anonymous)
        {
            MainMenu.DefineNavigation(anonymous);
            return this;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ManagementConsole Dashboard(UidlScreenPart dashboardScreenPart)
        {
            DashboardScreenPartQualifiedName = dashboardScreenPart.QualifiedName;
            MainContent.InitalScreenPartQualifiedName = dashboardScreenPart.QualifiedName;
            return this;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ManagementConsole StatusBarWidgets(params WidgetUidlNode[] widgets)
        {
            StatusBar.ContainedWidgets.AddRange(widgets);
            return this;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public Menu MainMenu { get; set; }
        [DataMember]
        public Container StatusBar { get; set; }
        [DataMember]
        public ScreenPartContainer MainContent { get; set; }
        [DataMember]
        public string DashboardScreenPartQualifiedName { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override IEnumerable<WidgetUidlNode> GetNestedWidgets()
        {
            return new WidgetUidlNode[] { MainMenu, MainContent, StatusBar };
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region IBuildableUidlNode Members

        void UidlBuilder.IBuildableUidlNode.Build(UidlBuilder builder)
        {
            builder.BuildNodes(this.GetNestedWidgets().Cast<AbstractUidlNode>().ToArray());
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void UidlBuilder.IBuildableUidlNode.DescribePresenter(UidlBuilder builder)
        {
            builder.DescribeNodePresenters(this.GetNestedWidgets().Cast<AbstractUidlNode>().ToArray());
        }

        #endregion
    }
}
