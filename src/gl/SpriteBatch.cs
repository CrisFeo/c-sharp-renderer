using System;
using System.Collections.Generic;
using System.Linq;

namespace Rendering {

public struct Sprite {
  public GL.Texture texture;
  public GL.Vertex topLeft;
  public GL.Vertex topRight;
  public GL.Vertex bottomRight;
  public GL.Vertex bottomLeft;
}

internal struct Batch {
  public int offset;
  public int count;
  public GL.Texture texture;
}

public struct SpriteBatcher {

  internal GL.VertexArray vertexArray;
  internal ArrLst<Sprite> sprites;
  internal ArrLst<Batch> batches;
  internal ArrLst<GL.Vertex> vertices;

  public static SpriteBatcher New() => new SpriteBatcher{
    vertexArray = GL.CreateVertexArray(),
    sprites = ArrLst<Sprite>.New(0),
    batches = ArrLst<Batch>.New(0),
    vertices = ArrLst<GL.Vertex>.New(0),
  };

}

public static class SpriteBatcherImpl {

  // Classes
  ///////////////////////////

  class SpriteByTexture : IComparer<Sprite> {
    public static readonly SpriteByTexture INSTANCE = new SpriteByTexture();
    public int Compare(Sprite a, Sprite b) => a.texture.id.CompareTo(b.texture.id);
  }

  // Public methods
  ///////////////////////////

  public static void Begin(this ref SpriteBatcher s) {
    s.sprites.Clear();
    s.batches.Clear();
    s.vertices.Clear();
  }

  public static void Draw(this ref SpriteBatcher s, Sprite sprite) {
    s.sprites.Add(sprite);
  }

  public static void End(this ref SpriteBatcher s, bool batchByTexture = true) {
    if (batchByTexture) {
      Array.Sort(
        s.sprites.data,
        0,
        s.sprites.count,
        SpriteByTexture.INSTANCE
      );
    }
    var current = default(Sprite);
    var offset = 0;
    for (var i = 0; i < s.sprites.count; i++) {
      var previous = current;
      current = s.sprites.At(i);
      if (current.texture != previous.texture) {
        s.batches.Add(new Batch{
          offset = offset,
          count = 6,
          texture = current.texture,
        });
      } else {
        ref var b = ref s.batches.At(s.batches.count-1);
        b.count += 6;
      }
      s.vertices.Add(current.topLeft);
      s.vertices.Add(current.bottomLeft);
      s.vertices.Add(current.bottomRight);
      s.vertices.Add(current.bottomRight);
      s.vertices.Add(current.topRight);
      s.vertices.Add(current.topLeft);
      offset += 6;
    }
    GL.Buffer(ref s.vertexArray, s.vertices.data, s.vertices.count);
  }

  public static void Render(this ref SpriteBatcher s) {
    GL.Bind(s.vertexArray);
    for (var i = 0; i < s.batches.count; i++) {
      var b = s.batches.At(i);
      GL.Bind(b.texture);
      GL.DrawArrays(b.offset, b.count);
    }
  }

}

}
