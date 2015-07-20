using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [SerializeField] int itemNumber = 10;
    int spacing = 2;

    List<Item> itemList;

    void Awake()
    {
        // TODO: Put this initializations in the first loaded scene.
        GeneratorDictionary.FillWithGenerators();
        ItemDictionary.FillWithItems();

        itemList = new List<Item>();
    }

    void Start()
    {
        CreateItems();
    }

    void CreateItems()
    {
        for (int x = 0; x < itemNumber; x++)
        {
            for (int y = 0; y < itemNumber; y++)
            {
                GameObject item = ItemDictionary.Create("Torch");
                item.transform.position = new Vector3(x * spacing, y * spacing, 0);
                Instantiate(item);
            }
        }
    }

    void CreateItemFromList()
    {

    }

    void CreateItemList()
    {

    }
}
