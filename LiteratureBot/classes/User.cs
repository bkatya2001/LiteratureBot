using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace LiteratureBot.classes
{
    class User : BaseUser
    {
        // Конструктор
        public User() : base() { }

        protected override List<Command> CreateCommands()
        {
            List<Command> commands = new List<Command>
            {
                new Command("Подобрать произведение", "Опишите, о чём Вы хотите прочитать, а мы постараемся выбрать наиболее подходящие варианты", GetLiterature),
                new Command("История", "Посмотрите все запросы, которые Вы направляли", GetRequestsHistory),
                new Command("Очистить историю", "Удалите историю поиска", DeleteHistory),
                new Command("Оценить", "Оцените качество работы бота", RateSystem),
                new Command("Список команд", "Ознакомьтесь с доступными Вам командами", GetListOfCommands)
            };
            return commands;
        }

        private List<MessagesSendParams> GetLiterature(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            MessageKeyboard keyboard = null;
            StringBuilder text = new StringBuilder();
            WebClient webClient = new WebClient();
            System.Collections.ObjectModel.ReadOnlyCollection<VkNet.Model.Attachments.Photo> photo = null;
            switch (thread.step)
            {
                case 0:
                    text.Append("Расскажите, о чём Вы хотите прочитать");
                    keyboard = CreateKeyboard(new List<string> { "Отменить" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text == "Отменить")
                    {
                        text.Append("Команда отменена");
                    }
                    else
                    {
                        Bot.database.UpdateRequestsCount();
                        LanguageProcessor lan_proc = new LanguageProcessor();
                        List<List<Book>> books = lan_proc.ProcessText(message.Text);
                        if (books[0] == null && books[1] == null)
                        {
                            text.Append("По Вашему запросу ничего не найдено.");
                            keyboard = CreateKeyboard(CommandsToList());
                            threads.Remove(thread);
                        }
                        else
                        {
                            if (books[0] == null)
                            {
                                text.AppendLine("По Вашему запросу ничего не найдено. Хотите ознакомиться с похожим?");
                                thread.step++;
                                thread.parametr = books[1];
                                keyboard = CreateKeyboard(new List<string> { "Ознакомиться", "Отменить" });
                            }
                            else
                            {
                                text.AppendLine("Запросу соответствуют следующие произведения:");
                                for (int i = 0; i < books[0].Count; i++)
                                {
                                    text.AppendLine((i + 1).ToString() + ") " + books[0][i].name + " - " + books[0][i].author);
                                    keyboard = CreateKeyboard(CommandsToList());
                                }
                                // 190981839
                                var uploadServer = Bot.vkApi.Photo.GetMessagesUploadServer(190981839);
                                var responseFile = Encoding.ASCII.GetString(webClient.UploadFile(uploadServer.UploadUrl, Bot.database.ConvertBytesInPhoto(books[0][0].photo)));
                                photo = Bot.vkApi.Photo.SaveMessagesPhoto(responseFile);
                                if (books[1].Count != 0)
                                {
                                    text.AppendLine().AppendLine("Хотите ознакомиться с похожим?");
                                    thread.step++;
                                    thread.parametr = books[1];
                                    keyboard = CreateKeyboard(new List<string> { "Ознакомиться", "Отменить" });
                                }
                                else threads.Remove(thread);
                            }
                        }
                        Bot.database.AddRequest(books, message.Text, message.FromId);
                    }
                    break;
                case 2:
                    if (message.Text == "Отменить")
                    {
                        text.Append("Команда отменена");
                    }
                    else
                    {
                        List<Book> same_books = (List<Book>)thread.parametr;
                        for (int i = 0; i < same_books.Count; i++)
                        {
                            text.AppendLine((i + 1).ToString() + ") " + same_books[i].name + " - " + same_books[i].author);
                        }
                        var uploadServer = Bot.vkApi.Photo.GetMessagesUploadServer(190981839);
                        var responseFile = Encoding.ASCII.GetString(webClient.UploadFile(uploadServer.UploadUrl, Bot.database.ConvertBytesInPhoto(same_books[0].photo)));
                        photo = Bot.vkApi.Photo.SaveMessagesPhoto(responseFile);
                    }
                    threads.Remove(thread);
                    keyboard = CreateKeyboard(CommandsToList());
                    break;
            }
            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = text.ToString(),
                Attachments = photo,
                Keyboard = keyboard
            });
            return messages;
        }

        private List<MessagesSendParams> GetRequestsHistory(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            MessageKeyboard keyboard = CreateKeyboard(CommandsToList());
            List<string> history = Bot.database.GetUserRequestsHistory(message.FromId);
            if (history.Count > 0) messages = GetSomeMessages(history, message.FromId, keyboard);
            else messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = "Информация по Вашим запросам отсутствует",
                Attachments = null,
                Keyboard = keyboard
            });
            threads.Remove(thread);
            return messages;
        }

        private List<MessagesSendParams> DeleteHistory(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            string text = "Что-то пошло не так";
            MessageKeyboard keyboard = null;

            switch (thread.step)
            {
                case 0:
                    text = "Вы уверены, что хотите удалить всю историю запросов? Введите 'Да' для удаления";
                    keyboard = CreateKeyboard(new List<string> { "Да", "Нет" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text.ToLower() == "да")
                    {
                        Bot.database.DeleteRequestsHistoryByVkId(message.FromId);
                        text = "История поиска удалена";
                    }
                    else text = "Команда отменена";
                    threads.Remove(thread);
                    keyboard = CreateKeyboard(CommandsToList());
                    break;
            }

            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = text,
                Attachments = null,
                Keyboard = keyboard
            });
            return messages;
        }

        private List<MessagesSendParams> RateSystem(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            string text = "Что-то пошло не так";
            MessageKeyboard keyboard = null;
            
            switch (thread.step)
            {
                case 0:
                    text = "Введите оценку по шкале от 1 до 5, где 1 - 'Ужасно', а 5 - 'Отлично'";
                    keyboard = CreateKeyboard(new List<string> { "5", "4", "3", "2", "1", "Отменить" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text == "Отменить")
                    {
                        text = "Команда отменена";
                        threads.Remove(thread);
                        keyboard = CreateKeyboard(CommandsToList());
                    }
                    else if (new List<string> { "1", "2", "3", "4", "5" }.Contains(message.Text))
                    {
                        text = "Вы можете дополнить отзыв комментарием. Для этого отправьте любое сообщение";
                        keyboard = CreateKeyboard(new List<string> { "Без комментария" });
                        thread.parametr = message.Text;
                        thread.step++;
                    }
                    else
                    {
                        if (thread.HasLives())
                        {
                            text = "Оценить бота можно по шкале от 1 до 5. Повторите ввод снова";
                            keyboard = CreateKeyboard(new List<string> { "5", "4", "3", "2", "1", "Отменить" });
                        }
                        else
                        {
                            text = "Начните команду заново";
                            threads.Remove(thread);
                            keyboard = CreateKeyboard(CommandsToList());
                        }
                    }
                    break;
                case 2:
                    int rating = Convert.ToInt32(thread.parametr.ToString());
                    Bot.database.AddRating(message.FromId, rating, message.Text);
                    Bot.database.UpdateRating(rating);
                    threads.Remove(thread);
                    text = "Спасибо за Вашу оценку!";
                    keyboard = CreateKeyboard(CommandsToList());
                    break;
            }

            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = text,
                Attachments = null,
                Keyboard = keyboard
            });
            return messages;
        }
    }
}
