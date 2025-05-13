using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using Telegram.Bot.Types;
using Newtonsoft.Json;

class Program
{
    private static readonly string OsTicketUrl = "URL API OSTicket";
    private static readonly string OsTicketApiKey = "API ключ для OSTicket";

    private static readonly string TelegramBotToken = "Токен Вашего Telegram-бота";
    private static readonly string TelegramChatId = "ID чата для отправки уведомлений";

    private static DateTime lastCheckedDate = DateTime.UtcNow.AddMinutes(-5);

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting osTicket to Telegram notifier...");

        while (true)
        {
            try
            {
                await CheckNewTickets();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // проверка каждые 5 минут
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    static async Task CheckNewTickets()
    {
        Console.WriteLine($"Checking for new tickets since {lastCheckedDate}...");

        // список тикетов
        var tickets = await GetTicketsFromOsTicket();

        // фильтрация только новых тикетов
        var newTickets = tickets.Where(t => t.Created > lastCheckedDate).ToList();

        if (newTickets.Any())
        {
            Console.WriteLine($"Found {newTickets.Count} new tickets");

            foreach (var ticket in newTickets.OrderBy(t => t.Created))
            {
                await SendTelegramNotification(ticket);
            }

            // обновление даты последней проверки
            lastCheckedDate = newTickets.Max(t => t.Created);
        }
        else
        {
            Console.WriteLine("No new tickets found");
        }
    }

    static async Task<List<OsTicket>> GetTicketsFromOsTicket()
    {
        using var httpClient = new HttpClient();

        // URL для API osTicket
        var url = $"{OsTicketUrl}/tickets/list?apikey={OsTicketApiKey}";

        // отправка GET запрос
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        // ответ
        var responseContent = await response.Content.ReadAsStringAsync();

        // парсинг JSON ответ
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<OsTicketApiResponse>(responseContent, options);

        if (result?.Data == null)
        {
            throw new Exception("Invalid response from osTicket API");
        }

        return result.Data;
    }

    static async Task SendTelegramNotification(OsTicket ticket)
    {
        using var httpClient = new HttpClient();

        // сообщение
        var message = $"🚨 <b>Новый тикет #{ticket.Number}</b>\n" +
                      $"📌 <b>Тема:</b> {ticket.Subject}\n" +
                      $"👤 <b>От:</b> {ticket.Name} ({ticket.Email})\n" +
                      $"📅 <b>Создан:</b> {ticket.Created.ToString("g", CultureInfo.GetCultureInfo("ru-RU"))}\n" +
                      $"🔗 <b>Ссылка:</b> {OsTicketUrl}/scp/tickets.php?id={ticket.Number}";

        // URL для Telegram API
        var url = $"https://api.telegram.org/bot{TelegramBotToken}/sendMessage";

        // создаение JSON тело запроса
        var requestBody = new
        {
            chat_id = TelegramChatId,
            text = message,
            parse_mode = "HTML",
            disable_web_page_preview = true
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // отправка POST запроса
        var response = await httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine($"Notification sent for ticket #{ticket.Number}");
    }
}

// парсинг JSON ответа от osTicket
public class OsTicketApiResponse
{
    public List<OsTicket> Data { get; set; }
}

public class OsTicket
{
    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonIgnore]
    public DateTime Created { get; private set; }

    [JsonPropertyName("created")]
    public string CreatedString
    {
        set
        {
            if (DateTime.TryParse(value, out var date))
            {
                Created = date;
            }
        }
    }
}
