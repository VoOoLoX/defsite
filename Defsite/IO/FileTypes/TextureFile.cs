using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Common;
using OpenTK;

namespace Defsite {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Pixel {

		public Pixel(byte r, byte g, byte b, byte a) {
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public Pixel(Color color) {
			R = color.R;
			G = color.G;
			B = color.B;
			A = color.A;
		}

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }

		public override string ToString() => $"({R} {G} {B} {A})";
	}

	public class TextureFile {
		public static TextureFile Default = new TextureFile(1, 1, new List<Pixel>() { new Pixel(Color.White), new Pixel(Color.White) });

		public byte[] Bytes {
			get {
				var ret = new List<byte>();
				foreach (var p in Pixels) {
					ret.Add(p.R);
					ret.Add(p.G);
					ret.Add(p.B);
					ret.Add(p.A);
				}

				return ret.ToArray();
			}
		}

		public byte Components { get; private set; }

		public byte Compressed { get; private set; }

		public ushort Width { get; private set; }

		public ushort Height { get; private set; }

		public List<Pixel> Pixels { get; private set; }

		public TextureFile(ushort width, ushort height, List<Pixel> pixels, byte comps = 4) {
			Width = width;
			Height = height;
			Pixels = pixels;
			Components = comps;
		}

		public TextureFile(string path) => Load(path);

		public TextureFile(Stream stream) => Load(stream);
		public void Save(string path) {
			if (path != string.Empty) {
				using var writer = new BinaryWriter(new StreamWriter(path).BaseStream);
				writer.Write("VIF".ToCharArray());
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Components);
				writer.Write(Compressed);
				writer.Write(Compressed > 0 ? Bytes.CompressAsync().Result : Bytes);
			} else {
				Log.Error("Invalid file path");
			}
		}

		void Load(string file_path) {
			var file_info = File.Exists(file_path) ? new FileInfo(file_path) : null;
			if (file_info == null)
				Log.Panic($"Invalid image file path: {file_path}");
			Load(file_info?.OpenRead());
		}

		void Load(Stream data_stream) {
			using var reader = new BinaryReader(data_stream);

			var vif = new string(reader.ReadChars(3));

			if (vif.ToUpper() != "VIF") Log.Error("Invalid VIF header");

			var width = reader.ReadUInt16();
			var height = reader.ReadUInt16();
			var comps = reader.ReadByte();
			var compressed = reader.ReadByte();

			if (width > 0 && height > 0 && comps > 0) {
				Width = width;
				Height = height;
				Components = comps;
				Compressed = compressed;
				Pixels = new List<Pixel>();

				var data_bytes = reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position);
				var data = compressed > 0 ? data_bytes.DecompressAsync().Result : data_bytes;

				for (var i = 0; i < data.Length; i += comps) {
					switch (comps) {
						case 4:
							Pixels.Add(new Pixel(data[i], data[i + 1], data[i + 2], data[i + 3]));
							break;

						case 3:
							Pixels.Add(new Pixel(data[i], data[i + 1], data[i + 2], 255));
							break;

						case 2:
							break;

						case 1:
							break;
					}
				}
			}
		}
	}
}