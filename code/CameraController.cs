public sealed class CameraController : Component
{
	protected override void OnUpdate()
	{
		if ( Input.Down( "jump" ) )
		{
			WorldPosition += Vector3.Forward * Time.Delta;
		}
	}
}
