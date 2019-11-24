// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Reader.QRCodeImageReader
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;
using System.Collections;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Reader.Pattern;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;

namespace ThoughtWorks.QRCode.Codec.Reader
{
  public class QRCodeImageReader
  {
    public static int DECIMAL_POINT = 21;
    public const bool POINT_DARK = true;
    public const bool POINT_LIGHT = false;
    internal DebugCanvas canvas;
    internal SamplingGrid samplingGrid;
    internal bool[][] bitmap;

    public QRCodeImageReader()
    {
      this.canvas = QRCodeDecoder.Canvas;
    }

    internal virtual bool[][] applyMedianFilter(bool[][] image, int threshold)
    {
      bool[][] flagArray = new bool[image.Length][];
      for (int index = 0; index < image.Length; ++index)
        flagArray[index] = new bool[image[0].Length];
      for (int index1 = 1; index1 < image[0].Length - 1; ++index1)
      {
        for (int index2 = 1; index2 < image.Length - 1; ++index2)
        {
          int num = 0;
          for (int index3 = -1; index3 < 2; ++index3)
          {
            for (int index4 = -1; index4 < 2; ++index4)
            {
              if (image[index2 + index4][index1 + index3])
                ++num;
            }
          }
          if (num > threshold)
            flagArray[index2][index1] = true;
        }
      }
      return flagArray;
    }

    internal virtual bool[][] applyCrossMaskingMedianFilter(bool[][] image, int threshold)
    {
      bool[][] flagArray = new bool[image.Length][];
      for (int index = 0; index < image.Length; ++index)
        flagArray[index] = new bool[image[0].Length];
      for (int index1 = 2; index1 < image[0].Length - 2; ++index1)
      {
        for (int index2 = 2; index2 < image.Length - 2; ++index2)
        {
          int num = 0;
          for (int index3 = -2; index3 < 3; ++index3)
          {
            if (image[index2 + index3][index1])
              ++num;
            if (image[index2][index1 + index3])
              ++num;
          }
          if (num > threshold)
            flagArray[index2][index1] = true;
        }
      }
      return flagArray;
    }

    internal virtual bool[][] filterImage(int[][] image)
    {
      this.imageToGrayScale(image);
      return this.grayScaleToBitmap(image);
    }

    internal virtual void imageToGrayScale(int[][] image)
    {
      for (int index1 = 0; index1 < image[0].Length; ++index1)
      {
        for (int index2 = 0; index2 < image.Length; ++index2)
        {
          int num = ((image[index2][index1] >> 16 & (int) byte.MaxValue) * 30 + (image[index2][index1] >> 8 & (int) byte.MaxValue) * 59 + (image[index2][index1] & (int) byte.MaxValue) * 11) / 100;
          image[index2][index1] = num;
        }
      }
    }

    internal virtual bool[][] grayScaleToBitmap(int[][] grayScale)
    {
      int[][] brightnessPerArea = this.getMiddleBrightnessPerArea(grayScale);
      int length = brightnessPerArea.Length;
      int num1 = grayScale.Length / length;
      int num2 = grayScale[0].Length / length;
      bool[][] flagArray = new bool[grayScale.Length][];
      for (int index = 0; index < grayScale.Length; ++index)
        flagArray[index] = new bool[grayScale[0].Length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
        {
          for (int index3 = 0; index3 < num2; ++index3)
          {
            for (int index4 = 0; index4 < num1; ++index4)
              flagArray[num1 * index2 + index4][num2 * index1 + index3] = grayScale[num1 * index2 + index4][num2 * index1 + index3] < brightnessPerArea[index2][index1];
          }
        }
      }
      return flagArray;
    }

    internal virtual int[][] getMiddleBrightnessPerArea(int[][] image)
    {
      int length = 4;
      int num1 = image.Length / length;
      int num2 = image[0].Length / length;
      int[][][] numArray1 = new int[length][][];
      for (int index1 = 0; index1 < length; ++index1)
      {
        numArray1[index1] = new int[length][];
        for (int index2 = 0; index2 < length; ++index2)
          numArray1[index1][index2] = new int[2];
      }
      for (int index1 = 0; index1 < length; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
        {
          numArray1[index2][index1][0] = (int) byte.MaxValue;
          for (int index3 = 0; index3 < num2; ++index3)
          {
            for (int index4 = 0; index4 < num1; ++index4)
            {
              int num3 = image[num1 * index2 + index4][num2 * index1 + index3];
              if (num3 < numArray1[index2][index1][0])
                numArray1[index2][index1][0] = num3;
              if (num3 > numArray1[index2][index1][1])
                numArray1[index2][index1][1] = num3;
            }
          }
        }
      }
      int[][] numArray2 = new int[length][];
      for (int index = 0; index < length; ++index)
        numArray2[index] = new int[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
          numArray2[index2][index1] = (numArray1[index2][index1][0] + numArray1[index2][index1][1]) / 2;
      }
      return numArray2;
    }

