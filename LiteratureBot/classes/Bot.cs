using LiteratureBot.database;
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
        public static LiteratureBotDataSet database; // Экземпляр класса базы данных
        Admin admin; // Экземпляр класса администраторов
        User user; // Экземпляр класса пользователей
        static public string password = "password"; // Пароль для получения прав администратора
        private Random r; // Рандомное значение
        public static string token;

        // Конструктор
        public Bot()
        {
            vkApi = new VkNet.VkApi();
            vkApi.RequestsPerSecond = 20; // Ограничение на количество запросов в секунду
            r = new Random();
            database = new LiteratureBotDataSet();
            backgroundMessages = new BackgroundWorker();
            backgroundMessages.DoWork += new DoWorkEventHandler(DoBackgroundWork); // Обработчик фоновых задач
            admin = new Admin();
            user = new User();
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
                        AccessToken = token // Уникальный токен
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
            if (message.FromId > 0) 
            {
                if (database.IsAdmin(message.FromId)) SendMessages(admin.PerformCommand(message));
                else
                {
                    database.CheckAndAddUser(message.FromId);
                    if (message.Text == password)
                    {
                        database.ChangeStatus(message.FromId, 1);
                        SendMessages(new List<MessagesSendParams> {
                        new MessagesSendParams {
                            UserId = message.FromId,
                            Message = "Вам доступны функции администратора. Добро пожаловать!",
                            Attachments = null,
                            Keyboard = admin.CreateKeyboard(admin.CommandsToList())
                         }
                    });
                    }
                    else SendMessages(user.PerformCommand(message));
                }
            }
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
