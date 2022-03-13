/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Pullenti.Ner.Core
{
    /// <summary>
    /// Сериализация сущностей
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// Сериализация в строку XML списка сущностей. Сущности могут быть взаимосвязаны, 
        /// то есть значениями атрибутов могут выступать другие сущности (то есть сериализуется по сути граф).
        /// </summary>
        /// <param name="refs">список сериализуемых сущностей</param>
        /// <param name="rootTagName">имя корневого узла</param>
        /// <param name="outOccurences">выводить ли вхождения в текст</param>
        /// <return>строка с XML</return>
        public static string SerializeReferentsToXmlString(List<Pullenti.Ner.Referent> refs, string rootTagName = "referents", bool outOccurences = false)
        {
            int id = 1;
            foreach (Pullenti.Ner.Referent r in refs) 
            {
                r.Tag = id;
                id++;
            }
            StringBuilder res = new StringBuilder();
            using (XmlWriter xml = XmlWriter.Create(res)) 
            {
                xml.WriteStartElement(rootTagName);
                foreach (Pullenti.Ner.Referent r in refs) 
                {
                    SerializeReferentToXml(r, xml, outOccurences, false);
                }
                xml.WriteEndElement();
            }
            _corrXmlFile(res);
            foreach (Pullenti.Ner.Referent r in refs) 
            {
                r.Tag = null;
            }
            return res.ToString();
        }
        /// <summary>
        /// Прямая сериализация сущности в строку XML.
        /// </summary>
        /// <param name="r">сериализуемая сущность</param>
        /// <param name="outOccurences">выводить ли вхождения в текст</param>
        public static string SerializeReferentToXmlString(Pullenti.Ner.Referent r, bool outOccurences = false)
        {
            StringBuilder res = new StringBuilder();
            using (XmlWriter xml = XmlWriter.Create(res)) 
            {
                SerializeReferentToXml(r, xml, outOccurences, true);
            }
            _corrXmlFile(res);
            return res.ToString();
        }
        /// <summary>
        /// Прямая сериализация сущности в XML.
        /// </summary>
        /// <param name="r">сериализуемая сущность</param>
        /// <param name="xml">куда сериализовать</param>
        /// <param name="outOccurences">выводить ли вхождения в текст</param>
        /// <param name="convertSlotRefsToString">преобразовывать ли ссылки в слотах на сущноси в строковые значения</param>
        public static void SerializeReferentToXml(Pullenti.Ner.Referent r, XmlWriter xml, bool outOccurences = false, bool convertSlotRefsToString = false)
        {
            xml.WriteStartElement("referent");
            if (r.Tag is int) 
                xml.WriteAttributeString("id", r.Tag.ToString());
            xml.WriteAttributeString("typ", r.TypeName);
            xml.WriteAttributeString("spel", _corrXmlValue(r.ToString()));
            foreach (Pullenti.Ner.Slot s in r.Slots) 
            {
                if (s.Value != null) 
                {
                    string nam = s.TypeName;
                    xml.WriteStartElement("slot");
                    xml.WriteAttributeString("typ", s.TypeName);
                    if ((s.Value is Pullenti.Ner.Referent) && ((s.Value as Pullenti.Ner.Referent).Tag is int)) 
                        xml.WriteAttributeString("ref", (s.Value as Pullenti.Ner.Referent).Tag.ToString());
                    if (s.Value != null) 
                        xml.WriteAttributeString("val", _corrXmlValue(s.Value.ToString()));
                    if (s.Count > 0) 
                        xml.WriteAttributeString("count", s.Count.ToString());
                    xml.WriteEndElement();
                }
            }
            if (outOccurences) 
            {
                foreach (Pullenti.Ner.TextAnnotation o in r.Occurrence) 
                {
                    xml.WriteStartElement("occ");
                    xml.WriteAttributeString("begin", o.BeginChar.ToString());
                    xml.WriteAttributeString("end", o.EndChar.ToString());
                    xml.WriteAttributeString("text", _corrXmlValue(o.GetText()));
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndElement();
        }
        static void _corrXmlFile(StringBuilder res)
        {
            int i = res.ToString().IndexOf('>');
            if (i > 10 && res[1] == '?') 
                res.Remove(0, i + 1);
            for (i = 0; i < res.Length; i++) 
            {
                char ch = res[i];
                int cod = (int)ch;
                if ((cod < 0x80) && cod >= 0x20) 
                    continue;
                if (Pullenti.Morph.LanguageHelper.IsCyrillicChar(ch)) 
                    continue;
                res.Remove(i, 1);
                res.Insert(i, string.Format("&#x{0};", cod.ToString("X04")));
            }
        }
        static string _corrXmlValue(string txt)
        {
            if (txt == null) 
                return "";
            foreach (char c in txt) 
            {
                if (((((int)c) < 0x20) && c != '\r' && c != '\n') && c != '\t') 
                {
                    StringBuilder tmp = new StringBuilder(txt);
                    for (int i = 0; i < tmp.Length; i++) 
                    {
                        char ch = tmp[i];
                        if (((((int)ch) < 0x20) && ch != '\r' && ch != '\n') && ch != '\t') 
                            tmp[i] = ' ';
                    }
                    return tmp.ToString();
                }
            }
            return txt;
        }
        /// <summary>
        /// Десериализация списка взаимосвязанных сущностей из строки
        /// </summary>
        /// <param name="xmlString">результат сериализации функцией SerializeReferentsToXmlString()</param>
        /// <return>Список экземпляров сущностей</return>
        public static List<Pullenti.Ner.Referent> DeserializeReferentsFromXmlString(string xmlString)
        {
            List<Pullenti.Ner.Referent> res = new List<Pullenti.Ner.Referent>();
            Dictionary<int, Pullenti.Ner.Referent> map = new Dictionary<int, Pullenti.Ner.Referent>();
            try 
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString);
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    if (x.LocalName == "referent") 
                    {
                        Pullenti.Ner.Referent r = _deserializeReferent(x);
                        if (r == null) 
                            continue;
                        res.Add(r);
                        if (r.Tag is int) 
                        {
                            if (!map.ContainsKey((int)r.Tag)) 
                                map.Add((int)r.Tag, r);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                return null;
            }
            // восстанавливаем ссылки
            foreach (Pullenti.Ner.Referent r in res) 
            {
                r.Tag = null;
                foreach (Pullenti.Ner.Slot s in r.Slots) 
                {
                    if (s.Tag is int) 
                    {
                        Pullenti.Ner.Referent rr = null;
                        map.TryGetValue((int)s.Tag, out rr);
                        if (rr != null) 
                            s.Value = rr;
                        s.Tag = null;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Десериализация сущности из строки XML
        /// </summary>
        /// <param name="xmlString">результат сериализации функцией SerializeReferentToXmlString()</param>
        /// <return>Экземпляр сущностей</return>
        public static Pullenti.Ner.Referent DeserializeReferentFromXmlString(string xmlString)
        {
            try 
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString);
                return _deserializeReferent(xml.DocumentElement);
            }
            catch(Exception ex) 
            {
            }
            return null;
        }
        static Pullenti.Ner.Referent _deserializeReferent(XmlNode xml)
        {
            string typ = null;
            int id = 0;
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "id") 
                        id = int.Parse(a.Value);
                    else if (a.LocalName == "typ") 
                        typ = a.Value;
                }
            }
            if (typ == null) 
                return null;
            Pullenti.Ner.Referent res = Pullenti.Ner.ProcessorService.CreateReferent(typ);
            if (res == null) 
                return null;
            res.Tag = id;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName != "slot") 
                    continue;
                string nam = null;
                string val = null;
                int cou = 0;
                int refid = 0;
                if (x.Attributes != null) 
                {
                    foreach (XmlAttribute a in x.Attributes) 
                    {
                        if (a.LocalName == "typ") 
                            nam = a.Value;
                        else if (a.LocalName == "count") 
                            cou = int.Parse(a.Value);
                        else if (a.LocalName == "ref") 
                            refid = int.Parse(a.Value);
                        else if (a.LocalName == "val") 
                            val = a.Value;
                    }
                }
                if (nam == null) 
                    continue;
                Pullenti.Ner.Slot slot = res.AddSlot(nam, val, false, 0);
                slot.Count = cou;
                if (refid > 0) 
                    slot.Tag = refid;
            }
            return res;
        }
        /// <summary>
        /// Сериализация в строку JSON списка сущностей. Сущности могут быть взаимосвязаны, 
        /// то есть значениями атрибутов могут выступать другие сущности (то есть сериализуется по сути граф).
        /// </summary>
        /// <param name="refs">список сериализуемых сущностей</param>
        /// <param name="rootTagName">имя корневого узла</param>
        /// <param name="outOccurences">выводить ли вхождения в текст</param>
        /// <return>строка с JSON (массив [...])</return>
        public static string SerializeReferentsToJsonString(List<Pullenti.Ner.Referent> refs, bool outOccurences = false)
        {
            int id = 1;
            foreach (Pullenti.Ner.Referent r in refs) 
            {
                r.Tag = id;
                id++;
            }
            StringBuilder res = new StringBuilder();
            res.Append("[");
            foreach (Pullenti.Ner.Referent r in refs) 
            {
                string json = SerializeReferentToJsonString(r, outOccurences);
                res.Append("\r\n");
                res.Append(json);
                if (r != refs[refs.Count - 1]) 
                    res.Append(", ");
            }
            res.Append("\r\n]");
            foreach (Pullenti.Ner.Referent r in refs) 
            {
                r.Tag = null;
            }
            return res.ToString();
        }
        /// <summary>
        /// Сериализация сущности в JSON (словарь {...}).
        /// </summary>
        /// <param name="r">сериализуемая сущность</param>
        /// <param name="outOccurences">выводить ли вхождения в текст</param>
        /// <return>строка со словарём JSON</return>
        public static string SerializeReferentToJsonString(Pullenti.Ner.Referent r, bool outOccurences = false)
        {
            StringBuilder res = new StringBuilder();
            res.Append("{");
            if (r.Tag is int) 
                res.AppendFormat("\r\n  \"id\" : {0},", r.Tag);
            res.AppendFormat("\r\n  \"typ\" : \"{0}\", ", r.TypeName);
            res.Append("\r\n  \"spel\" : \"");
            _corrJsonValue(r.ToString(), res);
            res.Append("\", ");
            res.Append("\r\n  \"slots\" : [");
            for (int i = 0; i < r.Slots.Count; i++) 
            {
                Pullenti.Ner.Slot s = r.Slots[i];
                res.AppendFormat("\r\n      {0} \"typ\" : \"{1}\", ", '{', s.TypeName);
                if (s.Value is Pullenti.Ner.Referent) 
                    res.AppendFormat("\"ref\" : {0}, ", (s.Value as Pullenti.Ner.Referent).Tag.ToString());
                if (s.Value != null) 
                    res.Append("\"val\" : \"");
                _corrJsonValue(s.Value.ToString(), res);
                res.Append("\"");
                if (s.Count > 0) 
                    res.AppendFormat(", \"count\" : {0}", s.Count.ToString());
                res.Append(" }");
                if ((i + 1) < r.Slots.Count) 
                    res.Append(",");
            }
            res.Append(" ]");
            if (outOccurences) 
            {
                res.Append(",\r\n  \"occs\" : [");
                for (int i = 0; i < r.Occurrence.Count; i++) 
                {
                    Pullenti.Ner.TextAnnotation o = r.Occurrence[i];
                    res.AppendFormat("\r\n      {0} \"begin\" : {1}, \"end\" : {2}, \"text\" : \"", '{', o.BeginChar, o.EndChar);
                    _corrJsonValue(o.GetText(), res);
                    res.Append("\" }");
                    if ((i + 1) < r.Occurrence.Count) 
                        res.Append(",");
                }
                res.Append(" ]");
            }
            res.Append("\r\n}");
            return res.ToString();
        }
        static void _corrJsonValue(string txt, StringBuilder res)
        {
            foreach (char ch in txt) 
            {
                if (ch == '"') 
                    res.Append("\\\"");
                else if (ch == '\\') 
                    res.Append("\\\\");
                else if (ch == '/') 
                    res.Append("\\/");
                else if (ch == 0xD) 
                    res.Append("\\r");
                else if (ch == 0xA) 
                    res.Append("\\n");
                else if (ch == '\t') 
                    res.Append("\\t");
                else if (((int)ch) < 0x20) 
                    res.Append(' ');
                else 
                    res.Append(ch);
            }
        }
    }
}