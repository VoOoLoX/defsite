using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace Client {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex {
		public Vector3 Position { get; set; }
		public Vector4 Color { get; set; }
		public Vector2 TextureCoordinates { get; set; }
		public Vector3 Normal { get; set; }

		public static int SizeInBytes => Vector3.SizeInBytes + Vector4.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes;

		public float this[int index] {
			get {
				return index switch
				{
					0 => Position[0],
					1 => Position[1],
					2 => Position[2],
					3 => Color[0],
					4 => Color[1],
					5 => Color[2],
					6 => Color[3],
					7 => TextureCoordinates[0],
					8 => TextureCoordinates[1],
					9 => Normal[0],
					10 => Normal[1],
					11 => Normal[2],
					_ => throw new IndexOutOfRangeException("You tried to access this vertex at index: " + index),
				};
			}
		}
	}
}