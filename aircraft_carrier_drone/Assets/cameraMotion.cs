using UnityEngine;
using System.Collections;

public class cameraMotion : MonoBehaviour {
	public Vector3[] waypoints = new Vector3[5];
	public float[] waitpoints = new float[5];
	public int maxWaypoints = 5;

	public Vector3[] lookpoints = new Vector3[5];
	public float[] looktimes = new float[5];
	public int maxLookPoints = 5;

	public int nextWaypoint = 1; // waypoint we're moving to next
	public int nextLookAt = 1; // next lookAt
	public float distanceEpsilon = 0.1f;
	public float speed = 0.1f; // for translation
	public float turningSpeed = 0.03f; // in radians. for looking

	public float currPositionWait = -1.0f;
	public float currLookAtWait = -1.0f;
	public float lastFrameTime = 0.0f;

	bool moving = false;

	// Use this for initialization
	void Start () {
		// start the camera at the first waypoint, looking at the first lookAt.
		// if there's no first lookAt, look at (0, 0, 0)
		transform.localPosition = waypoints[0];
		currPositionWait = waitpoints[0];
		currLookAtWait = looktimes[0];
		if (lookpoints.Length > 0) {
			transform.LookAt (lookpoints [0]);
		} else {
			transform.LookAt(new Vector3(0, 0, 0));
		}
		nextWaypoint = 1;
		nextLookAt = 1;
	}

	// fixed update is called once every fixed frame, if you need to accumulate forces or something
	void FixedUpdate() {
		if (Input.GetKey (KeyCode.Space))
			moving = true;
		if (!moving)
			return;

		if (currLookAtWait > 0.0f) { // wait
			currLookAtWait -= (Time.time - lastFrameTime);
			// update the lookAt, in case the camera is moving
			if (!float.IsNaN(lookpoints[nextLookAt - 1].x)) {
				transform.LookAt(lookpoints[nextLookAt - 1]);
			}
		} else { // change lookAt
			// check if we've arrived at the objective lookAt

			// otherwise, compute a new temporary lookAt that satisfies the rotational velocity

		}

		if (currPositionWait > 0.0f && nextWaypoint < waypoints.Length) {
			currPositionWait -= (Time.time - lastFrameTime);
		} else { // move
			// check if we've arrived at the objective position
			Vector3 dist = waypoints[nextWaypoint] - transform.localPosition;
			float distLength = dist.magnitude;
			if (distLength < distanceEpsilon) {
				// update wait time
				currPositionWait = waitpoints[nextWaypoint];
				transform.localPosition = waypoints[nextWaypoint];


				// start looking at the next waypoint
				nextWaypoint++;
			} else {
			// otherwise, compute a new temporary object position and go there
				Vector3 translation = dist.normalized;
				if (distLength < speed) {
					translation *= distLength;
				} else {
					translation *= speed;
				}
				transform.localPosition += translation;
			}
		}

		lastFrameTime = Time.time;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
