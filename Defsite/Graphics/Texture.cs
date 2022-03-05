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

	public bool Multisampled { get; init; } = false;

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

	public void Bind() {
		if(Multisampled) {
			GL.BindTexture(TextureTarget.Texture2DMultisample, ID);
		} else {
			GL.BindTexture(TextureTarget.Texture2D, ID);
		}
	}

	public void BindToSlot(int slot) => GL.BindTextureUnit(slot, ID);

	public void Unbind() {
		if(Multisampled) {
			GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
		} else {
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}
	}

	void Create(TextureData texture_data) {
		TextureData = texture_data;

		int id;

		if(Multisampled) {
			GL.CreateTextures(TextureTarget.Texture2DMultisample, 1, out id);
		} else {
			GL.CreateTextures(TextureTarget.Texture2D, 1, out id);
		}

		ID = id;
		Width = TextureData.Width;
		Height = TextureData.Height;

		Bind();
		if(Multisampled) {
			GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 8, PixelInternalFormat.Rgba, Width, Height, false);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		} else {
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, TextureData.Bytes);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		}

		Unbind();
	}

	public void Resize(int width, int height) {
		TextureData.Resize(width, height);

		Width = TextureData.Width;
		Height = TextureData.Height;

		Bind();
		if(Multisampled) {
			GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 8, PixelInternalFormat.Rgba, Width, Height, true);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		} else {
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, TextureData.Bytes);
			GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)(TextureMinFilter)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)(TextureMagFilter)TextureFilter);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		}

		Unbind();
	}

	public void Dispose() => GL.DeleteTexture(ID);
}
