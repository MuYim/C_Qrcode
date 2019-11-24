// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.ExceptionHandler.SymbolNotFoundException
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;

namespace ThoughtWorks.QRCode.ExceptionHandler
{
  [Serializable]
  public class SymbolNotFoundException : ArgumentException
  {
    internal string message = (string) null;

    public override string Message
    {
      get
      {
        return this.message;
      }
    }

    public SymbolNotFoundException(string message)
    {
      this.message = message;
    }
  }
}
