using System.Reflection.Metadata;

public sealed class CameraController : Component
{
	private float factor = 300f;
	private float factor2 = 0.1f;
	private float mouseX = 0f;
	private float mouseY = 0f;
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Input.Down( "jump" ) )
		{
			WorldPosition += Vector3.Up * Time.Delta * factor;
		}
		if ( Input.Down( "duck" ) )
		{
			WorldPosition += Vector3.Down * Time.Delta * factor;
		}
		if ( Input.Down( "forward" ) )
		{
			WorldPosition += Vector3.Forward * Time.Delta * factor;
		}
		if ( Input.Down( "backward" ) )
		{
			WorldPosition += Vector3.Backward * Time.Delta * factor;
		}
		if ( Input.Down( "left" ) )
		{
			WorldPosition += Vector3.Left * Time.Delta * factor;
		}
		if ( Input.Down( "right" ) )
		{
			WorldPosition += Vector3.Right * Time.Delta * factor;
		}

		mouseX = Input.MouseDelta.x;
		mouseY = Input.MouseDelta.y;

		this.GameObject.WorldRotation = this.GameObject.WorldRotation.RotateAroundAxis( Vector3.Down, mouseX * factor2);
		this.GameObject.WorldRotation = this.GameObject.WorldRotation.RotateAroundAxis( Vector3.Left, mouseY * factor2 );
	}
}
