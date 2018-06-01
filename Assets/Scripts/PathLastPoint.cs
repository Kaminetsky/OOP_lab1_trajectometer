using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLastPoint : MonoBehaviour {

	public PathRenderer pathRenderer;
	public float speed = 0f, qFade = 1000f;
	private int lastPathPoint = -1;

	Vector3 lastPos = Vector3.zero;
	Vector3 currentInput = Vector3.zero;

	void Start(){
		Input.gyro.enabled = true;
	}

	void FixedUpdate () {

		speed = Vector3.Distance(lastPos, transform.position)*4f/Time.deltaTime;
		lastPos = transform.position;

		Debug.Log(Input.gyro.userAcceleration.normalized);

		if(speed > 0.001f) lastPathPoint = pathRenderer.AddPathPoint(transform.position, speed*10,lastPathPoint);
		else lastPathPoint = -1;

		gameObject.GetComponent<Rigidbody>().AddRelativeForce(
			Input.gyro.userAcceleration.x * 1000f,
			0f,
			Input.gyro.userAcceleration.z * 1000f
		);
	}
}
