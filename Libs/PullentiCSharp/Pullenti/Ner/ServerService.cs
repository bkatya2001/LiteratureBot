/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Pullenti.Ner
{
    /// <summary>
    /// Поддержка проведения анализа текста на внешнем сервере
    /// </summary>
    public static class ServerService
    {
        /// <summary>
        /// Проверить работоспособность сервера Pullenti.Server. 
        /// Отправляется GET-запрос на сервер, возвращает ASCII-строку с версией SDK.
        /// </summary>
        /// <param name="address">адрес вместе с портом (если null, то это http://localhost:1111)</param>
        /// <return>версия SDK на сервере или null, если недоступен</return>
        public static string GetServerVersion(string address)
        {
            try 
            {
                WebClient web = new WebClient();
                byte[] res = web.DownloadData(address ?? "http://localhost:1111");
                if (res == null || res.Length == 0) 
                    return null;
                return Encoding.UTF8.GetString(res);
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        /// <summary>
        /// Подготовить данные для POST-отправки на сервер
        /// </summary>
        /// <param name="proc">процессор, настройки (анализаторы) которого должны быть воспроизведены на сервере</param>
        /// <param name="text">обрабатывамый текст</param>
        /// <param name="lang">язык (если не задан, то будет определён автоматически)</param>
        public static byte[] PreparePostData(Processor proc, string text, Pullenti.Morph.MorphLang lang = null)
        {
            byte[] dat = null;
            using (MemoryStream mem1 = new MemoryStream()) 
            {
                StringBuilder tmp = new StringBuilder();
                tmp.AppendFormat("{0};", (lang == null ? 0 : (int)lang.Value));
                foreach (Analyzer a in proc.Analyzers) 
                {
                    tmp.AppendFormat("{0}{1};", (a.IgnoreThisAnalyzer ? "-" : ""), a.Name);
                }
                Pullenti.Ner.Core.Internal.SerializerHelper.SerializeInt(mem1, 1234);
                Pullenti.Ner.Core.Internal.SerializerHelper.SerializeString(mem1, tmp.ToString());
                Pullenti.Ner.Core.Internal.SerializerHelper.SerializeString(mem1, text);
                dat = mem1.ToArray();
            }
            return dat;
        }
        /// <summary>
        /// Оформить результат из того, что вернул сервер
        /// </summary>
        /// <param name="dat">массив байт, возвращённый сервером</param>
        /// <return>результат (но может быть Exception, если была на сервере ошибка)</return>
        public static AnalysisResult CreateResult(byte[] dat)
        {
            if (dat == null || (dat.Length < 1)) 
                throw new Exception("Empty result");
            AnalysisResult res = new AnalysisResult();
            Pullenti.Ner.Core.AnalysisKit kit = new Pullenti.Ner.Core.AnalysisKit(null);
            using (MemoryStream mem2 = new MemoryStream(dat)) 
            {
                if (dat[0] == 'E' && dat[1] == 'R') 
                {
                    string err = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeString(mem2);
                    throw new Exception(err);
                }
                kit.Deserialize(mem2);
                res.Entities.AddRange(kit.Entities);
                res.FirstToken = kit.FirstToken;
                res.Sofa = kit.Sofa;
                int i = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeInt(mem2);
                res.BaseLanguage = new Pullenti.Morph.MorphLang() { Value = (short)i };
                i = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeInt(mem2);
                for (; i > 0; i--) 
                {
                    res.Log.Add(Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeString(mem2));
                }
            }
            return res;
        }
        /// <summary>
        /// Обработать текст на сервере (если он запущен). 
        /// Функция фактически оформляет данные для отправки на сервер через PreparePostData(...), 
        /// затем делает POST-запрос по http, полученный массив байт через CreateResult(...) оформляет как результат. 
        /// Внимание! Внешняя онтология не поддерживается, в отличие от локальной обработки через Process.
        /// </summary>
        /// <param name="address">адрес вместе с портом (если null, то это http://localhost:1111)</param>
        /// <param name="proc">процессор, настройки (анализаторы) которого должны быть воспроизведены на сервере</param>
        /// <param name="text">обрабатывамый текст</param>
        /// <param name="lang">язык (если не задан, то будет определён автоматически)</param>
        /// <return>аналитический контейнер с результатом</return>
        public static AnalysisResult ProcessOnServer(string address, Processor proc, string text, Pullenti.Morph.MorphLang lang = null)
        {
            byte[] dat = PreparePostData(proc, text, lang);
            WebClient web = new WebClient();
            byte[] res = web.UploadData(address ?? "http://localhost:1111", dat);
            return CreateResult(res);
        }
        // Для внутреннего использования
        public static byte[] InternalProc(Stream stream)
        {
            int tag = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeInt(stream);
            if (tag != 1234) 
                return null;
            string attrs = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeString(stream);
            if (string.IsNullOrEmpty(attrs)) 
                return null;
            string[] parts = attrs.Split(';');
            if (parts.Length < 1) 
                return null;
            Pullenti.Morph.MorphLang lang = null;
            if (parts[0] != "0") 
                lang = new Pullenti.Morph.MorphLang() { Value = short.Parse(parts[0]) };
            using (Processor proc = ProcessorService.CreateEmptyProcessor()) 
            {
                for (int i = 1; i < parts.Length; i++) 
                {
                    string nam = parts[i];
                    if (nam.Length == 0) 
                        continue;
                    bool ign = false;
                    if (nam[0] == '-') 
                    {
                        ign = true;
                        nam = nam.Substring(1);
                    }
                    foreach (Analyzer a in ProcessorService.Analyzers) 
                    {
                        if (a.Name == nam) 
                        {
                            Analyzer aa = a.Clone();
                            if (ign) 
                                a.IgnoreThisAnalyzer = true;
                            proc.AddAnalyzer(a);
                        }
                    }
                }
                string txt = Pullenti.Ner.Core.Internal.SerializerHelper.DeserializeString(stream);
                AnalysisResult ar = proc.Process(new SourceOfAnalysis(txt), null, lang);
                byte[] res = null;
                if (ar != null && (ar.Tag is Pullenti.Ner.Core.AnalysisKit)) 
                {
                    using (MemoryStream mem = new MemoryStream()) 
                    {
                        Pullenti.Ner.Core.AnalysisKit kit = ar.Tag as Pullenti.Ner.Core.AnalysisKit;
                        kit.Entities.Clear();
                        kit.Entities.AddRange(ar.Entities);
                        kit.Serialize(mem, true);
                        Pullenti.Ner.Core.Internal.SerializerHelper.SerializeInt(mem, (ar.BaseLanguage == null ? 0 : (int)ar.BaseLanguage.Value));
                        Pullenti.Ner.Core.Internal.SerializerHelper.SerializeInt(mem, ar.Log.Count);
                        foreach (string s in ar.Log) 
                        {
                            Pullenti.Ner.Core.Internal.SerializerHelper.SerializeString(mem, s);
                        }
                        res = mem.ToArray();
                    }
                }
                return res;
            }
        }
    }
}