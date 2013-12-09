﻿// Copyright 2013 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace VsChromiumCore.Win32.Memory {
  public class SafeHeapBlockHandle : SafeHandleZeroOrMinusOneIsInvalid {
    private readonly long _byteLength;
    private readonly SafeHeapHandle _heap;

    public SafeHeapBlockHandle(SafeHeapHandle heap, IntPtr handle, long byteLength)
        : base(true) {
      this._heap = heap;
      this._byteLength = byteLength;
      SetHandle(handle);
    }

    public long ByteLength {
      get {
        return this._byteLength;
      }
    }

    public IntPtr Pointer {
      get {
        return DangerousGetHandle();
      }
    }

    protected override bool ReleaseHandle() {
      HeapAllocStatic.OnFree(ByteLength);
      return NativeMethods.HeapFree(this._heap, HeapFlags.Default, handle);
    }

    public byte[] ToArray() {
      var buf = new byte[ByteLength];
      Marshal.Copy(Pointer, buf, 0, buf.Length);
      return buf;
    }
  }
}