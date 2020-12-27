namespace Defsite {

	public class MainScene : Scene {

		//Text fps = new Text("");

		//Text mouse = new Text("");
		//Entity player;

		public MainScene() {
			//SpriteRenderer = new SpriteRenderer(Camera);

			//player = new Entity();
			//player.AddComponent(new Transform());
			//player.AddComponent(new Sprite(Assets.Get<Texture>("Ground")) { Billboard = true });
			//player.AddComponent(new Sound(Assets.Get<SoundSource>("Fireplace")));
			//AddEntity(player);


			//Controls.Add(new RectangleControl(new Rectangle(0, 0, 100, Window.Height), new Color(255, 16, 16, 255)));

			//var fps_transform = fps.GetComponent<Transform>();
			//fps_transform.ScaleXY = 1;
			//fps_transform.Position = new Vector3(0, 0, 0);
			//Controls.Add(fps);

			//var mouse_transform = mouse.GetComponent<Transform>();
			//mouse_transform.ScaleXY =5;
			//mouse_transform.Position = new Vector3(0, 50, 0);
			//Controls.Add(mouse);
		}

		// public override void Render(float time) {
		//base.Render(time);
		//fps.Value = $"FPS : {MathF.Ceiling(1 / time)}";
		// }

		// public override void Update(float time) {
		//base.Update(time);
		//mouse.Value = $"{Input.MousePos.X}x{Input.MousePos.Y}\n{Input.IsActive(MouseButton.Left)} {Input.IsActive(MouseButton.Right)}";

		//var camera_direction_vector = Vector3.Zero;

		//var player_direction_vector = Vector3.Zero;

		//var rotation_vector = Vector3.Zero;

		//if (Input.IsActive(Key.Right))
		//	camera_direction_vector.X = -1;

		//if (Input.IsActive(Key.Left))
		//	camera_direction_vector.X = 1;

		//if (Input.IsActive(Key.Up))
		//	camera_direction_vector.Y = -1;

		//if (Input.IsActive(Key.Down))
		//	camera_direction_vector.Y = 1;

		//if (Input.IsActive(Key.D))
		//	player_direction_vector.X = 1;

		//if (Input.IsActive(Key.A))
		//	player_direction_vector.X = -1;

		//if (Input.IsActive(Key.W))
		//	player_direction_vector.Y = 1;

		//if (Input.IsActive(Key.S))
		//	player_direction_vector.Y = -1;

		//if (Input.IsActive(Key.E))
		//	camera_direction_vector.Z = 1;

		//if (Input.IsActive(Key.Q))
		//	camera_direction_vector.Z = -1;

		//if (Input.IsActive(Key.X))
		//	rotation_vector.Z = -1;

		//if (Input.IsActive(Key.Z))
		//	rotation_vector.Z = 1;

		//if (Input.IsActive(Key.R)) {
		//	var s = player.GetComponent<Sound>();
		//	if (!s.IsPlaying)
		//		s.Play();
		//	else
		//		s.Pause();
		//}

		//if (Input.IsActive(MouseButton.Left) && !clicked) {
		//	clicked = true;
		//	var p = ScreenToWorld(Input.MousePos.X, Input.MousePos.Y);
		//	var e = new Entity();

		//	e.AddComponent(new Transform {
		//		Position = new Vector3(p.X, p.Y, 0),
		//		Scale = new Vector3(-.5f)
		//	});

		//	e.AddComponent(new Sprite(Assets.Get<Texture>("Ground")));
		//	AddEntity(e);
		//}

		//if (!Input.IsActive(MouseButton.Left))
		//	clicked = false;

		//if (camera_direction_vector != Vector3.Zero || rotation_vector != Vector3.Zero || player_direction_vector != Vector3.Zero) {
		//	camera_direction_vector.NormalizeFast();
		//	player_direction_vector.NormalizeFast();
		//	rotation_vector.NormalizeFast();
		//}

		//var ct = Camera.GetComponent<Transform>();


		//var pt = player.GetComponent<Transform>();

		//var m = Matrix4.CreateTranslation(player_direction_vector * time);
		//var r = Matrix4.CreateRotationZ(rotation_vector.Z * time);

		//pt.Matrix *= m;

		//ct.Matrix *= m;

		////pt.Matrix *= r;

		////ct.Matrix *= r;


		//ct.MoveBy(camera_direction_vector * 20 * time);
		// }
	}
}