using System.Net.Http.Json;
using WebStore.Domain.Enums;
using WebStore.WebApiClients.Base;

namespace WebStore.WebApiClients.Common.RequestSender;

public class RequestSender: IRequestSender
{
    private readonly Uri _baseAddress; 
    
    public RequestSender(Uri baseAddress)
    {
        _baseAddress = baseAddress;
    }

    public async Task<HttpResponseMessage> GetResponse(
        string apiUrl,
        RequestType requestType, 
        string? urlExtension = null, 
        int? requestId = null,
        string? requestValue = null)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = _baseAddress;
        var requestAddress = $"{_baseAddress}{apiUrl}";
        var client = new ApiClient(httpClient, requestAddress);

        var requestUrl = $"{client.Address}{urlExtension}";

        switch (requestType)
        {
            case RequestType.Get:
                return requestId == null
                    ? await client.HttpClient.GetAsync(requestUrl)
                    : await client.HttpClient.GetAsync($"{requestUrl}/{requestId}");

            case RequestType.Post:
                return await client.HttpClient.PostAsJsonAsync(requestUrl, requestValue);

            case RequestType.Put:
                return await client.HttpClient.PutAsJsonAsync($"{requestUrl}/{requestId}", requestValue);

            case RequestType.Delete:
                return await client.HttpClient.DeleteAsync($"{requestUrl}/{requestId}");

            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
    }
}