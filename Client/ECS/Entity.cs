using System;
using System.Collections.Generic;

namespace Client {
	public class Entity {
		static uint start_id = 0;

		List<Component> components = new List<Component>();

		public Entity() {
			ID = start_id;
			Z = start_id;
			start_id++;
		}

		ComponentTypes ComponentsTypes { get; set; }

		public uint ID { get; private set; }
		public uint Z { get; set; }

		public bool HasComponent<T>() where T : Component {
			if (new Transform() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Transform);

			if (new Sprite() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Sprite);

			if (new Sound() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Sound);

			return false;
		}

		public void AddComponent<T>(T component) where T : Component {
			switch (component) {
				case Transform _:
					ComponentsTypes |= ComponentTypes.Transform;
					break;
				case Sprite _:
					ComponentsTypes |= ComponentTypes.Sprite;
					break;
				case Sound _:
					ComponentsTypes |= ComponentTypes.Sprite;
					break;
			}

			components.Add(component);
		}

		public void RemoveComponent<T>(T component) where T : Component {
			if (components.Contains(component))
				components.Remove(component);
		}

		public T GetComponent<T>() where T : Component {
			foreach (var c in components)
				if (c is T component)
					return component;

			return default;
		}

		public T GetComponent<T>(string name) where T : Component {
			foreach (var c in components)
				if (c is T component && component.Name == name)
					return component;

			return default;
		}

		public IEnumerable<T> GetComponents<T>() where T : Component {
			var result = new List<T>();
			foreach (var c in components)
				if (c is T component)
					result.Add(component);

			return result;
		}

		[Flags]
		enum ComponentTypes {
			None = 0b0000,
			Transform = 0b0001,
			Sprite = 0b0010,
			Sound = 0b0100
		}
	}
}