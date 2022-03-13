using Pullenti.Morph;
using Pullenti.Ner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiteratureBot.classes
{
    class LanguageProcessor
    {
        Processor processor; // Создаём экземпляр процессора со стандартными анализаторами
        List<string> adjectives; // Прилагательные
        List<string> nouns; // Прилагательные
        List<string> book_make; // Заставляет делать
        List<string> for_book; // Для кого предназначена книга

        public LanguageProcessor()
        {
            processor = ProcessorService.CreateProcessor();
            adjectives = new List<string>();
            nouns = new List<string>();
            for_book = new List<string>();
            book_make = new List<string>();
        }

        private void GetAdjectives(AnalysisResult result)
        {
            Regex r = new Regex("^\\d+$");
            for (Token t = result.FirstToken; t != null; t = t.Next)
            {
                if (t.Morph.Class.IsAdjective)
                {
                    string adj = t.GetNormalCaseText(MorphClass.Adjective, MorphNumber.Singular, MorphGender.Masculine);
                    if (!r.IsMatch(adj))
                        adjectives.Add(adj);
                }
            }
        }

        private void GetNoun(AnalysisResult result)
        {
            Regex r = new Regex("^\\d+$");
            for (Token t = result.FirstToken; t != null; t = t.Next)
            {
                if (t.Morph.Class.IsNoun)
                {
                    string noun = t.GetNormalCaseText(MorphClass.Noun, MorphNumber.Singular);
                    if (!r.IsMatch(noun))
                        nouns.Add(noun);
                }
            }
        }

        private void GetBookFor(AnalysisResult result)
        {
            string prev_prep = "";
            bool flag = false;
            for (Token t = result.FirstToken; t != null; t = t.Next)
            {
                if (t.Morph.Class.IsPreposition) prev_prep = t.GetNormalCaseText(MorphClass.Preposition, MorphNumber.Singular);
                else
                {
                    if (t.Morph.Class.IsNoun || t.Morph.Class.IsProperSurname)
                    {
                        if (prev_prep == "ДЛЯ")
                        {
                            string s = t.GetNormalCaseText(MorphClass.Noun, MorphNumber.Singular);
                            adjectives.Remove(s);
                            nouns.Remove(s);
                            for_book.Add(s);
                            flag = true;
                            prev_prep = "";
                        }
                        else if (flag == true && (t.Previous.Morph.Class.IsConjunction || t.Previous.Morph.Class.IsNoun))
                        {
                            string s = t.GetNormalCaseText(MorphClass.Noun, MorphNumber.Singular);
                            adjectives.Remove(s);
                            nouns.Remove(s);
                            for_book.Add(s);
                            flag = false;
                        }
                    }
                }
            }

            if (for_book.Contains("МАЛЬЧИК") || for_book.Contains("ДЕВОЧКА"))
            {
                for_book.Remove("МАЛЬЧИК");
                for_book.Remove("ДЕВОЧКА");
                for_book.Add("РЕБЁНОК");
            }

            if (for_book.Contains("МУЖЧИНА") || for_book.Contains("ЖЕНЩИНА") || for_book.Contains("ДЕВУШКА"))
            {
                for_book.Remove("МУЖЧИНА");
                for_book.Remove("ДЕВУШКА");
                for_book.Remove("ЖЕНЩИНА");
                for_book.Add("ВЗРОСЛЫЙ");
            }

            if (for_book.Count == 0)
            {
                if (adjectives.Contains("ВЗРОСЛЫЙ")) for_book.Add("ВЗРОСЛЫЙ");
                if (adjectives.Contains("РЕБЁНОК")) for_book.Add("РЕБЁНОК");
            }
        }

        public List<List<Book>> ProcessText(string text)
        {
            List<Book> main_books = new List<Book>();
            List<Book> same_books = new List<Book>();

            Dictionary<int, int> types_def = new Dictionary<int, int>();
            Dictionary<int, int> types_defs_adj = new Dictionary<int, int>();
            Dictionary<int, int> types_defs_adj_age = new Dictionary<int, int>();

            AnalysisResult result = processor.Process(new SourceOfAnalysis(text));

            GetAdjectives(result);
            GetNoun(result);
            GetBookFor(result);

            List<int> adjectivesId = Bot.database.GetWordsId(adjectives);
            List<int> nounId = Bot.database.GetWordsId(nouns);
            List<int> for_bookId = Bot.database.GetWordsId(for_book);
            
            Dictionary<int, int> books_by_types = Bot.database.GetWordsIdConnIs(nounId);
            Dictionary<int, int> books_by_def = Bot.database.GetWordsIdConnDef(nounId);
            Dictionary<int, int> books_by_adj = Bot.database.GetWordsIdConnAdj(adjectivesId);
            Dictionary<int, int> books_by_age = Bot.database.GetWordsIdConnFor(for_bookId);

            if (books_by_types.Count != 0)
            {
                if (books_by_def.Count != 0)
                {
                    foreach (int b in books_by_def.Keys)
                    {
                        if (books_by_types.ContainsKey(b))
                        {
                            types_def.Add(b, books_by_def[b]);
                        }
                    }

                }
                else types_def = books_by_types;
            }
            else types_def = books_by_def;


            if (types_def.Count != 0)
            {
                if (books_by_adj.Count != 0)
                {
                    foreach (int b in types_def.Keys)
                    {
                        if (books_by_adj.ContainsKey(b))
                        {
                            types_defs_adj.Add(b, types_def[b] + books_by_adj[b]);
                        }
                    }
                }
                else types_defs_adj = types_def;
            }
            else
            {
                main_books = null;
                same_books = null;
                return new List<List<Book>> { main_books, same_books };
            }

            if (types_defs_adj.Count != 0)
            {
                if (books_by_age.Count != 0)
                {
                    foreach (int b in types_defs_adj.Keys)
                    {
                        if (books_by_age.ContainsKey(b))
                        {
                            types_defs_adj_age.Add(b, types_defs_adj[b]);
                        }
                    }
                }
                else types_defs_adj_age = types_defs_adj;
            }
            else
            {
                main_books = null;
                foreach (int k in types_def.Keys)
                {
                    same_books.Add(Bot.database.GetBook(k));
                }
                return new List<List<Book>> { main_books, same_books };
            }

            if (types_defs_adj_age.Count != 0)
            {
                foreach (int k in types_defs_adj.Keys)
                {
                    if (types_defs_adj_age.ContainsKey(k))
                    {
                        main_books.Add(Bot.database.GetBook(k));
                    }
                    else
                    {
                        same_books.Add(Bot.database.GetBook(k));
                    }
                }
                return new List<List<Book>> { main_books, same_books };
            }
            else
            {
                main_books = null;
                foreach (int k in types_defs_adj.Keys)
                {
                    same_books.Add(Bot.database.GetBook(k));
                }
                return new List<List<Book>> { main_books, same_books };
            }
        }
    } 
}
