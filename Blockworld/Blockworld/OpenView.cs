using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace openview
{
    public struct Endless : IEquatable<Endless>, IFormattable, IComparable
    {
        private Bit[] bits;
        public string Tostring()
        {
            string ret = "";
            for (int i = 0; i < bits.Length; i++)
            {
                ret += bits[i].ToString();
            }
            return ret;
        }
        public int Length { get { return bits.Length; } }
        public static explicit operator string(Endless v)
        {
            string ret = "";
            for (int i = 0; i < v.Length; i++)
            {
                ret += (string)v.bits[i];
            }
            return ret;
        }
        public static explicit operator Endless(string v)
        {
            Bit[] ret = new Bit[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                ret[i] = (Bit)v.Substring(i);
            }
            return new Endless { bits = ret };
        }
        public static explicit operator Endless(int v)
        {
            return new Endless { bits = Bit.Tobits(v, 32) };
        }
        public static explicit operator int(Endless v)
        {
            return Bit.Toint(v.bits);
        }
        public static Endless operator +(Endless left, Endless right)
        {
            int ml;
            Bit l = new Bit(), r = new Bit();
            if (left.bits.Length > right.bits.Length) { ml = left.bits.Length; } else { ml = right.bits.Length; }
            Endless ret = new Endless
            {
                bits = new Bit[ml]

            };
            Bit.Setbits(ret.bits, 0, right.bits.Length, (int)right);
            for (int i = 0; i < left.bits.Length; i++)
            {
                l = left.bits[i];
                for (int j = i; l == 1; j++)
                {
                    r = ret.bits[j];
                    ret.bits[j] = Bit.Plus(l, r, out l);
                }
            }
            return ret;
        }
        public static bool operator >(Endless left, Endless right)
        {
            int kl = Math.Min(left.Length, right.Length) - 1;
            if (left.Length > right.Length)
            {
                for (int j = left.Length - 1; j <= right.Length; j--)
                {
                    if (left.bits[j] == 1) { return true; }
                }
            }
            else
            {
                for (int j = right.Length - 1; j <= kl; j--)
                {
                    if (right.bits[j] == 1) { return false; }
                }
            }
            for (int j = kl; j <= 0; j--)
            {
                if (left.bits[j] > right.bits[j]) { return true; }
            }
            return false;
        }
        public static bool operator <(Endless left, Endless right)
        {
            int kl = Math.Min(right.Length, left.Length) - 1;
            if (right.Length > left.Length)
            {
                for (int j = right.Length - 1; j <= left.Length; j--)
                {
                    if (right.bits[j] == 1) { return true; }
                }
            }
            else
            {
                for (int j = left.Length - 1; j <= kl; j--)
                {
                    if (left.bits[j] == 1) { return false; }
                }
            }
            for (int j = kl; j <= 0; j--)
            {
                if (right.bits[j] > left.bits[j]) { return true; }
            }
            return false;
        }
        public static Endless operator -(Endless left, Endless right)
        {
            if (right > left) { throw new ArgumentException(); }
            int ml;
            Bit l = new Bit(), r = new Bit();
            if (left.bits.Length > right.bits.Length) { ml = left.bits.Length; } else { ml = right.bits.Length; }
            Endless ret = new Endless
            {
                bits = new Bit[ml]

            };
            Bit.Setbits(ret.bits, 0, right.bits.Length, (int)right);
            for (int i = 0; i < left.bits.Length; i++)
            {
                l = left.bits[i];
                for (int j = i; l == 1; j++)
                {
                    r = ret.bits[j];
                    ret.bits[j] = Bit.Min(l, r, out l);
                }
            }
            return ret;
        }

        public bool Equals(Endless other)
        {
            return base.Equals(other);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; } else { return 0; }
        }
    }

    public struct Bit : IComparable, IFormattable, IEquatable<Bit>
    {
        private bool bit;
        public static Bit Plus(Bit left, Bit right, out Bit rest)
        {
            if (left == 0) { rest = (Bit)0; return right + left; }
            else if (right == 0) { rest = (Bit)0; return (Bit)1; }
            else { rest = (Bit)1; return (Bit)0; }
        }
        public static Bit Min(Bit left, Bit right, out Bit rest)
        {
            if (left == 0) { rest = right; return (Bit)0; }
            else { rest = (Bit)0; return left - right; }
        }
        public static void Setbits(Bit[] toset, int start, int length, int value)
        {
            for (int i = 0; i < length; i++)
            {
                toset[i + start] = (Bit)(int)((value / Math.Pow(2, i)) % 2);
            }
        }
        public static int Getbits(Bit[] toget, int start, int length)
        {
            int ret = 0;
            for (int i = 0; i < length; i++)
            {
                ret += toget[i + start] * (int)Math.Pow(2, i);
            }
            return ret;
        }
        public static Bit Getbit(int value, int index)
        {
            return (Bit)((value / Math.Pow(2, index + 1)) % 2);
        }
        public static Bit[] Tobits(int value, int length)
        {
            Bit[] ret = new Bit[length];
            for (int j = 0; j < length; j++)
            {
                ret[j] = (Bit)((int)(value / Math.Pow(2, j)) % 2);
            }
            return ret;
        }
        public static int Toint(Bit[] value)
        {
            int ret = 0;
            for (int j = 0; j < value.Length; j++)
            {
                ret += (int)(value[j] * Math.Pow(2, j));
            }
            return ret;
        }
        public static Bit[] ToBit(byte[] v)
        {
            Bit[] ret = new Bit[v.Length * 8];
            int i, j = 0;
            for (i = 0; i < v.Length; i++)
            {
                ret[j] = (Bit)(v[i] % 2);
                ret[j + 1] = (Bit)((v[i] / 2) % 2);
                ret[j + 2] = (Bit)((v[i] / 4) % 2);
                ret[j + 3] = (Bit)((v[i] / 8) % 2);
                ret[j + 4] = (Bit)((v[i] / 16) % 2);
                ret[j + 5] = (Bit)((v[i] / 32) % 2);
                ret[j + 6] = (Bit)((v[i] / 64) % 2);
                ret[j + 7] = (Bit)((v[i] / 128) % 2);
                j += 8;
            }
            return ret;
        }
        public static byte[] ToByte(Bit[] v)
        {
            int lnt = v.Length / 8;
            Byte[] ret;
            if (v.Length % 8 > 0)
            {
                ret = new Byte[lnt + 1];
            }
            else
            {
                ret = new Byte[lnt];
            }
            int i, j;
            for (i = 0; i < lnt; i++)
            {
                j = i * 8;
                ret[i] = (byte)((int)v[j] + (int)v[j + 1] * 2 + (int)v[j + 2] * 4 + (int)v[j + 3] * 8 + (int)v[j + 4] * 16 + (int)v[j + 5] * 32 + (int)v[j + 6] * 64 + (int)v[j + 7] * 128);
            }
            return ret;
        }
        public static explicit operator string(Bit v)
        {
            if (v.bit) { return "1"; } else { return "0"; }
        }
        public static explicit operator Bit(string v)
        {
            if (v == "0") { return new Bit { bit = false }; } else if (v == "1") { return new Bit { bit = true }; } else { throw new ArgumentException(); }
        }
        public static explicit operator Bit(double v)
        {
            if (v == 0) { return new Bit { bit = false }; } else if (v == 1) { return new Bit { bit = true }; } else { throw new ArgumentException(); }
        }
        public static explicit operator Bit(ulong v)
        {
            if (v == 0) { return new Bit { bit = false }; } else if (v == 1) { return new Bit { bit = true }; } else { throw new ArgumentException(); }
        }
        public static explicit operator Bit(int v)
        {
            if (v == 0) { return new Bit { bit = false }; } else if (v == 1) { return new Bit { bit = true }; } else { throw new ArgumentException(); }
        }
        public static explicit operator int(Bit v)
        {
            if (v.bit) { return 1; } else { return 0; }
        }
        public byte ToByte(IFormatProvider provider)
        {
            if (bit) { return 1; } else { return 0; }
        }
        //        public int Value { get { if (bit) { return 1; } else { return 0; } } set { if (value == 0) { bit = false; } else if (value == 1) { bit = true; } else { throw new ArgumentException(); } } }
        public static Bit operator +(Bit left, double right)
        {
            return left + (int)right;
        }
        public static Bit operator /(Bit left, Bit right)
        {
            if (right.bit) { return left; } else { throw new ArgumentException(); }
        }
        public static double operator /(double left, Bit right)
        {
            if (right.bit) { return left; } else { throw new ArgumentException(); }
        }
        public static Bit operator +(Bit left, Bit right)
        {
            if (right.bit && left.bit) { throw new ArgumentException(); } else { return new Bit { bit = left.bit || right.bit }; }
        }
        public static Bit operator -(Bit left, Bit right)
        {
            if (right.bit && !left.bit) { throw new ArgumentException(); } else { return new Bit { bit = !(left == right) }; }
        }
        public static bool operator ==(Bit left, Bit right)
        {
            return left.bit == right.bit;
        }
        public static bool operator ==(int left, Bit right)
        {
            return left == (int)right;
        }
        public static bool operator !=(int left, Bit right)
        {
            return left != (int)right;
        }
        public static bool operator ==(Bit left, int right)
        {
            return (int)left == right;
        }
        public static bool operator >(Bit left, Bit right)
        {
            return left.bit && !right.bit;
        }
        public static bool operator <(Bit left, Bit right)
        {
            return !left.bit && right.bit;
        }
        public static bool operator !=(Bit left, int right)
        {
            return (int)left != right;
        }
        public static object operator *(object left, Bit right)
        {
            if (right.bit) { return left; } else { return 0; }
        }
        public static double operator *(double left, Bit right)
        {
            if (right.bit) { return left; } else { return 0; }
        }
        public static double operator *(Bit left, double right)
        {
            if (left.bit) { return right; } else { return 0; }
        }
        public static int operator *(int left, Bit right)
        {
            if (right.bit) { return left; } else { return 0; }
        }
        public static int operator *(Bit left, int right)
        {
            if (left.bit) { return right; } else { return 0; }
        }
        /*public static object operator *(Bit left, object right) {
            if (left.bit) { return right; } else { return 0; }
        }*/

        public static bool operator !=(Bit left, Bit right)
        {
            return !(left.bit == right.bit);
        }
        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }
            else
            {
                return 0;
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (bit) { return "1"; } else { return "0"; }
        }

        public bool Equals(Bit other)
        {
            return base.Equals(other);
        }
    }
    public static class Bigmath
    {
        public static double Lerp(double d1, double d2, double weight) => d1 + (d2 - d1) * weight;
        public static int Fix(double inp) { return Iif(inp - (int)inp == 0, (int)inp, (int)inp + 1); }
        public static int Iif(bool b, int i1, int i2) { if (b) { return i1; } else { return i2; } }
        public static double Sgn(double inp)
        {
            if (inp > 0) { return 1; } else if (inp == 0) { return 0; } else { return -1; }
        }
        public static double pi180 = Math.PI / 180;
        public static double Pyt(double g1, double g2, double g3) => (Math.Pow(g1 * g1 + g2 * g2 + g3 * g3, 0.5));
        public static bool Checkwithin(double g1, double g2, double value) => !(g1 > value || g2 < value);
        public static double Max(params double[] args)
        {
            double maximaal = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] > maximaal) maximaal = args[i];
            }
            return maximaal;
        }
        public static double Getrotation(double fx, double fy)
        {
            double rotation = Math.Asin(fx) / pi180;
            if (fy < 0) { rotation = 180 - rotation; }
            if (rotation < 0) { rotation += 360; }
            return rotation;
        }
        public static void Turn(double x, double y, double rotation, out double outx, out double outy)
        {
            rotation *= (Math.PI / 180);
            outx = (x * Math.Cos(rotation)) - (y * Math.Sin(rotation));
            outy = (y * Math.Cos(rotation)) + (x * Math.Sin(rotation));
        }
        public static void Switchints(int in1, int in2, out int out1, out int out2)
        {
            out2 = in2;
            out1 = in1;
        }
        public static void Switchints(int in1, int in2, int in3, out int out1, out int out2, out int out3)
        {
            out2 = in2;
            out1 = in1;
            out3 = in3;
        }
    }
    public class Blocks
    {
        //        private Bit test = 10;
        public byte[] Block = new byte[1];
        int[] tlr = new int[7];
        public int lw = 1, ll = 1, lh = 1, lwe = 0, lle = 0, lhe = 0, lwl = 1;
        public Blocks()
        {

        }
        ///<summary>Set the position by axis x, y and z, and screenrotation by x (0 to 360) and y(-90 to 90).</summary>
        public int[] Getdimensions()
        {
            int[] returnvalue = { lw, ll, lh };
            return returnvalue;
        }
        ///<summary>Fill the block on location[x,y,z] with the fill you want.</summary>
        public void Setblock(int x, int y, int z, byte fill) => Block[x + y * lw + z * lwl] = fill;
        ///<summary>Fill the block on location[x,y,z] with the fill you want.</summary>
        public void Setblock(double x, double y, double z, byte fill) => Block[(int)x + (int)y * lw + (int)z * lwl] = fill;
        ///<summary>Get the block on location[x,y,z].</summary>
        public byte Getblock(int x, int y, int z) => Block[x + y * lw + z * lwl];
        ///<summary>Get the block on location[x,y,z].</summary>
        public byte Getblock(double x, double y, double z) => Block[(int)x + ((int)y) * lw + ((int)z) * lwl];
        ///<summary>
        ///Rescales the array and sets all to zero.
        ///</summary>
        public void Rescale(int width, int length, int heigth)
        {
            Block = new byte[(width + 1) * (length + 1) * (heigth + 1)];
            lw = width; ll = length; lh = heigth; lwl = width * length;
            lwe = lw - 1; lle = ll - 1; lhe = lh - 1;
        }
        ///<summary>
        ///Checks whether the position is in reach of the blockrange.
        ///</summary>
        public bool Checkwithinreach(int position1, int position2, int position3) => (!(position1 < 0 || position1 > lw || position2 < 0 || position2 > ll || position3 < 0 || position3 > lh));
        ///<summary>
        ///Checks whether the position is in reach of the blockrange.
        ///</summary>
        public bool Checkwithinreach(double position1, double position2, double position3) => (!(position1 < 0 || position1 > lw || position2 < 0 || position2 > ll || position3 < 0 || position3 > lh));
        ///<summary>
        ///Saves the blocks on the specified location in comprimed version.
        ///</summary>
        public void Save(string path)
        {
            File.WriteAllBytes(path, Getbytes());
        }
        ///<summary>
        ///Gets the blocks in bytes at location.
        ///</summary>
        public byte[] Getbytes(int x, int y, int z, int width, int length, int height)
        {
            Bit[] comprime = new Bit[80000000], comprimedblocks;
            byte vbit = 0, bit;
            int t = 0, bl = 9, ab = 5, j, k, l;
            int maxlength = (int)Math.Pow(2, bl) - 1;
            Bit.Setbits(comprime, 0, 16, width);
            Bit.Setbits(comprime, 16, 16, length);
            Bit.Setbits(comprime, 32, 16, height);
            int i = 48;
            for (l = z; l <= z + height; l++)
            {
                for (k = y; k <= y + length; k++)
                {
                    for (j = x; j <= x + width; j++)
                    {

                        bit = Block[j + k * lw + l * lwl];
                        if (!(vbit == bit && t < maxlength))
                        {
                            Bit.Setbits(comprime, i, ab, vbit);
                            Bit.Setbits(comprime, i + ab, bl, t);
                            t = 0;
                            i += ab + bl;
                            vbit = bit;
                        }
                        t++;

                    }
                }
            }
            Bit.Setbits(comprime, i, ab, vbit);
            Bit.Setbits(comprime, i + ab, bl, t);
            t = 0;
            i += ab + bl;
            comprimedblocks = new Bit[i];
            for (j = 0; j < i; j++)
            {
                comprimedblocks[j] = comprime[j];
            }
            return Bit.ToByte(comprimedblocks);
        }
        ///<summary>
        ///Gets the blocks in bytes.
        ///</summary>
        public byte[] Getbytes()
        {
            Bit[] comprime = new Bit[80000000];
            byte vbit = 0;
            int t = 0, bl = 9, ab = 5;
            int i, j;
            int pcount = Math.Max(bl, ab);
            int[] pow = new int[pcount];
            for (i = 0; i < pcount; i++)
            {
                pow[i] = (int)Math.Pow(2, i);
            }
            int maxlength = (int)Math.Pow(2, bl) - 1;
            Bit.Setbits(comprime, 0, 16, lw);
            Bit.Setbits(comprime, 16, 16, ll);
            Bit.Setbits(comprime, 32, 16, lh);
            i = 48;
            foreach (byte bit in Block)
            {
                if (!(vbit == bit && t < maxlength))
                {
                    for (j = 0; j < ab; j++)
                    {
                        comprime[j + i] = (Bit)((int)(vbit / pow[j]) % 2);
                    }
                    for (j = 0; j < bl; j++)
                    {
                        comprime[j + i + ab] = (Bit)((int)(t / pow[j]) % 2);
                    }
                    Bit.Setbits(comprime, i, ab, vbit);
                    Bit.Setbits(comprime, i + ab, bl, t);
                    t = 0;
                    i += ab + bl;
                    vbit = bit;
                }
                t++;
            }
            Bit.Setbits(comprime, i, ab, vbit);
            Bit.Setbits(comprime, i + ab, bl, t);
            t = 0;
            i += ab + bl;
            int lnt = i / 8;
            Byte[] ret;
            if (i % 8 > 0)
            {
                ret = new Byte[lnt + 1];
            }
            else
            {
                ret = new Byte[lnt];
            }
            for (i = 0; i < lnt; i++)
            {
                j = i * 8;
                ret[i] = (byte)((int)comprime[j] + (int)comprime[j + 1] * 2 + (int)comprime[j + 2] * 4 + (int)comprime[j + 3] * 8 + (int)comprime[j + 4] * 16 + (int)comprime[j + 5] * 32 + (int)comprime[j + 6] * 64 + (int)comprime[j + 7] * 128);
            }
            return ret;
            //                File.WriteAllBytes(path, Block);
        }
        ///<summary>
        ///Reads the blocks from path in comprimed version.
        ///</summary>
        public static Blocks Fromfile(string path)
        {

            return Frombytes(File.ReadAllBytes(path));
        }
        ///<summary>
        ///Sets the blocks from blocks on the specified location.
        ///</summary>
        public void Setblockrange(Shapes wa, int x1, int y1, int z1, int x2, int y2, int z2, byte fill)
        {
            //,params double[] args
            int j, w, l, h, midx = (x1 + x2) / 2, midy = (y1 + y2) / 2, midz = (z1 + z2) / 2, w2, l2, h2;
            if (x1 > x2) { j = x1; x1 = x2; x2 = j; }
            if (y1 > y2) { j = y1; y1 = y2; y2 = j; }
            if (z1 > z2) { j = z1; z1 = z2; z2 = j; }
            w = x2 - x1;
            l = y2 - y1;
            h = z2 - z1;
            w2 = w / 2; l2 = l / 2; h2 = h / 2;
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    for (int z = z1; z <= z2; z++)
                    {
                        switch (wa)
                        {
                            case Shapes.rectangle:
                                Block[x + y * lw + z * lwl] = fill;
                                break;
                            case Shapes.Circle:
                                if (Bigmath.Pyt((x - midx) / (w * 0.5), (y - midy) / (l * 0.5), (z - midz) / (h * 0.5)) <= 1)
                                {
                                    Block[x + y * lw + z * lwl] = fill;
                                }
                                break;
                            case Shapes.pyramid:
                                if ((Math.Abs((x - midx) / (double)w2) <= ((z2 - z) / (double)h)) && (Math.Abs((y - midy) / (double)l2) <= ((z2 - z) / (double)h)))
                                {
                                    Block[x + y * lw + z * lwl] = fill;
                                }
                                break;
                        }
                    }
                }
            }
        }
        ///<summary>
        ///Sets the blocks from blocks on the specified location.
        ///</summary>
        public void Setblockrange(Blocks blockrange, int x, int y, int z, bool alsoair)
        {
            int i, j, k;
            for (k = 0; k <= blockrange.lh; k++)
            {
                for (j = 0; j <= blockrange.ll; j++)
                {
                    for (i = 0; i <= blockrange.lw; i++)
                    {
                        if ((blockrange.Block[i + j * blockrange.lw + k * blockrange.lwl] == 0) && !alsoair) { }
                        else { Block[(i + x) + (j + y) * lw + (k + z) * lwl] = blockrange.Block[i + j * blockrange.lw + k * blockrange.lwl]; }
                    }
                }
            }
        }
        public enum Shapes
        {
            rectangle = 1,
            Circle = 2,
            pyramid = 3
        };

        ///<summary>
        ///Set the blocks from bytes in comprimed version.
        ///</summary>
        public static Blocks Frombytes(byte[] bytes)
        {
            Bit[] bits = Bit.ToBit(bytes);
            int bl = 9, ab = 5;
            Blocks block = new Blocks();
            int i, ctr = 0, j, howmuch, bytepos;
            byte what;
            block.Rescale(Bit.Getbits(bits, 0, 16), Bit.Getbits(bits, 16, 16), Bit.Getbits(bits, 32, 16));
            for (i = 48; i <= bits.Length - bl - ab; i += bl + ab)
            {
                what = 0;
                for (bytepos = 0; bytepos < ab; bytepos++)
                {
                    what += (byte)(bits[bytepos + i] * (int)Math.Pow(2, bytepos));
                }
                howmuch = 0;
                for (bytepos = 0; bytepos < bl; bytepos++)
                {
                    howmuch += (bits[bytepos + i + ab] * (int)Math.Pow(2, bytepos));
                }
                for (j = 0; j < howmuch; j++)
                {
                    block.Block[ctr] = what;
                    ctr++;
                }
            }
            return block;
        }


        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class Gui
    {

        public delegate void Clickeventargs(double x, double y, MouseButtons pressed);
        public delegate bool Closeeventargs();
        public delegate void Keypresseventargs(Keys pressed);
        public class Object
        {
            public Font font;
            public event Clickeventargs Clickormove;
            public event Closeeventargs Closing;
            public event Keypresseventargs Press;
            public event Keypresseventargs Release;
            public bool Close()
            {
                if (Closing != null)
                {
                    return Closing.Invoke();
                }
                else
                {
                    return true;
                }
            }
            public void Presskey(Keys pressed)
            {
                if (Press != null)
                {
                    Press.Invoke(pressed);
                }
            }
            public void Releasekey(Keys released)
            {
                if (Release != null)
                {
                    Release.Invoke(released);
                }
            }
            public string name = "";
            public void Clickonform(double x, double y, MouseButtons pressed) => Clickormove.Invoke(x, y, pressed);
            public Object()
            {
                Clickormove += new Clickeventargs(Onclickormove);
                font = new Font("Tahoma", (float)(Height * 0.8));
            }

            public static Object[] Combine(params Object[] objects) => objects;
            public void Addobject(Object obj) { Object[] o = objects; objects = new Object[o.Length + 1]; for (int i = 0; i < o.Length; i++) { objects[i] = o[i]; } objects[o.Length] = obj; }
            public void Removeobject(string name) { for (int i = 0; i < objects.Length; i++) { if (objects[i].name == name) { Removeobject(i); return; } } }
            public void Hide() => visible = false;
            public void Show() => visible = true;
            public void Hideall() { foreach (Object obj in objects) { obj.visible = false; } }
            public void Showall() { foreach (Object obj in objects) { obj.visible = true; } }
            public void Removeobject(int index)
            {
                Object[] obj = objects;
                objects = new Object[obj.Length - 1];
                int j = 0;
                for (int i = 0; i + j < obj.Length; i++)
                {
                    if (i == index) { j = 1; if (i + j == obj.Length) { return; } }
                    objects[i] = obj[i + j];
                }
            }
            private void Onclickormove(double x, double y, MouseButtons press)
            {
                foreach (Object obj in objects)
                {
                    if (obj.visible && Bigmath.Checkwithin(obj.Left, obj.Left + obj.Width, x) && Bigmath.Checkwithin(obj.Top, obj.Top + obj.Height, y))
                    {
                        obj.Clickormove(x - obj.Left, y - obj.Top, press);
                    }
                }
            }
            public Bitmap Draw(Bitmap on)
            {
                Graphics.FromImage(on).DrawImage(Draw(), Left, Top, Width, Height);
                return on;
            }
            public Bitmap Draw(Bitmap on, int left, int top, int width, int height)
            {
                Graphics.FromImage(on).DrawImage(Draw(), left, top, width, height);
                return on;
            }
            public bool Anobjectvisible() { foreach (Object obj in objects) { if (obj.visible) { return true; } } return false; }
            public bool changeresolution = false, visible = true;
            public int Width = 1, Height = 1, Borderwidth = 0, Top = 0, Left = 0;
            public bool relative = true;
            public Color bordercolor = Color.Black, backcolor = Color.White;
            public Brush textcolor = Brushes.Black;
            public Bitmap Backimage;
            public Object[] objects = new Object[0];
            public string text = "";
            public Bitmap Draw()
            {
                Bitmap outpicture;
                if (changeresolution && Borderwidth == 0 && (text == "") && !Anobjectvisible() && Backimage != null)
                {
                    return Backimage;
                }
                else
                {
                    outpicture = new Bitmap(Width, Height);
                }
                using (Graphics graphics = Graphics.FromImage(outpicture))
                {
                    graphics.FillRectangle(new SolidBrush(bordercolor), new Rectangle(0, 0, Width, Height));
                    if (Backimage != null)
                    {
                        graphics.DrawImage(Backimage, Borderwidth, Borderwidth, Width - Borderwidth - 1, Height - Borderwidth - 1);
                    }
                    else
                    {
                        graphics.FillRectangle(new SolidBrush(backcolor), new Rectangle(Borderwidth, Borderwidth, Width - Borderwidth - 1, Height - Borderwidth - 1));
                    }

                    foreach (Object obj in objects)
                    {
                        if (obj.visible)
                        {
                            graphics.DrawImage(obj.Draw(), obj.Left, obj.Top, obj.Width, obj.Height);
                        }
                    }



                    graphics.DrawString(text, font, textcolor, new Rectangle(Borderwidth, Borderwidth, Width - Borderwidth, Height - Borderwidth));
                } // graphics will be disposed at this line
                return outpicture;
            }
        }
    }
    ///<summary>Set and get the ThreeD - view.</summary>
    public class View
    {
        public class Light
        {
            public double[] Brightness = { 0, 1, 1, 1 };
            public int x, y, z;
            public double straal;
            public bool on = true;
        }
        Light[] Onlights;
        void Setlightson() {
            Light[] ol=new Light[lights.Length];
            int j=0;
          for(int i = 0; i < lights.Length;i++) {
                if (lights[i].on) {
                    ol[j] = lights[i];
                    j++;
                }
          }
            Onlights = new Light[j];
            for (int i = 0; i < j;i++)
            {
                Onlights[i] = ol[i];
            }
        }
        public double[] Getlights(double x, double y, double z, double[] brightnessalready,double[] Sunlight,double[] pos)
        {
            double prx=x+pos[1], pry=y+pos[2], prz=z+pos[3];
            int i, j, totpos, ototpos, ltotpos;
            double stepx, stepy, stepz, dist;
            double[] brightness =Sunlight;
            bool islighted=false;
            for (j = 0; j < Onlights.Length; j++)
            {
                stepx = Onlights[j].x - x;
                stepy = Onlights[j].y - y;
                stepz = Onlights[j].z - z;
                dist = Math.Sqrt(stepx * stepx + stepy * stepy + stepz * stepz);
                if (dist <= Onlights[j].straal)
                {
                    islighted = true;
                    stepx /= dist;
                    stepy /= dist;
                    stepz /= dist;
                    totpos = (int)(x) + (int)(y) * Block.lw + (int)(z) * Block.lwl;
                    ltotpos = (Onlights[j].x) + (Onlights[j].y) * Block.lw + (Onlights[j].z) * Block.lwl;
                    for (i = 1; i < dist; i++)
                    {
                        ototpos = totpos;
                        totpos = (int)(x + i * stepx) + (int)(y + i * stepy) * Block.lw + (int)(z + i * stepz) * Block.lwl;
                        if (totpos != ototpos)
                        {
                            if ((Block.Block[totpos]) > 1)
                            {
                                goto uit;
                            }
                        }
                    }
                    totpos = ltotpos;
                    uit:;
                    if (totpos != ltotpos)
                    {
                        goto buitenblok;
                    }
                    dist = Math.Sqrt((prx-Onlights[j].x) * (prx - Onlights[j].x) + (pry - Onlights[j].y) * (pry - Onlights[j].y) + (prz - Onlights[j].z) * (prz - Onlights[j].z));
                    for (int k = 1; k < 4; k++)
                    {
                        brightness[k] += (1 - brightness[k]) * (Onlights[j].Brightness[k] / Math.Sqrt((dist * 0.1) + 1) );
                    }
                    buitenblok:;
                }
            }
//            if (islighted) 
//            {
                brightness[1] = brightness[1] * brightnessalready[1];
                brightness[2] = brightness[2] * brightnessalready[2];
                brightness[3] = brightness[3] * brightnessalready[3];
//            }
//            else
//            { brightness = brightnessalready; }
            return brightness;
        }
        public void Rescale(int width, int length, int height)
        {
            Block.Rescale(width, length, height);
        }
        public void SwitchLight(int x, int y, int z)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].x == x && lights[i].y == y && lights[i].z == z)
                { lights[i].on = !lights[i].on; }
            }
        }
        public void Removelight(int x, int y, int z)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].x == x && lights[i].y == y && lights[i].z == z)
                { Removelight(i); }
            }
        }
        public void Removelight(int index)
        {
            Light[] lts = lights;
            lights = new Light[lts.Length - 1];
            int j = 0;
            for (int i = 0; i + j < lts.Length; i++)
            {
                if (i == index) { j = 1; if (i + j == lts.Length) { return; } }
                lights[i] = lts[i + j];
            }
        }
        public void Addlight(Light whichligt)
        {
            Light[] lts = lights;
            lights = new Light[lts.Length + 1];
            for (int i = 0; i < lts.Length; i++)
            {
                lights[i] = lts[i];
            }
            lights[lts.Length] = whichligt;
        }
        public Light[] lights = new Light[0];
        public Light Sun = new Light { straal = 1000, x = 500, y = 500, z = 500 };
        byte[,,] aircolor = { { { 0, 100, 200, 255 } } };
        byte[,,,,] blockcolor;
        double afstand, awr, pi180 = Math.PI / 180, airwidth = 1, airheight = 1;
        double[] snelh = new double[4], rad = new double[3], vrad = new double[3];
        const byte vol = 255;
        const double sa = 0.5;
        public Blocks Block = new Blocks();
        public Bitmap getview;
        int kijkwijdte = 100, xp, yp, texturewidth = 101, etexturewidth = 100, rood, groen, blauw;
        int[] currentpixelblock = new int[7], schaal = { 0, 1, 1 };
        double mist = 100, zfactor = 1, vaagheid = 1;
        double[] position = new double[4], rotation = new double[4], hoek = new double[3];
        double[,] dhe = new double[7, 4];
        bool lijn, vizier;

        public static View Fromblocks(Blocks blocks)
        {
            View retview = new View
            {
                Block = blocks
            };
            return retview;
        }
        ///<summary>Set the position by axis x, y and z, and screenrotation by x (0 to 360) and y(-90 to 90).</summary>
        public void Setposition(double x, double y, double z, double rotationx, double rotationy, double rotationz)
        {
            position[1] = x;
            position[2] = y;
            position[3] = z;
            rotation[1] = rotationx;
            rotation[2] = rotationy;
            rotation[3] = rotationz;
        }
        ///<summary>Makes a folder with empty images named to the block byte and the side(1 t0 6) of  the block.
        ///blockcount: the number of blocks you want to create images for.
        ///blocktexturewidth: the width and height(the same, because it's a square) in pixels of the block.
        ///</summary>
        public static void Make_color_dir(string dirpath, int blockcount, int blocktexturewidth)
        {
            int k, bdh;
            Bitmap bmp = new Bitmap(blocktexturewidth, blocktexturewidth);
            for (k = 2; k < 30; k++)
            {
                for (bdh = 1; bdh < 7; bdh++)
                {
                    bmp.Save(dirpath + "blok" + k + "," + bdh + ".BMP");
                }
            }
        }
        ///<summary>Get the location of block the middlest pixel of the screen pointed.
        ///[0] indicates whether air or water (returns 1) is pointed or a block(returns 0).
        ///[1],[2],[3] indicate the position of the block.
        ///[4] indicates the side of the block that have been pointed.
        /// .</summary>
        public int[] Getblockpointed { get => currentpixelblock; }
        ///<summary>
        ///Sets the screenproperties. Width,Height: Screenwidth and height,blodkgrid: whether the blocks must have a line,
        ///Gunsight: If you want to have an visor,Brightness: how much water you can see in the air and water,
        ///ViewWidth: maximal width you can see,Zoom: 1-based variabele remains the part of the screen you can see(0,5=2x zoom).
        ///</summary>
        public void Setwindowproperties(int width, int height, bool blockgrid, bool gunsight, double Brightness, int ViewWidth, double zoom, double vagueness)
        {
            schaal[1] = width; schaal[2] = height;
            kijkwijdte = ViewWidth;
            lijn = blockgrid;
            vizier = gunsight;
            mist = Brightness;
            if (zoom > 1) { zoom = 1; }
            zfactor = zoom;
            vaagheid = vagueness;
        }
        ///<summary>
        ///Initialize OpenView and load GPU.
        ///You must only load the blockcolors from a file to don't get any error.
        public View() =>
            //cl.Accelerator = AcceleratorDevice.GPU;
            //   cl.LoadKernel(Beeldfunc);
            Setcolortemperature(1, 1, 1);
        ///<summary>
        ///Loads textures from an folder width for each block six sides. 
        ///Filenames: "blok" & which block(2 to blockcount) & "," & which side(1 to 6) & ".bmp"
        ///For an sample: Make_color_dir creates a directory with black images.
        ///</summary>
        public void Load_colors(string dirpath, int blockcount, int blocktexturewidth)
        {
            blockcolor = new byte[blockcount + 1, blocktexturewidth, blocktexturewidth, 7, 4];
            texturewidth = blocktexturewidth;
            etexturewidth = blocktexturewidth - 1;
            int i, j, k, l, bdh, stap;
            for (k = 2; k <= blockcount; k++)
            {
                for (bdh = 1; bdh < 7; bdh++)
                {
                    Bitmap bmp = (Bitmap)Image.FromFile(dirpath + "blok" + k + "," + bdh + ".BMP");
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    IntPtr ptr = bmpdata.Scan0;
                    int bytes = Math.Abs(bmpdata.Stride) * bmp.Height;
                    byte[] rgbValues = new byte[bytes];
                    Marshal.Copy(ptr, rgbValues, 0, bytes);
                    l = 0;
                    stap = (bytes / (texturewidth * texturewidth));
                    for (j = 0; j < texturewidth; j++)
                    {
                        for (i = 0; i < texturewidth; i++)
                        {
                            blockcolor[k, i, j, bdh, 1] = rgbValues[l];
                            blockcolor[k, i, j, bdh, 2] = rgbValues[l + 1];
                            blockcolor[k, i, j, bdh, 3] = rgbValues[l + 2];
                            l += stap;
                        }
                        l += 4 - stap;
                    }
                    bmp.UnlockBits(bmpdata);
                }
            }
        }
        ///<summary>
        ///Loads textures from an folder width for each block six sides. 
        ///Filenames: "blok" & which block(2 to blockcount) & "," & which side(1 to 6) & ".bmp"
        ///For an sample: Make_color_dir creates a directory with black images.
        ///</summary>
        public void Load_airtexture(string path)
        {
            int stap, i, j, k, l, extrastap;

            Bitmap bmp = (Bitmap)Image.FromFile(path);
            airheight = bmp.Height;
            airwidth = bmp.Width;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            int bytes = Math.Abs(bmpdata.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            stap = (int)(bytes / (airwidth * airheight));
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                extrastap = (int)(((bytes + 1) / (airwidth * airheight) - stap) * airwidth);
            }
            else { extrastap = 0; }

            l = 0;
            aircolor = new byte[(int)airwidth, (int)airheight, 4];
            for (j = 0; j < airheight; j++)
            {
                for (i = 0; i < airwidth; i++)
                {
                    for (k = 2; k >= 0; k--)
                    {
                        aircolor[i, j, 3 - k] = rgbValues[l + k];
                    }
                    l += stap;
                }
                l += extrastap;
            }
            airwidth--;
            airheight--;
            bmp.UnlockBits(bmpdata);
        }
        public void Setmousetomidpicturebox(Form seton, Cursor toset, bool showing)
        {
            int mx = seton.Width / 2;
            int my = seton.Height / 2;

            switch (seton.FormBorderStyle)
            {
                case FormBorderStyle.Sizable:
                    Setmouse(mx + seton.Left + 8, my + 80 + seton.Top, toset, seton, showing);
                    break;
                case FormBorderStyle.None:
                    Setmouse(mx + seton.Left, my + seton.Top, toset, seton, showing);
                    break;
            }
        }
        public void Setmouse(int posx, int posy, Cursor mouse, Form seton, bool showing)
        {
            try
            {
                //mouse = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point(posx, posy);
                Cursor.Clip = new Rectangle(seton.Location, seton.Size);
                if (showing)
                {
                    Cursor.Show();
                }
                else { Cursor.Hide(); }
            }
            catch { MessageBox.Show("error"); }
        }

        public static Bitmap Makebitmap(int width, int height, byte[] rgbcolors)
        {
            Bitmap bmp = new Bitmap(width, height);
            //naar pixels
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            int bytes = width * height * 4;
            //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)
            //einde pixels
            Marshal.Copy(rgbcolors, 0, ptr, bytes);
            bmp.UnlockBits(bmpdata);
            return bmp;
        }
        ///<summary>
        ///Reloads the view to the bitmap getview.
        ///</summary>
        public void Reloadview()
        {
            int i, s1, s2, wiestap, w1 = schaal[1] / 2, w2 = schaal[2] / 2, totpos, ototpos, k = 0;
            int sx, sy, sz, posx, posy, posz, vblok;
            double dh, wtr, bgposx, bgposy, factorx, factory, factorz;
            byte j, a, inwhichblock;
            byte[] rgbValues = new byte[schaal[1] * schaal[2] * 4];
            bool ber, nber;
            double[] matfactor = new double[4], matf = new double[4], matepos = new double[4], mats = new double[4], matpos = new double[4], matv = new double[4], vradpos = new double[3],Water= {0,0,((double)176/255)*dhe[6,2], ((double)235 / 255)*dhe[6,3] }, posvlak = new double[4];
            double[,] csin = new double[4, 3];
            double pytft, vx, vy, vz, fx, fy, fz, eposx, eposy, eposz, posix, posiy, posiz, spx, spy, spz, distancefrommid, dikte = 0;
            const int waterscheiding = 5;
            Setlightson();
            if (schaal[1] > schaal[2])
            { hoek[1] = zfactor; hoek[2] = zfactor / (schaal[1] / (double)schaal[2]); }
            else
            {
                hoek[2] = zfactor;
                hoek[1] = zfactor / (schaal[2] / (double)schaal[1]);
            }
            csin[1, 1] = Math.Sin(rotation[1] * pi180);
            csin[1, 2] = Math.Cos(rotation[1] * pi180);
            csin[2, 1] = Math.Sin(rotation[2] * pi180);
            csin[2, 2] = Math.Cos(rotation[2] * pi180);
            csin[3, 1] = Math.Sin(rotation[3] * pi180);
            csin[3, 2] = Math.Cos(rotation[3] * pi180);
            posix = position[1];
            posiy = position[2];
            posiz = position[3];
            eposx = posix - (int)posix;
            eposy = posiy - (int)posiy;
            eposz = posiz - (int)posiz;
            dh = 6;
            ber = Block.Checkwithinreach(posix, posiy, posiz);
            if (ber)
            {
                inwhichblock = Block.Getblock(position[1], position[2], position[3]);
            }
            else { inwhichblock = 0; }
            ototpos = (int)posix + (int)posiy * Block.lw + (int)posiz * Block.lwl;
            s1 = schaal[1] - 1;
            s2 = schaal[2] - 1;
            for (yp = 0; yp < schaal[2]; yp++)
            {
                for (xp = 0; xp < schaal[1]; xp++)
                {
                    double[] br=new double[4];
                    vradpos[2] = ((((double)yp / schaal[2]) - 0.5) * hoek[2]); //yz
                    vradpos[1] = ((((double)xp / schaal[1]) - 0.5) * hoek[1]); //xy
                    Bigmath.Turn(vradpos[1], vradpos[2], rotation[3], out vrad[1], out vrad[2]);
                    factorz = vrad[2] * -csin[2, 2] + csin[2, 1] * sa;
                    factory = csin[2, 2] * sa + csin[2, 1] * vrad[2];
                    factorx = csin[1, 2] * vrad[1] + csin[1, 1] * factory;
                    factory = csin[1, 2] * factory - csin[1, 1] * vrad[1];
                    pytft = Math.Sqrt(factorx * factorx + factory * factory + factorz * factorz);
                    factorx /= pytft;
                    factory /= pytft;
                    factorz /= pytft;
                    totpos = ototpos;
                    nber = ber;
                    //x
                    vx = eposx;
                    fx = Math.Abs(factorx);
                    if (factorx < 0) { sx = -1; } else { sx = 1; }
                    posx = (int)posix;
                    if (factorx > 0) { vx = 1 - vx; }
                    spx = vx / fx;
                    //y
                    vy = eposy;
                    fy = Math.Abs(factory);
                    if (factory < 0) { sy = -1; } else { sy = 1; }
                    posy = (int)posiy;
                    if (factory > 0) { vy = 1 - vy; }
                    spy = vy / fy;
                    //z
                    vz = eposz;
                    fz = Math.Abs(factorz);
                    if (factorz < 0) { sz = -1; } else { sz = 1; }
                    posz = (int)posiz;
                    if (factorz > 0) { vz = 1 - vz; }
                    spz = vz / fz;

                    wtr = j = a = 0;
                    if (inwhichblock == 1) { vblok = -1; }
                    else
                    {
                        vblok = -2;
                    }
                    for (wiestap = 0; wiestap <= kijkwijdte; wiestap++)
                    {
                        if (!(spx > spy || spx > spz))//x
                        {
                            j = 1; totpos += sx; posx += sx; vx += 1; spx = vx / fx;
                            if (posx < 0 || posx > Block.lwe) { if (nber) { goto uit; } else { goto volgende; } }
                        }
                        else if (spy <= spz)//y
                        {
                            j = 2; totpos += sy * Block.lw; posy += sy; vy += 1; spy = vy / fy;
                            if (posy < 0 || posy > Block.lle) { if (nber) { goto uit; } else { goto volgende; } }
                        }
                        else//z
                        {
                            j = 3; totpos += sz * Block.lwl; posz += sz; vz += 1; spz = vz / fz;
                            if (posz < 0 || posz > Block.lhe) { if (nber) { goto uit; } else { goto volgende; } }
                        }
                        if (!ber)
                        {
                            if (posx < 0 || posx > Block.lwe || posy < 0 || posy > Block.lle || posz < 0 || posz > Block.lhe)
                            { goto volgende; }
                            else
                            { nber = true; }
                        }

                        a = Block.Block[totpos];
                        if (a == 0) { }
                        else
                        if (a == 1)
                        {
                            if (vblok == wiestap - 1) { wtr += 0.2; } else { if (vblok > -2) { wtr += waterscheiding * 2; } else { wtr += waterscheiding; } }
                            vblok = wiestap;
                        }
                        /*                        else if (a == 2) {
                                                    if (j == 1) { factorx *= -1; }
                                                    else if (j == 2) { factory *= -1; }
                                                    else if (j == 3) { factorz *= -1; }
                                                }
                          */
                        else { goto uit; }
                        a = 0;
                        volgende:;
                    }
                    uit:
                    if (inwhichblock == 1 && vblok != wiestap - 1) { wtr += waterscheiding; }
                    switch (j)
                    {
                        case 1:
                            dh = 3.5 + sx * -0.5; //vlak
                            break;
                        case 2:
                            dh = 3.5 + sy * -1.5; //vlak
                            break;
                        case 3:
                            dh = 3.5 + sz * -2.5; //vlak
                            break;
                    }
                    if (a < 2)
                    {
                        afstand = kijkwijdte / mist;
                        bgposx = Bigmath.Getrotation(factorx, factory);
                        //bgposx = Math.Asin(factorx) / pi180;
                        //if ((factory) > 0) { bgposx = (180 * (double)sx) - bgposx; }
                        bgposx = ((bgposx / 360) * airwidth);
                        bgposy = -Math.Asin(factorz) / pi180;
                        bgposy = (((bgposy / 180) + 0.5) * airheight);
                        wtr = (wtr * 0.2) + afstand;
                        awr = wtr + 1;
                        br[1] = (int)((aircolor[(int)bgposx, (int)bgposy, 1] + Water[1]) / awr)*(dhe[6,1]/255);
                        br[2] = (int)((aircolor[(int)bgposx, (int)bgposy, 2] + Water[2]) / awr) * (dhe[6, 2]/255);
                        br[3] = (int)((aircolor[(int)bgposx, (int)bgposy, 3] + Water[3]) / awr) * (dhe[6, 3]/255);
                        dh = 6;
                        goto Alleenwater;
                    }
                    else
                    {
                        afstand = (int)Math.Pow(vx * vx + vy * vy + vz * vz, 0.5) / mist;
                        matfactor[1] = factorx;
                        matfactor[2] = factory;
                        matfactor[3] = factorz;
                        matepos[1] = eposx;
                        matepos[2] = eposy;
                        matepos[3] = eposz;
                        matf[1] = fx;
                        matf[2] = fy;
                        matf[3] = fz;
                        matpos[1] = posx;
                        matpos[2] = posy;
                        matpos[3] = posz;


                        for (i = 1; i <= 3; i++)
                        {
                            if (i != j)
                            {
                                if (matf[j] == 0) { posvlak[i] = 0.5; }
                                else
                                {
                                    if (matfactor[j] > 0)
                                    { posvlak[i] = (matpos[j] - position[j]) * matfactor[i] / matf[j] + matepos[i]; }
                                    else
                                    {
                                        posvlak[i] = (position[j] - matpos[j] - 1) * matfactor[i] / matf[j] + matepos[i];
                                    }

                                    if (posvlak[i] < 0) { posvlak[i] += (int)(posvlak[i] * -1) + 1; } else { posvlak[i] -= (int)posvlak[i]; }
                                }
                                if (lijn)
                                {
                                    dikte = afstand * 0.1 * vaagheid * zfactor;
                                    if (dikte < 0.2 && (posvlak[i] < dikte || posvlak[i] > (1 - dikte))) {br=new double[4]; goto geenkleur; }
                                }
                            }
                        }
                        switch (dh)
                        {
                            case 1:
                                posvlak[1] = 1 - posvlak[1];
                                break;
                            case 2:
                                posvlak[2] = 1 - posvlak[3];
                                break;
                            case 3:
                                posvlak[1] = posvlak[2];
                                posvlak[2] = 1 - posvlak[3];
                                break;
                            case 4:
                                posvlak[1] = 1 - posvlak[2];
                                posvlak[2] = 1 - posvlak[3];
                                break;
                            case 5:
                                posvlak[1] = 1 - posvlak[1];
                                posvlak[2] = 1 - posvlak[3];
                                break;
                            case 6:
                                break;
                                //dh = 3.5 + ((double)(sx[j]) * (j - 0.5) * -1);
                        }
                        wtr = (wtr * 0.2) + afstand;
                        awr = (wtr + 1);
                        blauw = blockcolor[a, (int)(posvlak[1] * etexturewidth), (int)(posvlak[2] * etexturewidth), (int)dh, 1] ;
                        groen = blockcolor[a, (int)(posvlak[1] * etexturewidth), (int)(posvlak[2] * etexturewidth), (int)dh, 2];
                        rood = blockcolor[a, (int)(posvlak[1] * etexturewidth), (int)(posvlak[2] * etexturewidth), (int)dh, 3] ;
                    }
                    //shifting water and blockcolor
                    double[] sunlight = { 0, dhe[(int)dh, 1], dhe[(int)dh, 2], dhe[(int)dh, 3] };
                    br[1] = (double)rood / 255;
                    br[2] = (double)groen / 255;
                    br[3]= (double)blauw / 255;
                    switch (j)
                    {
                        case 1: br = Getlights(posx - sx, posy, posz, br,sunlight,posvlak); break;
                        case 2: br = Getlights(posx, posy - sy, posz, br,sunlight,posvlak); break;
                        case 3: br = Getlights(posx, posy, posz - sz, br,sunlight,posvlak); break;
                    }
                    Alleenwater:
                    br[3] = (br[3] + (wtr * Water[3])) / awr;
                    br[2] = (br[2] + (wtr * Water[2])) / awr;
                    br[1] /=awr;
                    geenkleur:;
                    if (yp == w2 || xp == w1)
                    {
                        if (yp == w2 && xp == w1)
                        {
                            if (a > 1) { currentpixelblock[4] = (int)dh; currentpixelblock[1] = posx; currentpixelblock[2] = posy; currentpixelblock[3] = posz; currentpixelblock[0] = 0; }
                            else { currentpixelblock[0] = 1; }
                        }
                        distancefrommid = Math.Sqrt(Math.Pow(xp - w1, 2) + Math.Pow(yp - w2, 2));
                        if (distancefrommid > ((schaal[1] * (hoek[2] / zfactor)) * 0.2) && (distancefrommid < ((schaal[1] * (hoek[2] / zfactor)) * 0.4)))
                        {
                            if (vizier) { if (br[1] + br[2] + br[3] > 1.5) { br=new double[4]; } else { br[1]=br[2]=br[3]=1; } }
                        }
                    }
                    rgbValues[k] = (byte)(br[3] * 255);
                    rgbValues[k + 1] = (byte)(br[2] * 255);
                    rgbValues[k + 2] = (byte)(br[1] * 255);
                    rgbValues[k + 3] = vol;
                    k += 4;
                }
            }
            getview = Makebitmap(schaal[1], schaal[2], rgbValues);
        }
        public double[] ShiftLight(double[] l1, double[] l2) {
            double[] ret = new double[4];
           ret[1]= l1[1] + (1 - l1[1]) * l2[1];
           ret[2]= l1[2] +  (1 - l1[2]) * l2[2];
           ret[3]= l1[3] +  (1 - l1[3]) * l2[3];
return ret;
        }
        public Bitmap Getmap()
        {
            byte[] colors = new byte[schaal[1] * schaal[2] * 4];
            int k = 0, x, y, z;
            byte a = 0;
            for (int j = 0; j < schaal[2]; j++)
            {
                for (int i = 0; i < schaal[1]; i++)
                {
                    for (z = Block.lhe; z >= 0; z--)
                    {
                        x = (int)((i / (double)schaal[1]) * Block.lwe);
                        y = (int)((j / (double)schaal[2]) * Block.lle);
                        a = Block.Block[x + y * Block.lw + z * Block.lwl];
                        if (a > 0) { goto uit; }
                    }
                    uit:
                    colors[k] = blockcolor[a, (int)(0.5 * etexturewidth), (int)(0.5 * etexturewidth), (int)6, 1];
                    colors[k + 1] = blockcolor[a, (int)(0.5 * etexturewidth), (int)(0.5 * etexturewidth), (int)6, 2];
                    colors[k + 2] = blockcolor[a, (int)(0.5 * etexturewidth), (int)(0.5 * etexturewidth), (int)6, 3];
                    colors[k + 3] = vol;
                    k += 4;
                }
            }


            return Makebitmap(schaal[1], schaal[2], colors);
        }
        public byte[] Combinecolors(byte[] color1, byte[] color2)
        {
            byte[] ret = new byte[4];
            ret[1] = (byte)(color1[1] + (255 - color1[1]) * color2[1]);
            ret[1] = (byte)(color1[2] + (255 - color1[2]) * color2[2]);
            ret[1] = (byte)(color1[3] + (255 - color1[3]) * color2[3]);
            return ret;

        }
        ///<summary>
        ///Sets the color tempreature. For an example: 1,1,1 is full colors and brightness 0.2 is night.
        ///Sunset - example: 1,0.8,0
        ///</summary>
        public void Setcolortemperature(double red, double green, double blue)
        {

            for (int i = 1; i < 7; i++)
            {
                dhe[i, 1] = red * (i * 0.1 + 0.4);
                dhe[i, 2] = green * (i * 0.1 + 0.4);
                dhe[i, 3] = blue * (i * 0.1 + 0.4);
            }
        }
        byte Countlight(byte l1, byte l2)
        {
            return (byte)(255 - ((510 - l1 - l2) * 0.5));
        }
        public override string ToString() => base.ToString();
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

    }
}