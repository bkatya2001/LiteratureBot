/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Pullenti.Ner.Geo.Internal
{
    public class CityItemToken : Pullenti.Ner.MetaToken
    {
        public static void Initialize()
        {
            if (m_Ontology != null) 
                return;
            m_Ontology = new Pullenti.Ner.Core.IntOntologyCollection();
            m_OntologyEx = new Pullenti.Ner.Core.IntOntologyCollection();
            m_CityAdjectives = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("ГОРОД");
            t.AddAbridge("ГОР.");
            t.AddAbridge("Г.");
            t.Tag = ItemType.Noun;
            t.AddVariant("ГОРОД ФЕДЕРАЛЬНОГО ЗНАЧЕНИЯ", false);
            t.AddVariant("ГОРОД ГОРОДСКОЕ ПОСЕЛЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОДОК");
            t.Tag = ItemType.Noun;
            t.AddVariant("ШАХТЕРСКИЙ ГОРОДОК", false);
            t.AddVariant("ПРИМОРСКИЙ ГОРОДОК", false);
            t.AddVariant("МАЛЕНЬКИЙ ГОРОДОК", false);
            t.AddVariant("НЕБОЛЬШОЙ ГОРОДОК", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("CITY");
            t.Tag = ItemType.Noun;
            t.AddVariant("TOWN", false);
            t.AddVariant("CAPITAL", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСТО", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("МІС.");
            t.AddAbridge("М.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОД-ГЕРОЙ") { CanonicText = "ГОРОД" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСТО-ГЕРОЙ", Pullenti.Morph.MorphLang.UA) { CanonicText = "МІСТО" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОД-КУРОРТ") { CanonicText = "ГОРОД" };
            t.AddAbridge("Г.К.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСТО-КУРОРТ", Pullenti.Morph.MorphLang.UA) { CanonicText = "МІСТО" };
            t.AddAbridge("М.К.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛО");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДЕРЕВНЯ");
            t.AddAbridge("ДЕР.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛЕНИЕ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛО", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОРТ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОРТ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОСЕЛЕНИЕ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОСЕЛОК");
            t.AddAbridge("ПОС.");
            t.Tag = ItemType.Noun;
            t.AddVariant("ЖИЛОЙ ПОСЕЛОК", false);
            t.AddVariant("КУРОРТНЫЙ ПОСЕЛОК", false);
            t.AddVariant("ВАХТОВЫЙ ПОСЕЛОК", false);
            t.AddVariant("ШАХТЕРСКИЙ ПОСЕЛОК", false);
            t.AddVariant("ПОСЕЛОК СОВХОЗА", false);
            t.AddVariant("ПОСЕЛОК КОЛХОЗА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛИЩЕ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("СЕЛ.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОСЕЛОК ГОРОДСКОГО ТИПА");
            t.Acronym = (t.AcronymSmart = "ПГТ");
            t.AddAbridge("ПГТ.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛИЩЕ МІСЬКОГО ТИПУ", Pullenti.Morph.MorphLang.UA);
            t.Acronym = (t.AcronymSmart = "СМТ");
            t.AddAbridge("СМТ.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАБОЧИЙ ПОСЕЛОК");
            t.AddAbridge("Р.П.");
            t.Tag = ItemType.Noun;
            t.AddAbridge("РАБ.П.");
            t.AddAbridge("Р.ПОС.");
            t.AddAbridge("РАБ.ПОС.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РОБОЧЕ СЕЛИЩЕ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("Р.С.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОСЕЛОК");
            t.AddAbridge("Д.П.");
            t.Tag = ItemType.Noun;
            t.AddAbridge("ДАЧ.П.");
            t.AddAbridge("Д.ПОС.");
            t.AddAbridge("ДАЧ.ПОС.");
            t.AddVariant("ЖИЛИЩНО ДАЧНЫЙ ПОСЕЛОК", false);
            t.AddVariant("ДАЧНОЕ ПОСЕЛЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЕ СЕЛИЩЕ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("Д.С.");
            t.Tag = ItemType.Noun;
            t.AddAbridge("ДАЧ.С.");
            t.AddAbridge("Д.СЕЛ.");
            t.AddAbridge("ДАЧ.СЕЛ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОДСКОЕ ПОСЕЛЕНИЕ");
            t.AddAbridge("Г.П.");
            t.Tag = ItemType.Noun;
            t.AddAbridge("Г.ПОС.");
            t.AddAbridge("ГОР.П.");
            t.AddAbridge("ГОР.ПОС.");
            t.AddAbridge("Г.О.Г.");
            t.AddAbridge("ГОРОДСКОЙ ОКРУГ Г.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОСЕЛКОВОЕ ПОСЕЛЕНИЕ") { CanonicText = "ПОСЕЛОК", Tag = ItemType.Noun };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МІСЬКЕ ПОСЕЛЕННЯ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕЛЬСКОЕ ПОСЕЛЕНИЕ");
            t.Tag = ItemType.Noun;
            t.AddAbridge("С.ПОС.");
            t.AddAbridge("С.П.");
            t.AddVariant("СЕЛЬСОВЕТ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СІЛЬСЬКЕ ПОСЕЛЕННЯ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("С.ПОС.");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАНИЦА");
            t.Tag = ItemType.Noun;
            t.AddAbridge("СТ-ЦА");
            t.AddAbridge("СТАН-ЦА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАНИЦЯ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТОЛИЦА") { CanonicText = "ГОРОД" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТОЛИЦЯ", Pullenti.Morph.MorphLang.UA) { CanonicText = "МІСТО" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАНЦИЯ");
            t.AddAbridge("СТАНЦ.");
            t.AddAbridge("СТ.");
            t.AddAbridge("СТАН.");
            t.Tag = ItemType.Noun;
            t.AddVariant("ПЛАТФОРМА", false);
            t.AddAbridge("ПЛАТФ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАНЦІЯ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖЕЛЕЗНОДОРОЖНАЯ СТАНЦИЯ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАЛІЗНИЧНА СТАНЦІЯ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НАСЕЛЕННЫЙ ПУНКТ");
            t.Tag = ItemType.Noun;
            t.AddAbridge("Н.П.");
            t.AddAbridge("Б.Н.П.");
            t.AddAbridge("НП");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НАСЕЛЕНИЙ ПУНКТ", Pullenti.Morph.MorphLang.UA);
            t.Tag = ItemType.Noun;
            t.AddAbridge("НП");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЙОННЫЙ ЦЕНТР") { CanonicText = "НАСЕЛЕННЫЙ ПУНКТ" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЙОННИЙ ЦЕНТР", Pullenti.Morph.MorphLang.UA) { CanonicText = "НАСЕЛЕНИЙ ПУНКТ" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЛАСТНОЙ ЦЕНТР") { CanonicText = "НАСЕЛЕННЫЙ ПУНКТ" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЛАСНИЙ ЦЕНТР", Pullenti.Morph.MorphLang.UA) { CanonicText = "НАСЕЛЕНИЙ ПУНКТ" };
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЧИНОК");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАИМКА");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ХУТОР");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АУЛ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ААЛ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АРБАН");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЫСЕЛКИ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("МЕСТЕЧКО");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УРОЧИЩЕ");
            t.Tag = ItemType.Noun;
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УСАДЬБА");
            t.Tag = ItemType.Noun;
            t.AddVariant("ЦЕНТРАЛЬНАЯ УСАДЬБА", false);
            t.AddAbridge("ЦЕНТР.УС.");
            t.AddAbridge("ЦЕНТР.УСАДЬБА");
            t.AddAbridge("Ц/У");
            t.AddAbridge("УС-БА");
            t.AddAbridge("ЦЕНТР.УС-БА");
            m_Ontology.Add(t);
            foreach (string s in new string[] {"ЖИТЕЛЬ", "МЭР"}) 
            {
                m_Ontology.Add(new Pullenti.Ner.Core.Termin(s) { Tag = ItemType.Misc });
            }
            foreach (string s in new string[] {"ЖИТЕЛЬ", "МЕР"}) 
            {
                m_Ontology.Add(new Pullenti.Ner.Core.Termin(s, Pullenti.Morph.MorphLang.UA) { Tag = ItemType.Misc });
            }
            t = new Pullenti.Ner.Core.Termin("АДМИНИСТРАЦИЯ") { Tag = ItemType.Misc };
            t.AddAbridge("АДМ.");
            m_Ontology.Add(t);
            m_StdAdjectives = new Pullenti.Ner.Core.IntOntologyCollection();
            t = new Pullenti.Ner.Core.Termin("ВЕЛИКИЙ");
            t.AddAbridge("ВЕЛ.");
            t.AddAbridge("ВЕЛИК.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("БОЛЬШОЙ");
            t.AddAbridge("БОЛ.");
            t.AddAbridge("БОЛЬШ.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАЛЫЙ");
            t.AddAbridge("МАЛ.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЕРХНИЙ");
            t.AddAbridge("ВЕР.");
            t.AddAbridge("ВЕРХ.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("НИЖНИЙ");
            t.AddAbridge("НИЖ.");
            t.AddAbridge("НИЖН.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("СРЕДНИЙ");
            t.AddAbridge("СРЕД.");
            t.AddAbridge("СРЕДН.");
            t.AddAbridge("СР.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАРЫЙ");
            t.AddAbridge("СТ.");
            t.AddAbridge("СТАР.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОВЫЙ");
            t.AddAbridge("НОВ.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЕЛИКИЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("ВЕЛ.");
            t.AddAbridge("ВЕЛИК.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("МАЛИЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("МАЛ.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЕРХНІЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("ВЕР.");
            t.AddAbridge("ВЕРХ.");
            t.AddAbridge("ВЕРХН.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("НИЖНІЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("НИЖ.");
            t.AddAbridge("НИЖН.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("СЕРЕДНІЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("СЕР.");
            t.AddAbridge("СЕРЕД.");
            t.AddAbridge("СЕРЕДН.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТАРИЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("СТ.");
            t.AddAbridge("СТАР.");
            m_StdAdjectives.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОВИЙ", Pullenti.Morph.MorphLang.UA);
            t.AddAbridge("НОВ.");
            m_StdAdjectives.Add(t);
            m_StdAdjectives.Add(new Pullenti.Ner.Core.Termin("SAN"));
            m_StdAdjectives.Add(new Pullenti.Ner.Core.Termin("LOS"));
            m_SpecNames = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"ГОРОДОК ПИСАТЕЛЕЙ ПЕРЕДЕЛКИНО", "ЦЕНТРАЛЬНАЯ УСАДЬБА", "ГОРКИ ЛЕНИНСКИЕ"}) 
            {
                m_SpecNames.Add(new Pullenti.Ner.Core.Termin(s) { IgnoreTermsOrder = true });
            }
            m_SpecAbbrs = new Pullenti.Ner.Core.TerminCollection();
            t = new Pullenti.Ner.Core.Termin("ЛЕСНИЧЕСТВО");
            t.AddAbridge("ЛЕС-ВО");
            t.AddAbridge("ЛЕСН-ВО");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕСОПАРК");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛЕСОУЧАСТОК");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("РУДНИК");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРИИСК");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("МЕСТОРОЖДЕНИЯ");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЗАПОВЕДНИК");
            t.AddAbridge("ЗАП-К");
            m_SpecAbbrs.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВХОЗ");
            t.AddAbridge("С/Х");
            t.AddAbridge("СВХ");
            m_SpecAbbrs.Add(t);
            byte[] dat = Pullenti.Ner.Address.Internal.ResourceHelper.GetBytes("c.dat");
            if (dat == null) 
                throw new Exception("Not found resource file c.dat in Analyzer.Location");
            using (MemoryStream tmp = new MemoryStream(MiscLocationHelper.Deflate(dat))) 
            {
                tmp.Position = 0;
                XmlDocument xml = new XmlDocument();
                xml.Load(tmp);
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    if (x.Name == "bigcity") 
                        LoadBigCity(x);
                    else if (x.Name == "city") 
                        LoadCity(x);
                }
            }
        }
        static void LoadCity(XmlNode xml)
        {
            Pullenti.Ner.Core.IntOntologyItem ci = new Pullenti.Ner.Core.IntOntologyItem(null);
            Pullenti.Ner.Core.IntOntologyCollection onto = m_OntologyEx;
            Pullenti.Morph.MorphLang lang = Pullenti.Morph.MorphLang.RU;
            if (xml.Attributes["l"] != null && xml.Attributes["l"].InnerText == "ua") 
                lang = Pullenti.Morph.MorphLang.UA;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.Name == "n") 
                {
                    string v = x.InnerText;
                    Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin();
                    t.InitByNormalText(v, lang);
                    ci.Termins.Add(t);
                    t.AddStdAbridges();
                    if (v.StartsWith("SAINT ")) 
                        t.AddAbridge("ST. " + v.Substring(6));
                    else if (v.StartsWith("SAITNE ")) 
                        t.AddAbridge("STE. " + v.Substring(7));
                }
            }
            onto.AddItem(ci);
        }
        static void LoadBigCity(XmlNode xml)
        {
            Pullenti.Ner.Core.IntOntologyItem ci = new Pullenti.Ner.Core.IntOntologyItem(null);
            ci.MiscAttr = ci;
            string adj = null;
            Pullenti.Ner.Core.IntOntologyCollection onto = m_OntologyEx;
            Pullenti.Ner.Core.TerminCollection cityAdj = m_CityAdjectives;
            Pullenti.Morph.MorphLang lang = Pullenti.Morph.MorphLang.RU;
            if (xml.Attributes["l"] != null) 
            {
                string la = xml.Attributes["l"].InnerText;
                if (la == "ua") 
                    lang = Pullenti.Morph.MorphLang.UA;
                else if (la == "en") 
                    lang = Pullenti.Morph.MorphLang.EN;
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.Name == "n") 
                {
                    string v = x.InnerText;
                    if (string.IsNullOrEmpty(v)) 
                        continue;
                    Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin();
                    t.InitByNormalText(v, lang);
                    ci.Termins.Add(t);
                    if (v == "САНКТ-ПЕТЕРБУРГ") 
                    {
                        if (m_StPeterburg == null) 
                            m_StPeterburg = ci;
                        t.Acronym = "СПБ";
                        t.AddAbridge("С.ПЕТЕРБУРГ");
                        t.AddAbridge("СП-Б");
                        ci.Termins.Add(new Pullenti.Ner.Core.Termin("ПЕТЕРБУРГ", lang));
                    }
                    else if (v.StartsWith("SAINT ")) 
                        t.AddAbridge("ST. " + v.Substring(6));
                    else if (v.StartsWith("SAITNE ")) 
                        t.AddAbridge("STE. " + v.Substring(7));
                }
                else if (x.Name == "a") 
                    adj = x.InnerText;
            }
            onto.AddItem(ci);
            if (!string.IsNullOrEmpty(adj)) 
            {
                Pullenti.Ner.Core.Termin at = new Pullenti.Ner.Core.Termin();
                at.InitByNormalText(adj, lang);
                at.Tag = ci;
                cityAdj.Add(at);
                bool spb = adj == "САНКТ-ПЕТЕРБУРГСКИЙ" || adj == "САНКТ-ПЕТЕРБУРЗЬКИЙ";
                if (spb) 
                    cityAdj.Add(new Pullenti.Ner.Core.Termin(adj.Substring(6), lang) { Tag = ci });
            }
        }
        static Pullenti.Ner.Core.IntOntologyCollection m_Ontology;
        static Pullenti.Ner.Core.IntOntologyCollection m_OntologyEx;
        static Pullenti.Ner.Core.IntOntologyItem m_StPeterburg;
        public static Pullenti.Ner.Core.TerminCollection m_CityAdjectives;
        static Pullenti.Ner.Core.IntOntologyCollection m_StdAdjectives;
        static Pullenti.Ner.Core.TerminCollection m_SpecNames;
        static Pullenti.Ner.Core.TerminCollection m_SpecAbbrs;
        public static Pullenti.Ner.Core.IntOntologyToken CheckOntoItem(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> li = m_OntologyEx.TryAttach(t, null, false);
            if (li != null) 
            {
                foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                {
                    if (nt.Item != null) 
                        return nt;
                }
            }
            return null;
        }
        public static Pullenti.Ner.Core.IntOntologyToken CheckKeyword(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> li = m_Ontology.TryAttach(t, null, false);
            if (li != null) 
            {
                foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                {
                    if (nt.Item == null) 
                        return nt;
                }
            }
            return null;
        }
        public static List<CityItemToken> TryParseList(Pullenti.Ner.Token t, int maxCount, GeoAnalyzerData ad = null)
        {
            CityItemToken ci = CityItemToken.TryParse(t, null, false, ad);
            if (ci == null) 
            {
                if (t == null) 
                    return null;
                if (((t is Pullenti.Ner.TextToken) && t.IsValue("МУНИЦИПАЛЬНЫЙ", null) && t.Next != null) && t.Next.IsValue("ОБРАЗОВАНИЕ", null)) 
                {
                    Pullenti.Ner.Token t1 = t.Next.Next;
                    bool br = false;
                    if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t1, false, false)) 
                    {
                        br = true;
                        t1 = t1.Next;
                    }
                    List<CityItemToken> lii = TryParseList(t1, maxCount, null);
                    if (lii != null && lii[0].Typ == ItemType.Noun) 
                    {
                        lii[0].BeginToken = t;
                        lii[0].Doubtful = false;
                        if (br && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(lii[lii.Count - 1].EndToken.Next, false, null, false)) 
                            lii[lii.Count - 1].EndToken = lii[lii.Count - 1].EndToken.Next;
                        return lii;
                    }
                }
                return null;
            }
            if (ci.Chars.IsLatinLetter && ci.Typ == ItemType.Noun && !t.Chars.IsAllLower) 
                return null;
            List<CityItemToken> li = new List<CityItemToken>();
            li.Add(ci);
            for (t = ci.EndToken.Next; t != null; t = t.Next) 
            {
                if (t.IsNewlineBefore) 
                {
                    if (t.NewlinesBeforeCount > 1) 
                        break;
                    if (li.Count == 1 && li[0].Typ == ItemType.Noun) 
                    {
                    }
                    else 
                        break;
                }
                CityItemToken ci0 = CityItemToken.TryParse(t, ci, false, ad);
                if (ci0 == null) 
                {
                    if (t.IsNewlineBefore) 
                        break;
                    if (ci.Typ == ItemType.Noun && Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                    {
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if ((br != null && (br.LengthChar < 50) && t.Next.Chars.IsCyrillicLetter) && !t.Next.Chars.IsAllLower) 
                        {
                            ci0 = new CityItemToken(br.BeginToken, br.EndToken) { Typ = ItemType.ProperName };
                            Pullenti.Ner.Token tt = br.EndToken.Previous;
                            string num = null;
                            if (tt is Pullenti.Ner.NumberToken) 
                            {
                                num = (tt as Pullenti.Ner.NumberToken).Value.ToString();
                                tt = tt.Previous;
                                if (tt != null && tt.IsHiphen) 
                                    tt = tt.Previous;
                            }
                            ci0.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(br.BeginToken.Next, tt, Pullenti.Ner.Core.GetTextAttr.No);
                            if (tt != br.BeginToken.Next) 
                                ci0.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(br.BeginToken.Next, tt, Pullenti.Ner.Core.GetTextAttr.No);
                            if (string.IsNullOrEmpty(ci0.Value)) 
                                ci0 = null;
                            else if (num != null) 
                            {
                                ci0.Value = string.Format("{0}-{1}", ci0.Value, num);
                                if (ci0.AltValue != null) 
                                    ci0.AltValue = string.Format("{0}-{1}", ci0.AltValue, num);
                            }
                        }
                    }
                    if ((ci0 == null && ((ci.Typ == ItemType.ProperName || ci.Typ == ItemType.City)) && t.IsComma) && li[0] == ci) 
                    {
                        Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(t.Next);
                        if (npt != null) 
                        {
                            for (Pullenti.Ner.Token tt = t.Next; tt != null && tt.EndChar <= npt.EndChar; tt = tt.Next) 
                            {
                                CityItemToken ci00 = CityItemToken.TryParse(tt, ci, false, null);
                                if (ci00 != null && ci00.Typ == ItemType.Noun) 
                                {
                                    CityItemToken ci01 = CityItemToken.TryParse(ci00.EndToken.Next, ci, false, null);
                                    if (ci01 == null) 
                                    {
                                        ci0 = ci00;
                                        ci0.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValue(t.Next, ci00.EndToken, (t.Kit.BaseLanguage.IsEn ? Pullenti.Ner.Core.GetTextAttr.IgnoreArticles : Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominativeSingle)).ToLower();
                                        break;
                                    }
                                }
                                if (!tt.Chars.IsAllLower) 
                                    break;
                            }
                        }
                    }
                    if (ci0 == null) 
                        break;
                }
                if ((ci0.Typ == ItemType.Noun && ci0.Value != null && Pullenti.Morph.LanguageHelper.EndsWith(ci0.Value, "УСАДЬБА")) && ci.Typ == ItemType.Noun) 
                {
                    ci.Doubtful = false;
                    t = (ci.EndToken = ci0.EndToken);
                    continue;
                }
                if (ci0.Typ == ItemType.Noun && ci.Typ == ItemType.Misc && ci.Value == "АДМИНИСТРАЦИЯ") 
                    ci0.Doubtful = false;
                if (ci.MergeWithNext(ci0)) 
                {
                    t = ci.EndToken;
                    continue;
                }
                ci = ci0;
                li.Add(ci);
                t = ci.EndToken;
                if (maxCount > 0 && li.Count >= maxCount) 
                    break;
            }
            if (li.Count > 1 && li[0].Value == "СОВЕТ") 
                return null;
            if (li.Count > 2 && li[0].Typ == ItemType.Noun && li[1].Typ == ItemType.Noun) 
            {
                if (li[0].MergeWithNext(li[1])) 
                    li.RemoveAt(1);
            }
            if (li.Count > 2 && li[0].IsNewlineAfter) 
                li.RemoveRange(1, li.Count - 1);
            if (!li[0].GeoObjectBefore) 
                li[0].GeoObjectBefore = MiscLocationHelper.CheckGeoObjectBefore(li[0].BeginToken, false);
            if (!li[li.Count - 1].GeoObjectAfter) 
                li[li.Count - 1].GeoObjectAfter = MiscLocationHelper.CheckGeoObjectAfter(li[li.Count - 1].EndToken, true);
            if ((li.Count == 2 && li[0].Typ == ItemType.Noun && li[1].Typ == ItemType.Noun) && ((li[0].GeoObjectBefore || li[1].GeoObjectAfter))) 
            {
                if (li[0].Chars.IsCapitalUpper && li[1].Chars.IsAllLower) 
                    li[0].Typ = ItemType.ProperName;
                else if (li[1].Chars.IsCapitalUpper && li[0].Chars.IsAllLower) 
                    li[1].Typ = ItemType.ProperName;
            }
            return li;
        }
        public CityItemToken(Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
        }
        public enum ItemType : int
        {
            ProperName,
            City,
            Noun,
            Misc,
        }

        public ItemType Typ;
        public string Value;
        public string AltValue;
        public Pullenti.Ner.Core.IntOntologyItem OntoItem;
        public bool Doubtful;
        public bool GeoObjectBefore;
        public bool GeoObjectAfter;
        public Pullenti.Ner.Geo.GeoReferent HigherGeo;
        public Pullenti.Ner.ReferentToken OrgRef;
        internal Condition Cond;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Cond != null) 
                res.AppendFormat("[{0}] ", Cond.ToString());
            res.AppendFormat("{0}", Typ.ToString());
            if (Value != null) 
                res.AppendFormat(" {0}", Value);
            if (OntoItem != null) 
                res.AppendFormat(" {0}", OntoItem.ToString());
            if (Doubtful) 
                res.Append(" (?)");
            if (OrgRef != null) 
                res.AppendFormat(" (Org: {0})", OrgRef.Referent);
            if (GeoObjectBefore) 
                res.Append(" GeoBefore");
            if (GeoObjectAfter) 
                res.Append(" GeoAfter");
            return res.ToString();
        }
        public bool MergeWithNext(CityItemToken ne)
        {
            if (Typ != ItemType.Noun || ne.Typ != ItemType.Noun) 
                return false;
            bool ok = false;
            if (Value == "ГОРОДСКОЕ ПОСЕЛЕНИЕ" && ne.Value == "ГОРОД") 
                ok = true;
            if (!ok) 
                return false;
            EndToken = ne.EndToken;
            Doubtful = false;
            return true;
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.CRegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                CityItemToken cit = TryParse(t, null, false, ad);
                if (cit != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.Cit = cit;
                }
            }
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d == null || d.Cit == null || d.Cit.Typ != ItemType.Noun) 
                    continue;
                Pullenti.Ner.Token tt = d.Cit.EndToken.Next;
                if (tt == null) 
                    continue;
                GeoTokenData dd = tt.Tag as GeoTokenData;
                if (dd != null && dd.Cit != null) 
                    continue;
                CityItemToken cit = TryParse(tt, d.Cit, false, ad);
                if (cit == null) 
                    continue;
                if (dd == null) 
                    dd = new GeoTokenData(tt);
                dd.Cit = cit;
            }
            ad.CRegime = true;
        }
        public static CityItemToken TryParse(Pullenti.Ner.Token t, CityItemToken prev = null, bool dontNormalize = false, GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            GeoTokenData d = t.Tag as GeoTokenData;
            if (t.IsValue("НП", null)) 
            {
            }
            if (d != null && d.NoGeo) 
                return null;
            if (SpeedRegime && ((ad.CRegime || ad.AllRegime)) && !dontNormalize) 
            {
                if (d == null) 
                    return null;
                if (d.Cit == null) 
                    return null;
                if (d.Cit.Cond != null) 
                {
                    if (ad.CheckRegime) 
                        return null;
                    ad.CheckRegime = true;
                    bool b = d.Cit.Cond.Check();
                    ad.CheckRegime = false;
                    if (!b) 
                        return null;
                }
                return d.Cit;
            }
            if (ad.CLevel > 1) 
                return null;
            ad.CLevel++;
            CityItemToken res = _tryParseInt(t, prev, dontNormalize, ad);
            ad.CLevel--;
            if (res != null && res.Typ == ItemType.Noun && (res.WhitespacesAfterCount < 2)) 
            {
                Pullenti.Ner.Core.NounPhraseToken nn = MiscLocationHelper.TryParseNpt(res.EndToken.Next);
                if (nn != null && ((nn.EndToken.IsValue("ЗНАЧЕНИЕ", "ЗНАЧЕННЯ") || nn.EndToken.IsValue("ТИП", null) || nn.EndToken.IsValue("ХОЗЯЙСТВО", "ХАЗЯЙСТВО")))) 
                {
                    if (OrgItemToken.TryParse(res.EndToken.Next, ad) == null) 
                        res.EndToken = nn.EndToken;
                }
            }
            if (((res != null && res.Typ == ItemType.ProperName && res.Value != null) && !res.Doubtful && res.BeginToken == res.EndToken) && res.Value.Length > 4) 
            {
                if (Pullenti.Morph.LanguageHelper.EndsWithEx(res.Value, "ГРАД", "ГОРОД", null, null)) 
                {
                    res.AltValue = null;
                    res.Typ = ItemType.City;
                }
                else if (Pullenti.Morph.LanguageHelper.EndsWithEx(res.Value, "СК", "ИНО", "ПОЛЬ", null) || Pullenti.Morph.LanguageHelper.EndsWithEx(res.Value, "ВЛЬ", "АС", "ЕС", null)) 
                {
                    List<Pullenti.Ner.Address.Internal.StreetItemToken> sits = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(res.EndToken.Next, 3, ad);
                    if (sits != null) 
                    {
                        if (sits.Count == 1 && sits[0].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                            return res;
                        if (sits.Count == 2 && sits[0].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Number && sits[1].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                            return res;
                    }
                    Pullenti.Morph.MorphClass mc = res.EndToken.GetMorphClassInDictionary();
                    if (mc.IsProperGeo || mc.IsUndefined) 
                    {
                        res.AltValue = null;
                        res.Typ = ItemType.City;
                    }
                }
                else if (Pullenti.Morph.LanguageHelper.EndsWithEx(res.Value, "АНЬ", "TOWN", null, null) || res.Value.StartsWith("SAN")) 
                    res.Typ = ItemType.City;
                else if (res.EndToken is Pullenti.Ner.TextToken) 
                {
                    string lem = (res.EndToken as Pullenti.Ner.TextToken).Lemma;
                    if (Pullenti.Morph.LanguageHelper.EndsWithEx(lem, "ГРАД", "ГОРОД", "СК", null) || Pullenti.Morph.LanguageHelper.EndsWithEx(lem, "АНЬ", "ПОЛЬ", null, null)) 
                    {
                        res.AltValue = res.Value;
                        res.Value = lem;
                        int ii = res.AltValue.IndexOf('-');
                        if (ii >= 0) 
                            res.Value = res.AltValue.Substring(0, ii + 1) + lem;
                        if (!Pullenti.Morph.LanguageHelper.EndsWith(res.Value, "АНЬ")) 
                            res.AltValue = null;
                    }
                }
            }
            return res;
        }
        static CityItemToken _tryParseInt(Pullenti.Ner.Token t, CityItemToken prev, bool dontNormalize, GeoAnalyzerData ad)
        {
            if (t == null || (((t is Pullenti.Ner.TextToken) && !t.Chars.IsLetter))) 
                return null;
            CityItemToken res = _TryParse(t, prev, dontNormalize, ad);
            if ((prev == null && t.Chars.IsCyrillicLetter && t.Chars.IsAllUpper) && t.LengthChar == 2) 
            {
                if (t.IsValue("ТА", null)) 
                {
                    res = _TryParse(t.Next, prev, dontNormalize, ad);
                    if (res != null) 
                    {
                        if (res.Typ == ItemType.Noun) 
                        {
                            res.BeginToken = t;
                            res.Doubtful = false;
                        }
                        else 
                            res = null;
                    }
                }
            }
            if (prev != null && prev.Typ == ItemType.Noun && ((prev.Value != "ГОРОД" && prev.Value != "МІСТО"))) 
            {
                if (res == null) 
                {
                    OrgItemToken det = OrgItemToken.TryParse(t, null);
                    if (det != null) 
                    {
                        int cou = 0;
                        for (Pullenti.Ner.Token ttt = det.BeginToken; ttt != null && ttt.EndChar <= det.EndChar; ttt = ttt.Next) 
                        {
                            if (ttt.Chars.IsLetter) 
                                cou++;
                        }
                        if (cou < 6) 
                        {
                            CityItemToken re = new CityItemToken(det.BeginToken, det.EndToken) { Typ = ItemType.ProperName };
                            if (det.Referent.TypeName == "ORGANIZATION") 
                                re.OrgRef = det;
                            else 
                            {
                                re.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(det, Pullenti.Ner.Core.GetTextAttr.No);
                                re.AltValue = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(det, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominative);
                            }
                            return re;
                        }
                    }
                }
            }
            if (res != null && res.Typ == ItemType.Noun && (res.WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(res.EndToken.Next);
                if (npt != null) 
                {
                    if (npt.EndToken.IsValue("ПОДЧИНЕНИЕ", "ПІДПОРЯДКУВАННЯ")) 
                        res.EndToken = npt.EndToken;
                }
                if (res.Value == "НАСЕЛЕННЫЙ ПУНКТ") 
                {
                    CityItemToken next = _TryParse(res.EndToken.Next, prev, dontNormalize, ad);
                    if (next != null && next.Typ == ItemType.Noun) 
                    {
                        next.BeginToken = res.BeginToken;
                        return next;
                    }
                }
            }
            if (res != null && t.Chars.IsAllUpper && res.Typ == ItemType.ProperName) 
            {
                Pullenti.Ner.Token tt = t.Previous;
                if (tt != null && tt.IsComma) 
                    tt = tt.Previous;
                Pullenti.Ner.Geo.GeoReferent geoPrev = null;
                if (tt != null && (tt.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                    geoPrev = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                if (geoPrev != null && ((geoPrev.IsRegion || geoPrev.IsCity))) 
                {
                    OrgItemToken det = OrgItemToken.TryParse(t, null);
                    if (det != null) 
                        res = null;
                }
            }
            if (res != null && res.Typ == ItemType.ProperName) 
            {
                if ((t.IsValue("ДУМА", "РАДА") || t.IsValue("ГЛАВА", "ГОЛОВА") || t.IsValue("АДМИНИСТРАЦИЯ", "АДМІНІСТРАЦІЯ")) || t.IsValue("МЭР", "МЕР") || t.IsValue("ПРЕДСЕДАТЕЛЬ", "ГОЛОВА")) 
                    return null;
            }
            if (res != null && res.Value == "НАСЕЛЕННЫЙ ПУНКТ" && (res.WhitespacesAfterCount < 2)) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken s = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(res.EndToken.Next, null, false, null);
                if (s != null && s.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && s.Termin.CanonicText == "ПОЧТОВОЕ ОТДЕЛЕНИЕ") 
                    res.EndToken = s.EndToken;
            }
            Pullenti.Ner.Geo.GeoReferent geoAfter = null;
            if (res == null) 
            {
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                {
                    Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                    if (br != null) 
                    {
                        res = _TryParse(t.Next, null, false, ad);
                        if (res != null && ((res.Typ == ItemType.ProperName || res.Typ == ItemType.City))) 
                        {
                            res.BeginToken = t;
                            res.Typ = ItemType.ProperName;
                            res.EndToken = br.EndToken;
                            if (res.EndToken.Next != br.EndToken) 
                            {
                                res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, br.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                                res.AltValue = null;
                            }
                            return res;
                        }
                    }
                }
                if (t is Pullenti.Ner.TextToken) 
                {
                    string txt = (t as Pullenti.Ner.TextToken).Term;
                    if (txt == "ИМ" || txt == "ИМЕНИ") 
                    {
                        Pullenti.Ner.Token t1 = t.Next;
                        if (t1 != null && t1.IsChar('.')) 
                            t1 = t1.Next;
                        res = _TryParse(t1, null, false, ad);
                        if (res != null && ((((res.Typ == ItemType.City && res.Doubtful)) || res.Typ == ItemType.ProperName))) 
                        {
                            res.BeginToken = t;
                            res.Morph = new Pullenti.Ner.MorphCollection();
                            return res;
                        }
                    }
                    if (t.Chars.IsCyrillicLetter && t.LengthChar == 1 && t.Chars.IsAllUpper) 
                    {
                        if ((t.Next != null && !t.IsWhitespaceAfter && ((t.Next.IsHiphen || t.Next.IsChar('.')))) && (t.Next.WhitespacesAfterCount < 2)) 
                        {
                            if (prev != null && prev.Typ == ItemType.Noun && (((!prev.Doubtful || prev.GeoObjectBefore || MiscLocationHelper.CheckGeoObjectBefore(prev.BeginToken, false)) || MiscLocationHelper.CheckGeoObjectBeforeBrief(prev.BeginToken, ad)))) 
                            {
                                CityItemToken res1 = _TryParse(t.Next.Next, null, false, ad);
                                if (res1 != null && ((res1.Typ == ItemType.ProperName || res1.Typ == ItemType.City))) 
                                {
                                    List<string> adjs = MiscLocationHelper.GetStdAdjFullStr(txt, res1.Morph.Gender, res1.Morph.Number, true);
                                    if (adjs == null && prev != null && prev.Typ == ItemType.Noun) 
                                        adjs = MiscLocationHelper.GetStdAdjFullStr(txt, prev.Morph.Gender, Pullenti.Morph.MorphNumber.Undefined, true);
                                    if (adjs == null) 
                                        adjs = MiscLocationHelper.GetStdAdjFullStr(txt, res1.Morph.Gender, res1.Morph.Number, false);
                                    if (adjs != null) 
                                    {
                                        if (res1.Value == null) 
                                            res1.Value = res1.GetSourceText().ToUpper();
                                        if (res1.AltValue != null) 
                                            res1.AltValue = string.Format("{0} {1}", adjs[0], res1.AltValue);
                                        else if (adjs.Count > 1) 
                                            res1.AltValue = string.Format("{0} {1}", adjs[1], res1.Value);
                                        res1.Value = string.Format("{0} {1}", adjs[0], res1.Value);
                                        res1.BeginToken = t;
                                        res1.Typ = ItemType.ProperName;
                                        return res1;
                                    }
                                }
                            }
                        }
                    }
                }
                Pullenti.Ner.Token tt = (prev == null ? t.Previous : prev.BeginToken.Previous);
                while (tt != null && tt.IsCharOf(",.")) 
                {
                    tt = tt.Previous;
                }
                Pullenti.Ner.Geo.GeoReferent geoPrev = null;
                if (tt != null && (tt.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                    geoPrev = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                Condition cond = null;
                Pullenti.Ner.Token tt0 = t;
                bool ooo = false;
                bool hasGeoAfter = false;
                if (geoPrev != null) 
                    ooo = true;
                else if (MiscLocationHelper.CheckNearBefore(t, ad) != null) 
                    ooo = true;
                else if (MiscLocationHelper.CheckGeoObjectBefore(t, false)) 
                    ooo = true;
                else if (t.Chars.IsLetter) 
                {
                    tt = t.Next;
                    if (tt != null && tt.IsChar('.')) 
                        tt = tt.Next;
                    if ((tt is Pullenti.Ner.TextToken) && !tt.Chars.IsAllLower) 
                    {
                        if (MiscLocationHelper.CheckGeoObjectAfterBrief(tt, ad)) 
                            ooo = (hasGeoAfter = true);
                        else if (MiscLocationHelper.CheckGeoObjectAfter(tt, true)) 
                            ooo = (hasGeoAfter = true);
                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(tt.Next, false)) 
                            ooo = true;
                        else if (ad.CLevel == 0) 
                        {
                            CityItemToken cit2 = TryParse(tt, null, false, ad);
                            if (cit2 != null && cit2.BeginToken != cit2.EndToken && ((cit2.Typ == ItemType.ProperName || cit2.Typ == ItemType.City))) 
                            {
                                if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(cit2.EndToken.Next, false)) 
                                    ooo = true;
                            }
                            if (cit2 != null && cit2.Typ == ItemType.City && tt.Previous.IsChar('.')) 
                            {
                                if (cit2.IsWhitespaceAfter || ((cit2.EndToken.Next != null && cit2.EndToken.Next.LengthChar == 1))) 
                                {
                                    ooo = true;
                                    if (cit2.OntoItem != null) 
                                        geoAfter = cit2.OntoItem.Referent as Pullenti.Ner.Geo.GeoReferent;
                                }
                            }
                        }
                    }
                }
                if ((ad != null && !ooo && !ad.CRegime) && SpeedRegime) 
                {
                    if (cond == null) 
                        cond = new Condition();
                    cond.GeoBeforeToken = t;
                    ooo = true;
                }
                if (ooo) 
                {
                    tt = t;
                    for (Pullenti.Ner.Token ttt = tt; ttt != null; ttt = ttt.Next) 
                    {
                        if (ttt.IsCharOf(",.")) 
                        {
                            tt = ttt.Next;
                            continue;
                        }
                        if (ttt.IsNewlineBefore) 
                            break;
                        Pullenti.Ner.Address.Internal.AddressItemToken det = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(ttt, null, ad);
                        if (det != null && det.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Detail) 
                        {
                            ttt = det.EndToken;
                            tt = (tt0 = det.EndToken.Next);
                            continue;
                        }
                        OrgItemToken org = OrgItemToken.TryParse(ttt, null);
                        if (org != null && org.IsGsk) 
                        {
                            ttt = org.EndToken;
                            tt0 = (tt = org.EndToken.Next);
                            continue;
                        }
                        Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(ttt, null, null);
                        if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Plot) 
                        {
                            ttt = ait.EndToken;
                            tt0 = (tt = ait.EndToken.Next);
                            continue;
                        }
                        break;
                    }
                    if (tt is Pullenti.Ner.TextToken) 
                    {
                        if (tt0.IsComma && tt0.Next != null) 
                            tt0 = tt0.Next;
                        string txt = (tt as Pullenti.Ner.TextToken).Term;
                        if ((((txt == "Д" || txt == "С" || txt == "C") || txt == "П" || txt == "Х")) && ((tt.Chars.IsAllLower || ((tt.Next != null && tt.Next.IsChar('.')))))) 
                        {
                            Pullenti.Ner.Token tt1 = tt;
                            if (tt1.Next != null && tt1.Next.IsChar('.')) 
                                tt1 = tt1.Next;
                            else if (txt == "С" && tt1.Next != null && ((tt1.Next.Morph.Case.IsInstrumental || tt1.Next.Morph.Case.IsGenitive))) 
                            {
                                if (!(tt1.Next is Pullenti.Ner.TextToken)) 
                                    return null;
                                if (!tt.Chars.IsAllLower || !tt1.Next.Chars.IsCapitalUpper) 
                                    return null;
                                if (tt1.Next.IsNewlineAfter) 
                                {
                                }
                                else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(tt1.Next.Next, false)) 
                                {
                                }
                                else 
                                    return null;
                            }
                            Pullenti.Ner.Token tt2 = tt1.Next;
                            if ((tt2 != null && tt2.LengthChar == 1 && tt2.Chars.IsCyrillicLetter) && tt2.Chars.IsAllUpper) 
                            {
                                if (tt2.Next != null && ((tt2.Next.IsChar('.') || tt2.Next.IsHiphen)) && !tt2.IsWhitespaceAfter) 
                                    tt2 = tt2.Next.Next;
                            }
                            bool ok = false;
                            if (txt == "Д" && (tt2 is Pullenti.Ner.NumberToken) && !tt2.IsNewlineBefore) 
                                ok = false;
                            else if (((txt == "С" || txt == "C")) && (tt2 is Pullenti.Ner.TextToken) && ((tt2.IsValue("О", null) || tt2.IsValue("O", null)))) 
                                ok = false;
                            else if (tt2 != null && tt2.Chars.IsCapitalUpper && (tt2.WhitespacesBeforeCount < 2)) 
                                ok = tt.Chars.IsAllLower;
                            else if (tt2 != null && tt2.Chars.IsAllUpper && (tt2.WhitespacesBeforeCount < 2)) 
                            {
                                ok = true;
                                if (tt.Chars.IsAllUpper) 
                                {
                                    Pullenti.Ner.ReferentToken rtt = tt.Kit.ProcessReferent("PERSON", tt, null);
                                    if (rtt != null) 
                                    {
                                        ok = false;
                                        Pullenti.Ner.Token ttt2 = rtt.EndToken.Next;
                                        if (ttt2 != null && ttt2.IsComma) 
                                            ttt2 = ttt2.Next;
                                        if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(ttt2, false, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(ttt2, false)) 
                                            ok = true;
                                    }
                                    else if (tt.Previous != null && tt.Previous.IsChar('.')) 
                                        ok = false;
                                }
                                else if (tt1 == tt) 
                                    ok = false;
                                if (!ok && tt1.Next != null) 
                                {
                                    Pullenti.Ner.Token ttt2 = tt1.Next.Next;
                                    if (ttt2 != null && ttt2.IsComma) 
                                        ttt2 = ttt2.Next;
                                    if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(ttt2, false, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(ttt2, false)) 
                                    {
                                        if (OrgItemToken.TryParse(tt1.Next, null) != null) 
                                        {
                                        }
                                        else 
                                            ok = true;
                                    }
                                }
                            }
                            else if (prev != null && prev.Typ == ItemType.ProperName && (tt.WhitespacesBeforeCount < 2)) 
                            {
                                if (MiscLocationHelper.CheckGeoObjectBefore(prev.BeginToken.Previous, false)) 
                                    ok = true;
                                if (txt == "П" && tt.Next != null && ((tt.Next.IsHiphen || tt.Next.IsCharOf("\\/")))) 
                                {
                                    Pullenti.Ner.Address.Internal.StreetItemToken sit = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(tt, null, false, null);
                                    if (sit != null && sit.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                                        ok = false;
                                }
                            }
                            else if (prev == null) 
                            {
                                if (MiscLocationHelper.CheckGeoObjectBefore(tt.Previous, false)) 
                                {
                                    if (tt1.IsNewlineAfter) 
                                    {
                                    }
                                    else 
                                        ok = true;
                                }
                                else if (geoAfter != null || hasGeoAfter) 
                                    ok = true;
                            }
                            if (tt.Previous != null && tt.Previous.IsHiphen && !tt.IsWhitespaceBefore) 
                            {
                                if (tt.Next != null && tt.Next.IsChar('.')) 
                                {
                                }
                                else 
                                    ok = false;
                            }
                            if (ok) 
                            {
                                int ii = 0;
                                for (Pullenti.Ner.Token ttt = t.Previous; ttt != null && (ii < 4); ttt = ttt.Previous,ii++) 
                                {
                                    OrgItemToken oo = OrgItemToken.TryParse(ttt, null);
                                    if (oo != null && oo.EndChar > tt.EndChar) 
                                        ok = false;
                                }
                            }
                            if (ok) 
                            {
                                res = new CityItemToken(tt0, tt1) { Typ = ItemType.Noun };
                                res.Value = (txt == "Д" ? "ДЕРЕВНЯ" : (txt == "П" ? "ПОСЕЛОК" : (txt == "Х" ? "ХУТОР" : "СЕЛО")));
                                if (txt == "П") 
                                    res.AltValue = "ПОСЕЛЕНИЕ";
                                else if (txt == "С" || txt == "C") 
                                {
                                    res.AltValue = "СЕЛЕНИЕ";
                                    if (tt0 == tt1) 
                                    {
                                        Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt1.Next, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePronouns, 0, null);
                                        if (npt != null && npt.Morph.Case.IsInstrumental) 
                                            return null;
                                    }
                                }
                                res.Doubtful = true;
                                return res;
                            }
                        }
                        if ((txt == "СП" || txt == "РП" || txt == "ГП") || txt == "ДП") 
                        {
                            if (tt.Next != null && tt.Next.IsChar('.')) 
                                tt = tt.Next;
                            if (tt.Next != null && tt.Next.Chars.IsCapitalUpper) 
                                return new CityItemToken(tt0, tt) { Typ = ItemType.Noun, Doubtful = true, Cond = new Condition() { GeoBeforeToken = t }, Value = (txt == "РП" ? "РАБОЧИЙ ПОСЕЛОК" : (txt == "ГП" ? "ГОРОДСКОЕ ПОСЕЛЕНИЕ" : (txt == "ДП" ? "ДАЧНЫЙ ПОСЕЛОК" : "СЕЛЬСКОЕ ПОСЕЛЕНИЕ"))) };
                        }
                        if (tt0 != tt && CityItemToken.CheckKeyword(tt) != null) 
                        {
                            res = TryParse(tt, null, false, ad);
                            if (res != null && res.Typ == ItemType.Noun) 
                            {
                                res.GeoObjectBefore = true;
                                res.BeginToken = tt0;
                                return res;
                            }
                        }
                        if (tt.Chars.IsAllUpper && tt.LengthChar > 2 && tt.Chars.IsCyrillicLetter) 
                            return new CityItemToken(tt, tt) { Typ = ItemType.ProperName, Value = (tt as Pullenti.Ner.TextToken).Term };
                    }
                }
                if ((t is Pullenti.Ner.NumberToken) && t.Next != null) 
                {
                    Pullenti.Ner.Core.NumberExToken net = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t);
                    if (net != null && net.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer) 
                        return new CityItemToken(t, net.EndToken) { Typ = ItemType.ProperName, Value = string.Format("{0}КМ", (int)net.RealValue) };
                }
                Pullenti.Ner.ReferentToken rt = t as Pullenti.Ner.ReferentToken;
                if ((rt != null && (rt.Referent is Pullenti.Ner.Geo.GeoReferent) && rt.BeginToken == rt.EndToken) && (rt.Referent as Pullenti.Ner.Geo.GeoReferent).IsState) 
                {
                    if (t.Previous == null) 
                        return null;
                    if (t.Previous.Morph.Number == Pullenti.Morph.MorphNumber.Singular && t.Morph.Case.IsNominative && !t.Morph.Case.IsGenitive) 
                        return new CityItemToken(t, t) { Typ = ItemType.ProperName, Value = rt.GetSourceText().ToUpper() };
                }
                return null;
            }
            if (res.Typ == ItemType.Noun) 
            {
                if (res.Value == "СЕЛО" && (t is Pullenti.Ner.TextToken)) 
                {
                    if (t.Previous == null) 
                    {
                    }
                    else if (t.Previous.Morph.Class.IsPreposition) 
                    {
                    }
                    else 
                        res.Doubtful = true;
                    res.Morph.Gender = Pullenti.Morph.MorphGender.Neuter;
                }
                if (Pullenti.Morph.LanguageHelper.EndsWith(res.Value, "УСАДЬБА") && res.AltValue == null) 
                    res.AltValue = "НАСЕЛЕННЫЙ ПУНКТ";
                if (res.Value == "СТАНЦИЯ" || res.Value == "СТАНЦІЯ") 
                    res.Doubtful = true;
                if (res.EndToken.IsValue("СТОЛИЦА", null) || res.EndToken.IsValue("СТОЛИЦЯ", null)) 
                {
                    res.Doubtful = true;
                    if (res.EndToken.Next != null) 
                    {
                        Pullenti.Ner.Geo.GeoReferent geo = res.EndToken.Next.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                        if (geo != null && ((geo.IsRegion || geo.IsState))) 
                        {
                            res.HigherGeo = geo;
                            res.EndToken = res.EndToken.Next;
                            res.Doubtful = false;
                            res.Value = "ГОРОД";
                            foreach (Pullenti.Ner.Core.Termin it in TerrItemToken.m_CapitalsByState.Termins) 
                            {
                                Pullenti.Ner.Geo.GeoReferent ge = it.Tag as Pullenti.Ner.Geo.GeoReferent;
                                if (ge == null || !ge.CanBeEquals(geo, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                                    continue;
                                Pullenti.Ner.Core.TerminToken tok = TerrItemToken.m_CapitalsByState.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                                if (tok != null && tok.Termin == it) 
                                    break;
                                res.Typ = ItemType.City;
                                res.Value = it.CanonicText;
                                return res;
                            }
                        }
                    }
                }
                if ((res.BeginToken.LengthChar == 1 && res.BeginToken.Chars.IsAllUpper && res.BeginToken.Next != null) && res.BeginToken.Next.IsChar('.')) 
                {
                    CityItemToken ne = _tryParseInt(res.BeginToken.Next.Next, null, false, ad);
                    if (ne != null && ne.Typ == ItemType.City) 
                    {
                    }
                    else if (ne != null && ne.Typ == ItemType.ProperName && Pullenti.Morph.LanguageHelper.EndsWithEx(ne.Value, "К", "О", null, null)) 
                    {
                    }
                    else 
                        return null;
                }
            }
            if (res.Typ == ItemType.ProperName || res.Typ == ItemType.City) 
            {
                string val = res.Value ?? ((res.OntoItem == null ? null : res.OntoItem.CanonicText));
                Pullenti.Ner.Token t1 = res.EndToken;
                if (((!t1.IsWhitespaceAfter && t1.Next != null && t1.Next.IsHiphen) && !t1.Next.IsWhitespaceAfter && (t1.Next.Next is Pullenti.Ner.NumberToken)) && (t1.Next.Next as Pullenti.Ner.NumberToken).IntValue != null && ((t1.Next.Next as Pullenti.Ner.NumberToken).IntValue.Value < 30)) 
                {
                    res.EndToken = t1.Next.Next;
                    res.Value = string.Format("{0}-{1}", val, (t1.Next.Next as Pullenti.Ner.NumberToken).Value);
                    if (res.AltValue != null) 
                        res.AltValue = string.Format("{0}-{1}", res.AltValue, (t1.Next.Next as Pullenti.Ner.NumberToken).Value);
                    res.Typ = ItemType.ProperName;
                }
                else if (t1.WhitespacesAfterCount == 1 && (t1.Next is Pullenti.Ner.NumberToken) && t1.Next.Morph.Class.IsAdjective) 
                {
                    bool ok = false;
                    if (t1.Next.Next == null || t1.Next.IsNewlineAfter) 
                        ok = true;
                    else if (!t1.Next.IsWhitespaceAfter && t1.Next.Next != null && t1.Next.Next.IsCharOf(",")) 
                        ok = true;
                    else if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t1.Next.Next) && t1.WhitespacesAfterCount <= t1.Next.WhitespacesAfterCount) 
                    {
                        if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(t1.Next.Next, false)) 
                            ok = true;
                    }
                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t1.Next.Next, false, false)) 
                        ok = true;
                    if (ok) 
                    {
                        res.EndToken = t1.Next;
                        res.Value = string.Format("{0}-{1}", val, (t1.Next as Pullenti.Ner.NumberToken).Value);
                        if (res.AltValue != null) 
                            res.AltValue = string.Format("{0}-{1}", res.AltValue, (t1.Next as Pullenti.Ner.NumberToken).Value);
                        res.Typ = ItemType.ProperName;
                    }
                }
            }
            if (res.Typ == ItemType.City && res.BeginToken == res.EndToken) 
            {
                if (res.BeginToken.GetMorphClassInDictionary().IsAdjective && (res.EndToken.Next is Pullenti.Ner.TextToken)) 
                {
                    bool ok = false;
                    Pullenti.Ner.Token t1 = null;
                    Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(res.BeginToken);
                    if (npt != null && npt.EndToken == res.EndToken.Next) 
                    {
                        t1 = npt.EndToken;
                        Pullenti.Morph.MorphClass mc = t1.GetMorphClassInDictionary();
                        if (mc.IsNoun) 
                        {
                            if (res.EndToken.Next.Chars.Equals(res.BeginToken.Chars)) 
                            {
                                ok = true;
                                if (res.BeginToken.Chars.IsAllUpper) 
                                {
                                    CityItemToken cii = _tryParseInt(res.EndToken.Next, null, dontNormalize, ad);
                                    if (cii != null && cii.Typ == ItemType.Noun) 
                                        ok = false;
                                }
                            }
                            else if (res.EndToken.Next.Chars.IsAllLower) 
                            {
                                Pullenti.Ner.Token ttt = res.EndToken.Next.Next;
                                if (ttt == null || ttt.IsCharOf(",.")) 
                                    ok = true;
                            }
                        }
                    }
                    else if (res.EndToken.Next.Chars.Equals(res.BeginToken.Chars) && res.BeginToken.Chars.IsCapitalUpper) 
                    {
                        Pullenti.Ner.Token ttt = res.EndToken.Next.Next;
                        if (ttt == null || ttt.IsCharOf(",.")) 
                            ok = true;
                        t1 = res.EndToken.Next;
                        npt = null;
                    }
                    if (ok && t1 != null) 
                    {
                        res.Typ = ItemType.ProperName;
                        res.OntoItem = null;
                        res.EndToken = t1;
                        if (npt != null) 
                        {
                            res.Value = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                            res.Morph = npt.Morph;
                        }
                        else 
                            res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    }
                }
                if ((res.EndToken.Next != null && res.EndToken.Next.IsHiphen && !res.EndToken.Next.IsWhitespaceAfter) && !res.EndToken.Next.IsWhitespaceBefore) 
                {
                    CityItemToken res1 = _TryParse(res.EndToken.Next.Next, null, false, ad);
                    if ((res1 != null && res1.Typ == ItemType.ProperName && res1.BeginToken == res1.EndToken) && res1.BeginToken.Chars.Equals(res.BeginToken.Chars)) 
                    {
                        if (res1.OntoItem == null && res.OntoItem == null) 
                        {
                            res.Typ = ItemType.ProperName;
                            res.Value = string.Format("{0}-{1}", (res.OntoItem == null ? res.Value : res.OntoItem.CanonicText), res1.Value);
                            if (res.AltValue != null) 
                                res.AltValue = string.Format("{0}-{1}", res.AltValue, res1.Value);
                            res.OntoItem = null;
                            res.EndToken = res1.EndToken;
                            res.Doubtful = false;
                        }
                    }
                    else if ((res.EndToken.Next.Next is Pullenti.Ner.NumberToken) && (res.EndToken.Next.Next as Pullenti.Ner.NumberToken).IntValue != null && ((res.EndToken.Next.Next as Pullenti.Ner.NumberToken).IntValue.Value < 30)) 
                    {
                        res.Typ = ItemType.ProperName;
                        res.Value = string.Format("{0}-{1}", (res.OntoItem == null ? res.Value : res.OntoItem.CanonicText), (res.EndToken.Next.Next as Pullenti.Ner.NumberToken).Value);
                        if (res.AltValue != null) 
                            res.AltValue = string.Format("{0}-{1}", res.AltValue, (res.EndToken.Next.Next as Pullenti.Ner.NumberToken).Value);
                        res.OntoItem = null;
                        res.EndToken = res.EndToken.Next.Next;
                    }
                }
                else if (res.BeginToken.GetMorphClassInDictionary().IsProperName) 
                {
                    if (res.BeginToken.IsValue("КИЇВ", null) || res.BeginToken.IsValue("АСТАНА", null) || res.BeginToken.IsValue("АЛМАТЫ", null)) 
                    {
                    }
                    else if ((res.EndToken is Pullenti.Ner.TextToken) && Pullenti.Morph.LanguageHelper.EndsWith((res.EndToken as Pullenti.Ner.TextToken).Term, "ВО")) 
                    {
                    }
                    else 
                    {
                        res.Doubtful = true;
                        Pullenti.Ner.Token tt = res.BeginToken.Previous;
                        if (tt != null && tt.Previous != null) 
                        {
                            if (tt.IsChar(',') || tt.Morph.Class.IsConjunction) 
                            {
                                Pullenti.Ner.Geo.GeoReferent geo = tt.Previous.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                                if (geo != null && geo.IsCity) 
                                    res.Doubtful = false;
                            }
                        }
                        if (tt != null && tt.IsValue("В", null) && tt.Chars.IsAllLower) 
                        {
                            Pullenti.Ner.Core.NounPhraseToken npt1 = MiscLocationHelper.TryParseNpt(res.BeginToken);
                            if (npt1 == null || npt1.EndChar <= res.EndChar) 
                                res.Doubtful = false;
                        }
                    }
                }
                if ((res.BeginToken == res.EndToken && res.Typ == ItemType.City && res.OntoItem != null) && res.OntoItem.CanonicText == "САНКТ - ПЕТЕРБУРГ") 
                {
                    for (Pullenti.Ner.Token tt = res.BeginToken.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (tt.IsHiphen || tt.IsChar('.')) 
                            continue;
                        if (tt.IsValue("С", null) || tt.IsValue("C", null) || tt.IsValue("САНКТ", null)) 
                            res.BeginToken = tt;
                        break;
                    }
                }
            }
            if ((res.BeginToken == res.EndToken && res.Typ == ItemType.ProperName && res.WhitespacesAfterCount == 1) && (res.EndToken.Next is Pullenti.Ner.TextToken) && res.EndToken.Chars.Equals(res.EndToken.Next.Chars)) 
            {
                bool ok = false;
                Pullenti.Ner.Token t1 = res.EndToken;
                if (t1.Next.Next == null || t1.Next.IsNewlineAfter) 
                    ok = true;
                else if (!t1.Next.IsWhitespaceAfter && t1.Next.Next != null && t1.Next.Next.IsCharOf(",.")) 
                    ok = true;
                if (ok) 
                {
                    CityItemToken pp = _TryParse(t1.Next, null, false, ad);
                    if (pp != null && pp.Typ == ItemType.Noun) 
                        ok = false;
                    if (ok) 
                    {
                        TerrItemToken te = TerrItemToken.TryParse(t1.Next, null, null);
                        if (te != null) 
                        {
                            if (te.TerminItem != null) 
                                ok = false;
                            else if (te.OntoItem != null && !te.Morph.Case.IsGenitive) 
                                ok = false;
                        }
                    }
                }
                if (ok) 
                {
                    res.EndToken = t1.Next;
                    res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValue(res.BeginToken, res.EndToken, Pullenti.Ner.Core.GetTextAttr.No);
                    res.AltValue = null;
                    res.Typ = ItemType.ProperName;
                }
            }
            return res;
        }
        static CityItemToken _TryParse(Pullenti.Ner.Token t, CityItemToken prev, bool dontNormalize, GeoAnalyzerData ad)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
            {
                if ((t is Pullenti.Ner.ReferentToken) && (t.GetReferent() is Pullenti.Ner.Date.DateReferent)) 
                {
                    List<Pullenti.Ner.Address.Internal.StreetItemToken> aii = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseSpec(t, null);
                    if (aii != null) 
                    {
                        if (aii.Count > 1 && aii[0].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Number && aii[1].Typ == Pullenti.Ner.Address.Internal.StreetItemType.StdName) 
                        {
                            CityItemToken res2 = new CityItemToken(t, aii[1].EndToken) { Typ = ItemType.ProperName };
                            res2.Value = string.Format("{0} {1}", (aii[0].Number == null ? aii[0].Value : aii[0].Number.IntValue.Value.ToString()), aii[1].Value);
                            return res2;
                        }
                    }
                }
                if ((((t is Pullenti.Ner.NumberToken) && prev != null && prev.Typ == ItemType.Noun) && (t.WhitespacesBeforeCount < 3) && (t.WhitespacesAfterCount < 3)) && (t.Next is Pullenti.Ner.TextToken) && t.Next.Chars.IsCapitalUpper) 
                {
                    if (prev.BeginToken.IsValue("СТ", null) || prev.BeginToken.IsValue("П", null)) 
                        return null;
                    CityItemToken cit1 = TryParse(t.Next, null, false, ad);
                    if (cit1 != null && cit1.Typ == ItemType.ProperName && cit1.Value != null) 
                    {
                        cit1.BeginToken = t;
                        cit1.Value = string.Format("{0}-{1}", cit1.Value, (t as Pullenti.Ner.NumberToken).Value);
                        return cit1;
                    }
                }
                return null;
            }
            CityItemToken spec = _tryParseSpec(t);
            if (spec != null) 
                return spec;
            List<Pullenti.Ner.Core.IntOntologyToken> li = null;
            List<Pullenti.Ner.Core.IntOntologyToken> li0 = null;
            bool isInLocOnto = false;
            if (t.Kit.Ontology != null && li == null) 
            {
                if ((((li0 = t.Kit.Ontology.AttachToken(Pullenti.Ner.Geo.GeoReferent.OBJ_TYPENAME, t)))) != null) 
                {
                    li = li0;
                    isInLocOnto = true;
                }
            }
            if (li == null) 
            {
                li = m_Ontology.TryAttach(t, null, false);
                if (li == null) 
                    li = m_OntologyEx.TryAttach(t, null, false);
            }
            else if (prev == null) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken stri = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(t.Previous, null, false, ad);
                if (stri != null && stri.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                    return null;
                stri = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(li[0].EndToken.Next, null, false, ad);
                if (stri != null && stri.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                    return null;
            }
            if (li != null && li.Count > 0) 
            {
                if (t is Pullenti.Ner.TextToken) 
                {
                    for (int i = li.Count - 1; i >= 0; i--) 
                    {
                        if (li[i].Item != null) 
                        {
                            Pullenti.Ner.Geo.GeoReferent g = li[i].Item.Referent as Pullenti.Ner.Geo.GeoReferent;
                            if (g != null) 
                            {
                                if (!g.IsCity) 
                                {
                                    li.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                    }
                    Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
                    foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                    {
                        if (nt.Item != null && nt.Item.CanonicText == tt.Term) 
                        {
                            if (!Pullenti.Ner.Core.MiscHelper.IsAllCharactersLower(nt.BeginToken, nt.EndToken, false)) 
                            {
                                CityItemToken ci = new CityItemToken(nt.BeginToken, nt.EndToken) { Typ = ItemType.City, OntoItem = nt.Item, Morph = nt.Morph };
                                if (nt.BeginToken == nt.EndToken && !isInLocOnto) 
                                    ci.Doubtful = CheckDoubtful(nt.BeginToken as Pullenti.Ner.TextToken);
                                Pullenti.Ner.Token tt1 = nt.EndToken.Next;
                                if ((((tt1 != null && tt1.IsHiphen && !tt1.IsWhitespaceBefore) && !tt1.IsWhitespaceAfter && prev != null) && prev.Typ == ItemType.Noun && (tt1.Next is Pullenti.Ner.TextToken)) && tt1.Previous.Chars.Equals(tt1.Next.Chars)) 
                                {
                                    li = null;
                                    break;
                                }
                                return ci;
                            }
                        }
                    }
                    if (li != null) 
                    {
                        foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                        {
                            if (nt.Item != null) 
                            {
                                if (!Pullenti.Ner.Core.MiscHelper.IsAllCharactersLower(nt.BeginToken, nt.EndToken, false)) 
                                {
                                    CityItemToken ci = new CityItemToken(nt.BeginToken, nt.EndToken) { Typ = ItemType.City, OntoItem = nt.Item, Morph = nt.Morph };
                                    if (nt.BeginToken == nt.EndToken && (nt.BeginToken is Pullenti.Ner.TextToken)) 
                                    {
                                        ci.Doubtful = CheckDoubtful(nt.BeginToken as Pullenti.Ner.TextToken);
                                        string str = (nt.BeginToken as Pullenti.Ner.TextToken).Term;
                                        if (str != nt.Item.CanonicText) 
                                        {
                                            if (Pullenti.Morph.LanguageHelper.EndsWithEx(str, "О", "А", null, null)) 
                                                ci.AltValue = str;
                                        }
                                    }
                                    return ci;
                                }
                            }
                        }
                    }
                }
                if (li != null) 
                {
                    foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                    {
                        if (nt.Item == null) 
                        {
                            ItemType ty = (nt.Termin.Tag == null ? ItemType.Noun : (ItemType)nt.Termin.Tag);
                            CityItemToken ci = new CityItemToken(nt.BeginToken, nt.EndToken) { Typ = ty, Morph = nt.Morph };
                            ci.Value = nt.Termin.CanonicText;
                            if (ty == ItemType.Misc && ci.Value == "ЖИТЕЛЬ" && t.Previous != null) 
                            {
                                if (t.Previous.IsValue("МЕСТНЫЙ", "МІСЦЕВИЙ")) 
                                    return null;
                                if (t.Previous.Morph.Class.IsPronoun) 
                                    return null;
                            }
                            if (ty == ItemType.Noun && !t.Chars.IsAllLower) 
                            {
                                if (t.Morph.Class.IsProperSurname) 
                                    ci.Doubtful = true;
                            }
                            if (nt.BeginToken == nt.EndToken) 
                            {
                                if (t.IsValue("СТ", null)) 
                                {
                                    if (OrgItemToken.TryParse(t, null) != null) 
                                        return null;
                                }
                            }
                            if (nt.BeginToken.Kit.BaseLanguage.IsUa) 
                            {
                                if (nt.BeginToken.IsValue("М", null) || nt.BeginToken.IsValue("Г", null)) 
                                {
                                    if (!nt.BeginToken.Chars.IsAllLower) 
                                        return null;
                                    ci.Doubtful = true;
                                }
                                else if (nt.BeginToken.IsValue("МІС", null)) 
                                {
                                    if ((t as Pullenti.Ner.TextToken).Term != "МІС") 
                                        return null;
                                    ci.Doubtful = true;
                                }
                            }
                            if (nt.BeginToken.Kit.BaseLanguage.IsRu) 
                            {
                                if (nt.BeginToken.IsValue("Г", null)) 
                                {
                                    if (nt.BeginToken.Previous != null && nt.BeginToken.Previous.Morph.Class.IsPreposition) 
                                    {
                                    }
                                    else 
                                    {
                                        bool ok = true;
                                        if (!nt.BeginToken.Chars.IsAllLower) 
                                        {
                                            ok = false;
                                            if (nt.EndToken.Next != null && !nt.EndToken.Next.Chars.IsAllUpper) 
                                                return null;
                                        }
                                        else if ((nt.EndToken == nt.BeginToken && nt.EndToken.Next != null && !nt.EndToken.IsWhitespaceAfter) && ((nt.EndToken.Next.IsCharOf("\\/") || nt.EndToken.Next.IsHiphen))) 
                                            ok = false;
                                        else if (!t.IsWhitespaceBefore && t.Previous != null && ((t.Previous.IsCharOf("\\/") || t.Previous.IsHiphen))) 
                                            return null;
                                        if (!ok) 
                                        {
                                            CityItemToken nex = TryParse(nt.EndToken.Next, null, false, null);
                                            if (nex != null && nex.Typ == ItemType.City && (nt.EndToken.WhitespacesAfterCount < 4)) 
                                            {
                                            }
                                            else 
                                                return null;
                                        }
                                    }
                                    ci.Doubtful = true;
                                }
                                else if (nt.BeginToken.IsValue("ГОР", null)) 
                                {
                                    if ((t as Pullenti.Ner.TextToken).Term != "ГОР") 
                                    {
                                        if (t.Chars.IsCapitalUpper) 
                                        {
                                            ci = null;
                                            break;
                                        }
                                        return null;
                                    }
                                    ci.Doubtful = true;
                                }
                                else if (nt.BeginToken.IsValue("ПОС", null)) 
                                {
                                    if ((t as Pullenti.Ner.TextToken).Term != "ПОС") 
                                        return null;
                                    ci.Doubtful = true;
                                }
                            }
                            Pullenti.Ner.Core.NounPhraseToken npt1 = MiscLocationHelper.TryParseNpt(t.Previous);
                            if (npt1 != null && npt1.Adjectives.Count > 0) 
                            {
                                string s = npt1.Adjectives[0].GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                                if ((s == "РОДНОЙ" || s == "ЛЮБИМЫЙ" || s == "РІДНИЙ") || s == "КОХАНИЙ") 
                                    return null;
                            }
                            if (t.IsValue("ПОСЕЛЕНИЕ", null)) 
                            {
                                if (t.Next != null && t.Next.IsValue("СТАНЦИЯ", null)) 
                                {
                                    CityItemToken ci1 = TryParse(t.Next.Next, null, false, null);
                                    if (ci1 != null && ((ci1.Typ == ItemType.ProperName || ci1.Typ == ItemType.City))) 
                                        ci.EndToken = t.Next;
                                }
                            }
                            return ci;
                        }
                    }
                }
            }
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            if ((t as Pullenti.Ner.TextToken).Term == "СПБ" && !t.Chars.IsAllLower && m_StPeterburg != null) 
                return new CityItemToken(t, t) { Typ = ItemType.City, OntoItem = m_StPeterburg, Value = m_StPeterburg.CanonicText };
            if (t.Chars.IsAllLower) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> stds = m_StdAdjectives.TryAttach(t, null, false);
            if (stds != null) 
            {
                CityItemToken cit = TryParse(stds[0].EndToken.Next, null, false, ad);
                if (cit != null && ((((cit.Typ == ItemType.ProperName && cit.Value != null)) || cit.Typ == ItemType.City))) 
                {
                    string adj = stds[0].Termin.CanonicText;
                    cit.Value = string.Format("{0} {1}", adj, cit.Value ?? cit.OntoItem.CanonicText);
                    if (cit.AltValue != null) 
                        cit.AltValue = string.Format("{0} {1}", adj, cit.AltValue);
                    cit.BeginToken = t;
                    Pullenti.Ner.Core.NounPhraseToken npt0 = MiscLocationHelper.TryParseNpt(t);
                    if (npt0 != null && npt0.EndToken == cit.EndToken) 
                    {
                        cit.Morph = npt0.Morph;
                        cit.Value = npt0.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false);
                    }
                    cit.Typ = ItemType.ProperName;
                    cit.Doubtful = false;
                    return cit;
                }
            }
            Pullenti.Ner.Token t1 = t;
            bool doubt = false;
            StringBuilder name = new StringBuilder();
            StringBuilder altname = null;
            int k = 0;
            bool isPrep = false;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (!(tt is Pullenti.Ner.TextToken)) 
                    break;
                if (!tt.Chars.IsLetter || ((tt.Chars.IsCyrillicLetter != t.Chars.IsCyrillicLetter && !tt.IsValue("НА", null)))) 
                    break;
                if (tt != t) 
                {
                    Pullenti.Ner.Address.Internal.StreetItemToken si = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(tt, null, false, null);
                    if (si != null && si.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                    {
                        if (si.EndToken.Next == null || si.EndToken.Next.IsCharOf(",.")) 
                        {
                        }
                        else 
                            break;
                    }
                    if (tt.LengthChar < 2) 
                        break;
                    if ((tt.LengthChar < 3) && !tt.IsValue("НА", null)) 
                    {
                        if (tt.IsWhitespaceBefore) 
                            break;
                    }
                }
                if (name.Length > 0) 
                {
                    name.Append('-');
                    if (altname != null) 
                        altname.Append('-');
                }
                if ((tt is Pullenti.Ner.TextToken) && ((isPrep || ((k > 0 && !tt.GetMorphClassInDictionary().IsProperGeo))))) 
                {
                    name.Append((tt as Pullenti.Ner.TextToken).Term);
                    if (altname != null) 
                        altname.Append((tt as Pullenti.Ner.TextToken).Term);
                }
                else 
                {
                    string ss = (dontNormalize ? (tt as Pullenti.Ner.TextToken).Term : GetNormalGeo(tt));
                    if (ss != (tt as Pullenti.Ner.TextToken).Term) 
                    {
                        if (altname == null) 
                            altname = new StringBuilder();
                        altname.Append(name.ToString());
                        altname.Append((tt as Pullenti.Ner.TextToken).Term);
                    }
                    else if (altname != null) 
                        altname.Append(ss);
                    name.Append(ss);
                }
                t1 = tt;
                isPrep = tt.Morph.Class.IsPreposition;
                if (tt.Next == null || tt.Next.Next == null) 
                    break;
                if (!tt.Next.IsHiphen) 
                    break;
                if (dontNormalize) 
                    break;
                if (tt.IsWhitespaceAfter || tt.Next.IsWhitespaceAfter) 
                {
                    if (tt.WhitespacesAfterCount > 1 || tt.Next.WhitespacesAfterCount > 1) 
                        break;
                    if (!tt.Next.Next.Chars.Equals(tt.Chars)) 
                        break;
                    Pullenti.Ner.Token ttt = tt.Next.Next.Next;
                    if (ttt != null && !ttt.IsNewlineAfter) 
                    {
                        if (ttt.Chars.IsLetter) 
                            break;
                    }
                }
                tt = tt.Next;
                k++;
            }
            if (k > 0) 
            {
                if (k > 2) 
                    return null;
                CityItemToken reee = new CityItemToken(t, t1) { Typ = ItemType.ProperName, Value = name.ToString(), Doubtful = doubt };
                if (altname != null) 
                    reee.AltValue = altname.ToString();
                return reee;
            }
            if (t == null) 
                return null;
            Pullenti.Ner.Core.NounPhraseToken npt = (t.Chars.IsLatinLetter ? null : Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.ReferentCanBeNoun, 0, null));
            if (npt != null && (npt.EndToken is Pullenti.Ner.ReferentToken) && (npt.EndToken as Pullenti.Ner.ReferentToken).BeginToken != (npt.EndToken as Pullenti.Ner.ReferentToken).EndToken) 
                npt = null;
            if ((npt != null && npt.EndToken != t && npt.Adjectives.Count > 0) && !npt.Adjectives[0].EndToken.Next.IsComma) 
            {
                CityItemToken cit = TryParse(t.Next, null, false, null);
                if (cit != null && cit.Typ == ItemType.Noun && ((Pullenti.Morph.LanguageHelper.EndsWithEx(cit.Value, "ПУНКТ", "ПОСЕЛЕНИЕ", "ПОСЕЛЕННЯ", "ПОСЕЛОК") || t.Next.IsValue("ГОРОДОК", null) || t.Next.IsValue("СЕЛО", null)))) 
                {
                    bool ok2 = false;
                    Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                    if (!mc.IsAdjective) 
                        ok2 = true;
                    else if (!Pullenti.Ner.Core.MiscHelper.CanBeStartOfSentence(t)) 
                        ok2 = true;
                    else if (MiscLocationHelper.CheckGeoObjectBefore(t, false)) 
                        ok2 = true;
                    if (ok2) 
                        return new CityItemToken(t, t) { Typ = ItemType.City, Value = t.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false), Morph = npt.Morph };
                }
                bool check = true;
                if (!npt.EndToken.Chars.Equals(t.Chars)) 
                {
                    if (npt.EndToken.IsValue("КИЛОМЕТР", null)) 
                        npt = null;
                    else if (OrgItemToken.TryParse(t.Next, null) != null) 
                        npt = null;
                    else if (npt.EndToken.Chars.IsAllLower && ((npt.EndToken.Next == null || npt.EndToken.Next.IsComma || Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(npt.EndToken.Next, false)))) 
                    {
                    }
                    else 
                    {
                        Pullenti.Ner.Address.Internal.AddressItemToken aid = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(t.Next, false, null, null);
                        if (aid != null) 
                            npt = null;
                        else if (prev != null && prev.Typ == ItemType.Noun && CityAttachHelper.CheckCityAfter(t.Next)) 
                            check = false;
                        else 
                        {
                            Pullenti.Ner.ReferentToken rt1 = t.Kit.ProcessReferent("NAMEDENTITY", t, null);
                            if (rt1 != null && rt1.EndToken == npt.EndToken) 
                            {
                            }
                            else 
                                npt = null;
                        }
                    }
                }
                if (check && !dontNormalize && npt != null) 
                {
                    OrgItemToken org1 = OrgItemToken.TryParse(t.Next, null);
                    if (org1 != null && !org1.IsDoubt) 
                    {
                        OrgItemToken org0 = OrgItemToken.TryParse(t, null);
                        if (org0 != null && org0.IsDoubt) 
                            npt = null;
                    }
                }
                if (check && !dontNormalize && npt != null) 
                {
                    if (npt.Adjectives.Count != 1) 
                        return null;
                    Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(npt.Noun.BeginToken);
                    if (ter != null) 
                        npt = null;
                    else 
                    {
                        Pullenti.Ner.Core.NounPhraseToken npt1 = MiscLocationHelper.TryParseNpt(npt.EndToken);
                        if (npt1 == null || npt1.Adjectives.Count == 0) 
                        {
                            Pullenti.Ner.Address.Internal.StreetItemToken si = Pullenti.Ner.Address.Internal.StreetItemToken.TryParse(npt.EndToken, null, false, null);
                            if ((si == null || si.Typ != Pullenti.Ner.Address.Internal.StreetItemType.Noun || si.Termin.CanonicText == "МОСТ") || si.Termin.CanonicText == "ПАРК" || si.Termin.CanonicText == "САД") 
                            {
                                t1 = npt.EndToken;
                                doubt = CheckDoubtful(t1 as Pullenti.Ner.TextToken);
                                return new CityItemToken(t, t1) { Typ = ItemType.ProperName, Value = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Undefined, Pullenti.Morph.MorphGender.Undefined, false), Doubtful = doubt, Morph = npt.Morph };
                            }
                        }
                    }
                }
            }
            if (t.Next != null && t.Next.Chars.Equals(t.Chars) && !t.IsNewlineAfter) 
            {
                bool ok = false;
                if (TerrItemToken.CheckOntoItem(t.Next) != null) 
                {
                }
                else if (t.Next.Next == null || !t.Next.Next.Chars.Equals(t.Chars)) 
                    ok = true;
                else if (t.Next.Next.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                    ok = true;
                else 
                {
                    List<TerrItemToken> tis = TerrItemToken.TryParseList(t.Next.Next, 2, null);
                    if (tis != null && tis.Count > 1) 
                    {
                        if (tis[0].IsAdjective && tis[1].TerminItem != null) 
                            ok = true;
                    }
                }
                if (ok && (((t.Next is Pullenti.Ner.TextToken) || (((t.Next is Pullenti.Ner.ReferentToken) && (t.Next as Pullenti.Ner.ReferentToken).BeginToken == (t.Next as Pullenti.Ner.ReferentToken).EndToken))))) 
                {
                    if (t.Next is Pullenti.Ner.TextToken) 
                        doubt = CheckDoubtful(t.Next as Pullenti.Ner.TextToken);
                    Pullenti.Ner.Core.StatisticBigrammInfo stat = t.Kit.Statistics.GetBigrammInfo(t, t.Next);
                    bool ok1 = false;
                    if ((stat != null && stat.PairCount >= 2 && stat.PairCount == stat.SecondCount) && !stat.SecondHasOtherFirst) 
                    {
                        if (stat.PairCount > 2) 
                            doubt = false;
                        ok1 = true;
                    }
                    else if (m_StdAdjectives.TryAttach(t, null, false) != null && (t.Next is Pullenti.Ner.TextToken)) 
                        ok1 = true;
                    else if (((t.Next.Next == null || t.Next.Next.IsComma)) && t.Morph.Class.IsNoun && ((t.Next.Morph.Class.IsAdjective || t.Next.Morph.Class.IsNoun))) 
                        ok1 = true;
                    if (!ok1 && t.Next.Chars.Value == t.Chars.Value) 
                    {
                        if (t.Next.Morph.Case.IsGenitive || (((((Pullenti.Morph.LanguageHelper.EndsWith(name.ToString(), "ОВ") || Pullenti.Morph.LanguageHelper.EndsWith(name.ToString(), "ВО"))) && (t.Next is Pullenti.Ner.TextToken) && !t.Next.Chars.IsAllLower) && t.Next.LengthChar > 1 && !t.Next.GetMorphClassInDictionary().IsUndefined))) 
                        {
                            if (t.Next.IsNewlineAfter) 
                                ok1 = true;
                            else if (MiscLocationHelper.CheckGeoObjectAfter(t.Next, false)) 
                                ok1 = true;
                            else 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken aid = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(t.Next.Next, false, null, null);
                                if (aid != null) 
                                {
                                    if (aid.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street || aid.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Plot || aid.Typ == Pullenti.Ner.Address.Internal.AddressItemType.House) 
                                        ok1 = true;
                                }
                            }
                        }
                    }
                    if (ok1) 
                    {
                        CityItemToken tne = _tryParseInt(t.Next, null, false, ad);
                        if (tne != null && tne.Typ == ItemType.Noun) 
                        {
                        }
                        else 
                        {
                            if (t.Next is Pullenti.Ner.TextToken) 
                            {
                                name.AppendFormat(" {0}", (t.Next as Pullenti.Ner.TextToken).Term);
                                if (altname != null) 
                                    altname.AppendFormat(" {0}", (t.Next as Pullenti.Ner.TextToken).Term);
                            }
                            else 
                            {
                                name.AppendFormat(" {0}", Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t.Next as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.No));
                                if (altname != null) 
                                    altname.AppendFormat(" {0}", Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t.Next as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.No));
                            }
                            t1 = t.Next;
                            return new CityItemToken(t, t1) { Typ = ItemType.ProperName, Value = name.ToString(), AltValue = (altname == null ? null : altname.ToString()), Doubtful = doubt, Morph = t.Next.Morph };
                        }
                    }
                }
            }
            if (t.LengthChar < 2) 
                return null;
            t1 = t;
            doubt = CheckDoubtful(t as Pullenti.Ner.TextToken);
            if (((t.Next != null && prev != null && prev.Typ == ItemType.Noun) && t.Next.Chars.IsCyrillicLetter && t.Next.Chars.IsAllLower) && t.WhitespacesAfterCount == 1) 
            {
                Pullenti.Ner.Token tt = t.Next;
                bool ok = false;
                if (tt.Next == null || tt.Next.IsCharOf(",;")) 
                    ok = true;
                if (ok && Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt.Next, false, null, null) == null) 
                {
                    t1 = tt;
                    name.AppendFormat(" {0}", t1.GetSourceText().ToUpper());
                }
            }
            if (Pullenti.Ner.Core.MiscHelper.IsEngArticle(t)) 
                return null;
            CityItemToken res = new CityItemToken(t, t1) { Typ = ItemType.ProperName, Value = name.ToString(), AltValue = (altname == null ? null : altname.ToString()), Doubtful = doubt, Morph = t.Morph };
            if (t1 == t && (t1 is Pullenti.Ner.TextToken) && (t1 as Pullenti.Ner.TextToken).Term0 != null) 
                res.AltValue = (t1 as Pullenti.Ner.TextToken).Term0;
            bool sog = false;
            bool glas = false;
            foreach (char ch in res.Value) 
            {
                if (Pullenti.Morph.LanguageHelper.IsCyrillicVowel(ch) || Pullenti.Morph.LanguageHelper.IsLatinVowel(ch)) 
                    glas = true;
                else 
                    sog = true;
            }
            if (t.Chars.IsAllUpper && t.LengthChar > 2) 
            {
                if (!glas || !sog) 
                    res.Doubtful = true;
            }
            else if (!glas || !sog) 
                return null;
            if (t == t1 && (t is Pullenti.Ner.TextToken)) 
            {
                if ((t as Pullenti.Ner.TextToken).Term != res.Value) 
                    res.AltValue = (t as Pullenti.Ner.TextToken).Term;
            }
            if ((res.WhitespacesAfterCount < 2) && (res.EndToken.Next is Pullenti.Ner.TextToken)) 
            {
                Pullenti.Ner.Core.TerminToken abbr = m_SpecAbbrs.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                if (abbr != null) 
                {
                    res.EndToken = abbr.EndToken;
                    res.Value = string.Format("{0} {1}", res.Value, abbr.Termin.CanonicText);
                    if (res.AltValue != null) 
                        res.AltValue = string.Format("{0} {1}", res.AltValue, abbr.Termin.CanonicText);
                }
                else if (!res.EndToken.Next.Chars.IsAllLower) 
                {
                    abbr = m_SpecAbbrs.TryParse(res.BeginToken, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (abbr != null && abbr.EndToken == res.EndToken) 
                    {
                        CityItemToken next = _tryParseInt(res.EndToken.Next, null, dontNormalize, ad);
                        if (next != null && ((next.Typ == ItemType.ProperName || next.Typ == ItemType.City))) 
                        {
                            res.EndToken = next.EndToken;
                            res.AltValue = string.Format("{0} {1}", next.Value, res.Value);
                            res.Value = string.Format("{0} {1}", res.Value, next.Value);
                        }
                    }
                }
            }
            return res;
        }
        static CityItemToken _tryParseSpec(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Core.TerminToken tok1 = m_SpecNames.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok1 != null) 
            {
                CityItemToken res = new CityItemToken(t, tok1.EndToken) { Typ = ItemType.ProperName, Value = tok1.Termin.CanonicText };
                if (res.Value == "ЦЕНТРАЛЬНАЯ УСАДЬБА") 
                {
                    CityItemToken res1 = _tryParseSpec(res.EndToken.Next);
                    if (res1 != null) 
                    {
                        res.Value = string.Format("{0} {1}", res1.Value, res.Value);
                        res.EndToken = res1.EndToken;
                    }
                }
                return res;
            }
            tok1 = m_SpecAbbrs.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok1 != null && tok1.Termin.CanonicText == "СОВХОЗ") 
            {
                Pullenti.Ner.Token tt = tok1.EndToken.Next;
                CityItemToken res = null;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(tt, true, false)) 
                {
                    Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(tt, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                    if (br != null) 
                    {
                        res = new CityItemToken(t, br.EndToken) { Typ = ItemType.ProperName };
                        res.Value = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(br, Pullenti.Ner.Core.GetTextAttr.No);
                    }
                }
                else 
                {
                    CityItemToken cit = TryParse(tt, null, false, null);
                    if (cit != null && ((cit.Typ == ItemType.ProperName || cit.Typ == ItemType.City))) 
                        res = cit;
                }
                if (res != null) 
                {
                    res.Typ = ItemType.ProperName;
                    tok1 = m_SpecNames.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok1 != null && tok1.Termin.CanonicText == "ЦЕНТРАЛЬНАЯ УСАДЬБА") 
                    {
                        res.Value = string.Format("{0} {1}", res.Value, tok1.Termin.CanonicText);
                        res.EndToken = tok1.EndToken;
                    }
                    return res;
                }
            }
            return null;
        }
        public static CityItemToken TryParseBack(Pullenti.Ner.Token t, bool onlyNoun = false)
        {
            while (t != null && ((t.IsCharOf("(,") || t.IsAnd))) 
            {
                t = t.Previous;
            }
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            int cou = 0;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Previous) 
            {
                if (!(tt is Pullenti.Ner.TextToken)) 
                    return null;
                if (!tt.Chars.IsLetter) 
                    continue;
                if (onlyNoun) 
                {
                    Pullenti.Ner.Core.IntOntologyToken vv = CheckKeyword(tt);
                    if (vv != null && vv.EndToken == t) 
                        return new CityItemToken(tt, t) { Typ = ItemType.Noun, Value = vv.Termin.CanonicText };
                }
                else 
                {
                    CityItemToken res = TryParse(tt, null, false, null);
                    if (res != null && res.EndToken == t) 
                        return res;
                }
                if ((++cou) > 2) 
                    break;
            }
            return null;
        }
        static string GetNormalGeo(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return null;
            char ch = tt.Term[tt.Term.Length - 1];
            if (((ch == 'О' || ch == 'В' || ch == 'Ы') || ch == 'Х' || ch == 'Ь') || ch == 'Й') 
                return tt.Term;
            foreach (Pullenti.Morph.MorphBaseInfo wf in tt.Morph.Items) 
            {
                if (wf.Class.IsProperGeo && (wf as Pullenti.Morph.MorphWordForm).IsInDictionary) 
                    return (wf as Pullenti.Morph.MorphWordForm).NormalCase;
            }
            bool geoEqTerm = false;
            foreach (Pullenti.Morph.MorphBaseInfo wf in tt.Morph.Items) 
            {
                if (wf.Class.IsProperGeo) 
                {
                    string ggg = (wf as Pullenti.Morph.MorphWordForm).NormalCase;
                    if (ggg == tt.Term) 
                        geoEqTerm = true;
                    else if (!wf.Case.IsNominative) 
                        return ggg;
                }
            }
            if (geoEqTerm) 
                return tt.Term;
            if (tt.Morph.ItemsCount > 0) 
                return (tt.Morph[0] as Pullenti.Morph.MorphWordForm).NormalCase;
            else 
                return tt.Term;
        }
        static bool CheckDoubtful(Pullenti.Ner.TextToken tt)
        {
            if (tt == null) 
                return true;
            if (tt.Chars.IsAllLower) 
                return true;
            if (tt.LengthChar < 3) 
                return true;
            if (((tt.Term == "СОЧИ" || tt.IsValue("КИЕВ", null) || tt.IsValue("ПСКОВ", null)) || tt.IsValue("БОСТОН", null) || tt.IsValue("РИГА", null)) || tt.IsValue("АСТАНА", null) || tt.IsValue("АЛМАТЫ", null)) 
                return false;
            if (Pullenti.Morph.LanguageHelper.EndsWith(tt.Term, "ВО")) 
                return false;
            if ((tt.Next is Pullenti.Ner.TextToken) && (tt.WhitespacesAfterCount < 2) && !tt.Next.Chars.IsAllLower) 
            {
                if (tt.Chars.Equals(tt.Next.Chars) && !tt.Chars.IsLatinLetter && ((!tt.Morph.Case.IsGenitive && !tt.Morph.Case.IsAccusative))) 
                {
                    Pullenti.Morph.MorphClass mc = tt.Next.GetMorphClassInDictionary();
                    if (mc.IsProperSurname || mc.IsProperSecname) 
                        return true;
                }
            }
            if ((tt.Previous is Pullenti.Ner.TextToken) && (tt.WhitespacesBeforeCount < 2) && !tt.Previous.Chars.IsAllLower) 
            {
                Pullenti.Morph.MorphClass mc = tt.Previous.GetMorphClassInDictionary();
                if (mc.IsProperSurname) 
                    return true;
            }
            bool ok = false;
            foreach (Pullenti.Morph.MorphBaseInfo wff in tt.Morph.Items) 
            {
                Pullenti.Morph.MorphWordForm wf = wff as Pullenti.Morph.MorphWordForm;
                if (wf.IsInDictionary) 
                {
                    if (!wf.Class.IsProper) 
                        ok = true;
                    if (wf.Class.IsProperSurname || wf.Class.IsProperName || wf.Class.IsProperSecname) 
                    {
                        if (wf.NormalCase != "ЛОНДОН" && wf.NormalCase != "ЛОНДОНЕ") 
                            ok = true;
                    }
                }
                else if (wf.Class.IsProperSurname) 
                {
                    string val = wf.NormalFull ?? wf.NormalCase ?? "";
                    if (Pullenti.Morph.LanguageHelper.EndsWithEx(val, "ОВ", "ЕВ", "ИН", null)) 
                    {
                        if (val != "БЕРЛИН") 
                        {
                            if (tt.Previous != null && tt.Previous.IsValue("В", null)) 
                            {
                            }
                            else 
                                return true;
                        }
                    }
                }
            }
            if (!ok) 
                return false;
            Pullenti.Ner.Token t0 = tt.Previous;
            if (t0 != null && ((t0.IsChar(',') || t0.Morph.Class.IsConjunction))) 
                t0 = t0.Previous;
            if (t0 != null && (t0.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                return false;
            if (MiscLocationHelper.CheckGeoObjectAfterBrief(tt, null)) 
                return false;
            return true;
        }
    }
}