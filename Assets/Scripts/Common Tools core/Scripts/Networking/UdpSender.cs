
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class UdpSender
	{
	UdpClient m_client;
	IPEndPoint m_sendPoint;


	public UdpSender (string server, int port)
		{
		// Create UDP socket and prepare the message

		m_client = new UdpClient();

		// Parse the server address

		IPAddress address = IPAddress.Parse(server);
		m_sendPoint = new IPEndPoint(address, port);
		}


	~UdpSender ()
		{
		Close();
		}


	public void SendSync (byte[] bytesToSend, int maxBytes = -1)
		{
		maxBytes = Mathf.Min(bytesToSend.Length, maxBytes);
		if (maxBytes < 0) maxBytes = bytesToSend.Length;

		m_client.Send(bytesToSend, maxBytes, m_sendPoint);
		}


	public void SendAsync (byte[] bytesToSend, int maxBytes = -1)
		{
		maxBytes = Mathf.Min(bytesToSend.Length, maxBytes);
		if (maxBytes < 0) maxBytes = bytesToSend.Length;

		m_client.BeginSend(bytesToSend, maxBytes, m_sendPoint, new System.AsyncCallback(MessageSentA), m_client);
		}


	void MessageSentA (System.IAsyncResult ar)
		{
		UdpClient client = (UdpClient)ar.AsyncState;
		client.EndSend(ar);
		}


	public void Close ()
		{
		m_client.Close();
		}
	}

