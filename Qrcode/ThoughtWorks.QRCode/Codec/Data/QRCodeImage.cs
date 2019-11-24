// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Data.QRCodeImage
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

namespace ThoughtWorks.QRCode.Codec.Data
{
  public interface QRCodeImage
  {
    int Width { get; }

    int Height { get; }

    int getPixel(int x, int y);
  }
}
