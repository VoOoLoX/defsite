using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client {
	public class Pixel {
		public Pixel(byte r, byte g, byte b, byte a) {
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }

		public override string ToString() {
			return $"{R} {G} {B} {A}";
		}
	}

	public class VIFImage {
		public VIFImage(int width, int height, List<Pixel> data, int comps = 4) {
			Width = width;
			Height = height;
			Data = data;
			Components = comps;
		}

		public int Width { get; }
		public int Height { get; }
		public List<Pixel> Data { get; private set; }
		public int Components { get; }

		public void Rotate180() {
			Data.Reverse();
		}

		public void FlipHorizontal() {
			var new_data = new List<Pixel[]>();
			for (var i = 0; i < Data.Count; i += Width)
				new_data.Add(Data.Skip(i).Take(Width).ToArray());

			for (var i = 0; i < new_data.Count; i++)
				new_data[i] = new_data[i].Reverse().ToArray();
			Data = new_data.SelectMany(x => x).ToList();
		}

		public byte[] Bytes() {
			var ret = new List<byte>();
			foreach (var p in Data) {
				ret.Add(p.R);
				ret.Add(p.G);
				ret.Add(p.B);
				ret.Add(p.A);
			}

			return ret.ToArray();
		}
	}

	internal static class VIF {
		public static VIFImage Load(string file) {
			return Parse(File.Exists(file) ? file : string.Empty);
		}

		static VIFImage Parse(string path) {
			if (path != string.Empty) {
				var reader = new BinaryReader(new StreamReader(path).BaseStream);
				var vif = reader.ReadChars(3);
				if (new string(vif).ToUpper() == "VIF") {
					var width = reader.ReadUInt16();
					var height = reader.ReadUInt16();
					var comps = reader.ReadByte();

					if (width > 0 && height > 0 && comps > 0 && comps < 5) {
						var data = reader.ReadBytes((int) reader.BaseStream.Length - 8).ToList();
						var res = new List<Pixel>();

						for (var i = 0; i < data.Count; i += 4)
							res.Add(new Pixel(data[i], data[i + 1], data[i + 2], data[i + 3]));

						return new VIFImage(width, height, res);
					}
				}

				throw new Exception("Invalid VIF header");
			}

			throw new Exception("Invalid file path");
		}
	}
}