using UnityEngine;
using System.Collections;


// PositionController
//
// Subsitute to transform.localPosition.
// Modify the position here using PositionController.position.
// Hard-reset the position using PositionController.ResetPosition(position)


public class PositionController : MonoBehaviour
	{
	public enum Mode { Free, FixedToTarget };

	public Transform target;					// Target transform for the FixedToTarget mode.
	public Vector3 position;					// Use this for modifying the object position.
	public Mode mode;							// Positioning mode.
	public Vector3 targetOffset;				// Offset to be used in FixedToTarget mode.

	public bool damped = true;
	public float damping = 8.0f;

	public bool clamped;
	public Bounds limits = new Bounds(Vector3.zero, Vector3.one*10.0f);

	public delegate void OnPositionFinished ();
	public OnPositionFinished onPositionFinished;

	Transform m_trans;
	Vector3 m_currentPosition;
	bool m_positioning;


	public void ResetPosition (Vector3 newPosition)
		{
		position = newPosition;
		m_currentPosition = newPosition;
		}


	void OnEnable ()
		{
		m_trans = GetComponent<Transform>();

		if (mode == Mode.Free)
			{
			position = m_trans.localPosition;
			m_currentPosition = position;
			}
		else
			{
			m_currentPosition = m_trans.localPosition;
			}

		m_positioning = false;
		}


	void ClampPosition (ref Vector3 pos)
		{
		if (clamped)
			{
			Vector3 min = limits.min;
			Vector3 max = limits.max;

			pos.x = Mathf.Clamp(pos.x, min.x, max.x);
			pos.y = Mathf.Clamp(pos.y, min.y, max.y);
			pos.z = Mathf.Clamp(pos.z, min.z, max.z);
			}
		}


	void LateUpdate ()
		{
		Transform parent = m_trans.parent;

		// Clamp the input data

		ClampPosition(ref position);

		// Calculate the final position based on the target and the mode

		Vector3 finalPos = position;

		if (target != null)
			{
			if (mode == Mode.FixedToTarget)
				{
				if (parent != null)
					finalPos = parent.InverseTransformPoint(target.position) + targetOffset;
				else
					finalPos = target.position + targetOffset;

				ClampPosition(ref finalPos);
				}
			}

		// Calculate the new position and set it

		m_currentPosition = damped? Vector3.Lerp(m_currentPosition, finalPos, damping * Time.deltaTime) : finalPos;
		m_trans.localPosition = m_currentPosition;

		// Trigger the onPositionFinished delegate

		if ((m_currentPosition - finalPos).magnitude > 0.001f)
			{
			m_positioning = true;
			}
		else
			{
			if (m_positioning)
				{
				m_positioning = false;
				if (onPositionFinished != null)
					onPositionFinished();
				}
			}
		}

	}
