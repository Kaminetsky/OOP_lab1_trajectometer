using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float xSpeed, ySpeed, zSpeed;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		  gameObject.transform.Rotate(Time.deltaTime*xSpeed, Time.deltaTime*ySpeed, Time.deltaTime*zSpeed, Space.World);

	}
}
