using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using GLFW;

namespace Rendering {

public class InputManager {

  // Internal vars
  ///////////////////////////

  SparseSet<Key, bool> state;

  // Constructors
  ///////////////////////////

  public InputManager() {
    state = new SparseSet<Key, bool>(k => (int)k);
  }

  // Public methods
  ///////////////////////////

  public void Update() {
    for (var i = 0; i < state.Size(); i++) {
      ref var isHeld = ref state.GetAt(i);
      isHeld = true;
    }
  }

  public void OnKey(Key k, InputState s) {
    if (s == InputState.Press) {
      ref var isHeld = ref state.GetOrAdd(k);
      isHeld = false;
    } else {
      state.Remove(k);
    }
  }

  public bool IsPressed(Key k) {
    return state.Has(k);
  }

  public bool IsDown(Key k) {
    return state.Has(k) && !state.Get(k);
  }

}

}
