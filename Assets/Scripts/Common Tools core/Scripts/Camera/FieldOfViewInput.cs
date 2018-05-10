using UnityEngine;
using System.Collections;





[RequireComponent(typeof(FieldOfViewController))]
public class FieldOfViewInput : MonoBehaviour
	{
	public enum InputSource { StandardInput, Messages };
	public enum MoveParameter { Angle, Size, SizeProportional };

	public InputSource source = InputSource.StandardInput;
	public string axisName = "Mouse ScrollWheel";

	public MoveParameter parameter = MoveParameter.Angle;
	public float angleSensitivity = 8.0f;
	public float sizeSensitivity = 0.5f;

	public float angleDefault = 50.0f;
	public float sizeDefault = 1.0f;

	FieldOfViewController m_fov;


	void OnEnable ()
		{
		m_fov = GetComponent<FieldOfViewController>();
		}


	public void Move (float delta)
		{
		if (!enabled) return;

		delta = -delta;

		switch (parameter)
			{
			case MoveParameter.Angle:
				m_fov.fieldOfView += delta * angleSensitivity;
				break;

			case MoveParameter.Size:
				m_fov.targetSize += delta * sizeSensitivity;
				break;

			case MoveParameter.SizeProportional:
				m_fov.targetSize *= 1.0f + delta * sizeSensitivity;
				break;
			}
		}


	public void ResetDefaults ()
		{
		if (!enabled) return;

		switch (parameter)
			{
			case MoveParameter.Angle:
				m_fov.fieldOfView = angleDefault;
				break;

			case MoveParameter.Size:
			case MoveParameter.SizeProportional:
				m_fov.targetSize = sizeDefault;
				break;
			}
		}


	void ProcessStandardInput ()
		{
		float delta = Input.GetAxis(axisName);
		Move(delta);
		}


	void Update ()
		{
		if (source == InputSource.StandardInput)
			ProcessStandardInput();
		}


	// Messages


	public void OnScroll (float delta)
		{
		if (source == InputSource.Messages)
			Move(delta / Screen.height);
		}


	// Scroll allows using messages and standard input at the same time

	public void Scroll (float delta)
		{
		Move(delta / Screen.height);
		}

	}
