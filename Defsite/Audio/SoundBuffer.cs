
using System.IO;
using Defsite.IO.DataFormats;

using OpenTK.Audio.OpenAL;

namespace Defsite.Audio;

public class SoundBuffer {

	public int ID { get; }

	public SoundBuffer(string path) {
		ID = AL.GenBuffer();

		var sound = SoundData.Default;

		if(File.Exists(path)) {
			sound = new SoundData(path);
		}

		AL.BufferData(ID, sound.Format, ref sound.Data[0], sound.Data.Length, sound.SampleRate);
	}

	public SoundBuffer(ALFormat format, byte[] data, int sample_rate) {
		ID = AL.GenBuffer();
		AL.BufferData(ID, format, ref data[0], data.Length, sample_rate);
	}
}