    public virtual QRCodeSymbol getQRCodeSymbol(int[][] image)
    {
      QRCodeImageReader.DECIMAL_POINT = 23 - QRCodeUtility.sqrt((image.Length < image[0].Length ? image[0].Length : image.Length) / 256);
      this.bitmap = this.filterImage(image);
      this.canvas.println("Drawing matrix.");
      this.canvas.drawMatrix(this.bitmap);
      this.canvas.println("Scanning Finder Pattern.");
      FinderPattern finderPattern;
      try
      {
        finderPattern = FinderPattern.findFinderPattern(this.bitmap);
      }
      catch (FinderPatternNotFoundException ex1)
      {
        this.canvas.println("Not found, now retrying...");
        this.bitmap = this.applyCrossMaskingMedianFilter(this.bitmap, 5);
        this.canvas.drawMatrix(this.bitmap);
        int num = 0;
        while (num < 1000000000)
          ++num;
        try
        {
          finderPattern = FinderPattern.findFinderPattern(this.bitmap);
        }
        catch (FinderPatternNotFoundException ex2)
        {
          throw new SymbolNotFoundException(ex2.Message);
        }
        catch (VersionInformationException ex2)
        {
          throw new SymbolNotFoundException(ex2.Message);
        }
      }
      catch (VersionInformationException ex)
      {
        throw new SymbolNotFoundException(ex.Message);
      }
      this.canvas.println("FinderPattern at");
      this.canvas.println(finderPattern.getCenter(0).ToString() + finderPattern.getCenter(1).ToString() + finderPattern.getCenter(2).ToString());
      int[] angle = finderPattern.getAngle();
      this.canvas.println("Angle*4098: Sin " + Convert.ToString(angle[0]) + "  Cos " + Convert.ToString(angle[1]));
      int version = finderPattern.Version;
      this.canvas.println("Version: " + Convert.ToString(version));
      if (version < 1 || version > 40)
        throw new InvalidVersionException("Invalid version: " + (object) version);
      AlignmentPattern alignmentPattern;
      try
      {
        alignmentPattern = AlignmentPattern.findAlignmentPattern(this.bitmap, finderPattern);
      }
      catch (AlignmentPatternNotFoundException ex)
      {
        throw new SymbolNotFoundException(ex.Message);
      }
      int length = alignmentPattern.getCenter().Length;
      this.canvas.println("AlignmentPatterns at");
      for (int index1 = 0; index1 < length; ++index1)
      {
        string str = "";
        for (int index2 = 0; index2 < length; ++index2)
          str += alignmentPattern.getCenter()[index2][index1].ToString();
        this.canvas.println(str);
      }
      this.canvas.println("Creating sampling grid.");
      this.samplingGrid = this.getSamplingGrid(finderPattern, alignmentPattern);
      this.canvas.println("Reading grid.");
      bool[][] qrCodeMatrix;
      try
      {
        qrCodeMatrix = this.getQRCodeMatrix(this.bitmap, this.samplingGrid);
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
      }
      return new QRCodeSymbol(qrCodeMatrix);
    }

    public virtual QRCodeSymbol getQRCodeSymbolWithAdjustedGrid(Point adjust)
    {
      if (this.bitmap == null || this.samplingGrid == null)
        throw new SystemException("This method must be called after QRCodeImageReader.getQRCodeSymbol() called");
      this.samplingGrid.adjust(adjust);
      this.canvas.println("Sampling grid adjusted d(" + (object) adjust.X + "," + (string) (object) adjust.Y + ")");
      bool[][] qrCodeMatrix;
      try
      {
        qrCodeMatrix = this.getQRCodeMatrix(this.bitmap, this.samplingGrid);
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
      }
      return new QRCodeSymbol(qrCodeMatrix);
    }

