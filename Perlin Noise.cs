// Perlin Noise
// Ported from C# http://lotsacode.wordpress.com/2010/02/24/perlin-noise-in-c/ to VB, back to C#

using System;

namespace MathLib.Noise
{
    public class PerlinNoise
    {
        private int mGradientSize = 256;
        private double[] mGradients;
        private double[,] mDirections; //[item, coord(0=X,1=Y)]
        private byte[] mPermutate; /* = new Byte[] {
        225, 155, 210, 108, 175, 199, 221, 144, 203, 116, 70, 213, 69, 158, 33, 252,
          5, 82, 173, 133, 222, 139, 174, 27, 9, 71, 90, 246, 75, 130, 91, 191, 
        169, 138, 2, 151, 194, 235, 81, 7, 25, 113, 228, 159, 205, 253, 134, 142, 
        248, 65, 224, 217, 22, 121, 229, 63, 89, 103, 96, 104, 156, 17, 201, 129, 
         36, 8, 165, 110, 237, 117, 231, 56, 132, 211, 152, 20, 181, 111, 239, 218, 
        170, 163, 51, 172, 157, 47, 80, 212, 176, 250, 87, 49, 99, 242, 136, 189, 
        162, 115, 44, 43, 124, 94, 150, 16, 141, 247, 32, 10, 198, 223, 255, 72, 
         53, 131, 84, 57, 220, 197, 58, 50, 208, 11, 241, 28, 3, 192, 62, 202, 
         18, 215, 153, 24, 76, 41, 15, 179, 39, 46, 55, 6, 128, 167, 23, 188, 
        106, 34, 187, 140, 164, 73, 112, 182, 244, 195, 227, 13, 35, 77, 196, 185, 
         26, 200, 226, 119, 31, 123, 168, 125, 249, 68, 183, 230, 177, 135, 160, 180,
         12, 1, 243, 148, 102, 166, 38, 238, 251, 37, 240, 126, 64, 74, 161, 40, 
        184, 149, 171, 178, 101, 66, 29, 59, 146, 61, 254, 107, 42, 86, 154, 4, 
        236, 232, 120, 21, 233, 209, 45, 98, 193, 114, 78, 19, 206, 14, 118, 127, 
         48, 79, 147, 85, 30, 207, 219, 54, 88, 234, 190, 122, 95, 67, 143, 109, 
        137, 214, 145, 93, 92, 100, 245, 0, 216, 186, 60, 83, 105, 97, 204, 52};*/

        public PerlinNoise(Random seed, int gradientSize = 256)
        {
            this.mGradientSize = gradientSize;
            this.Populate(seed);

            this.mDirections = new double[this.mGradientSize, 2];
            for(int a = 0; a < this.mGradientSize; a++) {
                this.mDirections[a, 0] = System.Math.Cos(a * 2.0 * System.Math.PI / this.mGradientSize);
                this.mDirections[a, 1] = System.Math.Sin(a * 2.0 * System.Math.PI / this.mGradientSize);
            } //next a
        } //Constructor

        public void Populate(Random seed)
        {
            this.InitializePermutateTable(seed);
            this.InitializeGradients(seed);
        } //Repopulate Function

        private void InitializePermutateTable(Random seed)
        {
            this.mPermutate = new byte[this.mGradientSize];

            for(int i = 0; i < this.mGradientSize; i++) {
                this.mPermutate[i] = System.Convert.ToByte(seed.Next(0, 256));
            } //next i
        } //InitializePermutateTable Function

        private void InitializeGradients(Random seed)
        {
            this.mGradients = new double[this.mGradientSize * 3];

            for(int i = 0; i < this.mGradientSize; i++) {
                double z = 1.0 - 2.0 * seed.NextDouble();
                double r = System.Math.Sqrt(1.0 - z * z);
                double theta = 2.0 * System.Math.PI * seed.NextDouble();
                this.mGradients[i * 3 + 0] = r * System.Math.Cos(theta);
                this.mGradients[i * 3 + 1] = r * System.Math.Sin(theta);
                this.mGradients[i * 3 + 2] = z;
            } //next i
        } //InitializeGradients Function

        /// <summary>The main noise function. Looks up the pseudorandom gradients at the nearest lattice points, dots them with the input vector, and interpolates the results to produce a single output value in [0, 1] range.</summary>
        /// <returns>A value in [0, 1] range.</returns>
        public double Noise(double x, double y, double z)
        {
            int    ix  = System.Convert.ToInt32(System.Math.Floor(x));
            double fx0 = x - ix;
            double fx1 = fx0 - 1;
            double wx  = Smooth(fx0);

            int    iy  = System.Convert.ToInt32(System.Math.Floor(y));
            double fy0 = y - iy;
            double fy1 = fy0 - 1;
            double wy  = Smooth(fy0);

            int    iz  = System.Convert.ToInt32(System.Math.Floor(z));
            double fz0 = z - iz;
            double fz1 = fz0 - 1;
            double wz  = Smooth(fz0);

            double vx0 = Lattice(ix, iy, iz, fx0, fy0, fz0);
            double vx1 = Lattice(ix + 1, iy, iz, fx1, fy0, fz0);
            double vy0 = Lerp(wx, vx0, vx1);

            vx0 = Lattice(ix, iy + 1, iz, fx0, fy1, fz0);
            vx1 = Lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0);
            double vy1 = Lerp(wx, vx0, vx1);

