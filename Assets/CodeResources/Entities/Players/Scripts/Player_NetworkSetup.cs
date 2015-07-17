using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_NetworkSetup : NetworkBehaviour
{

	[SerializeField] Camera FPSCharacterCam;
	[SerializeField] AudioListener audioListener;

	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) {

			// We disable the main scene camera
			GameObject.Find("Scene Camera").SetActive(false);

			// We enable the controllers of the player
			//GetComponent<CharacterController> ().enabled = true;
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			FPSCharacterCam.enabled = true;
			audioListener.enabled = true;
		}
	 
	}

}
