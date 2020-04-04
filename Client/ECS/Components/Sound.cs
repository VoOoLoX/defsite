namespace Client {

	public class Sound : Component {

		public bool IsPlaying { get; private set; }

		public SoundSource SoundSource { get; }

		public Sound() {
		}

		public Sound(SoundSource sound) {
			SoundSource = sound;
		}
		public void Pause() {
			SoundSource.Pause();
			IsPlaying = false;
		}

		public void Play() {
			SoundSource.Play();
			IsPlaying = true;
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