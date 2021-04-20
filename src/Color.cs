using System;

namespace Rendering {

public record Color {
  public GL.Color color { get; init; }
}

public static class Colors {

  // Constants
  ///////////////////////////

  public static readonly Color Black       = New(0,   0,   0  );
  public static readonly Color White       = New(255, 255, 255);
  public static readonly Color Gray        = New(128, 128, 128);
  public static readonly Color Red         = New(255, 0,   0  );
  public static readonly Color Blue        = New(0,   0,   255);
  public static readonly Color Green       = New(0,   255, 0  );
  public static readonly Color Cyan        = New(0,   255, 255);
  public static readonly Color Magenta     = New(255, 0,   255);
  public static readonly Color Yellow      = New(255, 255, 0  );
  public static readonly Color DarkGray    = New(64,  64,  64 );
  public static readonly Color DarkRed     = New(139, 0,   0  );
  public static readonly Color DarkBlue    = New(0,   0,   139);
  public static readonly Color DarkGreen   = New(0,   139, 0  );
  public static readonly Color DarkCyan    = New(0,   139, 139);
  public static readonly Color DarkMagenta = New(139, 0,   139);
  public static readonly Color DarkYellow  = New(139, 139, 0  );

  // Public methods
  ///////////////////////////

  public static Color New(byte r, byte g, byte b) {
    return new Color {
      color = new GL.Color{
        r = r / 255f,
        g = g / 255f,
        b = b / 255f,
      }
    };
  }

}

}
