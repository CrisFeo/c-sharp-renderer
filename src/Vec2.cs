using System;

namespace Rendering {

public struct Vec2 {
  public static Vec2 zero = new Vec2(0, 0);
  public static Vec2 one = new Vec2(1, 1);
  public int x;
  public int y;
  public int w { get => x; set => x = value; }
  public int h { get => y; set => y = value; }
  public Vec2(int x, int y) { this.x = x; this.y = y; }
  public static Vec2 operator+(Vec2 a, Vec2 b) => new Vec2(a.x + b.x, a.y + b.y);
  public static Vec2 operator-(Vec2 a, Vec2 b) => new Vec2(a.x - b.x, a.y - b.y);
  public static Vec2 operator/(Vec2 a, float b) => new Vec2((int)(a.x / b), (int)(a.y / b));
  public static bool operator==(Vec2 a, Vec2 b) => a.x == b.x && a.y == b.y;
  public static bool operator!=(Vec2 a, Vec2 b) => !(a == b);
  public override bool Equals(object o) => (o is Vec2 v) ? this == v : false;
  public override int GetHashCode() => (x << 2) ^ y;
}

public static class Vec2s {

  public static bool InBounds(this Vec2 v, Vec2 low, Vec2 high) {
    return v.x >= low.x && v.x < high.x && v.y >= low.y && v.y < high.y;
  }

}

}
