using UnityEngine;
using System.Collections;

public class DroneHover : MonoBehaviour {

	public float force = 0;
	public float forceAdd = 0;
	public float level = 18f;

	public float lift = 5f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Vector3 v = rb.GetRelativePointVelocity (Vector2.zero);

		forceAdd = 0;
		float posDif = level - rb.transform.position.y;
		if (posDif > 0) {
			forceAdd = -v.y * (level - rb.transform.position.y) * Time.fixedDeltaTime * lift;
			if (forceAdd < 0) {
				forceAdd = 0;
			}
			force += forceAdd;
		} else {
			force = 0;
		}

		//Debug.Log ("!" + v);
		rb.AddRelativeForce(Vector3.up * force * Time.fixedDeltaTime);		
	}
}
