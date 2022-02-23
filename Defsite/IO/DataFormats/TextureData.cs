using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Common;

using Defsite.Utils;

namespace Defsite.IO.DataFormats;

public interface IPixel {
	byte[] Bytes { get; }
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public record struct Pixel4(byte R = byte.MaxValue, byte G = byte.MaxValue, byte B = byte.MaxValue, byte A = byte.MaxValue) : IPixel {
	public byte[] Bytes => new[] { R, G, B, A };
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public record struct Pixel3(byte R = byte.MaxValue, byte G = byte.MaxValue, byte B = byte.MaxValue) : IPixel {
	public byte[] Bytes => new[] { R, G, B };
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public record struct Pixel2(byte X = byte.MaxValue, byte Y = byte.MaxValue) : IPixel {
	public byte[] Bytes => new[] { X, Y };
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public record struct Pixel1(byte X = byte.MaxValue) : IPixel {
	public byte[] Bytes => new[] { X };
}

public class TextureData {
	public static readonly TextureData Default = new(1, 1, new byte[] { 255, 255, 255 }, 3);

	public byte[] Bytes {
		get => Pixels.SelectMany(pixel => pixel.Bytes).ToArray();
		private set {
			var pixels = new List<IPixel>();
			for(var i = 0; i < value?.Length; i += Components) {
				IPixel pixel = Components switch {
					4 => new Pixel4(value[i], value[i + 1], value[i + 2], value[i + 3]),
					3 => new Pixel3(value[i], value[i + 1], value[i + 2]),
					2 => new Pixel2(value[i], value[i + 1]),
					1 => new Pixel1(value[i]),
					_ => throw new NotImplementedException()
				};
				pixels.Add(pixel);
			}

			Pixels = pixels;
		}
	}

	public byte Components { get; private set; }

	public byte Compressed { get; private set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public List<IPixel> Pixels { get; private set; }

	public TextureData(int width, int height, byte components = 4) {
		Width = width;
		Height = height;
		Components = components;
		Pixels = new List<IPixel>(width * height);
	}

	public TextureData(int width, int height, IntPtr data, int size, byte components) {
		Width = width;
		Height = height;
		Components = components;
		Pixels = new List<IPixel>(width * height);

		var bytes = new byte[size];
		Marshal.Copy(data, bytes, 0, size);
		Bytes = bytes;
	}

	public TextureData(int width, int height, byte[] data, byte components) {
		Width = width;
		Height = height;
		Components = components;
		Pixels = new List<IPixel>(width * height);
		Bytes = data;
	}

	public TextureData(string path) => Load(path);

	public TextureData(Stream stream) => Load(stream);

	public void Resize(int width, int height) {
		Width = width;
		Height = height;
		Pixels.Capacity = width * height;
	}

	public void Save(string path) {
		if(path != string.Empty) {
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
		if(file_info == null) {
			Log.Panic($"Invalid image file path: {file_path}");
		}

		Load(file_info?.OpenRead());
	}

	void Load(Stream data_stream) {
		using var reader = new BinaryReader(data_stream);

		var vif = new string(reader.ReadChars(3));

		if(vif.ToUpper() != "VIF") {
			Log.Error("Invalid VIF header");
		}

		var width = reader.ReadInt32();
		var height = reader.ReadInt32();
		var components = reader.ReadByte();
		var compressed = reader.ReadByte();

		if(width > 0 && height > 0 && components > 0) {
			Width = width;
			Height = height;
			Components = components;
			Compressed = compressed;
			Pixels = new List<IPixel>(width * height);

			var data_bytes = reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position);
			var data = compressed > 0 ? data_bytes.DecompressAsync().Result : data_bytes;

			for(var i = 0; i < data.Length; i += components) {
				switch(components) {
					case 4:
						Pixels.Add(new Pixel4(data[i], data[i + 1], data[i + 2], data[i + 3]));
						break;
					case 3:
						Pixels.Add(new Pixel3(data[i], data[i + 1], data[i + 2]));
						break;
					case 2:
						Pixels.Add(new Pixel2(data[i], data[i + 1]));
						break;
					case 1:
						Pixels.Add(new Pixel1(data[i]));
						break;
					default:
						break;
				}
			}
		}
	}
}
