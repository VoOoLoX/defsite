using OpenTK;

namespace Client {
	public static class Primitives {
		public static class Quad {
			public static Vector2[] PositionData =
				new Vector2[] {
					new Vector2(0, -1),
					new Vector2(1, -1),
					new Vector2(1, 0),
					new Vector2(0, 0),
				};

			public static Vector2[] UVData =
				new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1),
				};

			public static uint[] IndexBufferData =
				new uint[] {
					0,1,2,
					2,3,0
				};
		}

		public static class QuadCentered {
			public static Vector2[] PositionData =
				new Vector2[] {
					new Vector2(-.5f, -.5f),
					new Vector2( .5f, -.5f),
					new Vector2( .5f,  .5f),
					new Vector2(-.5f,  .5f),
				};

			public static Vector2[] UVData =
				new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1),
				};

			public static uint[] IndexBufferData =
				new uint[] {
					0,1,2,
					2,3,0
				};
		}
	}
}