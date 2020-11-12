using System;
using System.Collections.Generic;

namespace Rendering {

class SparseSet<K, V> where V : new() {

  // Constants
  ///////////////////////////

  const int INITIAL_CAPACITY = 4;

  static readonly V DEFAULT = new V();

  // Fields
  ///////////////////////////

  Func<K,int> indexer;
  Action<int,int> onMoveValue;
  int[] forwardLookup;
  int[] reverseLookup;
  K[] keys;
  V[] values;
  int nextValueIndex;

  // Constructors
  ///////////////////////////

  public SparseSet(Func<K, int> indexer, Action<int, int> onMoveValue = null) {
    this.indexer = indexer;
    this.onMoveValue = onMoveValue;
    forwardLookup = new int[INITIAL_CAPACITY];
    Array.Fill(forwardLookup, -1);
    reverseLookup = new int[INITIAL_CAPACITY];
    Array.Fill(reverseLookup, -1);
    keys = new K[INITIAL_CAPACITY];
    values = new V[INITIAL_CAPACITY];
    nextValueIndex = 0;
  }

  // Public methods
  ///////////////////////////

  public void Clear() {
    nextValueIndex = 0;
    Array.Fill(forwardLookup, -1);
  }

  public ref V Add(K key) {
    var keyIndex = indexer(key);
    if (IsValidKeyIndex(keyIndex)) {
      var valueIndex = forwardLookup[keyIndex];
      if (IsValidValueIndex(valueIndex)) {
        throw new ArgumentException("key already present in set", nameof(key));
      }
    }
    if (keyIndex >= forwardLookup.Length) {
      GrowAndFill(ref forwardLookup, keyIndex, -1);
    }
    if (nextValueIndex >= values.Length) {
      GrowAndFill(ref reverseLookup, nextValueIndex, -1);
      Array.Resize(ref keys, reverseLookup.Length);
      Array.Resize(ref values, reverseLookup.Length);
    }
    keys[nextValueIndex] = key;
    ref var value = ref values[nextValueIndex];
    reverseLookup[nextValueIndex] = keyIndex;
    forwardLookup[keyIndex] = nextValueIndex;
    nextValueIndex++;
    if (value == null) {
      value = new V();
    }
    return ref value;
  }

  public void Remove(K key) {
    var (keyIndex, valueToRemove, ok) = Indices(key);
    if (!ok) return;
    if (nextValueIndex != 0) {
      var valueToSwap = --nextValueIndex;
      var keyIndexToSwap = reverseLookup[valueToSwap];
      keys[valueToRemove] = keys[valueToSwap];
      values[valueToRemove] = values[valueToSwap];
      if (onMoveValue != null) onMoveValue(valueToSwap, valueToRemove);
      reverseLookup[valueToRemove] = keyIndexToSwap;
      forwardLookup[keyIndexToSwap] = valueToRemove;
      reverseLookup[valueToSwap] = -1;
    } else {
      reverseLookup[valueToRemove] = -1;
    }
    forwardLookup[keyIndex] = -1;
  }

  public bool Has(K key) {
    var (_, _, ok) = Indices(key);
    return ok;
  }

  public ref V Get(K key) {
    var (_, valueIndex, ok) = Indices(key);
    if (!ok) throw new KeyNotFoundException();
    return ref values[valueIndex];
  }

  public ref V GetOrAdd(K key) {
    var (_, valueIndex, ok) = Indices(key);
    if (ok) return ref values[valueIndex];
    return ref Add(key);
  }

  public int Size() {
    return nextValueIndex;
  }

  public K At(int index) {
    if (!IsValidValueIndex(index)) throw new IndexOutOfRangeException();
    return keys[index];
  }

  public ref V GetAt(int index) {
    return ref Get(At(index));
  }

  // Internal methods
  ///////////////////////////

  (int, int, bool) Indices(K key) {
    var keyIndex = indexer(key);
    if (!IsValidKeyIndex(keyIndex)) return (-1, -1, false);
    var valueIndex = forwardLookup[keyIndex];
    if (!IsValidValueIndex(valueIndex)) return (-1, -1, false);
    return (keyIndex, valueIndex, true);
  }

  bool IsValidKeyIndex(int index) {
    if (index < 0) return false;
    if (index >= forwardLookup.Length) return false;
    return true;
  }

  bool IsValidValueIndex(int index) {
    if (index < 0) return false;
    if (index > nextValueIndex) return false;
    return true;
  }

  static void GrowAndFill<T>(ref T[] array, int index, T value) {
    var oldLength = array.Length;
    var newSize = array.Length;
    while (newSize <= index) newSize *= 2;
    Array.Resize(ref array, newSize);
    Array.Fill(array, value, oldLength, array.Length - oldLength);
  }

}

}
