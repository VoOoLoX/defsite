using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public abstract class Scene {
		List<Entity> Entities { get; } = new List<Entity>();
		protected List<Entity> Controls { get; set; } = new List<Entity>();

		protected static PerspectiveCamera Camera { get; set; } = new PerspectiveCamera(new Vector3(0, 0, 1), Quaternion.Identity);

		SpriteRenderer SpriteRenderer { get; set; } = new SpriteRenderer(Camera);
		UIRenderer UIRenderer { get; set; } = new UIRenderer();

		public void AddEntity(Entity entity) {
			Entities.Add(entity);
		}

		public void RemoveEntity(Entity entity) {
			Entities.Remove(entity);
		}

		public Entity GetEntity(uint id) {
			foreach (var entity in Entities) {
				if (entity.ID == id)
					return entity;
			}

			return null;
		}

		public Entity GetEntity(Entity e) {
			foreach (var entity in Entities) {
				if (entity == e)
					return entity;
			}

			return null;
		}

		void SortEntities() {
			if (Entities.Count > 1)
				Entities.Sort((a, b) => a.Z.CompareTo(b.Z));
		}

		public virtual void Update(float time) { }

		public virtual void Render(float time) {
			SortEntities();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			SpriteRenderer.Render(Entities);
			UIRenderer.Render(Controls);
		}

		protected Vector3 ScreenToWorld(Point point) {
			var view_projection = Camera.GetComponent<Transform>().Matrix * Camera.ProjectionMatrix;

			var px = 2.0f * ((float) point.X / Window.Width) - 1.0f;

			var py = 1.0f - 2.0f * ((float) point.Y / Window.Height);

			var pz = 2.0f * Camera.GetComponent<Transform>().Position.Z - 1.0f;

			var pos = new Vector4(px, py, pz, 1);

			var res = Vector4.Transform(pos, view_projection);

			res.W = 1.0f / res.W;


			res.X *= res.W;
			res.Y *= res.W;
			res.Z *= res.W;

//			res.X *= Window.ClientWidth;
//			res.Y *= Window.ClientHeight;

			return res.Xyz;
		}
	}
}