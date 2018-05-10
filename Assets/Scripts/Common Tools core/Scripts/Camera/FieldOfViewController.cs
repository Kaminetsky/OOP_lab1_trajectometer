using UnityEngine;
using System.Collections;


// FieldOfViewController
//
// Substitute to Camera.fieldOfView.
// Modify the FoV here using FieldOfViewController.fieldOfView.
// Hard-reset the FoV using FieldOfViewController.ResetFieldOfView(angle)
//
//	AdjustToTarget
//		Mathes the target's bounding box radius. TargetSizeOffset is applied as adjustment.
//	AdjustSizeToTargetDistance
//		Matches the given TargetSize+TargetSizeOffset radius at the distance of the target.
//
// Can be clamped independently for fov angle and target size. If both are enforced then
// fov clamp has precedende.


[RequireComponent(typeof(Camera))]
public class FieldOfViewController : MonoBehaviour
	{
	public enum Mode { Free, AdjustToTarget, AdjustSizeToTargetDistance };

	public Transform target;
	public float fieldOfView = 60.0f;
	public Mode mode;
	public float targetSize = 1.0f;
	public float targetSizeOffset = 0.0f;

	public bool damped = true;
	public float damping = 4.0f;

	public bool clampedFov;
	public float minFov = 10.0f;
	public float maxFov = 75.0f;
	public bool clampedSize;
	public float minSize = 0.1f;
	public float maxSize = 2.0f;

	Camera m_cam;
	Transform m_trans;
	float m_currentFov;
	bool m_firstRun;

	Transform m_cachedTarget;
	Renderer m_targetRenderer;


	public void ResetFieldOfView (float fovAngle)
		{
		fieldOfView = fovAngle;
		m_currentFov = fovAngle;
		}


	void OnEnable ()
		{
		m_cam = GetComponent<Camera>();
		m_trans = GetComponent<Transform>();

		if (mode == Mode.Free)
			{
			fieldOfView = m_cam.fieldOfView;
			m_currentFov = fieldOfView;
			}

		m_firstRun = true;
		}


	float GetFovAngleBySize (float size, float distance)
		{
		if (clampedSize)
			size = Mathf.Clamp(size, minSize, maxSize);

		float angle = Mathf.Atan2(size, distance);
		return angle * 2.0f * Mathf.Rad2Deg;

		}


	void LateUpdate ()
		{
		// Cache the rendered of the target

		if (target != m_cachedTarget)
			{
			m_targetRenderer = target != null? target.GetComponent<Renderer>() : null;
			m_cachedTarget = target;
			}

		// Clamp the input data

		if (clampedFov)
			fieldOfView = Mathf.Clamp(fieldOfView, minFov, maxFov);

		if (clampedSize)
			targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

		// Calculate the FoV to apply based on the target and the mode

		float finalFov = fieldOfView;

		if (target != null && mode != Mode.Free)
			{
			if (mode == Mode.AdjustToTarget)
				{
				if (m_targetRenderer != null)
					{
					float size  = m_targetRenderer.bounds.extents.magnitude + targetSizeOffset;
					float dist = (m_targetRenderer.bounds.center - m_trans.position).magnitude;

					finalFov = GetFovAngleBySize(size, dist);
					}
				}
			else
			if (mode == Mode.AdjustSizeToTargetDistance)
				{
				float size = targetSize + targetSizeOffset;
				float dist = (target.position - m_trans.position).magnitude;

				finalFov = GetFovAngleBySize(size, dist);
				}

			// Clamp the modified FOV

			if (clampedFov)
				finalFov = Mathf.Clamp(finalFov, minFov, maxFov);

			// Bypass damping at the first run

			if (m_firstRun)
				{
				m_currentFov = finalFov;
				m_firstRun = false;
				}
			}

		// Final "hardcore" clamp: prevent undesired fov artifacts

		finalFov = Mathf.Clamp(finalFov, 0.05f, 170.0f);

		// Calculate actual FoV and apply it

		m_currentFov = damped? Mathf.Lerp(m_currentFov, finalFov, damping * Time.deltaTime) : finalFov;
		m_cam.fieldOfView = m_currentFov;
		}


	}


