using OpenTK;
using OpenTK.Audio.OpenAL;

namespace Client {

	public static class SoundListener {

		public static Vector3 Position {
			get {
				AL.GetListener(ALListener3f.Position, out var vec);
				return vec;
			}
			set => AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
		}

		public static void Init() {
			AL.Listener(ALListener3f.Position, 0, 0, 0);
			AL.Listener(ALListener3f.Velocity, 0, 0, 0);
		}
	}
}