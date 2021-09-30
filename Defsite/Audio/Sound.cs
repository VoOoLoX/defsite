using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace Defsite.Audio;

public class Sound {

	public int ID { get; }

	public Vector3 Position {
		get {
			AL.GetSource(ID, ALSource3f.Position, out var vec);
			return vec;
		}
		set => AL.Source(ID, ALSource3f.Position, value.X, value.Y, value.Z);
	}

	public Sound(string path) : this(path, Vector3.Zero) {
	}

	public Sound(string path, Vector3 position) {
		var buffer = new SoundBuffer(path);
		ID = AL.GenSource();
		AL.Source(ID, ALSourcei.Buffer, buffer.ID);

		AL.DistanceModel(ALDistanceModel.ExponentDistance);
		AL.Source(ID, ALSourcef.Gain, 1.0f);
		AL.Source(ID, ALSourcef.Pitch, 1.0f);
		AL.Source(ID, ALSource3f.Position, position.X, position.Y, position.Z);
		AL.Source(ID, ALSourcef.RolloffFactor, 1);
		AL.Source(ID, ALSourcef.ReferenceDistance, 1f);
		AL.Source(ID, ALSourcef.MaxDistance, 10);
	}
	public void Pause() => AL.SourcePause(ID);

	public void Play() => AL.SourcePlay(ID);
	public void Rewind() => AL.SourceRewind(ID);

	public void Stop() => AL.SourceStop(ID);
}