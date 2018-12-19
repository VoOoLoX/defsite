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
			Enable();
			var pixels = new List<Pixel>() { new Pixel(255, 0, 255, 255) };
			var img = new VIFImage(1, 1, pixels);

			if (File.Exists(path)) {
				if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) {
					var sprite = new System.Drawing.Bitmap(path);

					pixels.Clear();

					for (var y = 0; y < sprite.Height; y++)
						for (var x = 0; x < sprite.Width; x++) {
							var p = sprite.GetPixel(x, y);
							pixels.Add(new Pixel(p.R, p.G, p.B, p.A));
						}

					img = new VIFImage(sprite.Width, sprite.Height, pixels);
				} else {
					img = VIF.Load(path);
				}
			}

			img.Rotate180();
			img.FlipHorizontal();

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Bytes());
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
		}
		public void Enable() => GL.BindTexture(TextureTarget.Texture2D, ID);

		public void Disable() => GL.BindTexture(TextureTarget.Texture2D, 0);
	}
}
