using UnityEngine;
using System.Collections;

public class DroneControl : MonoBehaviour {

	public float maxCtrlThrottle = 5000f;
	public float maxCtrlYaw = 450f;
	public float maxCtrlRoll = 10f;
	public float maxCtrlPitch = 20f;

	public float autoBrakeYaw = .1f;
	public float autoCorrectPitch = .1f;
	public float autoCorrectPitchSpeed = .1f;
	public float maxPitch = 10f;

	private Vector3 resetPosition;
	private Quaternion resetRotation;

	private float ctrlThrottle = 0f;
	private float ctrlYaw = 0f;
	private float ctrlRoll = 0f;
	public float ctrlPitch = 0f;

	public float stability = 0.3f;
	public float stabilitySpeed = 2.0f;
	public Vector3 desiredUp = Vector3.up;    
    
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		resetPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z );
		resetRotation = new Quaternion( transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w );
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonUp ("DroneReset")) {
			DroneReset();
		}

		ctrlThrottle = Mathf.Max(0f, Input.GetAxis("DroneThrottle"));
		ctrlYaw = Input.GetAxis("DroneYaw");
		ctrlRoll = Input.GetAxis("DroneRoll");
		ctrlPitch = Input.GetAxis("DronePitch");
	}

	void FixedUpdate() {
		Vector3 eulerRot = transform.eulerAngles;

		//Auto brake yaw rotation
		if (Mathf.Abs (ctrlYaw) < autoBrakeYaw) {
			ctrlYaw = 0;
			rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * .95f, rb.angularVelocity.z);
		}

		//Control Throttle
		rb.AddRelativeForce (Vector3.up * maxCtrlThrottle * ctrlThrottle * Time.fixedDeltaTime);

		//Control Yaw & Roll
		//rb.AddRelativeTorque (Vector3.up * maxCtrlYaw * ctrlYaw * Time.fixedDeltaTime);
		//rb.AddRelativeTorque (-Vector3.forward * maxCtrlRoll * ctrlRoll * Time.fixedDeltaTime);

		//Auto correct pitch
		float pitchRot = eulerRot.x;
		if (pitchRot > 180f) {
			pitchRot = (360f - pitchRot) * -1f;
		}
		if (Mathf.Abs (ctrlPitch) < autoCorrectPitch) {
			//Debug.Log (Time.realtimeSinceStartup);
			ctrlPitch = 0;

			//Test Reset Pitch
			//transform.Rotate(-eulerRot.x, 0, 0);

			float pitchAngle = eulerRot.x;
			if (pitchRot > 0 && rb.angularVelocity.x > 0) {
				rb.AddRelativeForce (Vector3.right * autoCorrectPitchSpeed);
			}
			//rb.angularVelocity = new Vector3(rb.angularVelocity.x * autoCorrectPitchSpeed * pitchAngle, rb.angularVelocity.y, rb.angularVelocity.z);
		}

		//Control Pitch
		if ((pitchRot < maxPitch && ctrlPitch < 0) || (pitchRot > -maxPitch && ctrlPitch > 0) ) {
			rb.AddRelativeTorque (-Vector3.right * maxCtrlPitch * ctrlPitch * Time.fixedDeltaTime);
		} 
		/*else {
			//Slow Pitch
			//Vector3 returnPitch = new Vector3(rb.angularVelocity.x * .95f, rb.angularVelocity.y, rb.angularVelocity.z);
			//rb.angularVelocity = returnPitch;


			Vector3 returnPitch = new Vector3(, 0, 0);

			rb.AddRelativeTorque (returnPitch);
		}
		*/


		/*
		{
			new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * .95f, rb.angularVelocity.z);
			rb.AddRelativeTorque (Vector3.right * maxCtrlPitch * ctrlPitch * Time.fixedDeltaTime);
		}
		*/

		//Stabalizer
		Vector3 predictedUp = Quaternion.AngleAxis( rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilitySpeed, rb.angularVelocity ) * transform.up;
		
		Vector3 torqueVector = Vector3.Cross(predictedUp, desiredUp);
		rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);

    }
    
    private void DroneReset() {
        transform.position = resetPosition;
		transform.rotation = resetRotation;

		rb.velocity = new Vector3 (0, 0, 0);
		rb.angularVelocity = new Vector3 (0, 0, 0);
	}
    
}
