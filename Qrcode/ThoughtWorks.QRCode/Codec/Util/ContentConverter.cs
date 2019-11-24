// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Util.ContentConverter
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

namespace ThoughtWorks.QRCode.Codec.Util
{
  public class ContentConverter
  {
    internal static char n = '\n';

    public static string convert(string targetString)
    {
      if (targetString == null)
        return targetString;
      if (targetString.IndexOf("MEBKM:") > -1)
        targetString = ContentConverter.convertDocomoBookmark(targetString);
      if (targetString.IndexOf("MECARD:") > -1)
        targetString = ContentConverter.convertDocomoAddressBook(targetString);
      if (targetString.IndexOf("MATMSG:") > -1)
        targetString = ContentConverter.convertDocomoMailto(targetString);
      if (targetString.IndexOf("http\\://") > -1)
        targetString = ContentConverter.replaceString(targetString, "http\\://", "\nhttp://");
      return targetString;
    }

    private static string convertDocomoBookmark(string targetString)
    {
      targetString = ContentConverter.removeString(targetString, "MEBKM:");
      targetString = ContentConverter.removeString(targetString, "TITLE:");
      targetString = ContentConverter.removeString(targetString, ";");
      targetString = ContentConverter.removeString(targetString, "URL:");
      return targetString;
    }

    private static string convertDocomoAddressBook(string targetString)
    {
      targetString = ContentConverter.removeString(targetString, "MECARD:");
      targetString = ContentConverter.removeString(targetString, ";");
      targetString = ContentConverter.replaceString(targetString, "N:", "NAME1:");
      targetString = ContentConverter.replaceString(targetString, "SOUND:", (string) (object) ContentConverter.n + (object) "NAME2:");
      targetString = ContentConverter.replaceString(targetString, "TEL:", (string) (object) ContentConverter.n + (object) "TEL1:");
      targetString = ContentConverter.replaceString(targetString, "EMAIL:", (string) (object) ContentConverter.n + (object) "MAIL1:");
      targetString += (string) (object) ContentConverter.n;
      return targetString;
    }

    private static string convertDocomoMailto(string s)
    {
      string s1 = s;
      char ch = '\n';
      return ContentConverter.replaceString(ContentConverter.replaceString(ContentConverter.replaceString(ContentConverter.removeString(ContentConverter.removeString(s1, "MATMSG:"), ";"), "TO:", "MAILTO:"), "SUB:", (string) (object) ch + (object) "SUBJECT:"), "BODY:", (string) (object) ch + (object) "BODY:") + (object) ch;
    }

    private static string replaceString(string s, string s1, string s2)
    {
      string str = s;
      for (int length = str.IndexOf(s1, 0); length > -1; length = str.IndexOf(s1, length + s2.Length))
        str = str.Substring(0, length) + s2 + str.Substring(length + s1.Length);
      return str;
    }

    private static string removeString(string s, string s1)
    {
      return ContentConverter.replaceString(s, s1, "");
    }
  }
}
