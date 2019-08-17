using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Client {
	public class Texture {
		int height;
		int width;

		public Texture() {
			ID = GL.GenTexture();

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ImageFile.Default.Width, ImageFile.Default.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ImageFile.Default.Bytes);
			Disable();
		}

		public Texture(string path) {
			ID = GL.GenTexture();

			var img = ImageFile.Default;

			if (File.Exists(path))
				img = new ImageFile(path);

			img.Rotate180();
			img.FlipHorizontal();

			width = img.Width;
			height = img.Height;

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Bytes);
			Disable();
		}

		public Texture(ImageFile image) {
			ID = GL.GenTexture();

//			image.Rotate180();
//			image.FlipHorizontal();

			width = image.Width;
			height = image.Height;

			Enable();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Bytes);
			Disable();
		}

		public int ID { get; }
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