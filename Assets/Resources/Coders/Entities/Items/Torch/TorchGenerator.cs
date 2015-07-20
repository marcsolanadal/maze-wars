
public class TorchGenerator : ProceduralGenerator
{
    private string prefabPath = "Coders/Entities/Items/Torch/Torch";
    private string texturePath = "Artists/Zones/Ruins/Items/Torch/Textures";

    public TorchGenerator()
    {
        SetPrefab(prefabPath);
        GenerateMaterialList(texturePath);
    }

    public override void ApplyBlendShapes()
    {
        base.ApplyBlendShapes();
    }

}
