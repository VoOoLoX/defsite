using System;
using Defsite.Core;

using OpenTK.Mathematics;

namespace Defsite.Graphics.Cameras;
public class FirstPersonCamera : ICamera {

	float pitch;
	float yaw = -MathHelper.PiOver2;
	float fov = MathHelper.PiOver2;

	float old_x, old_y;
	float delta_x, delta_y;

	public FirstPersonCamera(Vector3 position, float client_width, float client_height, float sensitivity = 0.2f, float fov = 45, float z_near = 0.001f, float z_far = 100f) {
		Position = position;
		Fov = fov;
		ZNear = z_near;
		ZFar = z_far;
		Sensitivity = sensitivity;
		ClientWidth = client_width;
		ClientHeight = client_height;
	}

	public bool FirstUpdate { get; set; } = true;

	public Vector3 Position { get; set; }

	public Vector3 Forward { get; private set; }

	public Vector3 Right { get; private set; }

	public Vector3 Up { get; private set; }

	public float Sensitivity { get; set; }

	public float ZFar { get; set; }

	public float ZNear { get; set; }

	public float ClientWidth { get; set; }

	public float ClientHeight { get; set; }

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

	public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(fov, ClientWidth / ClientHeight, ZNear, ZFar);

	public Matrix4 ViewMatrix {
		get {
			UpdateVectors();
			return Matrix4.LookAt(Position, Position + Forward, Up);
		}
	}

	public void UpdateAspectRatio(float client_width, float client_height) {
		ClientWidth = client_width;
		ClientHeight = client_height;
	}

	public void Update() {
		var mouse = Input.MousePosition;

		if(FirstUpdate) {
			old_x = mouse.X;
			old_y = mouse.Y;
			FirstUpdate = false;
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