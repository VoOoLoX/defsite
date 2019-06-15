using OpenTK.Audio.OpenAL;

namespace Client {
	public class SoundBuffer {
		public SoundBuffer(ALFormat format, byte[] data, int sample_rate) {
			ID = AL.GenBuffer();
			AL.BufferData(ID, format, data, data.Length, sample_rate);
		}

		public int ID { get; }
	}
}