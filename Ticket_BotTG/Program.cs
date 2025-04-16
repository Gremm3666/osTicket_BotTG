using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Ticket_BotTG
{
    class Program
    {
        private const string OSTicketApiUrl = "http://osticket.tuning-admina.ru/scp/login.php";
        private const string OsticketApiKey = "9C1EF878C492564F701B3DDD0D83C0CA";

        private const string TelegramBotToken = "7619756591:AAFLQBLsFyt7ePiVfvO8SJEdXWtmw0JBHBY";
        private const string TelegramChatId = "-1002241000251";

        //http://osticket.tuning-admina.ru/scp/login.php
        //http://osticket.tuning-admina.ru
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
                //Console.WriteLine($"Subject: {content}");
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
                              $"ID: {ticket.Id}\n";
                //$"Тема: {ticket.Subject}\n" +
                //$"Пользователь: {ticket.Requester}\n" +
                //$"Сообщение: {ticket.Message}";

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
        //public string Subject { get; set; }
        //public string Requester { get; set; }
        //public string Message { get; set; }
    }
}
