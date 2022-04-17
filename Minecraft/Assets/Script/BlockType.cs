public enum Type { Basic, Soil, Stone, Wood }
public class BlockType
{
    public ushort[] textureAtlases = new ushort[6];
    public string blockName;
    public bool isSolid;
    public bool renderNeighborFaces;
    public float hardness;
    public Type type;

}