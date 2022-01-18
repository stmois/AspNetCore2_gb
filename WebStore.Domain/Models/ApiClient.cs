namespace WebStore.WebApiClients.Base;

public class ApiClient
{
    public ApiClient(HttpClient httpClient, string address)
    {
        HttpClient = httpClient;
        Address = address;
    }

    public HttpClient HttpClient { get; }

    public string Address { get; }
}