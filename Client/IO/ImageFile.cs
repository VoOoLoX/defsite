using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client {
	public class Pixel {
		public byte R { get; private set; }
		public byte G { get; private set; }
		public byte B { get; private set; }
		public byte A { get; private set; }

		public Pixel(byte r, byte g, byte b, byte a) {
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public override string ToString() => $"{R} {G} {B} {A}";
	}

	public class VIFImage {
		public int Width { get; private set; }
		public int Height { get; private set; }
		public List<Pixel> Data { get; private set; }
		public int Components { get; private set; }

		public VIFImage(int width, int height, List<Pixel> data, int comps = 4) {
			Width = width;
			Height = height;
			Data = data;
			Components = comps;
		}

		public void Rotate180() => Data.Reverse();

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

	static class VIF {
		public static VIFImage Load(string file) => Parse(File.Exists(file) ? file : string.Empty);

		static VIFImage Parse(string path) {
			if (path != string.Empty) {
				var reader = new BinaryReader(new StreamReader(path).BaseStream);
				var vif = reader.ReadChars(3);
				if (new string(vif).ToUpper() == "VIF") {
					var width = reader.ReadUInt16();
					var height = reader.ReadUInt16();
					var comps = reader.ReadByte();

					if ((width > 0 && height > 0) && (comps > 0 && comps < 5)) {
						var data = reader.ReadBytes((int)reader.BaseStream.Length - 8).ToList();
						var res = new List<Pixel>();

						for (var i = 0; i < data.Count; i += 4)
							res.Add(new Pixel(data[i], data[i + 1], data[i + 2], data[i + 3]));

						return new VIFImage(width, height, res);
					}
				}
				throw (new Exception("Invalid VIF header"));
			}
			throw (new Exception("Invalid file path"));
		}
	}
}