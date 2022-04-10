public static class SystemBaseExt
{
    public static string FormatCoin(this int self)
    {
        float value = self;
        if (value < 1000)
        {
            return value.ToString("f0");// + "$";
        }
        value /= 1000f;
        if (value < 1000)
        {
            var str = value.ToString("#.##");
            return str + "K";
        }
        value /= 1000f;
        if (value < 1000)
        {
            var str = value.ToString("#.##");
            return str + "B";
        }
        value /= 1000f;
        if (value < 1000)
        {
            var str = value.ToString("#.##");
            return str + "T";
        }
        value /= 1000f;
        if (value < 1000)
        {
            var str = value.ToString("#.##");
            return str + "q";
        }
        value /= 1000f;
        {
            var str = value.ToString("#.##");
            return str + "Q";
        }
    }
}
