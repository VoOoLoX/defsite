using System;
using System.Collections.Generic;

namespace Defsite {

	public class Entity {

		public Guid ID { get; private set; }

		public string Name { get; set; }

		List<Component> components = new List<Component>();

		public Entity() {
			ID = Guid.NewGuid();
		}

		public void AddComponent<T>() where T : Component, new() {
			components.Add(new T());
		}

		public T GetComponent<T>() where T : Component {
			foreach (var c in components)
				if (c is T component)
					return component;

			return null;
		}

		public IEnumerable<T> GetComponents<T>() where T : Component {
			var result = new List<T>();
			foreach (var c in components)
				if (c is T component)
					result.Add(component);

			return result;
		}

		public bool HasComponent<T>() where T : Component {
			return GetComponent<T>() != null;
		}

		public void RemoveComponent<T>(T component) where T : Component {
			if (components.Contains(component))
				components.Remove(component);
		}
	}

}