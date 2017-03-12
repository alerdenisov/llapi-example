using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    public class FirererPlayerInput : MonoBehaviour
    {
        public FirererController Firerer;
        public GameplayCamera Camera;

        private float horizontal;
        private float vertical;

        private Vector3 targetPosition;
        private Vector3 cameraPosition;

        // Update is called once per frame
        void Update()
        {
            // Wait for camera and character
            if(!Firerer || !Camera)
            {
                return;
            }

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            var rawDirection = new Vector3(horizontal, 0f, vertical);
            var camRotation = Quaternion.Euler(0, Camera.YawAngle, 0f);
            var camDirection = camRotation * rawDirection;

            targetPosition = Firerer.transform.position + camDirection * 1f;
            cameraPosition = Firerer.transform.position + camDirection * 2f;

            Firerer.Move(targetPosition);
            Camera.Position = Vector3.Lerp(Camera.Position, cameraPosition, Time.deltaTime * 3f);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 30), string.Format("{0}, {1}", horizontal, vertical));
        }

        private void OnDrawGizmos()
        {
            var vector = Vector3.right * horizontal + Vector3.forward * vertical;
            vector.Normalize();

            var tmpColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + vector * 5f);


            var cameraVector = Quaternion.Euler(0, Camera.YawAngle, 0f) * vector;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + cameraVector * 5f);


            Gizmos.color = tmpColor;
        }
    }
#endif
}