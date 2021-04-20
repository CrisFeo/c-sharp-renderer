using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

using static OpenGL.Gl;

namespace Rendering {

public static unsafe partial class GL {

  // Constants
  ///////////////////////////

  static readonly FieldInfo VERTEX_LIST_ITEMS_FIELD = typeof(List<Vertex>).GetField(
    "_items",
    BindingFlags.NonPublic | BindingFlags.Instance
  );

  // Structs
  ///////////////////////////

  [StructLayout(LayoutKind.Sequential)]
  public struct Vec2 {
    public float x;
    public float y;
    public static bool operator==(Vec2 a, Vec2 b) => a.x == b.x && a.y == b.y;
    public static bool operator!=(Vec2 a, Vec2 b) => !(a == b);
    public override bool Equals(object o) => (o is Vec2 v) ? this == v : false;
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct Color {
    public float r;
    public float g;
    public float b;
    public static bool operator==(Color a, Color b) => a.r == b.r && a.g == b.g && a.b == b.b;
    public static bool operator!=(Color a, Color b) => !(a == b);
    public override bool Equals(object o) => (o is Color c) ? this == c : false;
    public override int GetHashCode() => r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct Vertex {
    public Vec2 pos;
    public Vec2 uv;
    public Color fg;
    public Color bg;
  }

  public struct VertexArray {
    public uint vao;
    public uint vbo;
    public int count;
  }


  // Public methods
  ///////////////////////////

  public static GL.Vertex V(
    float x,
    float y,
    float u,
    float v,
    Color fg,
    Color bg
  ) {
    return new Vertex {
      pos = new Vec2{ x = x, y = y },
      uv =  new Vec2{ x = u, y = v },
      fg =  fg,
      bg =  bg,
    };
  }

  public static VertexArray CreateVertexArray() {
    var d = new VertexArray();
    glGenVertexArrays(1, &d.vao);
    glBindVertexArray(d.vao);
    glGenBuffers(1, &d.vbo);
    glBindBuffer(GL_ARRAY_BUFFER, d.vbo);
    glEnableVertexAttribArray(0);
    glEnableVertexAttribArray(1);
    glEnableVertexAttribArray(2);
    glEnableVertexAttribArray(3);
    glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("pos"));
    glVertexAttribPointer(1, 2, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("uv"));
    glVertexAttribPointer(2, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("fg"));
    glVertexAttribPointer(3, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("bg"));
    glBindVertexArray(0);
    return d;
  }

  public static void Buffer(ref VertexArray vertexArray, Vertex[] vertices, int count) {
    glBindBuffer(GL_ARRAY_BUFFER, vertexArray.vbo);
    glBufferData(GL_ARRAY_BUFFER, count * sizeof(Vertex), IntPtr.Zero, GL_DYNAMIC_DRAW);
    fixed (Vertex* v = vertices) {
      glBufferSubData(GL_ARRAY_BUFFER, 0, count * sizeof(Vertex), (IntPtr)v);
    }
    vertexArray.count = count;
  }

  public static void Bind(VertexArray vertexArray) {
    glBindVertexArray(vertexArray.vao);
  }

  public static void DrawArrays(int offset, int count) {
    glDrawArrays(GL_TRIANGLES, offset, count);
  }

}

}
