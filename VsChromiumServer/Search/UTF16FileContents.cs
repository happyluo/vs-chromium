﻿// Copyright 2013 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VsChromiumCore.Win32.Memory;

namespace VsChromiumServer.Search {
  public class UTF16FileContents : FileContents {
    private readonly SafeHeapBlockHandle _heap;

    public UTF16FileContents(SafeHeapBlockHandle heap, DateTime utcLastWriteTime)
        : base(utcLastWriteTime) {
      this._heap = heap;
    }

    public override long ByteLength {
      get {
        return this._heap.ByteLength;
      }
    }

    [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = false)]
    public static extern IntPtr StrStrIW(IntPtr pszFirst, IntPtr pszSrch);

    [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = false)]
    public static extern IntPtr StrStrW(IntPtr pszFirst, IntPtr pszSrch);

    public override List<int> Search(SearchContentsData searchContentsData) {
      List<int> result = null;
      var contentsPtr = this._heap.Pointer;
      while (true) {
        var foundPtr = StrStrW(contentsPtr, searchContentsData.UniTextPtr.Pointer);
        if (foundPtr == IntPtr.Zero)
          break;

        if (result == null) {
          result = new List<int>();
        }
        // Note: We are limited to 2GB files by design.
        var position = (int)(foundPtr.ToInt64() - this._heap.Pointer.ToInt64());
        result.Add(position);

        contentsPtr = foundPtr + searchContentsData.Text.Length;
      }
      return result ?? NoPositions;
    }
  }
}