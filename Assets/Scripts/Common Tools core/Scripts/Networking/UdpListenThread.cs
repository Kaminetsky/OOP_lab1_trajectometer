
using UnityEngine;
using System;
using System.Threading;


public class UdpListenThread
	{
	public int threadStopTimeoutMs = 500;
	public int threadSleepIntervalMs = 5;
	public bool debug = false;

	public delegate void OnReceiveData ();


	UdpConnection m_connection = null;
	OnReceiveData m_onReceiveDataCb;

	Thread m_thread = null;
	bool m_threadExit = false;


	~UdpListenThread ()
		{
		Stop();
		}


	public void Start (UdpConnection connection, OnReceiveData receiveDataCb)
		{
		if (m_thread != null) Stop();

		m_thread = new Thread(ListenThread);
		m_threadExit = false;
		m_connection = connection;
		m_onReceiveDataCb = receiveDataCb;

		if (debug) Debug.Log("Thread started");
		m_thread.Start();
		}


	public void Stop ()
		{
		if (m_thread != null)
			{
			m_threadExit = true;
			m_thread.Join(threadStopTimeoutMs);

			if (debug) Debug.Log("Thread ended: " + !m_thread.IsAlive);
			m_thread = null;
			}
		}


	void ListenThread ()
		{
		try {
			while (!m_threadExit)
				{
				if (m_connection.messageReceived)
					{
					if (m_onReceiveDataCb != null) m_onReceiveDataCb();
					}

				Thread.Sleep(threadSleepIntervalMs);
				}
			}
		catch (Exception err)
			{
			if (err is ThreadAbortException)
				Debug.LogWarning("Thread aborted");
			else
				Debug.LogError("Exception inside thread: " + err.ToString());
			}
		}
	}
