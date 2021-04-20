using System;
using System.Collections.Generic;

namespace Rendering {

internal struct ArrLst<T> {

  public T[] data;
  public int count;

  public static ArrLst<T> New(int size) => new ArrLst<T>{
    data = new T[size],
    count = 0,
  };

}

internal static class ArrLstImpl {

  // Public methods
  ///////////////////////////

  public static void Clear<T>(this ref ArrLst<T> lst) {
    lst.count = 0;
  }

  public static void Add<T>(this ref ArrLst<T> lst, T value) {
    if (lst.data.Length <= lst.count) {
      var newLength = 2 * lst.data.Length;
      if (newLength == 0) newLength = 1;
      Array.Resize(ref lst.data, newLength);
    }
    lst.data[lst.count] = value;
    lst.count++;
  }

  public static ref T At<T>(this ref ArrLst<T> lst, int i) {
    return ref lst.data[i];
  }

}

}
