using OpenTK.Graphics.OpenGL4;

namespace Defsite.Graphics.Buffers;

public class FrameBuffer {
	public int ID { get; }
	public Texture ColorTexture { get; private set; }
	public Texture DepthTexture { get; private set; }

	public FrameBuffer(int width, int height) {
		ID = GL.GenFramebuffer();
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
		Create(width, height);
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void Bind() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

	public void Unbind() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

	void Create(int width, int height) {
		ColorTexture = new Texture(width, height, 3) {
			TextureFilter = TextureFilter.Linear,
			Multisampled = false
		};
		DepthTexture = new Texture(width, height, 3) {
			TextureFilter = TextureFilter.Linear,
			Multisampled = false
		};
		SetData();
	}

	public void Resize(int width, int height) {
		ColorTexture.Resize(width, height);
		DepthTexture.Resize(width, height);
	}

	public void SetData() {
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

		ColorTexture.Bind();
		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, ColorTexture.ID, 0);
		ColorTexture.Unbind();

		DepthTexture.Bind();
		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2DMultisample, DepthTexture.ID, 0);
		DepthTexture.Unbind();

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
