using System.Collections.Generic;

namespace Defsite.ECS;

class System<T> where T : Component {
	protected static List<T> components = new();

	public static void Register(T component) => components.Add(component);

	public static void Update(float time) {
		foreach(var component in components) {
			component.Update(time);
		}
	}
}