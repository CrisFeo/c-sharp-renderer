using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using GLFW;

using static OpenGL.Gl;

namespace Renderer {

public static class Window {

  // Internal vars
  ///////////////////////////

  static GLFW.Window window;
  static HashSet<Key> input;
  static Vec2 size;
  static Action<int, int> onResize;

  // Public methods
  ///////////////////////////

  public static void Startup(
    string title,
    int initialWidth,
    int initialHeight,
    Action<int, int> resizeCallback
  ) {
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
    input = new HashSet<Key>();
    size = new Vec2(initialWidth, initialHeight);
    onResize = resizeCallback;
    Glfw.MakeContextCurrent(window);
    Glfw.SwapInterval(1);
    Import(Glfw.GetProcAddress);
    Glfw.SetKeyCallback(window, (wnd, k, sc, s, m) => {
      switch (s) {
        case InputState.Press:   input.Add((Key)k);    break;
        case InputState.Release: input.Remove((Key)k); break;
      }
    });
    Glfw.SetWindowSizeCallback(window, (wnd, w, h) => {
      size = new Vec2(w, h);
      glViewport(0, 0, w, h);
      onResize(w, h);
    });
    glViewport(0, 0, initialWidth, initialHeight);
    glClearColor(0, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);
    SwapBuffers();
  }

  public static void Shutdown() {
    Glfw.Terminate();
    window = default;
    input = default;
    size = default;
    onResize = default;
  }

  public static void Poll() {
    Glfw.PollEvents();
  }

  public static bool ShouldClose() {
    return Glfw.WindowShouldClose(window);
  }

  public static HashSet<Key> FetchInput() {
    return input;
  }

  public static Vec2 Size() {
    return size;
  }

  public static void SwapBuffers() {
    Glfw.SwapBuffers(window);
    glClear(GL_COLOR_BUFFER_BIT);
  }

}

}
