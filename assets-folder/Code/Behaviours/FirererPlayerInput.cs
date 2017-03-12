using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        private Vector3 rayPoint;
        private Vector3 shootDirection;
        
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

            Firerer.Move(targetPosition);

            var mouseRay = Camera.UnityCamera.ScreenPointToRay(Input.mousePosition);
            float enter;

            if (new Plane(Vector3.up, Vector3.zero).Raycast(mouseRay, out enter))
            {
                rayPoint = mouseRay.origin + mouseRay.direction * enter;
                shootDirection = (rayPoint - Firerer.transform.position).normalized;
            }

            Firerer.ShootDirection(shootDirection);

            if (Input.GetAxis("Fire1") > 0f)
            {
                Firerer.Shoot();
            }


            cameraPosition = Vector3.Lerp(Firerer.transform.position + camDirection * 5f, Firerer.transform.position + shootDirection * 5f, 0.5f);
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

            var tmpColor = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(Firerer.transform.position, Firerer.transform.position + vector * 5f);


            var cameraVector = Quaternion.Euler(0, Camera.YawAngle, 0f) * vector;
            Handles.color = Color.green;
            Handles.DrawLine(Firerer.transform.position, Firerer.transform.position + cameraVector * 5f);


            Handles.color = new Color(0,1f,0,0.3f);
            var angle = 120f;
            Handles.DrawSolidArc(Firerer.transform.position, Vector3.up, Quaternion.Euler(0, -angle*0.5f, 0) * Firerer.transform.forward, angle, 2f);

            var spread = angle / 180f;
            if (Vector3.Angle(Firerer.transform.forward, shootDirection) > angle / 2f)
                Handles.color = Color.red;
            else
                Handles.color = Color.green;

            Handles.DrawLine(Firerer.transform.position, rayPoint);

            Handles.color = tmpColor;
        }
    }
#endif
}