using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using OpenTK.Graphics.OpenGL;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System.Linq;
using SixLabors.ImageSharp.Processing.Overlays;

namespace Client {
	public class Texture {
		public int ID { get; private set; }

		public Texture(string path) : this(new FileInfo(path)) { }


		public Texture(VIFImage img) {
			ID = GL.GenTexture();
			Enable();

			var w = img.Width;
			var h = img.Height;
			var data = img.Bytes();

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
		}

		public Texture(FileInfo file) {
			ID = GL.GenTexture();
			Enable();

			var w = 0;
			var h = 0;
			byte[] data;

			using (var img = Image.Load<Rgba32>(file.FullName)) {
				w = img.Width;
				h = img.Height;
				img.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));
				data = img.SavePixelData();
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
		}

		public void Enable() => GL.BindTexture(TextureTarget.Texture2D, ID);
		public void Disable() => GL.BindTexture(TextureTarget.Texture2D, 0);
	}
}
