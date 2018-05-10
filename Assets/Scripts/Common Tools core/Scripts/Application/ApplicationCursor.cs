using UnityEngine;
using System.Collections;

public class ApplicationCursor : MonoBehaviour
	{
	public bool showCursor = true;
	public bool lockCursor = false;


	void OnEnable ()
		{
		if (!Application.isEditor)
			{
			#if UNITY_5
			Cursor.visible = showCursor;
			Cursor.lockState = lockCursor? CursorLockMode.Locked : CursorLockMode.None;
			#else
			Cursor.visible = showCursor;
			Screen.lockCursor = lockCursor;
			#endif
			}
		}
	}
