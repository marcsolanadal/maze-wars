using UnityEngine;

public class Chest_Controller : MonoBehaviour {

    private bool open = false;

    private AudioSource audioSource;
    public AudioClip chestOpen;
    public AudioClip chestClose;

    private SkinnedMeshRenderer skinnedMeshRendererBottom;
    private SkinnedMeshRenderer skinnedMeshRendererTop;
    private Mesh skinnedMeshBottom;
    private Mesh skinnedMeshTop;

    private int blendShapeCountBottom = 0;
    private int blendShapeCountTop = 0;

    [SerializeField][Range(0, 1)] float size;
    [SerializeField][Range(0, 1)] float depth;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Getting Renderer and Mesh for the bottom part of the chest.
        skinnedMeshRendererBottom = transform.FindChild("Bottom").GetComponent<SkinnedMeshRenderer>();
        skinnedMeshBottom = skinnedMeshRendererBottom.sharedMesh;

        // Getting Renderer and Mesh for the top part of the chest.
        skinnedMeshRendererTop = transform.FindChild("Top").GetComponent<SkinnedMeshRenderer>();
        skinnedMeshTop = skinnedMeshRendererTop.sharedMesh;
    }

    // Use this for initialization
    void Start ()
    {
        CreateBlendMesh(skinnedMeshRendererBottom, skinnedMeshBottom, "Bottom", false);
        CreateBlendMesh(skinnedMeshRendererTop, skinnedMeshTop, "Top", true);
    }

    void Update()
    {
        // TODO: Possible to have the sound clip depending on the rotation of the pivot of the chest?
        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody rb = transform.FindChild("Top").GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 100);

            // XOR the open boolean.
            open = !open;

            HingeJoint hingeJoint = transform.FindChild("Bottom").GetComponent<HingeJoint>();
            Debug.Log(hingeJoint.angle);


            // Assigning the correct clip.
            audioSource.clip = (open) ? chestOpen : chestClose;

            // Stoping the previous clip and starting the next one.
            audioSource.Stop();
            audioSource.Play();

        }

    }

    // TODO: Put this into Misc class?
    private void CreateBlendMesh(SkinnedMeshRenderer skinnedMeshRenderer, Mesh skinnedMesh, string name, bool convex)
    {
        // Detecting how many BlendShapes we have.
        int blendShapeCount = 0;
        blendShapeCount = skinnedMesh.blendShapeCount;
        Debug.Log("BlendShape count bottom: " + blendShapeCount);

        // Applying BlendShapes.
        if (blendShapeCount != 0)
            skinnedMeshRenderer.SetBlendShapeWeight(0, size * 100);

        // Creates a snapshot of the SkinnedMeshRenderer and stores it in the mesh.
        // That skinned mesh renderer should have the shape with the BlendShapes applyied.
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Recalcultate the bounding volume of the mesh from the vertices.
        bakedMesh.RecalculateBounds();
        Debug.Log("Baked mesh bounds: " + bakedMesh.bounds.ToString());
    
        // Selecting part and destroying MeshCollider in case there is one.
        GameObject child = transform.FindChild(name).gameObject;
        DestroyImmediate(child.GetComponent<MeshCollider>());

        // Adding MeshCollider and assigning the bakedMesh.
        MeshCollider meshCollider = child.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
        meshCollider.convex = convex;
    }

}