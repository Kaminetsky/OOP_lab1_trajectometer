using UnityEngine;
using System.Collections;

public class CameraFixTo : MonoBehaviour
	{
	public Transform target;	

	void LateUpdate ()
		{
		if (target)
			{
			transform.position = target.position;
			transform.rotation = target.rotation;
			}
		}
	}
