using System;
using System.Collections.Generic;
using System.Linq;
using Defsite.IO;
using Defsite.IO.DataFormats;

using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

namespace Defsite.Core;

public partial class Application {

	public GameWindow Window { get; private set; }

	public Scene Scene { get; private set; }

	public IEnumerable<MonitorInfo> MonitorList { get; private set; }

	static readonly Logger log = LogManager.GetCurrentClassLogger();

	public Application(ApplicationSettings settings, Scene scene) {
		MonitorList = GetMonitors();
		CreateWindow(settings);

		Scene = scene;
		Scene.Window = Window;

		LogInfo();
		InitializeWindow();
		//InitializeScene();
	}

	void InitializeWindow() {
		//Native window
		//Window.Move += (position_event) => { };

		//Window.Resize += (resize_event) => { };

		//Window.Minimized += (minimized_event) => { };

		//Window.Maximized += (maximized_event) => { };

		//Window.FocusedChanged += (focused_changed_event) => { };

		//Window.TextInput += (text_input_event) => { };

		//Window.MonitorConnected += (MonitorEventArgs monitor_event) => { };

		//Window.JoystickConnected += (JoystickEventArgs joystick_event) => { };

		//Window.MouseLeave += () => { };

		//Window.MouseEnter += () => { };

		//Window.FileDrop += (FileDropEventArgs file_drop_event) => { };

		//Game window
		Window.Load += () => {
			//TODO(@VoOoLoX): make this more flexible
			Assets.LoadAssets("Assets/Assets.json");

			Scene.Start();
		};

		Window.UpdateFrame += (frame_event) => {
			InputController.Instance.SetState(Window.KeyboardState);
			InputController.Instance.SetState(Window.MouseState);
			InputController.Instance.SetState(Window.JoystickStates);

			Scene.Update(frame_event);
		};

		Window.RenderFrame += (frame_event) => {
			GL.Viewport(0, 0, Window.ClientSize.X, Window.ClientSize.Y);
			GL.ClearColor(Scene.ClearColor);
			GL.ClearDepth(1000.0);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Scene.Render(frame_event);

			Window.SwapBuffers();
		};

		Window.Unload += () => { };

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
			Window = new GameWindow(game_window_settings, native_window_settings);
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
		//	log.Info($"Devices: {string.Join(", ", devices_list)}");

		//	var device_name = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);

		//	foreach(var d in devices_list.Where(d => d.Contains("OpenAL Soft"))) {
		//		device_name = d;
		//	}

		//	var device = ALC.OpenDevice(device_name);
		//	var audio_context = ALC.CreateContext(device, (int[])null);
		//	ALC.MakeContextCurrent(audio_context);
		//} catch {
		//	log.Error("Could not load 'openal32.dll'. Try installing OpenAL.");
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