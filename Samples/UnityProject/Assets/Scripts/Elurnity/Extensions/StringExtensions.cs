using System;

namespace Elurnity
{
    public static class MiscExtensions
    {
        public static bool TryParseEnum<T>(this string input, out T value) where T : struct
        {
            if (Enum.IsDefined(typeof(T), input))
            {
                value = (T)Enum.Parse(typeof(T), input);
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }
    }
}
