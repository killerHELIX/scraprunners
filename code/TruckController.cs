using System;
using Sandbox;
using static Sandbox.Component;

public sealed class TruckController : Component, IPressable
{

	public GameObject Driver;

	public bool Press( IPressable.Event e )
	{
		Transform t = WorldTransform.Add(Vector3.Forward * 100f, true);
		Driver = e.Source.GameObject;
		// Driver.WorldTransform = t;
		return true;
	}

	protected override void OnUpdate()
	{

		Log.Info(Driver);
		if (Driver != null)
		{
			Log.Info("Changing camera");
			// var camera = Scene.GetComponent<CameraComponent>();
			// camera.WorldTransform = WorldTransform.Add((Vector3.Up * 100f) + (Vector3.Backward * 100f), true);
			HandleDriverInput();
			var pc = Driver.GetComponent<PlayerController>();
			if (pc.IsValid()) pc.Enabled = false;
		}

	}

	private void HandleDriverInput()
	{
		if (Input.Pressed("use"))
		{
			var pc = Driver.GetComponent<PlayerController>();
			if (pc.IsValid()) pc.Enabled = true;
			Driver = null;
		}

	}
}
