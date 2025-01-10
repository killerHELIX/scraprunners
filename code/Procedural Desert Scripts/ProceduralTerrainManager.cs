using Sandbox;
using System;
using System.Threading.Tasks;

public class ProceduralTerrainManager : Component, IPlayerEvent
{
	[Property, Feature( "Procedural Settings" )] public Vector2 gridPosition;
	[Property, Feature( "Procedural Settings" )] public int chunkSize;
	[Property, Feature( "Procedural Settings" )] public int totalAreaWidth;
	[Property, Feature( "Procedural Settings" )] public int totalAreaHeight;
	[Property, Feature( "Procedural Settings" )] public int lod;
	[Property, Feature( "Procedural Settings" )] public float gridFactor;
	[Property, Feature( "Procedural Settings" )] public float heightFactor;
	[Property, Feature( "Procedural Settings" )] public float heightTranslation;
	[Property, Feature( "Procedural Settings" )] public GameObject chunkObject;
	[Property, Feature( "Procedural Settings" )] public GameObject player;

	[Property, Feature( "Material Array" )] public List<Material> materials;

	Random random = new Random();
	public int seed;
	private List<GameObject> chunkObjects = new List<GameObject>();
	public TerrainChunk terrainChunk = new TerrainChunk();
	public GameObject defaultChunkObject = new GameObject();
	public GameObject currentChunkObject = null;

	protected override void OnStart()
	{
		base.OnStart();
		
		seed = random.Next( 0, 2000 );
		SpawnChunkObjects();
		CheckForChunkChange();
	}

	private void SpawnChunkObjects()
	{
		for(int x = -4; x < totalAreaWidth + 4; x++ )
		{
			for (int y = -4; y < totalAreaHeight + 4; y++ )
			{
				Transform transform = new Transform();
				transform.Position = new Vector3( (float)x, (float)y, 0f );
				GameObject newChunkObject = chunkObject.Clone(transform, name: $"Chunk Object: {x}, {y}" );
				chunkObjects.Add(newChunkObject);
			}
		}
		defaultChunkObject = (GameObject)Scene.Directory.FindByName( "Chunk Object: 0, 0" );
	}

	private async Task CheckForChunkChange()
	{
		await Task.DelaySeconds( 3f );
		currentChunkObject = defaultChunkObject;
		CheckForChunkChange();
	}

}

