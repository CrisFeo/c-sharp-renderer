using System;
using System.Collections.Generic;

namespace Rendering.App {

public record Cmd<E>(Action<Action<E>> run);

public record QuitCmd<E>() : Cmd<E>(default(Action<Action<E>>));

public static partial class Cmd {

  public static Cmd<E> Quit<E>() {
    return new QuitCmd<E>();
  }

  public static Cmd<E> Many<E>(params Cmd<E>[] cmds) {
    return new Cmd<E>(dispatch => {
      foreach (var c in cmds) c.run(dispatch);
    });
  }

  public static Cmd<E> CurrentTime<E>(Func<float, E> map) {
    return new Cmd<E>(dispatch => dispatch(map(Time.Now())));
  }

}

}
