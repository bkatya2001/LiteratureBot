using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace LiteratureBot.classes
{
    // Абстрактный класс пользователя
    public abstract class BaseUser
    {
        protected List<Command> commands; // Список доступных команд
        protected List<Thread> threads; // Список неоконченных диалогов

        // Конструктор
        public BaseUser()
        {
            commands = CreateCommands();
            threads = new List<Thread>();
        }

        // Метод для создания списка команд
        protected abstract List<Command> CreateCommands();

        // Метод обработки сообщения
        public List<MessagesSendParams> PerformCommand(VkNet.Model.Message message)
        {
            // Проверка на наличие неоконченных диалогов у пользователя
            foreach (Thread thread in threads)
            {
                if (thread.id == message.FromId) return thread.command.cb(message, thread);
            }

            // Создание нового диалога и обработка
            foreach (Command command in commands)
            {
                if (command.name.ToLower() == message.Text.ToLower())
                {
                    Thread thread = new Thread(command, message.FromId);
                    threads.Add(thread);
                    return command.cb(message, thread);
                }
            }

            // Необработанное сообщение
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            if (message.Text != Bot.password)
            {
                messages.Add(new MessagesSendParams
                {
                    UserId = message.FromId,
                    Message = "Команда не найдена.\n\n" + CommandsToListString(),
                    Attachments = null,
                    Keyboard = CreateKeyboard(CommandsToList())
                });
            }
            else messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = "Добро пожаловать, Администратор!\n\n" + CommandsToListString(),
                Attachments = null,
                Keyboard = CreateKeyboard(CommandsToList())
            });
            return messages;
        }

        // Формирование клавиатуры по списку названий кнопок
        public MessageKeyboard CreateKeyboard(List<string> buttons)
        {
            KeyboardBuilder kb = new KeyboardBuilder(true);
            if (buttons.Count > 40) return null;
            else
            {
                int rowCount = buttons.Count / 10;
                if (buttons.Count % 10 != 0) rowCount++;
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (i != 0 && i % rowCount == 0) kb.AddLine();
                    if (buttons[i].ToLower() == "ознакомиться" || buttons[i].ToLower() == "да" || buttons[i].ToLower() == "подобрать произведение") kb.AddButton(buttons[i], null, KeyboardButtonColor.Positive, "text");
                    else if (buttons[i].ToLower() == "нет" || buttons[i].ToLower() == "отменить") kb.AddButton(buttons[i], null, KeyboardButtonColor.Negative, "text");
                    else kb.AddButton(buttons[i], null);
                }
            }
            return kb.Build();
        }

        // Метод для отправки пользователю списка команд
        protected List<MessagesSendParams> GetListOfCommands(VkNet.Model.Message message, Thread thread)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            messages.Add(new MessagesSendParams
            {
                UserId = message.FromId,
                Message = CommandsToListString(),
                Attachments = null,
                Keyboard = CreateKeyboard(CommandsToList())
            }); ;
            threads.Remove(thread);
            return messages;
        }

        // Метод формирования текстового списка команд
        public List<string> CommandsToList()
        {
            List<string> result = new List<string>();
            foreach (var command in commands)
            {
                result.Add(command.name);
            }
            return result;
        }

        // Метод для получения списка команд одной строкой
        protected string CommandsToListString()
        {
            string result = "Список команд:\n\n";
            for (int i = 0; i < commands.Count; i++) result += (i + 1).ToString() + ") " + commands[i].name + " - " + commands[i].description + ";\n";
            return result;
        }

        // Метод определения числа
        protected bool IsNumber(string num)
        {
            try
            {
                int result = Convert.ToInt32(num);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Метод для формирования нескольких сообщений
        protected List<MessagesSendParams> GetSomeMessages(List<string> strings, long? id, MessageKeyboard keyboard)
        {
            List<MessagesSendParams> messages = new List<MessagesSendParams>();
            StringBuilder text = new StringBuilder();
            int count = 0;
            foreach (string s in strings) {
                if ((count + s.Length) < 4096)
                {
                    text.AppendLine(s);
                    count += s.Length;
                }
                else
                {
                    messages.Add(new MessagesSendParams
                    {
                        UserId = id,
                        Message = text.ToString(),
                        Attachments = null,
                        Keyboard = null
                    });
                    text.Clear();
                    count = 0;
                    text.AppendLine(s);
                    count += s.Length;
                }
            }
            messages.Add(new MessagesSendParams
            {
                UserId = id,
                Message = text.ToString(),
                Attachments = null,
                Keyboard = null
            });
            messages[messages.Count - 1].Keyboard = keyboard;
            return messages;
        }
    }
}
