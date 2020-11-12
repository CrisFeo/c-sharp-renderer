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

public struct Batch {
  public int offset;
  public int count;
  public GL.Texture texture;
}

public struct SpriteBatcher {
  public GL.VertexArray vertexArray;
  public List<Sprite> sprites;
  public List<Batch> batches;
  public List<GL.Vertex> vertices;
}

public static class SpriteBatchers {

  // Public methods
  ///////////////////////////

  public static SpriteBatcher New() {
    return new SpriteBatcher {
      vertexArray = GL.CreateVertexArray(),
      sprites = new List<Sprite>(),
      batches = new List<Batch>(),
      vertices = new List<GL.Vertex>(),
    };
  }

  public static void Begin(this ref SpriteBatcher s) {
    s.sprites.Clear();
    s.batches.Clear();
    s.vertices.Clear();
  }

  public static void Draw(this ref SpriteBatcher s, Sprite sprite) {
    s.sprites.Add(sprite);
  }

  public static void End(this ref SpriteBatcher s) {
    s.sprites.Sort((a, b) => a.texture.id.CompareTo(b.texture.id));
    var spriteEnumerator = s.sprites.GetEnumerator();
    var current = spriteEnumerator.Current;
    var offset = 0;
    while (spriteEnumerator.MoveNext()) {
      var previous = current;
      current = spriteEnumerator.Current;
      if (current.texture != previous.texture) {
        s.batches.Add(new Batch{
          offset = offset,
          count = 6,
          texture = current.texture,
        });
      } else {
        var b = s.batches[s.batches.Count-1];
        b.count += 6;
        s.batches[s.batches.Count-1] = b;
      }
      s.vertices.Add(current.topLeft);
      s.vertices.Add(current.bottomLeft);
      s.vertices.Add(current.bottomRight);
      s.vertices.Add(current.bottomRight);
      s.vertices.Add(current.topRight);
      s.vertices.Add(current.topLeft);
      offset += 6;
    }
    GL.Buffer(ref s.vertexArray, s.vertices);
  }

  public static void Render(this ref SpriteBatcher s) {
    GL.Bind(s.vertexArray);
    foreach (var b in s.batches) {
      GL.Bind(b.texture);
      GL.DrawArrays(b.offset, b.count);
    }
  }

}

}
