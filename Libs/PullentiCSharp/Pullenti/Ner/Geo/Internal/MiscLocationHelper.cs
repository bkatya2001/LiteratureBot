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

namespace Pullenti.Ner.Geo.Internal
{
    public static class MiscLocationHelper
    {
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
        }
        internal static Pullenti.Ner.Core.NounPhraseToken TryParseNpt(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            return Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
        }
        public static Pullenti.Ner.Token CheckTerritory(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            Pullenti.Ner.Core.TerminToken tok = m_Terrs.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null) 
                return null;
            if (t.IsValue("САД", null)) 
            {
                OrgTypToken oo = OrgTypToken.TryParse(t, false, null);
                if (oo != null) 
                    return null;
            }
            Pullenti.Ner.Token tt2 = tok.EndToken;
            if (tt2.Next != null && tt2.Next.IsValue("ВЛАДЕНИЕ", null)) 
                tt2 = tt2.Next;
            Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(tt2.Next);
            if (npt != null && npt.EndToken.IsValue("ЗЕМЛЯ", null)) 
                tt2 = npt.EndToken;
            return tt2;
        }
        public static bool CheckGeoObjectBefore(Pullenti.Ner.Token t, bool pureGeo = false)
        {
            if (t == null) 
                return false;
            for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
            {
                if ((tt.IsCharOf(",.;:") || tt.IsHiphen || tt.IsAnd) || tt.Morph.Class.IsConjunction || tt.Morph.Class.IsPreposition) 
                    continue;
                if (m_Terrs.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                    continue;
                if (m_GeoBefore.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                    return true;
                if (tt.LengthChar == 2 && (tt is Pullenti.Ner.TextToken) && tt.Chars.IsAllUpper) 
                {
                    string term = (tt as Pullenti.Ner.TextToken).Term;
                    if (!string.IsNullOrEmpty(term) && term[0] == 'Р') 
                        return true;
                }
                Pullenti.Ner.ReferentToken rt = tt as Pullenti.Ner.ReferentToken;
                if (rt != null) 
                {
                    if (rt.Referent is Pullenti.Ner.Geo.GeoReferent) 
                        return true;
                    if (!pureGeo) 
                    {
                        if ((rt.Referent is Pullenti.Ner.Address.AddressReferent) || (rt.Referent is Pullenti.Ner.Address.StreetReferent)) 
                            return true;
                    }
                }
                break;
            }
            return false;
        }
        public static bool CheckGeoObjectBeforeBrief(Pullenti.Ner.Token t, GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return false;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return false;
            int miss = 0;
            for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
            {
                if (tt.IsNewlineAfter) 
                    break;
                if (tt.IsCharOf(",.;") || tt.IsHiphen || tt.Morph.Class.IsConjunction) 
                    continue;
                if (CheckTerritory(tt) != null) 
                    return true;
                Pullenti.Ner.ReferentToken rt = tt as Pullenti.Ner.ReferentToken;
                if (rt != null) 
                {
                    if ((rt.Referent is Pullenti.Ner.Geo.GeoReferent) || (rt.Referent is Pullenti.Ner.Address.AddressReferent) || (rt.Referent is Pullenti.Ner.Address.StreetReferent)) 
                        return true;
                    break;
                }
                GeoTokenData d = tt.Tag as GeoTokenData;
                if (d != null) 
                {
                    if (d.Cit != null && ((d.Cit.Typ == CityItemToken.ItemType.Noun || d.Cit.Typ == CityItemToken.ItemType.City))) 
                        return true;
                    if (d.Terr != null && ((d.Terr.TerminItem != null || d.Terr.OntoItem != null))) 
                        return true;
                    if (d.Street != null && d.Street.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && d.Street.NounIsDoubtCoef == 0) 
                        return true;
                }
                if ((++miss) > 2) 
                    break;
            }
            return false;
        }
        public static bool CheckGeoObjectAfterBrief(Pullenti.Ner.Token t, GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return false;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return false;
            int miss = 0;
            for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore) 
                    break;
                if (tt.IsCharOf(",.;") || tt.IsHiphen || tt.Morph.Class.IsConjunction) 
                    continue;
                if (CheckTerritory(tt) != null) 
                    return true;
                Pullenti.Ner.ReferentToken rt = tt as Pullenti.Ner.ReferentToken;
                if (rt != null) 
                {
                    if ((rt.Referent is Pullenti.Ner.Geo.GeoReferent) || (rt.Referent is Pullenti.Ner.Address.AddressReferent) || (rt.Referent is Pullenti.Ner.Address.StreetReferent)) 
                        return true;
                    break;
                }
                GeoTokenData d = tt.Tag as GeoTokenData;
                if (d != null) 
                {
                    if (d.Cit != null && ((d.Cit.Typ == CityItemToken.ItemType.Noun || d.Cit.Typ == CityItemToken.ItemType.City))) 
                        return true;
                    if (d.Terr != null && ((d.Terr.TerminItem != null || d.Terr.OntoItem != null))) 
                        return true;
                    if (d.Street != null && d.Street.Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun && d.Street.NounIsDoubtCoef == 0) 
                        return true;
                }
                if (CityItemToken.CheckKeyword(tt) != null) 
                    return true;
                if ((tt is Pullenti.Ner.TextToken) && tt.Chars.IsAllLower) 
                {
                    if (!tt.Morph.Class.IsPreposition) 
                        break;
                }
                miss++;
                if (miss > 4) 
                    break;
            }
            return false;
        }
        public static bool CheckGeoObjectAfter(Pullenti.Ner.Token t, bool dontCheckCity = false)
        {
            if (t == null) 
                return false;
            int cou = 0;
            for (Pullenti.Ner.Token tt = t.Next; tt != null; tt = tt.Next) 
            {
                if (tt.IsCharOf(",.;") || tt.IsHiphen || tt.Morph.Class.IsConjunction) 
                    continue;
                if (tt.Morph.Class.IsPreposition) 
                {
                    if (!dontCheckCity && tt.IsValue("С", null) && tt.Next != null) 
                    {
                        Pullenti.Ner.Token ttt = tt.Next;
                        if (ttt.IsChar('.') && (ttt.WhitespacesAfterCount < 3)) 
                            ttt = ttt.Next;
                        List<CityItemToken> cits = CityItemToken.TryParseList(ttt, 3, null);
                        if (cits != null && cits.Count == 1 && ((cits[0].Typ == CityItemToken.ItemType.ProperName || cits[0].Typ == CityItemToken.ItemType.City))) 
                        {
                            if (tt.Chars.IsAllUpper && !cits[0].Chars.IsAllUpper) 
                            {
                            }
                            else 
                                return true;
                        }
                    }
                    continue;
                }
                if (CheckTerritory(tt) != null) 
                    return true;
                Pullenti.Ner.ReferentToken rt = tt as Pullenti.Ner.ReferentToken;
                if (rt == null) 
                {
                    if (!dontCheckCity) 
                    {
                        List<CityItemToken> cits = CityItemToken.TryParseList(tt, 3, null);
                        if ((cits != null && cits.Count >= 2 && cits[0].Typ == CityItemToken.ItemType.Noun) && ((cits[1].Typ == CityItemToken.ItemType.ProperName || cits[1].Typ == CityItemToken.ItemType.City))) 
                        {
                            if (cits[0].Chars.IsAllUpper && !cits[1].Chars.IsAllUpper) 
                            {
                            }
                            else 
                                return true;
                        }
                        if (cits != null && cits[0].Typ == CityItemToken.ItemType.Noun && (cits[0].WhitespacesAfterCount < 3)) 
                        {
                            if (OrgItemToken.TryParse(cits[0].EndToken.Next, null) != null) 
                                return true;
                        }
                    }
                    if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar > 2 && cou == 0) 
                    {
                        cou++;
                        continue;
                    }
                    else 
                        break;
                }
                if ((rt.Referent is Pullenti.Ner.Geo.GeoReferent) || (rt.Referent is Pullenti.Ner.Address.AddressReferent) || (rt.Referent is Pullenti.Ner.Address.StreetReferent)) 
                    return true;
                break;
            }
            return false;
        }
        public static Pullenti.Ner.Token CheckNearBefore(Pullenti.Ner.Token t, GeoAnalyzerData ad)
        {
            if (t == null || t.Previous == null) 
                return null;
            int cou = 0;
            for (Pullenti.Ner.Token tt = t.Previous; tt != null && (cou < 5); tt = tt.Previous,cou++) 
            {
                if (tt.Morph.Class.IsPreposition && (cou < 2)) 
                {
                    if (m_Near.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No) != null) 
                        return tt;
                }
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(tt, null, ad);
                if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Detail) 
                    return tt;
            }
            return null;
        }
        public static Pullenti.Ner.Token CheckUnknownRegion(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(t);
            if (npt == null) 
                return null;
            if (TerrItemToken.m_UnknownRegions.TryParse(npt.EndToken, Pullenti.Ner.Core.TerminParseAttr.FullwordsOnly) != null) 
                return npt.EndToken;
            return null;
        }
        public static List<string> GetStdAdjFull(Pullenti.Ner.Token t, Pullenti.Morph.MorphGender gen, Pullenti.Morph.MorphNumber num, bool strict)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            return GetStdAdjFullStr((t as Pullenti.Ner.TextToken).Term, gen, num, strict);
        }
        public static List<string> GetStdAdjFullStr(string v, Pullenti.Morph.MorphGender gen, Pullenti.Morph.MorphNumber num, bool strict)
        {
            List<string> res = new List<string>();
            if (v.StartsWith("Б")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("БОЛЬШИЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("БОЛЬШИЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("БОЛЬШАЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("БОЛЬШОЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("БОЛЬШОЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v.StartsWith("М")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("МАЛЫЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("МАЛЫЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("МАЛАЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("МАЛЫЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("МАЛОЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v.StartsWith("В")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("ВЕРХНИЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("ВЕРХНИЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("ВЕРХНЯЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("ВЕРХНИЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("ВЕРХНЕЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v == "Н") 
            {
                List<string> r1 = GetStdAdjFullStr("НОВ", gen, num, strict);
                List<string> r2 = GetStdAdjFullStr("НИЖ", gen, num, strict);
                if (r1 == null && r2 == null) 
                    return null;
                if (r1 == null) 
                    return r2;
                if (r2 == null) 
                    return r1;
                r1.Insert(1, r2[0]);
                r2.RemoveAt(0);
                r1.AddRange(r2);
                return r1;
            }
            if (v == "С" || v == "C") 
            {
                List<string> r1 = GetStdAdjFullStr("СТ", gen, num, strict);
                List<string> r2 = GetStdAdjFullStr("СР", gen, num, strict);
                if (r1 == null && r2 == null) 
                    return null;
                if (r1 == null) 
                    return r2;
                if (r2 == null) 
                    return r1;
                r1.Insert(1, r2[0]);
                r2.RemoveAt(0);
                r1.AddRange(r2);
                return r1;
            }
            if (v.StartsWith("НОВ")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("НОВЫЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("НОВЫЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("НОВАЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("НОВЫЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("НОВОЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v.StartsWith("НИЖ")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("НИЖНИЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("НИЖНИЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("НИЖНЯЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("НИЖНИЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("НИЖНЕЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v.StartsWith("СТ")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("СТАРЫЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("СТАРЫЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("СТАРАЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("СТАРЫЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("СТАРОЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            if (v.StartsWith("СР")) 
            {
                if (num == Pullenti.Morph.MorphNumber.Plural) 
                {
                    res.Add("СРЕДНИЕ");
                    return res;
                }
                if (!strict && ((num & Pullenti.Morph.MorphNumber.Plural)) != Pullenti.Morph.MorphNumber.Undefined) 
                    res.Add("СРЕДНИЕ");
                if (((gen & Pullenti.Morph.MorphGender.Feminie)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Feminie) 
                        res.Add("СРЕДНЯЯ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Masculine)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Masculine) 
                        res.Add("СРЕДНИЙ");
                }
                if (((gen & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    if (!strict || gen == Pullenti.Morph.MorphGender.Neuter) 
                        res.Add("СРЕДНЕЕ");
                }
                if (res.Count > 0) 
                    return res;
                return null;
            }
            return null;
        }
        public static Pullenti.Ner.Geo.GeoReferent GetGeoReferentByName(string name)
        {
            Pullenti.Ner.Geo.GeoReferent res = null;
            if (m_GeoRefByName.TryGetValue(name, out res)) 
                return res;
            foreach (Pullenti.Ner.Referent r in TerrItemToken.m_AllStates) 
            {
                if (r.FindSlot(null, name, true) != null) 
                {
                    res = r as Pullenti.Ner.Geo.GeoReferent;
                    break;
                }
            }
            m_GeoRefByName.Add(name, res);
            return res;
        }
        static Dictionary<string, Pullenti.Ner.Geo.GeoReferent> m_GeoRefByName = new Dictionary<string, Pullenti.Ner.Geo.GeoReferent>();
        public static Pullenti.Ner.MetaToken TryAttachNordWest(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            Pullenti.Ner.Core.TerminToken tok = m_Nords.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null) 
                return null;
            Pullenti.Ner.MetaToken res = new Pullenti.Ner.MetaToken(t, t) { Morph = t.Morph };
            Pullenti.Ner.Token t1 = null;
            if ((t.Next != null && t.Next.IsHiphen && !t.IsWhitespaceAfter) && !t.IsWhitespaceAfter) 
                t1 = t.Next.Next;
            else if (t.Morph.Class.IsAdjective && (t.WhitespacesAfterCount < 2)) 
                t1 = t.Next;
            if (t1 != null) 
            {
                if ((((tok = m_Nords.TryParse(t1, Pullenti.Ner.Core.TerminParseAttr.No)))) != null) 
                {
                    res.EndToken = tok.EndToken;
                    res.Morph = tok.Morph;
                }
            }
            return res;
        }
        static Pullenti.Ner.Core.TerminCollection m_Terrs;
        static Pullenti.Ner.Core.TerminCollection m_GeoBefore;
        static Pullenti.Ner.Core.TerminCollection m_Near;
        internal static void Initialize()
        {
            if (m_Nords != null) 
                return;
            m_Nords = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"СЕВЕРНЫЙ", "ЮЖНЫЙ", "ЗАПАДНЫЙ", "ВОСТОЧНЫЙ", "ЦЕНТРАЛЬНЫЙ", "БЛИЖНИЙ", "ДАЛЬНИЙ", "СРЕДНИЙ", "СЕВЕР", "ЮГ", "ЗАПАД", "ВОСТОК", "СЕВЕРО", "ЮГО", "ЗАПАДНО", "ВОСТОЧНО", "СЕВЕРОЗАПАДНЫЙ", "СЕВЕРОВОСТОЧНЫЙ", "ЮГОЗАПАДНЫЙ", "ЮГОВОСТОЧНЫЙ"}) 
            {
                m_Nords.Add(new Pullenti.Ner.Core.Termin(s, Pullenti.Morph.MorphLang.RU, true));
            }
            m_Near = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"У", "ОКОЛО", "ВБЛИЗИ", "ВБЛИЗИ ОТ", "НЕДАЛЕКО ОТ", "НЕПОДАЛЕКУ ОТ"}) 
            {
                m_Near.Add(new Pullenti.Ner.Core.Termin(s));
            }
            m_GeoBefore = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"ПРОЖИВАТЬ", "ПРОЖИВАТИ", "РОДИТЬ", "НАРОДИТИ", "ЗАРЕГИСТРИРОВАТЬ", "ЗАРЕЄСТРУВАТИ", "АДРЕС", "УРОЖЕНЕЦ", "УРОДЖЕНЕЦЬ", "УРОЖЕНКА", "УРОДЖЕНКА"}) 
            {
                m_GeoBefore.Add(new Pullenti.Ner.Core.Termin(s));
            }
            m_Terrs = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("ТЕРРИТОРИЯ");
            t.AddVariant("ТЕР", false);
            t.AddVariant("ТЕРР", false);
            t.AddVariant("ТЕРИТОРІЯ", false);
            t.AddAbridge("ТЕР.");
            t.AddAbridge("ТЕРР.");
            m_Terrs.Add(t);
            m_Terrs.Add(new Pullenti.Ner.Core.Termin("САД"));
            m_Terrs.Add(new Pullenti.Ner.Core.Termin("ГРАНИЦА"));
            m_Terrs.Add(new Pullenti.Ner.Core.Termin("В ГРАНИЦАХ"));
            string table = "\nAF\tAFG\nAX\tALA\nAL\tALB\nDZ\tDZA\nAS\tASM\nAD\tAND\nAO\tAGO\nAI\tAIA\nAQ\tATA\nAG\tATG\nAR\tARG\nAM\tARM\nAW\tABW\nAU\tAUS\nAT\tAUT\nAZ\tAZE\nBS\tBHS\nBH\tBHR\nBD\tBGD\nBB\tBRB\nBY\tBLR\nBE\tBEL\nBZ\tBLZ\nBJ\tBEN\nBM\tBMU\nBT\tBTN\nBO\tBOL\nBA\tBIH\nBW\tBWA\nBV\tBVT\nBR\tBRA\nVG\tVGB\nIO\tIOT\nBN\tBRN\nBG\tBGR\nBF\tBFA\nBI\tBDI\nKH\tKHM\nCM\tCMR\nCA\tCAN\nCV\tCPV\nKY\tCYM\nCF\tCAF\nTD\tTCD\nCL\tCHL\nCN\tCHN\nHK\tHKG\nMO\tMAC\nCX\tCXR\nCC\tCCK\nCO\tCOL\nKM\tCOM\nCG\tCOG\nCD\tCOD\nCK\tCOK\nCR\tCRI\nCI\tCIV\nHR\tHRV\nCU\tCUB\nCY\tCYP\nCZ\tCZE\nDK\tDNK\nDJ\tDJI\nDM\tDMA\nDO\tDOM\nEC\tECU\nEG\tEGY\nSV\tSLV\nGQ\tGNQ\nER\tERI\nEE\tEST\nET\tETH\nFK\tFLK\nFO\tFRO\nFJ\tFJI\nFI\tFIN\nFR\tFRA\nGF\tGUF\nPF\tPYF\nTF\tATF\nGA\tGAB\nGM\tGMB\nGE\tGEO\nDE\tDEU\nGH\tGHA\nGI\tGIB\nGR\tGRC\nGL\tGRL\nGD\tGRD\nGP\tGLP\nGU\tGUM\nGT\tGTM\nGG\tGGY\nGN\tGIN\nGW\tGNB\nGY\tGUY\nHT\tHTI\nHM\tHMD\nVA\tVAT\nHN\tHND\nHU\tHUN\nIS\tISL\nIN\tIND\nID\tIDN\nIR\tIRN\nIQ\tIRQ\nIE\tIRL\nIM\tIMN\nIL\tISR\nIT\tITA\nJM\tJAM\nJP\tJPN\nJE\tJEY\nJO\tJOR\nKZ\tKAZ\nKE\tKEN\nKI\tKIR\nKP\tPRK\nKR\tKOR\nKW\tKWT\nKG\tKGZ\nLA\tLAO\nLV\tLVA\nLB\tLBN\nLS\tLSO\nLR\tLBR\nLY\tLBY\nLI\tLIE\nLT\tLTU\nLU\tLUX\nMK\tMKD\nMG\tMDG\nMW\tMWI\nMY\tMYS\nMV\tMDV\nML\tMLI\nMT\tMLT\nMH\tMHL\nMQ\tMTQ\nMR\tMRT\nMU\tMUS\nYT\tMYT\nMX\tMEX\nFM\tFSM\nMD\tMDA\nMC\tMCO\nMN\tMNG\nME\tMNE\nMS\tMSR\nMA\tMAR\nMZ\tMOZ\nMM\tMMR\nNA\tNAM\nNR\tNRU\nNP\tNPL\nNL\tNLD\nAN\tANT\nNC\tNCL\nNZ\tNZL\nNI\tNIC\nNE\tNER\nNG\tNGA\nNU\tNIU\nNF\tNFK\nMP\tMNP\nNO\tNOR\nOM\tOMN\nPK\tPAK\nPW\tPLW\nPS\tPSE\nPA\tPAN\nPG\tPNG\nPY\tPRY\nPE\tPER\nPH\tPHL\nPN\tPCN\nPL\tPOL\nPT\tPRT\nPR\tPRI\nQA\tQAT\nRE\tREU\nRO\tROU\nRU\tRUS\nRW\tRWA\nBL\tBLM\nSH\tSHN\nKN\tKNA\nLC\tLCA\nMF\tMAF\nPM\tSPM\nVC\tVCT\nWS\tWSM\nSM\tSMR\nST\tSTP\nSA\tSAU\nSN\tSEN\nRS\tSRB\nSC\tSYC\nSL\tSLE\nSG\tSGP\nSK\tSVK\nSI\tSVN\nSB\tSLB\nSO\tSOM\nZA\tZAF\nGS\tSGS\nSS\tSSD\nES\tESP\nLK\tLKA\nSD\tSDN\nSR\tSUR\nSJ\tSJM\nSZ\tSWZ\nSE\tSWE\nCH\tCHE\nSY\tSYR\nTW\tTWN\nTJ\tTJK\nTZ\tTZA\nTH\tTHA\nTL\tTLS\nTG\tTGO\nTK\tTKL\nTO\tTON\nTT\tTTO\nTN\tTUN\nTR\tTUR\nTM\tTKM\nTC\tTCA\nTV\tTUV\nUG\tUGA\nUA\tUKR\nAE\tARE\nGB\tGBR\nUS\tUSA\nUM\tUMI\nUY\tURY\nUZ\tUZB\nVU\tVUT\nVE\tVEN\nVN\tVNM\nVI\tVIR\nWF\tWLF\nEH\tESH\nYE\tYEM\nZM\tZMB\nZW\tZWE ";
            foreach (string s in table.Split('\n')) 
            {
                string ss = s.Trim();
                if ((ss.Length < 6) || !char.IsWhiteSpace(ss[2])) 
                    continue;
                string cod2 = ss.Substring(0, 2);
                string cod3 = ss.Substring(3).Trim();
                if (cod3.Length != 3) 
                    continue;
                if (!m_Alpha2_3.ContainsKey(cod2)) 
                    m_Alpha2_3.Add(cod2, cod3);
                if (!m_Alpha3_2.ContainsKey(cod3)) 
                    m_Alpha3_2.Add(cod3, cod2);
            }
        }
        static Pullenti.Ner.Core.TerminCollection m_Nords;
        internal static Dictionary<string, string> m_Alpha2_3 = new Dictionary<string, string>();
        internal static Dictionary<string, string> m_Alpha3_2 = new Dictionary<string, string>();
        internal static byte[] Deflate(byte[] zip)
        {
            using (MemoryStream unzip = new MemoryStream()) 
            {
                MemoryStream data = new MemoryStream(zip);
                data.Position = 0;
                Pullenti.Morph.Internal.MorphDeserializer.DeflateGzip(data, unzip);
                data.Dispose();
                return unzip.ToArray();
            }
        }
    }
}