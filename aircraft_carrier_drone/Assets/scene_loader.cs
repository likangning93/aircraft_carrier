using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class scene_loader : MonoBehaviour {
	List<Vector3> waypoints = new List<Vector3>();
	List<float> waitTimes = new List<float>();
	string[] separators = {",", ", "};
	string currentPrefab;
	public Transform soldierPrefab;
	public Transform planePrefab;

	// Use this for initialization
	void Start () {
		TextReader tr = new StreamReader("setup.txt");
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
       if (numbers.Length != 2) return new Vector4(0, -1, 0, 0); // aka bad vec3
       float x; // x coordinate
       float z; // z coordinate, b/c unity is y up
	   float t = -1.0f; // wait time at this waypoint.
       if (!float.TryParse(numbers[0], out x)) return new Vector3(0, -1, 0);
       if (!float.TryParse(numbers[1], out z)) return new Vector3(0, -1, 0);
	   if (numbers.Length == 4) {
			if (!float.TryParse(numbers[2], out t)) t = 0;
		}
       return new Vector4(x, 0, z, t);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
