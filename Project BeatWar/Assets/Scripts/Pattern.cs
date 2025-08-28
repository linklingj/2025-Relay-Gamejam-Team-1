[System.Flags]
public enum Pattern : byte
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
    D = 1 << 3,
    All = A | B | C | D,
}

public static class PatternUtils
{
    public static Pattern ToPattern(this int index)
    {
        return (Pattern)(1 << index);
    }

    public static bool HasPattern(this Pattern pattern, int index)
    {
        return (((byte)pattern >> index) & 1) == 1;
    }

    public static int Count(this Pattern pattern)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (pattern.HasPattern(i))
            {
                count++;
            }
        }
        return count;
    }

    public static Pattern Random(this int count)
    {
        return count switch
        {
            0 => Pattern.None,
            1 => UnityEngine.Random.Range(0, 4).ToPattern(),
            2 => FindTwo(),
            3 => Pattern.All & ~UnityEngine.Random.Range(0, 4).ToPattern(),
            _ => Pattern.All
        };
        static Pattern FindTwo()
        {
            int one = UnityEngine.Random.Range(0, 4);
            int two = UnityEngine.Random.Range(0, 3);
            if (two >= one)
            {
                two++;
            }
            return one.ToPattern() | two.ToPattern();
        }
    }
}