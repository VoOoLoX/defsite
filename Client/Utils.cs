using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	public static class Utils {

		public static float WorldUnitToScreen(float scale) => (scale * Camera.ZoomFactor);
		public static float TextHeight(float scale) => WorldUnitToScreen(scale);
		public static float TextWidth(string text, float scale) => text.Length * WorldUnitToScreen(scale);

		public static float Map(float value, float min, float max, float new_min, float new_max) =>
			(value - min) * (new_max - new_min) / (max - min) + new_min;

		public static Vector2 ScreenToWorld(float x, float y) =>
			new Vector2(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1));
	}
}
