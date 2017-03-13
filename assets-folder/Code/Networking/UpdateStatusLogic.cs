using Zenject;
using UniRx;
using UnityEngine;
using System;

namespace LlapiExample
{
    public class UpdateStatusLogic : INetworkLogic, IClientLogic
    {
        [Inject]
        private StatusView statusView;
        [Inject]
        private ConnectionStatus status;

        public UpdateStatusLogic()
        {
        }

        public void Update()
        {
        }

        public void Setup()
        {
            statusView.Show();
            status.Observable.Subscribe(OnStatusChange);
        }

        private void OnStatusChange(ConnectionState state)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var values = Enum.GetValues(typeof(ConnectionState));
            foreach (var v in values)
            {
                if ((state & (ConnectionState)v) == (ConnectionState)v)
                    sb.Append(((ConnectionState)v).ToString() + " ");
            }

            statusView.Message = sb.ToString();
        }
    }
}