using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Client {
	public class Texture {
		public int ID { get; private set; }

		public Texture(string path) {
			ID = GL.GenTexture();
			var pixels = new List<Pixel>() { new Pixel(255, 0, 255, 255) };
			var img = new VIFImage(1, 1, pixels);

			if (File.Exists(path))
				img = VIF.Load(path);

			img.Rotate180();
			img.FlipHorizontal();

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Bytes());
			Disable();
		}
		public void Enable() => GL.BindTexture(TextureTarget.Texture2D, ID);

		public void Disable() => GL.BindTexture(TextureTarget.Texture2D, 0);

		~Texture() {
			GL.DeleteTexture(ID);
		}
	}
}
