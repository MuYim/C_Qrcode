// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Reader.Pattern.AlignmentPattern
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;

namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
  public class AlignmentPattern
  {
    internal static DebugCanvas canvas = QRCodeDecoder.Canvas;
    internal const int RIGHT = 1;
    internal const int BOTTOM = 2;
    internal const int LEFT = 3;
    internal const int TOP = 4;
    internal Point[][] center;
    internal int patternDistance;

    public virtual int LogicalDistance
    {
      get
      {
        return this.patternDistance;
      }
    }

    internal AlignmentPattern(Point[][] center, int patternDistance)
    {
      this.center = center;
      this.patternDistance = patternDistance;
    }

    public static AlignmentPattern findAlignmentPattern(bool[][] image, FinderPattern finderPattern)
    {
      Point[][] logicalCenter = AlignmentPattern.getLogicalCenter(finderPattern);
      int patternDistance = logicalCenter[1][0].X - logicalCenter[0][0].X;
      return new AlignmentPattern(AlignmentPattern.getCenter(image, finderPattern, logicalCenter), patternDistance);
    }

    public virtual Point[][] getCenter()
    {
      return this.center;
    }

    public virtual void setCenter(Point[][] center)
    {
      this.center = center;
    }

    internal static Point[][] getCenter(bool[][] image, FinderPattern finderPattern, Point[][] logicalCenters)
    {
      int moduleSize = finderPattern.getModuleSize();
      Axis axis = new Axis(finderPattern.getAngle(), moduleSize);
      int length = logicalCenters.Length;
      Point[][] pointArray = new Point[length][];
      for (int index = 0; index < length; ++index)
        pointArray[index] = new Point[length];
      axis.Origin = finderPattern.getCenter(0);
      pointArray[0][0] = axis.translate(3, 3);
      AlignmentPattern.canvas.drawCross(pointArray[0][0], Color_Fields.BLUE);
      axis.Origin = finderPattern.getCenter(1);
      pointArray[length - 1][0] = axis.translate(-3, 3);
      AlignmentPattern.canvas.drawCross(pointArray[length - 1][0], Color_Fields.BLUE);
      axis.Origin = finderPattern.getCenter(2);
      pointArray[0][length - 1] = axis.translate(3, -3);
      AlignmentPattern.canvas.drawCross(pointArray[0][length - 1], Color_Fields.BLUE);
      Point p1 = pointArray[0][0];
      for (int index1 = 0; index1 < length; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
        {
          if ((index2 != 0 || index1 != 0) && (index2 != 0 || index1 != length - 1) && (index2 != length - 1 || index1 != 0))
          {
            Point point1 = (Point) null;
            if (index1 == 0)
            {
              if (index2 > 0 && index2 < length - 1)
                point1 = axis.translate(pointArray[index2 - 1][index1], logicalCenters[index2][index1].X - logicalCenters[index2 - 1][index1].X, 0);
              pointArray[index2][index1] = new Point(point1.X, point1.Y);
              AlignmentPattern.canvas.drawCross(pointArray[index2][index1], Color_Fields.RED);
            }
            else if (index2 == 0)
            {
              if (index1 > 0 && index1 < length - 1)
                point1 = axis.translate(pointArray[index2][index1 - 1], 0, logicalCenters[index2][index1].Y - logicalCenters[index2][index1 - 1].Y);
              pointArray[index2][index1] = new Point(point1.X, point1.Y);
              AlignmentPattern.canvas.drawCross(pointArray[index2][index1], Color_Fields.RED);
            }
            else
            {
              Point point2 = axis.translate(pointArray[index2 - 1][index1], logicalCenters[index2][index1].X - logicalCenters[index2 - 1][index1].X, 0);
              Point point3 = axis.translate(pointArray[index2][index1 - 1], 0, logicalCenters[index2][index1].Y - logicalCenters[index2][index1 - 1].Y);
              pointArray[index2][index1] = new Point((point2.X + point3.X) / 2, (point2.Y + point3.Y) / 2 + 1);
            }
            if (finderPattern.Version > 1)
            {
              Point precisionCenter = AlignmentPattern.getPrecisionCenter(image, pointArray[index2][index1]);
              if (pointArray[index2][index1].distanceOf(precisionCenter) < 6)
              {
                AlignmentPattern.canvas.drawCross(pointArray[index2][index1], Color_Fields.RED);
                int num1 = precisionCenter.X - pointArray[index2][index1].X;
                int num2 = precisionCenter.Y - pointArray[index2][index1].Y;
                AlignmentPattern.canvas.println("Adjust AP(" + (object) index2 + "," + (string) (object) index1 + ") to d(" + (string) (object) num1 + "," + (string) (object) num2 + ")");
                pointArray[index2][index1] = precisionCenter;
              }
            }
            AlignmentPattern.canvas.drawCross(pointArray[index2][index1], Color_Fields.BLUE);
            AlignmentPattern.canvas.drawLine(new Line(p1, pointArray[index2][index1]), Color_Fields.LIGHTBLUE);
            p1 = pointArray[index2][index1];
          }
        }
      }
      return pointArray;
    }

    internal static Point getPrecisionCenter(bool[][] image, Point targetPoint)
    {
      int x1 = targetPoint.X;
      int y1 = targetPoint.Y;
      if (x1 < 0 || y1 < 0 || (x1 > image.Length - 1 || y1 > image[0].Length - 1))
        throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
      if (!image[targetPoint.X][targetPoint.Y])
      {
        int num = 0;
        bool flag = false;
        while (!flag)
        {
          ++num;
          for (int index1 = num; index1 > -num; --index1)
          {
            for (int index2 = num; index2 > -num; --index2)
            {
              int index3 = targetPoint.X + index2;
              int index4 = targetPoint.Y + index1;
              if (index3 < 0 || index4 < 0 || (index3 > image.Length - 1 || index4 > image[0].Length - 1))
                throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
              if (image[index3][index4])
              {
                targetPoint = new Point(targetPoint.X + index2, targetPoint.Y + index1);
                flag = true;
              }
            }
          }
        }
      }
      int x2;
      int x3 = x2 = targetPoint.X;
      int x4 = x2;
      int num1 = x2;
      int y2;
      int y3 = y2 = targetPoint.Y;
      int y4 = y2;
      int num2 = y2;
      while (x4 >= 1 && !AlignmentPattern.targetPointOnTheCorner(image, x4, num2, x4 - 1, num2))
        --x4;
      while (x3 < image.Length - 1 && !AlignmentPattern.targetPointOnTheCorner(image, x3, num2, x3 + 1, num2))
        ++x3;
      while (y4 >= 1 && !AlignmentPattern.targetPointOnTheCorner(image, num1, y4, num1, y4 - 1))
        --y4;
      while (y3 < image[0].Length - 1 && !AlignmentPattern.targetPointOnTheCorner(image, num1, y3, num1, y3 + 1))
        ++y3;
      return new Point((x4 + x3 + 1) / 2, (y4 + y3 + 1) / 2);
    }

    internal static bool targetPointOnTheCorner(bool[][] image, int x, int y, int nx, int ny)
    {
      if (x < 0 || y < 0 || (nx < 0 || ny < 0) || (x > image.Length || y > image[0].Length || nx > image.Length) || ny > image[0].Length)
        throw new AlignmentPatternNotFoundException("Alignment Pattern Finder exceeded image edge");
      return !image[x][y] && image[nx][ny];
    }

    public static Point[][] getLogicalCenter(FinderPattern finderPattern)
    {
      int version = finderPattern.Version;
      Point[][] pointArray1 = new Point[1][];
      for (int index = 0; index < 1; ++index)
        pointArray1[index] = new Point[1];
      int[] numArray = new int[1];
      int[] seed = LogicalSeed.getSeed(version);
      Point[][] pointArray2 = new Point[seed.Length][];
      for (int index = 0; index < seed.Length; ++index)
        pointArray2[index] = new Point[seed.Length];
      for (int index1 = 0; index1 < pointArray2.Length; ++index1)
      {
        for (int index2 = 0; index2 < pointArray2.Length; ++index2)
          pointArray2[index2][index1] = new Point(seed[index2], seed[index1]);
      }
      return pointArray2;
    }
  }
}
