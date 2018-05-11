using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLastPoint : MonoBehaviour {

	public PathRenderer pathRenderer;
	public float speed = 0f, qFade = 1000f;
	private int lastPathPoint = -1;

	Vector3 lastPos;

	private AccelerometerUtil accelerometerUtil;

	// Use this for initialization
	void Start(){
		accelerometerUtil = new AccelerometerUtil();
		lastPos = Vector3.zero;
	}

	void FixedUpdate () {

		speed = Vector3.Distance(lastPos, transform.position)*4;
		lastPos = transform.position;

		//Debug.Log("Speed is " + speed);

		if(speed > 0.001f) lastPathPoint = pathRenderer.AddPathPoint(transform.position, speed*10,lastPathPoint);
		else lastPathPoint = -1;

		Vector3 currentInput = accelerometerUtil.LowPassFiltered();

		transform.Translate(currentInput.x/qFade, 0f, currentInput.z/qFade);

	}

}
