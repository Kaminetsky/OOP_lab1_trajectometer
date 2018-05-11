using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour {

	public PathLastPoint lastPoint;
	public Camera camera;

	void Start(){
		camera = Camera.main;
	}

	void Update () {
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize,Mathf.Clamp(Mathf.Pow((lastPoint.speed), 2), 0.8f, 8.0f), Time.deltaTime*3);
	}
}
