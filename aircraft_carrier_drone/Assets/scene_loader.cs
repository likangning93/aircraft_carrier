using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class scene_loader : MonoBehaviour {
	TextReader tr;

	List<Vector3> waypoints = new List<Vector3>();
	List<float> waitTimes = new List<float>();
	string[] separators = {",", ", ", "point:", "look:"};
	string currentPrefab;
	public Transform soldierPrefab;
	public Transform planePrefab;
	public Transform cameraPrefab;

	// Use this for initialization
	void Start () {
		tr = new StreamReader("setup.txt");
		string line = tr.ReadLine();
		while (line != null) {
			handleText(line);
			line = tr.ReadLine();
		}
		tr.Close();
		if (waypoints.Count > 0 ) {
			if (currentPrefab == "soldier") addSoldierPrefab();
			if (currentPrefab == "plane") addPlanePrefab();
			waypoints.Clear();
		}
	}

	void handleText(string line) {
		line = line.Trim();
		Vector4 vec4Try = parseVec4(line);
		// case of waypoint coordinate
		if (vec4Try[1] != -1) {
			Debug.Log(vec4Try[0] + " " + vec4Try[2] + " " + vec4Try[3]);
			Vector3 position = new Vector3(vec4Try.x, vec4Try.y, vec4Try.z);
			waypoints.Insert(waypoints.Count, position);
			waitTimes.Insert(waitTimes.Count, vec4Try[3]);
		}
		// case of "soldier" or "plane"
		if (line == "soldier" || line == "plane") {
			// if we have waypoints loaded, dump them into a new prefab.
			// otherwise, don't do anything.
			if (waypoints.Count > 0 ) {
				if (currentPrefab == "soldier") addSoldierPrefab();
				if (currentPrefab == "plane") addPlanePrefab();
				waypoints.Clear();
				waitTimes.Clear();
			}
			currentPrefab = line;
		}
		// handle case of a camera separately
		if (line == "camera") {
			currentPrefab = line;
			loadCamera();
		}
	}

	void addSoldierPrefab() {
		Transform soldierClone = Instantiate(soldierPrefab);
		int numWaypoints = waypoints.Count;
		Vector3[] waypointArray = new Vector3[numWaypoints];
		waypoints.CopyTo(waypointArray);
		float[] waitpointArray = new float[numWaypoints];
		waitTimes.CopyTo(waitpointArray);
		soldierClone.GetComponent<soldier_motion>().waypoints = waypointArray;
		soldierClone.GetComponent<soldier_motion>().waitpoints = waitpointArray;
		soldierClone.GetComponent<soldier_motion>().maxWayPoints = numWaypoints;
	}

	void addPlanePrefab() {
		Transform planeClone = Instantiate(planePrefab);
		int numWaypoints = waypoints.Count;
		Vector3[] waypointArray = new Vector3[numWaypoints];
		waypoints.CopyTo(waypointArray);
		float[] waitpointArray = new float[numWaypoints];
		waitTimes.CopyTo(waitpointArray);
		planeClone.GetComponent<jetMotion>().waypoints = waypointArray;
		planeClone.GetComponent<jetMotion>().waitpoints = waitpointArray;
		planeClone.GetComponent<jetMotion>().maxWayPoints = numWaypoints;	
	}

	Vector4 parseVec4(string line) {
       string[] numbers = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
       if (numbers.Length < 2 || line.Contains("//")) {
			return new Vector4 (0, -1, 0, -1); // aka bad vec3
		}
       float x; // x coordinate
       float z; // z coordinate, b/c unity is y up
	   float t = -1.0f; // wait time at this waypoint.
       if (!float.TryParse(numbers[0], out x)) return new Vector4(0, -1, 0, -1);
       if (!float.TryParse(numbers[1], out z)) return new Vector4(0, -1, 0, -1);
	   if (numbers.Length == 3) {
			if (!float.TryParse(numbers[2], out t)) t = -1;
		}
       return new Vector4(x, 0, z, t);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void loadCamera() {
		String line = tr.ReadLine();
		List<Vector3> camwaypoints = new List<Vector3>();
		List<float> camwaittimes = new List<float>();
		List<Vector3> lookpoints = new List<Vector3>();
		List<float> looktimes = new List<float>();

		while (line != null) {
			if (line == "soldier" || line == "jet") {
				currentPrefab = line;
				break;
			}
			Vector4 lineData = handleCameraLine(line);
			if (line.Contains("point")) {
				camwaypoints.Insert(camwaypoints.Count, new Vector3(lineData.x, lineData.y, lineData.z));
				camwaittimes.Insert(camwaittimes.Count, lineData.w);
			}
			else if (line.Contains ("look") && !line.Contains("//")) {
				if (line.Contains("x")) {
					float nan = float.NaN;
					lookpoints.Insert(lookpoints.Count, new Vector3(nan, nan, nan));
					looktimes.Insert(looktimes.Count, lineData.w);
				} else { // normal lookAt
					lookpoints.Insert(lookpoints.Count, new Vector3(lineData.x, lineData.y, lineData.z));
					looktimes.Insert(looktimes.Count, lineData.w);
				}
			}
			line = tr.ReadLine();
		}

		// we're done loading? instantiate and add the camera
		Transform cameraClone = Instantiate(cameraPrefab);
		int numWaypoints = camwaypoints.Count;
		Vector3[] waypointArray = new Vector3[numWaypoints];
		camwaypoints.CopyTo(waypointArray);
		float[] waitpointArray = new float[numWaypoints];
		camwaittimes.CopyTo(waitpointArray);

		cameraClone.GetComponent<cameraMotion>().waypoints = waypointArray;
		cameraClone.GetComponent<cameraMotion>().waitpoints = waitpointArray;
		cameraClone.GetComponent<cameraMotion>().maxWaypoints = numWaypoints;

		int numLookPoints = lookpoints.Count;
		Vector3[] lookpointArray = new Vector3[numLookPoints];
		lookpoints.CopyTo(lookpointArray);
		float[] looktimeArray = new float[numLookPoints];
		looktimes.CopyTo(looktimeArray);

		cameraClone.GetComponent<cameraMotion>().lookpoints = lookpointArray;
		cameraClone.GetComponent<cameraMotion>().looktimes = looktimeArray;
		cameraClone.GetComponent<cameraMotion>().maxLookPoints = numLookPoints;
	}


	Vector4 handleCameraLine(String line) {
		string[] numbers = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		if (numbers.Length < 3 || line.Contains("//")) {
			return new Vector4 (0, -1, 0, -1); // aka bad vec3
		}
		// we added a specifier in front, so numbers is of length 5 in the normal case.
		float x; // x coordinate
		float y; // y coordinate
		float z; // z coordinate, b/c unity is y up
		float t = -1.0f; // wait time at this waypoint.
		if (numbers.Length == 5) {
			if (!float.TryParse(numbers[4], out t)) t = -1.0f;
		}
		if (!float.TryParse(numbers[1], out x))
			return new Vector4 (0, -1, 0, t);
		if (!float.TryParse(numbers[2], out y)) 
			return new Vector4 (0, -1, 0, t);
		if (!float.TryParse(numbers[3], out z)) 
			return new Vector4 (0, -1, 0, t);
		return new Vector4(x, y, z, t);
	}
}
