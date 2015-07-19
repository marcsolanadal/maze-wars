using UnityEngine;

public class Item
{
    string name;
    float provability;
    IGenerator generator;
    GameObject prefab = null;

    public Item(string name, float provability, IGenerator generator)
    {
        this.name = name;
        this.provability = provability;
        this.generator = generator;
    }

    public Item(string name, float provability)
    {
        this.name = name;
        this.provability = provability;

        prefab = Resources.Load(name) as GameObject;
    }

    public GameObject Create()
    {
        return (prefab != null) ? prefab : generator.Create();
    }

}
