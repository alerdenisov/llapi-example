using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    [CreateAssetMenu(fileName = "CameraSetup.asset", menuName = "Camera Setup")]
    public class GameplayCameraSetup : ScriptableObject
    {
        public float PitchAngle = 45f;
        public float Distance = 20f;
        public float YawAngle = 60f;
    }

    public sealed class GameplayCamera : MonoBehaviour
    {
        #region Editor Links
        [SerializeField]
        private Transform YawTransform;
        [SerializeField]
        private Transform DistanceTransform;
        [SerializeField]
        private Transform PitchTransform;
        [SerializeField]
        private GameplayCameraSetup InitialSetup;
        [SerializeField]
        private Camera CameraObject;
        #endregion

        public float PitchAngle
        {
            get
            {
                return PitchTransform.localRotation.eulerAngles.x;
            }
            set
            {
                var current = PitchTransform.localRotation.eulerAngles;
                current.x = value;
                PitchTransform.localRotation = Quaternion.Euler(current);
            }
        }

        public float Distance
        {
            get
            {
                return -DistanceTransform.localPosition.z;
            }
            set
            {
                var current = DistanceTransform.localPosition;
                current.z = -value;
                DistanceTransform.localPosition = current;
            }
        }
        public float YawAngle
        {
            get
            {
                return YawTransform.localRotation.eulerAngles.y;
            }
            set
            {
                var current = YawTransform.localRotation.eulerAngles;
                current.y = value;
                YawTransform.localRotation = Quaternion.Euler(current);
            }
        }
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        private void Start()
        {
            // Check object state
            if (!PitchTransform)
                throw new NullReferenceException("PitchTransform cannot be null! Set it in editor");
            if(!YawTransform)
                throw new NullReferenceException("YawTransform cannot be null! Set it in editor");
            if(!DistanceTransform)
                throw new NullReferenceException("DistanceTransform cannot be null! Set it in editor");
            if(!CameraObject)
                throw new NullReferenceException("CameraObject cannot be null! Set it in editor");
            if (!InitialSetup)
                throw new NullReferenceException("InitialSetup cannot be null! Set it in editor");

            // Set initial values
            PitchAngle = InitialSetup.PitchAngle;
            YawAngle = InitialSetup.YawAngle;
            Distance = InitialSetup.Distance;
        }
    }
}