            double vz0 = Lerp(wy, vy0, vy1);

            vx0 = Lattice(ix, iy, iz + 1, fx0, fy0, fz1);
            vx1 = Lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1);
            vy0 = Lerp(wx, vx0, vx1);
            vx0 = Lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1);
            vx1 = Lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1);
            vy1 = Lerp(wx, vx0, vx1);
            double vz1 = Lerp(wy, vy0, vy1);
            return Lerp(wz, vz0, vz1);
        } //Noise Function

        public double Noise(double x, double y) { return this.Noise(x, y, 0.0); }

        public double Noise2(double x, double y, double per)
        {
            int intX = System.Convert.ToInt32(x);
            int intY = System.Convert.ToInt32(y);
            return Surflet(x, y, intX + 0, intY + 0, per) + Surflet(x, y, intX + 1, intY + 0, per) + Surflet(x, y, intX + 0, intY + 1, per) + Surflet(x, y, intX + 1, intY + 1, per);
        } //Noise2 Function

        public double FractalNoise(double x, double y, double alpha, double beta, int octaves)
        {
            double sum = 0.0, val = 0.0;
            double scale = 1.0;

            double newX = x;
            double newY = y;
            for(int i = 0; i < octaves; i++) {
                val = Noise(newX, newY);
                sum += val / scale;
                scale *= alpha;
                newX *= beta;
                newY *= beta;
            } //next i

            return System.Math.Min(1.0, System.Math.Max(0.0, sum));
        } //FractalNoise Function

        public double FractalNoise(double x, double y, double per, int octaves)
        {
            double val = 0.0;

            for(int o = 0; o < octaves; o++) {
                val += System.Math.Pow(0.5, o) * this.Noise2(System.Math.Pow(x * 2, o), System.Math.Pow(y * 2, o), System.Math.Pow(per * 2, o));
            } //next o

            return val;
        } //FractalNoise Function

        public double Noise(double x, double y, int octaves, double widthDivisor, double heightDivisor)
        {
            double v = 0.0;

            int level = 2;
            for(int octave = 1; octave <= octaves; octave++) {
                v += (Noise(level * x * widthDivisor, level * y * heightDivisor, -0.5 + (octave / octaves)) + 1) / 2 * (1.0 - (octave / octaves));
                level *= 2;
            } //next octave

            return System.Math.Min(1.0, System.Math.Max(0.0, v));
        } //Noise Function

        private int Permutate(int x)
        {
            int mask = (this.mGradientSize - 1);
            return this.mPermutate[x & mask];
        } //Permutate Function

        /// <summary>Turn an XYZ triplet into a single gradient table index.</summary>
        private int Index(int ix, int iy, int iz) { return this.Permutate(ix + this.Permutate(iy + this.Permutate(iz))); }

        /// <summary>Turn an XY into a single gradient table index.</summary>
        private int Index(int ix, int iy) { return this.Permutate(ix + this.Permutate(iy)); }

        /// <summary>Look up a random gradient at [ix,iy,iz] and dot it with the [fx,fy,fz] vector.</summary>
        private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
        {
            int g = this.Index(ix, iy, iz) * 3;
            return this.mGradients[g] * fx + this.mGradients[g + 1] * fy + this.mGradients[g + 2] * fz;
        } //Lattice Function

        /// <summary>Look up a random gradient at [ix,iy,iz] and dot it with the [fx,fy,fz] vector.</summary>
        private double Lattice(int ix, int iy, double fx, double fy)
        {
            int g = this.Index(ix, iy) * 3;
            return this.mGradients[g] * fx + this.mGradients[g + 1] * fy;
        } //Lattice Function

        /// <summary>Simple linear interpolation.</summary>
        private double Lerp(double t, double value0, double value1) { return (value0 + t * (value1 - value0)); }

        /// <summary>Smoothing curve. This is used to calculate interpolants so that the noise doesn't look blocky when the frequency is low.</summary>
        private static double Smooth(double x) { return (x * x * (3 - 2 * x)); }

        private double Surflet(double x, double y, double gridX, double gridY, double per)
        {
            double distX  = System.Math.Abs(x - gridX);
            double distY  = System.Math.Abs(y - gridY);
            double polyX  = 1.0 - System.Math.Pow(6.0 * distX, 5.0) + System.Math.Pow(15.0 * distX, 4.0) - System.Math.Pow(10.0 * distX, 3.0);
            double polyY  = 1.0 - System.Math.Pow(6.0 * distY, 5.0) + System.Math.Pow(15.0 * distY, 4.0) - System.Math.Pow(10.0 * distY, 3.0);
            double hashed = this.mPermutate[this.mPermutate[System.Convert.ToInt32(gridX % per)] + System.Convert.ToInt32(gridY % per)];
            double grad   = (x - gridX) * this.mDirections[System.Convert.ToInt32(hashed), 0] + (y - gridY) * this.mDirections[System.Convert.ToInt32(hashed), 1];
            return (polyX * polyY * grad);
        } //Surflet Function
    } //PerlinNoise Class
} //MathLib.Noise Namespace
