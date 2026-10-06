using System.Net.Http.Headers;
using System.Text.Json;
using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public class GigaChatService : IGigaChatService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GigaChatService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<FlangeModel?> ParseFlangeAsync(string prompt)
    {
        var accessToken = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.giga.chat/v1/chat/completions");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        var body = new
        {
            model = "GigaChat-2-Max",

            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        """
                        Извлеки параметры фланца из описания пользователя.
                        Все размеры должны быть в миллиметрах.
                        Не придумывай отсутствующие значения.
                        """
                },

                new
                {
                    role = "user",
                    content = prompt
                }
            },

            response_format = new
            {
                type = "json_schema",

                schema = new
                {
                    type = "object",

                    properties = new
                    {
                        outerDiameter = new
                        {
                            type = "number"
                        },

                        innerDiameter = new
                        {
                            type = "number"
                        },

                        thickness = new
                        {
                            type = "number"
                        },

                        holeCount = new
                        {
                            type = "integer"
                        },

                        holeDiameter = new
                        {
                            type = "number"
                        },

                        boltCircleDiameter = new
                        {
                            type = "number"
                        }
                    },

                    required = new[]
                    {
                        "outerDiameter",
                        "innerDiameter",
                        "thickness",
                        "holeCount",
                        "holeDiameter",
                        "boltCircleDiameter"
                    },

                    additionalProperties = false
                },

                strict = true
            }
        };

        request.Content =
            JsonContent.Create(body);

        using var response =
            await _httpClient.SendAsync(request);

        var responseText =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"GigaChat error: {responseText}");

        using var document =
            JsonDocument.Parse(responseText);

        var content =
            document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        if (string.IsNullOrWhiteSpace(content))
            return null;

        Console.WriteLine("GigaChat content:");
        Console.WriteLine(content);
        return JsonSerializer.Deserialize<FlangeModel>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var authorizationKey =
            _configuration["GigaChat:AuthorizationKey"];

        if (string.IsNullOrWhiteSpace(authorizationKey))
            throw new InvalidOperationException(
                "GigaChat Authorization Key не найден.");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://ngw.devices.sberbank.ru:9443/api/v2/oauth");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                authorizationKey);

        request.Headers.Add(
            "RqUID",
            Guid.NewGuid().ToString());

        request.Content =
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["scope"] = "GIGACHAT_API_PERS"
                });

        using var response =
            await _httpClient.SendAsync(request);

        var responseText =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Ошибка получения GigaChat token: {responseText}");

        using var document =
            JsonDocument.Parse(responseText);

        return document.RootElement
                   .GetProperty("access_token")
                   .GetString()
               ?? throw new InvalidOperationException(
                   "GigaChat не вернул access_token.");
    }
}