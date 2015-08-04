using UnityEngine;
using System.Collections;

public class BladeProp : MonoBehaviour {

	public float propSpeed = 1.0f;

	private float maxPropSpeed = 3000f;
	private Transform prop;

	// Use this for initialization
	void Start () {
		prop = transform.GetChild (0).GetChild (0).transform;
	}
	
	// Update is called once per frame
	void Update () {
		prop.Rotate (0, 0, propSpeed * maxPropSpeed * Time.deltaTime);	
	}
}
