using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace LiteratureBot.classes
{
    class Admin : BaseUser
    {
        // Конструктор
        public Admin() : base() {}

        protected override List<Command> CreateCommands()
        {
            List<Command> commands = new List<Command>
            {
                new Command("Статистика", "Посмотрите количество пользователей и запросов к боту, а также среднюю оценку за день", GetStatistics),
                new Command("История запросов", "Посмотрите все запросы, которые бот получал от пользователей", GetRequestsHistory),
                new Command("Выйти", "Выйдите из системы под правами администратора", LogOut),
                new Command("Список команд", "Ознакомьтесь с доступными Вам командами", GetListOfCommands)
            };
            return commands;
        }

        // Метод для получения статистических данных
        private List<MessagesSendParams> GetStatistics(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            string text = "Что-то пошло не так";
            MessageKeyboard keyboard = null;
            switch (thread.step)
            {
                case 0:
                    text = "Введите дату в формате ДД.ММ.ГГГГ";
                    keyboard = CreateKeyboard(new List<string> { "Сегодня" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text == "Сегодня")
                    {
                        text = Bot.database.GetStatistics(DateTime.Today.ToShortDateString());
                        keyboard = CreateKeyboard(CommandsToList());
                        threads.Remove(thread);
                    }
                    else
                    {
                        Regex r = new Regex("^[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}$");
                        if (r.IsMatch(message.Text)) text = Bot.database.GetStatistics(r.Match(message.Text).Value);
                        else
                        {
                            if (thread.HasLives())
                            {
                                text = "Неправильный формат даты. Повторите ввод снова";
                                keyboard = CreateKeyboard(new List<string> { "Сегодня" });
                            }
                            else
                            {
                                text = "Начните команду заново";
                                threads.Remove(thread);
                                keyboard = CreateKeyboard(CommandsToList());
                            }
                        }
                    }
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

        private List<MessagesSendParams> GetRequestsHistory(VkNet.Model.Message message, Thread thread)
        {
            threads.Remove(thread);
            return GetSomeMessages(Bot.database.GetRequestsHistory(), message.FromId, CreateKeyboard(CommandsToList()));
        }

        // Выход из системы под правами администратора
        private List<MessagesSendParams> LogOut(VkNet.Model.Message message, Thread thread)
        {
            Bot.database.ChangeStatus(message.FromId, 0);
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            threads.Remove(thread);
            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = "С нетерпением жду Вашего возвращения!",
                Attachments = null,
                Keyboard = CreateKeyboard(new List<string> { "Список команд" })
            });
            return messages;
        }
    }
}
