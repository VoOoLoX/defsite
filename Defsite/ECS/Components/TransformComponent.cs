using Defsite.ECS.Systems;

namespace Defsite.ECS.Components;

class TransformComponent : Component {
	public TransformComponent() => TransformSystem.Register(this);
}
