using Sandbox;
using System.ComponentModel.Design;
public class ProceduralDesertGeneration : Component
{
	public int resolution = 5;
	public int size = 10;
	private FastNoiseLite noise = new FastNoiseLite();
	public float frequency = 1000.0f;
	public float wavelength;
	public float xyFactor = 100f;
	public float zFactor = 100f;
	public float zTranslation = 9f;
	private Model model { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
		wavelength = 1.0f / frequency;
		noise.SetNoiseType( FastNoiseLite.NoiseType.Perlin );
		model = GenerateDesert();
		this.AddComponent<ModelRenderer>().Model = model;
		this.AddComponent<ModelCollider>().Model = model;
	}

	public Model GenerateDesert()
	{
		List<Vertex> vertices = new List<Vertex>();
		List<int> indices = new List<int>();
		int dimensionLength = size * resolution;
		float[,] noiseMatrix = new float[ dimensionLength, dimensionLength ];
		Vector3 normal = Vector3.Up;
		Vector3 tangent = Vector3.Forward;


		for (int x = 0; x < size * resolution; x++ )
		{
			for ( int y = 0; y < size * resolution; y++ )
			{
				noiseMatrix[x, y] = noise.GetNoise((float)x, (float) y);
			}
		}

		for (int x = 0; x < dimensionLength - 1; x++ )
		{
			for ( int y = 0; y < dimensionLength - 1; y++ )
			{
				float x0 = (float)(x - dimensionLength/2)* xyFactor / resolution;
				float y0 = (float)(y - dimensionLength / 2) * xyFactor / resolution;

				float x1 = (float)(x + 1) * xyFactor / resolution;
				float y1 = (float)(y + 1) * xyFactor / resolution;

				//quad
				Vector3 botLeft = new Vector3( x0, y0, zFactor * noiseMatrix[x, y] - zTranslation);
				Vector3 botRight = new Vector3( x1, y0, zFactor * noiseMatrix[x + 1, y] - zTranslation );
				Vector3 topRight = new Vector3( x1, y1, zFactor * noiseMatrix[x + 1, y + 1] - zTranslation );
				Vector3 topLeft = new Vector3( x0, y1, zFactor * noiseMatrix[x, y + 1] - zTranslation );
				Log.Info( botLeft.z );
				// uv corners
				Vector4 uvBotLeft = new Vector4( 0f, 0f, 0f, 0f );
				Vector4 uvBotRight = new Vector4( 2f, 0f, 0f, 0f );
				Vector4 uvTopRight = new Vector4( 2f, 2f, 0f, 0f );
				Vector4 uvTopLeft = new Vector4( 0f, 2f, 0f, 0f );

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

		var material = Material.Load( "materials/sand1.vmat" );
		var mesh = new Mesh( material );

		mesh.CreateVertexBuffer<Vertex>( vertices.Count, Vertex.Layout, vertices.ToArray() );
		mesh.CreateIndexBuffer(indices.Count, indices.ToArray() );
		mesh.Bounds = BBox.FromPositionAndSize( Vector3.Zero, resolution );

		return Model.Builder.AddMesh( mesh ) .Create();
	}
}
