using Sandbox;
using System;

public class ProceduralTerrainManager : Component
{
	[Property, Feature( "Procedural Settings" )] public Vector2 gridPosition;
	[Property, Feature( "Procedural Settings" )] public int chunkSize;
	[Property, Feature( "Procedural Settings" )] public int lod;
	[Property, Feature( "Procedural Settings" )] public float gridFactor;
	[Property, Feature( "Procedural Settings" )] public float heightFactor;
	[Property, Feature( "Procedural Settings" )] public float heightTranslation;
	[Property, Feature( "Procedural Settings" )] public GameObject chunkObject;

	[Property, Feature( "Material Array" )] public List<Material> materials;

	Random random = new Random();
	private int seed;
	private List<Model> chunkModels = new List<Model>();
	public TerrainChunk terrainChunk = new TerrainChunk();

	protected override void OnStart()
	{
		base.OnStart();
		seed = random.Next( 0, 2000 );
		
		terrainChunk.Initialize( gridPosition, chunkSize, lod, seed, gridFactor, heightFactor, heightTranslation, materials );
		Model model = null;

		model = terrainChunk.GenerateTerrainChunk();
		chunkModels.Add( model );
		GenerateChunks();
		//chunkSpawner.Clone( transform, name: $"Chunk Spawner: {0}" );

	}
	private void GenerateChunks()
	{
		this.AddComponent<ModelRenderer>().Model = chunkModels[0];
		foreach(Model model in chunkModels )
		{
			Transform transform = new Transform();
			transform.Position = new Vector3( gridPosition, 0f );
			chunkObject.Clone( transform, name: $"Chunk Object: {0}" );
		}
	}
}

