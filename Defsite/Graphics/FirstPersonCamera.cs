
using System;
using Defsite.Core;

using OpenTK.Mathematics;

namespace Defsite.Graphics;
public class FirstPersonCamera {

	float pitch;

	float yaw = -MathHelper.PiOver2;
	float fov = MathHelper.PiOver2;

	bool move = true;

	float old_x, old_y;
	float delta_x, delta_y;

	public FirstPersonCamera(Vector3 position, float sensitivity = 0.2f, float fov = 45, float z_near = 0.001f, float z_far = 100f) {
		Position = position;
		Fov = fov;
		ZNear = z_near;
		ZFar = z_far;
		Sensitivity = sensitivity;
	}

	public Vector3 Position { get; set; }

	public Vector3 Forward { get; private set; }

	public Vector3 Right { get; private set; }

	public Vector3 Up { get; private set; }

	public float Sensitivity { get; set; }

	public float ZFar { get; set; }

	public float ZNear { get; set; }

	public float Fov {
		get => MathHelper.RadiansToDegrees(fov);
		set {
			var angle = MathHelper.Clamp(value, 1f, 45f);
			fov = MathHelper.DegreesToRadians(angle);
		}
	}

	public float Pitch {
		get => MathHelper.RadiansToDegrees(pitch);
		set {
			var angle = MathHelper.Clamp(value, -89f, 89f);
			pitch = MathHelper.DegreesToRadians(angle);
			UpdateVectors();
		}
	}

	public float Yaw {
		get => MathHelper.RadiansToDegrees(yaw);
		set {
			yaw = MathHelper.DegreesToRadians(value);
			UpdateVectors();
		}
	}

	public Matrix4 GetViewMatrix() {
		UpdateVectors();
		return Matrix4.LookAt(Position, Position + Forward, Up);
	}

	public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(fov, (float)Playground.GameWidth / Playground.GameHeight, ZNear, ZFar);

	//TODO Make it so camera can be paused and resumed regardless of mouse movement when it's paused
	public void Update() {
		var mouse = Input.MousePosition;

		if(move) {
			old_x = mouse.X;
			old_y = mouse.Y;
			move = false;
		} else {
			delta_x += mouse.X - old_x;
			delta_y += mouse.Y - old_y;

			old_x = mouse.X;
			old_y = mouse.Y;

			Yaw = delta_x * Sensitivity;
			Pitch = -delta_y * Sensitivity;
		}
	}

	void UpdateVectors() {
		var front = new Vector3(
			(float)Math.Cos(pitch) * (float)Math.Cos(yaw),
			(float)Math.Sin(pitch),
			(float)Math.Cos(pitch) * (float)Math.Sin(yaw)
		);

		Forward = Vector3.Normalize(front);

		Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));

		Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
	}
}
