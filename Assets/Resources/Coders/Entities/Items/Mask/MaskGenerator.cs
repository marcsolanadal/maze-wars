
public class MaskGenerator : ProceduralGenerator
{
    private string prefabPath = "Coders/Entities/Items/Mask/Mask";
    private string texturePath = "Artists/Zones/Ruins/Items/Mask/Textures";

    public MaskGenerator()
    {
        SetPrefab(prefabPath);
        GenerateMaterialList(texturePath);
    }

    public override void ApplyBlendShapes()
    {
        base.ApplyBlendShapes();
    }

}