using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLastPoint : MonoBehaviour {

	public PathRenderer pathRenderer;
	public float speed = 0.9f;
	private int lastPathPoint = -1;

	void Update () {
		if(speed > 0.1f) lastPathPoint = pathRenderer.AddPathPoint(transform.position, speed,lastPathPoint);
		else lastPathPoint = -1;
	}

}
