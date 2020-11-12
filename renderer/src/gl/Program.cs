using System;
using System.IO;
using System.Runtime.InteropServices;

using static OpenGL.Gl;

namespace Renderer {

public static unsafe partial class GL {

  // Structs
  ///////////////////////////

  public struct Program {
    public uint id;
    public static bool operator==(Program a, Program b) => a.id == b.id;
    public static bool operator!=(Program a, Program b) => !(a == b);
    public override bool Equals(object o) => (o is Program p) ? this == p : false;
    public override int GetHashCode() => id.GetHashCode();
  }

  // Public methods
  ///////////////////////////

  public static Program CreateProgram(string vertexPath, string fragmentPath) {
    var vertex = CreateShader(
      GL_VERTEX_SHADER,
      File.ReadAllText(vertexPath)
    );
    var fragment = CreateShader(
      GL_FRAGMENT_SHADER,
      File.ReadAllText(fragmentPath)
    );
    var program = glCreateProgram();
    glAttachShader(program, vertex);
    glAttachShader(program, fragment);
    glLinkProgram(program);
    var success = GL_FALSE;
    glGetProgramiv(program, GL_LINK_STATUS, &success);
    if (success != GL_TRUE) {
      var log = glGetProgramInfoLog(program);
      throw new System.Exception("failed to link shader program:\n" + log);
    }
    glDeleteShader(vertex);
    glDeleteShader(fragment);
    return new Program{ id = program };
  }

  public static void Use(Program program) {
    glUseProgram(program.id);
  }

  // Internal methods
  ///////////////////////////

  static uint CreateShader(int type, string source) {
    var shader = glCreateShader(type);
    glShaderSource(shader, source);
    glCompileShader(shader);
    var success = GL_FALSE;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
    if (success != GL_TRUE) {
      var log = glGetShaderInfoLog(shader);
      throw new System.Exception("failed to compile shader\n" + log);
    }
    return shader;
  }

}

}
