using Sandbox;
using System;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
public class ProceduralDesertGeneration : Component
{
	//timer
	Stopwatch stopwatch = new Stopwatch();
	//how many quads "within" quads
	public int resolution = 1;
	//how large the in game grid is
	public int chunkSize = 256;
	private FastNoiseLite noise = new FastNoiseLite();
	Random random = new Random();

	//manually changing the scale and position of the grid
	public float xyFactor = 10f;
	public float zFactor = 100f;
	public float zTranslation = 9f;
	private Model model { get; set; }
	private Collider collider { get; set; }
	[Property]
	public GameObject shrub;
	protected override void OnStart()
	{
		base.OnStart();
		stopwatch.Start();
		noise.SetNoiseType( FastNoiseLite.NoiseType.Perlin );
		model = GenerateDesert();
		this.AddComponent<ModelRenderer>().Model = model;
		this.AddComponent<ModelCollider>().Model = model;
		stopwatch.Stop();
		Log.Info( "Time Elasped(ms): "+stopwatch.ElapsedMilliseconds );
	}

	public Model GenerateDesert()
	{
		List<Vertex> vertices = new List<Vertex>();
		List<int> indices = new List<int>();
		List<Vector3> vector3s = new List<Vector3>();
		Dictionary<Vector2,float> coordinateNoise = new Dictionary<Vector2,float>();

		int dimensionLength = chunkSize * resolution;
		float[,] noiseMatrix = new float[ dimensionLength, dimensionLength ];
		Vector3 normal = Vector3.Up;
		Vector3 tangent = Vector3.Forward;
		noise.mSeed = random.Next( 0, 2000 );

		for (int x = 0; x < dimensionLength - 1; x++ )
		{
			for ( int y = 0; y < dimensionLength - 1; y++ )
			{
				float x0 = (float)(x - dimensionLength / 2) * xyFactor / resolution;
				float y0 = (float)(y - dimensionLength / 2) * xyFactor / resolution;

				float x1 = (float)(x + 1 - dimensionLength / 2) * xyFactor / resolution;
				float y1 = (float)(y + 1 - dimensionLength / 2) * xyFactor / resolution;

				//cringe
				if( !coordinateNoise.ContainsKey( new Vector2( x, y ) ) )
				{
					coordinateNoise.Add( new Vector2( x, y ), noise.GetNoise( x, y ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x + 1, y ) ) )
				{
					coordinateNoise.Add( new Vector2( x + 1, y ), noise.GetNoise( x + 1, y ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x, y + 1 ) ) )
				{
					coordinateNoise.Add( new Vector2( x, y + 1 ), noise.GetNoise( x, y + 1 ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x + 1, y + 1 ) ) )
				{
					coordinateNoise.Add( new Vector2( x + 1, y + 1 ), noise.GetNoise( x + 1, y + 1 ) );
				}

				//quad
				Vector3 botLeft = new Vector3( x0, y0, zFactor * coordinateNoise[new Vector2( x, y )] - zTranslation );
				Vector3 botRight = new Vector3( x1, y0, zFactor * coordinateNoise[new Vector2( x + 1, y )] - zTranslation );
				Vector3 topRight = new Vector3( x1, y1, zFactor * coordinateNoise[new Vector2( x + 1, y + 1 )] - zTranslation );
				Vector3 topLeft = new Vector3( x0, y1, zFactor * coordinateNoise[new Vector2( x, y + 1 )] - zTranslation );

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

				vector3s.Add( botLeft );
				vector3s.Add( botRight );
				vector3s.Add( topRight );
				vector3s.Add( topLeft );
			}
		}

		var material = Material.Load( "materials/sand1.vmat" );
		var mesh = new Mesh( material );

		mesh.CreateVertexBuffer<Vertex>( vertices.Count, Vertex.Layout, vertices.ToArray() );
		mesh.CreateIndexBuffer(indices.Count, indices.ToArray() );
		//mesh.Bounds = BBox.FromPositionAndSize( Vector3.Zero, resolution );
		GenerateFoliage( vertices.ToArray() );

		return Model.Builder.AddMesh( mesh ) .AddCollisionMesh(vector3s.ToArray(), indices.ToArray()) .Create();
	}

	private void GenerateFoliage( Vertex[] vertices )
	{
		Transform transform = new Transform();
		int rand1 = random.Next( 300, 500 );

		for (int i = 0; i < rand1 ; i++ )
		{
			int rand2 = random.Next( 0, vertices.Length );
			transform.Position = vertices[rand2].Position;

			var shrubPrefab = shrub.Clone( transform, name: $"Shrub - {i}" );
		}
	}
}
