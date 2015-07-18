using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_ProceduralGenerator : NetworkBehaviour 
{

	[SerializeField] GameObject body;
	[SerializeField] GameObject arm;
	
	[SyncVar] Color syncBodyColor = Color.white;
	[SyncVar] Color syncArmColor = Color.white;
	[SyncVar] bool updateColorsFlag = false;
	
	private MeshRenderer bodyRenderer;
	private MeshRenderer armRenderer;

	private Material bodyMaterial;
	private Material armMaterial;

	void Start () 
	{		
		// If is the local player we ask the server for colors.
		if (isLocalPlayer) {
			CmdAskRandomColors();         
		}
		else {
			updateColorsFlag = true; // The non local players need to update it's color once.
		}
	
		// Creating materials
		bodyMaterial = new Material(Shader.Find("Standard"));
		armMaterial = new Material(Shader.Find("Standard"));
		
		// Getting the object mesh 
		bodyRenderer = body.GetComponent<MeshRenderer>();
		armRenderer = arm.GetComponent<MeshRenderer>();
		
	}
	
	// We update the colors only once when the local or non-local players are created.
	void FixedUpdate()
	{
		if (updateColorsFlag) 
			ApplyColors();
	}

	void ApplyColors() 
	{
		// Assigning colors to materials
		bodyMaterial.color = syncBodyColor;
		armMaterial.color = syncArmColor;
		
		// assigning the new material to meshes
		bodyRenderer.material = bodyMaterial;
		armRenderer.material = armMaterial;
		
		updateColorsFlag = false;
	}
	
	[Command]
	void CmdAskRandomColors()
	{
		syncBodyColor = new Color(Random.value, Random.value, Random.value);
		syncArmColor = new Color(Random.value, Random.value, Random.value);
		updateColorsFlag = true;
	}
	
}
