using System;
using System.Collections.Generic;

namespace Defsite {

	public class EntityOLD {
		[Flags]
		enum ComponentTypes {
			None = 0b0000,
			Transform = 0b0001,
			Sprite = 0b0010,
			Sound = 0b0100
		}

		static uint start_id;

		List<Component> components = new();

		public uint ID { get; private set; }

		public uint Z { get; set; }

		ComponentTypes ComponentsTypes { get; set; }

		public EntityOLD() {
			ID = start_id;
			Z = start_id;
			start_id++;
		}
		public void AddComponent<T>(T component) where T : Component {
			switch (component) {
				case Transform _:
					ComponentsTypes |= ComponentTypes.Transform;
					break;

				case SpriteComponent _:
					ComponentsTypes |= ComponentTypes.Sprite;
					break;

				case Sound _:
					ComponentsTypes |= ComponentTypes.Sprite;
					break;
			}

			components.Add(component);
		}

		public T GetComponent<T>() where T : Component {
			foreach (var c in components)
				if (c is T component)
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

		public bool HasComponent<T>() where T : Component {
			if (new Transform() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Transform);

			if (new SpriteComponent() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Sprite);

			if (new SoundComponent() is T)
				return ComponentsTypes.HasFlag(ComponentTypes.Sound);

			return false;
		}
		public void RemoveComponent<T>(T component) where T : Component {
			if (components.Contains(component))
				components.Remove(component);
		}
	}
}