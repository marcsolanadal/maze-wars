using UnityEngine;
using System.Collections.Generic;
using System;

// TODO: Need to improve that interface. It doesn't make a lot of sense to have it.
public interface IGenerator
{
    GameObject Create();
    void ApplyBlendShapes();
}

public class ProceduralGenerator : IGenerator
{
    GameObject newPrefab;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    Mesh bakedMesh;
    MeshCollider meshCollider;
    List<Material> materialList;

    // TODO: Tunning of the seed of the pseudoRNG with a static parameter.
    protected System.Random pseudoRNG;
        
    public ProceduralGenerator()
    {
        // Getting random seed.
        pseudoRNG = new System.Random("lolerpoper".GetHashCode());
        //pseudoRNG = new System.Random(DateTime.Now.Ticks.GetHashCode());

        materialList = new List<Material>();
    }

    public GameObject Create()
    {        
        // Getting the SkinnedMeshRenderer and the mesh of the prefab.
        skinnedMeshRenderer = newPrefab.GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;

        // Applying BlendShapes with random values and recalculating the new mesh.
        // The collider is also calculated for the given mesh and a random material 
        // is chosen.
        ApplyBlendShapes();
        RecalculateMesh();
        RecalculateCollider(false);
        ChooseMaterial();

        return newPrefab;
    }

    public virtual void ApplyBlendShapes()
    {
        // Applying BlendShapes with random values by default.
        for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, pseudoRNG.Next(0, 100));
        }
    }

    protected void SetPrefab(string prefabPath)
    {
        newPrefab = Resources.Load(prefabPath) as GameObject;
    }

    protected void RecalculateMesh()
    {
        // Creates a snapshot of the SkinnedMeshRenderer and stores it in the mesh.
        // That skinned mesh renderer should have the shape with the BlendShapes applyied.
        bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Recalcultate the bounding volume of the mesh from the vertices.
        bakedMesh.RecalculateBounds();

        // Selecting part and destroying MeshCollider in case there is one.
        //DestroyImmediate(parent.GetComponent<MeshCollider>());
    }

    protected void RecalculateCollider(bool convex)
    {
        // Adding MeshCollider and assigning the bakedMesh.
        meshCollider = newPrefab.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
        meshCollider.convex = convex;
    }

    protected void GenerateMaterialList(string texturePath)
    {
        // Loading all mask textures and generating one material for each texture.
        // All the materials are stored in the materialList.
        // We fill the material list of the parent ProceduralGenerator.
        // This list must be here because we don't want to have the ProceduralGenerator
        // dealing with the textures of a given material. The general class should only
        // have contact with the list of the given materials.
        Texture[] textureList = Resources.LoadAll<Texture>(texturePath);
        foreach (Texture texture in textureList)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", texture);
            materialList.Add(material);
        }
    }

    protected void ChooseMaterial()
    {
        // Selecting random material from the list and assigning it to the mesh.
        Material material = materialList[pseudoRNG.Next(0, materialList.Count - 1)];
        skinnedMeshRenderer.material = material;
    }

}
