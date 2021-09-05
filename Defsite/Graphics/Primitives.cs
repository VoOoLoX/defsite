using System.Drawing;
using OpenTK.Mathematics;

namespace Defsite {

	public static class Primitives {

		public static class CubeCentered {

			public static uint[] IndexBufferData = {
				0, 1, 2, 3,
				4, 5, 6, 7,
				8, 9, 10, 11,
				12, 13, 14, 15,
				16, 17, 18, 19,
				20, 21, 22, 23
			};

			public static Vector3[] NormalsData = {
				new(0, 0, 1),
				new(0, 0, 1),
				new(0, 0, 1),
				new(0, 0, 1),

				new(0, 1, 0),
				new(0, 1, 0),
				new(0, 1, 0),
				new(0, 1, 0),

				new(1, 0, 0),
				new(1, 0, 0),
				new(1, 0, 0),
				new(1, 0, 0),

				new(0, -1, 0),
				new(0, -1, 0),
				new(0, -1, 0),
				new(0, -1, 0),

				new(-1, 0, 0),
				new(-1, 0, 0),
				new(-1, 0, 0),
				new(-1, 0, 0),

				new(0, 0, -1),
				new(0, 0, -1),
				new(0, 0, -1),
				new(0, 0, -1)
			};

			public static Vector3[] PositionData = {
				//Front
				new(-1, -1, 1),
				new(1, -1, 1),
				new(1, 1, 1),
				new(-1, 1, 1),

				//Top
				new(-1, 1, 1),
				new(-1, 1, -1),
				new(1, 1, -1),
				new(1, 1, 1),

				//Right
				new(1, 1, 1),
				new(1, 1, -1),
				new(1, -1, -1),
				new(1, -1, 1),

				//Bottom
				new(1, -1, 1),
				new(1, -1, -1),
				new(-1, -1, -1),
				new(-1, -1, 1),

				//Left
				new(-1, -1, 1),
				new(-1, -1, -1),
				new(-1, 1, -1),
				new(-1, 1, 1),

				//Back
				new(-1, -1, -1),
				new(1, -1, -1),
				new(1, 1, -1),
				new(-1, 1, -1),
			};
			/* How texture should look like
			 *
			 *   #			|B|
			 *  ###		BACK|R|FRONT
			 *   #			|T|
			 *   #			|L|
			 */

			public static Vector2[] UVData = {
				//Front
				new(0.66f, 0.5f),
				new(0.99f, 0.5f),
				new(0.99f, 0.75f),
				new(0.66f, 0.75f),

				//Top
				new(0.33f, 0.25f),
				new(0.66f, 0.25f),
				new(0.66f, 0.5f),
				new(0.33f, 0.5f),

				//Right
				new(0.33f, 0.5f),
				new(0.66f, 0.5f),
				new(0.66f, 0.75f),
				new(0.33f, 0.75f),

				//Bottom
				new(0.33f, 0.75f),
				new(0.66f, 0.75f),
				new(0.66f, 1),
				new(0.33f, 1),

				//Left
				new(0.33f, 0),
				new(0.66f, 0),
				new(0.66f, 0.25f),
				new(0.33f, 0.25f),

				//Back
				new(0, 0.5f),
				new(0.33f, 0.5f),
				new(0.33f, 0.75f),
				new(0, 0.75f),
			};
		}

		public static class Quad {

			public static uint[] IndexBufferData = {
				0, 1, 2, 3
			};

			public static Vector2[] PositionData = {
				new(0, -1),
				new(1, -1),
				new(1, 0),
				new(0, 0)
			};

			//"Flipped" (UVs have origin at 0,0 in the bottom left corner) because of OpenGLs coordinate system
			public static Vector2[] UVData = {
				new(0, 1),
				new(1, 1),
				new(1, 0),
				new(0, 0)
			};
		}

		public static class QuadCentered {

			public static uint[] IndexBufferData = {
				0, 1, 2, 3
			};

			public static Vector2[] PositionData = {
				new(-1f, -1f),
				new(1f, -1f),
				new(1f, 1f),
				new(-1f, 1f)
			};

			//"Flipped" (UVs have origin at 0,0 in the bottom left corner) because of OpenGLs coordinate system
			public static Vector2[] UVData = {
				new(0, 1),
				new(1, 1),
				new(1, 0),
				new(0, 0)
			};
		}

		public static Vertex[] CreateQuad(Vector3 position, Color color) {
			var color_vector = color.ToVector();
			Vertex[] quad = {
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + 1, position.Y, position.Z),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X + 1, position.Y, position.Z+ 1),
					Color = color_vector,
				},
				new()
				{
					Position = new Vector3(position.X, position.Y, position.Z+ 1),
					Color = color_vector,
				},
			};


			return quad;
		}

		//public static Vertex[] CreateQuad(Vector3 position, Texture texture) {
		//	var color_vector = color.ToVector();
		//	Vertex[] quad = {
		//		new Vertex
		//		{
		//			Position = new Vector3(position.X, position.Y, position.Z),
		//			Color = color_vector,
		//		},
		//		new Vertex
		//		{
		//			Position = new Vector3(position.X + 1, position.Y, position.Z),
		//			Color = color_vector,
		//		},
		//		new Vertex
		//		{
		//			Position = new Vector3(position.X + 1, position.Y + 1, position.Z),
		//			Color = color_vector,
		//		},
		//		new Vertex
		//		{
		//			Position = new Vector3(position.X, position.Y + 1, position.Z),
		//			Color = color_vector,
		//		},
		//	};

		//	return quad;
		//}
	}
}