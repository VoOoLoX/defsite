using System.Collections.Generic;

namespace Client {
	public class Scene {
		public List<Entity> Entities { get; }
		public Camera Camera { get; }

		public virtual void Init() {
		}

		public void AddEntity(Entity entity) {
			Entities.Add(entity);
		}

		public Entity GetEntity(uint id) {
			foreach (var entity in Entities) {
				if (entity.ID == id)
					return entity;
			}

			return null;
		}

		public void RemoveEntity(Entity entity) {
			Entities.Remove(entity);
		}
	}
}