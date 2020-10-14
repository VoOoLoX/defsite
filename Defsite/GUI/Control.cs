using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Defsite {

	public class Control {
		public string Name { get; protected set; }
		public Rectangle Rectangle { get; protected set; }
		public List<Vertex> Vertices { get; protected set; }
		public Control Parent { get; protected set; } = null;
		public List<Control> Children { get; protected set; }

		public void Add(Control control) {
			// Checl if control with same name already exists in children
			control.Parent = this;
			Children.Add(control);
		}
	}
}
