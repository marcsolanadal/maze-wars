using UnityEngine;
using System;

public class MaskGenerator : MonoBehaviour
{
    [SerializeField] GameObject maskPrefab;
    [SerializeField] int maskNumber = 10;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    private System.Random pseudoRNG;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;

    Texture[] textureList;

    int spacing = 2;

    void Start ()
    {
        // Loading all mask textures.
        string localPath = "Zones/Ruins/Items/Mask/Textures";
        textureList = Resources.LoadAll<Texture>(localPath);
     

        if (useRandomSeed)
            seed = DateTime.Now.Ticks.ToString();

        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // TODO: Get random texture and apply it to mesh.
        for (int x = 0; x < maskNumber; x++)
        {
            for (int y = 0; y < maskNumber; y++)
            {
                GameObject mask = CreateMask(maskPrefab, true);
                mask.transform.position = new Vector3(x * spacing, y * spacing, 0);
                //Debug.Log(mask.transform.rotation);
                //mask.transform.rotation = new Quaternion(-0.7f, -0.7f, 0, 0.7f);
            }
        }    

    }

    private GameObject CreateMask(GameObject mask, bool convex)
    {
        // Getting the SkinnedMeshRenderer and the mesh of the prefab.
        skinnedMeshRenderer = mask.GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;

        // Applying BlendShapes with random values.
        for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, pseudoRNG.Next(0, 100));
        }

        // Creates a snapshot of the SkinnedMeshRenderer and stores it in the mesh.
        // That skinned mesh renderer should have the shape with the BlendShapes applyied.
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Recalcultate the bounding volume of the mesh from the vertices.
        bakedMesh.RecalculateBounds();

        // Selecting part and destroying MeshCollider in case there is one.
        DestroyImmediate(mask.GetComponent<MeshCollider>());

        // Adding MeshCollider and assigning the bakedMesh.
        MeshCollider meshCollider = mask.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
        meshCollider.convex = convex;

        // Selecting random texture.
        Texture texture = textureList[pseudoRNG.Next(0, textureList.Length - 1)];

        // Generating material with randomly chosen texture.
        Material material = new Material(Shader.Find("Standard"));
        material.SetTexture("_MainTex", texture);
        skinnedMeshRenderer.material = material;

        return Instantiate(mask);
    }

}
