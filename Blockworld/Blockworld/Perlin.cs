using System;
namespace Perlin
{
    class SimplexNoise {
        int precised = 3;
        double min=0, max=1,scale=1;
        double[] offset;
        Noise[] noises;
        public double Getnoise(double x,double y) {
            x /= scale;
            y /= scale;
            double value=0;
            for (int i = 0; i < precised; i++) {
                value += (1 / offset[i]) * noises[i].GetZerobasednoise(offset[i] * x, offset[i] * y);
            }
            double divby = (2 - Math.Pow(2, -(precised - 1)));
            return value / divby;
        }
        public SimplexNoise(int precised,double min, double max,double scale,int width,int height) {
            this.precised = precised;
            this.max = max;
            this.min = min;
            this.scale = scale;
            offset = new double[precised];
            noises = new Noise[precised];
            for (int i = 0; i < precised; i++) {
                offset[i] = Math.Pow(2, i);
                noises[i] = new Noise(openview.Bigmath.Fix((width / scale)*offset[i]), openview.Bigmath.Fix((height / scale)*offset[i]));
}
        }

    }
    class Noise
    {
        double[,,] gradients;
        public double basevalue;
        public double GetZerobasednoise(double x, double y) => (Perlin(x, y ) * basevalue + 0.5);
        public Noise(int width, int height)
        {
            init(width, height );
        }
        public void init(int width, int height)
        {
 
            double rotation;
            width++; height++;
            basevalue = (0.5*Math.Pow(0.5, 0.5));
            gradients = new double[width, height, 2];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    rotation = Generating.Consts.Tools.doblstn.NextDouble() * (Math.PI * 2);
                    gradients[i, j, 0] = Math.Sin(rotation);//x
                    gradients[i, j, 1] = Math.Cos(rotation);//y
                }
            }
        }
        // Computes the dot product of the distance and gradient vectors.
        double OldDotGridGradient(int ix, int iy, double x, double y)
        {
            // Compute the distance vector
            double dx = x - ix;
            double dy = y - iy;
            // Compute the dot-product from gradients at each grid node
            return (dx * gradients[iy, ix, 0] + dy * gradients[iy, ix, 1]);
        }
        // Compute Perlin noise at coordinates x, y
        public double Perlin(double x, double y)
        {
            //https://www.redblobgames.com/maps/terrain-from-noise/
            // Determine grid cell coordinates
            int x0 = (int)x;
            int x1 = x0 + 1;
            int y0 = (int)y;
            int y1 = y0 + 1;

            // Determine interpolation weights
            // Could also use higher order polynomial/s-curve here
            double sx = x % 1;
            double sy = y % 1;

            // Interpolate between grid point gradients
            double n0, n1, ix0, ix1, value;
            n0 = OldDotGridGradient(x0, y0, x, y);
            n1 = OldDotGridGradient(x1, y0, x, y);
            ix0 = openview.Bigmath.Lerp(n0, n1, sx);
            n0 = OldDotGridGradient(x0, y1, x, y);
            n1 = OldDotGridGradient(x1, y1, x, y);
            ix1 = openview.Bigmath.Lerp(n0, n1, sx);
            value = openview.Bigmath.Lerp(ix0, ix1, sy);
            return value;
        }
    }
}