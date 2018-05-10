using UnityEngine;
using System.Collections;


[RequireComponent(typeof(RotationController))]
public class RotationInput : MonoBehaviour
	{
	public enum InputSource { StandardInput, Messages };

	public InputSource source = InputSource.StandardInput;
	public string horizontalAxis = "Mouse X";
	public string verticalAxis = "Mouse Y";
	public int mouseButtonForDrag = -1;				// Set to 0, 1 or 2 to allow movement when the specified button is held down
	public bool mouseButtonExclusive = false;		// If true the movement is allowed when only that single button is pressed.

	public float horizontalSensitivity = 4.0f;
	public float verticalSensitivity = 4.0f;

	public float horizontalDefault = 0.0f;
	public float verticalDefault = 0.0f;

	RotationController m_rot;


	void OnEnable ()
		{
		m_rot = GetComponent<RotationController>();
		}


	public void Move (Vector2 delta)
		{
		if (enabled)
			{
			m_rot.horizontal.angle += delta.x * horizontalSensitivity;
			m_rot.vertical.angle += delta.y * verticalSensitivity;
			}
		}


	public void ResetDefaults ()
		{
		if (enabled)
			{
			// Recortar el ángulo actual para que el Reset se haga por el camino más corto
			m_rot.horizontal.ResetAngle(CommonTools.ClampAngle(m_rot.horizontal.angle));
			m_rot.vertical.ResetAngle(CommonTools.ClampAngle(m_rot.vertical.angle));

			m_rot.horizontal.angle = CommonTools.ClampAngle(horizontalDefault);
			m_rot.vertical.angle = CommonTools.ClampAngle(verticalDefault);
			}
		}


	public void ResetDefaultsImmediate ()
		{
		if (enabled)
			{
			m_rot.horizontal.ResetAngle(CommonTools.ClampAngle(horizontalDefault));
			m_rot.vertical.ResetAngle(CommonTools.ClampAngle(verticalDefault));
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
			Move(new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)));
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


	public void OnDrag (Vector2 delta)
		{
		if (source == InputSource.Messages)
			Move(delta);
		}

	}

