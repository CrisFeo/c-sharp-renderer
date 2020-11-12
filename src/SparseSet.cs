using System;
using System.Collections.Generic;

namespace Rendering {

public struct SparseSet<T> {
  public List<int> forwardLookup;
  public List<int> reverseLookup;
  public List<T> data;
}

public struct SparseEntry<T> {
  public int lookupIndex;
  public int dataIndex;
}

public static class SparseSets {

  public static SparseSet<T> New<T>() {
    return new SparseSet<T>{
      forwardLookup = new List<int>(),
      reverseLookup = new List<int>(),
      data = new List<T>(),
    };
  }

  public static SparseEntry<T> Set<T>(this ref SparseSet<T> s, int i, T v) {
    var dataIndex = default(int);
    if (s.forwardLookup.Count > i && s.forwardLookup[i] != -1) {
      dataIndex = s.forwardLookup[i];
      s.data[dataIndex] = v;
    } else {
      dataIndex = s.data.Count;
      s.data.Add(v);
      s.reverseLookup.Add(i);
      while (s.forwardLookup.Count <= i) {
        s.forwardLookup.Add(-1);
      }
      s.forwardLookup[i] = dataIndex;
    }
    return new SparseEntry<T> {
      lookupIndex = i,
      dataIndex = dataIndex,
    };
  }

  public static SparseEntry<T>? Lookup<T>(this ref SparseSet<T> s, int i) {
    if (i >= s.forwardLookup.Count) return null;
    var dataIndex = s.forwardLookup[i];
    if (dataIndex == -1) return null;
    return new SparseEntry<T> {
      lookupIndex = i,
      dataIndex = dataIndex,
    };
  }

  public static void Remove<T>(this ref SparseSet<T> s, SparseEntry<T> e) {
    var lastIndex = s.data.Count - 1;
    var lastLookupIndex = s.reverseLookup[lastIndex];
    s.forwardLookup[e.lookupIndex] = -1;
    s.forwardLookup[lastLookupIndex] = e.dataIndex;
    s.data[e.dataIndex] = s.data[lastIndex];
    s.data.RemoveAt(lastIndex);
    s.reverseLookup.RemoveAt(lastIndex);
  }

  public static void Clear<T>(this ref SparseSet<T> s) {
    s.forwardLookup.Clear();
    s.reverseLookup.Clear();
    s.data.Clear();
  }

  public static T Get<T>(this ref SparseSet<T> s, SparseEntry<T> e) {
    return s.data[e.dataIndex];
  }

  public static T? Get<T>(this ref SparseSet<T> s, int e) where T : struct {
    var instance = s.Lookup(e);
    if (!instance.HasValue) return null;
    return s.Get(instance.Value);
  }

}

}
