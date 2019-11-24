// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Util.ConsoleCanvas
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;
using ThoughtWorks.QRCode.Geom;

namespace ThoughtWorks.QRCode.Codec.Util
{
  public class ConsoleCanvas : DebugCanvas
  {
    public void println(string str)
    {
      Console.WriteLine(str);
    }

    public void drawPoint(Point point, int color)
    {
    }

    public void drawCross(Point point, int color)
    {
    }

    public void drawPoints(Point[] points, int color)
    {
    }

    public void drawLine(Line line, int color)
    {
    }

    public void drawLines(Line[] lines, int color)
    {
    }

    public void drawPolygon(Point[] points, int color)
    {
    }

    public void drawMatrix(bool[][] matrix)
    {
    }
  }
}
