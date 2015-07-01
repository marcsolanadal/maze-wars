using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_Manager : NetworkBehaviour {

	private NetworkManager nManager;
	private NetworkClient myClient;
	
	void Start() 
	{
		nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		myClient = nManager.client;
	}

	public void ClickExitGame() {	
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif 
	}
	
	public void CreateHost() {	
		myClient = nManager.StartHost();
	}
	
	public void ConnectToServer() {
		myClient.Connect("127.0.0.1", 7777);
		//myClient.Connect(nManager.networkAddress, nManager.networkPort);
	}
	
	public void DisconnectFromServer() {	
		Debug.Log("Disconnecting from server...");
		myClient.Disconnect();
	}
	
}
