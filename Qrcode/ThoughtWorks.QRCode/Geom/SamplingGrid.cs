// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Geom.SamplingGrid
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

namespace ThoughtWorks.QRCode.Geom
{
  public class SamplingGrid
  {
    private SamplingGrid.AreaGrid[][] grid;

    public virtual int TotalWidth
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.grid.Length; ++index)
        {
          num += this.grid[index][0].Width;
          if (index > 0)
            --num;
        }
        return num;
      }
    }

    public virtual int TotalHeight
    {
      get
      {
        int num = 0;
        for (int index = 0; index < this.grid[0].Length; ++index)
        {
          num += this.grid[0][index].Height;
          if (index > 0)
            --num;
        }
        return num;
      }
    }

    public SamplingGrid(int sqrtNumArea)
    {
      this.grid = new SamplingGrid.AreaGrid[sqrtNumArea][];
      for (int index = 0; index < sqrtNumArea; ++index)
        this.grid[index] = new SamplingGrid.AreaGrid[sqrtNumArea];
    }

    public virtual void initGrid(int ax, int ay, int width, int height)
    {
      this.grid[ax][ay] = new SamplingGrid.AreaGrid(this, width, height);
    }

    public virtual void setXLine(int ax, int ay, int x, Line line)
    {
      this.grid[ax][ay].setXLine(x, line);
    }

    public virtual void setYLine(int ax, int ay, int y, Line line)
    {
      this.grid[ax][ay].setYLine(y, line);
    }

    public virtual Line getXLine(int ax, int ay, int x)
    {
      return this.grid[ax][ay].getXLine(x);
    }

    public virtual Line getYLine(int ax, int ay, int y)
    {
      return this.grid[ax][ay].getYLine(y);
    }

    public virtual Line[] getXLines(int ax, int ay)
    {
      return this.grid[ax][ay].XLines;
    }

    public virtual Line[] getYLines(int ax, int ay)
    {
      return this.grid[ax][ay].YLines;
    }

    public virtual int getWidth()
    {
      return this.grid[0].Length;
    }

    public virtual int getHeight()
    {
      return this.grid.Length;
    }

    public virtual int getWidth(int ax, int ay)
    {
      return this.grid[ax][ay].Width;
    }

    public virtual int getHeight(int ax, int ay)
    {
      return this.grid[ax][ay].Height;
    }

    public virtual int getX(int ax, int x)
    {
      int num = x;
      for (int index = 0; index < ax; ++index)
        num += this.grid[index][0].Width - 1;
      return num;
    }

    public virtual int getY(int ay, int y)
    {
      int num = y;
      for (int index = 0; index < ay; ++index)
        num += this.grid[0][index].Height - 1;
      return num;
    }

    public virtual void adjust(Point adjust)
    {
      int x = adjust.X;
      int y = adjust.Y;
      for (int index1 = 0; index1 < this.grid[0].Length; ++index1)
      {
        for (int index2 = 0; index2 < this.grid.Length; ++index2)
        {
          for (int index3 = 0; index3 < this.grid[index2][index1].XLines.Length; ++index3)
            this.grid[index2][index1].XLines[index3].translate(x, y);
          for (int index3 = 0; index3 < this.grid[index2][index1].YLines.Length; ++index3)
            this.grid[index2][index1].YLines[index3].translate(x, y);
        }
      }
    }

    private class AreaGrid
    {
      private SamplingGrid enclosingInstance;
      private Line[] xLine;
      private Line[] yLine;

      public virtual int Width
      {
        get
        {
          return this.xLine.Length;
        }
      }

      public virtual int Height
      {
        get
        {
          return this.yLine.Length;
        }
      }

      public virtual Line[] XLines
      {
        get
        {
          return this.xLine;
        }
      }

      public virtual Line[] YLines
      {
        get
        {
          return this.yLine;
        }
      }

      public SamplingGrid Enclosing_Instance
      {
        get
        {
          return this.enclosingInstance;
        }
      }

      public AreaGrid(SamplingGrid enclosingInstance, int width, int height)
      {
        this.InitBlock(enclosingInstance);
        this.xLine = new Line[width];
        this.yLine = new Line[height];
      }

      private void InitBlock(SamplingGrid enclosingInstance)
      {
        this.enclosingInstance = enclosingInstance;
      }

      public virtual Line getXLine(int x)
      {
        return this.xLine[x];
      }

      public virtual Line getYLine(int y)
      {
        return this.yLine[y];
      }

      public virtual void setXLine(int x, Line line)
      {
        this.xLine[x] = line;
      }

      public virtual void setYLine(int y, Line line)
      {
        this.yLine[y] = line;
      }
    }
  }
}
