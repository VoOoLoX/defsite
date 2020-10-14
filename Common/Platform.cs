using System.Runtime.InteropServices;

namespace Common {
	public static class Platform {
#if DEBUG
		public static readonly bool IsDebug = true;
#else
		public static readonly bool IsDebug = false;
#endif

		public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		public static readonly bool IsOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	}
}