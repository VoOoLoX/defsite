namespace Defsite.ECS;

class Component {
	public Entity Entity { get; set; }
	public virtual void Update(float time) { }
}
