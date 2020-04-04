using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Client {

	public enum VertexAttributeType {
		Matrix2,
		Matrix3,
		Matrix4,
		Vector2,
		Vector3,
		Vector4,
		Color,
		Bool,
		Int,
		Float
	}

	public class BufferLayout {
		public List<VertexAttribute> Attributes { get; private set; } = new List<VertexAttribute>();
		public int Stride { get; private set; } = 0;

		public BufferLayout(List<VertexAttribute> attributes) {
			int offset = 0;

			foreach (var attribute in attributes) {
				attribute.ComponentCount = GetComponentCount(attribute.Type);
				attribute.Offset = offset;

				Stride += GetComponentSize(attribute.Type);
				offset += GetComponentSize(attribute.Type);
				Attributes.Add(attribute);
			}
		}

		public int GetComponentCount(VertexAttributeType type) {
			switch (type) {
				case VertexAttributeType.Matrix2:
					return 2 * 2;

				case VertexAttributeType.Matrix3:
					return 3 * 3;

				case VertexAttributeType.Matrix4:
					return 4 * 4;

				case VertexAttributeType.Bool:
				case VertexAttributeType.Int:
				case VertexAttributeType.Float:
					return 1;

				case VertexAttributeType.Vector2:
					return 2;

				case VertexAttributeType.Vector3:
					return 3;

				case VertexAttributeType.Vector4:
				case VertexAttributeType.Color:
					return 4;

				default:
					return 0;
			}
		}

		int GetComponentSize(VertexAttributeType type) {
			switch (type) {
				case VertexAttributeType.Matrix2:
					return 2 * 2 * sizeof(float);

				case VertexAttributeType.Matrix3:
					return 3 * 3 * sizeof(float);

				case VertexAttributeType.Matrix4:
					return 4 * 4 * sizeof(float);

				case VertexAttributeType.Bool:
					return 1;

				case VertexAttributeType.Int:
				case VertexAttributeType.Float:
					return sizeof(float);

				case VertexAttributeType.Vector2:
					return 2 * sizeof(float);

				case VertexAttributeType.Vector3:
					return 3 * sizeof(float);

				case VertexAttributeType.Vector4:
				case VertexAttributeType.Color:
					return 4 * sizeof(float);

				default:
					return 0;
			}
		}
	}

	public class VertexAttribute {
		public int ComponentCount { get; set; }
		public int ID { get; }
		public string Name { get; }
		public bool Normalized { get; private set; } = false;
		public int Offset { get; set; }
		public VertexAttributeType Type { get; set; }
		public VertexAttribute(int id, string name, VertexAttributeType type, bool normalized = false) {
			ID = id;
			Name = name;
			Type = type;
			Normalized = normalized;
		}

		public VertexAttribPointerType GetVertexAttribPointerType() {
			switch (Type) {
				case VertexAttributeType.Matrix2:
				case VertexAttributeType.Matrix3:
				case VertexAttributeType.Matrix4:
				case VertexAttributeType.Float:
				case VertexAttributeType.Vector2:
				case VertexAttributeType.Vector3:
				case VertexAttributeType.Vector4:
				case VertexAttributeType.Color:
					return VertexAttribPointerType.Float;

				case VertexAttributeType.Bool:
				case VertexAttributeType.Int:
					return VertexAttribPointerType.Int;

				default:
					return VertexAttribPointerType.Int;
			}
		}
	}
}