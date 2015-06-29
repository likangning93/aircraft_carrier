using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class scene_loader : MonoBehaviour {
	List<Vector3> waypoints = new List<Vector3>();
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
		Vector3 vec3Try = parseVec3(line);
		// case of waypoint coordinate
		if (vec3Try[1] != -1) {
			Debug.Log(vec3Try[0] + " " + vec3Try[2]);
			waypoints.Insert(waypoints.Count, vec3Try);
		}
		// case of "soldier" or "plane"
		if (line == "soldier" || line == "plane") {
			// if we have waypoints loaded, dump them into a new prefab.
			// otherwise, don't do anything.
			if (waypoints.Count > 0 ) {
				if (currentPrefab == "soldier") addSoldierPrefab();
				if (currentPrefab == "plane") addPlanePrefab();
				waypoints.Clear();
			}
			currentPrefab = line;
		}
	}

	void addSoldierPrefab() {
		Transform soldierClone = Instantiate(soldierPrefab);
		int numWaypoints = waypoints.Count;
		Vector3[] waypointArray = new Vector3[numWaypoints];
		waypoints.CopyTo(waypointArray);
		soldierClone.GetComponent<soldier_motion>().waypoints = waypointArray;
		soldierClone.GetComponent<soldier_motion>().maxWayPoints = numWaypoints;
	}

	void addPlanePrefab() {
		Transform planeClone = Instantiate(planePrefab);
		int numWaypoints = waypoints.Count;
		Vector3[] waypointArray = new Vector3[numWaypoints];
		waypoints.CopyTo(waypointArray);
		planeClone.GetComponent<jetMotion>().waypoints = waypointArray;
		planeClone.GetComponent<jetMotion>().maxWayPoints = numWaypoints;	
	}

	Vector3 parseVec3(string line) {
       string[] numbers = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
       if (numbers.Length != 2) return new Vector3(0, -1, 0); // aka bad vec3
       float x;
       float z;
       if (!float.TryParse(numbers[0], out x)) return new Vector3(0, -1, 0);
       if (!float.TryParse(numbers[1], out z)) return new Vector3(0, -1, 0);
       return new Vector3(x, 0, z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
