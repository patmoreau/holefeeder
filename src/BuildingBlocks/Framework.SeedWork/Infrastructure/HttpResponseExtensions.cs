using System.Net;
using System.Net.Http;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure
{
    public static class HttpResponseExtensions
    {
        public static HttpResponseMessage StatusCodeShouldBeSuccess(this HttpResponseMessage self)
        {
            if (self.IsSuccessStatusCode)
            {
                return self;
            }

            if (self.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException(
                    $"Status Code: {self.StatusCode}  ({self.ReasonPhrase}){System.Environment.NewLine}{self.Content.ReadAsStringAsync().Result}");
            }

            throw new HttpRequestException($"Status Code: {self.StatusCode} ({self.ReasonPhrase})");
        }
    }
}
