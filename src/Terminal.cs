using System;
using System.Collections.Generic;
using System.IO;

namespace Rendering {

public class Terminal : IDisposable {

  // Constants
  ///////////////////////////

  const int FONT_CHARACTER_SIZE = 12;
  const string FONT_PATH = "cp437_12x12.png";
  const string VERTEX_SHADER_PATH = "terminal.vert";
  const string FRAGMENT_SHADER_PATH = "terminal.frag";
  const string PROJECTION_UNIFORM_NAME = "projection";

  static readonly string DATA_PATH = Path.Combine(
    Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
    "dat"
  );

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

  GL.Texture font;
  SparseSet<int, Cell> buffer;
  SpriteBatcher batcher;
  GL.Program program;
  GL.Uniform projection;

  // Public properties
  ///////////////////////////

  public bool ShouldClose { get => Window.ShouldClose; }
  public (int, int) Size { get; private set; }

  // Constructors
  ///////////////////////////

  public Terminal(int cellWidth, int cellHeight, string title) {
    var pixelWidth = FONT_CHARACTER_SIZE * cellWidth;
    var pixelHeight = FONT_CHARACTER_SIZE * cellHeight;
    Window.Startup(title, pixelWidth, pixelHeight, OnResize);
    font = GL.CreateTexture(Path.Combine(DATA_PATH, FONT_PATH));
    buffer = new SparseSet<int, Cell>(i => i);
    batcher = SpriteBatchers.New();
    program = GL.CreateProgram(
      Path.Combine(DATA_PATH, VERTEX_SHADER_PATH),
      Path.Combine(DATA_PATH, FRAGMENT_SHADER_PATH)
    );
    projection = GL.CreateUniform(program, PROJECTION_UNIFORM_NAME);
    OnResize(pixelWidth, pixelHeight);
  }

  // Public methods
  ///////////////////////////

  public void Dispose() {
    Window.Shutdown();
  }

  public void Poll() {
    Window.Poll();
  }

  public bool KeyPress(Key k) {
    return Window.Input.Has(k);
  }

  public bool KeyDown(Key k) {
    return Window.Input.Has(k) && !Window.Input.Get(k);
  }

  public void Clear() {
    buffer.Clear();
  }

  public void Render() {
    batcher.Begin();
    for (var i = 0; i < buffer.Size(); i++) {
      var cell = buffer.GetAt(i);
      batcher.Draw(NewGlyph(
        font,
        cell.x,
        cell.y,
        cell.c,
        cell.fg.color,
        cell.bg.color
      ));
    }
    batcher.End();
    batcher.Render();
    Window.SwapBuffers();
  }

  public void Set(
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

  public void Set(
    int x,
    int y,
    char character,
    Color? foreground = null,
    Color? background = null
  ) {
    var (_, h) = Size;
    var index = CellIndex(x, y);
    ref var cell = ref buffer.GetOrAdd(index);
    cell.x = x;
    cell.y = h - 1 - y;
    cell.c = character;
    cell.fg = foreground ?? Colors.White;
    cell.bg = background ?? Colors.Black;
  }

  // Internal methods
  ///////////////////////////

  void OnResize(int w, int h) {
    GL.Use(program);
    GL.Set(projection, GL.Ortho(0, w, 0, h, -1, 1));
    buffer.Clear();
    Size = (w / FONT_CHARACTER_SIZE, h / FONT_CHARACTER_SIZE);
  }

  int CellIndex(int x, int y) {
    var (w, _) = Size;
    return w * y + x;
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
