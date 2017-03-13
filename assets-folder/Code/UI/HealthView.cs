using UnityEngine;
using UnityEngine.UI;
using System;
using Zenject;

namespace LlapiExample
{
    public class HealthView : BaseView
    {
        [SerializeField]
        private Slider healthSlider;

        [Inject]
        private GameplayCamera camera;

        public float Amount { set { healthSlider.value = Mathf.Clamp01(value); } }

        public Vector3 WorldPosition
        {
            set
            {
                var screenPoint = camera.UnityCamera.WorldToScreenPoint(value);
                (transform as RectTransform).position = screenPoint;
            }
        }
    }
}