using System.Collections.Generic;
using VkNet.Model.RequestParams;

namespace LiteratureBot.classes
{
    public delegate List<MessagesSendParams> callback(VkNet.Model.Message message, Thread thread); // Делегат функции для выполнения команды

    public class Command
    {
        public string name; // Название команды
        public string description; // Описание команды
        public callback cb { get; } // Функция команды

        // Конструктор
        public Command(string _name, string _description, callback _cb)
        {
            name = _name;
            description = _description;
            cb = _cb;
        }
    }
}
