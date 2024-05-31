namespace PxGraf
{
    public static class CoverageTestFunctions
    {
        public static int CoveredTestFunction(int input)
        {
            int output = input * 2;
            return output;
        }

        public static int UncoveredTestFunction(int input)
        {
            int output = input * 3;
            return output;
        }
    }
}
