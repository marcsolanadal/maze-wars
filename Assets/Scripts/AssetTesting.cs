using UnityEngine;
using System.Collections;

public class AssetTesting : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // TODO: Place on the loading screen
        GameObject cat = Resources.Load("cat") as GameObject;


        GameObject catClone = Instantiate(cat, new Vector3(5, 0, 0), Quaternion.identity) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
