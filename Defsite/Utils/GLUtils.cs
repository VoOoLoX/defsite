using System;
using System.Runtime.InteropServices;

using Common;

using OpenTK.Graphics.OpenGL;

namespace Defsite.Utils;
public static class GLUtils {
	static readonly DebugProc debug_proc_callback = DebugCallback;
	static GCHandle debug_proc_callback_handle;

	static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr user_param) {
		var message_string = Marshal.PtrToStringAnsi(message, length);

		if(type == DebugType.DebugTypeError) {
			Log.Error($"{severity} {type} | {message_string}");
		} else {
			Log.Info($"{severity} {type} | {message_string}");
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
