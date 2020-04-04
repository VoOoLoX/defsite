using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Client {

	public class Texture {
		public int Height { get; private set; }

		public int ID { get; private set; }

		public int Width { get; private set; }

		public Texture(string path = "") {
			var texture = TextureFile.Default;

			// TODO: Kinda redundant and bad way of handling this, default texture should be set inside the texturefile class.
			if (File.Exists(path))
				texture = new TextureFile(path);

			Create(texture);
		}

		public Texture(TextureFile texture) => Create(texture);

		public void Disable() {
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public void Dispose() {
			GL.DeleteTexture(ID);
		}

		public void Enable() {
			GL.BindTexture(TextureTarget.Texture2D, ID);
		}

		void Create(TextureFile texture) {
			ID = GL.GenTexture();

			//TODO:
			//texture.Rotate180();
			//texture.FlipHorizontal();
			//UVs are inverted so there is no need proccess textures on the CPU

			Width = texture.Width;
			Height = texture.Height;

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.Bytes);
			Disable();
		}
	}
}