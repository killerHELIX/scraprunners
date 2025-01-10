using Sandbox;
using System;

public class QuadTreeNode
{
	public Vector2 center;
	public float size;
	public QuadTreeNode[] children;
	public TerrainChunk chunk;

	public bool isLeaf => children == null;

	public void Subdivide()
	{
		if ( !isLeaf )
		{
			return;
		}

		children = new QuadTreeNode[4];
		float halfsize = size * 0.5f;

		children[0] = new QuadTreeNode { center = center + new Vector2( -halfsize, halfsize ), size = halfsize };
		children[1] = new QuadTreeNode { center = center + new Vector2( halfsize, halfsize ), size = halfsize };
		children[2] = new QuadTreeNode { center = center + new Vector2( -halfsize, -halfsize ), size = halfsize };
		children[3] = new QuadTreeNode { center = center + new Vector2( halfsize, -halfsize ), size = halfsize };
	}
	public void Merge()
	{
		if ( isLeaf )
		{
			return;
		}

		for( int i = 0; i < children.Length; i++  )
		{
			children[i] = null;
		}

		children = null;
	}
}
