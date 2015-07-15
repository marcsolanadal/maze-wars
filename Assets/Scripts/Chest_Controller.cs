using UnityEngine;

public class Chest_Controller : MonoBehaviour {

    //private Animator animator;
    private bool open = false;
    //private int openHash = Animator.StringToHash("Open");

    private AudioSource audioSource;
    public AudioClip chestOpen;
    public AudioClip chestClose;

    private SkinnedMeshRenderer skinnedMeshRendererBottom;
    private SkinnedMeshRenderer skinnedMeshRendererTop;
    private Mesh skinnedMeshBottom;
    private Mesh skinnedMeshTop;

    private int blendShapeCountBottom = 0;
    private int blendShapeCountTop = 0;

    private MeshCollider meshColliderBottom;
    private MeshCollider meshColliderTop;

    [SerializeField][Range(0, 1)] float size;
    [SerializeField][Range(0, 1)] float depth;

    void Awake()
    {
        //animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //meshColliderBottom = transform.FindChild("Bottom").GetComponent<MeshCollider>();
        //meshColliderTop = transform.FindChild("Top").GetComponent<MeshCollider>();

        // Using BlendShapes

        // Bottom
        skinnedMeshRendererBottom = transform.FindChild("Bottom").GetComponent<SkinnedMeshRenderer>();
        skinnedMeshBottom = skinnedMeshRendererBottom.sharedMesh;

        // Top
        skinnedMeshRendererTop = transform.FindChild("Top").GetComponent<SkinnedMeshRenderer>();
        skinnedMeshTop = skinnedMeshRendererTop.sharedMesh;
    }

    // Use this for initialization
    void Start ()
    {
        // Detecting how many BlendShapes we have.
        blendShapeCountBottom = skinnedMeshBottom.blendShapeCount;
        Debug.Log("BlendShape count bottom: " + blendShapeCountBottom);

        // Applying BlendShapes.
        if (blendShapeCountBottom != 0)
            skinnedMeshRendererBottom.SetBlendShapeWeight(0, size * 100);
        
        // Creates a snapshot of the SkinnedMeshRenderer and stores it in the mesh.
        // That skinned mesh renderer should have the shape with the BlendShapes applyied.
        Mesh bakedMesh = new Mesh();
        skinnedMeshRendererBottom.BakeMesh(bakedMesh);

        // Recalcultate the bounding volume of the mesh from the vertices.
        bakedMesh.RecalculateBounds();
        Debug.Log("Baked mesh bounds: " + bakedMesh.bounds.ToString());


        GameObject child = transform.FindChild("Bottom").gameObject;
        DestroyImmediate(child.GetComponent<MeshCollider>());

        meshColliderBottom = child.AddComponent<MeshCollider>();
        meshColliderBottom.sharedMesh = bakedMesh;
        meshColliderBottom.convex = false;

    }

	// TODO: It's more eficcient to compare hash id's instead of strings.
	void Update () {


        //DestroyImmediate(transform.FindChild("Bottom").GetComponent<MeshCollider>());
        //meshColliderBottom = 

        //DestroyImmediate(this.GetComponent<MeshCollider>());
        //var collider = this.AddComponent<MeshCollider>();
        //collider.sharedMesh = myMesh;

        //meshColliderBottom.sharedMesh = null;
        //meshColliderBottom.sharedMesh = skinnedMeshBottom;

        //meshColliderBottom.transform. = new Vector3(2, 1, 1);

        //skinnedMeshRendererBottom.
        //meshCollider.sharedMesh = skinnedMeshRendererBottom.sharedMesh;

        // The index of the layer we are dealing (base layer = 0).
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // TODO: Possible to have the sound clip depending on the rotation of the pivot of the chest?
        // TODO: Implement opening from the front of the chest only.
        if (Input.GetMouseButtonDown(0))
        {
            // XOR the open boolean.
            open = !open;

            // Setting the open boolean for the animator to the current open value.
            //animator.SetBool(openHash, open);
            
            // Assigning the correct clip.
            audioSource.clip = (open) ? chestOpen : chestClose;

            // Stoping the previous clip and starting the next one.
            audioSource.Stop();
            audioSource.Play();

        }

    }

}