using System;
using System.Collections.Generic;
using OpenTK;

namespace Defsite {

	public class RectangleControl : Control {
		public RectangleControl(string name, Rectangle rectangle, Color color) {
			Name = name;
			Rectangle = rectangle;
			Create(rectangle, color);
		}

		public RectangleControl(Rectangle rectangle, Color color) {
			Name = Guid.NewGuid().ToString();
			Rectangle = rectangle;
			Create(rectangle, color);
		}

		void Create(Rectangle rectangle, Color color) {
			var color_vector = color.ToVector();
			Vertices.Add(new Vertex {
				Position = new Vector3(rectangle.X, rectangle.Y, 0),
				Color = color_vector,
			});
			Vertices.Add(new Vertex {
				Position = new Vector3(rectangle.X + rectangle.Width, rectangle.Y, 0),
				Color = color_vector,
			});
			Vertices.Add(new Vertex {
				Position = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0),
				Color = color_vector,
			});
			Vertices.Add(new Vertex {
				Position = new Vector3(rectangle.X, rectangle.Y + rectangle.Height, 0),
				Color = color_vector,
			});
		}
	}
}