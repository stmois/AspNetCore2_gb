using System.Net.Http.Json;
using WebStore.Domain.Enums;

namespace WebStore.WebApiClients.Common.RequestSender;

public class RequestSender: IRequestSender
{
    private readonly HttpClient _client; 

    public RequestSender(Uri baseAddress)
    {
        _client = new HttpClient
            {
                BaseAddress = baseAddress
            };
    }

    public async Task<HttpResponseMessage> GetResponse(
        string apiUrl,
        RequestType requestType, 
        string? urlExtension = null, 
        int? requestId = null,
        string? requestValue = null)
    {

        var requestAddress = $"{_client.BaseAddress}{apiUrl}";

        if (!string.IsNullOrWhiteSpace(urlExtension))
        {
            requestAddress = $"{requestAddress}/{urlExtension}";
        }
        
        switch (requestType)
        {
            case RequestType.Get:
                return requestId == null
                    ? await _client.GetAsync(requestAddress)
                    : await _client.GetAsync($"{requestAddress}/{requestId}");

            case RequestType.Post:
                return await _client.PostAsJsonAsync(requestAddress, requestValue);

            case RequestType.Put:
                return await _client.PutAsJsonAsync($"{requestAddress}/{requestId}", requestValue);

            case RequestType.Delete:
                return await _client.DeleteAsync($"{requestAddress}/{requestId}");

            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
    }
}