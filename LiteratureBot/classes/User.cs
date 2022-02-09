using System;
using System.Collections.Generic;
using System.Linq;
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
                    keyboard = CreateKeyboard(new List<string> { "Да" });
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
                    keyboard = CreateKeyboard(new List<string> { "5", "4", "3", "2", "1" });
                    thread.step++;
                    break;
                case 1:
                    if (new List<string> { "1", "2", "3", "4", "5" }.Contains(message.Text))
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
                            keyboard = CreateKeyboard(new List<string> { "5", "4", "3", "2", "1" });
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
