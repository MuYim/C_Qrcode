// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Data.QRCodeBitmapImage
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System.Drawing;

namespace ThoughtWorks.QRCode.Codec.Data
{
  public class QRCodeBitmapImage : QRCodeImage
  {
    private Bitmap image;

    public virtual int Width
    {
      get
      {
        return this.image.Width;
      }
    }

    public virtual int Height
    {
      get
      {
        return this.image.Height;
      }
    }

    public QRCodeBitmapImage(Bitmap image)
    {
      this.image = image;
    }

    public virtual int getPixel(int x, int y)
    {
      return this.image.GetPixel(x, y).ToArgb();
    }
  }
}
