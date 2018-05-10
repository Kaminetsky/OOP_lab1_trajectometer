
using UnityEngine;


[ExecuteInEditMode]
public class OnScreenNotes : MonoBehaviour
	{
	[TextArea(2, 10)]
	public string text;

	public Vector2 screenPosition = new Vector2(8, 8);


	[Space(5)]
	public Font font;

	GUIStyle m_smallStyle = new GUIStyle();
	GUIStyle m_bigStyle = new GUIStyle();


	void OnEnable ()
		{
		UpdateTextProperties();
		}


	void OnValidate ()
		{
		UpdateTextProperties();
		}


	void UpdateTextProperties ()
		{
		m_smallStyle.font = font;
		m_smallStyle.fontSize = 10;
		m_smallStyle.normal.textColor = Color.white;

		m_bigStyle.font = font;
		m_bigStyle.fontSize = 20;
		m_bigStyle.normal.textColor = Color.white;
		}


	void OnGUI ()
		{
		float xPos = screenPosition.x < 0? Screen.width + screenPosition.x : screenPosition.x;
		float yPos = screenPosition.y < 0? Screen.height + screenPosition.y : screenPosition.y;

		GUI.Label(new Rect (xPos+8, yPos+8, Screen.width, Screen.height), text, m_smallStyle);
		}
	}
