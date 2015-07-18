using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Using channel 0 for positions and rotations
[NetworkSettings(channel=0,sendInterval=0.03f)]
public class Player_SyncPosition : NetworkBehaviour
{
	// The hook attribute can be used to specify a function to be called when the sync var changes value on the client.
	[SyncVar (hook="SyncPositionValues")] 
	private Vector3 syncPos;
	
	// Instance position
	[SerializeField] Transform myTransform;
	
	// Decreassing network bandwidth
	private Vector3 lastPos;
	private float maxStepDistance = 0.5f;
	
	// Latency displaying
	private NetworkClient client;
	private int latency;
	private Text latencyText;
	
	// Lerping parameters
	//private float lerpRate;
	//private float normalLerpRate = 20;
	//private float fasterLerpRate = 30;
	
	// Movement
	// FIXME: We need to dinamically take that speed from the player controller.
	private int speed = 6;
	
	// Historical lerping
	private List<Vector3> syncPosList = new List<Vector3>();
	[SerializeField] bool useHistoricalLerping = false;
	private int lerpThereshold = 10;
	private float closeEnough = 0.01f;
	
	void Start () 
	{
		// We get the client from the network manager object in the menu scene.
		client = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find("LatencyText").GetComponent<Text>();
		//lerpRate = normalLerpRate;
	}
	
	void Update ()  
	{
		LerpPosition ();
		ShowLatency ();
	}
	
	void FixedUpdate ()
	{
		TransmitPosition ();
	}
	
	/* 
	 * Because we are using a server authoritative system. The positions of each player are sended to 
	 * the server and then the server sends back the positions to the other clients.
	 * This function will take the current position of the player object, if its not local it will lerp it
	 * using the deltaTime to have the lerp frame independent.
	 */
	void LerpPosition ()
	{
		if (!isLocalPlayer) {
			if (useHistoricalLerping) {
				HistoricalLerping();
			}
			else {
				OrdinaryLerping();
			}
		}
	}

	void OrdinaryLerping ()
	{
		//myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
		myTransform.position = Vector3.MoveTowards(myTransform.position, syncPos, Time.deltaTime * speed);
		
	}
	
	void HistoricalLerping () 
	{
		if (syncPosList.Count > 0) {

			myTransform.position = Vector3.MoveTowards(myTransform.position, syncPosList[0], Time.deltaTime * speed);
			//myTransform.position = Vector3.Lerp (myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);
			
			if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough) {
				syncPosList.RemoveAt(0);
			}
			
			speed = (syncPosList.Count > lerpThereshold) ? 10 : 6;
			//lerpRate = (syncPosList.Count > lerpThereshold) ? fasterLerpRate : normalLerpRate;
			
			Debug.Log(syncPosList.Count.ToString());
		}
	}

	/*
	 * This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on 
	 * the server by sending a command from a client.
	 */
	[Command]
	void CmdProvidePositionToServer (Vector3 pos)
	{	
		syncPos = pos; // The server updates the position status for the current instance of the object.
	}

	/*
	 * We can use [Client] as well our choice will not generate any warnings.
	 * When the server is hosting it is like a client and it can run that function.
	 */
	[Client]
	void TransmitPosition ()
	{
		if (isLocalPlayer && (Vector3.Distance(myTransform.position, lastPos) > maxStepDistance)) {
			CmdProvidePositionToServer (myTransform.position);
			lastPos = myTransform.position;
		}
	}
	
	[Client]
	void SyncPositionValues (Vector3 pos) 
	{
		syncPos = pos;
		syncPosList.Add(syncPos);
	}
	
	void ShowLatency () {
		if (isLocalPlayer) {
			latency = client.GetRTT();
			latencyText.text = "Latency: " + latency.ToString();
		}
	}
}