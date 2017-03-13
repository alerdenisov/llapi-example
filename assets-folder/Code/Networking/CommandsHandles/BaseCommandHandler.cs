using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public abstract class BaseCommandHandler<T> : INetworkLogic where T : ICommand
    {
        private IncomingCommandsQueue incomings;

        public BaseCommandHandler(IncomingCommandsQueue incomings)
        {
            this.incomings = incomings;
        }

        public virtual void Setup()
        {
            incomings.Observable.Subscribe(Handle);
        }

        private void Handle(ICommand command)
        {
            if(command is T)
            {
                OnCommand((T)command);
            }
        }

        public virtual void Update()
        {
        }

        protected abstract void OnCommand(T command);
    }
}
