using OpenTK.Audio.OpenAL;

namespace Client {
	public class SoundSource {
		public SoundSource() {
			ID = AL.GenSource();
			AL.Source(ID, ALSourcef.Gain, 1.0f);
			AL.Source(ID, ALSourcef.Pitch, 1.0f);
			AL.Source(ID, ALSource3f.Position, 0, 0, 0);
		}

		public int ID { get; }

		public void Play(SoundBuffer buffer) {
			AL.Source(ID, ALSourcei.Buffer, buffer.ID);
			AL.SourcePlay(ID);
		}

		public void Stop() {
			AL.SourceStop(ID);
		}
	}
}