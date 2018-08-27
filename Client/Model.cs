using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public abstract class Model {
		Matrix4 model_matrix = Matrix4.Identity;
		public Matrix4 ModelMatrix { get => model_matrix; set => model_matrix = value; }
		public abstract Shader Shader { get; }
		public abstract VertextArray VA { get; }
		public abstract IndexBuffer IB { get; }
		public abstract Texture Texture { get; }

		public virtual void PreDraw() { }
		public virtual void Update(double delta_time) { }

		public void Move(Vector3 move_vector) => ModelMatrix *= Matrix4.CreateTranslation(move_vector.X, move_vector.Y, move_vector.Z);
		public void Move(Vector2 move_vector) => ModelMatrix *= Matrix4.CreateTranslation(move_vector.X, move_vector.Y, 0);
		public void Move(float x, float y, float z) => ModelMatrix *= Matrix4.CreateTranslation(x, y, z);
		public void Move(float x, float y) => ModelMatrix *= Matrix4.CreateTranslation(x, y, 0);
		public void Scale(float scale_factor) => ModelMatrix *= Matrix4.CreateScale(scale_factor, scale_factor, 0);
		public void Scale(float scale_factor_x, float scale_factor_y) => ModelMatrix *= Matrix4.CreateScale(scale_factor_x, scale_factor_y, 0);
	}
}
