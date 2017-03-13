using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class RespawnView : BaseView
    {
        [SerializeField]
        private GameObject awaitTransform;
        [SerializeField]
        private GameObject processTransform;
        [SerializeField]
        private UnityEngine.UI.Text respawnTimer;

        public Action OnRespawn;

        public bool Await
        {
            set
            {
                awaitTransform.SetActive(value);
                processTransform.SetActive(!value);
            }
        }

        public void RespawnButton()
        {
            OnRespawn();
        }

        public void RespawnWait(float time)
        {
            respawnTimer.text = time.ToString("0.00");
        }
    }
}