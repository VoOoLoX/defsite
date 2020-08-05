using System.IO;
using Defsite;
using OpenTK.Graphics.OpenGL;

namespace Client {

	public class Texture {
		public static Texture Default = new Texture(TextureFile.Default);
		public int Height { get; private set; }

		public int ID { get; private set; }

		public int Width { get; private set; }

		public Texture(string path) {
			if (File.Exists(path))
				Create(new TextureFile(path));
			else
				Log.Error($"Can't create texture, {path} doesn't exist.");
		}

		public Texture(TextureFile texture_file) => Create(texture_file);

		public void Disable() {
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public void Dispose() {
			GL.DeleteTexture(ID);
		}

		public void Enable() {
			GL.BindTexture(TextureTarget.Texture2D, ID);
		}

		void Create(TextureFile texture_file) {
			ID = GL.GenTexture();

			//TODO:
			//texture.Rotate180();
			//texture.FlipHorizontal();
			//UVs are inverted so there is no need proccess textures on the CPU

			Width = texture_file.Width;
			Height = texture_file.Height;

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture_file.Width, texture_file.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture_file.Bytes);
			Disable();
		}
	}
}