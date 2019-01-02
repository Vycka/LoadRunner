namespace Viki.LoadRunner.Engine.Utils
{
    public static class ArrayExtensions
    {
        public static bool IsNullOrEmpty<T>(this T[] reference)
        {
            return reference == null || reference.Length == 0;
        }
    }
}