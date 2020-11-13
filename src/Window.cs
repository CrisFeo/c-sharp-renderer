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

  // Public properites
  ///////////////////////////

  public static SparseSet<Key, bool> Input { get; private set; }
  public static bool ShouldClose { get; private set; }

  // Public methods
  ///////////////////////////

  public static void Startup(
    string title,
    int initialWidth,
    int initialHeight,
    Action<int, int> resizeCallback
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
    Glfw.SetKeyCallback(window, (wnd, k, sc, s, m) => {
      var key = (Key)k;
      if (s == InputState.Press) {
        ref var isHeld = ref Input.GetOrAdd(key);
        isHeld = false;
      } else {
        Input.Remove(key);
      }
    });
    Glfw.SetWindowSizeCallback(window, (wnd, w, h) => {
      glViewport(0, 0, w, h);
      onResize(w, h);
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

}

}
