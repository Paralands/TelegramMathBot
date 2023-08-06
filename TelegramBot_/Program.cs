using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Diagnostics;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_
{
    class Program
    {
        public static string TOKEN = Config.TOKEN;
        public static string PATH = Config.PATH;
        public static string START_TEXT = Config.START_TEXT;
        public static string MATH_TEXT = Config.MATH_TEXT;     

        static List<ChatUser> users;

        static void Main(string[] args)
        {
            users = new List<ChatUser>();
            TelegramBotClient client = new TelegramBotClient(TOKEN);
            client.StartReceiving(UpdateAsync, Error);
      
            Console.ReadLine();
        }

        async static Task UpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Message message = update.Message;

            string messageText;

            if (message == null)
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await BotOnInlineQueryReceived(client, update.CallbackQuery);
                }
                return;
            } 
            else messageText = message.Text;

            ChatUser user;
            var ID = message.Chat.Id;
            var username = message.Chat.Username != null ? message.Chat.Username : (message.Chat.FirstName != null ? message.Chat.FirstName : (message.Chat.LastName != null ? message.Chat.LastName : $"User {ID}"));

            //Adding user to the list
            if (!users.Any(x => x.ID == ID))
                users.AddUser<ChatUser>(new ChatUser(ID, username));
            user = users.Single(x => x.ID == ID);

            if (messageText != null)
            {
                await SaveMessage(message, ID, user.GetDirectory(ID));
                Console.WriteLine($"{username} - {ID}: {messageText}");

                if (messageText.ToLower().Contains("/help"))
                {
                    StartMessage(client, ID);
                    return;
                }

                if(messageText.ToLower().Contains("/mathinfo"))
                {
                    MathMessage(client, ID);
                    return;
                }

                if(messageText.ToLower().Contains("math"))
                {
                    for (int j = 0; j< messageText.Length; j++)
                    {
                        if (MathController.numbers.Contains(messageText.ElementAt(j).ToString()))
                        {
                            try
                            {
                                var answer = MathController.DoMath(messageText).Item1;

                                var list = MathController.GetExpression(messageText);
                                string text = "";
                                for (int i = 0; i < list.Count; i++)
                                    text += list[i];

                                if(MathController.DoMath(messageText).Item2 == "")
                                    await client.SendTextMessageAsync(ID, $"{text}={answer}");
                                else if (text != "")
                                    await client.SendTextMessageAsync(ID, $"{MathController.DoMath(messageText).Item2} Пример: '{text}'");
                                else await client.SendTextMessageAsync(ID, $"{MathController.DoMath(messageText).Item2}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        }
                    }
                    return;
                }
            }
            if (message.Photo != null)
            {
                Console.WriteLine($"{username} - {ID}: /фото/");
                await client.SendTextMessageAsync(ID, "Отправь не сжатое фото");
                return;
            }
            if (message.Document != null)
            {
                await SaveDocument(message, ID, client, user.GetDirectory(ID));
                return;
            }

            StartMessage(client, ID);
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient client, CallbackQuery inlineQuery)
        {
            string messageText = inlineQuery.Data;
            Console.WriteLine(messageText);
            var chat = inlineQuery.Message.Chat;

            if (messageText.ToLower().Contains("math"))
            {
                for (int j = 0; j < messageText.Length; j++)
                {
                    if (MathController.numbers.Contains(messageText.ElementAt(j).ToString()))
                    {
                        try
                        {
                            var answer = MathController.DoMath(messageText).Item1;

                            var list = MathController.GetExpression(messageText);
                            string text = "";
                            for (int i = 0; i < list.Count; i++)
                                text += list[i];

                            if (MathController.DoMath(messageText).Item2 == "")
                                await client.SendTextMessageAsync(chat, $"{text}={answer}");
                            else if (text != "")
                                await client.SendTextMessageAsync(chat, $"{MathController.DoMath(messageText).Item2}. Пример: '{text}'");
                            else await client.SendTextMessageAsync(chat, $"{MathController.DoMath(messageText).Item2}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    }
                }
                return;
            }
        }

        public static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine("ERROR");
            throw new NotImplementedException();
        }


        //Sending the StartMessage
        private async static void StartMessage(ITelegramBotClient client, long id)
        {
            ReplyKeyboardMarkup markup = new(new[]
            {
                new KeyboardButton[] { "/mathinfo", "/help" },
            })
            {
                ResizeKeyboard = true,
                IsPersistent = true,
            };
            await client.SendTextMessageAsync(id, START_TEXT, replyMarkup: markup, parseMode: ParseMode.Html);
        }

        //Sending the MathMessage
        private async static void MathMessage(ITelegramBotClient client, long id)
        {
            InlineKeyboardMarkup markup = new(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData("Сколько будет 2+2?", "Math 2+2") }
            });

            await client.SendTextMessageAsync(id, MATH_TEXT, parseMode: ParseMode.Html, replyMarkup: markup);
        }

        private async static Task SaveMessage(Message message, long id, string directory)
        {
            DateTime now = DateTime.Now;
            string destinationFilePath = $"{directory}\\{now.Day}.{now.Month}.{now.Year}.txt";

            await using (StreamWriter sw = new StreamWriter(destinationFilePath, true, Encoding.UTF8))
            {
                sw.WriteLine($"{message.Date.ToLocalTime().Hour}:{message.Date.ToLocalTime().Minute}:{message.Date.ToLocalTime().Second} - {users.Single(x => x.ID == id).UserName}: {message.Text}");
            }
        }

        private async static Task<string> SaveDocument(Message message, long id, ITelegramBotClient client, string directory)
        {
            DateTime now = DateTime.Now;
            string stringFilePath = $"{directory}\\{now.Day}.{now.Month}.{now.Year}.txt";

            await using (StreamWriter sw = new StreamWriter(stringFilePath, true, Encoding.UTF8))
            {
                sw.WriteLine($"{message.Date.ToLocalTime().Hour}:{message.Date.ToLocalTime().Minute}:{message.Date.ToLocalTime().Second} - {users.Single(x => x.ID == id).UserName}: <document/> {message.Document.FileName}");
            }

            var fileInfo = await client.GetFileAsync(message.Document.FileId);
            string destinationFilePath = $"{directory}\\{message.Document.FileName}";

            using (FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath))
            {
                await client.DownloadFileAsync(fileInfo.FilePath, fileStream);
            }

            return destinationFilePath;
        }
    }
}
