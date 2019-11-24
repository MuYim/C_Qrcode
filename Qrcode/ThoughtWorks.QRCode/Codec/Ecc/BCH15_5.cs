// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Ecc.BCH15_5
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

namespace ThoughtWorks.QRCode.Codec.Ecc
{
  public class BCH15_5
  {
    internal static string[] bitName = new string[15]
    {
      "c0",
      "c1",
      "c2",
      "c3",
      "c4",
      "c5",
      "c6",
      "c7",
      "c8",
      "c9",
      "d0",
      "d1",
      "d2",
      "d3",
      "d4"
    };
    internal int[][] gf16;
    internal bool[] recieveData;
    internal int numCorrectedError;

    public virtual int NumCorrectedError
    {
      get
      {
        return this.numCorrectedError;
      }
    }

    public BCH15_5(bool[] source)
    {
      this.gf16 = this.createGF16();
      this.recieveData = source;
    }

    public virtual bool[] correct()
    {
      return this.correctErrorBit(this.recieveData, this.detectErrorBitPosition(this.calcSyndrome(this.recieveData)));
    }

    internal virtual int[][] createGF16()
    {
      this.gf16 = new int[16][];
      for (int index = 0; index < 16; ++index)
        this.gf16[index] = new int[4];
      int[] numArray = new int[4]
      {
        1,
        1,
        0,
        0
      };
      for (int index = 0; index < 4; ++index)
        this.gf16[index][index] = 1;
      for (int index = 0; index < 4; ++index)
        this.gf16[4][index] = numArray[index];
      for (int index1 = 5; index1 < 16; ++index1)
      {
        for (int index2 = 1; index2 < 4; ++index2)
          this.gf16[index1][index2] = this.gf16[index1 - 1][index2 - 1];
        if (this.gf16[index1 - 1][3] == 1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            this.gf16[index1][index2] = (this.gf16[index1][index2] + numArray[index2]) % 2;
        }
      }
      return this.gf16;
    }

    internal virtual int searchElement(int[] x)
    {
      int index = 0;
      while (index < 15 && (x[0] != this.gf16[index][0] || x[1] != this.gf16[index][1] || x[2] != this.gf16[index][2] || x[3] != this.gf16[index][3]))
        ++index;
      return index;
    }

    internal virtual int[] getCode(int input)
    {
      int[] numArray1 = new int[15];
      int[] numArray2 = new int[8];
      for (int index = 0; index < 15; ++index)
      {
        int num1 = numArray2[7];
        int num2;
        int num3;
        if (index < 7)
        {
          num2 = (input >> 6 - index) % 2;
          num3 = (num2 + num1) % 2;
        }
        else
        {
          num2 = num1;
          num3 = 0;
        }
        numArray2[7] = (numArray2[6] + num3) % 2;
        numArray2[6] = (numArray2[5] + num3) % 2;
        numArray2[5] = numArray2[4];
        numArray2[4] = (numArray2[3] + num3) % 2;
        numArray2[3] = numArray2[2];
        numArray2[2] = numArray2[1];
        numArray2[1] = numArray2[0];
        numArray2[0] = num3;
        numArray1[14 - index] = num2;
      }
      return numArray1;
    }

    internal virtual int addGF(int arg1, int arg2)
    {
      int[] x = new int[4];
      for (int index = 0; index < 4; ++index)
      {
        int num1 = arg1 < 0 || arg1 >= 15 ? 0 : this.gf16[arg1][index];
        int num2 = arg2 < 0 || arg2 >= 15 ? 0 : this.gf16[arg2][index];
        x[index] = (num1 + num2) % 2;
      }
      return this.searchElement(x);
    }

    internal virtual int[] calcSyndrome(bool[] y)
    {
      int[] numArray = new int[5];
      int[] x1 = new int[4];
      for (int index1 = 0; index1 < 15; ++index1)
      {
        if (y[index1])
        {
          for (int index2 = 0; index2 < 4; ++index2)
            x1[index2] = (x1[index2] + this.gf16[index1][index2]) % 2;
        }
      }
      int num1 = this.searchElement(x1);
      numArray[0] = num1 >= 15 ? -1 : num1;
      int[] x2 = new int[4];
      for (int index1 = 0; index1 < 15; ++index1)
      {
        if (y[index1])
        {
          for (int index2 = 0; index2 < 4; ++index2)
            x2[index2] = (x2[index2] + this.gf16[index1 * 3 % 15][index2]) % 2;
        }
      }
      int num2 = this.searchElement(x2);
      numArray[2] = num2 >= 15 ? -1 : num2;
      int[] x3 = new int[4];
      for (int index1 = 0; index1 < 15; ++index1)
      {
        if (y[index1])
        {
          for (int index2 = 0; index2 < 4; ++index2)
            x3[index2] = (x3[index2] + this.gf16[index1 * 5 % 15][index2]) % 2;
        }
      }
      int num3 = this.searchElement(x3);
      numArray[4] = num3 >= 15 ? -1 : num3;
      return numArray;
    }

    internal virtual int[] calcErrorPositionVariable(int[] s)
    {
      int[] numArray = new int[4];
      numArray[0] = s[0];
      int num1 = (s[0] + s[1]) % 15;
      int num2 = this.addGF(s[2], num1);
      int num3 = num2 >= 15 ? -1 : num2;
      int num4 = (s[2] + s[1]) % 15;
      int num5 = this.addGF(s[4], num4);
      int num6 = num5 >= 15 ? -1 : num5;
      numArray[1] = num6 >= 0 || num3 >= 0 ? (num6 - num3 + 15) % 15 : -1;
      int num7 = (s[1] + numArray[0]) % 15;
      int num8 = this.addGF(s[2], num7);
      int num9 = (s[0] + numArray[1]) % 15;
      numArray[2] = this.addGF(num8, num9);
      return numArray;
    }

    internal virtual int[] detectErrorBitPosition(int[] s)
    {
      int[] numArray1 = this.calcErrorPositionVariable(s);
      int[] numArray2 = new int[4];
      if (numArray1[0] == -1)
        return numArray2;
      if (numArray1[1] == -1)
      {
        numArray2[0] = 1;
        numArray2[1] = numArray1[0];
        return numArray2;
      }
      for (int index = 0; index < 15; ++index)
      {
        int num1 = index * 3 % 15;
        int num2 = index * 2 % 15;
        int num3 = index;
        int num4 = (numArray1[0] + num2) % 15;
        if (this.addGF(this.addGF(num1, num4), this.addGF((numArray1[1] + num3) % 15, numArray1[2])) >= 15)
        {
          ++numArray2[0];
          numArray2[numArray2[0]] = index;
        }
      }
      return numArray2;
    }

    internal virtual bool[] correctErrorBit(bool[] y, int[] errorPos)
    {
      for (int index = 1; index <= errorPos[0]; ++index)
        y[errorPos[index]] = !y[errorPos[index]];
      this.numCorrectedError = errorPos[0];
      return y;
    }
  }
}
