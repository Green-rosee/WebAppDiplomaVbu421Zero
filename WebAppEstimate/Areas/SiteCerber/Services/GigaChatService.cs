using System.Net.Http.Headers;
using System.Text.Json;
using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;


/// <summary>
/// Сервис взаимодействия с GigaChat API.
/// Получает параметры фланца из текстового описания пользователя
/// и преобразует ответ модели в объект <see cref="FlangeModel"/>.
/// </summary>
public class GigaChatService : IGigaChatService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Создаёт сервис для работы с GigaChat API.
    /// </summary>
    /// <param name="httpClient">
    /// HTTP-клиент для отправки запросов к API.
    /// </param>
    /// <param name="configuration">
    /// Конфигурация приложения, содержащая параметры доступа к GigaChat.
    /// </param>
    public GigaChatService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Отправляет текстовое описание фланца в GigaChat
    /// и извлекает из ответа структурированные геометрические параметры.
    /// </summary>
    /// <param name="prompt">
    /// Текстовое описание фланца, введённое пользователем.
    /// </param>
    /// <returns>
    /// Объект <see cref="FlangeModel"/> с распознанными параметрами
    /// либо null, если корректный ответ получить не удалось.
    /// </returns>
    public async Task<FlangeModel?> ParseFlangeAsync(
        string prompt)
    {
        // Получаем OAuth access_token,
        // необходимый для доступа к GigaChat API.
        var accessToken =
            await GetAccessTokenAsync();

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.giga.chat/v1/chat/completions");

        // Передаём access_token в заголовке Authorization.
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        // Формируем запрос к языковой модели.
        // System prompt задаёт правила извлечения параметров,
        // а response_format требует строгий JSON по заданной схеме.
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

        // Сериализуем объект запроса в JSON.
        request.Content =
            JsonContent.Create(body);

        // Отправляем запрос в GigaChat API.
        using var response =
            await _httpClient.SendAsync(request);

        var responseText =
            await response.Content
                .ReadAsStringAsync();

        // При ошибке API возвращаем техническую информацию
        // через исключение.
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"GigaChat error: {responseText}");
        }

        // Извлекаем поле content из ответа chat/completions.
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

        // Преобразуем JSON-ответ модели
        // в объект FlangeModel.
        return JsonSerializer.Deserialize<FlangeModel>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    /// <summary>
    /// Получает временный OAuth access_token
    /// для обращения к GigaChat API.
    /// </summary>
    /// <returns>
    /// Строковое значение access_token.
    /// </returns>
    private async Task<string> GetAccessTokenAsync()
    {
        // Получаем ключ авторизации из конфигурации приложения.
        var authorizationKey =
            _configuration[
                "GigaChat:AuthorizationKey"];

        if (string.IsNullOrWhiteSpace(
                authorizationKey))
        {
            throw new InvalidOperationException(
                "GigaChat Authorization Key не найден.");
        }

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "https://ngw.devices.sberbank.ru:9443/api/v2/oauth");

        // Для получения токена используется Basic-аутентификация.
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                authorizationKey);

        // Уникальный идентификатор запроса.
        request.Headers.Add(
            "RqUID",
            Guid.NewGuid().ToString());

        // Указываем scope доступа к GigaChat API.
        request.Content =
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["scope"] =
                        "GIGACHAT_API_PERS"
                });

        using var response =
            await _httpClient.SendAsync(request);

        var responseText =
            await response.Content
                .ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Ошибка получения GigaChat token: {responseText}");
        }

        // Извлекаем access_token из OAuth-ответа.
        using var document =
            JsonDocument.Parse(responseText);

        return document.RootElement
                   .GetProperty("access_token")
                   .GetString()
               ?? throw new InvalidOperationException(
                   "GigaChat не вернул access_token.");
    }
}











