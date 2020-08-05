using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Defsite;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Client {
	public class FirstPersonCamera {

		float pitch;

		float yaw = -MathHelper.PiOver2;

		float fov = MathHelper.PiOver2;

		float z_far;

		float z_near;

		public FirstPersonCamera(Vector3 position, float fov = 45, float z_near = 0.001f, float z_far = 1000f) {
			Position = position;
			Fov = fov;
			ZNear = z_near;
			ZFar = z_far;
		}

		public Vector3 Position { get; set; }

		public Vector3 Front { get; private set; }

		public Vector3 Up { get; private set; }

		public Vector3 Right { get; private set; }

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
			return Matrix4.LookAt(Position, Position + Front, Up);
		}

		public Matrix4 GetProjectionMatrix() {
			return Matrix4.CreatePerspectiveFieldOfView(fov, (float)Window.Width / Window.Height, ZNear, ZFar);
		}

		void UpdateVectors() {
			var front = new Vector3(
				(float)Math.Cos(pitch) * (float)Math.Cos(yaw),
				(float)Math.Sin(pitch),
				(float)Math.Cos(pitch) * (float)Math.Sin(yaw)
			);

			Front = Vector3.Normalize(front);

			Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));

			Up = Vector3.Normalize(Vector3.Cross(Right, Front));
		}
	}
}