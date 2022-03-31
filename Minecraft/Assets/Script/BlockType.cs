public enum BlockType
{
    Air, Stone, Earth , Tree
}
public struct Block
{
    public ushort blockID;
    public BlockType blockType;
    public float transparency;
}
