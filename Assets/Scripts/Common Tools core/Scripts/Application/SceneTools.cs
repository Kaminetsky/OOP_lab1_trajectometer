
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif


public class SceneTools : MonoBehaviour
	{
	public bool slowTimeMode = false;
	public float slowTime = 0.3f;

	public KeyCode hotkeyReset = KeyCode.R;
	public KeyCode hotkeyTime = KeyCode.T;


	void OnEnable ()
		{
		Time.timeScale = slowTimeMode? slowTime : 1.0f;
		}


	void Update ()
		{
		if (Input.GetKeyDown(hotkeyReset))
			{
			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(0, LoadSceneMode.Single);
			#else
			Application.LoadLevel(0);
			#endif
			}

		if (Input.GetKeyDown(hotkeyTime))
			{
			slowTimeMode = !slowTimeMode;
			Time.timeScale = slowTimeMode? slowTime : 1.0f;
			}
		}
	}
