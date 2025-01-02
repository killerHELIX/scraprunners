using Sandbox;
using System.ComponentModel.Design;
public class ProceduralDesertGeneration : Component
{
	public int resolution = 1;
	public int size = 500;
	private FastNoiseLite noise = new FastNoiseLite();
	public float frequency = 10.0f;
	public float wavelength;
	public float factor = 10f;
	public float factor2 = 100f;
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
		float[,] noiseMatrix = new float[2 * size * resolution, 2 * size * resolution];
		//noiseMatrix = new float[size * resolution, size * resolution];
		Vector3 normal = Vector3.Up;
		Vector3 tangent = Vector3.Forward;


		for (int x = -size * resolution; x < size * resolution; x++ )
		{
			for ( int y = -size * resolution; y < size * resolution; y++ )
			{
				float x0 = (float)x * factor / resolution;
				float y0 = (float)y * factor / resolution;
				noiseMatrix[x+ (size * resolution), y+ (size * resolution)] = noise.GetNoise(x0 / wavelength, y0 / wavelength);
				//Log.Info(noiseMatrix[x, y] );
				float z0 = factor2 * noiseMatrix[x + (size * resolution), y + (size * resolution)];
				//float z0 = 0.0f;

				float x1 = (float)(x + 1) * factor / resolution;
				float y1 = (float)(y + 1) * factor / resolution;
				float z1 = factor2 * noise.GetNoise( x1 / wavelength, y1 / wavelength ); ;

				//quad
				Vector3 botLeft = new Vector3(x0, y0, z0);
				Vector3 botRight = new Vector3(x1, y0, z1);
				Vector3 topRight = new Vector3(x1, y1, z0);
				Vector3 topLeft = new Vector3(x0, y1, z1);

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
		var material = Material.Load( "materials/sand1.vmat" );
		var mesh = new Mesh( material );

		mesh.CreateVertexBuffer<Vertex>( vertices.Count, Vertex.Layout, vertices.ToArray() );
		mesh.CreateIndexBuffer(indices.Count, indices.ToArray() );
		mesh.Bounds = BBox.FromPositionAndSize( Vector3.Zero, resolution );

		return Model.Builder.AddMesh( mesh ) .Create();
	}
}
