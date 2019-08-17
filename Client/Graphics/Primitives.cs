using OpenTK;

namespace Client {
	public static class Primitives {
		public static class Quad {
			public static Vector2[] PositionData = {
				new Vector2(0, -1),
				new Vector2(1, -1),
				new Vector2(1, 0),
				new Vector2(0, 0)
			};

			public static Vector2[] UVData = {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			};

			public static uint[] IndexBufferData = {
				0, 1, 2, 3
			};
		}

		public static class QuadCentered {
			public static Vector2[] PositionData = {
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, 1f)
			};

			public static Vector2[] UVData = {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			};

			public static uint[] IndexBufferData = {
				0, 1, 2, 3
			};
		}

		public static class CubeCentered {
			public static Vector3[] PositionData = {
				//Front
				new Vector3(-1, -1, 1),
				new Vector3(1, -1, 1),
				new Vector3(1, 1, 1),
				new Vector3(-1, 1, 1),

				//Top
				new Vector3(-1, 1, 1),
				new Vector3(-1, 1, -1),
				new Vector3(1, 1, -1),
				new Vector3(1, 1, 1),

				//Right
				new Vector3(1, 1, 1),
				new Vector3(1, 1, -1),
				new Vector3(1, -1, -1),
				new Vector3(1, -1, 1),

				//Bottom
				new Vector3(1, -1, 1),
				new Vector3(1, -1, -1),
				new Vector3(-1, -1, -1),
				new Vector3(-1, -1, 1),

				//Left
				new Vector3(-1, -1, 1),
				new Vector3(-1, -1, -1),
				new Vector3(-1, 1, -1),
				new Vector3(-1, 1, 1),

				//Back
				new Vector3(-1, -1, -1),
				new Vector3(1, -1, -1),
				new Vector3(1, 1, -1),
				new Vector3(-1, 1, -1),
			};

			public static uint[] IndexBufferData = {
				0, 1, 2, 3,
				4, 5, 6, 7,
				8, 9, 10, 11,
				12, 13, 14, 15,
				16, 17, 18, 19,
				20, 21, 22, 23
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
				new Vector2(0.66f, 0.5f),
				new Vector2(0.99f, 0.5f),
				new Vector2(0.99f, 0.75f),
				new Vector2(0.66f, 0.75f),

				//Top
				new Vector2(0.33f, 0.25f),
				new Vector2(0.66f, 0.25f),
				new Vector2(0.66f, 0.5f),
				new Vector2(0.33f, 0.5f),

				//Right
				new Vector2(0.33f, 0.5f),
				new Vector2(0.66f, 0.5f),
				new Vector2(0.66f, 0.75f),
				new Vector2(0.33f, 0.75f),

				//Bottom
				new Vector2(0.33f, 0.75f),
				new Vector2(0.66f, 0.75f),
				new Vector2(0.66f, 1),
				new Vector2(0.33f, 1),

				//Left
				new Vector2(0.33f, 0),
				new Vector2(0.66f, 0),
				new Vector2(0.66f, 0.25f),
				new Vector2(0.33f, 0.25f),

				//Back
				new Vector2(0, 0.5f),
				new Vector2(0.33f, 0.5f),
				new Vector2(0.33f, 0.75f),
				new Vector2(0, 0.75f),
			};

			public static Vector3[] NormalsData = {
				new Vector3(0, 0, 1),
				new Vector3(0, 0, 1),
				new Vector3(0, 0, 1),
				new Vector3(0, 0, 1),

				new Vector3(0, 1, 0),
				new Vector3(0, 1, 0),
				new Vector3(0, 1, 0),
				new Vector3(0, 1, 0),

				new Vector3(1, 0, 0),
				new Vector3(1, 0, 0),
				new Vector3(1, 0, 0),
				new Vector3(1, 0, 0),

				new Vector3(0, -1, 0),
				new Vector3(0, -1, 0),
				new Vector3(0, -1, 0),
				new Vector3(0, -1, 0),

				new Vector3(-1, 0, 0),
				new Vector3(-1, 0, 0),
				new Vector3(-1, 0, 0),
				new Vector3(-1, 0, 0),

				new Vector3(0, 0, -1),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, -1)
			};
		}
	}
}