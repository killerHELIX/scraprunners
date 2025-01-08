using System.Reflection.Metadata;

public sealed class CameraController : Component
{
	private float factor = 30f;
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
		mouseX = Input.MouseDelta.x;
		mouseY = Input.MouseDelta.y;

		this.GameObject.WorldRotation = this.GameObject.WorldRotation.RotateAroundAxis( Vector3.Down, mouseX * factor2);
		this.GameObject.WorldRotation = this.GameObject.WorldRotation.RotateAroundAxis( Vector3.Left, mouseY * factor2 );
	}
}
