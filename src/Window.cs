using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using GLFW;

using static OpenGL.Gl;

namespace Rendering {

public class Window : IDisposable {

  // Internal vars
  ///////////////////////////

  GLFW.Window window;
  SizeCallback onResize;
  KeyCallback onKey;
  MouseButtonCallback onMouseKey;
  MouseCallback onMouse;

  // Public properites
  ///////////////////////////

  public bool ShouldClose { get; private set; }
  public (float, float) Size { get; private set; }

  // Public events
  ///////////////////////////

  public event Action<int, int> OnResize;
  public event Action<Key, bool> OnKey;
  public event Action<float, float> OnMouse;

  // Constructors
  ///////////////////////////

  static Window() {
    AppDomain.CurrentDomain.ProcessExit += (o, e) => {
      Glfw.Terminate();
    };
  }

  public Window (
    string title,
    int initialWidth,
    int initialHeight
  ) {
    // Set up GLFW window context
    {
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
      Glfw.MakeContextCurrent(window);
      Glfw.SwapInterval(1);
      Import(Glfw.GetProcAddress);
    }
    // Bind resize callback
    {
      onResize = (wnd, w, h) => {
        Size = (w, h);
        glViewport(0, 0, w, h);
        OnResize?.Invoke(w, h);
      };
      Glfw.SetWindowSizeCallback(window, onResize);
    }
    // Bind key callback
    {
      onKey = (wnd, k, sc, s, m) =>{
        if (s != InputState.Press && s != InputState.Release) return;
        OnKey?.Invoke(
          (Key)k,
          s == InputState.Press
        );
      };
      Glfw.SetKeyCallback(window, onKey);
    }
    // Bind mouse key callback
    {
      onMouseKey = (wnd, b, s, m) => {
        var key = default(Key);
        switch (b) {
          case MouseButton.Left:   key = Key.MouseLeft;   break;
          case MouseButton.Right:  key = Key.MouseRight;  break;
          case MouseButton.Middle: key = Key.MouseMiddle; break;
        }
        OnKey?.Invoke(key, s == InputState.Press);
      };
      Glfw.SetMouseButtonCallback(window, onMouseKey);
    }
    // Bind mouse callback
    {
      onMouse = (wnd, x, y) => {
        var (_, screenHeight) = Size;
        OnMouse?.Invoke((float)x, (float)(screenHeight - y));
      };
      Glfw.SetCursorPositionCallback(window, onMouse);
    }
    // Setup initial GL state
    {
      Size = (initialWidth, initialHeight);
      glViewport(0, 0, initialWidth, initialHeight);
      glClearColor(0, 0, 0, 1);
      glClear(GL_COLOR_BUFFER_BIT);
      SwapBuffers();
    }
  }

  // Public methods
  ///////////////////////////

  public void Dispose() {
    window = default;
  }

  public void Poll() {
    Glfw.PollEvents();
    ShouldClose = Glfw.WindowShouldClose(window);
  }

  public void SwapBuffers() {
    Glfw.SwapBuffers(window);
    glClear(GL_COLOR_BUFFER_BIT);
  }

}

}