    internal virtual SamplingGrid getSamplingGrid(FinderPattern finderPattern, AlignmentPattern alignmentPattern)
    {
      Point[][] center = alignmentPattern.getCenter();
      int version = finderPattern.Version;
      int num = version / 7 + 2;
      center[0][0] = finderPattern.getCenter(0);
      center[num - 1][0] = finderPattern.getCenter(1);
      center[0][num - 1] = finderPattern.getCenter(2);
      int sqrtNumArea = num - 1;
      SamplingGrid samplingGrid = new SamplingGrid(sqrtNumArea);
      Axis axis = new Axis(finderPattern.getAngle(), finderPattern.getModuleSize());
      for (int ay = 0; ay < sqrtNumArea; ++ay)
      {
        for (int ax = 0; ax < sqrtNumArea; ++ax)
        {
          QRCodeImageReader.ModulePitch modulePitch = new QRCodeImageReader.ModulePitch(this);
          Line line1 = new Line();
          Line line2 = new Line();
          axis.ModulePitch = finderPattern.getModuleSize();
          Point[][] logicalCenter = AlignmentPattern.getLogicalCenter(finderPattern);
          Point point1 = center[ax][ay];
          Point point2 = center[ax + 1][ay];
          Point point3 = center[ax][ay + 1];
          Point point4 = center[ax + 1][ay + 1];
          Point point5 = logicalCenter[ax][ay];
          Point point6 = logicalCenter[ax + 1][ay];
          Point point7 = logicalCenter[ax][ay + 1];
          Point point8 = logicalCenter[ax + 1][ay + 1];
          if (ax == 0 && ay == 0)
          {
            if (sqrtNumArea == 1)
            {
              point1 = axis.translate(point1, -3, -3);
              point2 = axis.translate(point2, 3, -3);
              point3 = axis.translate(point3, -3, 3);
              point4 = axis.translate(point4, 6, 6);
              point5.translate(-6, -6);
              point6.translate(3, -3);
              point7.translate(-3, 3);
              point8.translate(6, 6);
            }
            else
            {
              point1 = axis.translate(point1, -3, -3);
              point2 = axis.translate(point2, 0, -6);
              point3 = axis.translate(point3, -6, 0);
              point5.translate(-6, -6);
              point6.translate(0, -6);
              point7.translate(-6, 0);
            }
          }
          else if (ax == 0 && ay == sqrtNumArea - 1)
          {
            point1 = axis.translate(point1, -6, 0);
            point3 = axis.translate(point3, -3, 3);
            point4 = axis.translate(point4, 0, 6);
            point5.translate(-6, 0);
            point7.translate(-6, 6);
            point8.translate(0, 6);
          }
          else if (ax == sqrtNumArea - 1 && ay == 0)
          {
            point1 = axis.translate(point1, 0, -6);
            point2 = axis.translate(point2, 3, -3);
            point4 = axis.translate(point4, 6, 0);
            point5.translate(0, -6);
            point6.translate(6, -6);
            point8.translate(6, 0);
          }
          else if (ax == sqrtNumArea - 1 && ay == sqrtNumArea - 1)
          {
            point3 = axis.translate(point3, 0, 6);
            point2 = axis.translate(point2, 6, 0);
            point4 = axis.translate(point4, 6, 6);
            point7.translate(0, 6);
            point6.translate(6, 0);
            point8.translate(6, 6);
          }
          else if (ax == 0)
          {
            point1 = axis.translate(point1, -6, 0);
            point3 = axis.translate(point3, -6, 0);
            point5.translate(-6, 0);
            point7.translate(-6, 0);
          }
          else if (ax == sqrtNumArea - 1)
          {
            point2 = axis.translate(point2, 6, 0);
            point4 = axis.translate(point4, 6, 0);
            point6.translate(6, 0);
            point8.translate(6, 0);
          }
          else if (ay == 0)
          {
            point1 = axis.translate(point1, 0, -6);
            point2 = axis.translate(point2, 0, -6);
            point5.translate(0, -6);
            point6.translate(0, -6);
          }
          else if (ay == sqrtNumArea - 1)
          {
            point3 = axis.translate(point3, 0, 6);
            point4 = axis.translate(point4, 0, 6);
            point7.translate(0, 6);
            point8.translate(0, 6);
          }
          if (ax == 0)
          {
            point6.translate(1, 0);
            point8.translate(1, 0);
          }
          else
          {
            point5.translate(-1, 0);
            point7.translate(-1, 0);
          }
          if (ay == 0)
          {
            point7.translate(0, 1);
            point8.translate(0, 1);
          }
          else
          {
            point5.translate(0, -1);
            point6.translate(0, -1);
          }
          int width = point6.X - point5.X;
          int height = point7.Y - point5.Y;
          if (version < 7)
          {
            width += 3;
            height += 3;
          }
          modulePitch.top = this.getAreaModulePitch(point1, point2, width - 1);
          modulePitch.left = this.getAreaModulePitch(point1, point3, height - 1);
          modulePitch.bottom = this.getAreaModulePitch(point3, point4, width - 1);
          modulePitch.right = this.getAreaModulePitch(point2, point4, height - 1);
          line1.setP1(point1);
          line2.setP1(point1);
          line1.setP2(point3);
          line2.setP2(point2);
          samplingGrid.initGrid(ax, ay, width, height);
          for (int index = 0; index < width; ++index)
          {
            Line line3 = new Line(line1.getP1(), line1.getP2());
            axis.Origin = line3.getP1();
            axis.ModulePitch = modulePitch.top;
            line3.setP1(axis.translate(index, 0));
            axis.Origin = line3.getP2();
            axis.ModulePitch = modulePitch.bottom;
            line3.setP2(axis.translate(index, 0));
            samplingGrid.setXLine(ax, ay, index, line3);
          }
          for (int index = 0; index < height; ++index)
          {
            Line line3 = new Line(line2.getP1(), line2.getP2());
            axis.Origin = line3.getP1();
            axis.ModulePitch = modulePitch.left;
            line3.setP1(axis.translate(0, index));
            axis.Origin = line3.getP2();
            axis.ModulePitch = modulePitch.right;
            line3.setP2(axis.translate(0, index));
            samplingGrid.setYLine(ax, ay, index, line3);
          }
        }
      }
      return samplingGrid;
    }

