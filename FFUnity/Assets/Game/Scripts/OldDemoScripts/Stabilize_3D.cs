using UnityEngine;
using System.Collections;


namespace OldDemoScripts
{
    public class Stabilize_3D : MonoBehaviour
    {

        public float stability = 0.3f;
        public float speed = 2.0f;
        public Vector3 desiredUp = Vector3.up;

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 predictedUp = Quaternion.AngleAxis(
                GetComponent<Rigidbody>().angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
                GetComponent<Rigidbody>().angularVelocity
                ) * transform.up;

            Vector3 torqueVector = Vector3.Cross(predictedUp, desiredUp);
            GetComponent<Rigidbody>().AddTorque(torqueVector * speed * speed);
        }
    }
}