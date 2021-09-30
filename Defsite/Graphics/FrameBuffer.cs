using OpenTK.Graphics.OpenGL;

namespace Defsite.Graphics;

public class FrameBuffer {
	public int ID { get; }
	public Texture Texture { get; private set; }

	public FrameBuffer(Texture texture) {
		ID = GL.GenFramebuffer();
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
		SetData(texture);
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void Disable() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

	public void Enable() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

	public void SetData(Texture texture) {
		Texture = texture;

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

		Texture.Enable();

		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.ID, 0);

		Texture.Disable();

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
