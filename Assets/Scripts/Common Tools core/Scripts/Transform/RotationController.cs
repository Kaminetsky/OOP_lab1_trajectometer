using UnityEngine;
using System.Collections;


// RotationController
//
// Substitute to transform.rotation.
// Use the properties here for modifying the angles:
//
//	horizontal.angle
//	vertical.angle
//	pitch.angle
//
// Hard-reset the angles to the desired values with:
//
//	horizontal.ResetAngle(angle);
//	vertical.ResetAngle(angle);
//	pitch.ResetAngle(angle);
//


public class RotationController : MonoBehaviour
	{
	public Transform target;
	public bool rotateInWorldSpace = false;
	public bool invertHVorder = false;

	public Rotation horizontal = new Rotation(Rotation.Axis.Up, Rotation.Mode.Free);
	public Rotation vertical = new Rotation(Rotation.Axis.Right, Rotation.Mode.Free, -60, 60);
	public Rotation pitch = new Rotation(Rotation.Axis.Forward);

	public delegate void OnRotationFinished ();
	public OnRotationFinished onRotationFinished;


	Transform m_trans;
	Transform m_parent;
	Transform m_cachedTarget;
	Renderer m_targetRenderer;
	bool m_rotating;


	[System.Serializable]
	public class Rotation
		{
		// Free: modified manually with Move(delta)
		// LookAtTarget: looks at the target's position
		// LookAtTargetCenter: look at the target's center as described by its bounding box (Renderer.bounds)

		public enum Axis { Up, Right, Forward };
		public enum Mode { Disabled, Free, LookAtTarget, LookAtTargetCenter };   // TODO: MatchToTarget, MatchToTargetLocal

		public float angle;
		public Mode mode = Mode.Free;
		public float targetOffset;

		public bool damped = true;
		public float damping = 4.0f;

		public bool clamped = false;
		public float minAngle = -120.0f;
		public float maxAngle = 120.0f;

		float m_currentAngle;
		float m_currentTarget;
		Axis m_axis;
		Vector3 m_axisVector;


		public Rotation (Axis axis)
			{
			m_axis = axis;
			m_axisVector = GetAxisVector(axis);
			mode = Mode.Disabled;
			}


		public Rotation (Axis axis, Mode rotationMode)
			{
			m_axis = axis;
			m_axisVector = GetAxisVector(axis);
			mode = rotationMode;
			}


		public Rotation (Axis axis, Mode rotationMode, float min, float max)
			{
			m_axis = axis;
			m_axisVector = GetAxisVector(axis);
			mode = rotationMode;
			clamped = true;
			minAngle = min;
			maxAngle = max;
			}


		public void ResetAngle (float newAngle)
			{
			m_currentAngle = newAngle;
			m_currentTarget = newAngle;
			angle = newAngle;
			}


		public Quaternion rotation
			{
			get {
				return mode != Mode.Disabled? Quaternion.AngleAxis(m_currentAngle, m_axisVector) : Quaternion.identity;
				}
			}


		public bool isRotating
			{
			get {
				return mode != Mode.Disabled && Mathf.Abs(m_currentAngle - m_currentTarget) < 0.001f;
				}
			}


		public void Update (float deltaTime, Transform self, Transform target, Renderer targetRenderer, Transform reference)
			{
			if (mode == Mode.Disabled) return;

			// Clamp the input data

			if (clamped)
				angle = Mathf.Clamp(angle, minAngle, maxAngle);

			// Calculate the angle based

			float targetValue = angle;

			if (target != null && mode > Mode.Free)
				{
				if (mode == Mode.LookAtTarget)
					{
					targetValue = AngleToTarget(self.position, target.position, reference) + targetOffset;
					}
				else
				if (mode == Mode.LookAtTargetCenter)
					{
					Vector3 targetPos = targetRenderer != null? targetRenderer.bounds.center : target.position;
					targetValue = AngleToTarget(self.position, targetPos, reference) + targetOffset;
					}

				if (clamped)
					targetValue = Mathf.Clamp(targetValue, minAngle, maxAngle);
				}

			// Calculate the actual damped angle.
			// LerpAngle always follows the closest path between both angles, correct for target.
			// Lerp seems better because allow bigger increments without the value taking "shortcuts".

			if (damped)
				{
				if (mode == Mode.Free)
					m_currentAngle = Mathf.Lerp(m_currentAngle, targetValue, damping * deltaTime);
				else
					m_currentAngle = Mathf.LerpAngle(m_currentAngle, targetValue, damping * deltaTime);
				}
			else
				{
				m_currentAngle = targetValue;
				}
			m_currentTarget = targetValue;
			}


		float AngleToTarget (Vector3 pos, Vector3 targetPos, Transform reference)
			{
			Vector3 dir = targetPos - pos;
			Vector3 relative = reference != null? reference.InverseTransformDirection(dir) : dir;

			float result;

			if (m_axis == Axis.Up)
				result = Mathf.Atan2(relative.x, relative.z);
			else
			if (m_axis == Axis.Right)
				result = Mathf.Atan2(relative.y, Mathf.Sqrt(relative.x*relative.x + relative.z*relative.z));
			else
				result = angle;

			return result * Mathf.Rad2Deg;
			}


		Vector3 GetAxisVector (Axis axis)
			{
			if (axis == Axis.Up)
				return Vector3.up;
			else
			if (axis == Axis.Right)
				return Vector3.left;
			else
				return Vector3.forward;
			}
		}

	// ----- [/class Rotation] ---


	void OnEnable ()
		{
		m_trans = GetComponent<Transform>();
		m_rotating = false;

		Vector3 angles = rotateInWorldSpace? m_trans.rotation.eulerAngles : m_trans.localRotation.eulerAngles;

		if (horizontal.mode == Rotation.Mode.Free) horizontal.ResetAngle(angles.y);
		if (vertical.mode == Rotation.Mode.Free) vertical.ResetAngle(-angles.x);
		if (pitch.mode == Rotation.Mode.Free) pitch.ResetAngle(angles.z);
		}


	void LateUpdate ()
		{
		// Cache the rendered of the target

		if (target != m_cachedTarget)
			{
			m_targetRenderer = target != null? target.GetComponent<Renderer>() : null;
			m_cachedTarget = target;
			}

		// Update each rotation

		float dt = Time.deltaTime;
		Transform reference = rotateInWorldSpace? null : m_trans.parent;

		horizontal.Update(dt, m_trans, target, m_targetRenderer, reference);
		vertical.Update(dt, m_trans, target, m_targetRenderer, reference);
		pitch.Update(dt, m_trans, target, m_targetRenderer, reference);

		// Calculate and apply the rotation

		Quaternion rot = new Quaternion();

		if (invertHVorder)
			rot = vertical.rotation * horizontal.rotation;
		else
			rot = horizontal.rotation * vertical.rotation;

		rot = rot * pitch.rotation;

		if (rotateInWorldSpace)
			m_trans.rotation = rot;
		else
			m_trans.localRotation = rot;

		// Trigger the onRotationFinished delegate

		if (vertical.isRotating || horizontal.isRotating || pitch.isRotating)
			{
			m_rotating = true;
			}
		else
			{
			if (m_rotating)
				{
				m_rotating = false;
				if (onRotationFinished != null)
					onRotationFinished();
				}
			}
		}
	}
