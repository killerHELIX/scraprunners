using System;

public class TerrainChunk : Component
{
	//public Material material { get; private set; }
	public Vector2 gridPosition { get; private set; }
	public FastNoiseLite noise = new FastNoiseLite();
	public int lod { get; private set; }
	public int chunkSize { get; private set; }
	public int seed { get; private set; }

	private Model model = null;

	//manually changing the scale and position of the grid
	public float gridFactor { get; private set; }
	public float heightFactor { get; private set; }
	public float heightTranslation { get; private set; }

	//materials
	public List<Material> materials { get; private set; }

	protected override void OnStart()
	{
		base.OnStart();
		Initialize();
	}

	public void Initialize()
	{
		GameObject terrainManager = (GameObject)Scene.Directory.FindByName( "ProceduralTerrainManager" );
		gridPosition = this.WorldPosition;
		chunkSize = terrainManager.GetComponent<ProceduralTerrainManager>().chunkSize;
		lod = terrainManager.GetComponent<ProceduralTerrainManager>().lod;
		seed = terrainManager.GetComponent<ProceduralTerrainManager>().seed;
		gridFactor = terrainManager.GetComponent<ProceduralTerrainManager>().gridFactor;
		heightFactor = terrainManager.GetComponent<ProceduralTerrainManager>().heightFactor;
		heightTranslation = terrainManager.GetComponent<ProceduralTerrainManager>().heightTranslation;
		materials = terrainManager.GetComponent<ProceduralTerrainManager>().materials;

		noise.SetNoiseType( FastNoiseLite.NoiseType.Perlin );
		noise.mSeed = seed;
		//GenerateTerrainChunk();
	}

	public Model GenerateTerrainChunk()
	{
		Vertex[] vertices = new Vertex[((chunkSize - 1) * (chunkSize - 1) / lod / lod) * 4];
		//int[] indices = new int[vertices.Length + vertices.Length/2];
		List<int> indices = new List<int>();
		Vector3[] vertexPositions = new Vector3[vertices.Length];

		Dictionary<Vector2, float> coordinateNoise = new Dictionary<Vector2, float>();
		Vector3 normal = Vector3.Up;
		Vector3 tangent = Vector3.Forward;
		int baseVertexIndex = 0;

		for (int x = 0; x < chunkSize - 1; x+=lod )
		{
			for(int y = 0; y < chunkSize - 1; y+=lod )
			{
				float x0 = (gridPosition.x + x - chunkSize * 0.5f) * gridFactor;
				float y0 = (gridPosition.y + y - chunkSize * 0.5f) * gridFactor;
						   
				float x1 = (gridPosition.x + x + lod - chunkSize * 0.5f) * gridFactor;
				float y1 = (gridPosition.y + y + lod - chunkSize * 0.5f) * gridFactor;

				//cringe
				if ( !coordinateNoise.ContainsKey( new Vector2( x, y ) ) )
				{
					coordinateNoise.Add( new Vector2( x, y ), noise.GetNoise( x, y ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x + lod, y ) ) )
				{
					coordinateNoise.Add( new Vector2( x + lod, y ), noise.GetNoise( x + lod, y ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x, y + lod ) ) )
				{
					coordinateNoise.Add( new Vector2( x, y + lod ), noise.GetNoise( x, y + lod ) );
				}
				if ( !coordinateNoise.ContainsKey( new Vector2( x + lod, y + lod ) ) )
				{
					coordinateNoise.Add( new Vector2( x + lod, y + lod ), noise.GetNoise( x + lod, y + 1 ) );
				}

				//quad
				Vector3 botLeft = new Vector3( x0, y0, heightFactor * coordinateNoise[new Vector2( x, y )] - heightTranslation );
				Vector3 botRight = new Vector3( x1, y0, heightFactor * coordinateNoise[new Vector2( x + lod, y )] - heightTranslation );
				Vector3 topRight = new Vector3( x1, y1, heightFactor * coordinateNoise[new Vector2( x + lod, y + lod )] - heightTranslation );
				Vector3 topLeft = new Vector3( x0, y1, heightFactor * coordinateNoise[new Vector2( x, y + lod )] - heightTranslation );

				// uv corners
				Vector4 uvBotLeft = new Vector4( 0f, 0f, 0f, 0f );
				Vector4 uvBotRight = new Vector4( 2f, 0f, 0f, 0f );
				Vector4 uvTopRight = new Vector4( 2f, 2f, 0f, 0f );
				Vector4 uvTopLeft = new Vector4( 0f, 2f, 0f, 0f );

				vertices[baseVertexIndex] = ( new Vertex( botLeft, normal, tangent, uvBotLeft ) );
				vertices[baseVertexIndex + 1] = ( new Vertex( botRight, normal, tangent, uvBotRight ) );
				vertices[baseVertexIndex + 2] = ( new Vertex( topRight, normal, tangent, uvTopRight ) );
				vertices[baseVertexIndex + 3] = ( new Vertex( topLeft, normal, tangent, uvTopLeft ) );

				//indices[baseVertexIndex] = baseVertexIndex;
				//indices[baseVertexIndex + 1] = baseVertexIndex + 1;
				//indices[baseVertexIndex + 2] = baseVertexIndex + 2;
				//indices[baseVertexIndex + 3] = baseVertexIndex + 0;
				//indices[baseVertexIndex + 4] = baseVertexIndex + 2;
				//indices[baseVertexIndex + 5] = baseVertexIndex + 3;
				indices.Add( baseVertexIndex + 0 );
				indices.Add( baseVertexIndex + 1 );
				indices.Add( baseVertexIndex + 2 );
				indices.Add( baseVertexIndex + 0 );
				indices.Add( baseVertexIndex + 2 );
				indices.Add( baseVertexIndex + 3 );

				vertexPositions[baseVertexIndex] = botLeft ;
				vertexPositions[baseVertexIndex + 1] = botRight;
				vertexPositions[baseVertexIndex + 2] = topRight;
				vertexPositions[baseVertexIndex + 3] = topLeft;

				baseVertexIndex += 4;
			}
		}

		//Log.Info( vertices.Length );
		//Log.Info( indices.Count );
		//Log.Info( vertexPositions.Length );
		Material material = AssignMaterial();
		var mesh = new Mesh( material );

		mesh.CreateVertexBuffer<Vertex>( vertices.Length, Vertex.Layout, vertices );
		mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );

		return Model.Builder.AddMesh( mesh ).AddCollisionMesh( vertexPositions, indices.ToArray() ).Create();
	}

	private Material AssignMaterial()
	{
		Random random = new Random();

		int randomInt = random.Next( 0, materials.Count - 1 );
		return materials[ randomInt ];
	}
}
