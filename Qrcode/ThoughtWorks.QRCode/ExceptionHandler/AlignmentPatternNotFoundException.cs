﻿// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.ExceptionHandler.AlignmentPatternNotFoundException
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;

namespace ThoughtWorks.QRCode.ExceptionHandler
{
  [Serializable]
  public class AlignmentPatternNotFoundException : ArgumentException
  {
    internal string message = (string) null;

    public override string Message
    {
      get
      {
        return this.message;
      }
    }

    public AlignmentPatternNotFoundException(string message)
    {
      this.message = message;
    }
  }
}