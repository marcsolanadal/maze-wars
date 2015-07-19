using UnityEngine;
using System.Collections.Generic;

// Why use a static class instead of the simple dictionary?
// That class will be accessible through all our code so we will
// be able to create objects in all our classes.
public static class GeneratorDictionary
{
    private static Dictionary<string, IGenerator> generators;

    public static void FillWithGenerators()
    {
        // Initializing the generator dictionary.
        generators = new Dictionary<string, IGenerator>();

        // Filling the dictionary with generators.
        generators.Add("Mask", new MaskGenerator());
    }

    public static GameObject Create(string itemName)
    {
        return generators[itemName].Create();
    }
}