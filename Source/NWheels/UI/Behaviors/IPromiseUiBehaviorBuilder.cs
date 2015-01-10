﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.UI.Behaviors
{
    public interface IPromiseUiBehaviorBuilder<TModel, TState, TPromise> : IBoundUiElementBuilder<TModel, TState>
    {
        IUiBehaviorSelector<TModel, TState, TPromise> Then(
            Action<IUiBehaviorSelector<TModel, TState, TPromise>> onSuccess = null,
            Action<IUiBehaviorSelector<TModel, TState, TPromise>> onError = null,
            bool onErrorContinue = false);
    }
}