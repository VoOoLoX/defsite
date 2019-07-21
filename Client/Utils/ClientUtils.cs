using OpenTK;

namespace Client {
	public static class ClientUtils {
		// Doesn't work for now find better way
		public static float WorldUnitToScreen(float scale) {
			return scale * 1;
		}

		public static float TextHeight(float scale) {
			return WorldUnitToScreen(scale);
		}

		public static float TextWidth(string text, float scale) {
			return text.Length * WorldUnitToScreen(scale);
		}

		public static float Map(float value, float min, float max, float new_min, float new_max) {
			return (value - min) * (new_max - new_min) / (max - min) + new_min;
		}

		public static Vector2 ScreenToWorld(float x, float y) {
			return new Vector2(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1));
		}

		public static Vector3 ScreenToWorld(float x, float y, float z) {
			return new Vector3(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1), Map(z, 0, Window.ClientHeight, -1, 1));
		}

		public static float Lerp(this float start, float end, float amount) {
			return start + end - start * amount;
		}

		public static Color Lerp(this Color color, Color to, float amount) {
			float sr = color.R, sg = color.G, sb = color.B;

			float er = to.R, eg = to.G, eb = to.B;

			byte r = (byte) sr.Lerp(er, amount),
				g = (byte) sg.Lerp(eg, amount),
				b = (byte) sb.Lerp(eb, amount);

			return Color.FromArgb(255, r, g, b);
		}
	}
}