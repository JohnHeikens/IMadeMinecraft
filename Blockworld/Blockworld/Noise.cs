using Generating;
using openview;
using System;
using Microsoft.VisualBasic;
namespace Noise {
    public class Blocknoise {
        private Random doblstn=new Random();
        private int widthwithscale, lengthwithscale;
        public int width, length, scale,scaledivided;
        public double minimum = 0, maximum = 1,area;
        private double difference,multiply;
        public double[] Shuffle(double[] input) {
            double[] output=new double[input.Length];
            bool[] used = new bool[input.Length];
            int i,j;
            for (i = 1; i < input.Length; i++) {

                do
                {
                    j = doblstn.Next(0, input.Length);
                } while (used[j]);
                used[j] = true;
                output[j] = input[i];
            }
            return output;
        }
        public Blocknoise(int width, int length, int scale,double minimum,double maximum)
        {
            this.scale = scale;
            this.minimum = minimum;
            this.maximum = maximum;
            difference = maximum - minimum;
            scaledivided = scale / 2;
            //            area = scaledivided * scaledivided * Math.PI;
            widthwithscale = width + scale; lengthwithscale=length+scale;
            Randomvalues = new double[widthwithscale, lengthwithscale];
            Totals = new double[width, length];
            made = new bool[width, length];
            for (int i = 1; i < widthwithscale; i++)
            {
                for (int j = 1; j < lengthwithscale; j++)
                {
                    Randomvalues[i, j] = doblstn.NextDouble();// -0.5;
                }
            }
            Rounding = new int[scale + 1, 2];
            int xposition,yposition;
            area = 0;
            for (yposition = 0; yposition <= scaledivided; yposition++)
            {
                
                xposition = Bigmath.Fix(scaledivided - Math.Sqrt(scaledivided * scaledivided - (yposition - scaledivided) * (yposition - scaledivided)));
                Rounding[yposition, 0] = Rounding[scaledivided * 2 - yposition, 0] = xposition;
                Rounding[yposition, 1] = Rounding[scaledivided * 2 - yposition, 1] = scale - xposition;
                area += (scaledivided - xposition);
            }
            area = (1) + (area * 4);
            multiply = area*0.5/scale;
            /*            widthfactor = new int[4, heighestvlkgt * 2 + 1, 2];
            int xpos;
            for (int i = 1; i < 4; i++)
            {
                for (int ypos = 0; ypos <= vlakgt[i]; ypos++)
                {
                    xpos = (int)(vlakgt[i] - Math.Sqrt(vlakgt[i] * vlakgt[i] - (ypos - vlakgt[i]) * (ypos - vlakgt[i])));
                    widthfactor[i, ypos, 0] = widthfactor[i, (int)vlakgt[i] * 2 - ypos, 0] = (int)xpos;
                    widthfactor[i, ypos, 1] = widthfactor[i, (int)vlakgt[i] * 2 - ypos, 1] = (int)vlakgt[i] * 2 - xpos;
                }
*/

        }
        private double[,] Randomvalues,Totals;
        private bool[,] made;
        private int[,] Rounding;
        private double Newtotal(int x,int y) {
            int i, j,ctr=0;
            double tot=0;
            for (i = 0; i <= scale; i++) {
                for (j = Rounding[i, 0]; j <= Rounding[i, 1]; j++) {
                    tot += Randomvalues[i+x, j+y];
                    ctr++;
                }
            }
            return tot;
        }
        private double Fromaside(int x,int y,int shiftx,int shifty) {
            int xshifted = shiftx + x,yshifted = shifty + y,shiftright,shiftleft,iterator=0;
            double tot = Totals[xshifted, yshifted];
            if (shiftx == 1 || shifty == 1) { shiftleft = 0; shiftright = 1; } else { shiftleft = -1;shiftright = 0; }
            if (shifty == 0)
            {//y is moving
                for (; iterator <= scale; iterator++) {
                    tot += shiftx * (Randomvalues[x+Rounding[iterator,0]+shiftleft,y+iterator]-Randomvalues[x+Rounding[iterator,1]+shiftright,y+iterator]);
                }
            }
            else
            {//x is moving
                for (; iterator <= scale; iterator++) {
                    tot += shifty * (Randomvalues[x+iterator,y+Rounding[iterator,0]+shiftleft]-Randomvalues[x+iterator,y+Rounding[iterator,1]+shiftright]);
                }
            }
            return tot;
        }
        public double Getnoise(int x,int y) {
            if (!made[x, y])
            {
                if (x+1<width && made[x + 1, y]) { Totals[x, y] = Fromaside(x, y, 1, 0); }
                else if (x>0 && made[x - 1, y]) { Totals[x, y] = Fromaside(x, y, -1, 0); }
                else if (y+1<length && made[x, y + 1]) { Totals[x, y] = Fromaside(x, y, 0, 1); }
                else if (y>0 && made[x, y - 1]) { Totals[x, y] = Fromaside(x, y, 0, -1); }
                else { Totals[x, y] = Newtotal(x, y); }
            }
            made[x, y] = true;
            double ret = Totals[x, y];
            ret/= area;
            ret = Awayfrommid(ret);
            ret = minimum + ret * difference;
            
            return ret;
        }
        public double Awayfrommid(double inp) {
            return ((inp - 0.5) * multiply) + 0.5;
        }
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
}
/*
// Function to linearly interpolate between a0 and a1
 // Weight w should be in the range [0.0, 1.0]
 function lerp(float a0, float a1, float w) {
     return (1.0 - w)*a0 + w*a1;
 }
 
 // Computes the dot product of the distance and gradient vectors.
 function dotGridGradient(int ix, int iy, float x, float y) {
 
     // Precomputed (or otherwise) gradient vectors at each grid node
     extern float Gradient[IYMAX][IXMAX][2];
 
     // Compute the distance vector
     float dx = x - (float)ix;
     float dy = y - (float)iy;
 
     // Compute the dot-product
     return (dx*Gradient[iy][ix][0] + dy*Gradient[iy][ix][1]);
 }
 
 // Compute Perlin noise at coordinates x, y
 function perlin(float x, float y) {
 
     // Determine grid cell coordinates
     int x0 = floor(x);
     int x1 = x0 + 1;
     int y0 = floor(y);
     int y1 = y0 + 1;
 
     // Determine interpolation weights
     // Could also use higher order polynomial/s-curve here
     float sx = x - (float)x0;
     float sy = y - (float)y0;
 
     // Interpolate between grid point gradients
     float n0, n1, ix0, ix1, value;
     n0 = dotGridGradient(x0, y0, x, y);
     n1 = dotGridGradient(x1, y0, x, y);
     ix0 = lerp(n0, n1, sx);
     n0 = dotGridGradient(x0, y1, x, y);
     n1 = dotGridGradient(x1, y1, x, y);
     ix1 = lerp(n0, n1, sx);
     value = lerp(ix0, ix1, sy);
 
     return value;
 }*/