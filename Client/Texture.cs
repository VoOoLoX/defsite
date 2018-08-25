using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Linq;

namespace Client {
	public class Texture {
		public int ID { get; private set; }

		public Texture(string path) {
			ID = GL.GenTexture();
			Enable();

			var img = new VIFImage(1, 1, new List<Pixel>() { new Pixel(255, 0, 255, 255) });

			if (File.Exists(path))
				img = VIF.Load(path);
			var w = img.Width;
			var h = img.Height;
			img.Rotate180();
			img.FlipHorizontal();
			var data = img.Bytes();

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
		}
		public void Enable() => GL.BindTexture(TextureTarget.Texture2D, ID);
		public void Disable() => GL.BindTexture(TextureTarget.Texture2D, 0);
	}
}
