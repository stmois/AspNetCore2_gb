using System.Net.Http.Json;
using WebStore.Domain.Enums;
using WebStore.WebApiClients.Common.RequestSender;

namespace WebStore.WebApiClients.Values;

public class ValuesClientService : IValuesClientService
{
    private const string CLIENT_URL = "api/values"; 
    private readonly IRequestSender _requestSender;

    public ValuesClientService(IRequestSender requestSender)
    {
        _requestSender = requestSender;
    }

    public async Task<string[]?> GetValuesAsync()
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Get); 

        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<string[]>() 
            : null;
    }

    public async Task<int> CountAsync()
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Get, 
            urlExtension: "/count");
        
        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<int>() 
            : -1;
    }

    public async Task<string?> GetByIdAsync(int id)
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Get, 
            requestId: id);

        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<string>() 
            : null;
    }

    public async Task AddAsync(string value)
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Post, 
            requestValue: value);

        response.EnsureSuccessStatusCode();
    }

    public async Task EditAsync(int id, string value)
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Put, 
            requestValue: value, 
            requestId: id);

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteAsync(int id) 
    {
        var response = await _requestSender.GetResponse(
            apiUrl: CLIENT_URL,
            requestType: RequestType.Delete,
            requestId: id);

        return response.IsSuccessStatusCode;
    }
}