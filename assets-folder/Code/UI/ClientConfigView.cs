using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class ClientConfigView : BaseView
    {
        public Action<int> OnPortChange;
        public Action<string> OnAddressChange;
        public Action OnStart;

        public void AddressChange(string address)
        {
            OnAddressChange(address);
        }

        public void PortChange(string port)
        {
            OnPortChange(int.Parse(port));
        }

        public void StartButton()
        {
            OnStart();
        }
    }
}