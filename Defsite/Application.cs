using System;
using System.Collections.Generic;
using System.Linq;

using Defsite.Core;
using Defsite.IO;
using Defsite.IO.DataFormats;

using NLog;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

namespace Defsite;

public interface IWindowProperties {
	public int X { get; }
	public int Y { get; }
	public int Width { get; }
	public int Height { get; }
	public bool IsFocused { get; }
	public bool IsMinimized { get; }
	public bool IsMaximized { get; }
	public int ClientX { get; }
	public int ClientY { get; }
	public int ClientWidth { get; }
	public int ClientHeight { get; }
}

public class Application {

	class WindowProperties : IWindowProperties {
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsFocused { get; set; }
		public bool IsMinimized { get; set; }
		public bool IsMaximized { get; set; }
		public int ClientX { get; set; }
		public int ClientY { get; set; }
		public int ClientWidth { get; set; }
		public int ClientHeight { get; set; }
	}


	public GameWindow Window { get; private set; }

	public IWindowProperties Properties => window_properties;

	public Scene Scene { get; private set; }

	public IEnumerable<MonitorInfo> MonitorList { get; private set; }

	readonly InputController input_controller = new();

	readonly WindowProperties window_properties = new();

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public Application(ApplicationSettings settings, Scene scene) {
		Scene = scene;
		Scene.WindowProperties = new WindowProperties();

		MonitorList = GetMonitors();
		CreateWindow(settings);
		LogInfo();
		//InitializeWindow();
		//InitializeScene();
	}

	void InitializeWindow() {
		//Native window
		Window.Move += (WindowPositionEventArgs position_event) => {
			window_properties.X = position_event.X;
			window_properties.Y = position_event.Y;
			window_properties.ClientX = Window.ClientRectangle.Min.X;
			window_properties.ClientY = Window.ClientRectangle.Min.Y;
		};

		Window.Resize += (ResizeEventArgs resize_event) => {
			window_properties.Width = resize_event.Width;
			window_properties.Height = resize_event.Height;
			window_properties.ClientWidth = Window.ClientSize.X;
			window_properties.ClientHeight = Window.ClientSize.Y;
		};

		Window.Minimized += (MinimizedEventArgs minimized_event) => window_properties.IsMinimized = minimized_event.IsMinimized;
		Window.Maximized += (MaximizedEventArgs maximized_event) => window_properties.IsMaximized = maximized_event.IsMaximized;
		Window.FocusedChanged += (FocusedChangedEventArgs focused_changed_event) => window_properties.IsFocused = focused_changed_event.IsFocused;

		Window.TextInput += (TextInputEventArgs text_input_event) => {
		};

		//Window.MonitorConnected += (MonitorEventArgs monitor_event) => {
		//};
		//Window.JoystickConnected += (JoystickEventArgs joystick_event) => {
		//};

		//Window.MouseLeave += () => {
		//};
		//Window.MouseEnter += () => {
		//};

		//Window.FileDrop += (FileDropEventArgs file_drop_event) => {
		//};

		//Game window
		Window.Load += () => {
			Assets.LoadAssets("Assets/Assets.json");

			Scene.WindowProperties = window_properties;

			Scene.Start();
		};

		Window.UpdateFrame += (FrameEventArgs frame_event) => {
			input_controller.SetState(Window.KeyboardState);
			input_controller.SetState(Window.MouseState);
			input_controller.SetState(Window.JoystickStates);

			Scene.WindowProperties = window_properties;

			Scene.Update(frame_event);
		};

		Window.RenderFrame += (FrameEventArgs frame_event) => {
			GL.Viewport(0, 0, Window.ClientSize.X, Window.ClientSize.Y);
			GL.ClearColor(.1f, .1f, .1f, 1);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			Scene.Render(frame_event);

			Window.SwapBuffers();
		};

		Window.Unload += () => {

		};

		Window.Closed += () => { };
		//Window.RenderThreadStarted += () => {}; //Used if IsMultiThreaded == true
	}

