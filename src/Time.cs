using System;
using System.Diagnostics;

namespace Rendering {

static class Time {

  // Internal vars
  ////////////////////

  static Stopwatch stopwatch;

  // Constructor
  ////////////////////

  static Time() {
    stopwatch = new Stopwatch();
    stopwatch.Start();
  }

  // Public methods
  ////////////////////

  public static float Now() {
    return (float)stopwatch.Elapsed.TotalSeconds;
  }

}

}
