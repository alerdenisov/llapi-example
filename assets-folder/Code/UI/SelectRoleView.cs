using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class SelectRoleView : BaseView
    {
        public Action OnServerSelected;
        public Action OnClientSelected;

        public void ServerButton()
        {
            OnServerSelected();
        }

        public void ClientButton()
        {
            OnClientSelected();
        }
    }
}