    internal virtual int getAreaModulePitch(Point start, Point end, int logicalDistance)
    {
      return (new Line(start, end).Length << QRCodeImageReader.DECIMAL_POINT) / logicalDistance;
    }

    internal virtual bool[][] getQRCodeMatrix(bool[][] image, SamplingGrid gridLines)
    {
      int totalWidth = gridLines.TotalWidth;
      this.canvas.println("gridSize=" + (object) totalWidth);
      Point point = (Point) null;
      bool[][] flagArray = new bool[totalWidth][];
      for (int index = 0; index < totalWidth; ++index)
        flagArray[index] = new bool[totalWidth];
      for (int ay = 0; ay < gridLines.getHeight(); ++ay)
      {
        for (int ax = 0; ax < gridLines.getWidth(); ++ax)
        {
          ArrayList.Synchronized(new ArrayList(10));
          for (int y1 = 0; y1 < gridLines.getHeight(ax, ay); ++y1)
          {
            for (int x1 = 0; x1 < gridLines.getWidth(ax, ay); ++x1)
            {
              int x2 = gridLines.getXLine(ax, ay, x1).getP1().X;
              int y2 = gridLines.getXLine(ax, ay, x1).getP1().Y;
              int x3 = gridLines.getXLine(ax, ay, x1).getP2().X;
              int y3 = gridLines.getXLine(ax, ay, x1).getP2().Y;
              int x4 = gridLines.getYLine(ax, ay, y1).getP1().X;
              int y4 = gridLines.getYLine(ax, ay, y1).getP1().Y;
              int x5 = gridLines.getYLine(ax, ay, y1).getP2().X;
              int y5 = gridLines.getYLine(ax, ay, y1).getP2().Y;
              int num1 = (y3 - y2) * (x4 - x5) - (y5 - y4) * (x2 - x3);
              int num2 = (x2 * y3 - x3 * y2) * (x4 - x5) - (x4 * y5 - x5 * y4) * (x2 - x3);
              int num3 = (x4 * y5 - x5 * y4) * (y3 - y2) - (x2 * y3 - x3 * y2) * (y5 - y4);
              flagArray[gridLines.getX(ax, x1)][gridLines.getY(ay, y1)] = image[num2 / num1][num3 / num1];
              if (ay == gridLines.getHeight() - 1 && ax == gridLines.getWidth() - 1 && y1 == gridLines.getHeight(ax, ay) - 1 && x1 == gridLines.getWidth(ax, ay) - 1)
                point = new Point(num2 / num1, num3 / num1);
            }
          }
        }
      }
      if (point.X > image.Length - 1 || point.Y > image[0].Length - 1)
        throw new IndexOutOfRangeException("Sampling grid pointed out of image");
      this.canvas.drawPoint(point, Color_Fields.BLUE);
      return flagArray;
    }

    private class ModulePitch
    {
      public int top;
      public int left;
      public int bottom;
      public int right;
      private QRCodeImageReader enclosingInstance;

      public QRCodeImageReader Enclosing_Instance
      {
        get
        {
          return this.enclosingInstance;
        }
      }

      public ModulePitch(QRCodeImageReader enclosingInstance)
      {
        this.InitBlock(enclosingInstance);
      }

      private void InitBlock(QRCodeImageReader enclosingInstance)
      {
        this.enclosingInstance = enclosingInstance;
      }
    }
  }
}
