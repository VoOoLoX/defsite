using System.Collections.Generic;

namespace Client {
	public class Entity {
		static ulong start_id = 0;
		public ulong ID { get; private set; }
		
		List<IComponent> components = new List<IComponent>();
		
		public Entity() {
			ID = start_id++;
		}

		public bool ContainsComponent<T>() where T : IComponent {
			foreach (var c in components) 
				if (c is T ) return true;
			return false;
		}

		public void AddComponent<T>(T component) where T : IComponent {
			foreach (var c in components) 
				if (c is T ) return;
			
			components.Add(component);
		}

		public T GetComponent<T>() where T : IComponent {
			foreach (var c in components) 
				if (c is T component)
					return component;
			
			return default;
		}
		
	}
}