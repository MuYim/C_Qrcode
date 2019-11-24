// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Reader.QRCodeDataBlockReader
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;
using System.IO;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;

namespace ThoughtWorks.QRCode.Codec.Reader
{
  public class QRCodeDataBlockReader
  {
    private int[][] sizeOfDataLengthInfo = new int[3][]
    {
      new int[4]
      {
        10,
        9,
        8,
        8
      },
      new int[4]
      {
        12,
        11,
        16,
        10
      },
      new int[4]
      {
        14,
        13,
        16,
        12
      }
    };
    private const int MODE_NUMBER = 1;
    private const int MODE_ROMAN_AND_NUMBER = 2;
    private const int MODE_8BIT_BYTE = 4;
    private const int MODE_KANJI = 8;
    internal int[] blocks;
    internal int dataLengthMode;
    internal int blockPointer;
    internal int bitPointer;
    internal int dataLength;
    internal int numErrorCorrectionCode;
    internal DebugCanvas canvas;

    internal virtual int NextMode
    {
      get
      {
        if (this.blockPointer > this.blocks.Length - this.numErrorCorrectionCode - 2)
          return 0;
        return this.getNextBits(4);
      }
    }

    public virtual sbyte[] DataByte
    {
      get
      {
        this.canvas.println("Reading data blocks.");
        MemoryStream memoryStream = new MemoryStream();
        try
        {
          int nextMode;
          while (true)
          {
            nextMode = this.NextMode;
            int num;
            switch (nextMode)
            {
              case 0:
                goto label_3;
              case 1:
              case 2:
              case 4:
                num = 1;
                break;
              default:
                num = nextMode == 8 ? 1 : 0;
                break;
            }
            if (num != 0)
            {
              this.dataLength = this.getDataLength(nextMode);
              if (this.dataLength >= 1)
              {
                switch (nextMode)
                {
                  case 1:
                    sbyte[] sbyteArray1 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getFigureString(this.dataLength)));
                    memoryStream.Write(SystemUtils.ToByteArray(sbyteArray1), 0, sbyteArray1.Length);
                    break;
                  case 2:
                    sbyte[] sbyteArray2 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getRomanAndFigureString(this.dataLength)));
                    memoryStream.Write(SystemUtils.ToByteArray(sbyteArray2), 0, sbyteArray2.Length);
                    break;
                  case 4:
                    sbyte[] sbyteArray3 = this.get8bitByteArray(this.dataLength);
                    memoryStream.Write(SystemUtils.ToByteArray(sbyteArray3), 0, sbyteArray3.Length);
                    break;
                  case 8:
                    sbyte[] sbyteArray4 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getKanjiString(this.dataLength)));
                    memoryStream.Write(SystemUtils.ToByteArray(sbyteArray4), 0, sbyteArray4.Length);
                    break;
                }
              }
              else
                goto label_10;
            }
            else
              goto label_8;
          }
label_3:
          if (memoryStream.Length <= 0L)
            throw new InvalidDataBlockException("Empty data block");
          goto label_19;
label_8:
          throw new InvalidDataBlockException("Invalid mode: " + (object) nextMode + " in (block:" + (string) (object) this.blockPointer + " bit:" + (string) (object) this.bitPointer + ")");
label_10:
          throw new InvalidDataBlockException("Invalid data length: " + (object) this.dataLength);
        }
        catch (IndexOutOfRangeException ex)
        {
          SystemUtils.WriteStackTrace((Exception) ex, Console.Error);
          throw new InvalidDataBlockException("Data Block Error in (block:" + (object) this.blockPointer + " bit:" + (string) (object) this.bitPointer + ")");
        }
        catch (IOException ex)
        {
          throw new InvalidDataBlockException(ex.Message);
        }
