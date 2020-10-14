using OpenTK;

namespace Defsite {

	public class Transform : Component {
		Matrix4 matrix = Matrix4.Identity;

		Vector3 position;
		Quaternion rotation;
		Vector3 scale;

		public Matrix4 Matrix {
			get => GetMatrix();
			set => SetMatrix(value);
		}

		public Vector3 Position {
			get => position;
			set => MoveTo(value.X, value.Y, value.Z);
		}

		public Quaternion Rotation {
			get => rotation;
			set => RotateTo(value.X, value.Y, value.Z);
		}

		public Vector3 Scale {
			get => scale;
			set => ScaleTo(value.X, value.Y, value.Z);
		}

		public float ScaleXY {
			set => ScaleTo(value, value, Scale.Z);
		}

		public float ScaleXYZ {
			set => ScaleTo(value, value, value);
		}

		public Transform(Vector3 position = default, Quaternion rotation = default, Vector3 scale = default) {
			Position = position == default ? Vector3.Zero : position;
			Rotation = rotation == default ? Quaternion.Identity : rotation;
			Scale = scale == default ? Vector3.One : scale;
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

		public void MoveTo(float x, float y, float z) {
			var pos = new Vector3(x, y, z);
			pos -= position;
			position = pos;
			matrix *= Matrix4.CreateTranslation(pos);
		}

		public void RotateBy(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var rot = new Vector3(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			var q = new Quaternion(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			rotation += q;

			matrix *= Matrix4.CreateRotationX(rot.X);
			matrix *= Matrix4.CreateRotationY(rot.Y);
			matrix *= Matrix4.CreateRotationZ(rot.Z);

			matrix *= Matrix4.CreateTranslation(p);
		}

		public void RotateTo(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var rot = new Vector3(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			var q = new Quaternion(MathHelper.DegreesToRadians(x), MathHelper.DegreesToRadians(y), MathHelper.DegreesToRadians(z));
			q -= rotation;
			rotation = q;

			matrix *= Matrix4.CreateRotationX(rot.X);
			matrix *= Matrix4.CreateRotationY(rot.Y);
			matrix *= Matrix4.CreateRotationZ(rot.Z);

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

		public void ScaleTo(float x, float y, float z) {
			var p = position;
			matrix *= Matrix4.CreateTranslation(-position);

			var sc = new Vector3(x, y, z);
			sc += scale;
			scale = sc;

			matrix *= Matrix4.CreateScale(sc);

			matrix *= Matrix4.CreateTranslation(p);
		}

		public Matrix4 GetMatrix() {
			return matrix;
		}

		public void SetMatrix(Matrix4 mat) {
			matrix = mat;
			position = matrix.ExtractTranslation();
			rotation = matrix.ExtractRotation();
			scale = matrix.ExtractScale();
		}
	}
}