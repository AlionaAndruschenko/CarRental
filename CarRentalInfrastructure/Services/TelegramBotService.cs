using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace CarRentalInfrastructure.Services;

public class TelegramBotOptions
{
    public string Token { get; set; } = string.Empty;
}

public class TelegramBotService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public TelegramBotService(HttpClient httpClient, IOptions<TelegramBotOptions> options)
    {
        _httpClient = httpClient;
        _token = options.Value.Token;
    }

    public async Task<bool> SendMessageAsync(string chatId, string text)
    {
        try
        {
            var payload = new { chat_id = chatId, text = text };
            var response = await _httpClient.PostAsJsonAsync(
                $"https://api.telegram.org/bot{_token}/sendMessage",
                payload
            );
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> SendMessageAsync(string chatId, string text, string parseMode = "HTML")
    {
        try
        {
            var payload = new
            {
                chat_id = chatId,
                text = text,
                parse_mode = parseMode 
            };
            var response = await _httpClient.PostAsJsonAsync(
                $"https://api.telegram.org/bot{_token}/sendMessage",
                payload
            );
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}