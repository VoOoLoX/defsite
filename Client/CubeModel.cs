using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client {
	public class Cube {
		public int Width;
		public int Height;
		public int Depth;

		public int X;
		public int Y;
		public int Z;

		public Cube(int w, int h, int d, int x, int y, int z) {
			Width = w;
			Height = h;
			Depth = d;
			X = x;
			Y = y;
			Z = z;
		}

		public static Cube Zero => new Cube(20, 20, 80, 0, 0, 0);
	}

	class CubeModel : Model {
		VertextArray va = new VertextArray();
		VertexBuffer<Vector3> vbo_pos = new VertexBuffer<Vector3>(Primitives.CubeCentered.PositionData);
		IndexBuffer ib = new IndexBuffer(Primitives.CubeCentered.IndexBufferData);

		static Shader shader = AssetManager.Get<Shader>("ColorShader");

		Vector3 cube_world_position = Vector3.Zero;
		Cube cube_screen = Cube.Zero;

		float cube_scale_x = 1;
		float cube_scale_y = 1;
		float cube_scale_z = 1;

		Color cube_color = new Color(0, 0, 0, 255);

		public CubeModel(Color color = default) {
			VA.Enable();

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 3);

			ResizeCube(cube_screen.Width, cube_screen.Height, cube_screen.Depth);
			MoveCube(cube_screen.X, cube_screen.Y, cube_screen.Z);

			if (color != default)
				cube_color = color;

			SetColor(cube_color);

			VA.Disable();
		}

		void SetColor(Color color) => cube_color = color;

		void ResizeCube(int width, int height, int depth) {
			cube_screen.Width = width;
			cube_screen.Height = height;
			cube_screen.Depth = depth;

			var sx = cube_screen.Width / ClientUtils.WorldUnitToScreen(cube_scale_x);
			var sy = cube_screen.Height / ClientUtils.WorldUnitToScreen(cube_scale_y);
			var sz = cube_scale_z;

			cube_scale_x *= sx;
			cube_scale_y *= sy;

			MoveCube(Window.ClientCenter.X, Window.ClientCenter.Y, 0);
			Scale(sx, sy, sz);
			MoveCube(cube_screen.X, cube_screen.Y, cube_screen.Z);
		}

		void MoveCube(int x, int y, int z) {
			cube_screen.X = x;
			cube_screen.Y = y;

			var move_pos = ClientUtils.ScreenToWorld(x, y, z);

			Move(move_pos.X - cube_world_position.X, -move_pos.Y + cube_world_position.Y);
			cube_world_position = move_pos;
		}

		public override void PreDraw() => Shader.SetUniform("color", Color.WhiteSmoke);

		public int Width { get => (int)ClientUtils.WorldUnitToScreen(cube_scale_x); set => ResizeCube(value, Height, Depth); }

		public int Height { get => (int)ClientUtils.WorldUnitToScreen(cube_scale_y); set => ResizeCube(Width, value, Depth); }

		public int Depth { get => (int)ClientUtils.WorldUnitToScreen(cube_scale_z); set => ResizeCube(Width, Height, value); }

		public int X { get => cube_screen.X; set => MoveCube(value, cube_screen.Y, cube_screen.Z); }

		public int Y { get => cube_screen.Y; set => MoveCube(cube_screen.X, value, cube_screen.Z); }

		public int Z { get => cube_screen.Z; set => MoveCube(cube_screen.X, cube_screen.Y, value); }

		public Cube Cube { get => new Cube(cube_screen.X, cube_screen.Y, cube_screen.Z, Width, Height, Depth); set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; } }

		public Color Color { get => cube_color; set => SetColor(value); }

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;
	}
}
