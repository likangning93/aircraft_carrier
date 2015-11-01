using UnityEngine;
using System.Collections;

public class cameraMotion : MonoBehaviour {
	public Vector3[] waypoints = new Vector3[5];
	public float[] waitpoints = new float[5];
	public int maxWaypoints = 5;

	public Vector3[] lookpoints = new Vector3[5];
	public float[] looktimes = new float[5];
	public int maxLookPoints = 5;

	public int currWaypoint = 1; // waypoint we're moving to next
	public int nextLookAt = 1; // next lookAt
	public float distanceEpsilon = 0.1f;
	public float speed = 0.1f;
	public float turningSpeed = 0.03f;

	public float currPositionWait = -1.0f;
	public float currLookAtWait = -1.0f;
	public float lastFrameTime = 0.0f;
	
	// for turning
	private float realTurningSpeed;
	private int turningSteps = 0;
	private float travelAngle;
	private Vector3 axis;
	private Vector3 goalDirection;

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
		currWaypoint = 1;
		nextLookAt = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
