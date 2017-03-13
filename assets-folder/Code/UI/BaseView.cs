using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    public abstract class BaseView : MonoBehaviour
    {
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
    }
}