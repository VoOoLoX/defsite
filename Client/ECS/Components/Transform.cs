using OpenTK;

namespace Client {
	public class Transform : IComponent {
		Matrix4 matrix = Matrix4.Identity;
		//Add different constructors e.g. Transform(position), Transform(position, rotation) etc.

		Vector3 position;
		Vector3 rotation;
		Vector3 scale;

		public Transform() {
			Position = Vector3.Zero;
			Rotation = Vector3.Zero;
			Scale = Vector3.One;
		}

		public Vector3 Position {
			get => position;
			set => MoveTo(value.X, value.Y, value.Z);
		}

		public Vector3 Rotation {
			get => rotation;
			set => RotateTo(value.X, value.Y, value.Z);
		}

		public Vector3 Scale {
			get => scale;
			set => ScaleTo(value.X, value.Y, value.Z);
		}

		public Matrix4 GetMatrix() {
			return matrix;
		}

		public void SetMatrix(Matrix4 mat) {
			matrix = mat;
		}

		public void MoveTo(float x, float y, float z) {
			var pos = new Vector3(x, y, z);
			pos -= position;
			position = pos;
			matrix *= Matrix4.CreateTranslation(pos);
		}

		public void MoveBy(float x, float y, float z) {
			var pos = new Vector3(x, y, z);
			position += pos;
			matrix *= Matrix4.CreateTranslation(pos);
		}

		public void MoveBy(Vector3 postion_vector) {
			var pos = postion_vector;
			position += pos;
			matrix *= Matrix4.CreateTranslation(pos);
		}

		public void RotateTo(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var rot = new Vector3(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			rot -= rotation;
			rotation = rot;

			matrix *= Matrix4.CreateRotationX(rot.X);
			matrix *= Matrix4.CreateRotationY(rot.Y);
			matrix *= Matrix4.CreateRotationZ(rot.Z);

			matrix *= Matrix4.CreateTranslation(p);
		}

		public void RotateBy(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var rot = new Vector3(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			rotation += rot;

			matrix *= Matrix4.CreateRotationX(rot.X);
			matrix *= Matrix4.CreateRotationY(rot.Y);
			matrix *= Matrix4.CreateRotationZ(rot.Z);

			matrix *= Matrix4.CreateTranslation(p);
		}

		public void ScaleTo(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var sc = new Vector3(x, y, z);
			sc += scale;
			scale = sc;

			matrix *= Matrix4.CreateScale(sc);

			matrix *= Matrix4.CreateTranslation(p);
		}

		public void ScaleBy(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var sc = new Vector3(x, y, z);
			scale += sc;

			matrix *= Matrix4.CreateScale(sc);

			matrix *= Matrix4.CreateTranslation(p);
		}
	}
}