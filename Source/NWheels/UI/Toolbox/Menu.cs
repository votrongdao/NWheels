﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using NWheels.Extensions;
using NWheels.UI.Uidl;

namespace NWheels.UI.Toolbox
{
    [DataContract(Namespace = UidlDocument.DataContractNamespace)]
    public class Menu : WidgetBase<Menu, Empty.Data, Empty.State>
    {
        public Menu(string idName, ControlledUidlNode parent)
            : base(idName, parent)
        {
            this.Items = new List<MenuItem>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void DefineNavigation(object anonymous)
        {
            DefineNavigation(anonymous, Items, level: 0, parent: this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override IEnumerable<WidgetUidlNode> GetNestedWidgets()
        {
            return Items;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public List<MenuItem> Items { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void DescribePresenter(PresenterBuilder<Menu, Empty.Data, Empty.State> presenter)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void DefineNavigation(object anonymous, List<MenuItem> destination, int level, ControlledUidlNode parent)
        {
            foreach ( var property in anonymous.GetType().GetProperties().Where(IsMenuItemProperty) )
            {
                var item = new MenuItem(property.Name, parent);
                
                item.Text = property.Name;
                item.Level = level;

                destination.Add(item);

                if ( property.PropertyType.IsAnonymousType() )
                {
                    DefineNavigation(property.GetValue(anonymous), item.SubItems, level + 1, parent);
                }
                else if ( property.PropertyType == typeof(ItemAction) )
                {
                    item.Action = (ItemAction)property.GetValue(anonymous);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static bool IsMenuItemProperty(PropertyInfo property)
        {
            return (property.DeclaringType != typeof(object));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public delegate void DescribeActionCallback(PresenterBuilder<MenuItem, Empty.Data, Empty.State>.BehaviorBuilder<Empty.Payload> behavior);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static class Action
        {
            public static ItemAction Goto<TInput>(IScreenWithInput<TInput> screen, TInput value = default(TInput))
            {
                return Describe(b => b.Navigate().ToScreen(screen).WithInput((payload, data, state) => value));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction Goto<TInput>(IScreenPartWithInput<TInput> screenPart, ScreenPartContainer targetContainer, TInput value = default(TInput))
            {
                return Describe(b => b.Navigate().FromContainer(targetContainer).ToScreenPart(screenPart).WithInput((payload, data, state) => value));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction AlertUserPopup<TRepo>(Expression<Func<TRepo, UidlUserAlert>> alertCall)
                where TRepo : IUserAlertRepository
            {
                return Describe(b => b.UserAlertFrom<TRepo>().ShowPopup(alertCall));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction AlertUserInline<TRepo>(Expression<Func<TRepo, UidlUserAlert>> alertCall)
                where TRepo : IUserAlertRepository
            {
                return Describe(b => b.UserAlertFrom<TRepo>().ShowInline(alertCall));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction Notify(UidlNotification notification, BroadcastDirection direction = BroadcastDirection.BubbleUp)
            {
                return Describe(b => b.Broadcast(notification).Direction(direction));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction InvokeCommand(UidlCommand command)
            {
                return Describe(b => b.InvokeCommand(command));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction Notify<TPayload>(
                UidlNotification<TPayload> notification, 
                TPayload payload, 
                BroadcastDirection direction = BroadcastDirection.BubbleUp)
            {
                return Describe(b => b.Broadcast(notification).WithPayload((data, state, input) => payload).Direction(direction));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static ItemAction Describe(DescribeActionCallback onDescribe)
            {
                return new ItemAction(onDescribe);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class ItemAction
        {
            public ItemAction(DescribeActionCallback onDescribe)
            {
                this.OnDescribe = onDescribe;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public DescribeActionCallback OnDescribe { get; private set; }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    [DataContract(Namespace = UidlDocument.DataContractNamespace)]
    public class MenuItem : WidgetBase<MenuItem, Empty.Data, Empty.State>
    {
        public MenuItem(string idName, ControlledUidlNode parent)
            : base(idName, parent)
        {
            this.SubItems = new List<MenuItem>();
            this.Selected = new UidlNotification("Selected", this);
            base.Notifications.Add(this.Selected);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override IEnumerable<WidgetUidlNode> GetNestedWidgets()
        {
            return SubItems;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public List<MenuItem> SubItems { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public UidlNotification Selected { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void DescribePresenter(PresenterBuilder<MenuItem, Empty.Data, Empty.State> presenter)
        {
            if ( this.Action != null )
            {
                this.Action.OnDescribe(presenter.On(Selected));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        internal Menu.ItemAction Action { get; set; }
    }
}
