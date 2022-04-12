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
                new Command("Оценки пользователей", "Посмотрите оценки системы от пользователей", GetRating),
                new Command("История запросов", "Посмотрите все запросы, которые бот получал от пользователей", GetRequestsHistory),
                new Command("Список команд", "Ознакомьтесь с доступными Вам командами", GetListOfCommands),
                new Command("Выйти", "Выйдите из системы под правами администратора", LogOut)
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
                    keyboard = CreateKeyboard(new List<string> { "Сегодня", "Отменить" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text == "Отменить")
                    {
                        threads.Remove(thread);
                        text = "Команда отменена";
                        keyboard = CreateKeyboard(CommandsToList());
                    }
                    else
                    {
                        if (message.Text == "Сегодня")
                        {
                            text = Bot.database.GetStatistics(DateTime.Today.ToShortDateString());
                            keyboard = CreateKeyboard(CommandsToList());
                            threads.Remove(thread);
                        }
                        else
                        {
                            Regex r = new Regex("^[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}$");
                            if (r.IsMatch(message.Text))
                            {
                                text = Bot.database.GetStatistics(r.Match(message.Text).Value);
                                keyboard = CreateKeyboard(CommandsToList());
                                threads.Remove(thread);
                            }
                            else
                            {
                                if (thread.HasLives())
                                {
                                    text = "Неправильный формат даты. Повторите ввод снова";
                                    keyboard = CreateKeyboard(new List<string> { "Сегодня", "Отменить" });
                                }
                                else
                                {
                                    text = "Начните команду заново";
                                    threads.Remove(thread);
                                    keyboard = CreateKeyboard(CommandsToList());
                                }
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

        // Метод для просмотра оценок пользователей
        private List<MessagesSendParams> GetRating(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            string text = "Что-то пошло не так";
            MessageKeyboard keyboard = null;
            switch(thread.step)
            {
                case 0:
                    text = "Введите оценки, комментарии к которым Вы хотите посмотреть, в формате '2-3'";
                    keyboard = CreateKeyboard(new List<string> { "Все", "5", "4", "3", "2", "1", "Отменить" });
                    thread.step++;
                    break;
                case 1:
                    if (message.Text == "Отменить")
                    {
                        threads.Remove(thread);
                        text = "Команда отменена";
                    }
                    else
                    {
                        List<string> sentences = new List<string>();
                        if (message.Text == "Все")
                        {
                            sentences = Bot.database.GetRates(1, 5);
                        }
                        else if (message.Text == "5")
                        {
                            sentences = Bot.database.GetRates(5, 5);
                        }
                        else if (message.Text == "4")
                        {
                            sentences = Bot.database.GetRates(4, 4);
                        }
                        else if (message.Text == "3")
                        {
                            sentences = Bot.database.GetRates(3, 3);
                        }
                        else if (message.Text == "2")
                        {
                            sentences = Bot.database.GetRates(2, 2);
                        }
                        else if (message.Text == "1")
                        {
                            sentences = Bot.database.GetRates(1, 1);
                        }
                        else
                        {
                            string[] rates = message.Text.Split('-');
                            if (rates.Length != 2)
                            {
                                if (thread.HasLives())
                                {
                                    text = "Неправильный формат. Повторите ввод снова";
                                    keyboard = CreateKeyboard(new List<string> { "Все", "5", "4", "3", "2", "1", "Отменить" });
                                }
                                else
                                {
                                    text = "Начните команду заново";
                                    threads.Remove(thread);
                                    keyboard = CreateKeyboard(CommandsToList());
                                }
                            }
                            else
                            {
                                try
                                {
                                    int first_rate = Convert.ToInt32(rates[0]);
                                    int second_rate = Convert.ToInt32(rates[1]);
                                    sentences = Bot.database.GetRates(first_rate, second_rate);
                                }
                                catch
                                {
                                    if (thread.HasLives())
                                    {
                                        text = "Неправильный формат. Повторите ввод снова";
                                        keyboard = CreateKeyboard(new List<string> { "Все", "5", "4", "3", "2", "1", "Отменить" });
                                    }
                                    else
                                    {
                                        text = "Начните команду заново";
                                        threads.Remove(thread);
                                        keyboard = CreateKeyboard(CommandsToList());
                                    }
                                }
                            }
                        }
                        if (sentences.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string s in sentences)
                            {
                                sb.AppendLine(s);
                            }
                            text = sb.ToString();
                            keyboard = CreateKeyboard(CommandsToList());
                        }
                    }
                    threads.Remove(thread);
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
