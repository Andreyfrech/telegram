using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("6960682040:AAFy92-r_5bkQ14DZdB875OYotbPhHpe0fI");
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            client.StartReceiving(Update, Error, receiverOptions, cancellationToken);
            

            while (true)
            {
               // Console.ReadLine();
            
            var notificationTime = new TimeSpan(20, 56, 0);

            var thread = new Thread(() =>
            {
                while (true)
                {
                    // Получение текущего времени
                    var currentTime = DateTime.Now.TimeOfDay;

                    // Проверка, совпадает ли текущее время с заданным временем уведомления
                    if (currentTime >= notificationTime && currentTime < notificationTime.Add(new TimeSpan(0, 0, 1))) // Таймстамп 10 секунд
                    {
                        Console.WriteLine("1234");
                        // Отправка уведомления при помощи Telegram.Bot
                        var chatId = 277213277;
                        var message = "Время для уведомления!";
                        Thread.Sleep(100000);
                        client.SendTextMessageAsync(chatId, message).GetAwaiter().GetResult();
                        
                    }
                    Console.WriteLine("Уведомление");
                    // Задержка потока на 1 секунду для экономии ресурсов процессора
                    Thread.Sleep(60000);
                }
            });

            thread.Start();

}
           
        }
      
        public  string SomeWork(object state)
        {

            string urlString = "https://api.telegram.org/bot6960682040:AAFy92-r_5bkQ14DZdB875OYotbPhHpe0fI/sendMessage?chat_id=277213277&text=text";
            WebClient webclient = new WebClient();

            return webclient.DownloadString(urlString);
        }
        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            var chat = message.Chat;
            if (message != null)
            {
                if (message.Text != null)
                {
                    Console.WriteLine($"{message.Chat.FirstName}  |  {message.Text}");
                    if (message.Text.ToLower().Contains("здорова"))
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Здоровей видали");
                        return;
                    }
                }
                if(message.Text == "/start")
                {
                   
                    var replyKeyboard = new ReplyKeyboardMarkup(
                                    new List<KeyboardButton[]>()
                                    {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Привет!"),
                                            new KeyboardButton("Пока!"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Позвони мне!")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Напиши моему соседу!")
                                        }
                                    })
                    {
                        // автоматическое изменение размера клавиатуры, если не стоит true,
                        // тогда клавиатура растягивается чуть ли не до луны,
                        // проверить можете сами
                        ResizeKeyboard = true,
                    };

                    await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Это reply клавиатура!",
                        replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

                    return;
                }
            }
                if (message.Photo != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Нормальное фото, но лучше отправь документом");
                    Console.WriteLine($"{message.Chat.FirstName}  |  фото");
                }
                if (message.Video != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "С видео я работать не умею");
                    Console.WriteLine($"{message.Chat.FirstName}  |  видео");
                }
                if (message.Document != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ща, погодь, сделаю лучше...");


                    var fileId = update.Message.Document.FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;


                    string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";
                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await botClient.DownloadFileAsync(filePath, fileStream);
                    fileStream.Close();

                    var timeStart = DateTime.Now;
                    //Process.Start(@"C:\Users\andre\OneDrive\Рабочий стол\color.exe", $@"""{destinationFilePath}"""); ;
                    await Task.Delay(90000);

                    await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                    await botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, message.Document.FileName.Replace(".JPG", "(edited).jpg")));
                    var timeEnd = DateTime.Now;
                    Console.WriteLine($"{message.Chat.FirstName}  |  файл обработан за {timeEnd.Subtract(timeStart).TotalSeconds} сек");

                }
            }
        }
    }

