using System.Collections.Generic;
using OpenTK;

namespace Client {

	public class GUIRectangle : Entity {

		public GUIRectangle(OpenTK.Rectangle rectangle, Color color) {
			var texture = new Texture(new TextureFile(1, 1, new List<Pixel> { new Pixel(color) }));
			AddComponent(new Sprite(texture));
			AddComponent(new Transform() {
				Position = new Vector3(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f, 0),
				Scale = new Vector3(rectangle.Width / 2f, rectangle.Height / 2f, 0)
			});
		}
	}
}