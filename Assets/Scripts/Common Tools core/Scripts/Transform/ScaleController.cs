using UnityEngine;
using System.Collections;

// ScaleController
//
// Subsitute to transform.localScale
// Modify the scale here using ScaleController.scale
// Hard-reset the scale using ScaleController.ResetScale(scale)


public class ScaleController : MonoBehaviour
	{
	public Vector3 scale = Vector3.one;		// Use this for modifying the scale

	public bool unified = true;				// scale.x is used as scale for all components

	public bool damped = true;
	public float damping = 8.0f;

	public bool clamped;
	public Vector3 min = Vector3.one * 0.1f;
	public Vector3 max = Vector3.one * 2.0f;

	public delegate void OnScaleFinished ();
	public OnScaleFinished onScaleFinished;

	Transform m_trans;						// TO-DO: In Unity 5 (from a13) caching transform is no longer necessary
	Vector3 m_currentScale;
	bool m_scaling;


	public void ResetScale (Vector3 newScale)
		{
		scale = newScale;
		m_currentScale = newScale;
		}


	void OnEnable ()
		{
		m_trans = GetComponent<Transform>();

		scale = m_trans.localScale;
		m_currentScale = scale;
		m_scaling = false;
		}


	public void ClampScale (ref Vector3 s)
		{
		if (clamped)
			{
			s.x = Mathf.Clamp(s.x, min.x, max.x);
			s.y = Mathf.Clamp(s.y, min.y, max.y);
			s.z = Mathf.Clamp(s.z, min.z, max.z);
			}
		}


	void LateUpdate ()
		{
		// Clamp the input data

		ClampScale(ref scale);

		// Calculate the final position based on the settings

		Vector3 finalScale = scale;

		if (unified)
			{
			finalScale.y = finalScale.x;
			finalScale.z = finalScale.x;
			}

		// Calculate the localScale for this frame and set it

		if (damped)
			{
			m_currentScale = Vector3.Lerp(m_currentScale, finalScale, damping * Time.deltaTime);
			}
		else
			{
			if (m_currentScale != finalScale) m_scaling = true;
			m_currentScale = finalScale;
			}

		m_trans.localScale = m_currentScale;

		// Trigger the onScaleFinished delegate

		if ((m_currentScale - finalScale).magnitude > 0.001f)
			{
			m_scaling = true;
			}
		else
			{
			if (m_scaling)
				{
				m_scaling = false;
				if (onScaleFinished != null)
					onScaleFinished();
				}
			}
		}
	}
