﻿namespace NWheels.Processing.Workflows.Impl
{
    public class StateTriggerWorkItem<TState, TTrigger>
    {
        public StateTriggerWorkItem(StateMachineFeedbackEventArgs<TState, TTrigger> eventArgs)
        {
            this.EventArgs = eventArgs;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public StateMachineFeedbackEventArgs<TState, TTrigger> EventArgs { get; private set; }
    }
}
