using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Common;

using Defsite.Graphics;
using Defsite.Utils;

namespace Defsite.IO.DataFormats;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct CharInfo {

	public int Height { get; }

	public int Width { get; }

	public int X { get; }

	public int XAdvance { get; }

	public int XOffset { get; }

	public int Y { get; }

	public int YOffset { get; }

	public CharInfo(int x, int y, int width, int height, int x_offset, int y_offset, int x_advance) {
		X = x;
		Y = y;
		Width = width;
		Height = height;
		XOffset = x_offset;
		YOffset = y_offset;
		XAdvance = x_advance;
	}
	public override string ToString() => $"{X} {Y} {Width} {Height} {XOffset} {YOffset} {XAdvance}";
}

public class FontData {

	public Dictionary<char, CharInfo> Layout { get; }

	public Texture Texture { get; }

	public FontData(string file_path) {
		var path = File.Exists(file_path) ? file_path : string.Empty;

		if(string.IsNullOrEmpty(path)) {
			Log.Error($"Invalid font file path: {path}");
		}

		using var reader = new BinaryReader(File.OpenRead(path));

		var vff = new string(reader.ReadChars(3));

		if(vff.ToUpper() != "VFF") {
			Log.Error("Invalid VFF header");
		}

		var count = reader.ReadByte();
		var compressed = reader.ReadByte();

		var position = reader.BaseStream.Position;

		if(count > 0) {
			Layout = new Dictionary<char, CharInfo>();

			var data_bytes = reader.ReadBytes((int)reader.BaseStream.Length - (int)position);
			var vif_index = data_bytes.Index(new[] { (byte)'V', (byte)'I', (byte)'F' }).First();

			reader.BaseStream.Position = position;

			var font_bytes = reader.ReadBytes(vif_index);
			using var data_stream = compressed > 0 ? new BinaryReader(new MemoryStream(font_bytes.DecompressAsync().Result)) : new BinaryReader(new MemoryStream(font_bytes));

			for(var i = 0; i < count; i++) {
				var ch = data_stream.ReadChar();
				var x = data_stream.ReadUInt16();
				var y = data_stream.ReadUInt16();
				var width = data_stream.ReadUInt16();
				var height = data_stream.ReadUInt16();
				var x_offset = data_stream.ReadByte();
				var y_offset = data_stream.ReadByte();
				var x_advance = data_stream.ReadByte();
				Layout[ch] = new CharInfo(x, y, width, height, x_offset, y_offset, x_advance);
			}

			using var image_stream = new MemoryStream(reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));

			Texture = new Texture(new TextureData(image_stream));
		}
	}
}
