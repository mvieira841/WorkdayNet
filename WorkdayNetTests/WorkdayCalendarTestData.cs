using System.Collections;
using System.Globalization;

namespace WorkdayNetTests;

internal class WorkdayCalendarTestData : IEnumerable<object[]>
{
    public const string Format = "dd-MM-yyyy HH:mm";
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { GetDateTime("24-05-2004 18:05"), -5.5, GetDateTime("14-05-2004 12:00") };
        yield return new object[] { GetDateTime("24-05-2004 19:03"), 44.723656, GetDateTime("27-07-2004 13:47") };
        yield return new object[] { GetDateTime("24-05-2004 18:03"), -6.7470217, GetDateTime("13-05-2004 10:02") };
        yield return new object[] { GetDateTime("24-05-2004 08:03"), 12.782709, GetDateTime("10-06-2004 14:18") };
        yield return new object[] { GetDateTime("24-05-2004 07:03"), 8.276628, GetDateTime("04-06-2004 10:12") };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private DateTime GetDateTime(string dtString)
    {
        return DateTime.ParseExact(dtString, Format, CultureInfo.InvariantCulture);
    }
}