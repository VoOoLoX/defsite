using System.IO;
using OpenTK.Audio.OpenAL;

namespace Defsite {

	public class SoundBuffer {

		public int ID { get; }

		public SoundBuffer(string path) {
			ID = AL.GenBuffer();

			var sound = SoundFile.Default;

			if (File.Exists(path))
				sound = new SoundFile(path);

			AL.BufferData(ID, sound.Format, sound.Data, sound.Data.Length, sound.SampleRate);
		}

		public SoundBuffer(ALFormat format, byte[] data, int sample_rate) {
			ID = AL.GenBuffer();
			AL.BufferData(ID, format, data, data.Length, sample_rate);
		}
	}
}