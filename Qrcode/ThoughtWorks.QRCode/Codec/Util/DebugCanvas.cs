// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Util.DebugCanvas
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using ThoughtWorks.QRCode.Geom;

namespace ThoughtWorks.QRCode.Codec.Util
{
  public interface DebugCanvas
  {
    void println(string str);

    void drawPoint(Point point, int color);

    void drawCross(Point point, int color);

    void drawPoints(Point[] points, int color);

    void drawLine(Line line, int color);

    void drawLines(Line[] lines, int color);

    void drawPolygon(Point[] points, int color);

    void drawMatrix(bool[][] matrix);
  }
}
