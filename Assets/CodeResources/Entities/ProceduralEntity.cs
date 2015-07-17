using UnityEngine;
using System;

public class ProceduralEntity : MonoBehaviour
{
    GameObject parent;
    public Material material { get; set; }

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    MeshCollider meshCollider;

    System.Random pseudoRNG;

    // TODO: Change to Awake() or Start()?
    void Start()
    {
        // Get parent object
        parent = transform.parent.gameObject;

        // Getting random seed and instantiating the pseudoRNG.
        pseudoRNG = new System.Random(DateTime.Now.Ticks.GetHashCode());

        // Getting the SkinnedMeshRenderer and the mesh of the prefab.
        skinnedMeshRenderer = parent.GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;

        // Applying BlendShapes with random values and recalculating the new mesh.
        ApplyBlendShapes();
        RecalculateMesh();

        // Generating material with randomly chosen texture.
        skinnedMeshRenderer.material = material;

        Instantiate(parent);

    }

    private void RecalculateMesh()
    {
        // Creates a snapshot of the SkinnedMeshRenderer and stores it in the mesh.
        // That skinned mesh renderer should have the shape with the BlendShapes applyied.
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Recalcultate the bounding volume of the mesh from the vertices.
        bakedMesh.RecalculateBounds();

        // Selecting part and destroying MeshCollider in case there is one.
        //DestroyImmediate(item.GetComponent<MeshCollider>());

        // Adding MeshCollider and assigning the bakedMesh.
        meshCollider = parent.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
        meshCollider.convex = false;
    }

    public virtual void ApplyBlendShapes()
    {
        // Applying BlendShapes with random values.
        for (int i = 0; i < skinnedMesh.blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, pseudoRNG.Next(0, 100));
        }
    }

}
