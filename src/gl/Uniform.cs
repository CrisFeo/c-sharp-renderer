using System;
using System.IO;
using System.Runtime.InteropServices;

using static OpenGL.Gl;

namespace Renderer {

public static unsafe partial class GL {

  // Structs
  ///////////////////////////

  public struct Uniform {
    public int id;
    public static bool operator==(Uniform a, Uniform b) => a.id == b.id;
    public static bool operator!=(Uniform a, Uniform b) => !(a == b);
    public override bool Equals(object o) => (o is Uniform u) ? this == u : false;
    public override int GetHashCode() => id.GetHashCode();
  }

  // Public methods
  ///////////////////////////

  public static Uniform CreateUniform(Program program, string name) {
    var id = glGetUniformLocation(program.id, name);
    return new Uniform{ id = id };
  }

  public static void Set(Uniform uniform, float[] value) {
    fixed (float* v = value) {
      glUniformMatrix4fv(uniform.id, 1, false, v);
    }
  }

  public static void Set(Uniform uniform, float value) {
    glUniform1f(uniform.id, value);
  }

}

}
