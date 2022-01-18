using System.Net.Http.Json;
using WebStore.Domain.Enums;
using WebStore.WebApiClients.Base;

namespace WebStore.WebApiClients.Common.RequestSender;

public class RequestSender: IRequestSender
{
    public async Task<HttpResponseMessage> GetResponse(
        string apiUrl,
        RequestType requestType, 
        string? urlExtension = null, 
        int? requestId = null,
        string? requestValue = null)
    {
        var client = new ApiClient(new HttpClient(), apiUrl);
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