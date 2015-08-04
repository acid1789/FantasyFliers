using UnityEngine;
using System.Collections;

public class DroneFollow : MonoBehaviour {

	public GameObject drone;

	void OnEnable() {
		transform.position = drone.transform.position;
	}

	// Update is called once per frame
	void FixedUpdate () {

		Vector3 posDif = drone.transform.position - transform.position;
		//Debug.Log (posDif);

		float camDistance = posDif.magnitude;

		if (camDistance <= 300f) { 
			//If the distance is small enougth then interpolate the camera
			transform.position += posDif * Time.fixedDeltaTime * camDistance * camDistance;
		} else {
			//Otherwise teleport the camera
			transform.position = drone.transform.position;
		}

		Vector3 droneEuler = drone.transform.eulerAngles;

		transform.rotation = new Quaternion ();
		transform.Rotate(new Vector3(0, droneEuler.y, 0));
	}
}
