using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Defsite {

	public abstract class Scene {
		// public PerspectiveCamera Camera { get; private set; } = new PerspectiveCamera(new Vector3(0, 0, 1), Quaternion.Identity, 60, 0.001f, 10000);
		// protected List<EntityOLD> Controls { get; set; } = new List<EntityOLD>();
		// protected SpriteRenderer SpriteRenderer { get; set; }
		// List<EntityOLD> Entities { get; } = new List<EntityOLD>();
		// UIRenderer UIRenderer { get; set; } = new UIRenderer();

		// public void AddEntity(EntityOLD entity) => Entities.Add(entity);

		// public EntityOLD GetEntity(uint id) {
		// 	foreach (var entity in Entities) {
		// 		if (entity.ID == id)
		// 			return entity;
		// 	}

		// 	return null;
		// }

		// public EntityOLD GetEntity(EntityOLD e) {
		// 	foreach (var entity in Entities) {
		// 		if (entity == e)
		// 			return entity;
		// 	}

		// 	return null;
		// }

		// public void RemoveEntity(EntityOLD entity) => Entities.Remove(entity);
		// public virtual void Render(float time) {
		// 	SortEntities();
		// 	//GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		// 	SpriteRenderer.Render(Entities);
		// 	UIRenderer.Render(Controls);
		// }

		// public virtual void Update(float time) {
		// }

		//public Point ScreenToWorld(int x, int y) {
		//	int[] viewport = new int[4];
		//	GL.GetInteger(GetPName.Viewport, viewport);
		//	Vector2 mouse;
		//	mouse.X = x;
		//	mouse.Y = viewport[3] - y;
		//	Vector4 vector = UnProject(Camera.ProjectionMatrix, Camera.GetComponent<Transform>().Matrix, new Size(viewport[2], viewport[3]), mouse);
		//	Point coords = new Point((int)vector.X, (int)vector.Y);
		//	return coords;
		//}
		//public Vector4 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse) {
		//	Vector4 vec;

		//	vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
		//	vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
		//	vec.Z = 0;
		//	vec.W = 1.0f;

		//	Matrix4 viewInv = Matrix4.Invert(view);
		//	Matrix4 projInv = Matrix4.Invert(projection);

		//	Vector4.Transform(ref vec, ref projInv, out vec);
		//	Vector4.Transform(ref vec, ref viewInv, out vec);

		//	if (vec.W > float.Epsilon || vec.W < float.Epsilon) {
		//		vec.X /= vec.W;
		//		vec.Y /= vec.W;
		//		vec.Z /= vec.W;
		//	}

		//	return vec;
		//}

		// void SortEntities() {
		// 	if (Entities.Count > 1)
		// 		Entities.Sort((a, b) => a.Z.CompareTo(b.Z));
		// }
	}
}