using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    public class StatusView : BaseView
    {
        public UnityEngine.UI.Text StatusText;

        public string Message
        {
            set
            {
                StatusText.text = value;
            }
        }
    }
}