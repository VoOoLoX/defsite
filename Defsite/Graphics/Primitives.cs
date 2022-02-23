using System.Drawing;

using Defsite.Utils;

using OpenTK.Mathematics;

namespace Defsite.Graphics;

public static class Primitives {
	#region Tiles
	public static ColoredVertex[] CreateTile(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		ColoredVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z + size),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z + size),
					Color = color_vector,
				},
			};

		return quad;
	}

	public static ColoredVertex[] CreateTileCentered(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		ColoredVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y, position.Z - size / 2f),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y, position.Z - size / 2f),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y, position.Z + size / 2f),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y, position.Z + size / 2f),
					Color = color_vector,
				},
			};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedTile(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		TexturedVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z),
					TextureCoordinates = new Vector2(0, 0),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z),
					TextureCoordinates = new Vector2(1, 0),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z + size),
					TextureCoordinates = new Vector2(1, 1),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z + size),
					TextureCoordinates = new Vector2(0, 1),
					Color = color_vector,
				},
			};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedTileCentered(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		TexturedVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y, position.Z - size / 2f),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y, position.Z - size / 2f),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y, position.Z + size / 2f),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 1)
				},
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y, position.Z + size / 2f),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 1)
				},
			};

		return quad;
	}
	#endregion

	#region Quads
	public static ColoredVertex[] CreateQuad(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		ColoredVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z),
					Color = color_vector
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z),
					Color = color_vector
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y + size, position.Z),
					Color = color_vector
				},
				new()
				{
					Position = new Vector3(position.X, position.Y + size, position.Z),
					Color = color_vector
				},
			};

		return quad;
	}

	public static ColoredVertex[] CreateQuadCentered(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		ColoredVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y - size / 2f, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y - size / 2f, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y + size / 2f, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y + size / 2f, position.Z),
					Color = color_vector,
				},
			};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedQuad(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		TexturedVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size, position.Y + size, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 1)
				},
				new()
				{
					Position = new Vector3(position.X, position.Y + size, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 1)
				},
			};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedQuadCentered(Vector3 position, Color color, float size = 1f) {
		var color_vector = color.ToVector();
		TexturedVertex[] quad = {
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y - size / 2f, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y - size / 2f, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 0)
				},
				new()
				{
					Position = new Vector3(position.X + size / 2f, position.Y + size / 2f, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(1, 1)
				},
				new()
				{
					Position = new Vector3(position.X - size / 2f, position.Y + size / 2f, position.Z),
					Color = color_vector,
					TextureCoordinates = new Vector2(0, 1)
				},
			};

		return quad;
	}
	#endregion

	#region Line
	public static ColoredVertex[] CreateLine(Vector3 start_position, Vector3 end_position, Color color) {
		var color_vector = color.ToVector();
		ColoredVertex[] line = {
				new()
				{
					Position = new Vector3(start_position.X, start_position.Y, start_position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(end_position.X, end_position.Y, end_position.Z),
					Color = color_vector,
				},
			};

		return line;
	}
	#endregion
}
