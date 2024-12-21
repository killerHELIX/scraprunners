using Sandbox;
using Sandbox.Sdf;

public sealed class ProceduralQuadGenerator : Component
{

	public GameObject go;
	protected override void OnStart()
	{
		base.OnStart();
		GenerateQuad();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		Rotate();
	}

	private void GenerateQuad()
	{
		var sdfWorld = new Sdf2DWorld();
		var quad = new CircleSdf( new Vector2( 0f, 0f ), 2f );

		var material = ResourceLibrary.Get<Sdf2DLayer>( "layers/sdf2d/checkerboard.sdflayer" );

		sdfWorld.AddAsync( quad, material );
	}

	private void Rotate()
	{
		go = this.GameObject;
		go.LocalRotation *= Rotation.FromYaw( 0.5f );
		go.LocalRotation *= Rotation.FromRoll( 0.5f );
	}
}
