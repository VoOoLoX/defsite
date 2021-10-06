using System;
using System.Runtime.InteropServices;

using NLog;

using OpenTK.Graphics.OpenGL4;

namespace Defsite.Utils;
public static class GLUtils {
	static GCHandle debug_proc_callback_handle;

	static readonly DebugProc debug_proc_callback = DebugCallback;

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr user_param) {
		var message_string = Marshal.PtrToStringAnsi(message, length);

		if(type == DebugType.DebugTypeError) {
			log.Error($"{message_string}");
		} else {
			log.Info($"{message_string}");
		}
	}

	public static void InitDebugCallback() {
		debug_proc_callback_handle = GCHandle.Alloc(debug_proc_callback);

		GL.DebugMessageCallback(debug_proc_callback, IntPtr.Zero);
		GL.Enable(EnableCap.DebugOutput);
		GL.Enable(EnableCap.DebugOutputSynchronous);
	}

	public static void Dispose() {
		if(debug_proc_callback_handle.IsAllocated) {
			debug_proc_callback_handle.Free();
		}
	}
}
