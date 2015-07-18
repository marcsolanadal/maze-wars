using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel=0,sendInterval=0.03f)]
public class Player_SyncRotation : NetworkBehaviour
{
	[SyncVar] Quaternion syncBodyRot; // We only need the Y axis
    [SyncVar] Quaternion syncCamRot; // We noly need the Z axis
	
	private Quaternion lastBodyRot;
	private Quaternion lastCamRot;
	private float angleStep = 5.0f;

	[SerializeField] float lerpRate = 15;
	[SerializeField] Transform bodyTransform;
	[SerializeField] Transform camTransform;

	void Update () 
	{
		LerpRotations();
	}
	
	void FixedUpdate ()
	{
		TransmitRotations();
	}
	
	void LerpRotations () 
	{
		if (!isLocalPlayer) {
			bodyTransform.rotation = Quaternion.Lerp (bodyTransform.rotation, syncBodyRot, Time.deltaTime * lerpRate);
			camTransform.rotation = Quaternion.Lerp (camTransform.rotation, syncCamRot, Time.deltaTime * lerpRate);
		}
	}
	
	[Command]
	void CmdProvideRotationsToServer (Quaternion bodyRot, Quaternion camRot)
	{
		syncBodyRot = bodyRot;
		syncCamRot = camRot;
		//Debug.Log("Syncing rotations...");
	}
	
	[ClientCallback]
	void TransmitRotations ()
	{
		float diffBodyAngle = Quaternion.Angle(bodyTransform.rotation, lastBodyRot);
		float diffCamAngle = Quaternion.Angle(camTransform.rotation, lastCamRot);
		
		if (isLocalPlayer && (diffBodyAngle > angleStep) || (diffCamAngle > angleStep)) {
			CmdProvideRotationsToServer(bodyTransform.rotation, camTransform.rotation);
			lastBodyRot = bodyTransform.rotation;
			lastCamRot = camTransform.rotation;
		}
	}
	
}
