using Sandbox;
using System;

public sealed class CameraRotate : Component
{
	public float speed = 0.5f;
	public float amplitude = 500f;
	public Vector3 origin = new Vector3( 0f, 0f, 0f );
	protected override void OnUpdate()
	{
		base.OnUpdate();
		this.GameObject.WorldPosition = amplitude * new Vector3( MathF.Sin( Time.Now * speed ), 0f, MathF.Cos( Time.Now * speed ) );
		this.GameObject.WorldRotation = Rotation.LookAt(Vector3.Direction(this.GameObject.WorldPosition, origin), Vector3.Left);
		//this.GameObject.LocalRotation *= Rotation.FromYaw( 0.5f );
	}
}
