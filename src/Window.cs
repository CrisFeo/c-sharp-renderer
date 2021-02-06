using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using GLFW;

using static OpenGL.Gl;

namespace Rendering {

static class Window {

  // Internal vars
  ///////////////////////////

  static bool isRunning;
  static GLFW.Window window;
  static Action<int, int> onResize;
  static Action<float, float> onMousePosition;
  static Action<Key> onKeyDown;

  // Public properites
  ///////////////////////////

  public static bool ShouldClose { get; private set; }
  public static SparseSet<Key, bool> Input { get; private set; }

  // Public methods
  ///////////////////////////

  public static void Startup(
    string title,
    int initialWidth,
    int initialHeight,
    Action<int, int> resizeCallback,
    Action<float, float> mousePositionCallback,
    Action<Key> keyDownCallback
  ) {
    Debug.Assert(!isRunning);
    isRunning = true;
    Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
    Glfw.WindowHint(Hint.ContextVersionMajor, 3);
    Glfw.WindowHint(Hint.ContextVersionMinor, 3);
    Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
    Glfw.WindowHint(Hint.Doublebuffer, true);
    Glfw.WindowHint(Hint.Decorated, true);
    window = Glfw.CreateWindow(
      initialWidth,
      initialHeight,
      title,
      Monitor.None,
      GLFW.Window.None
    );
    onResize = resizeCallback;
    Input = new SparseSet<Key, bool>(k => (int)k);
    ShouldClose = false;
    Glfw.MakeContextCurrent(window);
    Glfw.SwapInterval(1);
    Import(Glfw.GetProcAddress);
    onMousePosition = mousePositionCallback;
    Glfw.SetCursorPositionCallback(
      window,
      (wnd, x, y) => onMousePosition?.Invoke((float)x, (float)y)
    );
    Glfw.SetMouseButtonCallback(window, (wnd, b, s, m) => {
      var key = default(Key);
      switch (b) {
        case MouseButton.Left: key = Key.MouseLeft; break;
        case MouseButton.Right: key = Key.MouseRight; break;
        case MouseButton.Middle: key = Key.MouseMiddle; break;
      }
      OnKey(key, s);
    });
    onKeyDown = keyDownCallback;
    Glfw.SetKeyCallback(window, (wnd, k, sc, s, m) => {
      var key = (Key)k;
      OnKey(key, s);
    });
    Glfw.SetWindowSizeCallback(window, (wnd, w, h) => {
      glViewport(0, 0, w, h);
      onResize?.Invoke(w, h);
    });
    glViewport(0, 0, initialWidth, initialHeight);
    glClearColor(0, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);
    SwapBuffers();
  }

  public static void Shutdown() {
    Debug.Assert(isRunning);
    isRunning = false;
    Glfw.Terminate();
    window = default;
    onResize = default;
    Input = default;
    ShouldClose = default;
  }

  public static void Poll() {
    Debug.Assert(isRunning);
    for (var i = 0; i < Input.Size(); i++) {
      ref var isHeld = ref Input.GetAt(i);
      isHeld = true;
    }
    Glfw.PollEvents();
    ShouldClose = Glfw.WindowShouldClose(window);
  }

  public static void SwapBuffers() {
    Debug.Assert(isRunning);
    Glfw.SwapBuffers(window);
    glClear(GL_COLOR_BUFFER_BIT);
  }

  // Internal vars
  ////////////////////

  static void OnKey(Key k, InputState s) {
    if (s == InputState.Press) {
      ref var isHeld = ref Input.GetOrAdd(k);
      isHeld = false;
      onKeyDown?.Invoke(k);
    } else {
      Input.Remove(k);
    }
  }

}

}
