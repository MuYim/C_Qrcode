// Decompiled with JetBrains decompiler
// Type: ThoughtWorks.QRCode.Codec.Ecc.ReedSolomon
// Assembly: ThoughtWorks.QRCode, Version=1.0.4778.30637, Culture=neutral, PublicKeyToken=null
// MVID: 2CFC48F2-5A3C-423F-9022-029D4BB7C8B6
// Assembly location: 

namespace ThoughtWorks.QRCode.Codec.Ecc
{
  public class ReedSolomon
  {
    internal int[] gexp = new int[512];
    internal int[] glog = new int[256];
    internal int[] ErrorLocs = new int[256];
    internal int[] ErasureLocs = new int[256];
    internal int NErasures = 0;
    internal bool correctionSucceeded = true;
    internal int[] y;
    internal int NPAR;
    internal int MAXDEG;
    internal int[] synBytes;
    internal int[] Lambda;
    internal int[] Omega;
    internal int NErrors;

    public virtual bool CorrectionSucceeded
    {
      get
      {
        return this.correctionSucceeded;
      }
    }

    public virtual int NumCorrectedErrors
    {
      get
      {
        return this.NErrors;
      }
    }

    public ReedSolomon(int[] source, int NPAR)
    {
      this.initializeGaloisTables();
      this.y = source;
      this.NPAR = NPAR;
      this.MAXDEG = NPAR * 2;
      this.synBytes = new int[this.MAXDEG];
      this.Lambda = new int[this.MAXDEG];
      this.Omega = new int[this.MAXDEG];
    }

    internal virtual void initializeGaloisTables()
    {
      int num1;
      int num2 = num1 = 0;
      int num3 = num1;
      int num4 = num1;
      int num5 = num1;
      int num6 = num1;
      int num7 = num1;
      int num8 = num1;
      int num9 = 1;
      this.gexp[0] = 1;
      this.gexp[(int) byte.MaxValue] = this.gexp[0];
      this.glog[0] = 0;
      for (int index = 1; index < 256; ++index)
      {
        int num10 = num2;
        num2 = num3;
        num3 = num4;
        num4 = num5;
        num5 = num6 ^ num10;
        num6 = num7 ^ num10;
        num7 = num8 ^ num10;
        num8 = num9;
        num9 = num10;
        this.gexp[index] = num9 + num8 * 2 + num7 * 4 + num6 * 8 + num5 * 16 + num4 * 32 + num3 * 64 + num2 * 128;
        this.gexp[index + (int) byte.MaxValue] = this.gexp[index];
      }
      for (int index1 = 1; index1 < 256; ++index1)
      {
        for (int index2 = 0; index2 < 256; ++index2)
        {
          if (this.gexp[index2] == index1)
          {
            this.glog[index1] = index2;
            break;
          }
        }
      }
    }

    internal virtual int gmult(int a, int b)
    {
      if (a == 0 || b == 0)
        return 0;
      return this.gexp[this.glog[a] + this.glog[b]];
    }

    internal virtual int ginv(int elt)
    {
      return this.gexp[(int) byte.MaxValue - this.glog[elt]];
    }

    internal virtual void decode_data(int[] data)
    {
      for (int index1 = 0; index1 < this.MAXDEG; ++index1)
      {
        int b = 0;
        for (int index2 = 0; index2 < data.Length; ++index2)
          b = data[index2] ^ this.gmult(this.gexp[index1 + 1], b);
        this.synBytes[index1] = b;
      }
    }

    public virtual void correct()
    {
      this.decode_data(this.y);
      this.correctionSucceeded = true;
      bool flag = false;
      for (int index = 0; index < this.synBytes.Length; ++index)
      {
        if (this.synBytes[index] != 0)
          flag = true;
      }
      if (!flag)
        return;
      this.correctionSucceeded = this.correct_errors_erasures(this.y, this.y.Length, 0, new int[1]);
    }

    internal virtual void Modified_Berlekamp_Massey()
    {
      int[] numArray1 = new int[this.MAXDEG];
      int[] numArray2 = new int[this.MAXDEG];
      int[] numArray3 = new int[this.MAXDEG];
      int[] numArray4 = new int[this.MAXDEG];
      this.init_gamma(numArray4);
      this.copy_poly(numArray3, numArray4);
      this.mul_z_poly(numArray3);
      this.copy_poly(numArray1, numArray4);
      int num1 = -1;
      int L = this.NErasures;
      for (int n = this.NErasures; n < 8; ++n)
      {
        int discrepancy = this.compute_discrepancy(numArray1, this.synBytes, L, n);
        if (discrepancy != 0)
        {
          for (int index = 0; index < this.MAXDEG; ++index)
            numArray2[index] = numArray1[index] ^ this.gmult(discrepancy, numArray3[index]);
          if (L < n - num1)
          {
            int num2 = n - num1;
            num1 = n - L;
            for (int index = 0; index < this.MAXDEG; ++index)
              numArray3[index] = this.gmult(numArray1[index], this.ginv(discrepancy));
            L = num2;
          }
          for (int index = 0; index < this.MAXDEG; ++index)
            numArray1[index] = numArray2[index];
        }
        this.mul_z_poly(numArray3);
      }
      for (int index = 0; index < this.MAXDEG; ++index)
        this.Lambda[index] = numArray1[index];
      this.compute_modified_omega();
    }

