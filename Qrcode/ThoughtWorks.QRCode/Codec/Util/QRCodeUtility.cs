// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Util.QRCodeUtility
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

using System.Text;

namespace ThoughtWorks.QRCode.Codec.Util
{
  public class QRCodeUtility
  {
    public static int sqrt(int val)
    {
      int num1 = 0;
      int num2 = 32768;
      int num3 = 15;
      do
      {
        int num4 = val;
        int num5 = (num1 << 1) + num2;
        int num6 = num3-- & 31;
        int num7;
        int num8 = num7 = num5 << num6;
        if (num4 >= num7)
        {
          num1 += num2;
          val -= num8;
        }
      }
      while ((num2 >>= 1) > 0);
      return num1;
    }

    public static bool IsUniCode(string value)
    {
      return QRCodeUtility.FromASCIIByteArray(QRCodeUtility.AsciiStringToByteArray(value)) != QRCodeUtility.FromUnicodeByteArray(QRCodeUtility.UnicodeStringToByteArray(value));
    }

    public static bool IsUnicode(byte[] byteData)
    {
      return (int) QRCodeUtility.AsciiStringToByteArray(QRCodeUtility.FromASCIIByteArray(byteData))[0] != (int) QRCodeUtility.UnicodeStringToByteArray(QRCodeUtility.FromUnicodeByteArray(byteData))[0];
    }

    public static string FromASCIIByteArray(byte[] characters)
    {
      return new ASCIIEncoding().GetString(characters);
    }

    public static string FromUnicodeByteArray(byte[] characters)
    {
      return Encoding.GetEncoding("gb2312").GetString(characters);
    }

    public static byte[] AsciiStringToByteArray(string str)
    {
      return new ASCIIEncoding().GetBytes(str);
    }

    public static byte[] UnicodeStringToByteArray(string str)
    {
      return Encoding.GetEncoding("gb2312").GetBytes(str);
    }
  }
}
