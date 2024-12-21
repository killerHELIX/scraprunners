using Sandbox;

[Library( "example_terrain_quad" )]
public class ProceduralDesertGeneration : Component
{
	public int chunkDimension = 16;
	public int resolution = 10;
	//public SceneObject quad;
	public GameObject quad = new GameObject();
	public ModelBuilder model = new ModelBuilder();
	public SceneWorld world = new SceneWorld();

	protected override void OnStart()
	{
		base.OnStart();
		GenerateDesert();
	}

	public void GenerateDesert()
	{
		List<Vertex> vertices = new List<Vertex>();
		//Vertex[] vertices = new Vertex[resolution * resolution];

		Vector3 position = new Vector3( 0f, 0f, 0f );
		Vector3 normal = new Vector3( 0f, 0f, 1f );
		Vector3 tangent = new Vector3( 0f, 0f, 0f );
		Vector4 uv = new Vector4( 0f, 0f, 0f, 0f );

		List<int> indices = new List<int>();

		for (int x = 0; x < resolution; x++ )
		{
			for ( int y = 0; y < resolution; y++ )
			{
				float x0 = (float)x / resolution;
				float y0 = (float)y / resolution;
				float z0 = 0f;//(float)y / resolution;

				float x1 = (float)(x + 1) / resolution;
				float y1 = (float)(y + 1) / resolution;
				float z1 = 0f;// (float)(y + 1) / resolution;

				//quad
				Vector3 botLeft = new Vector3(x0, y0, z0);
				Vector3 botRight = new Vector3(x1, y0, z0);
				Vector3 topRight = new Vector3(x1, y1, z0);
				Vector3 topLeft = new Vector3(x0, y1, z0);

				// uv corners
				Vector2 uvBotLeft = new Vector2(0f, 0f);
				Vector2 uvBotRight = new Vector2(1f, 0f);
				Vector2 uvTopRight = new Vector2( 1f, 1f );
				Vector2 uvTopLeft = new Vector2(0f, 1f);

				int baseVertexIndex = vertices.Count;

				vertices.Add( new Vertex( botLeft, normal, tangent, uvBotLeft ) );
				vertices.Add( new Vertex( botRight, normal, tangent, uvBotRight ) );
				vertices.Add( new Vertex( topRight, normal, tangent, uvTopRight ) );
				vertices.Add( new Vertex( topLeft, normal, tangent, uvTopLeft ) );

				indices.Add( baseVertexIndex + 0 );
				indices.Add( baseVertexIndex + 1 );
				indices.Add( baseVertexIndex + 2 );
				indices.Add( baseVertexIndex + 0 );
				indices.Add( baseVertexIndex + 2 );
				indices.Add( baseVertexIndex + 3 );
			}
		}

		var material = Material.Load( "materials/hotpink.vmat" );
		var mesh = new Mesh( material, MeshPrimitiveType.Triangles );

		mesh.CreateVertexBuffer<Vertex>( vertices.Count, Vertex.Layout, vertices.ToArray() );
		mesh.CreateIndexBuffer(indices.Count, indices.ToArray() );
		mesh.Bounds = BBox.FromPositionAndSize( Vector3.Zero, 100f );

		Model model = Model.Builder.AddMesh( mesh ) .Create();
		quad.AddComponent<ModelRenderer>().Model = model;
		quad.Name = "Quad";
	}
}
