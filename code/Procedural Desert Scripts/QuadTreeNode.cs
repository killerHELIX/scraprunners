using Sandbox;

public class QuadTreeNode
{
	public Vector2 center;
	public float size;
	public QuadTreeNode[] children;
	public TerrainChunk chunk;
}
