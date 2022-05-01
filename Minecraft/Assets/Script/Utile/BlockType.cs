public enum EBlockType { Basic, Soil, Stone, Wood }
public class BlockType
{
    public ushort[] textureAtlases = new ushort[6];
    public string blockName;
    public bool isSolid;
    public bool renderNeighborFaces;
    public float hardness;
    public EBlockType type;
}
public enum EItemType { Basic, Sword, Pick, Axe, Shovel}
public class ItemType
{
    public ushort textureAtlases;
    public string itemName;
    public float value;
    public EItemType type;
}