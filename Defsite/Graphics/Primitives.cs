using System.Drawing;
using Defsite.Graphics.VertexTypes;
using Defsite.Utils;

using OpenTK.Mathematics;

namespace Defsite.Graphics;

public static class Primitives {
	#region Tiles
	public static ColoredVertex[] CreateTile(Vector3 position, Vector2 width_and_height = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var color_vector = color == default ? Color.White.ToVector() : color.ToVector();
		var wh = width_and_height == default ? Vector2.One : width_and_height;
		var half_width = wh.X / 2;
		var half_height = wh.Y / 2;
		var top_left = centered ? new Vector4(position.X - half_width, position.Y, position.Z - half_height, 1f) : new Vector4(position.X, position.Y, position.Z + wh.Y, 1f);
		var top_right = centered ? new Vector4(position.X + half_width, position.Y, position.Z - half_height, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z + wh.Y, 1f);
		var bottom_left = centered ? new Vector4(position.X - half_width, position.Y, position.Z + half_height, 1f) : new Vector4(position.X, position.Y, position.Z, 1f);
		var bottom_right = centered ? new Vector4(position.X + half_width, position.Y, position.Z + half_height, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z, 1f);
		ColoredVertex[] quad = {
			new()
			{
				Position = transform == default ? bottom_left : bottom_left * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? bottom_right: bottom_right * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? top_right: top_right * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? top_left: top_left * transform,
				Color = color_vector
			},
		};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedTile(Vector3 position, Vector2 width_and_height = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var color_vector = color == default ? Color.White.ToVector() : color.ToVector();
		var wh = width_and_height == default ? Vector2.One : width_and_height;
		var half_width = wh.X / 2;
		var half_height = wh.Y / 2;
		var top_left = centered ? new Vector4(position.X - half_width, position.Y, position.Z - half_height, 1f) : new Vector4(position.X, position.Y, position.Z + wh.Y, 1f);
		var top_right = centered ? new Vector4(position.X + half_width, position.Y, position.Z - half_height, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z + wh.Y, 1f);
		var bottom_left = centered ? new Vector4(position.X - half_width, position.Y, position.Z + half_height, 1f) : new Vector4(position.X, position.Y, position.Z, 1f);
		var bottom_right = centered ? new Vector4(position.X + half_width, position.Y, position.Z + half_height, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z, 1f);
		TexturedVertex[] quad = {
			new()
			{
				Position = transform == default ? bottom_left : bottom_left * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(0, 0)
			},
			new()
			{
				Position = transform == default ? bottom_right: bottom_right * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(1, 0)
			},
			new()
			{
				Position = transform == default ? top_right: top_right * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(1, 1)
			},
			new()
			{
				Position = transform == default ? top_left: top_left * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(0, 1)
			},
		};

		return quad;
	}
	#endregion

	#region Quads
	public static ColoredVertex[] CreateQuad(Vector3 position, Vector2 width_and_height = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var color_vector = color == default ? Color.White.ToVector() : color.ToVector();
		var wh = width_and_height == default ? Vector2.One : width_and_height;
		var half_width = wh.X / 2;
		var half_height = wh.Y / 2;
		var top_left = centered ? new Vector4(position.X - half_width, position.Y - half_height, position.Z, 1f) : new Vector4(position.X, position.Y + wh.Y, position.Z, 1f);
		var top_right = centered ? new Vector4(position.X + half_width, position.Y - half_height, position.Z, 1f) : new Vector4(position.X + wh.X, position.Y + wh.Y, position.Z, 1f);
		var bottom_left = centered ? new Vector4(position.X - half_width, position.Y + half_height, position.Z, 1f) : new Vector4(position.X, position.Y, position.Z, 1f);
		var bottom_right = centered ? new Vector4(position.X + half_width, position.Y + half_height, position.Z, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z, 1f);
		ColoredVertex[] quad = {
			new()
			{
				Position = transform == default ? bottom_left : bottom_left * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? bottom_right: bottom_right * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? top_right: top_right * transform,
				Color = color_vector
			},
			new()
			{
				Position = transform == default ? top_left: top_left * transform,
				Color = color_vector
			},
		};

		return quad;
	}

	public static TexturedVertex[] CreateTexturedQuad(Vector3 position, Vector2 width_and_height = default, Color color = default, bool centered = true, Matrix4 transform = default) {
		var color_vector = color == default ? Color.White.ToVector() : color.ToVector();
		var wh = width_and_height == default ? Vector2.One : width_and_height;
		var half_width = wh.X / 2;
		var half_height = wh.Y / 2;
		var top_left = centered ? new Vector4(position.X - half_width, position.Y - half_height, position.Z, 1f) : new Vector4(position.X, position.Y + wh.Y, position.Z, 1f);
		var top_right = centered ? new Vector4(position.X + half_width, position.Y - half_height, position.Z, 1f) : new Vector4(position.X + wh.X, position.Y + wh.Y, position.Z, 1f);
		var bottom_left = centered ? new Vector4(position.X - half_width, position.Y + half_height, position.Z, 1f) : new Vector4(position.X, position.Y, position.Z, 1f);
		var bottom_right = centered ? new Vector4(position.X + half_width, position.Y + half_height, position.Z, 1f) : new Vector4(position.X + wh.X, position.Y, position.Z, 1f);
		TexturedVertex[] quad = {
			new()
			{
				Position = transform == default ? bottom_left : bottom_left * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(0, 0)
			},
			new()
			{
				Position = transform == default ? bottom_right: bottom_right * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(1, 0)
			},
			new()
			{
				Position = transform == default ? top_right: top_right * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(1, 1)
			},
			new()
			{
				Position = transform == default ? top_left: top_left * transform,
				Color = color_vector,
				TextureCoordinates = new Vector2(0, 1)
			},
		};

		return quad;
	}
	#endregion

	#region Line
	public static ColoredVertex[] CreateLine(Vector3 start_position, Vector3 end_position, Color color = default) {
		var color_vector = color == default ? Color.White.ToVector() : color.ToVector();
		ColoredVertex[] line = {
			new()
			{
				Position = new Vector4(start_position.X, start_position.Y, start_position.Z, 1f),
				Color = color_vector,
			},
			new()
			{
				Position = new Vector4(end_position.X, end_position.Y, end_position.Z, 1f),
				Color = color_vector,
			},
		};

		return line;
	}

	public static ColoredVertex[] CreateLine(Vector3 start_position, Vector3 end_position, Color start_color = default, Color end_color = default) {
		var start_color_vector = start_color == default ? Color.White.ToVector() : start_color.ToVector();
		var end_color_vector = end_color == default ? Color.White.ToVector() : end_color.ToVector();
		ColoredVertex[] line = {
			new()
			{
				Position = new Vector4(start_position.X, start_position.Y, start_position.Z, 1f),
				Color = start_color_vector,
			},
			new()
			{
				Position = new Vector4(end_position.X, end_position.Y, end_position.Z, 1f),
				Color = end_color_vector,
			},
		};

		return line;
	}
	#endregion
}
