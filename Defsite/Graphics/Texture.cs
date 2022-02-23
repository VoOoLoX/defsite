using System.IO;
using Common;

using Defsite.IO.DataFormats;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics;

public enum TextureFilter {
	Nearest = TextureMinFilter.Nearest,
	Linear = TextureMinFilter.Linear
}

public class Texture {
	public static Texture Default = new(TextureData.Default);

	public TextureData TextureData { get; private set; }

	public TextureFilter TextureFilter { get; init; } = TextureFilter.Nearest;

	public int Width { get; private set; }

	public int Height { get; private set; }

	public int ID { get; private set; }

	public Texture(string path) {
		if(File.Exists(path)) {
			Create(new TextureData(path));
		} else {
			Log.Error($"Can't create texture, {path} doesn't exist.");
		}
	}

	public Texture(TextureData texture_data) => Create(texture_data);

	public Texture(int width, int height, byte components = 4) => Create(new TextureData(width, height, components));

	public void Enable() => GL.BindTexture(TextureTarget.Texture2D, ID);

	public void Disable() => GL.BindTexture(TextureTarget.Texture2D, 0);

	void Create(TextureData texture_data) {
		TextureData = texture_data;

		GL.CreateTextures(TextureTarget.Texture2D, 1, out int id);

		ID = id;
		Width = TextureData.Width;
		Height = TextureData.Height;

		Enable();
		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, TextureData.Bytes);
		GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureFilter);
		GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureFilter);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		Disable();
	}

	public void Resize(int width, int height) {
		TextureData.Resize(width, height);

		Width = TextureData.Width;
		Height = TextureData.Height;

		Enable();
		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, TextureData.Bytes);
		GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)(TextureMinFilter)TextureFilter);
		GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)(TextureMagFilter)TextureFilter);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		Disable();
	}

	public void Dispose() => GL.DeleteTexture(ID);
}
