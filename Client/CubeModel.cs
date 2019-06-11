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

		Vector3 CubeWorldPosition = Vector3.Zero;
		Cube CubeScreen = Cube.Zero;

		float CubeScaleX = 1;
		float CubeScaleY = 1;
		float CubeScaleZ = 1;

		Color CubeColor = new Color(0, 0, 0, 255);

		public CubeModel(Color color = default) {
			VA.Enable();

			var pos = Shader.GetAttribute("position");
			VA.AddBuffer(vbo_pos, pos, 3);

			ResizeCube(CubeScreen.Width, CubeScreen.Height, CubeScreen.Depth);
			MoveCube(CubeScreen.X, CubeScreen.Y, CubeScreen.Z);

			if (color != default)
				CubeColor = color;

			SetColor(CubeColor);

			VA.Disable();
		}

		public void SetColor(Color color) => CubeColor = color;

		public void ResizeCube(int width, int height, int depth) {
			CubeScreen.Width = width;
			CubeScreen.Height = height;
			CubeScreen.Depth = depth;

			var sx = CubeScreen.Width / ClientUtils.WorldUnitToScreen(CubeScaleX);
			var sy = CubeScreen.Height / ClientUtils.WorldUnitToScreen(CubeScaleY);
			var sz = CubeScaleZ;

			CubeScaleX *= sx;
			CubeScaleY *= sy;

			MoveCube(Window.ClientCenter.X, Window.ClientCenter.Y, 0);
			Scale(sx, sy, sz);
			MoveCube(CubeScreen.X, CubeScreen.Y, CubeScreen.Z);
		}

		public void MoveCube(int x, int y, int z) {
			CubeScreen.X = x;
			CubeScreen.Y = y;

			var move_pos = ClientUtils.ScreenToWorld(x, y, z);

			Move(move_pos.X - CubeWorldPosition.X, -move_pos.Y + CubeWorldPosition.Y);
			CubeWorldPosition = move_pos;
		}

		public override void PreDraw() => Shader.SetUniform("color", Color.WhiteSmoke);

		public int Width { get => (int)ClientUtils.WorldUnitToScreen(CubeScaleX); set => ResizeCube(value, Height, Depth); }

		public int Height { get => (int)ClientUtils.WorldUnitToScreen(CubeScaleY); set => ResizeCube(Width, value, Depth); }

		public int Depth { get => (int)ClientUtils.WorldUnitToScreen(CubeScaleZ); set => ResizeCube(Width, Height, value); }

		public int X { get => CubeScreen.X; set => MoveCube(value, CubeScreen.Y, CubeScreen.Z); }

		public int Y { get => CubeScreen.Y; set => MoveCube(CubeScreen.X, value, CubeScreen.Z); }

		public int Z { get => CubeScreen.Z; set => MoveCube(CubeScreen.X, CubeScreen.Y, value); }

		public Cube Rect { get => new Cube(CubeScreen.X, CubeScreen.Y, CubeScreen.Z, Width, Height, Depth); set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; } }

		public Color Color { get => CubeColor; set => SetColor(value); }

		public override Shader Shader => shader;

		public override VertextArray VA => va;

		public override IndexBuffer IB => ib;
	}
}
