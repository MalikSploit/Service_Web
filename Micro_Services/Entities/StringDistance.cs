using System.Diagnostics;

namespace Entities;

// Thank to https://www.csharpstar.com/csharp-string-distance-algorithm/
/*
    * Class: StringDistance
    * -----------------------
    * This class is the context for the BookService database. It contains a
    * DbSet of Book objects.
    * We don't use this class in the project, because its search only 1 word
    * and not the title of the book. (For the moment).
*/
public static class StringDistance
{
    /// <summary>
    /// Compute the distance between two strings.
    /// </summary>
    public static int LevenshteinDistance(this string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        // Step 7
        int r = d[n, m];
        return r;
    }
}