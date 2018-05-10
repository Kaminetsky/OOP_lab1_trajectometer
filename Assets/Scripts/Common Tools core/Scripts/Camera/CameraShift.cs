
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class CameraShift : MonoBehaviour
	{
	// Offset from viewport's center:
	//		0 = center, default
	//		-1 = left or bottom
	//		+1 = right or top

	public Vector2 offset = Vector2.zero;

	// Cached camera component
	
	Camera m_camera;
	
	
	void OnEnable ()
		{
		m_camera = GetComponent<Camera>();		
		}
		
		
	void OnDisable ()
		{
		m_camera.ResetProjectionMatrix();
		}
	
	
	void LateUpdate()
		{
		SetPerspectiveOffset(m_camera, offset);
		}
	

	static void SetPerspectiveOffset (Camera cam, Vector2 perspectiveOffset)
		{
		float heightRate = Mathf.Tan(cam.fieldOfView*0.5f*Mathf.Deg2Rad);
		float widthRate = heightRate * cam.aspect;
		float viewDist = cam.nearClipPlane;
		
		float halfHeight = viewDist * heightRate;
		float halfWidth = viewDist * widthRate;
		
		float offsetX = perspectiveOffset.x * halfWidth;
		float offsetY = perspectiveOffset.y * halfHeight;
		
		float left   = -halfWidth  - offsetX;
		float right  =  halfWidth  - offsetX;		
		float top    =  halfHeight - offsetY;
		float bottom = -halfHeight - offsetY;

		cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
		}
	

	static Matrix4x4 PerspectiveOffCenter (
			float left, float right,
			float bottom, float top,
			float near, float far)
		{
		var x =	 (2.0f * near)		/ (right - left);
		var y =	 (2.0f * near)		/ (top - bottom);
		var a =	 (right + left)		/ (right - left);
		var b =	 (top + bottom)		/ (top - bottom);
		var c = -(far + near)		/ (far - near);
		var d = -(2.0f * far * near) / (far - near);
		var e = -1.0f;

		Matrix4x4 m = new Matrix4x4();
		m[0,0] =	x;	m[0,1] = 0.0f;	m[0,2] = a;	  m[0,3] = 0.0f;
		m[1,0] = 0.0f;	m[1,1] =	y;	m[1,2] = b;	  m[1,3] = 0.0f;
		m[2,0] = 0.0f;	m[2,1] = 0.0f;	m[2,2] = c;	  m[2,3] =	  d;
		m[3,0] = 0.0f;	m[3,1] = 0.0f;	m[3,2] = e;	  m[3,3] = 0.0f;
		return m;
		}
	}