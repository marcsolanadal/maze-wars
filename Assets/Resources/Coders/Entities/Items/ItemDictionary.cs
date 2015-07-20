using UnityEngine;
using System.Collections.Generic;

public static class ItemDictionary {

     static Dictionary<string, Item> items;

    public static void FillWithItems()
    {
        // Initializing the items dictionary.
        items = new Dictionary<string, Item>();

        // Filling the dictionary with items.
        // TODO: I'm sure this doubled itemName can be improved...
        items.Add("Mask", new Item("Mask", 0.1f, new MaskGenerator()));
        items.Add("Torch", new Item("Torch", 0.1f, new TorchGenerator()));

    }

    public static GameObject Create(string itemName)
    {
        return items[itemName].Create();

    }
}
