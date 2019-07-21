using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;

namespace Client {
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

		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }

		public override string ToString() {
			return $"{R} {G} {B} {A}";
		}
	}

	public class Image {
		public static Image Default = new Image(2, 2, new List<Pixel>() {
			new Pixel(Color.Magenta), new Pixel(Color.Black), new Pixel(Color.Black), new Pixel(Color.Magenta)
		});

		public Image(ushort width, ushort height, List<Pixel> pixels, byte comps = 4) {
			Width = width;
			Height = height;
			Pixels = pixels;
			Components = comps;
		}

		public Image(string path) => Load(path);

		public ushort Width { get; private set; }
		public ushort Height { get; private set; }
		public List<Pixel> Pixels { get; private set; }
		public byte Components { get; private set; }

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

		void Load(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path == string.Empty) throw new Exception($"Invalid image file path: {path}");

			var reader = new BinaryReader(new StreamReader(path).BaseStream);
			var vif = reader.ReadChars(3);

			if (new string(vif).ToUpper() != "VIF") throw new Exception("Invalid VIF header");

			var width = reader.ReadUInt16();
			var height = reader.ReadUInt16();
			var comps = reader.ReadByte();

			if (width > 0 && height > 0 && comps > 0) {
				Pixels = new List<Pixel>();

				var data = reader.ReadBytes((int) reader.BaseStream.Length - 8).ToList();

				for (var i = 0; i < data.Count; i += 4)
					Pixels.Add(new Pixel(data[i], data[i + 1], data[i + 2], data[i + 3]));

				Width = width;
				Height = height;
				Components = comps;
			}
		}

		public void Save(string path) {
			path = File.Exists(path) ? path : string.Empty;
			if (path != string.Empty) {
				var writer = new BinaryWriter(new StreamWriter(path).BaseStream);
				writer.Write("VIF");
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Components);
				writer.Write(Bytes);
			}
			else {
				throw new Exception("Invalid file path");
			}
		}

		public void Rotate180() {
			Pixels.Reverse();
		}

		public void FlipHorizontal() {
			var new_data = new List<Pixel[]>();
			for (var i = 0; i < Pixels.Count; i += Width)
				new_data.Add(Pixels.Skip(i).Take(Width).ToArray());

			for (var i = 0; i < new_data.Count; i++)
				new_data[i] = new_data[i].Reverse().ToArray();
			Pixels = new_data.SelectMany(x => x).ToList();
		}
	}
}