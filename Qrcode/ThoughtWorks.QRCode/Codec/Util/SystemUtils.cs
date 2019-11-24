// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Util.SystemUtils
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System;
using System.IO;
using System.Text;

namespace ThoughtWorks.QRCode.Codec.Util
{
  public class SystemUtils
  {
    public static int ReadInput(Stream sourceStream, sbyte[] target, int start, int count)
    {
      if (target.Length == 0)
        return 0;
      byte[] buffer = new byte[target.Length];
      int num = sourceStream.Read(buffer, start, count);
      if (num == 0)
        return -1;
      for (int index = start; index < start + num; ++index)
        target[index] = (sbyte) buffer[index];
      return num;
    }

    public static int ReadInput(TextReader sourceTextReader, short[] target, int start, int count)
    {
      if (target.Length == 0)
        return 0;
      char[] buffer = new char[target.Length];
      int num = sourceTextReader.Read(buffer, start, count);
      if (num == 0)
        return -1;
      for (int index = start; index < start + num; ++index)
        target[index] = (short) buffer[index];
      return num;
    }

    public static void WriteStackTrace(Exception throwable, TextWriter stream)
    {
      stream.Write(throwable.StackTrace);
      stream.Flush();
    }

    public static int URShift(int number, int bits)
    {
      if (number >= 0)
        return number >> bits;
      return (number >> bits) + (2 << ~bits);
    }

    public static int URShift(int number, long bits)
    {
      return SystemUtils.URShift(number, (int) bits);
    }

    public static long URShift(long number, int bits)
    {
      if (number >= 0L)
        return number >> bits;
      return (number >> bits) + (2L << ~bits);
    }

    public static long URShift(long number, long bits)
    {
      return SystemUtils.URShift(number, (int) bits);
    }

    public static byte[] ToByteArray(sbyte[] sbyteArray)
    {
      byte[] numArray = (byte[]) null;
      if (sbyteArray != null)
      {
        numArray = new byte[sbyteArray.Length];
        for (int index = 0; index < sbyteArray.Length; ++index)
          numArray[index] = (byte) sbyteArray[index];
      }
      return numArray;
    }

    public static byte[] ToByteArray(string sourceString)
    {
      return Encoding.UTF8.GetBytes(sourceString);
    }

    public static byte[] ToByteArray(object[] tempObjectArray)
    {
      byte[] numArray = (byte[]) null;
      if (tempObjectArray != null)
      {
        numArray = new byte[tempObjectArray.Length];
        for (int index = 0; index < tempObjectArray.Length; ++index)
          numArray[index] = (byte) tempObjectArray[index];
      }
      return numArray;
    }

    public static sbyte[] ToSByteArray(byte[] byteArray)
    {
      sbyte[] numArray = (sbyte[]) null;
      if (byteArray != null)
      {
        numArray = new sbyte[byteArray.Length];
        for (int index = 0; index < byteArray.Length; ++index)
          numArray[index] = (sbyte) byteArray[index];
      }
      return numArray;
    }

    public static char[] ToCharArray(sbyte[] sByteArray)
    {
      return Encoding.UTF8.GetChars(SystemUtils.ToByteArray(sByteArray));
    }

    public static char[] ToCharArray(byte[] byteArray)
    {
      return Encoding.UTF8.GetChars(byteArray);
    }
  }
}
