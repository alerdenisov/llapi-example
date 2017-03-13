using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class ServerConfigView : BaseView
    {
        public Action<int> OnPortChange;
        public Action OnStart;

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