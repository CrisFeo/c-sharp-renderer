using System;
using System.Collections.Generic;

namespace Renderer {

public static class Terminal {

  // Constants
  ///////////////////////////

  const int FONT_CHARACTER_SIZE = 12;
  const string FONT_PATH = "dat/cp437_12x12.png";
  const string VERTEX_SHADER_PATH = "dat/main.vert";
  const string FRAGMENT_SHADER_PATH = "dat/main.frag";
  const string PROJECTION_UNIFORM_NAME = "projection";

  // Internal structs
  ///////////////////////////

  struct Cell {
    public int x;
    public int y;
    public char c;
    public Color fg;
    public Color bg;
    public static bool operator==(Cell a, Cell b) => a.x == b.x && a.y == b.y && a.c == b.c && a.fg == b.fg && a.bg == b.bg;
    public static bool operator!=(Cell a, Cell b) => !(a == b);
    public override bool Equals(object o) => (o is Cell c) ? this == c : false;
    public override int GetHashCode() => x ^ y ^ c ^ fg.GetHashCode() ^ bg.GetHashCode();
  }

  // Internal vars
  ///////////////////////////

  static GL.Texture font;
  static SparseSet<Cell> buffer;
  static SpriteBatcher batcher;
  static GL.Program program;
  static GL.Uniform projection;

  // Public methods
  ///////////////////////////

  public static void Startup(int cellWidth, int cellHeight, string title) {
    var pixelWidth = FONT_CHARACTER_SIZE * cellWidth;
    var pixelHeight = FONT_CHARACTER_SIZE * cellHeight;
    Window.Startup(title, pixelWidth, pixelHeight, OnResize);
    font = GL.CreateTexture(FONT_PATH);
    buffer = SparseSets.New<Cell>();
    batcher = SpriteBatchers.New();
    program = GL.CreateProgram(VERTEX_SHADER_PATH, FRAGMENT_SHADER_PATH);
    projection = GL.CreateUniform(program, PROJECTION_UNIFORM_NAME);
    OnResize(pixelWidth, pixelHeight);
  }

  public static void Shutdown() {
    Window.Shutdown();
    font = default;
    buffer = default;
    batcher = default;
    program = default;
    projection = default;
  }

  public static void Poll() {
    Window.Poll();
  }

  public static bool ShouldClose() {
    return Window.ShouldClose();
  }

  public static HashSet<Key> FetchInput() {
    return Window.FetchInput();
  }

  public static Vec2 Size() {
    return Window.Size() / FONT_CHARACTER_SIZE;
  }

  public static void Clear() {
    buffer.Clear();
  }

  public static void Render() {
    batcher.Begin();
    foreach (var c in buffer.data) {
      batcher.Draw(NewGlyph(
        font,
        c.x,
        c.y,
        c.c,
        c.fg.color,
        c.bg.color
      ));
    }
    batcher.End();
    batcher.Render();
    Window.SwapBuffers();
  }

  public static void Set(
    Vec2 position,
    char character,
    Color? foreground = null,
    Color? background = null
  ) {
    Set(position.x, position.y, character, foreground, background);
  }

  public static void Set(
    Vec2 position,
    string str,
    Color? foreground = null,
    Color? background = null
  ) {
    Set(position.x, position.y, str, foreground, background);
  }

  public static void Set(
    int x,
    int y,
    string str,
    Color? foreground = null,
    Color? background = null
  ) {
    for (var i = 0; i < str.Length; i++) {
      Set(x + i, y, str[i], foreground, background);
    }
  }

  public static void Set(
    int x,
    int y,
    char character,
    Color? foreground = null,
    Color? background = null
  ) {
    buffer.Set(CellIndex(x, y), new Cell {
      x = x,
      y = Size().y - 1 - y,
      c = character,
      fg = foreground ?? Colors.White,
      bg = background ?? Colors.Black,
    });
  }

  // Internal methods
  ///////////////////////////

  static void OnResize(int w, int h) {
    GL.Use(program);
    GL.Set(projection, GL.Ortho(0, w, 0, h, -1, 1));
    buffer.Clear();
  }

  static int CellIndex(int x, int y) {
    return Size().w * y + x;
  }

  static Sprite NewGlyph(
    GL.Texture texture,
    float column,
    float row,
    char character,
    GL.Color fg,
    GL.Color bg
  ) {
    var pxDim = texture.width;
    var chDim = 16;
    var pxSize = pxDim / chDim;
    var x = column * pxSize;
    var y = row * pxSize;
    var chIdx = (int)character;
    var chRow = chIdx / chDim;
    var chCol = chIdx - (chRow * chDim);
    var uvSize = 1f / chDim;
    var uvRow = 1 - (uvSize * (chRow + 1));
    var uvCol = uvSize * chCol;
    return new Sprite {
      texture =     texture,
      topLeft =     GL.V(x,          y + pxSize, uvCol,          uvRow + uvSize, fg, bg),
      topRight =    GL.V(x + pxSize, y + pxSize, uvCol + uvSize, uvRow + uvSize, fg, bg),
      bottomRight = GL.V(x + pxSize, y,          uvCol + uvSize, uvRow,          fg, bg),
      bottomLeft =  GL.V(x,          y,          uvCol,          uvRow,          fg, bg),
    };
  }

}

}
