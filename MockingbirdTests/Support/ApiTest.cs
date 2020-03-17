using System.Net.Http;
using System.Text;

namespace MockingbirdTests.Support
{
    public abstract class ApiTest
    {
        protected static StringContent AsStringContent(string stringPayload)
        {
            return new StringContent(stringPayload, Encoding.UTF8, "application/json");
        }
    }
}