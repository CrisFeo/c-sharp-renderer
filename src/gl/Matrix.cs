using System;
using System.IO;
using System.Runtime.InteropServices;

using static OpenGL.Gl;

namespace Renderer {

public static unsafe partial class GL {

  // Public methods
  ///////////////////////////

  public static float[] Ortho(float l, float r, float b, float t, float n, float f) {
    return new float[]{
      2.0f/(r-l),   0,            0,            0,
      0,            2.0f/(t-b),   0,            0,
      0,            0,            -2.0f/(f-n),  0,
      -(r+l)/(r-l), -(t+b)/(t-b), -(f+n)/(f-n), 1
    };
  }

}

}
