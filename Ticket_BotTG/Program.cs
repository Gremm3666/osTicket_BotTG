using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ticket_BotTG
{
    class Program
    {
        private const string OSTicketApiUrl = "URL API OSTicket";
        private const string OsticketApiKey = "API ключ для OSTicket";

        private const string TelegramBotToken = "Токен Вашего Telegram-бота";
        private const string TelegramChatId = "ID чата для отправки уведомлений";

        static async Task Main()
        {
            // HTTP клиент
            var osticketClient = new HttpClient();
            osticketClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {OsticketApiKey}");

            var telegramClient = new HttpClient();
            telegramClient.BaseAddress = new Uri("https://api.telegram.org/bot" + TelegramBotToken);

            // список новых тикетов
            var newTickets = await GetNewTickets(osticketClient);

            // уведомления
            foreach (var ticket in newTickets)
            {
                await SendTelegramNotification(telegramClient, ticket);
            }
        }

        private static async Task<Ticket[]> GetNewTickets(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(OSTicketApiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Ticket[]>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении тикетов: {ex.Message}");
                return new Ticket[0];
            }
        }

        private static async Task SendTelegramNotification(HttpClient client, Ticket ticket)
        {
            try
            {
                var message = $"Новый тикет создан!\n" +
                              $"ID: {ticket.Id}\n"+
                              $"Тема: {ticket.Subject}\n" +
                              $"Пользователь: {ticket.Requester}\n" +
                              $"Сообщение: {ticket.Message}";
                
                var response = await client.GetAsync($"sendMessage?chat_id={TelegramChatId}&text={Uri.EscapeDataString(message)}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке уведомления: {ex.Message}");
            }
        }
    }

    public class Ticket
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Requester { get; set; }
        public string Message { get; set; }
    }
}
