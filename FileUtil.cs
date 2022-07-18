namespace QPlixInvestment
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    static class FileUtil
    {
        public static List<List<string>> ParseLines(string filePath, char separator = ';', int skipLines = 1)
        {
            return File.ReadAllLines(filePath, Encoding.UTF8)
                .Skip(skipLines)
                .Select(line => line.Split(separator, StringSplitOptions.TrimEntries).ToList())
                .ToList();
        }
    }
}
