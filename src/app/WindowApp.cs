using System;
using System.Collections.Generic;
using Rendering;
using Rendering.App;

namespace Rendering.App {

public static class WindowApp {

  public static void Run<State, Event>(
    Func<Window, State> init,
    Func<Window, Sub<Event>> subs,
    Func<State, Event, (State, Cmd<Event>)> step,
    Action<Window, State> view,
    int width,
    int height,
    string title
  ) {
    var prevSize = (0, 0);
    var nextSize = (width, height);
    using (var window = new Window(title, width, height))
    using (var store = new Store<State, Event>(
      init(window),
      subs(window),
      step,
      s => {
        view(window, s);
        window.SwapBuffers();
      }
    )) {
      window.OnResize += (w, h) => nextSize = (w, h);
      store.Start();
      while (!store.ShouldQuit && !window.ShouldClose) {
        if (nextSize != prevSize) {
          prevSize = nextSize;
          store.ForceRedraw();
        }
        store.Process();
        window.Poll();
      }
    }
  }

  public static Sub<E> Key<E>(Window w, Func<Key, bool, E> map) {
    var onKey = default(Action<Key, bool>);
    return new Sub<E>(
      dispatch => {
        onKey = (k, s) => dispatch(map(k, s));
        w.OnKey += onKey;
      },
      () => w.OnKey -= onKey
    );
  }

  public static Sub<E> Mouse<E>(Window w, Func<float, float, E> map) {
    var onMouse = default(Action<float, float>);
    return new Sub<E>(
      dispatch => {
        onMouse = (x, y) => dispatch(map(x, y));
        w.OnMouse += onMouse;
      },
      () => w.OnMouse -= onMouse
    );
  }

}

}

