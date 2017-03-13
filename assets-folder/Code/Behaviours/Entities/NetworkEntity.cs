using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public abstract class NetworkEntity : MonoBehaviour
    {        
        protected CommanderStatus status {  get { return Commander;  } }
        public CommanderStatus Commander{ get; set; }
        public int CommanderId { get; set; }
        public bool IsMine { get; set; }

#if UNITY_EDITOR
        [SerializeField]
        private string debugId;
#endif

        private Guid id;
        public Guid Id
        {
            get { return id; }
            set
            {
                if (id == default(Guid))
                {
                    id = value;
#if UNITY_EDITOR
                    debugId = value.ToString();
#endif
                }
            }
        }
    }
}