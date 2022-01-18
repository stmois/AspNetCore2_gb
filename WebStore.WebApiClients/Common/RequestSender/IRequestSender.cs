using WebStore.Domain.Enums;

namespace WebStore.WebApiClients.Common.RequestSender;

public interface IRequestSender
{
    Task<HttpResponseMessage> GetResponse(
        string apiUrl,
        RequestType requestType,
        string? urlExtension = null,
        int? requestId = null,
        string? requestValue = null);
}