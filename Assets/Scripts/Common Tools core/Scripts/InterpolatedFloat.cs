
using UnityEngine;


public class InterpolatedFloat
	{
	float m_prevValue;
	float m_value;


	public InterpolatedFloat (float initialValue = 0.0f)
		{
		Reset(initialValue);
		}


	public void Reset (float value)
		{
		m_prevValue = value;
		m_value = value;
		}


	// Call from MonoBehaviour.FixedUpdate

	public void Set (float value)
		{
		m_prevValue = m_value;
		m_value = value;
		}


	// Raw non-interpolated value

	public float Get ()
		{
		return m_value;
		}


	// Returns relative position within current fixed step.
	// Use to feed GetInterpolated when querying multiple interpolated values.

	public static float GetFrameRatio()
		{
		return Mathf.InverseLerp(Time.fixedTime, Time.fixedTime+Time.fixedDeltaTime, Time.time);
		}


	// Call from MonoBehaviour.Update

	public float GetInterpolated()
		{
		float t = GetFrameRatio();
		return Mathf.Lerp(m_prevValue, m_value, t);
		}


	public float GetInterpolated (float t)
		{
		return Mathf.Lerp(m_prevValue, m_value, t);
		}
	}
