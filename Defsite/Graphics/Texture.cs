using System.IO;
using Common;
using OpenTK.Graphics.OpenGL;

namespace Defsite {

	public class Texture {
		public static Texture Default = new(TextureFile.Default);
		
		public TextureFile TextureFile { get; private set; }
		
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
			TextureFile = texture_file;
			
			GL.CreateTextures(TextureTarget.Texture2D, 1, out int id);

			ID = id; 

			Width = TextureFile.Width;
			Height = TextureFile.Height;

			Enable();
			GL.TextureStorage2D(ID, 1, SizedInternalFormat.Rgba8, Width, Height);
			GL.TextureSubImage2D(ID, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, TextureFile.Bytes);
			GL.GenerateTextureMipmap(ID);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			Disable();
		}
	}
}