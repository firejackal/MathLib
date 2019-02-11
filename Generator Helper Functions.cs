namespace MathLib.Generation
{
    public static class HelperFunctions
    {
        public static float[] Create1DNoiseMapF(int length, System.Random seed)
        {
            // First check for problems with the parameters, if any exit.
            if(length < 1 || seed == null) return null;
            // Create the output array.
            float[] result = new float[length];
            // Fill the output array with random garbage.
            for(int x = 0; x < length; x++) {
                result[x] = System.Convert.ToSingle(seed.NextDouble());
            } //x
            // return the results.
            return result;
        } //Create1DNoiseMapF

        public static float[,] Create2DNoiseMapF(int width, int height, System.Random seed)
        {
            // First check for problems with the parameters, if any exit.
            if(width < 1 || height < 1 || seed == null) return null;
            // Create the output array.
            float[,] result = new float[width, height];
            // Fill the output array with random garbage.
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    result[x, y] = System.Convert.ToSingle(seed.NextDouble());
                } //x
            } // y
            // return the results.
            return result;
        } //Create2DNoiseMapF

        public static float[] CreateAverageMap(float[] array, int start, int end, int range)
        {
            // Create the output array.
            float[] result = new float[array.Length];
            // Go through the desired range.
            for (int x = start; x <= end; x++) {
                result[x] = GetArrayAverage(array, x, range);
            } //x
            // return the results.
            return result;
        } //CreateAverageMap

        public static float GetArrayAverage(float[] array, int position, int range)
        {
            int count = 0;
            float max = 0.0f;
            for(int offset = (position - range); offset <= (position + range); offset++) {
                if(offset >= 0 && offset < array.Length) {;
                    count += 1;
                    max += array[offset];
                }
            } //offset

            if(count == 0)
                return 0.0F;
            else
                return (float)max / (float)count;
        } //GetArrayAverage
    } //HelperFunctions
} // MathLib.Generation namespace