	IEnumerable<MonitorInfo> GetMonitors() {
		var monitors = new List<MonitorInfo>();

		for(var monitor_index = 0; monitor_index < Monitors.Count; monitor_index++) {
			Monitors.TryGetMonitorInfo(monitor_index, out var info);
			monitors.Add(info);
		}

		return monitors;
	}

	void CreateWindow(ApplicationSettings settings) {
		var monitor_center = MonitorList.FirstOrDefault().ClientArea.Center;

		var game_window_settings = new GameWindowSettings() {
			IsMultiThreaded = settings.IsMultiThreaded,
			UpdateFrequency = settings.UpdateFrequency,
			RenderFrequency = settings.RenderFrequency
		};

		var icon_texture = new TextureData(settings.Icon);
		var icon = new WindowIcon(new Image(icon_texture.Width, icon_texture.Height, icon_texture.Bytes));

		var native_window_settings = new NativeWindowSettings() {
			Icon = icon,
			Title = settings.Title,
			Size = settings.Size,
			Location = settings.Position == (-1, -1) ? ((int)monitor_center.X - settings.Size.Width / 2, (int)monitor_center.Y - settings.Size.Height / 2) : settings.Position,

			StartVisible = settings.StartVisible,
			StartFocused = settings.StartFocused,
			WindowBorder = settings.WindowBorder,
			WindowState = settings.WindowState,

			API = ContextAPI.OpenGL,
			APIVersion = new Version(settings.GLVersion),
			Flags = settings.Flags,
			NumberOfSamples = settings.NumberOfSamples,
			AutoLoadBindings = true,
			CurrentMonitor = MonitorList.FirstOrDefault().Handle
		};

		try {
			//Window = new GameWindow(game_window_settings, native_window_settings);
			Window = new Playground(game_window_settings, native_window_settings);
		} catch(Exception e) {
			log.Error(e);
			log.Fatal($"Couldn't create a game window.");
		}
	}

	void LogInfo() {
		log.Info("Environment:");

		log.Info($"OS: {Environment.OSVersion}");
		log.Info($"Runtime: {Environment.Version}");
		log.Info($"CWD: {Environment.CurrentDirectory}");

		log.Info("Monitors:");

		for(var index = 0; index < MonitorList.Count(); index++) {
			var monitor = MonitorList.ElementAtOrDefault(index);
			log.Info($"Monitor {index}: {monitor.ClientArea} DPI ({monitor.HorizontalDpi}, {monitor.VerticalDpi})");
		}

		if(Window.Context is null) {
			log.Fatal("Invalid graphics context");
		}

		log.Info("OpenGL:");

		log.Info($"Vendor: {GL.GetString(StringName.Vendor)}");
		log.Info($"Renderer: {GL.GetString(StringName.Renderer)}");
		log.Info($"Context: {GL.GetString(StringName.Version)}");
		log.Info($"GLSL: {GL.GetString(StringName.ShadingLanguageVersion)}");
		//log.Info(string.Join('\n', GL.GetString(StringName.Extensions).Split(' ')));

		//try {
		//	var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
		//	var devices_list = devices.ToList();
		//	Log.Info($"Devices: {string.Join(", ", devices_list)}");

		//	var device_name = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);

		//	foreach(var d in devices_list.Where(d => d.Contains("OpenAL Soft"))) {
		//		device_name = d;
		//	}

		//	var device = ALC.OpenDevice(device_name);
		//	audio_context = ALC.CreateContext(device, (int[])null);
		//	ALC.MakeContextCurrent(audio_context);
		//} catch {
		//	Log.Panic("Could not load 'openal32.dll'. Try installing OpenAL.");
		//}

		//Log.Info("OpenAL Info:");
		//Log.Indent();
		//Log.Info(AL.Get(ALGetString.Vendor));
		//Log.Info(AL.Get(ALGetString.Renderer));
		//Log.Info(AL.Get(ALGetString.Version));
		//Log.Unindent();
	}

	public void Run() => Window?.Run();
}