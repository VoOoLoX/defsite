using System.IO;
using Defsite;
using OpenTK.Audio.OpenAL;

namespace Client {
	public class SoundFile {
		public static SoundFile Default = new SoundFile(ALFormat.Mono8, 0, new byte[] { });

		public SoundFile(ALFormat format, int sample_rate, byte[] data) {
			Format = format;
			SampleRate = sample_rate;
			Data = data;
		}

		public SoundFile(string path) => Load(path);
		
		public SoundFile(Stream stream) => Load(stream);
		public ALFormat Format { get; private set; }
		public int SampleRate { get; private set; }
		public byte Compressed { get; private set; }
		public byte[] Data { get; private set; }

		void Load(string file_path) {
			var file_info = File.Exists(file_path) ? new FileInfo(file_path) : null;
			if (file_info == null)
				Log.Panic($"Invalid sound file path: {file_path}");
			Load(file_info?.OpenRead());
		}

		void Load(Stream data_stream) {
			var reader = new BinaryReader(data_stream);

			var vsf = new string(reader.ReadChars(3));

			if (vsf.ToUpper() != "VSF") Log.Error("Invalid VSF header");

			var channels = reader.ReadByte();
			var sample_width = reader.ReadByte();

			SampleRate = reader.ReadUInt16();

			Compressed = reader.ReadByte();

			var data_bytes = reader.ReadBytes((int) reader.BaseStream.Length - (int) reader.BaseStream.Position);

			Data = Compressed > 0 ? data_bytes.Decompress() : data_bytes;

			switch (channels) {
				case 1:
					Format = sample_width == 1 ? ALFormat.Mono8 : ALFormat.Mono16;
					break;
				case 2:
					Format = sample_width == 1 ? ALFormat.Stereo8 : ALFormat.Stereo16;
					break;
				default:
					Log.Error("The specified sound format is not supported.");
					break;
			}
		}
	}
}