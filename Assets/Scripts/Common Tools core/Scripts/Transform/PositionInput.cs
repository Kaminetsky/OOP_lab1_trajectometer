using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PositionController))]
public class PositionInput : MonoBehaviour
	{
	public enum InputSource { StandardInput, Messages };
	public enum OutputPlane { XZ, XY, ZY };

	public InputSource source = InputSource.StandardInput;
	public OutputPlane outputPlane = OutputPlane.XZ;
	public bool swapCoordinates = false;

	public string inputAxisX = "Mouse X";
	public string inputAxisY = "Mouse Y";
	public int mouseButtonForDrag = -1;				// Set to 0, 1 or 2 to allow movement when the specified button is held down
	public bool mouseButtonExclusive = false;		// If true the movement is allowed when only that single button is pressed.

	public Vector2 inputSensitivity = Vector2.one;
	public Vector2 defaultPosition = Vector2.zero;


	PositionController m_pos;


	void OnEnable ()
		{
		m_pos = GetComponent<PositionController>();
		}


	public Vector3 MapToPlane (Vector2 v, Vector3 defaultPos)
		{
		if (swapCoordinates)
			{
			float t = v.x;
			v.x = v.y;
			v.y = t;
			}

		Vector3 res = Vector3.zero;

		switch (outputPlane)
			{
			case OutputPlane.XZ:
				res.x = v.x;
				res.y = defaultPos.y;
				res.z = v.y;
				break;

			case OutputPlane.XY:
				res.x = v.x;
				res.y = v.y;
				res.z = defaultPos.z;
				break;

			case OutputPlane.ZY:
				res.x = defaultPos.x;
				res.y = v.y;
				res.z = v.x;
				break;
			}

		return res;
		}


	public void Move (Vector2 delta)
		{
		if (enabled)
			{
			m_pos.position += MapToPlane(Vector3.Scale(delta, inputSensitivity), Vector2.zero);
			}
		}


	public void ResetDefaults ()
		{
		if (enabled)
			{
			m_pos.ResetPosition(MapToPlane(defaultPosition, m_pos.position));
			}
		}


	void ProcessStandardInput ()
		{
		bool allowMove = true;

		if (mouseButtonForDrag != -1)
			{
			int buttons = 0;

			if (Input.GetMouseButton(mouseButtonForDrag))
				{
				if (mouseButtonExclusive)
					{
					// Only the selected mouse button must be held

					if (Input.GetMouseButton(0)) buttons++;
					if (Input.GetMouseButton(1)) buttons++;
					if (Input.GetMouseButton(2)) buttons++;
					}
				else
					buttons = 1;
				}

			allowMove = buttons == 1;
			}

		if (allowMove)
			{
			float axisX = string.IsNullOrEmpty(inputAxisX)? 0.0f : Input.GetAxis(inputAxisX);
			float axisY = string.IsNullOrEmpty(inputAxisY)? 0.0f : Input.GetAxis(inputAxisY);

			Move(new Vector2(axisX, axisY));
			}
		}


	void Update ()
		{
		switch (source)
			{
			case InputSource.StandardInput:
				{
				ProcessStandardInput();
				break;
				}
			}
		}


	// Message handling


	public void OnMove (Vector2 delta)
		{
		if (source == InputSource.Messages)
			Move(delta);
		}

	}

