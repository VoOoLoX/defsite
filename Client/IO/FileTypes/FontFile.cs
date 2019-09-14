using System.Collections.Generic;
using System.IO;
using System.Linq;
using Defsite;

namespace Client {
	public struct CharInfo {
		public CharInfo(int x, int y, int width, int height, int xOffset, int yOffset, int xAdvance) {
			X = x;
			Y = y;
			Width = width;
			Height = height;
			XOffset = xOffset;
			YOffset = yOffset;
			XAdvance = xAdvance;
		}

		public int X { get; }
		public int Y { get; }
		public int Width { get; }
		public int Height { get; }
		public int XOffset { get; }
		public int YOffset { get; }
		public int XAdvance { get; }

		public override string ToString() => $"{X} {Y} {Width} {Height} {XOffset} {YOffset} {XAdvance}";
	}

	public class FontFile {
		public FontFile(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path == string.Empty) Log.Error($"Invalid font file path: {path}");

			var reader = new BinaryReader(File.OpenRead(path));

			var vff = new string(reader.ReadChars(3));

			if (vff.ToUpper() != "VFF") Log.Error("Invalid VFF header");

			var count = reader.ReadByte();
			var compressed = reader.ReadByte();

			var position = reader.BaseStream.Position;

			if (count > 0) {
				Texture = new Texture();
				Layout = new Dictionary<char, CharInfo>();

				var data_bytes = reader.ReadBytes((int) reader.BaseStream.Length - (int) position);
				var vif_index = data_bytes.Index(new[] {(byte) 'V', (byte) 'I', (byte) 'F'}).First();

				reader.BaseStream.Position = position;

				var font_bytes = reader.ReadBytes(vif_index);
				var data_stream = compressed > 0 ? new BinaryReader(new MemoryStream(font_bytes.Decompress())) : new BinaryReader(new MemoryStream(font_bytes));

				for (var i = 0; i < count; i++) {
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

				var image_stream = new MemoryStream(reader.ReadBytes((int) reader.BaseStream.Length - (int) reader.BaseStream.Position));

				Texture = new Texture(new TextureFile(image_stream));
			}
		}

		public Texture Texture { get; }

		public Dictionary<char, CharInfo> Layout { get; }
	}
}