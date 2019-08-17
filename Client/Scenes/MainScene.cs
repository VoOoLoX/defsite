using System;
using OpenTK;
using OpenTK.Input;

namespace Client {
	public class MainScene : Scene {
		bool clicked;
		Text fps = new Text("");
		Text mouse = new Text("");
		Entity Player;

		public MainScene() {
			Player = new Entity();
			Player.AddComponent(new Transform());
			Player.AddComponent(new Sprite(Assets.Get<Texture>("Ground")));
			Player.AddComponent(new Sound(Assets.Get<SoundSource>("Fireplace")));
			AddEntity(Player);

			var t = new Entity();
			t.AddComponent(new Transform() {
				Position = new Vector3(30, 30, 0),
				Scale = new Vector3(25, 25, 1)
			});
			t.AddComponent(new Sprite(Assets.Get<Texture>("Ground")));
			Controls.Add(t);

			fps.GetComponent<Transform>().ScaleXY = 2;
			Controls.Add(fps);
			mouse.GetComponent<Transform>().ScaleXY = 2;
			mouse.GetComponent<Transform>().Position = new Vector3(0, 50, 0);
			Controls.Add(mouse);
		}

		public override void Render(float time) {
			base.Render(time);
			fps.Value = $"{MathF.Ceiling(1 / time)}";
		}

		public override void Update(float time) {
			base.Update(time);
			mouse.Value = $"{Input.MousePos.X}x{Input.MousePos.Y} {Input.IsActive(MouseButton.Left)} {Input.IsActive(MouseButton.Right)}";

			var camera_direction_vector = Vector3.Zero;

			var player_direction_vector = Vector3.Zero;

			var rotation_vector = Vector3.Zero;

			if (Input.IsActive(Key.Right))
				camera_direction_vector.X = -1;

			if (Input.IsActive(Key.Left))
				camera_direction_vector.X = 1;

			if (Input.IsActive(Key.Up))
				camera_direction_vector.Y = -1;

			if (Input.IsActive(Key.Down))
				camera_direction_vector.Y = 1;

			if (Input.IsActive(Key.D))
				player_direction_vector.X = 1;

			if (Input.IsActive(Key.A))
				player_direction_vector.X = -1;

			if (Input.IsActive(Key.W))
				player_direction_vector.Y = 1;

			if (Input.IsActive(Key.S))
				player_direction_vector.Y = -1;

			if (Input.IsActive(Key.E))
				camera_direction_vector.Z = 1;

			if (Input.IsActive(Key.Q))
				camera_direction_vector.Z = -1;

			if (Input.IsActive(Key.X))
				rotation_vector.Z = 1;

			if (Input.IsActive(Key.Z))
				rotation_vector.Z = -1;

			if (Input.IsActive(Key.R)) {
				var s = GetEntity(5).GetComponent<Sound>();
				if (!s.IsPlaying)
					s.Play();
				else
					s.Pause();
			}

			if (Input.IsActive(MouseButton.Left) && !clicked) {
				clicked = true;
				var p = ScreenToWorld(Input.MousePos);
				var e = new Entity();


				e.AddComponent(new Transform {
					Position = new Vector3(p.X, p.Y, 0),
					Scale = new Vector3(-.5f)
				});

				e.AddComponent(new Sprite(Assets.Get<Texture>("Ground")));
				AddEntity(e);
			}

			if (!Input.IsActive(MouseButton.Left))
				clicked = false;

//			direction_vector.X = Joystick.GetState(0).IsConnected ? -Joystick.GetState(0).GetAxis(JoystickAxis.Axis0) : direction_vector.X;
//			direction_vector.Y = Joystick.GetState(0).IsConnected ? Joystick.GetState(0).GetAxis(JoystickAxis.Axis1) : direction_vector.Y;

//			Log.Info($"X:{MathHelper.RadiansToDegrees(Camera.Transform.Rotation.X)} Y:{Camera.Transform.Rotation.Y} Z:{Camera.Transform.Rotation.Z}");


			//For some FUCKING reason this works kinda but the error gets larger with the increase in rotation
			//Find the proper way to do this

			if (camera_direction_vector != Vector3.Zero || rotation_vector != Vector3.Zero || player_direction_vector != Vector3.Zero) {
				camera_direction_vector.NormalizeFast();
				player_direction_vector.NormalizeFast();
				rotation_vector.NormalizeFast();
			}

			Camera.GetComponent<Transform>().MoveBy(camera_direction_vector * 10 * time);

			Player.GetComponent<Transform>().RotateTo(0, 0, -rotation_vector.Z * 30 * time);

			Camera.GetComponent<Transform>().RotateBy(0, 0, rotation_vector.Z * 30 * time);

//			var rot_q = Matrix4.CreateRotationZ(MathHelper.RadiansToDegrees(-Camera.GetComponent<Transform>().Rotation.X / 30)).ExtractRotation();
//			
//			player_direction_vector = Vector2.Transform(player_direction_vector, rot_q);

			Player.GetComponent<Transform>().MoveBy(player_direction_vector * 10 * time);
		}
	}
}