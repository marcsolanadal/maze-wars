using UnityEngine;
using System;

// TODO: We should use generics to instiantate all our items regarding the class they are.
public class ItemManager : MonoBehaviour
{
    [SerializeField] GameObject maskPrefab;
    [SerializeField] int maskNumber = 2;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    private System.Random pseudoRNG;

    GameObject[] itemList;
    Texture[] textureList;
    Material[] materialList;

    int spacing = 2;

    void Awake()
    {
        // Loading all Items
        itemList = Resources.LoadAll<GameObject>("Items");
        //GameObject mask = Resources.Load<GameObject>("Items");
        Debug.Log("item count: " + itemList.Length);

        // Loading all object textures.
        textureList = Resources.LoadAll<Texture>("Zones/Ruins/Items/Mask/Textures");
        Debug.Log("texture count: " + textureList.Length);
    }

    void Start ()
    {
        if (useRandomSeed)
            seed = DateTime.Now.Ticks.ToString();

        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // TODO: Get random texture and apply it to mesh.
        for (int x = 0; x < maskNumber; x++)
        {
            for (int y = 0; y < maskNumber; y++)
            {
                // Selecting random texture and generating the material.
                Texture texture = textureList[pseudoRNG.Next(0, textureList.Length - 1)];
                Material material = new Material(Shader.Find("Standard"));
                material.SetTexture("_MainTex", texture);

                GameObject mask = Resources.Load<GameObject>("Prefabs/Items");
                
                Instantiate(mask);

                //Mask mask = new Mask(maskPrefab, material);
                //BlendShapeObject mask = new BlendShapeObject(maskPrefab, material);
                //mask.transform.position = new Vector3(x * spacing, y * spacing, 0);
                //mask.prefab.transform.position = new Vector3(x * spacing, y * spacing, 0);
            } 
        }    

    }

    //private void createMaterials()

}
