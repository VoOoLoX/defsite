using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace Defsite.Audio;

public static class SoundListener {

	public static Vector3 Position {
		get {
			AL.GetListener(ALListener3f.Position, out var vec);
			return vec;
		}
		set => AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
	}

	public static (Vector3 forward, Vector3 up) Orientation {
		get {
			AL.GetListener(ALListenerfv.Orientation, out var forward, out var up);
			return (forward, up);
		}
		set => AL.Listener(ALListenerfv.Orientation, ref value.forward, ref value.up);
	}

	public static void Init() {
		AL.Listener(ALListener3f.Position, 0, 0, 0);
		AL.Listener(ALListener3f.Velocity, 0, 0, 0);
	}
}