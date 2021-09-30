using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Defsite.Utils;

public static class Utils {

	public static async Task<byte[]> CompressAsync(this byte[] byte_array) {
		await using var result = new MemoryStream();
		await using var g_zip_stream = new GZipStream(result, CompressionMode.Compress);

		await g_zip_stream.WriteAsync(byte_array);
		g_zip_stream.Close();
		return result.ToArray();
	}

	public static async Task<byte[]> DecompressAsync(this byte[] byte_array) {
		await using var stream = new MemoryStream(byte_array);
		await using var g_zip_stream = new GZipStream(stream, CompressionMode.Decompress);
		await using var result = new MemoryStream();

		await g_zip_stream.CopyToAsync(result);
		return result.ToArray();
	}

	public static IEnumerable<int> Index(this byte[] x, byte[] y) {
		var index = Enumerable.Range(0, x.Length - y.Length + 1);
		for(var i = 0; i < y.Length; i++) {
			index = index.Where(n => x[n + i] == y[i]).ToArray();
		}

		return index;
	}

	public static float Lerp(float start, float end, float amount) => start + end - start * amount;

	public static Color Lerp(this Color color, Color to, float amount) {
		byte sr = color.R, sg = color.G, sb = color.B;

		byte er = to.R, eg = to.G, eb = to.B;

		var r = Convert.ToByte(Lerp(sr, er, amount));
		var g = Convert.ToByte(Lerp(sg, eg, amount));
		var b = Convert.ToByte(Lerp(sb, eb, amount));

		return Color.FromArgb(255, r, g, b);
	}

	public static Vector4 Lerp(this Vector4 color, Vector4 to, float amount) {
		var (sr, sg, sb, sa) = color;

		var (er, eg, eb, ea) = to;

		var r = Lerp(sr, er, amount);
		var g = Lerp(sg, eg, amount);
		var b = Lerp(sb, eb, amount);
		var a = Lerp(sa, ea, amount);

		return new Vector4(r, g, b, a);
	}

	public static Vector4 ToVector(this Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

	public static Vector2 ToVector(this Point point) => new(point.X, point.Y);

	public static float Blerp(float start_x, float end_x, float start_y, float end_y, float amount_x, float amount_y) => Lerp(Lerp(start_x, end_x, amount_x), Lerp(start_y, end_y, amount_x), amount_y);

	public static Color Blerp(this Color color, Color to, float amount_x, float amount_y) {
		byte sr = color.R, sg = color.G, sb = color.B;

		byte er = to.R, eg = to.G, eb = to.B;

		var r = Convert.ToByte(Blerp(sr, er, sr, er, amount_x, amount_y));
		var g = Convert.ToByte(Blerp(sg, eg, sg, eg, amount_x, amount_y));
		var b = Convert.ToByte(Blerp(sb, eb, sb, eb, amount_x, amount_y));

		return Color.FromArgb(255, r, g, b);
	}
	public static float Map(float value, float min, float max, float new_min, float new_max) => (value - min) * (new_max - new_min) / (max - min) + new_min;
}
