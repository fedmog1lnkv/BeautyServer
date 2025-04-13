using Domain.Entities;
using Domain.Repositories.PhoneChallenges;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Repositories.PhoneChallenges;

internal sealed class PhoneChallengeRepository(ApplicationDbContext dbContext, IOptions<TwoFaSettings> twoFaSettings)
    : IPhoneChallengeRepository
{
    private readonly TwoFaSettings _twoFaSettings = twoFaSettings.Value;

    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromSeconds(90)
    };

    public async Task<Domain.Entities.PhoneChallenge?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Domain.Entities.PhoneChallenge>()
            .FirstOrDefaultAsync(pc => pc.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<string?> SendAuthRequestAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            // Создаем объект запроса
            var authRequest = new AuthRequest(phoneNumber);
            var requestBody = JsonConvert.SerializeObject(authRequest);

            // Получаем URL и токен из переменных окружения
            var url = _twoFaSettings.Server;
            var token = _twoFaSettings.Token;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("TELEGRAM_2FA_SERVER is not set.");

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("TELEGRAM_2FA_TOKEN is not set.");

            // Создаем HTTP-запрос
            var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/api/challenge/auth")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("accessToken", token);

            // Отправляем запрос
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Десериализуем ответ
            var responseBody = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

            if (authResponse?.SystemMessage == "Пользователь не зарегистрирован в телеграм-боте")
            {
                return authResponse.SystemMessage;
            }
            
            if (authResponse == null || !authResponse.IsSuccess)
            {
                return null;
            }

            // Возвращаем успешный результат с именем пользователя
            var userName = $"{authResponse.User.FirstName} {authResponse.User.LastName}";
            return userName;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> SendCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Получаем URL и токен из переменных окружения
            var url = _twoFaSettings.Server;
            var token = _twoFaSettings.Token;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("TELEGRAM_2FA_SERVER is not set.");
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("TELEGRAM_2FA_TOKEN is not set.");

            // Формируем сообщение с кодом
            var message = $"Ваш код <b>{code}</b> для входа в приложение. Не передавайте этот код посторонним.";

            // Создаем данные для отправки
            var data = new
            {
                Phone = phoneNumber,
                Message = message
            };

            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Создаем HTTP-запрос
            var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/api/sms/send")
            {
                Content = content
            };

            // Добавляем заголовок с токеном
            request.Headers.Add("accessToken", token);

            // Отправляем запрос
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error sending SMS. Status code: {response.StatusCode}");
            }

            // Обрабатываем ответ
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

            if (responseData != null && responseData.ContainsKey("isSuccess") &&
                responseData["isSuccess"] is bool isSuccess && isSuccess)
            {
                return true; // Успешная отправка
            }
            else
            {
                var systemMessage = responseData?["systemMessage"]?.ToString();
                return false; // Успешная отправка
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public void Add(Domain.Entities.PhoneChallenge phoneChallenge)
    {
        dbContext.Set<Domain.Entities.PhoneChallenge>().Add(phoneChallenge);
        dbContext.SaveChanges();
    }

    public void Remove(Domain.Entities.PhoneChallenge phoneChallenge)
    {
        dbContext.Set<Domain.Entities.PhoneChallenge>().Remove(phoneChallenge);
        dbContext.SaveChanges();
    }
}

public class AuthRequest(string phone)
{
    [JsonProperty("phone")]
    public string Phone { get; set; } = phone;
}

// Класс для пользователя
public class User
{
    [JsonProperty("telegramId")]
    public int TelegramId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
}

// Класс ответа от авторизации
public class AuthResponse
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonProperty("systemMessage")]
    public string SystemMessage { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }
}