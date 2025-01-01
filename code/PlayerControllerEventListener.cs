using Sandbox;

public sealed class PlayerControllerEventListener : Component, PlayerController.IEvents
{
	protected override void OnUpdate()
	{

	}

	Component PlayerController.IEvents.GetUsableComponent(GameObject go)
        {
			// Log.Info(go);
            return null;
        }

	void PlayerController.IEvents.StartPressing(Component target)
	{
			// Log.Info(target);
	}

	void PlayerController.IEvents.StopPressing(Component target)
	{
			// Log.Info(target);
	}
}
