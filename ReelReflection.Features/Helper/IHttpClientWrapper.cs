namespace ReelReflection.Features.Helper;

public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string requestUri);
}
