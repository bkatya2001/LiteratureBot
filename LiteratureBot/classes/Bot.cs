using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace LiteratureBot.classes
{
    class Bot
    {
        public static VkNet.VkApi vkApi; // API ВКонтакте
        private LongPollServerResponse response; // История событий сообщества
        BackgroundWorker backgroundMessages; // Отдельный поток
        //BotBMSTUDataSet database;
        //Admin admin;
        //User user;
        //Guest guest;
        //static public string password = "password";
        private Random r; // Рандомное значение

        // Конструктор
        public Bot()
        {
            vkApi = new VkNet.VkApi();
            vkApi.RequestsPerSecond = 20; // Ограничение на количество запросов в секунду
            r = new Random();
            //database = new BotBMSTUDataSet();
            backgroundMessages = new BackgroundWorker();
            backgroundMessages.DoWork += new DoWorkEventHandler(DoBackgroundWork); // Обработчик фоновых задач
            //admin = new Admin();
            //user = new User();
            //guest = new Guest();
        }

        // Инициализация бота
        public void StartBot()
        {
            // Авторизация
            if (!vkApi.IsAuthorized)
            {
                Settings scope = Settings.Messages | Settings.Wall | Settings.Friends | Settings.Photos | Settings.Documents | Settings.Offline;      // Приложение имеет доступ к друзьям
                try
                {
                    vkApi.Authorize(new ApiAuthParams
                    {
                        Settings = scope,
                        AccessToken = "51c90eda4c95f2343ea557cc47fdd98246e52f22eb71a3f94efbd0bbec3a8ae2683b36c93942bc3cf6627" // Уникальный токен
                    });
                }
                catch { }
                response = vkApi.Messages.GetLongPollServer(needPts: true); // Данные для подключения к серверу с историей событий
            }

            // Получение списка диалогов
            var conversations = vkApi.Messages.GetConversations(new GetConversationsParams
            {
                StartMessageId = null,
                Offset = 0,
                Count = 200,
                Filter = VkNet.Enums.SafetyEnums.GetConversationFilter.Unanswered
            }).Items;

            foreach (var c in conversations) performCommand(c.LastMessage); // Обработка непрочитанных сообщений
            backgroundMessages.RunWorkerAsync(); // Обработка сообщений онлайн
        }

        // Обработчик команд
        void performCommand(VkNet.Model.Message message)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = "Вы написали: " + message.Text,
                Attachments = null,
                //Keyboard = CreateKeyboard(new List<string> { "Список команд" })
            });
            /*if (database.IsAdmin(message.FromId)) SendMessages(admin.PerformCommand(message));
            else if (database.IsUser(message.FromId)) SendMessages(user.PerformCommand(message));
            else if (message.Text == password)
            {
                database.Add(message.FromId);
                SendMessages(admin.PerformCommand(message));
            }
            else SendMessages(guest.PerformCommand(message));*/
            SendMessages(messages); // Отправка сообщения
        }

        // Отправка сообщений
        private void SendMessages(List<MessagesSendParams> messages)
        {
            foreach (var m in messages)
            {
                try
                {
                    if (vkApi.RequestsPerSecond <= 10)
                    {
                        System.Threading.Thread.Sleep(1000);
                        vkApi.RequestsPerSecond = 20;
                    }
                    m.RandomId = r.Next();
                    vkApi.Messages.Send(m);
                    vkApi.RequestsPerSecond--;
                }
                catch (Exception e) { }
            }
        }

        // Обработчик фоновых событий
        void DoBackgroundWork(object sender, DoWorkEventArgs e)
        {
            // Обновления в личных сообщениях
            var resp = vkApi.Messages.GetLongPollHistory(new MessagesGetLongPollHistoryParams
            {
                Ts = Convert.ToUInt64(response.Ts),
                Pts = Convert.ToUInt64(response.Pts)
            });

            while (true)
            {
                try
                {
                    resp = vkApi.Messages.GetLongPollHistory(new MessagesGetLongPollHistoryParams
                    {
                        Pts = resp.NewPts
                    });
                    foreach (VkNet.Model.Message message in resp.Messages)
                    {
                        if (message.Type == VkNet.Enums.MessageType.Received)
                        {
                            performCommand(message); // Обработка непрочитанных сообщений
                        }
                    }
                    if (backgroundMessages.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    //database.ClearBase();
                    System.Threading.Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("token")) vkApi.RequestsPerSecond = 20;

                }
            }

        }
    }
}
