using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OpenTK;

namespace Client {
	public static class Utils {
		public static IEnumerable<int> Index(this byte[] x, byte[] y) {
			var index = Enumerable.Range(0, x.Length - y.Length + 1);
			for (var i = 0; i < y.Length; i++) {
				index = index.Where(n => x[n + i] == y[i]).ToArray();
			}

			return index;
		}

		public static byte[] Compress(this byte[] byte_array) {
			var stream = new MemoryStream();
			var g_zip_stream = new GZipStream(stream, CompressionMode.Compress);

			g_zip_stream.Write(byte_array, 0, byte_array.Length);
			g_zip_stream.Flush();
			stream.Flush();

			var ret = stream.GetBuffer();
			g_zip_stream.Close();
			stream.Close();
			return ret;
		}

		public static byte[] Decompress(this byte[] byte_array) {
			var stream = new MemoryStream(byte_array);
			var g_zip_stream = new GZipStream(stream, CompressionMode.Decompress);
			var data = new List<byte>();

			var bytes_read = g_zip_stream.ReadByte();
			while (bytes_read != -1) {
				data.Add((byte) bytes_read);
				bytes_read = g_zip_stream.ReadByte();
			}

			g_zip_stream.Flush();
			stream.Flush();
			g_zip_stream.Close();
			stream.Close();
			return data.ToArray();
		}

		// Doesn't work for now find better way
//		public static float WorldUnitToScreen(float scale) {
//			return scale * 1;
//		}
//
//		public static float TextHeight(float scale) {
//			return WorldUnitToScreen(scale);
//		}
//
//		public static float TextWidth(string text, float scale) {
//			return text.Length * WorldUnitToScreen(scale);
//		}
//
//		public static Vector2 ScreenToWorld(float x, float y) {
//			return new Vector2(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1));
//		}
//
//		public static Vector3 ScreenToWorld(float x, float y, float z) {
//			return new Vector3(Map(x, 0, Window.ClientWidth, -1, 1), Map(y, 0, Window.ClientHeight, -1, 1), Map(z, 0, Window.ClientHeight, -1, 1));
//		}

		public static float Map(float value, float min, float max, float new_min, float new_max) {
			return (value - min) * (new_max - new_min) / (max - min) + new_min;
		}

		public static float Lerp(float start, float end, float amount) {
			return start + end - start * amount;
		}

		public static float Blerp(float start_x, float end_x, float start_y, float end_y, float amount_x, float amount_y) {
			return Lerp(Lerp(start_x, end_x, amount_x), Lerp(start_y, end_y, amount_x), amount_y);
		}


		public static Color Lerp(this Color color, Color to, float amount) {
			float sr = color.R, sg = color.G, sb = color.B;

			float er = to.R, eg = to.G, eb = to.B;

			byte r = (byte) Lerp(sr, er, amount),
				g = (byte) Lerp(sg, eg, amount),
				b = (byte) Lerp(sb, eb, amount);

			return Color.FromArgb(255, r, g, b);
		}
	}
}