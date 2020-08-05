using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using OpenTK;

namespace Client {

	public static class Utils {

		public static byte[] Compress(this byte[] byte_array) {
			using var result = new MemoryStream();
			using var g_zip_stream = new GZipStream(result, CompressionMode.Compress);

			g_zip_stream.Write(byte_array, 0, byte_array.Length);
			g_zip_stream.Close();
			return result.ToArray();
		}

		public static byte[] Decompress(this byte[] byte_array) {
			using var stream = new MemoryStream(byte_array);
			using var g_zip_stream = new GZipStream(stream, CompressionMode.Decompress);
			using var result = new MemoryStream();

			g_zip_stream.CopyTo(result);
			return result.ToArray();
		}

		public static IEnumerable<int> Index(this byte[] x, byte[] y) {
			var index = Enumerable.Range(0, x.Length - y.Length + 1);
			for (var i = 0; i < y.Length; i++) {
				index = index.Where(n => x[n + i] == y[i]).ToArray();
			}

			return index;
		}

		public static float Lerp(float start, float end, float amount) => start + end - start * amount;

		public static Color Lerp(this Color color, Color to, float amount) {
			byte sr = color.R, sg = color.G, sb = color.B;

			byte er = to.R, eg = to.G, eb = to.B;

			byte r = Convert.ToByte(Lerp(sr, er, amount));
			byte g = Convert.ToByte(Lerp(sg, eg, amount));
			byte b = Convert.ToByte(Lerp(sb, eb, amount));

			return Color.FromArgb(255, r, g, b);
		}

		public static Vector4 Lerp(this Vector4 color, Vector4 to, float amount) {
			float sr = color.X, sg = color.Y, sb = color.Z;

			float er = to.X, eg = to.Y, eb = to.Z;

			float r = Lerp(sr, er, amount);
			float g = Lerp(sg, eg, amount);
			float b = Lerp(sb, eb, amount);

			return new Vector4(r, g, b, 1);
		}

		public static Vector4 ToVector(this Color color) => new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

		public static float Blerp(float start_x, float end_x, float start_y, float end_y, float amount_x, float amount_y) => Lerp(Lerp(start_x, end_x, amount_x), Lerp(start_y, end_y, amount_x), amount_y);

		public static Color Blerp(this Color color, Color to, float amount_x, float amount_y) {
			byte sr = color.R, sg = color.G, sb = color.B;

			byte er = to.R, eg = to.G, eb = to.B;

			byte r = Convert.ToByte(Blerp(sr, er, sr, er, amount_x, amount_y));
			byte g = Convert.ToByte(Blerp(sg, eg, sg, eg, amount_x, amount_y));
			byte b = Convert.ToByte(Blerp(sb, eb, sb, eb, amount_x, amount_y));

			return Color.FromArgb(255, r, g, b);
		}
		public static float Map(float value, float min, float max, float new_min, float new_max) => (value - min) * (new_max - new_min) / (max - min) + new_min;
	}
}