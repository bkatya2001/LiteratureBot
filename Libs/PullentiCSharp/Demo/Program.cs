/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Diagnostics;

namespace Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            // инициализация - необходимо проводить один раз до обработки текстов
            Console.Write("Initializing SDK Pullenti ver {0} ({1}) ... ", Pullenti.Sdk.Version, Pullenti.Sdk.VersionDate);
            // инициализируются движок и все имеющиеся анализаторы
            Pullenti.Sdk.InitializeAll();
            sw.Stop();
            Console.WriteLine("OK (by {0} ms), version {1}", (int)sw.ElapsedMilliseconds, Pullenti.Ner.ProcessorService.Version);
            // посмотрим, какие анализаторы доступны
            foreach (Pullenti.Ner.Analyzer a in Pullenti.Ner.ProcessorService.Analyzers) 
            {
                Console.WriteLine("   {0} {1} \"{2}\"", (a.IsSpecific ? "Specific analyzer" : "Common analyzer"), a.Name, a.Caption);
            }
            // анализируемый текст
            string txt = "Система разрабатывается с 2011 года российским программистом Михаилом Жуковым, проживающим в Москве на Красной площади в доме номер один на втором этаже. Конкурентов у него много: Abbyy, Yandex, ООО \"Russian Context Optimizer\" (RCO) и другие компании. Он планирует продать SDK за 1.120.000.001,99 (миллиард сто двадцать миллионов один рубль 99 копеек) рублей, без НДС.";
            Console.WriteLine("Text: {0}", txt);
            // запускаем обработку на пустом процессоре (без анализаторов NER)
            Pullenti.Ner.AnalysisResult are = Pullenti.Ner.ProcessorService.EmptyProcessor.Process(new Pullenti.Ner.SourceOfAnalysis(txt), null, null);
            Console.Write("Noun groups: ");
            // перебираем токены
            for (Pullenti.Ner.Token t = are.FirstToken; t != null; t = t.Next) 
            {
                // выделяем именную группу с текущего токена
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                // не получилось
                if (npt == null) 
                    continue;
                // получилось, выводим в нормализованном виде
                Console.Write("[{0}=>{1}] ", npt.GetSourceText(), npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false));
                // указатель на последний токен именной группы
                t = npt.EndToken;
            }
            using (Pullenti.Ner.Processor proc = Pullenti.Ner.ProcessorService.CreateProcessor()) 
            {
                // анализируем текст
                Pullenti.Ner.AnalysisResult ar = proc.Process(new Pullenti.Ner.SourceOfAnalysis(txt), null, null);
                // результирующие сущности
                Console.WriteLine("\r\n==========================================\r\nEntities: ");
                foreach (Pullenti.Ner.Referent e in ar.Entities) 
                {
                    Console.WriteLine("{0}: {1}", e.TypeName, e.ToString());
                    foreach (Pullenti.Ner.Slot s in e.Slots) 
                    {
                        Console.WriteLine("   {0}: {1}", s.TypeName, s.Value);
                    }
                }
                // пример выделения именных групп
                Console.WriteLine("\r\n==========================================\r\nNoun groups: ");
                for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
                {
                    // токены с сущностями игнорируем
                    if (t.GetReferent() != null) 
                        continue;
                    // пробуем создать именную группу
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.AdjectiveCanBeLast, 0, null);
                    // не получилось
                    if (npt == null) 
                        continue;
                    Console.WriteLine(npt);
                    // указатель перемещаем на последний токен группы
                    t = npt.EndToken;
                }
                // попробуем проанализировать через сервер (если он запущен, естественно)
                string serverAddress = "http://localhost:1111";
                string serverSdkVersion = Pullenti.Ner.ServerService.GetServerVersion(serverAddress);
                if (serverSdkVersion == null) 
                    Console.WriteLine("Server not exists on {0}, OK", serverAddress);
                else 
                {
                    Console.WriteLine("Server SDK Version: {0}", serverSdkVersion);
                    // желательно проверить тождественность версий, а то мало ли...
                    if (serverSdkVersion != Pullenti.Ner.ProcessorService.Version) 
                        Console.WriteLine("Server version {0} not equals current SDK version {1}", serverSdkVersion, Pullenti.Ner.ProcessorService.Version);
                    else 
                    {
                        // по идее, должны получить абсолютно эквивалентный результат, что и в ar
                        Pullenti.Ner.AnalysisResult ar2 = Pullenti.Ner.ServerService.ProcessOnServer(serverAddress, proc, txt, null);
                        if (ar2 == null) 
                            Console.WriteLine("Server execution ERROR! ");
                        else if (ar2.Entities.Count != ar.Entities.Count) 
                            Console.WriteLine("Entities on server = {0}, but on local = {1}", ar2.Entities.Count, ar.Entities.Count);
                        else 
                        {
                            bool eq = true;
                            for (int i = 0; i < ar2.Entities.Count; i++) 
                            {
                                if (!ar2.Entities[i].CanBeEquals(ar.Entities[i], Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                                {
                                    Console.WriteLine("Server entity '{0}' not equal local entity '{1}'", ar2.Entities[i], ar.Entities[i]);
                                    eq = false;
                                }
                            }
                            if (eq) 
                                Console.WriteLine("Process on server equals local process!");
                        }
                    }
                }
            }
            using (Pullenti.Ner.Processor proc = Pullenti.Ner.ProcessorService.CreateSpecificProcessor(Pullenti.Ner.Keyword.KeywordAnalyzer.ANALYZER_NAME)) 
            {
                Pullenti.Ner.AnalysisResult ar = proc.Process(new Pullenti.Ner.SourceOfAnalysis(txt), null, null);
                Console.WriteLine("\r\n==========================================\r\nKeywords1: ");
                foreach (Pullenti.Ner.Referent e in ar.Entities) 
                {
                    if (e is Pullenti.Ner.Keyword.KeywordReferent) 
                        Console.WriteLine(e);
                }
                Console.WriteLine("\r\n==========================================\r\nKeywords2: ");
                for (Pullenti.Ner.Token t = ar.FirstToken; t != null; t = t.Next) 
                {
                    if (t is Pullenti.Ner.ReferentToken) 
                    {
                        Pullenti.Ner.Keyword.KeywordReferent kw = t.GetReferent() as Pullenti.Ner.Keyword.KeywordReferent;
                        if (kw == null) 
                            continue;
                        string kwstr = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominativeSingle | Pullenti.Ner.Core.GetTextAttr.KeepRegister);
                        Console.WriteLine("{0} = {1}", kwstr, kw);
                    }
                }
            }
            Console.WriteLine("Over!");
        }
    }
}