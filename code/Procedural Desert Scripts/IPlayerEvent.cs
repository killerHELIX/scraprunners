using Sandbox;

public interface IPlayerEvent : ISceneEvent<IPlayerEvent>
{
	void OnChunkChange( CameraComponent camera ) {}
}
