using System;
using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace Defsite.Graphics;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct TexturedVertex : IVertex {
	public Vector4 Position { get; set; }
	public Vector4 Color { get; set; }
	public Vector2 TextureCoordinates { get; set; }

	public static int Size => Vector4.SizeInBytes + Vector4.SizeInBytes + Vector2.SizeInBytes;

	public int SizeInBytes => Size;

	public float this[int index] => index switch {
		0 => Position[0],
		1 => Position[1],
		2 => Position[2],
		3 => Position[3],
		4 => Color[0],
		5 => Color[1],
		6 => Color[2],
		7 => Color[3],
		8 => TextureCoordinates[0],
		9 => TextureCoordinates[1],
		_ => throw new IndexOutOfRangeException("You tried to access this vertex at index: " + index),
	};

	public override bool Equals(object obj) => obj is TexturedVertex vertex && Equals(vertex);

	bool Equals(TexturedVertex other) => Position == other.Position && Color == other.Color && TextureCoordinates == other.TextureCoordinates;

	public override int GetHashCode() => base.GetHashCode();

	public static bool operator ==(TexturedVertex left, TexturedVertex right) => left.Equals(right);

	public static bool operator !=(TexturedVertex left, TexturedVertex right) => !(left == right);
}
