using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteratureBot.classes
{
    class Book
    {
        public string author;
        public string name;
        public byte[] photo;
        
        public Book(string _name, string _author, byte[] _photo)
        {
            name = _name;
            author = _author;
            photo = _photo;
        }
    }
}
