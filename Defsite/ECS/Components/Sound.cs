namespace Defsite {

	public class SoundComponent : Component {

		public bool IsPlaying { get; private set; }

		public Sound SoundSource { get; }

		public SoundComponent() {
		}

		public SoundComponent(Sound sound) {
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