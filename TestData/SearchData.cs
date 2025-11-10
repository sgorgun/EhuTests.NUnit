using System.Collections;

namespace EhuTests.NUnit.TestData;

public static class SearchData
{
    public static IEnumerable Terms
    {
        get
        {
            yield return "study programs";
            yield return "humanities";
        }
    }
}
