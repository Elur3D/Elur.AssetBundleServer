using System.IO;

namespace Elurnity
{
    public static class PathExtensions 
    {
        public static string PathCombine(params string[] parts)
        {
            var path = parts[0];
            for (var i = 1; i < parts.Length; ++i)
                path = Path.Combine(path, parts[i]);
            return path;
        }
    }
}