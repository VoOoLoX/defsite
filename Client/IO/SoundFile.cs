using System.IO;
using Defsite;
using OpenTK.Audio.OpenAL;

namespace Client {
	public class WAVFile {
		short AudioFormat;
		short BitsPerSample;
		short BlockAlign;
		int ByteRate;
		short Channels;

		string DATA;
		int DataChunkSize;
		int FileLength;

		string FMT;
		int FormatChunkSize;
		string RIFF;

		string WAVE;

		public WAVFile(string path) {
			ParseWAV(new FileInfo(path));
		}

		public byte[] Data { get; protected set; }
		public int SampleRate { get; protected set; }
		public ALFormat Format { get; protected set; }

		void ParseWAV(FileInfo file) {
			if (!file.Exists)
				Log.Panic($"Can't find {file.Name}");

			var bin = new BinaryReader(file.OpenRead());

			RIFF = new string(bin.ReadChars(4));
			if (RIFF != "RIFF")
				Log.Panic("Invalid file header: RIFF");

			FileLength = bin.ReadInt32();

			WAVE = new string(bin.ReadChars(4));
			if (WAVE != "WAVE")
				Log.Panic("Invalid file header: WAVE");

			FMT = new string(bin.ReadChars(4));
			if (FMT != "fmt ")
				Log.Panic("Invalid file header: fmt");

			FormatChunkSize = bin.ReadInt32();
			AudioFormat = bin.ReadInt16();
			Channels = bin.ReadInt16();
			SampleRate = bin.ReadInt32();
			ByteRate = bin.ReadInt32();
			BlockAlign = bin.ReadInt16();
			BitsPerSample = bin.ReadInt16();

			DATA = new string(bin.ReadChars(4));
			if (DATA != "data")
				Log.Panic("Invalid file header: data");

			DataChunkSize = bin.ReadInt32();

			Data = bin.ReadBytes(DataChunkSize);

			switch (Channels) {
				case 1:
					if (BitsPerSample == 8)
						Format = ALFormat.Mono8;
					else
						Format = ALFormat.Mono16;
					break;
				case 2:
					if (BitsPerSample == 8)
						Format = ALFormat.Stereo8;
					else
						Format = ALFormat.Stereo16;
					break;
				default:
					Log.Panic("The specified sound format is not supported.");
					break;
			}
		}
	}

	// public class SoundFile {
	// 	public SoundFile(string path) {

	// 	}
	// }
}