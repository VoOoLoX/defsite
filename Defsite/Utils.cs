using System.Runtime.InteropServices;

namespace Defsite {
	public static class Utils {
#if DEBUG
		public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

		public static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public static bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		public static bool IsOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	}
}