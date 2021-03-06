﻿namespace NWheels.Processing.Workflows.Core
{
    public interface IWorkflowActor
    {
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public interface IWorkflowActor<TWorkItem> : IWorkflowActor
    {
        void Execute(IWorkflowActorContext context, TWorkItem workItem);
    }
}
