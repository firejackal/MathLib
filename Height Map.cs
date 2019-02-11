using System;

namespace MathLib
{
    public static class HeightMapTools
    {
        public static float[,] MakeRandomMap(int width, int height, Random seed)
        {
            float[,] results = new float[width, height];

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    results[x, y] = (float)(seed.NextDouble());
                } //next x
            } //next y

            return results;
        } //MakeRandomMap Function

        public static float[,] MakePerlinNoiseMap(int width, int height, Noise.PerlinNoise perlin, float smoothness = 10.0F, int iterations = 4)
        {
            float[,] results = new float[width, height];

            float smoothnessx = smoothness * width / 40;
            float smoothnessy = smoothness * height / 40;
            int amplitude = 0;
            float n = 0.0f;
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    n = 0.0f;
                    amplitude = 2;
                    // Generate the terrain
                    // 'n' is the height value of the terrain
                    for(int i = 0; i < iterations; i++) {
                        n = n + ((float)perlin.Noise(x / smoothnessx * amplitude, y / smoothnessy * amplitude) + 1.0f) / (float)amplitude;
                        amplitude *= 2;
                    } //next i

                    results[x, y] = n;
                } //next x
            } //next y

            return results;
        } //MakeNoiseMap Function

        public static float[,] MakeCloudsMap(int width, int height, Noise.PerlinNoise perlin, int iterations = 4)
        {
            float[,] results = new float[width, height];
                    
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    results[x, y] = (float)perlin.Noise(x, y, iterations, 1 / width, 1 / height);
                } //next x
            } //next y

            return results;
        } //MakeClouds Function

        public static float[,] MakeSmoothedMap(float[,] map, int averageRange, bool wrap)
        {
            if(map == null || map.GetUpperBound(0) == 0 || map.GetUpperBound(1) == 0) return null;
                    
            float[,] results = new float[map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1];
            SmoothMap(map, ref results, averageRange, wrap);

            return results;
        } //MakeSmoothedMap Function

        public static void SmoothMap(ref float[,] map, int averageRange, bool wrap)
        {
            if(map == null) return;
            float[,] old = CloneMap(map);
            SmoothMap(old, ref map, averageRange, wrap);
        } //SmoothMap Function

        public static void SmoothMap(float[,] map, ref float[,] target, int averageRange, bool wrap)
        {
            if(map == null || target == null) return;
                    
            for(int y = 0; y <= map.GetUpperBound(1); y++) {
                for(int x = 0; x <= map.GetUpperBound(0); x++) {
                    if(x <= target.GetUpperBound(0) && y <= target.GetUpperBound(1)) {
                        target[x, y] = CalculateAverage(map, x, y, averageRange, wrap);
                    }
                } //next x
            } //next y
        } //SmoothMap Function

        public static float CalculateAverage(float[,] map, int left, int top, int range, bool wrap)
        {
            if(map == null) return 0.0f;
            int xUpper = map.GetUpperBound(0);
            int yUpper = map.GetUpperBound(1);

            int count = 0;
            float max = 0.0f;

            int realX = 0, realY = 0;
            for(int yOffset = top - range; yOffset <= top + range; yOffset++) {
                for(int xOffset = left - range; xOffset <= left + range; xOffset++) {
                    // check bounds
                    if(wrap || (!wrap && xOffset >= 0 && xOffset <= xUpper && yOffset >= 0 && yOffset <= yUpper)) {
                        realX = SuperMod(xOffset, xUpper);
                        realY = SuperMod(yOffset, yUpper);

                        count += 1;
                        max += map[realX, realY];
                    }
                } //next xOffset
            } //next yOffset

            return (count == 0 ? 0.0f : max / (float)count);
        } //CalculateAverage Function

        public static void CutOff(ref float[,] map, float min, float max, float newValue)
        {
            if(map == null) return;
            
            for(int y = 0; y <= map.GetUpperBound(1); y++) {
                for(int x = 0; x <= map.GetUpperBound(0); x++) {
                    if(map[x, y] >= min && map[x, y] <= max) map[x, y] = newValue;
                } //next x
            } //next y
        } //CutOffAndAdjust Function

        public static void Adjust(ref float[,] map, float oldMin, float oldMax, float newMin, float newMax, bool cutOff = false)
        {
            if(map == null) return;
            
            for(int y = 0; y <= map.GetUpperBound(1); y++) {
                for(int x = 0; x <= map.GetUpperBound(0); x++) {
                    if(map[x, y] >= oldMin && map[x, y] <= oldMax) {
                        map[x, y] = newMin + (((map[x, y] - oldMin) / (oldMax - oldMin)) * (newMax - newMin));
                    } else if(cutOff) {
                        map[x, y] = 0.0F;
                    }
                } //next x
            } //next y
        } //CutOffAndAdjust Function

        public static void WrapMap(ref float[,] map, int size)
        {
            if(map == null || map.GetUpperBound(0) == 0 || map.GetUpperBound(1) == 0) return;

            for(int x = 0; x <= map.GetUpperBound(0); x += size) {
                for(int y = 0; y <= map.GetUpperBound(1); y += size) {
                    if((x % size) != 0 || (y % size) != 0) {
                        int ax = x - x % size;
                        int bx = (ax + size) % map.GetUpperBound(0);
                        int ay = y - y % size;
                        int by = (ay + size) % map.GetUpperBound(1); // wrap-around
                        float h = (x % size) / size; // horizontal balance, floating-point calculation
                        float v = (y % size) / size; // vertical balance, floating-point calculation
                        map[x, y] = map[ax, ay] * (1 - h) * (1 - v) + map[bx, ay] * h * (1 - v) + map[ax, by] * (1 - h) * v + map[bx, by] * h * v;
                    }
                } //next y
            } //next x
        } //WrapMap Function

        public static void MakeIsland(ref float[,] map)
        {
            if(map == null || map.GetUpperBound(0) == 0 || map.GetUpperBound(1) == 0) return;

            for(int y = 0; y <= map.GetUpperBound(1); y++) {
                for(int x = 0; x <= map.GetUpperBound(0); x++) {
                    // Make the height value go to 0 near the edge
                    map[x, y] *= (1.0f - System.Math.Abs(((float)x / (float)(map.GetUpperBound(0) + 1) * 2.0f) - 1.0f))
                        * (1.0f - System.Math.Abs(((float)y / (float)(map.GetUpperBound(1) + 1) * 2.0f) - 1.0f));
                } //next x
            } //next y
        } //MakeIsland Function

        public static bool GetMapRange(float[,] map, out float outX, out float outY)
        {
            outX = 0.0f;
            outY = 0.0f;

            if(map == null) return false;
            
            outX = map[0, 0];
            outY = map[0, 0];

            for(int y = 0; y <= map.GetUpperBound(1); y++) {
                for(int x = 0; x <= map.GetUpperBound(0); x++) {
                    if(map[x, y] < outX) outX = map[x, y];
                    if(map[x, y] > outY) outY = map[x, y];
                } //next x
            } //next y

            return true;
        } //GetMapRange Function

        public static float[,] CloneMap(float[,] map) { return (float[,])map.Clone(); }

        private static float SuperMod(float value, float max)
        {
            return (value < 0.0 ? (max - (System.Math.Abs(value) % max)) : (value % max));
        } //SuperMod Function
        
        private static int SuperMod(int value, int max)
        {
            return (value < 0 ? (max - (System.Math.Abs(value) % max)) : (value % max));
        } //SuperMod Function
    } //HeightMapTools Class
} //MathLib Namespace