label_19:
        return SystemUtils.ToSByteArray(memoryStream.ToArray());
      }
    }

    public virtual string DataString
    {
      get
      {
        this.canvas.println("Reading data blocks...");
        string str = "";
        while (true)
        {
          int nextMode = this.NextMode;
          this.canvas.println("mode: " + (object) nextMode);
          if (nextMode != 0)
          {
            if (nextMode == 1 || nextMode == 2 || nextMode == 4 || nextMode == 8)
              ;
            this.dataLength = this.getDataLength(nextMode);
            this.canvas.println(Convert.ToString(this.blocks[this.blockPointer]));
            Console.Out.WriteLine("length: " + (object) this.dataLength);
            switch (nextMode)
            {
              case 1:
                str += this.getFigureString(this.dataLength);
                break;
              case 2:
                str += this.getRomanAndFigureString(this.dataLength);
                break;
              case 4:
                str += this.get8bitByteString(this.dataLength);
                break;
              case 8:
                str += this.getKanjiString(this.dataLength);
                break;
            }
          }
          else
            break;
        }
        Console.Out.WriteLine("");
        return str;
      }
    }

    public QRCodeDataBlockReader(int[] blocks, int version, int numErrorCorrectionCode)
    {
      this.blockPointer = 0;
      this.bitPointer = 7;
      this.dataLength = 0;
      this.blocks = blocks;
      this.numErrorCorrectionCode = numErrorCorrectionCode;
      if (version <= 9)
        this.dataLengthMode = 0;
      else if (version >= 10 && version <= 26)
        this.dataLengthMode = 1;
      else if (version >= 27 && version <= 40)
        this.dataLengthMode = 2;
      this.canvas = QRCodeDecoder.Canvas;
    }

    internal virtual int getNextBits(int numBits)
    {
      if (numBits < this.bitPointer + 1)
      {
        int num1 = 0;
        for (int index = 0; index < numBits; ++index)
          num1 += 1 << index;
        int num2 = (this.blocks[this.blockPointer] & num1 << this.bitPointer - numBits + 1) >> this.bitPointer - numBits + 1;
        this.bitPointer -= numBits;
        return num2;
      }
      if (numBits < this.bitPointer + 1 + 8)
      {
        int num1 = 0;
        for (int index = 0; index < this.bitPointer + 1; ++index)
          num1 += 1 << index;
        int num2 = (this.blocks[this.blockPointer] & num1) << numBits - (this.bitPointer + 1);
        ++this.blockPointer;
        int num3 = num2 + (this.blocks[this.blockPointer] >> 8 - (numBits - (this.bitPointer + 1)));
        this.bitPointer = this.bitPointer - numBits % 8;
        if (this.bitPointer < 0)
          this.bitPointer = 8 + this.bitPointer;
        return num3;
      }
      if (numBits < this.bitPointer + 1 + 16)
      {
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < this.bitPointer + 1; ++index)
          num1 += 1 << index;
        int num3 = (this.blocks[this.blockPointer] & num1) << numBits - (this.bitPointer + 1);
        ++this.blockPointer;
        int num4 = this.blocks[this.blockPointer] << numBits - (this.bitPointer + 1 + 8);
        ++this.blockPointer;
        for (int index = 0; index < numBits - (this.bitPointer + 1 + 8); ++index)
          num2 += 1 << index;
        int num5 = (this.blocks[this.blockPointer] & num2 << 8 - (numBits - (this.bitPointer + 1 + 8))) >> 8 - (numBits - (this.bitPointer + 1 + 8));
        int num6 = num3 + num4 + num5;
        this.bitPointer = this.bitPointer - (numBits - 8) % 8;
        if (this.bitPointer < 0)
          this.bitPointer = 8 + this.bitPointer;
        return num6;
      }
      Console.Out.WriteLine("ERROR!");
      return 0;
    }

    internal virtual int guessMode(int mode)
    {
      switch (mode)
      {
        case 3:
          return 1;
        case 5:
          return 4;
        case 6:
          return 4;
        case 7:
          return 4;
        case 9:
          return 8;
        case 10:
          return 8;
        case 11:
          return 8;
        case 12:
          return 4;
        case 13:
          return 4;
        case 14:
          return 4;
        case 15:
          return 4;
        default:
          return 8;
      }
    }

    internal virtual int getDataLength(int modeIndicator)
    {
      int index = 0;
      while (true)
      {
        if (modeIndicator >> index != 1)
          ++index;
        else
          break;
      }
      return this.getNextBits(this.sizeOfDataLengthInfo[this.dataLengthMode][index]);
    }

    internal virtual string getFigureString(int dataLength)
    {
      int num1 = dataLength;
      int num2 = 0;
      string str = "";
      do
      {
        if (num1 >= 3)
        {
          num2 = this.getNextBits(10);
          if (num2 < 100)
            str += "0";
          if (num2 < 10)
            str += "0";
          num1 -= 3;
        }
        else if (num1 == 2)
        {
          num2 = this.getNextBits(7);
          if (num2 < 10)
            str += "0";
          num1 -= 2;
        }
        else if (num1 == 1)
        {
          num2 = this.getNextBits(4);
          --num1;
        }
        str += Convert.ToString(num2);
      }
      while (num1 > 0);
      return str;
    }

    internal virtual string getRomanAndFigureString(int dataLength)
    {
      int num = dataLength;
      string str = "";
      char[] chArray = new char[45]
      {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        ' ',
        '$',
        '%',
        '*',
        '+',
        '-',
        '.',
        '/',
        ':'
      };
      do
      {
        if (num > 1)
        {
          int nextBits = this.getNextBits(11);
          int index1 = nextBits / 45;
          int index2 = nextBits % 45;
          str = str + Convert.ToString(chArray[index1]) + Convert.ToString(chArray[index2]);
          num -= 2;
        }
        else if (num == 1)
        {
          int nextBits = this.getNextBits(6);
          str += Convert.ToString(chArray[nextBits]);
          --num;
        }
      }
      while (num > 0);
      return str;
    }

    public virtual sbyte[] get8bitByteArray(int dataLength)
    {
      int num = dataLength;
      MemoryStream memoryStream = new MemoryStream();
      do
      {
        this.canvas.println("Length: " + (object) num);
        int nextBits = this.getNextBits(8);
        memoryStream.WriteByte((byte) nextBits);
        --num;
      }
      while (num > 0);
      return SystemUtils.ToSByteArray(memoryStream.ToArray());
    }

    internal virtual string get8bitByteString(int dataLength)
    {
      int num = dataLength;
      string str = "";
      do
      {
        int nextBits = this.getNextBits(8);
        str += (string) (object) (char) nextBits;
        --num;
      }
      while (num > 0);
      return str;
    }

    internal virtual string getKanjiString(int dataLength)
    {
      int num1 = dataLength;
      string str = "";
      do
      {
        int nextBits = this.getNextBits(13);
        int num2 = nextBits % 192;
        int num3 = (nextBits / 192 << 8) + num2;
        int num4 = num3 + 33088 > 40956 ? num3 + 49472 : num3 + 33088;
        sbyte[] sbyteArray = new sbyte[2]
        {
          (sbyte) (num4 >> 8),
          (sbyte) (num4 & (int) byte.MaxValue)
        };
        str += new string(SystemUtils.ToCharArray(SystemUtils.ToByteArray(sbyteArray)));
        --num1;
      }
      while (num1 > 0);
      return str;
    }
  }
}
