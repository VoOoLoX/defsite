using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Defsite {

	public enum VertexAttributeType {
		Matrix2,
		Matrix3,
		Matrix4,
		Vector2,
		Vector3,
		Vector4,
		Vector4b,
		Color,
		Bool,
		Int,
		Float
	}

	public class BufferLayout {
		public List<VertexAttribute> Attributes { get; private set; } = new();
		public int Stride { get; private set; }

		public BufferLayout(List<VertexAttribute> attributes) {
			var offset = 0;

			foreach (var attribute in attributes) {
				attribute.ComponentCount = GetComponentCount(attribute.Type);
				attribute.Offset = offset;

				Stride += GetComponentSize(attribute.Type);
				offset += GetComponentSize(attribute.Type);
				Attributes.Add(attribute);
			}
		}

		public int GetComponentCount(VertexAttributeType type) {
			return type switch {
				VertexAttributeType.Matrix2 => 2 * 2,
				VertexAttributeType.Matrix3 => 3 * 3,
				VertexAttributeType.Matrix4 => 4 * 4,
				VertexAttributeType.Bool or VertexAttributeType.Int or VertexAttributeType.Float => 1,
				VertexAttributeType.Vector2 => 2,
				VertexAttributeType.Vector3 => 3,
				VertexAttributeType.Vector4 or VertexAttributeType.Vector4b or VertexAttributeType.Color => 4,
				_ => 0,
			};
		}

		int GetComponentSize(VertexAttributeType type) {
			return type switch {
				VertexAttributeType.Matrix2 => 2 * 2 * sizeof(float),
				VertexAttributeType.Matrix3 => 3 * 3 * sizeof(float),
				VertexAttributeType.Matrix4 => 4 * 4 * sizeof(float),
				VertexAttributeType.Bool => 1,
				VertexAttributeType.Int or VertexAttributeType.Float => sizeof(float),
				VertexAttributeType.Vector2 => 2 * sizeof(float),
				VertexAttributeType.Vector3 => 3 * sizeof(float),
				VertexAttributeType.Vector4b => 4 * sizeof(byte),
				VertexAttributeType.Vector4 or VertexAttributeType.Color => 4 * sizeof(float),
				_ => 0,
			};
		}
	}

	public class VertexAttribute {
		public int ComponentCount { get; set; }
		public int ID { get; }
		public bool Normalized { get; private set; }
		public int Offset { get; set; }
		public VertexAttributeType Type { get; set; }
		public VertexAttribute(int id, VertexAttributeType type, bool normalized = false) {
			ID = id;
			Type = type;
			Normalized = normalized;
		}

		public VertexAttribPointerType GetVertexAttribPointerType() {
			return Type switch {
				VertexAttributeType.Matrix2 or VertexAttributeType.Matrix3 or VertexAttributeType.Matrix4 or VertexAttributeType.Float or VertexAttributeType.Vector2 or VertexAttributeType.Vector3 or VertexAttributeType.Vector4 or VertexAttributeType.Color => VertexAttribPointerType.Float,
				VertexAttributeType.Bool or VertexAttributeType.Int => VertexAttribPointerType.Int,
				VertexAttributeType.Vector4b => VertexAttribPointerType.UnsignedByte,
				_ => VertexAttribPointerType.Int,
			};
		}
	}
}