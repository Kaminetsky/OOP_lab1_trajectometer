using UnityEngine;
using System.Collections;

public class ApplicationQuit : MonoBehaviour
	{
	public bool desktop = true;
	public bool mobile = true;


	bool m_isDesktop = false;
	bool m_isMobile = false;


	void OnEnable ()
		{
		#if UNITY_STANDALONE
		m_isDesktop = true;
		#endif

		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP_8_1
		m_isMobile = true;
		#endif
		}


	void Update ()
		{
		if (desktop && m_isDesktop || mobile && m_isMobile)
			{
			if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
			}
		}
	}
