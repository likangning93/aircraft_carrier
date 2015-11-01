using UnityEngine;
using System.Collections;

public class jetMotion : MonoBehaviour {
	public Vector3[] waypoints = new Vector3[5];
	public float[] waitpoints = new float[5];

	public int maxWayPoints = 5;
	public int currWaypoint = 1; // waypoint we're aiming for ATM. these... aren't zero indexed and I'm so ok with that
	public bool moving = false;
	public float distanceEpsilon = 0.1f;
	public float speed = 0.1f;
	public float turningSpeed = 0.03f;
	public float currWait = -1.0f;
	public float lastFrameTime = 0.0f;

	// for turning
	private float realTurningSpeed;
	private int turningSteps = 0;
	private float travelAngle;
	private Vector3 axis;
	private Vector3 goalDirection;

	// Use this for initialization
	void Start () {
		// start the jet at the first waypoint, facing towards the second
		transform.localPosition = waypoints[0];
		transform.LookAt (waypoints [1]);
		currWaypoint = 1;
		currWait = waitpoints [0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// fixed update is called once every fixed frame, if you need to accumulate forces or something
	void FixedUpdate() {
		if (Input.GetKey (KeyCode.Space))
			moving = true;
		if (currWaypoint == maxWayPoints) return; // don't do anything at the last waypoint

		if (moving) {

			// don't do anything while waiting
			if (currWait >= 0.0f) {
				currWait -= (Time.time - lastFrameTime);
				lastFrameTime = Time.time;
				return;
			}

			Vector3 dist = waypoints[currWaypoint] - transform.localPosition;
			float distLength = dist.magnitude;

			// turning
			if (turningSteps > 0)
			{
				if (turningSteps == 1)
				{
					// necessary b/c the jitter. THE JITTER!!!
					transform.LookAt(transform.localPosition + goalDirection);
					turningSteps = 0;
					return;
				}
				// use a cross product and axis-angle rotation to correct towards goal. sniffle.
				transform.RotateAround(transform.localPosition, axis, realTurningSpeed);
				turningSteps--;
				return;
			}

			// check if we've reached a waypoint
			if (distLength <= distanceEpsilon)// && currWaypoint < maxWayPoints)
			{
				// update current wait time
				currWait = waitpoints[currWaypoint];

				currWaypoint++;
				if (currWaypoint > maxWayPoints - 1) return;
				// turnnnnnn
				// compute direction to next waypoint
				goalDirection = waypoints[currWaypoint] - waypoints[currWaypoint - 1];
				goalDirection = goalDirection.normalized;
				// slightly jitter forward direction to avoid singularities
				transform.forward += new Vector3((0.5f - Random.value) / 10.0f, 0.0f, (0.5f - Random.value) / 10.0f);
				float dotProduct = Vector3.Dot(transform.forward, goalDirection);
				travelAngle = Mathf.Acos(dotProduct);
				turningSteps = (int) (travelAngle / turningSpeed);
				realTurningSpeed = travelAngle / (float) turningSteps;
				realTurningSpeed = Mathf.Rad2Deg * realTurningSpeed; // because of something with rotatearound. booo hisisss
				axis = Vector3.Cross(transform.forward, goalDirection);
			}
			
			// otherwise move closer
			else{
				Vector3 dir = dist.normalized;
				Vector3 movement = dir * speed;
				transform.localPosition = transform.localPosition + movement;
				transform.LookAt(transform.localPosition + dir);
			}
		}
		lastFrameTime = Time.time;

	}
}
