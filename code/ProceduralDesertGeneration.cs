using Sandbox;
public class ProceduralDesertGeneration : Component
{
	public int resolution = 100;
	private Model model { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
		model = GenerateDesert();
		this.AddComponent<ModelRenderer>().Model = model;
	}

	public Model GenerateDesert()
	{
		List<Vertex> vertices = new List<Vertex>();
		List<int> indices = new List<int>();

		Vector3 normal = Vector3.Up;
		Vector3 tangent = Vector3.Forward;


		for (int x = -resolution; x < resolution; x++ )
		{
			for ( int y = -resolution; y < resolution; y++ )
			{
				float x0 = (float)x / 2f;
				float y0 = (float)y / 2f;
				float z0 = 0f;

				float x1 = (float)(x + 1) / 2f;
				float y1 = (float)(y + 1) / 2f;

				//quad
				Vector3 botLeft = new Vector3(x0, y0, z0);
				Vector3 botRight = new Vector3(x1, y0, z0);
				Vector3 topRight = new Vector3(x1, y1, z0);
				Vector3 topLeft = new Vector3(x0, y1, z0);

				// uv corners
				Vector4 uvBotLeft = new Vector4(0f, 0f, 0f, 0f);
				Vector4 uvBotRight = new Vector4(2f, 0f, 0f, 0f );
				Vector4 uvTopRight = new Vector4( 2f, 2f, 0f, 0f );
				Vector4 uvTopLeft = new Vector4(0f, 2f, 0f, 0f );

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
		var mesh = new Mesh( material );

		mesh.CreateVertexBuffer<Vertex>( vertices.Count, Vertex.Layout, vertices.ToArray() );
		mesh.CreateIndexBuffer(indices.Count, indices.ToArray() );
		mesh.Bounds = BBox.FromPositionAndSize( Vector3.Zero, resolution );

		return Model.Builder.AddMesh( mesh ) .Create();
	}
}
