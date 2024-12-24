using System.Runtime.InteropServices;
using Sandbox;

public sealed class ProceduralQuadGenerator : Component
{
	[Property] public float PlaneSize { get; set; } = 2.0f;

	private Model _model { get; set; }

	

	protected override void OnStart()
	{
		base.OnStart();

		_model = CreatePlane();

		this.AddComponent<ModelRenderer>().Model = _model;
	}

	private Model CreatePlane()
	{
		var material = Material.Load( "materials/hotpink.vmat" );
		var mesh = new Mesh( material );
		mesh.CreateVertexBuffer<Vertex>( 4, Vertex.Layout, new[]
		{
new Vertex( new Vector3( (-PlaneSize/2), (-PlaneSize/2), 0 ), Vector3.Up, Vector3.Forward, new Vector4( 0, 0, 0, 0 ) ),
new Vertex( new Vector3( (PlaneSize/2), (-PlaneSize/2), 0 ), Vector3.Up, Vector3.Forward, new Vector4( 2, 0, 0, 0 ) ),
new Vertex( new Vector3( (PlaneSize/2), (PlaneSize/2), 0 ), Vector3.Up, Vector3.Forward, new Vector4( 2, 2, 0, 0 ) ),
new Vertex( new Vector3( (-PlaneSize/2), (PlaneSize/2), 0 ), Vector3.Up, Vector3.Forward, new Vector4( 0, 2, 0, 0 ) ),
} );
		mesh.CreateIndexBuffer( 6, new[] { 0, 1, 2, 2, 3, 0 } );
		mesh.Bounds = BBox.FromPositionAndSize( 0, PlaneSize );

		return Model.Builder.AddMesh( mesh ).Create();
	}
}
