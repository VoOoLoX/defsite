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
				//Back
				new Vector3(-1, -1, -1),
				new Vector3(1, -1, -1),
				new Vector3(1, 1, -1),
				new Vector3(-1, 1, -1),

				//Front
				new Vector3(-1, -1, 1),
				new Vector3(1, -1, 1),
				new Vector3(1, 1, 1),
				new Vector3(-1, 1, 1)
			};

			public static Vector2[] UVData = {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),

				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),

				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),

				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),

				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),

				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),
			};

			public static uint[] IndexBufferData = {
				0, 1, 2, 3,
				4, 5, 1, 0,
				5, 1, 2, 6,
				7, 6, 2, 3,
				4, 0, 3, 7,
				4, 5, 6, 7
			};
		}
	}
}