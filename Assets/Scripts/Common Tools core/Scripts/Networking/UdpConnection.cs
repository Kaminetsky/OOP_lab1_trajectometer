
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class UdpConnection
	{
	private class UdpState
		{
		public UdpClient client;
		public IPEndPoint endPoint;
		}


	UdpClient m_client = null;
	IPEndPoint m_listenPoint;
	UdpState m_state;

	byte[] m_receivedBytes;
	bool m_received = false;

	IPEndPoint m_sendPoint;
	bool m_sent = false;
	int m_lastBytesSent = 0;


	// Connection & listening to messages
	// --------------------------------------------------------------------------------------------


	public void StartConnection (int listenPort)
		{
		if (m_client != null) StopConnection();

		m_listenPoint = new IPEndPoint(IPAddress.Any, listenPort);
		m_client = new UdpClient(m_listenPoint);

		m_state = new UdpState();
		m_state.endPoint = m_listenPoint;
		m_state.client = m_client;

		m_sendPoint = new IPEndPoint(IPAddress.Any, listenPort);
		m_sent = true;

		QueryNextMessage();
		}


	public void StopConnection ()
		{
		if (m_client != null)
			{
			m_client.Close();
			m_client = null;
			}

		//Debug.Log("Connection closed");
		}


	// Information


	public bool connected { get { return m_client != null; } }
	public string localDescription { get { return m_client != null? m_listenPoint.ToString() : "none"; } }
	public string remoteDescription { get { return m_client != null? m_sendPoint.ToString() : "none"; } }


	// Message arrival


	public bool messageReceived { get { return m_client != null && m_received; } }
	public int receivedMessageSize { get { return messageReceived? m_receivedBytes.Length : 0; } }


	public string GetMessageString ()
		{
		if (m_client != null && m_received)
			{
			string message = System.Text.Encoding.ASCII.GetString(m_receivedBytes);
			m_received = false;
			QueryNextMessage();
			return message;
			}

		return "";
		}


	public int GetMessageBinary (byte[] buffer)
		{
		if (m_client != null && m_received)
			{
			int maxBytes = Mathf.Min(buffer.Length, m_receivedBytes.Length);

            for (int i=0; i<maxBytes; i++)
				buffer[i] = m_receivedBytes[i];

			m_received = false;
			QueryNextMessage();
			return maxBytes;
			}

		return 0;
		}


	// Private asynchronous UDP listening functions


	void QueryNextMessage ()
		{
		//Debug.Log("Listening for messages");
		m_received = false;
		m_client.BeginReceive(new System.AsyncCallback(MessageReceived), m_state);
		}


	void MessageReceived (System.IAsyncResult ar)
		{
		UdpState state = (UdpState)ar.AsyncState;

		// Store the received bytes

		m_receivedBytes = state.client.EndReceive(ar, ref state.endPoint);
		m_received = true;

		// Store the remote address so we could bounce back messages directly

		if (m_sendPoint.Address == IPAddress.Any)
			m_sendPoint.Address = state.endPoint.Address;
		}


	// Bouncing messages to the other side
	// --------------------------------------------------------------------------------------------


	public void SetDestination(string dest, int port)
		{
		if (m_client == null) return;

		IPHostEntry host = Dns.GetHostEntry(dest);
		m_sendPoint = new IPEndPoint(host.AddressList[0], port);
		}


	public int lastBytesSent { get { return m_client != null? m_lastBytesSent : 0; } }

	public bool canSendMessage { get { return m_client != null && m_sendPoint.Address != IPAddress.Any && m_sent; } }


	public void SendMessageBinary (byte[] bytesToSend)
		{
		SendMessageBinary(bytesToSend, bytesToSend.Length);
		}


	public void SendMessageBinary (byte[] bytesToSend, int size)
		{
		if (canSendMessage && bytesToSend.Length > 0)
			{
			m_sent = false;
			m_client.BeginSend(bytesToSend, size, m_sendPoint, new System.AsyncCallback(MessageSent), m_client);
			}
		}


	public void SendMessageString (string message)
		{
		byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
		SendMessageBinary(messageBytes);
		}


	// Private asynchronous UDP sending function


	void MessageSent (System.IAsyncResult ar)
		{
		UdpClient client = (UdpClient)ar.AsyncState;
		m_lastBytesSent = client.EndSend(ar);
		m_sent = true;
		// Debug.Log("Message sent (" + m_lastBytesSent + " bytes)");
		}


	// Static utility functions
	// --------------------------------------------------------------------------------------------


	// Synchronously send an UDP message. The calling thread is blocked until the operation completes


	static void SendMessageSync (string server, int port, byte[] bytesToSend)
		{
		var Client = new UdpClient();

		try {
			Client.Send(bytesToSend, bytesToSend.Length, server, port);
			}
		catch (System.Exception e)
			{
			Debug.LogError("SendMessageSync error: " + e.ToString());
			}
		}


	static void SendMessageSync (string server, int port, string message)
		{
		byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
		SendMessageSync(server, port, bytesToSend);
		}


	// Asyncronously send an UDP message. The calling thread returns inmediately while the operation
	// gets performed in the background.


	static void SendMessageAsync (string server, int port, byte[] bytesToSend)
		{
		// Create UDP socket and prepare the message

		UdpClient client = new UdpClient();

		// Resolve server name

		IPHostEntry host = Dns.GetHostEntry(server);
		IPEndPoint endPoint = new IPEndPoint(host.AddressList[0], port);

		client.BeginSend(bytesToSend, bytesToSend.Length, endPoint, new System.AsyncCallback(MessageSentA), client);
		}


	static void SendMessageAsync (string server, int port, string message)
		{
		byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
		SendMessageAsync(server, port, bytesToSend);
		}


	static void MessageSentA (System.IAsyncResult ar)
		{
		UdpClient client = (UdpClient)ar.AsyncState;
		client.EndSend(ar);
		client.Close();
		}
	}
