using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using WebSocketSharp;
using System;


public class AppWebSocket : MonoBehaviour {


	//受信のデリゲート
	public Action<int, int, string> ReceivePacketAction;
	int m_fromId = 0;
	public List<string> m_buttonList = new List<string>();

	Queue<string> m_tmpQueue = new Queue<string>();

	//受信リスト
	private List<string> m_receiveDataList = new List<string>();
	
	WebSocket m_webSocket;
	bool m_IsConnect = false;

	void Awake () {

		// 仮データ生成
		m_buttonList = new List<string>();
		m_buttonList.Add("aaaaaaaaaaaa");
		m_buttonList.Add("bbbbbbbbbbb");
		m_buttonList.Add("ccccccccccc");
		m_buttonList.Add("ddddddddddd");

		m_fromId = int.Parse(DateTime.Now.ToString("MdHms"));
	}
	



	void Update () {
		while ( 0 < m_tmpQueue.Count)
		{
			ReceiveData(m_tmpQueue.Dequeue());
		}
	}


	//接続処理
	private void Connect()
	{
		m_webSocket = new WebSocket("ws://localhost:8000");
		//接続完了
		m_webSocket.OnOpen += (sender, e) =>
		{
			Debug.Log("m_webSocket.OnOpen");
			m_IsConnect = true;
			//デリゲート登録
			ReceivePacketAction += ReceivedMessage;
		};

		//受信したとき
		m_webSocket.OnMessage += (sender, e) =>
		{
			string message = e.Data;
			Debug.Log(string.Format("OnMessage {0}", message));
			m_tmpQueue.Enqueue(message);
		};

		//閉じたとき
		m_webSocket.OnClose += (sender, e) =>
		{
			Debug.Log(string.Format("OnClosed {0}", e.Reason));
			m_IsConnect = false;
		};

		m_webSocket.Connect();
	}




	//切断
	private void Close()
	{
		//デリゲートをクリア
		ReceivePacketAction = null;
		m_tmpQueue.Clear();
		//接続を切断
		m_webSocket.Close();
	}




	void Send(string stream)
	{
		stream = "{ \"type\": " + (int)Type.SEND_DATA + " , \"from\": " + m_fromId + " , \"to\": " + m_fromId + " , \"msg\": \"" + stream + "\" }";

		Debug.Log("send data. stream=" + stream);
		m_webSocket.Send(stream);
	}


	enum Type
	{
		REGISTER = 1,
		SEND_DATA = 2,


	}





	//受信
	void ReceiveData(string receivePackets)
	{
		if (receivePackets != null && receivePackets.Length != 0)
		{
			foreach (string stream in receivePackets.Split('|'))
			{
				int id = 0;
				int type = 0;
				// 登録しておいたデリゲートを実行する
				ReceivePacketAction(type, id, stream);
			}
		}
	}


	// タイプにあわせて処理を振り分ける
	void ReceivedMessage(int type, int id, string jsonData)
	{
		Debug.Log("Received jsonData=" + jsonData);

		m_receiveDataList.Add(jsonData);
		if(m_receiveDataList.Count > 5)
			m_receiveDataList.RemoveAt(0);
	}






	void OnGUI()
	{
		if (!m_IsConnect)
		{
			if (GUILayout.Button("接続する", GUILayout.Width(100)))
			{
				Connect();
			}
		}


		if (m_IsConnect)
		{
			if (GUILayout.Button("切断する", GUILayout.Width(100)))
			{
				Close();
			}

			GUILayout.Space(20);

			foreach (string msg in m_buttonList)
			{
				if (GUILayout.Button(msg, GUILayout.Width(100)))
				{
					Send(msg);
				}
			}
		}


		GUILayout.Space(20);
		foreach (string data in m_receiveDataList)
		{
			GUILayout.TextField(data);
		}

	}

}
