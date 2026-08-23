using System.Text;
using System.Text.Json;

namespace BrewUp.Rest.Tests.Rest;

public class TestClient(HttpClient client)
{
public void AddHeader(string name, string value)
    {
        client.DefaultRequestHeaders.Add(name, value);
    }

    public void ResetHeaders()
    {
        client.DefaultRequestHeaders.Clear();
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        return await InternalSendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, object? content)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        if (content is MultipartFormDataContent dataContent)
        {
            requestMessage.Content = dataContent;
        }
        else if (content is not null)
        {
            var stringJson = JsonSerializer.Serialize(content);
            var httpContent = new StringContent(stringJson, Encoding.UTF8, "application/json");
            requestMessage.Content = httpContent;
        }

        return await InternalSendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, object? content)
    {
        var stringJson = JsonSerializer.Serialize(content);
        var httpContent = new StringContent(stringJson, Encoding.UTF8, "application/json");
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);
        requestMessage.Content = httpContent;
        return await InternalSendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        return await InternalSendAsync(requestMessage);
    }

    private async Task<HttpResponseMessage> InternalSendAsync(HttpRequestMessage request)
    {
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return await client.SendAsync(request);
    }

    public void Dispose()
    {
        client.Dispose();
    }
}