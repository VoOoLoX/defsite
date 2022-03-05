using System;
using System.Collections.Generic;

namespace Defsite.ECS;

class Entity {
	static int id = 0;
	public int ID { get; set; }
	public Guid GUID { get; set; }

	public List<Component> Components { get; private set; } = new();

	public Entity() {
		ID = id++;
		GUID = Guid.NewGuid();
	}

	public void AddComponent(Component component) {
		Components.Add(component);
		component.Entity = this;
	}

	public T GetComponent<T>() where T : Component {
		foreach(var component in Components) {
			if(component.GetType().Equals(typeof(T))) {
				return (T)component;
			}
		}

		return null;
	}
}
