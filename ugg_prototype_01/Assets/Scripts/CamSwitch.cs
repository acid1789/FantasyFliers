using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamSwitch : MonoBehaviour {

	public List<GameObject> cameras;
	public int defaultCam = 0;

	private int activeCamera = 0;

	// Use this for initialization
	void Start () {
		foreach (GameObject cam in cameras) {
			cam.SetActive(false);
		}

		activeCamera = defaultCam;
		cameras [activeCamera].SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonUp ("DroneCamSelect")) 
		{
			cameras [activeCamera].SetActive (false);

			if (++activeCamera >= cameras.Count )
				activeCamera = 0;

			cameras [activeCamera].SetActive (true);
		}	
	}
}
