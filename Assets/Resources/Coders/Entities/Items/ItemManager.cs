using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemManager : MonoBehaviour
{
    [SerializeField] int itemNumber = 2;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    private System.Random pseudoRNG;
    int spacing = 2;

    GameObject[] itemList;

    public static Dictionary<string, IGenerator> generators;

    void Start ()
    {
        if (useRandomSeed)
            seed = DateTime.Now.Ticks.ToString();

        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // Initializing the generator dictionary.
        generators = new Dictionary<string, IGenerator>();

        Generators.FillGeneratorDictionary();

        CreateItems(); 

    }

    void CreateItems()
    {
        for (int x = 0; x < itemNumber; x++)
        {
            for (int y = 0; y < itemNumber; y++)
            {
                GameObject item = Generators.Create("Mask");
                item.transform.position = new Vector3(x * spacing, y * spacing, 0);
                Instantiate(item);
            }
        }
    }

}
