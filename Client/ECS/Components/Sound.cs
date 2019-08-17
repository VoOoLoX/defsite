namespace Client {
	public class Sound : Component {
		public Sound() { }

		public Sound(SoundSource sound) {
			SoundSource = sound;
		}

		public SoundSource SoundSource { get; }

		public bool IsPlaying { get; private set; }

		public void Play() {
			SoundSource.Play();
			IsPlaying = true;
		}

		public void Pause() {
			SoundSource.Pause();
			IsPlaying = false;
		}

		public void Rewind() {
			SoundSource.Rewind();
		}

		public void Stop() {
			SoundSource.Stop();
			IsPlaying = false;
		}
	}
}