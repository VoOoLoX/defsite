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

			AL.BufferData(ID, sound.Format, ref sound.Data[0], sound.Data.Length, sound.SampleRate);
		}

		public SoundBuffer(ALFormat format, byte[] data, int sample_rate) {
			ID = AL.GenBuffer();
			AL.BufferData(ID, format, ref data[0], data.Length, sample_rate);
		}
	}
}