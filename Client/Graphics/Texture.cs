using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Texture {
		int height;
		int width;

		public Texture(string path = "") {
			var texture = TextureFile.Default;

			// TODO: Kinda redundant and bad way of handling this, default texture should be set inside the texturefile class.
			if (File.Exists(path))
				texture = new TextureFile(path);

			Create(texture);
		}

		public Texture(TextureFile texture) => Create(texture);

		void Create(TextureFile texture) {
			ID = GL.GenTexture();

			//TODO: 
//			texture.Rotate180();
//			texture.FlipHorizontal();
			//UVs are inverted so there is no need proccess textures on the CPU

			width = texture.Width;
			height = texture.Height;

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.Bytes);
			Disable();
		}

		public int ID { get; private set; }
		public int Width => width;
		public int Height => height;

		public void Enable() {
			GL.BindTexture(TextureTarget.Texture2D, ID);
		}

		public void Disable() {
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		~Texture() {
			GL.DeleteTexture(ID);
		}
	}
}