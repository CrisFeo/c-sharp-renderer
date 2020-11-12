using System;
using System.IO;
using System.Runtime.InteropServices;
using Hjg.Pngcs;

using static OpenGL.Gl;

namespace Rendering {

public static unsafe partial class GL {

  // Structs
  ///////////////////////////

  public struct Texture {
    public uint id;
    public int width;
    public int height;
    public static bool operator==(Texture a, Texture b) => a.id == b.id;
    public static bool operator!=(Texture a, Texture b) => !(a == b);
    public override bool Equals(object o) => (o is Texture t) ? this == t : false;
    public override int GetHashCode() => id.GetHashCode();
  }

  // Public methods
  ///////////////////////////

  public static Texture CreateTexture(string path) {
    var texture = default(uint);
    glGenTextures(1, &texture);
    glBindTexture(GL_TEXTURE_2D, texture);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    var (pixels, w, h) = LoadPng(path);
    fixed (byte* p = pixels) {
      glTexImage2D(
        GL_TEXTURE_2D,
        0,
        GL_RGBA,
        w,
        h,
        0,
        GL_RGBA,
        GL_UNSIGNED_BYTE,
        (IntPtr)p
      );
      glBindTexture(GL_TEXTURE_2D, 0);
    }
    return new Texture{
      id = texture,
      width = w,
      height = h,
    };
  }

  public static void Bind(Texture texture) {
    glBindTexture(GL_TEXTURE_2D, texture.id);
  }

  // Internal methods
  ///////////////////////////

  static (byte[], int, int) LoadPng(string path) {
    var png = new PngReader(File.OpenRead(path));
    if (png.ImgInfo.Channels < 4 || png.ImgInfo.BitDepth != 8) {
      throw new Exception($"Unable to load '{path}' must be 8-bit (was {png.ImgInfo.BitDepth}) with at least 3 channels (was {png.ImgInfo.Channels})");
    }
    var colSize = png.ImgInfo.Channels;
    var rowSize = colSize * png.ImgInfo.Cols;
    var bytes = new byte[png.ImgInfo.Rows * rowSize];
    for (var r = 0; r < png.ImgInfo.Rows; r++) {
      var row = png.ReadRow(r).GetScanlineInt();
      for (var c = 0; c < png.ImgInfo.Cols; c++) {
        var rowOffset = (png.ImgInfo.Rows - 1 - r) * rowSize;
        var colOffset = c * colSize;
        bytes[rowOffset + colOffset + 0] = (byte)row[colOffset + 0];
        bytes[rowOffset + colOffset + 1] = (byte)row[colOffset + 1];
        bytes[rowOffset + colOffset + 2] = (byte)row[colOffset + 2];
        bytes[rowOffset + colOffset + 3] = (byte)row[colOffset + 3];
      }
    }
    return (bytes, png.ImgInfo.Cols, png.ImgInfo.Rows);
  }

}

}