    internal virtual void compute_modified_omega()
    {
      int[] dst = new int[this.MAXDEG * 2];
      this.mult_polys(dst, this.Lambda, this.synBytes);
      this.zero_poly(this.Omega);
      for (int index = 0; index < this.NPAR; ++index)
        this.Omega[index] = dst[index];
    }

    internal virtual void mult_polys(int[] dst, int[] p1, int[] p2)
    {
      int[] numArray = new int[this.MAXDEG * 2];
      for (int index = 0; index < this.MAXDEG * 2; ++index)
        dst[index] = 0;
      for (int index1 = 0; index1 < this.MAXDEG; ++index1)
      {
        for (int index2 = this.MAXDEG; index2 < this.MAXDEG * 2; ++index2)
          numArray[index2] = 0;
        for (int index2 = 0; index2 < this.MAXDEG; ++index2)
          numArray[index2] = this.gmult(p2[index2], p1[index1]);
        for (int index2 = this.MAXDEG * 2 - 1; index2 >= index1; --index2)
          numArray[index2] = numArray[index2 - index1];
        for (int index2 = 0; index2 < index1; ++index2)
          numArray[index2] = 0;
        for (int index2 = 0; index2 < this.MAXDEG * 2; ++index2)
          dst[index2] ^= numArray[index2];
      }
    }

    internal virtual void init_gamma(int[] gamma)
    {
      int[] numArray = new int[this.MAXDEG];
      this.zero_poly(gamma);
      this.zero_poly(numArray);
      gamma[0] = 1;
      for (int index = 0; index < this.NErasures; ++index)
      {
        this.copy_poly(numArray, gamma);
        this.scale_poly(this.gexp[this.ErasureLocs[index]], numArray);
        this.mul_z_poly(numArray);
        this.add_polys(gamma, numArray);
      }
    }

    internal virtual void compute_next_omega(int d, int[] A, int[] dst, int[] src)
    {
      for (int index = 0; index < this.MAXDEG; ++index)
        dst[index] = src[index] ^ this.gmult(d, A[index]);
    }

    internal virtual int compute_discrepancy(int[] lambda, int[] S, int L, int n)
    {
      int num = 0;
      for (int index = 0; index <= L; ++index)
        num ^= this.gmult(lambda[index], S[n - index]);
      return num;
    }

    internal virtual void add_polys(int[] dst, int[] src)
    {
      for (int index = 0; index < this.MAXDEG; ++index)
        dst[index] ^= src[index];
    }

    internal virtual void copy_poly(int[] dst, int[] src)
    {
      for (int index = 0; index < this.MAXDEG; ++index)
        dst[index] = src[index];
    }

    internal virtual void scale_poly(int k, int[] poly)
    {
      for (int index = 0; index < this.MAXDEG; ++index)
        poly[index] = this.gmult(k, poly[index]);
    }

    internal virtual void zero_poly(int[] poly)
    {
      for (int index = 0; index < this.MAXDEG; ++index)
        poly[index] = 0;
    }

    internal virtual void mul_z_poly(int[] src)
    {
      for (int index = this.MAXDEG - 1; index > 0; --index)
        src[index] = src[index - 1];
      src[0] = 0;
    }

    internal virtual void Find_Roots()
    {
      this.NErrors = 0;
      for (int index1 = 1; index1 < 256; ++index1)
      {
        int num = 0;
        for (int index2 = 0; index2 < this.NPAR + 1; ++index2)
          num ^= this.gmult(this.gexp[index2 * index1 % (int) byte.MaxValue], this.Lambda[index2]);
        if (num == 0)
        {
          this.ErrorLocs[this.NErrors] = (int) byte.MaxValue - index1;
          ++this.NErrors;
        }
      }
    }

    internal virtual bool correct_errors_erasures(int[] codeword, int csize, int nerasures, int[] erasures)
    {
      this.NErasures = nerasures;
      for (int index = 0; index < this.NErasures; ++index)
        this.ErasureLocs[index] = erasures[index];
      this.Modified_Berlekamp_Massey();
      this.Find_Roots();
      if (this.NErrors > this.NPAR && this.NErrors <= 0)
        return false;
      for (int index = 0; index < this.NErrors; ++index)
      {
        if (this.ErrorLocs[index] >= csize)
          return false;
      }
      for (int index1 = 0; index1 < this.NErrors; ++index1)
      {
        int num1 = this.ErrorLocs[index1];
        int a = 0;
        for (int index2 = 0; index2 < this.MAXDEG; ++index2)
          a ^= this.gmult(this.Omega[index2], this.gexp[((int) byte.MaxValue - num1) * index2 % (int) byte.MaxValue]);
        int elt = 0;
        int index3 = 1;
        while (index3 < this.MAXDEG)
        {
          elt ^= this.gmult(this.Lambda[index3], this.gexp[((int) byte.MaxValue - num1) * (index3 - 1) % (int) byte.MaxValue]);
          index3 += 2;
        }
        int num2 = this.gmult(a, this.ginv(elt));
        codeword[csize - num1 - 1] ^= num2;
      }
      return true;
    }
  }
}
