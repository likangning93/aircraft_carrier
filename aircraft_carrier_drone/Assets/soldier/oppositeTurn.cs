using UnityEngine;
using System.Collections;

public class oppositeTurn : MonoBehaviour {

	public bool left;
	public bool right;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (left)
			transform.eulerAngles = transform.eulerAngles + new Vector3 (0.0f, 1.0f, 0.0f);
		if (right)
			transform.eulerAngles = transform.eulerAngles + new Vector3 (0.0f, -1.0f, 0.0f);
	}
}
