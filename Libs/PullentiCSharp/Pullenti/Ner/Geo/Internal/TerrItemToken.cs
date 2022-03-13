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
    public class TerrItemToken : Pullenti.Ner.MetaToken
    {
        public TerrItemToken(Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
        }
        public Pullenti.Ner.Core.IntOntologyItem OntoItem;
        public Pullenti.Ner.Core.IntOntologyItem OntoItem2;
        public TerrTermin TerminItem;
        public TerrTermin TerminItem2;
        public bool IsAdjective;
        public bool IsDistrictName;
        public Pullenti.Ner.ReferentToken AdjectiveRef;
        public bool CanBeCity;
        public bool CanBeSurname;
        public bool IsAdjInDictionary;
        public bool IsGeoInDictionary;
        public bool IsDoubt;
        public bool IsCityRegion
        {
            get
            {
                if (TerminItem == null) 
                    return false;
                return (TerminItem.CanonicText.Contains("ГОРОДС") || TerminItem.CanonicText.Contains("МІСЬК") || TerminItem.CanonicText.Contains("МУНИЦИПАЛ")) || TerminItem.CanonicText.Contains("МУНІЦИПАЛ") || TerminItem.CanonicText == "ПОЧТОВОЕ ОТДЕЛЕНИЕ";
            }
        }
        public string AdditionalTyp;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (OntoItem != null) 
                res.AppendFormat("{0} ", OntoItem.CanonicText);
            else if (TerminItem != null) 
                res.AppendFormat("{0} ", TerminItem.CanonicText);
            else 
                res.AppendFormat("{0} ", base.ToString());
            if (AdjectiveRef != null) 
                res.AppendFormat(" (Adj: {0})", AdjectiveRef.Referent.ToString());
            return res.ToString().Trim();
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.TRegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                TerrItemToken ter = TryParse(t, null, ad);
                if (ter != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.Terr = ter;
                }
            }
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d == null || d.Terr == null || d.Terr.TerminItem == null) 
                    continue;
                Pullenti.Ner.Token tt = d.Terr.EndToken.Next;
                if (tt == null) 
                    continue;
                GeoTokenData dd = tt.Tag as GeoTokenData;
                TerrItemToken ter = TryParse(tt, d.Terr, ad);
                if (ter == null) 
                    continue;
                if (dd == null) 
                    dd = new GeoTokenData(tt);
                if (dd.Terr == null || (dd.Terr.EndChar < ter.EndChar)) 
                    dd.Terr = ter;
            }
            ad.TRegime = true;
        }
        public static TerrItemToken TryParse(Pullenti.Ner.Token t, TerrItemToken prev = null, GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t) as GeoAnalyzerData;
            if (ad == null) 
                return null;
            GeoTokenData d = t.Tag as GeoTokenData;
            if (d != null && d.NoGeo) 
                return null;
            if (SpeedRegime && ((ad.TRegime || ad.AllRegime)) && ((t.LengthChar != 2 || !t.Chars.IsAllUpper))) 
            {
                if (d != null) 
                    return d.Terr;
                if (t.Tag == null) 
                    return null;
            }
            if (ad.TLevel > 1) 
                return null;
            ad.TLevel++;
            TerrItemToken res = _TryParse(t, prev, false);
            ad.TLevel--;
            if (res == null) 
            {
                if (t.Chars.IsAllUpper && t.LengthChar == 2 && (t is Pullenti.Ner.TextToken)) 
                {
                    string term = (t as Pullenti.Ner.TextToken).Term;
                    if (((term == "РБ" || term == "РК" || term == "TC") || term == "ТС" || term == "РТ") || term == "УР" || term == "РД") 
                    {
                        if ((term == "РБ" && (t.Previous is Pullenti.Ner.TextToken) && (t.WhitespacesBeforeCount < 3)) && !t.Previous.Chars.IsAllLower && t.Previous.Morph.Class.IsAdjective) 
                            return null;
                        foreach (Pullenti.Ner.Core.IntOntologyItem it in ad.LocalOntology.Items) 
                        {
                            if (it.Referent is Pullenti.Ner.Geo.GeoReferent) 
                            {
                                string alph2 = (it.Referent as Pullenti.Ner.Geo.GeoReferent).Alpha2;
                                if (((alph2 == "BY" && term == "РБ")) || ((alph2 == "KZ" && term == "РК"))) 
                                    return new TerrItemToken(t, t) { OntoItem = it };
                                if (term == "РТ") 
                                {
                                    if (it.Referent.FindSlot(null, "ТАТАРСТАН", true) != null) 
                                        return new TerrItemToken(t, t) { OntoItem = it };
                                }
                                if (term == "РД") 
                                {
                                    if (it.Referent.FindSlot(null, "ДАГЕСТАН", true) != null) 
                                        return new TerrItemToken(t, t) { OntoItem = it };
                                }
                                if (term == "РБ") 
                                {
                                    if (it.Referent.FindSlot(null, "БАШКИРИЯ", true) != null) 
                                        return new TerrItemToken(t, t) { OntoItem = it };
                                }
                            }
                        }
                        bool ok = false;
                        if ((t.WhitespacesBeforeCount < 2) && (t.Previous is Pullenti.Ner.TextToken)) 
                        {
                            string term2 = (t.Previous as Pullenti.Ner.TextToken).Term;
                            if ((t.Previous.IsValue("КОДЕКС", null) || t.Previous.IsValue("ЗАКОН", null) || term2 == "КОАП") || term2 == "ПДД" || term2 == "МЮ") 
                                ok = true;
                            else if ((t.Previous.Chars.IsAllUpper && t.Previous.LengthChar > 1 && (t.Previous.LengthChar < 4)) && term2.EndsWith("К")) 
                                ok = true;
                            else if (term == "РТ" || term == "УР" || term == "РД") 
                            {
                                Pullenti.Ner.Token tt = t.Previous;
                                if (tt != null && tt.IsComma) 
                                    tt = tt.Previous;
                                if (tt != null) 
                                {
                                    if ((tt.GetReferent() is Pullenti.Ner.Geo.GeoReferent) && (tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent).Alpha2 == "RU") 
                                        ok = true;
                                    else if ((tt is Pullenti.Ner.NumberToken) && tt.LengthChar == 6 && (tt as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit) 
                                        ok = true;
                                }
                            }
                            if (!ok) 
                            {
                                int cou = 0;
                                for (Pullenti.Ner.Token tt = t.Previous; tt != null && (cou < 4); tt = tt.Previous,cou++) 
                                {
                                    OrgItemToken org = OrgItemToken.TryParse(tt, null);
                                    if (org != null) 
                                    {
                                        ok = true;
                                        break;
                                    }
                                    Pullenti.Ner.ReferentToken kk = tt.Kit.ProcessReferent("PERSONPROPERTY", tt, null);
                                    if (kk != null) 
                                    {
                                        ok = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (((t.WhitespacesBeforeCount < 2) && (t.Previous is Pullenti.Ner.NumberToken) && t.Previous.LengthChar == 6) && (t.Previous as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit) 
                            ok = true;
                        if (ok) 
                        {
                            if (term == "РК" && m_Kazahstan != null) 
                                return new TerrItemToken(t, t) { OntoItem = m_Kazahstan };
                            if (term == "РТ" && m_Tatarstan != null) 
                                return new TerrItemToken(t, t) { OntoItem = m_Tatarstan };
                            if (term == "РД" && m_Dagestan != null) 
                                return new TerrItemToken(t, t) { OntoItem = m_Dagestan };
                            if (term == "УР" && m_Udmurtia != null) 
                                return new TerrItemToken(t, t) { OntoItem = m_Udmurtia };
                            if (term == "РБ" && m_Belorussia != null && ad.Step > 0) 
                                return new TerrItemToken(t, t) { OntoItem = m_Belorussia };
                            if (((term == "ТС" || term == "TC")) && m_TamogSous != null) 
                                return new TerrItemToken(t, t) { OntoItem = m_TamogSous };
                        }
                    }
                }
                if (((t is Pullenti.Ner.TextToken) && ((t.IsValue("Р", null) || t.IsValue("P", null))) && t.Next != null) && t.Next.IsChar('.') && !t.Next.IsNewlineAfter) 
                {
                    res = TryParse(t.Next.Next, null, ad);
                    if (res != null && res.OntoItem != null) 
                    {
                        string str = res.OntoItem.ToString().ToUpper();
                        if (str.Contains("РЕСПУБЛИКА")) 
                        {
                            res.BeginToken = t;
                            res.IsDoubt = false;
                            return res;
                        }
                    }
                }
                return _tryParseDistrictName(t, 0);
            }
            if ((res.BeginToken.LengthChar == 1 && res.BeginToken.Chars.IsAllUpper && res.BeginToken.Next != null) && res.BeginToken.Next.IsChar('.')) 
                return null;
            if ((res.EndToken.Next != null && res.EndToken.Next.IsChar('.') && res.EndToken.Next.Next != null) && res.EndToken.Next.Next.IsComma) 
                res.EndToken = res.EndToken.Next;
            if (res.TerminItem != null && res.TerminItem.CanonicText == "ОКРУГ") 
            {
                if (t.Previous != null && ((t.Previous.IsValue("ГОРОДСКОЙ", null) || t.Previous.IsValue("МІСЬКИЙ", null)))) 
                    return null;
            }
            if (res.OntoItem != null) 
            {
                Pullenti.Ner.Core.IntOntologyToken cit1 = CityItemToken.CheckOntoItem(res.BeginToken);
                if (cit1 != null && cit1.Item.MiscAttr != null) 
                {
                    if (cit1.EndToken.IsValue("CITY", null)) 
                        return null;
                    if (cit1.EndToken == res.EndToken) 
                    {
                        res.CanBeCity = true;
                        if (cit1.EndToken.Next != null && cit1.EndToken.Next.IsValue("CITY", null)) 
                            return null;
                    }
                }
                CityItemToken cit = CityItemToken.TryParseBack(res.BeginToken.Previous, true);
                if (cit != null && cit.Typ == CityItemToken.ItemType.Noun && ((res.IsAdjective || (cit.WhitespacesAfterCount < 1)))) 
                    res.CanBeCity = true;
            }
            if (res.TerminItem != null) 
            {
                if (((res.TerminItem.CanonicText == "МУНИЦИПАЛЬНЫЙ ОКРУГ" || res.TerminItem.CanonicText == "ГОРОДСКОЙ ОКРУГ")) && (res.WhitespacesAfterCount < 3)) 
                {
                    TerrItemToken next0 = TryParse(res.EndToken.Next, null, null);
                    if (next0 != null && next0.TerminItem != null && next0.TerminItem.Acronym == "ЗАТО") 
                    {
                        res.EndToken = next0.EndToken;
                        res.TerminItem2 = next0.TerminItem;
                    }
                    Pullenti.Ner.Core.IntOntologyToken next = CityItemToken.CheckKeyword(res.EndToken.Next);
                    if (next != null) 
                    {
                        res.EndToken = next.EndToken;
                        res.AdditionalTyp = next.Termin.CanonicText;
                    }
                }
                res.IsDoubt = res.TerminItem.IsDoubt;
                if (!res.TerminItem.IsRegion) 
                {
                    if (res.TerminItem.IsMoscowRegion && res.BeginToken == res.EndToken) 
                        res.IsDoubt = true;
                    else if (res.TerminItem.Acronym == "МО" && res.BeginToken == res.EndToken && res.LengthChar == 2) 
                    {
                        if (res.BeginToken.Previous != null && res.BeginToken.Previous.IsValue("ВЕТЕРАН", null)) 
                            return null;
                        res.IsDoubt = true;
                        if (res.BeginToken == res.EndToken && res.LengthChar == 2) 
                        {
                            if (res.BeginToken.Previous == null || res.BeginToken.Previous.IsCharOf(",") || res.BeginToken.IsNewlineBefore) 
                            {
                                if (res.EndToken.Next == null || res.EndToken.Next.IsCharOf(",") || res.IsNewlineAfter) 
                                {
                                    res.TerminItem = null;
                                    res.OntoItem = m_MosRegRU;
                                }
                            }
                        }
                    }
                    else if (res.TerminItem.Acronym == "ЛО" && res.BeginToken == res.EndToken && res.LengthChar == 2) 
                    {
                        res.IsDoubt = true;
                        if (res.BeginToken.Previous == null || res.BeginToken.Previous.IsCommaAnd || res.BeginToken.IsNewlineBefore) 
                        {
                            res.TerminItem = null;
                            res.OntoItem = m_LenRegRU;
                        }
                    }
                    else if (!res.Morph.Case.IsNominative && !res.Morph.Case.IsAccusative) 
                        res.IsDoubt = true;
                    else if (res.Morph.Number != Pullenti.Morph.MorphNumber.Singular) 
                    {
                        if (res.TerminItem.IsMoscowRegion && res.Morph.Number != Pullenti.Morph.MorphNumber.Plural) 
                        {
                        }
                        else 
                            res.IsDoubt = true;
                    }
                }
                if (((res.TerminItem != null && res.TerminItem.CanonicText == "АО")) || ((res.OntoItem == m_MosRegRU && res.LengthChar == 2))) 
                {
                    Pullenti.Ner.Token tt = res.EndToken.Next;
                    Pullenti.Ner.ReferentToken rt = res.Kit.ProcessReferent("ORGANIZATION", res.BeginToken, null);
                    if (rt == null) 
                        rt = res.Kit.ProcessReferent("ORGANIZATION", res.BeginToken.Next, null);
                    if (rt != null) 
                    {
                        foreach (Pullenti.Ner.Slot s in rt.Referent.Slots) 
                        {
                            if (s.TypeName == "TYPE") 
                            {
                                string ty = (string)s.Value;
                                if (res.TerminItem != null && ty != res.TerminItem.CanonicText) 
                                    return null;
                            }
                        }
                    }
                }
            }
            if (res != null && res.BeginToken == res.EndToken && res.TerminItem == null) 
            {
                if (t is Pullenti.Ner.TextToken) 
                {
                    string str = (t as Pullenti.Ner.TextToken).Term;
                    if (str == "ЧАДОВ" || str == "ТОГОВ") 
                        return null;
                }
                if ((((t.Next is Pullenti.Ner.TextToken) && (t.WhitespacesAfterCount < 2) && !t.Next.Chars.IsAllLower) && t.Chars.Equals(t.Next.Chars) && !t.Chars.IsLatinLetter) && ((!t.Morph.Case.IsGenitive && !t.Morph.Case.IsAccusative))) 
                {
                    Pullenti.Morph.MorphClass mc = t.Next.GetMorphClassInDictionary();
                    if (mc.IsProperSurname || mc.IsProperSecname) 
                        res.IsDoubt = true;
                }
                if ((t.Previous is Pullenti.Ner.TextToken) && (t.WhitespacesBeforeCount < 2) && !t.Previous.Chars.IsAllLower) 
                {
                    Pullenti.Morph.MorphClass mc = t.Previous.GetMorphClassInDictionary();
                    if (mc.IsProperSurname) 
                        res.IsDoubt = true;
                }
                if (t.LengthChar <= 2 && res.OntoItem != null && !t.IsValue("РФ", null)) 
                {
                    res.IsDoubt = true;
                    Pullenti.Ner.Token tt = t.Next;
                    if (tt != null && ((tt.IsCharOf(":") || tt.IsHiphen))) 
                        tt = tt.Next;
                    if (tt != null && tt.GetReferent() != null && tt.GetReferent().TypeName == "PHONE") 
                        res.IsDoubt = false;
                    else if (t.LengthChar == 2 && t.Chars.IsAllUpper && t.Chars.IsLatinLetter) 
                        res.IsDoubt = false;
                }
            }
            return res;
        }
        static TerrItemToken _TryParse(Pullenti.Ner.Token t, TerrItemToken prev, bool ignoreOnto = false)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> li = null;
            if (!ignoreOnto) 
            {
                if (t.Kit.Ontology != null) 
                    li = t.Kit.Ontology.AttachToken(Pullenti.Ner.Geo.GeoReferent.OBJ_TYPENAME, t);
                if (li == null || li.Count == 0) 
                    li = m_TerrOntology.TryAttach(t, null, false);
                else 
                {
                    List<Pullenti.Ner.Core.IntOntologyToken> li1 = m_TerrOntology.TryAttach(t, null, false);
                    if (li1 != null && li1.Count > 0) 
                    {
                        if (li1[0].LengthChar > li[0].LengthChar) 
                            li = li1;
                    }
                }
            }
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (li != null) 
            {
                for (int i = li.Count - 1; i >= 0; i--) 
                {
                    if (li[i].Item != null) 
                    {
                        Pullenti.Ner.Geo.GeoReferent g = li[i].Item.Referent as Pullenti.Ner.Geo.GeoReferent;
                        if (g == null) 
                            continue;
                        if (g.IsCity && !g.IsRegion && !g.IsState) 
                            li.RemoveAt(i);
                        else if (g.IsState && t.LengthChar == 2 && li[i].LengthChar == 2) 
                        {
                            if (!t.IsWhitespaceBefore && t.Previous != null && t.Previous.IsChar('.')) 
                                li.RemoveAt(i);
                            else if (t.Previous != null && t.Previous.IsValue("ДОМЕН", null)) 
                                li.RemoveAt(i);
                        }
                    }
                }
                foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                {
                    if (nt.Item != null && !(nt.Termin.Tag is Pullenti.Ner.Core.IntOntologyItem)) 
                    {
                        if (!Pullenti.Ner.Core.MiscHelper.IsAllCharactersLower(nt.BeginToken, nt.EndToken, false) || nt.BeginToken != nt.EndToken) 
                        {
                            TerrItemToken res0 = new TerrItemToken(nt.BeginToken, nt.EndToken) { OntoItem = nt.Item, Morph = nt.Morph };
                            if (nt.EndToken.Morph.Class.IsAdjective && nt.BeginToken == nt.EndToken) 
                            {
                                if (nt.BeginToken.GetMorphClassInDictionary().IsProperGeo) 
                                {
                                }
                                else 
                                    res0.IsAdjective = true;
                            }
                            if (nt.BeginToken == nt.EndToken && nt.Chars.IsLatinLetter) 
                            {
                                if ((nt.Item.Referent as Pullenti.Ner.Geo.GeoReferent).IsState) 
                                {
                                }
                                else if (nt.Item.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "state", true) != null) 
                                {
                                }
                                else 
                                    res0.IsDoubt = true;
                            }
                            if ((li.Count == 2 && nt == li[0] && li[1].Item != null) && !(li[1].Termin.Tag is Pullenti.Ner.Core.IntOntologyItem)) 
                                res0.OntoItem2 = li[1].Item;
                            return res0;
                        }
                    }
                }
                foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                {
                    if (nt.Item != null && (nt.Termin.Tag is Pullenti.Ner.Core.IntOntologyItem)) 
                    {
                        if (nt.EndToken.Next == null || !nt.EndToken.Next.IsHiphen) 
                        {
                            TerrItemToken res1 = new TerrItemToken(nt.BeginToken, nt.EndToken) { OntoItem = nt.Item, IsAdjective = true, Morph = nt.Morph };
                            if ((li.Count == 2 && nt == li[0] && li[1].Item != null) && (li[1].Termin.Tag is Pullenti.Ner.Core.IntOntologyItem)) 
                                res1.OntoItem2 = li[1].Item;
                            if (t.Kit.BaseLanguage.IsUa && res1.OntoItem.CanonicText == "СУДАН" && t.IsValue("СУД", null)) 
                                return null;
                            return res1;
                        }
                    }
                }
                foreach (Pullenti.Ner.Core.IntOntologyToken nt in li) 
                {
                    if (nt.Termin != null && nt.Item == null) 
                    {
                        if (nt.EndToken.Next == null || !nt.EndToken.Next.IsHiphen || !(nt.Termin as TerrTermin).IsAdjective) 
                        {
                            TerrItemToken res1 = new TerrItemToken(nt.BeginToken, nt.EndToken) { TerminItem = nt.Termin as TerrTermin, IsAdjective = (nt.Termin as TerrTermin).IsAdjective, Morph = nt.Morph };
                            if (!res1.IsAdjective) 
                            {
                                if (res1.TerminItem.CanonicText == "РАЙОН") 
                                {
                                    if (t.Previous != null) 
                                    {
                                        if (t.Previous.IsValue("МИКРО", null)) 
                                            return null;
                                        if (t.Previous.IsCharOf("\\/.") && t.Previous.Previous != null && t.Previous.Previous.IsValue("М", null)) 
                                            return null;
                                    }
                                    if (!res1.BeginToken.IsValue(res1.TerminItem.CanonicText, null)) 
                                    {
                                        if (res1.EndToken.Next != null && res1.EndToken.Next.IsChar('.')) 
                                            res1.EndToken = res1.EndToken.Next;
                                    }
                                }
                                if (res1.TerminItem.CanonicText == "РЕСПУБЛИКА" || res1.TerminItem.CanonicText == "ШТАТ") 
                                {
                                    Pullenti.Ner.Core.NounPhraseToken npt1 = MiscLocationHelper.TryParseNpt(res1.BeginToken.Previous);
                                    if (npt1 != null && npt1.Morph.Number == Pullenti.Morph.MorphNumber.Plural) 
                                    {
                                        TerrItemToken res2 = TryParse(res1.EndToken.Next, null, null);
                                        if ((res2 != null && res2.OntoItem != null && res2.OntoItem.Referent != null) && res2.OntoItem.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", true) != null) 
                                        {
                                        }
                                        else 
                                            return null;
                                    }
                                }
                                if (res1.TerminItem.CanonicText == "ГОСУДАРСТВО") 
                                {
                                    if (t.Previous != null && t.Previous.IsValue("СОЮЗНЫЙ", null)) 
                                        return null;
                                }
                                if (nt.BeginToken == nt.EndToken && nt.BeginToken.IsValue("ОПС", null)) 
                                {
                                    if (!MiscLocationHelper.CheckGeoObjectBefore(nt.BeginToken, false)) 
                                        return null;
                                }
                            }
                            return res1;
                        }
                    }
                }
            }
            if (tt == null) 
                return null;
            if (!tt.Chars.IsCapitalUpper && !tt.Chars.IsAllUpper) 
                return null;
            if (((tt.LengthChar == 2 || tt.LengthChar == 3)) && tt.Chars.IsAllUpper) 
            {
                if (m_Alpha2State.ContainsKey(tt.Term)) 
                {
                    bool ok = false;
                    Pullenti.Ner.Token tt2 = tt.Next;
                    if (tt2 != null && tt2.IsChar(':')) 
                        tt2 = tt2.Next;
                    if (tt2 is Pullenti.Ner.ReferentToken) 
                    {
                        Pullenti.Ner.Referent r = tt2.GetReferent();
                        if (r != null && r.TypeName == "PHONE") 
                            ok = true;
                    }
                    if (ok) 
                        return new TerrItemToken(tt, tt) { OntoItem = m_Alpha2State[tt.Term] };
                }
            }
            if (tt.LengthChar < 3) 
                return null;
            if (Pullenti.Ner.Core.MiscHelper.IsEngArticle(tt)) 
                return null;
            if (tt.LengthChar < 5) 
            {
                if (tt.Next == null || !tt.Next.IsHiphen) 
                    return null;
            }
            Pullenti.Ner.TextToken t0 = tt;
            string prefix = null;
            if (t0.Next != null && t0.Next.IsHiphen && (t0.Next.Next is Pullenti.Ner.TextToken)) 
            {
                tt = t0.Next.Next as Pullenti.Ner.TextToken;
                if (!tt.Chars.IsAllLower && ((t0.IsWhitespaceAfter || t0.Next.IsWhitespaceAfter))) 
                {
                    TerrItemToken tit = _TryParse(tt, prev, false);
                    if (tit != null) 
                    {
                        if (tit.OntoItem != null) 
                            return null;
                    }
                }
                if (tt.LengthChar > 1) 
                {
                    if (tt.Chars.IsCapitalUpper) 
                        prefix = t0.Term;
                    else if (!tt.IsWhitespaceBefore && !t0.IsWhitespaceAfter) 
                        prefix = t0.Term;
                    if (((!tt.IsWhitespaceAfter && tt.Next != null && tt.Next.IsHiphen) && !tt.Next.IsWhitespaceAfter && (tt.Next.Next is Pullenti.Ner.TextToken)) && tt.Next.Next.Chars.Equals(t0.Chars)) 
                    {
                        prefix = string.Format("{0}-{1}", prefix, tt.Term);
                        tt = tt.Next.Next as Pullenti.Ner.TextToken;
                    }
                }
                if (prefix == null) 
                    tt = t0;
            }
            if (tt.Morph.Class.IsAdverb) 
                return null;
            if (CityItemToken.CheckKeyword(t0) != null) 
                return null;
            if (!tt.Morph.Class.IsAdjective) 
            {
                if (CityItemToken.CheckOntoItem(t0) != null) 
                    return null;
            }
            Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(t0);
            if (npt != null) 
            {
                if (((npt.Noun.IsValue("ФЕДЕРАЦИЯ", null) || npt.Noun.IsValue("ФЕДЕРАЦІЯ", null))) && npt.Adjectives.Count == 1) 
                {
                    if (Pullenti.Ner.Core.MiscHelper.IsNotMoreThanOneError("РОССИЙСКАЯ", npt.Adjectives[0]) || Pullenti.Ner.Core.MiscHelper.IsNotMoreThanOneError("РОСІЙСЬКА", npt.Adjectives[0])) 
                        return new TerrItemToken(npt.BeginToken, npt.EndToken) { OntoItem = (t0.Kit.BaseLanguage.IsUa ? m_RussiaUA : m_RussiaRU), Morph = npt.Morph };
                }
            }
            if (t0.Morph.Class.IsProperName) 
            {
                if (t0.IsWhitespaceAfter || t0.Next.IsWhitespaceAfter) 
                    return null;
            }
            if (!t0.Chars.IsAllLower) 
            {
                Pullenti.Ner.Core.TerminToken tok2 = m_SpecNames.TryParse(t0, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok2 != null) 
                    return new TerrItemToken(t0, tok2.EndToken) { IsDistrictName = true };
            }
            if (npt != null && npt.EndToken != npt.BeginToken) 
            {
                if (npt.EndToken.IsValue("КИЛОМЕТР", null)) 
                    npt = null;
                else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(npt.EndToken, true)) 
                    npt = null;
                else 
                {
                    List<Pullenti.Ner.Core.IntOntologyToken> tok = m_TerrOntology.TryAttach(npt.EndToken, null, false);
                    if (tok != null) 
                        npt = null;
                    else 
                    {
                        TerrItemToken next = TryParse(npt.EndToken, null, null);
                        if (next != null && next.TerminItem != null) 
                        {
                            if (MiscLocationHelper.CheckGeoObjectAfter(npt.EndToken.Previous, false)) 
                                npt = null;
                        }
                        else if (CityItemToken.CheckKeyword(npt.EndToken) != null) 
                        {
                            if (MiscLocationHelper.CheckGeoObjectAfter(npt.EndToken.Previous, false)) 
                                npt = null;
                        }
                    }
                }
            }
            if (npt != null && npt.EndToken == tt.Next) 
            {
                bool adj = false;
                bool regAfter = false;
                if (npt.Adjectives.Count == 1 && !t0.Chars.IsAllLower) 
                {
                    if (((((tt.Next.IsValue("РАЙОН", null) || tt.Next.IsValue("ОБЛАСТЬ", null) || tt.Next.IsValue("КРАЙ", null)) || tt.Next.IsValue("ВОЛОСТЬ", null) || tt.Next.IsValue("УЛУС", null)) || tt.Next.IsValue("ОКРУГ", null) || tt.Next.IsValue("АВТОНОМИЯ", "АВТОНОМІЯ")) || tt.Next.IsValue("РЕСПУБЛИКА", "РЕСПУБЛІКА") || tt.Next.IsValue("COUNTY", null)) || tt.Next.IsValue("STATE", null) || tt.Next.IsValue("REGION", null)) 
                        regAfter = true;
                    else 
                    {
                        List<Pullenti.Ner.Core.IntOntologyToken> tok = m_TerrOntology.TryAttach(tt.Next, null, false);
                        if (tok != null) 
                        {
                            if ((((tok[0].Termin.CanonicText == "РАЙОН" || tok[0].Termin.CanonicText == "ОБЛАСТЬ" || tok[0].Termin.CanonicText == "УЛУС") || tok[0].Termin.CanonicText == "КРАЙ" || tok[0].Termin.CanonicText == "ВОЛОСТЬ") || tok[0].Termin.CanonicText == "ОКРУГ" || tok[0].Termin.CanonicText == "АВТОНОМИЯ") || tok[0].Termin.CanonicText == "АВТОНОМІЯ" || ((tok[0].Chars.IsLatinLetter && (tok[0].Termin is TerrTermin) && (tok[0].Termin as TerrTermin).IsRegion))) 
                                regAfter = true;
                        }
                    }
                }
                if (regAfter) 
                {
                    adj = true;
                    foreach (Pullenti.Morph.MorphBaseInfo wff in tt.Morph.Items) 
                    {
                        Pullenti.Morph.MorphWordForm wf = wff as Pullenti.Morph.MorphWordForm;
                        if (wf == null) 
                            continue;
                        if (wf.Class.IsVerb && wf.IsInDictionary) 
                        {
                            adj = false;
                            break;
                        }
                        else if (wf.IsInDictionary && !wf.Class.IsAdjective) 
                        {
                        }
                    }
                    if (!adj && prefix != null) 
                        adj = true;
                    if (!adj) 
                    {
                        if (CityItemToken.CheckKeyword(tt.Next.Next) != null || CityItemToken.CheckOntoItem(tt.Next.Next) != null) 
                            adj = true;
                    }
                    if (!adj) 
                    {
                        if (MiscLocationHelper.CheckGeoObjectBefore(npt.BeginToken, false)) 
                            adj = true;
                    }
                    Pullenti.Ner.Token te = tt.Next.Next;
                    if (te != null && te.IsCharOf(",")) 
                        te = te.Next;
                    if (!adj && (te is Pullenti.Ner.ReferentToken)) 
                    {
                        if (te.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                            adj = true;
                    }
                    if (!adj) 
                    {
                        te = t0.Previous;
                        if (te != null && te.IsCharOf(",")) 
                            te = te.Previous;
                        if (te is Pullenti.Ner.ReferentToken) 
                        {
                            if (te.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                                adj = true;
                        }
                    }
                    if (adj && npt.Adjectives[0].BeginToken != npt.Adjectives[0].EndToken) 
                    {
                        if (!npt.Adjectives[0].BeginToken.Chars.Equals(npt.Adjectives[0].EndToken.Chars)) 
                            return null;
                    }
                }
                else if ((npt.Adjectives.Count == 1 && (npt.EndToken is Pullenti.Ner.TextToken) && npt.EndToken.GetMorphClassInDictionary().IsNoun) && prev != null && prev.TerminItem != null) 
                {
                    adj = true;
                    tt = npt.EndToken as Pullenti.Ner.TextToken;
                }
                if (!adj && !t0.Chars.IsLatinLetter) 
                    return null;
            }
            TerrItemToken res = new TerrItemToken(t0, tt);
            res.IsAdjective = tt.Morph.Class.IsAdjective;
            res.Morph = tt.Morph;
            if (npt != null && npt.EndChar > res.EndChar && npt.Morph.Gender != Pullenti.Morph.MorphGender.Undefined) 
            {
                res.Morph = new Pullenti.Ner.MorphCollection(tt.Morph);
                res.Morph.RemoveItems(npt.Morph.Gender);
            }
            if (t0 is Pullenti.Ner.TextToken) 
            {
                foreach (Pullenti.Morph.MorphBaseInfo wf in t0.Morph.Items) 
                {
                    Pullenti.Morph.MorphWordForm f = wf as Pullenti.Morph.MorphWordForm;
                    if (!f.IsInDictionary) 
                        continue;
                    if (wf.Class.IsProperSurname && f.IsInDictionary) 
                        res.CanBeSurname = true;
                    else if (wf.Class.IsAdjective && f.IsInDictionary) 
                        res.IsAdjInDictionary = true;
                    else if (wf.Class.IsProperGeo) 
                    {
                        if (!t0.Chars.IsAllLower) 
                            res.IsGeoInDictionary = true;
                    }
                }
            }
            if ((tt.WhitespacesAfterCount < 2) && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.Chars.IsCapitalUpper) 
            {
                Pullenti.Ner.MetaToken dir = MiscLocationHelper.TryAttachNordWest(tt.Next);
                if (dir != null) 
                    res.EndToken = dir.EndToken;
            }
            if (((res.BeginToken == res.EndToken && res.IsAdjective && (res.WhitespacesAfterCount < 2)) && (res.EndToken.Next is Pullenti.Ner.TextToken) && res.EndToken.Next.Chars.IsCapitalUpper) && prev != null) 
            {
                if (MiscLocationHelper.CheckGeoObjectAfter(res.EndToken.Next, false)) 
                    res.EndToken = res.EndToken.Next;
                else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(res.EndToken.Next.Next, false)) 
                    res.EndToken = res.EndToken.Next;
            }
            return res;
        }
        static TerrItemToken _tryParseDistrictName(Pullenti.Ner.Token t, int lev = 0)
        {
            if (lev > 2) 
                return null;
            if (!(t is Pullenti.Ner.TextToken) || !t.Chars.IsCapitalUpper || !t.Chars.IsCyrillicLetter) 
                return null;
            if ((t.Next != null && t.Next.IsHiphen && (t.Next.Next is Pullenti.Ner.TextToken)) && t.Next.Next.Chars.Equals(t.Chars)) 
            {
                List<Pullenti.Ner.Core.IntOntologyToken> tok = m_TerrOntology.TryAttach(t, null, false);
                if ((tok != null && tok[0].Item != null && (tok[0].Item.Referent is Pullenti.Ner.Geo.GeoReferent)) && (tok[0].Item.Referent as Pullenti.Ner.Geo.GeoReferent).IsState) 
                    return null;
                tok = m_TerrOntology.TryAttach(t.Next.Next, null, false);
                if ((tok != null && tok[0].Item != null && (tok[0].Item.Referent is Pullenti.Ner.Geo.GeoReferent)) && (tok[0].Item.Referent as Pullenti.Ner.Geo.GeoReferent).IsState) 
                    return null;
                return new TerrItemToken(t, t.Next.Next);
            }
            if ((t.Next is Pullenti.Ner.TextToken) && t.Next.Chars.Equals(t.Chars)) 
            {
                Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(t);
                if (npt != null && npt.EndToken == t.Next && npt.Adjectives.Count == 1) 
                {
                    if (!npt.EndToken.Morph.Class.IsAdjective || ((npt.EndToken.Morph.Case.IsNominative && (npt.EndToken is Pullenti.Ner.TextToken) && Pullenti.Morph.LanguageHelper.EndsWith((npt.EndToken as Pullenti.Ner.TextToken).Term, "О")))) 
                    {
                        TerrItemToken ty = _TryParse(t.Next, null, false);
                        if (ty != null && ty.TerminItem != null) 
                            return null;
                        return new TerrItemToken(t, t.Next);
                    }
                }
            }
            string str = (t as Pullenti.Ner.TextToken).Term;
            TerrItemToken res = new TerrItemToken(t, t) { IsDoubt = true };
            if (!Pullenti.Morph.LanguageHelper.EndsWith(str, "О")) 
                res.IsDoubt = true;
            Pullenti.Ner.MetaToken dir = MiscLocationHelper.TryAttachNordWest(t);
            if (dir != null) 
            {
                res.EndToken = dir.EndToken;
                res.IsDoubt = false;
                if (res.EndToken.WhitespacesAfterCount < 2) 
                {
                    TerrItemToken res2 = _tryParseDistrictName(res.EndToken.Next, lev + 1);
                    if (res2 != null && res2.TerminItem == null) 
                        res.EndToken = res2.EndToken;
                }
            }
            return res;
        }
        public static List<TerrItemToken> TryParseList(Pullenti.Ner.Token t, int maxCount, GeoAnalyzerData ad = null)
        {
            TerrItemToken ci = TerrItemToken.TryParse(t, null, ad);
            if (ci == null) 
                return null;
            List<TerrItemToken> li = new List<TerrItemToken>();
            li.Add(ci);
            t = ci.EndToken.Next;
            if (t == null) 
                return li;
            if (ci.TerminItem != null && ci.TerminItem.CanonicText == "АВТОНОМИЯ") 
            {
                if (t.Morph.Case.IsGenitive) 
                    return null;
            }
            for (t = ci.EndToken.Next; t != null; ) 
            {
                ci = TerrItemToken.TryParse(t, li[li.Count - 1], ad);
                if (ci == null) 
                {
                    if (t.Chars.IsCapitalUpper && li.Count == 1 && ((li[0].IsCityRegion || ((li[0].TerminItem != null && li[0].TerminItem.IsSpecificPrefix))))) 
                    {
                        CityItemToken cit = CityItemToken.TryParse(t, null, false, null);
                        if (cit != null && cit.Typ == CityItemToken.ItemType.ProperName) 
                            ci = new TerrItemToken(cit.BeginToken, cit.EndToken);
                    }
                    else if ((Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, false, false) && t.Next != null && ((t.Next.Chars.IsCapitalUpper || t.Next.Chars.IsAllUpper))) && li.Count == 1 && ((li[0].IsCityRegion || ((li[0].TerminItem != null && li[0].TerminItem.IsSpecificPrefix))))) 
                    {
                        CityItemToken cit = CityItemToken.TryParse(t.Next, null, false, null);
                        if (cit != null && ((cit.Typ == CityItemToken.ItemType.ProperName || cit.Typ == CityItemToken.ItemType.City)) && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(cit.EndToken.Next, false, null, false)) 
                            ci = new TerrItemToken(t, cit.EndToken.Next);
                        else 
                        {
                            Pullenti.Ner.Core.BracketSequenceToken brr = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                            if (brr != null) 
                            {
                                bool ok = false;
                                Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", t.Next, null);
                                if (rt != null && rt.ToString().ToUpper().Contains("СОВЕТ")) 
                                    ok = true;
                                else if (brr.LengthChar < 40) 
                                    ok = true;
                                if (ok) 
                                    ci = new TerrItemToken(t, brr.EndToken);
                            }
                        }
                    }
                    else if (t.IsChar('(')) 
                    {
                        ci = TerrItemToken.TryParse(t.Next, null, ad);
                        if (ci != null && ci.EndToken.Next != null && ci.EndToken.Next.IsChar(')')) 
                        {
                            TerrItemToken ci0 = li[li.Count - 1];
                            if (ci0.OntoItem != null && ci.OntoItem == ci0.OntoItem) 
                            {
                                ci0.EndToken = ci.EndToken.Next;
                                t = ci0.EndToken.Next;
                            }
                            else 
                            {
                                TerrItemToken ci1 = new TerrItemToken(t, ci.EndToken.Next) { OntoItem = ci.OntoItem, TerminItem = ci.TerminItem };
                                li.Add(ci1);
                                t = ci1.EndToken;
                            }
                            continue;
                        }
                    }
                    else if ((t.IsComma && li.Count == 1 && li[0].TerminItem == null) && (t.WhitespacesAfterCount < 3)) 
                    {
                        List<TerrItemToken> li2 = TryParseList(t.Next, 2, null);
                        if (li2 != null && li2.Count == 1 && li2[0].TerminItem != null) 
                        {
                            Pullenti.Ner.Token tt2 = li2[0].EndToken.Next;
                            bool ok = false;
                            if (tt2 == null || tt2.WhitespacesBeforeCount > 3) 
                                ok = true;
                            else if (((tt2.LengthChar == 1 && !tt2.IsLetters)) || !(tt2 is Pullenti.Ner.TextToken)) 
                                ok = true;
                            if (ok) 
                            {
                                li.Add(li2[0]);
                                t = li2[0].EndToken;
                                break;
                            }
                        }
                    }
                    if (ci == null && Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, false, false)) 
                    {
                        List<TerrItemToken> lii = TryParseList(t.Next, maxCount, null);
                        if (lii != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(lii[lii.Count - 1].EndToken.Next, false, null, false)) 
                        {
                            li.AddRange(lii);
                            return li;
                        }
                    }
                    if (ci == null) 
                        break;
                }
                if (t.IsTableControlChar) 
                    break;
                if (t.IsNewlineBefore) 
                {
                    if (t.NewlinesBeforeCount > 1) 
                        break;
                    if (li.Count > 0 && li[li.Count - 1].IsAdjective && ci.TerminItem != null) 
                    {
                    }
                    else if (li.Count == 1 && li[0].TerminItem != null && ci.TerminItem == null) 
                    {
                    }
                    else 
                        break;
                }
                if (ci.TerminItem != null && ci.TerminItem.CanonicText == "ТЕРРИТОРИЯ") 
                    break;
                li.Add(ci);
                t = ci.EndToken.Next;
                if (maxCount > 0 && li.Count >= maxCount) 
                    break;
            }
            foreach (TerrItemToken cc in li) 
            {
                if (cc.OntoItem != null && !cc.IsAdjective) 
                {
                    if (!cc.BeginToken.Chars.IsCyrillicLetter) 
                        continue;
                    string alpha2 = null;
                    if (cc.OntoItem.Referent is Pullenti.Ner.Geo.GeoReferent) 
                        alpha2 = (cc.OntoItem.Referent as Pullenti.Ner.Geo.GeoReferent).Alpha2;
                    if (alpha2 == "TG") 
                    {
                        if (cc.BeginToken is Pullenti.Ner.TextToken) 
                        {
                            if (cc.BeginToken.GetSourceText() != "Того") 
                                return null;
                            if (li.Count == 1 && cc.BeginToken.Previous != null && cc.BeginToken.Previous.IsChar('.')) 
                                return null;
                            Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(cc.BeginToken, Pullenti.Ner.Core.NounPhraseParseAttr.ParsePronouns, 0, null);
                            if (npt != null && npt.EndToken != cc.BeginToken) 
                                return null;
                            if (cc.BeginToken.Next != null) 
                            {
                                if (cc.BeginToken.Next.Morph.Class.IsPersonalPronoun || cc.BeginToken.Next.Morph.Class.IsPronoun) 
                                    return null;
                            }
                        }
                        if (li.Count < 2) 
                            return null;
                    }
                    if (alpha2 == "PE") 
                    {
                        if (cc.BeginToken is Pullenti.Ner.TextToken) 
                        {
                            if ((cc.BeginToken as Pullenti.Ner.TextToken).GetSourceText() != "Перу") 
                                return null;
                            if (li.Count == 1 && cc.BeginToken.Previous != null && cc.BeginToken.Previous.IsChar('.')) 
                                return null;
                        }
                        if (li.Count < 2) 
                            return null;
                    }
                    if (alpha2 == "DM") 
                    {
                        if (cc.EndToken.Next != null) 
                        {
                            if (cc.EndToken.Next.Chars.IsCapitalUpper || cc.EndToken.Next.Chars.IsAllUpper) 
                                return null;
                        }
                        return null;
                    }
                    if (alpha2 == "JE") 
                    {
                        if (cc.BeginToken.Previous != null && cc.BeginToken.Previous.IsHiphen) 
                            return null;
                    }
                    return li;
                }
                else if (cc.OntoItem != null && cc.IsAdjective) 
                {
                    string alpha2 = null;
                    if (cc.OntoItem.Referent is Pullenti.Ner.Geo.GeoReferent) 
                        alpha2 = (cc.OntoItem.Referent as Pullenti.Ner.Geo.GeoReferent).Alpha2;
                    if (alpha2 == "SU") 
                    {
                        if (cc.EndToken.Next == null || !cc.EndToken.Next.IsValue("СОЮЗ", null)) 
                            cc.OntoItem = null;
                    }
                }
            }
            for (int i = 0; i < li.Count; i++) 
            {
                if (li[i].OntoItem != null && li[i].OntoItem2 != null) 
                {
                    Pullenti.Ner.Core.Termin nou = null;
                    if (i > 0 && li[i - 1].TerminItem != null) 
                        nou = li[i - 1].TerminItem;
                    else if (((i + 1) < li.Count) && li[i + 1].TerminItem != null) 
                        nou = li[i + 1].TerminItem;
                    if (nou == null || li[i].OntoItem.Referent == null || li[i].OntoItem2.Referent == null) 
                        continue;
                    if (li[i].OntoItem.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, nou.CanonicText.ToLower(), true) == null && li[i].OntoItem2.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, nou.CanonicText.ToLower(), true) != null) 
                    {
                        li[i].OntoItem = li[i].OntoItem2;
                        li[i].OntoItem2 = null;
                    }
                    else if (li[i].OntoItem.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "республика", true) != null && nou.CanonicText != "РЕСПУБЛИКА") 
                    {
                        li[i].OntoItem = li[i].OntoItem2;
                        li[i].OntoItem2 = null;
                    }
                }
            }
            if ((li.Count >= 3 && li[0].TerminItem == null && li[1].TerminItem != null) && li[2].TerminItem == null) 
            {
                if (li.Count == 3 || ((li.Count >= 5 && ((((li[3].TerminItem != null && li[4].TerminItem == null)) || ((li[4].TerminItem != null && li[3].TerminItem == null))))))) 
                {
                    Pullenti.Ner.Token t1 = li[0].BeginToken.Previous;
                    if (t1 != null && t1.IsChar('.') && t1.Previous != null) 
                    {
                        t1 = t1.Previous;
                        CityItemToken cit = CityItemToken.TryParseBack(t1, false);
                        if (cit != null) 
                            li.RemoveAt(0);
                        else if (t1.Chars.IsAllLower && ((t1.IsValue("С", null) || t1.IsValue("П", null) || t1.IsValue("ПОС", null)))) 
                            li.RemoveAt(0);
                    }
                }
            }
            foreach (TerrItemToken cc in li) 
            {
                if (cc.OntoItem != null || ((cc.TerminItem != null && !cc.IsAdjective))) 
                    return li;
            }
            return null;
        }
        public static void Initialize()
        {
            if (m_TerrOntology != null) 
                return;
            m_TerrOntology = new Pullenti.Ner.Core.IntOntologyCollection();
            m_TerrAdjs = new Pullenti.Ner.Core.TerminCollection();
            m_MansByState = new Pullenti.Ner.Core.TerminCollection();
            m_UnknownRegions = new Pullenti.Ner.Core.TerminCollection();
            m_TerrNounAdjectives = new Pullenti.Ner.Core.TerminCollection();
            m_CapitalsByState = new Pullenti.Ner.Core.TerminCollection();
            m_GeoAbbrs = new Pullenti.Ner.Core.TerminCollection();
            TerrTermin t = new TerrTermin("РЕСПУБЛИКА") { Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("РЕСП.");
            t.AddAbridge("РЕСП-КА");
            t.AddAbridge("РЕСПУБ.");
            t.AddAbridge("РЕСПУБЛ.");
            t.AddAbridge("Р-КА");
            t.AddAbridge("РЕСП-КА");
            m_TerrOntology.Add(t);
            m_TerrOntology.Add(new TerrTermin("РЕСПУБЛІКА", Pullenti.Morph.MorphLang.UA) { Gender = Pullenti.Morph.MorphGender.Feminie });
            t = new TerrTermin("ГОСУДАРСТВО") { IsState = true, Gender = Pullenti.Morph.MorphGender.Neuter };
            t.AddAbridge("ГОС-ВО");
            m_TerrOntology.Add(t);
            t = new TerrTermin("ДЕРЖАВА", Pullenti.Morph.MorphLang.UA) { IsState = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("АВТОНОМНАЯ СОВЕТСКАЯ СОЦИАЛИСТИЧЕСКАЯ РЕСПУБЛИКА") { Gender = Pullenti.Morph.MorphGender.Feminie };
            t.Acronym = "АССР";
            m_TerrOntology.Add(t);
            foreach (string s in new string[] {"СОЮЗ", "СОДРУЖЕСТВО", "ФЕДЕРАЦИЯ", "КОНФЕДЕРАЦИЯ"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s) { IsState = true, IsDoubt = true });
            }
            foreach (string s in new string[] {"СОЮЗ", "СПІВДРУЖНІСТЬ", "ФЕДЕРАЦІЯ", "КОНФЕДЕРАЦІЯ"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s, Pullenti.Morph.MorphLang.UA) { IsState = true, IsDoubt = true });
            }
            foreach (string s in new string[] {"КОРОЛЕВСТВО", "КНЯЖЕСТВО", "ГЕРЦОГСТВО", "ИМПЕРИЯ", "ЦАРСТВО", "KINGDOM", "DUCHY", "EMPIRE"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s) { IsState = true });
            }
            foreach (string s in new string[] {"КОРОЛІВСТВО", "КНЯЗІВСТВО", "ГЕРЦОГСТВО", "ІМПЕРІЯ"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s, Pullenti.Morph.MorphLang.UA) { IsState = true });
            }
            foreach (string s in new string[] {"НЕЗАВИСИМЫЙ", "ОБЪЕДИНЕННЫЙ", "СОЕДИНЕННЫЙ", "НАРОДНЫЙ", "НАРОДНО", "ФЕДЕРАТИВНЫЙ", "ДЕМОКРАТИЧЕСКИЙ", "СОВЕТСКИЙ", "СОЦИАЛИСТИЧЕСКИЙ", "КООПЕРАТИВНЫЙ", "ИСЛАМСКИЙ", "АРАБСКИЙ", "МНОГОНАЦИОНАЛЬНЫЙ", "СУВЕРЕННЫЙ", "САМОПРОВОЗГЛАШЕННЫЙ", "НЕПРИЗНАННЫЙ"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s) { IsState = true, IsAdjective = true });
            }
            foreach (string s in new string[] {"НЕЗАЛЕЖНИЙ", "ОБЄДНАНИЙ", "СПОЛУЧЕНИЙ", "НАРОДНИЙ", "ФЕДЕРАЛЬНИЙ", "ДЕМОКРАТИЧНИЙ", "РАДЯНСЬКИЙ", "СОЦІАЛІСТИЧНИЙ", "КООПЕРАТИВНИЙ", "ІСЛАМСЬКИЙ", "АРАБСЬКИЙ", "БАГАТОНАЦІОНАЛЬНИЙ", "СУВЕРЕННИЙ"}) 
            {
                m_TerrOntology.Add(new TerrTermin(s, Pullenti.Morph.MorphLang.UA) { IsState = true, IsAdjective = true });
            }
            t = new TerrTermin("ОБЛАСТЬ") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ОБЛ.");
            m_TerrNounAdjectives.Add(new Pullenti.Ner.Core.Termin("ОБЛАСТНОЙ") { Tag = t });
            m_TerrOntology.Add(t);
            t = new TerrTermin("REGION") { IsRegion = true };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ОБЛАСТЬ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            t.AddAbridge("ОБЛ.");
            m_TerrOntology.Add(t);
            t = new TerrTermin(null) { IsRegion = true, Acronym = "АО" };
            t.AddVariant("АОБЛ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin(null, Pullenti.Morph.MorphLang.UA) { IsRegion = true, Acronym = "АО" };
            m_TerrOntology.Add(t);
            t = new TerrTermin("РАЙОН") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("Р-Н");
            t.AddAbridge("Р-ОН");
            t.AddAbridge("РН.");
            m_TerrNounAdjectives.Add(new Pullenti.Ner.Core.Termin("РАЙОННЫЙ") { Tag = t });
            m_TerrOntology.Add(t);
            t = new TerrTermin("РАЙОН", Pullenti.Morph.MorphLang.UA) { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("Р-Н");
            t.AddAbridge("Р-ОН");
            t.AddAbridge("РН.");
            m_TerrOntology.Add(t);
            t = new TerrTermin("УЛУС") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_TerrOntology.Add(t);
            t = new TerrTermin("УЕЗД") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ГУБЕРНАТОРСТВО") { IsRegion = true, IsAlwaysPrefix = true, Gender = Pullenti.Morph.MorphGender.Neuter };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ШТАТ") { IsRegion = true, IsAlwaysPrefix = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_TerrOntology.Add(t);
            t = new TerrTermin("STATE") { IsRegion = true };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ШТАТ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, IsAlwaysPrefix = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ПРОВИНЦИЯ") { IsRegion = true, IsAlwaysPrefix = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ПРОВІНЦІЯ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, IsAlwaysPrefix = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("PROVINCE") { IsRegion = true };
            t.AddVariant("PROVINCIAL", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ПРЕФЕКТУРА") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("PREFECTURE") { IsRegion = true };
            m_TerrOntology.Add(t);
            t = new TerrTermin("АВТОНОМИЯ") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("AUTONOMY") { IsRegion = true };
            m_TerrOntology.Add(t);
            t = new TerrTermin("АВТОНОМІЯ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Feminie };
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЗАКРЫТОЕ АДМИНИСТРАТИВНО ТЕРРИТОРИАЛЬНОЕ ОБРАЗОВАНИЕ") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Neuter, Acronym = "ЗАТО" };
            m_TerrOntology.Add(t);
            foreach (string s in new string[] {"РЕСПУБЛИКА", "КРАЙ", "ОКРУГ", "ФЕДЕРАЛЬНЫЙ ОКРУГ", "АВТОНОМНЫЙ ОКРУГ", "АВТОНОМНАЯ ОБЛАСТЬ", "НАЦИОНАЛЬНЫЙ ОКРУГ", "ВОЛОСТЬ", "ФЕДЕРАЛЬНАЯ ЗЕМЛЯ", "ВОЕВОДСТВО", "МУНИЦИПАЛЬНЫЙ РАЙОН", "МУНИЦИПАЛЬНЫЙ ОКРУГ", "АДМИНИСТРАТИВНЫЙ ОКРУГ", "ГОРОДСКОЙ РАЙОН", "ВНУТРИГОРОДСКОЙ РАЙОН", "АДМИНИСТРАТИВНЫЙ РАЙОН", "ВНУТРИГОРОДСКОЕ МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ", "СЕЛЬСКОЕ МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ", "ВНУТРИГОРОДСКАЯ ТЕРРИТОРИЯ", "МЕЖСЕЛЕННАЯ ТЕРРИТОРИЯ", "REPUBLIC", "COUNTY", "BOROUGH", "PARISH", "MUNICIPALITY", "CENSUS AREA", "AUTONOMOUS REGION", "ADMINISTRATIVE REGION", "SPECIAL ADMINISTRATIVE REGION"}) 
            {
                t = new TerrTermin(s) { IsRegion = true, IsStrong = s.Contains(" ") };
                if (s == "КРАЙ") 
                {
                    t.AddAbridge("КР.");
                    m_TerrNounAdjectives.Add(new Pullenti.Ner.Core.Termin("КРАЕВОЙ") { Tag = t });
                    t.Gender = Pullenti.Morph.MorphGender.Masculine;
                }
                else if (s == "ОКРУГ") 
                {
                    m_TerrNounAdjectives.Add(new Pullenti.Ner.Core.Termin("ОКРУЖНОЙ") { Tag = t });
                    t.AddAbridge("ОКР.");
                }
                else if (s == "ФЕДЕРАЛЬНЫЙ ОКРУГ") 
                    t.Acronym = "ФО";
                if (Pullenti.Morph.LanguageHelper.EndsWith(s, "РАЙОН")) 
                {
                    t.AddAbridge(s.Replace("РАЙОН", "Р-Н"));
                    t.Gender = Pullenti.Morph.MorphGender.Masculine;
                    if (s == "МУНИЦИПАЛЬНЫЙ РАЙОН") 
                    {
                        t.AddAbridge("М.Р-Н");
                        t.AddAbridge("М Р-Н");
                        t.AddAbridge("МУН.Р-Н");
                    }
                }
                if (Pullenti.Morph.LanguageHelper.EndsWith(s, "ОКРУГ")) 
                    t.Gender = Pullenti.Morph.MorphGender.Masculine;
                if (Pullenti.Morph.LanguageHelper.EndsWith(s, "ОБРАЗОВАНИЕ")) 
                    t.Gender = Pullenti.Morph.MorphGender.Neuter;
                m_TerrOntology.Add(t);
            }
            foreach (string s in new string[] {"РЕСПУБЛІКА", "КРАЙ", "ОКРУГ", "ФЕДЕРАЛЬНИЙ ОКРУГ", "АВТОНОМНИЙ ОКРУГ", "АВТОНОМНА ОБЛАСТЬ", "НАЦІОНАЛЬНИЙ ОКРУГ", "ВОЛОСТЬ", "ФЕДЕРАЛЬНА ЗЕМЛЯ", "МУНІЦИПАЛЬНИЙ РАЙОН", "МУНІЦИПАЛЬНИЙ ОКРУГ", "АДМІНІСТРАТИВНИЙ ОКРУГ", "МІСЬКИЙ РАЙОН", "ВНУТРИГОРОДСКОЕ МУНІЦИПАЛЬНЕ УТВОРЕННЯ"}) 
            {
                t = new TerrTermin(s, Pullenti.Morph.MorphLang.UA) { IsRegion = true, IsStrong = s.Contains(" ") };
                if (Pullenti.Morph.LanguageHelper.EndsWith(s, "РАЙОН")) 
                {
                    t.AddAbridge(s.Replace("РАЙОН", "Р-Н"));
                    t.Gender = Pullenti.Morph.MorphGender.Masculine;
                }
                m_TerrOntology.Add(t);
            }
            t = new TerrTermin("ГОРОДСКОЙ ОКРУГ") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("ГОР. ОКРУГ");
            t.AddAbridge("Г.О.");
            t.AddAbridge("Г/ОКРУГ");
            t.AddAbridge("Г/О");
            m_TerrOntology.Add(t);
            t = new TerrTermin("СЕЛЬСКИЙ ОКРУГ") { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("С.О.");
            t.AddAbridge("C.O.");
            t.AddAbridge("ПС С.О.");
            t.AddAbridge("С/ОКРУГ");
            t.AddAbridge("С/О");
            m_TerrOntology.Add(t);
            t = new TerrTermin("СІЛЬСЬКИЙ ОКРУГ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("С.О.");
            t.AddAbridge("C.O.");
            t.AddAbridge("С/ОКРУГ");
            m_TerrOntology.Add(t);
            t = new TerrTermin("СЕЛЬСКИЙ СОВЕТ") { CanonicText = "СЕЛЬСКИЙ ОКРУГ", IsSovet = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddVariant("СЕЛЬСОВЕТ", false);
            t.AddAbridge("С.С.");
            t.AddAbridge("С/С");
            t.AddVariant("СЕЛЬСКАЯ АДМИНИСТРАЦИЯ", false);
            t.AddAbridge("С.А.");
            t.AddAbridge("С.АДМ.");
            m_TerrOntology.Add(t);
            t = new TerrTermin("ПОСЕЛКОВЫЙ ОКРУГ") { IsRegion = true };
            t.AddVariant("ПОСЕЛКОВАЯ АДМИНИСТРАЦИЯ", false);
            t.AddAbridge("П.А.");
            t.AddAbridge("П.АДМ.");
            t.AddAbridge("П/А");
            m_TerrOntology.Add(t);
            t = new TerrTermin("ПОСЕЛКОВЫЙ СОВЕТ") { CanonicText = "ПОСЕЛКОВЫЙ ОКРУГ", IsSovet = true, Gender = Pullenti.Morph.MorphGender.Masculine };
            t.AddAbridge("П.С.");
            m_TerrOntology.Add(t);
            m_TerrOntology.Add(new TerrTermin("АВТОНОМНЫЙ") { IsRegion = true, IsAdjective = true });
            m_TerrOntology.Add(new TerrTermin("АВТОНОМНИЙ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, IsAdjective = true });
            m_TerrOntology.Add(new TerrTermin("МУНИЦИПАЛЬНОЕ СОБРАНИЕ") { IsRegion = true, IsSpecificPrefix = true, IsAlwaysPrefix = true });
            m_TerrOntology.Add(new TerrTermin("МУНІЦИПАЛЬНЕ ЗБОРИ", Pullenti.Morph.MorphLang.UA) { IsRegion = true, IsSpecificPrefix = true, IsAlwaysPrefix = true });
            t = new TerrTermin("МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ") { Acronym = "МО", Gender = Pullenti.Morph.MorphGender.Neuter };
            m_TerrOntology.Add(t);
            t = new TerrTermin("МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ МУНИЦИПАЛЬНЫЙ РАЙОН") { Acronym = "МОМР", IsRegion = true };
            t.AddVariant("МО МР", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("МУНИЦИПАЛЬНЫЙ ОКРУГ ГОРОДСКОЙ ОКРУГ") { CanonicText = "МУНИЦИПАЛЬНЫЙ ОКРУГ", Acronym = "МОГО", IsRegion = true };
            t.AddVariant("МУНИЦИПАЛЬНОЕ ОБРАЗОВАНИЕ ГОРОДСКОЙ ОКРУГ", false);
            t.AddVariant("МО ГО", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЦЕНТРАЛЬНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ЦАО");
            t.AddVariant("ЦЕНТРАЛЬНЫЙ АО", false);
            t.AddVariant("ЦЕНТРАЛЬНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("СЕВЕРНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("САО");
            t.AddVariant("СЕВЕРНЫЙ АО", false);
            t.AddVariant("СЕВЕРНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("СЕВЕРО-ВОСТОЧНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("СВАО");
            t.AddVariant("СЕВЕРО-ВОСТОЧНЫЙ АО", false);
            t.AddVariant("СЕВЕРО-ВОСТОЧНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ВОСТОЧНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ВАО");
            t.AddVariant("ВОСТОЧНЫЙ АО", false);
            t.AddVariant("ВОСТОЧНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЮГО-ВОСТОЧНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ЮВАО");
            t.AddVariant("ЮГО-ВОСТОЧНЫЙ АО", false);
            t.AddVariant("ЮГО-ВОСТОЧНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЮЖНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ЮАО");
            t.AddVariant("ЮЖНЫЙ АО", false);
            t.AddVariant("ЮЖНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЗАПАДНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ЗАО");
            t.AddVariant("ЗАПАДНЫЙ АО", false);
            t.AddVariant("ЗАПАДНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("СЕВЕРО-ЗАПАДНЫЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("СЗАО");
            t.AddVariant("СЕВЕРО-ЗАПАДНЫЙ АО", false);
            t.AddVariant("СЕВЕРО-ЗАПАДНЫЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ЗЕЛЕНОГРАДСКИЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ЗЕЛАО");
            t.AddVariant("ЗЕЛЕНОГРАДСКИЙ АО", false);
            t.AddVariant("ЗЕЛЕНОГРАДСКИЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ТРОИЦКИЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ТАО");
            t.AddVariant("ТРОИЦКИЙ АО", false);
            t.AddVariant("ТРОИЦКИЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("НОВОМОСКОВСКИЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("НАО");
            t.AddVariant("НОВОМОСКОВСКИЙ АО", false);
            t.AddVariant("НОВОМОСКОВСКИЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            t = new TerrTermin("ТРОИЦКИЙ И НОВОМОСКОВСКИЙ АДМИНИСТРАТИВНЫЙ ОКРУГ") { IsMoscowRegion = true };
            t.AddAbridge("ТИНАО");
            t.AddAbridge("НИТАО");
            t.AddVariant("ТРОИЦКИЙ И НОВОМОСКОВСКИЙ АО", false);
            t.AddVariant("ТРОИЦКИЙ И НОВОМОСКОВСКИЙ ОКРУГ", false);
            m_TerrOntology.Add(t);
            m_SpecNames = new Pullenti.Ner.Core.TerminCollection();
            foreach (string s in new string[] {"МАРЬИНА РОЩА", "ПРОСПЕКТ ВЕРНАДСКОГО"}) 
            {
                m_SpecNames.Add(new Pullenti.Ner.Core.Termin(s));
            }
            m_Alpha2State = new Dictionary<string, Pullenti.Ner.Core.IntOntologyItem>();
            byte[] dat = Pullenti.Ner.Address.Internal.ResourceHelper.GetBytes("t.dat");
            if (dat == null) 
                throw new Exception("Not found resource file t.dat in Analyzer.Location");
            dat = MiscLocationHelper.Deflate(dat);
            using (MemoryStream tmp = new MemoryStream(dat)) 
            {
                tmp.Position = 0;
                XmlDocument xml = new XmlDocument();
                xml.Load(tmp);
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    Pullenti.Morph.MorphLang lang = Pullenti.Morph.MorphLang.RU;
                    XmlAttribute a = x.Attributes["l"];
                    if (a != null) 
                    {
                        if (a.Value == "en") 
                            lang = Pullenti.Morph.MorphLang.EN;
                        else if (a.Value == "ua") 
                            lang = Pullenti.Morph.MorphLang.UA;
                    }
                    if (x.Name == "state") 
                        LoadState(x, lang);
                    else if (x.Name == "reg") 
                        LoadRegion(x, lang);
                    else if (x.Name == "unknown") 
                    {
                        a = x.Attributes["name"];
                        if (a != null && a.Value != null) 
                            m_UnknownRegions.Add(new Pullenti.Ner.Core.Termin(a.Value) { Lang = lang });
                    }
                }
            }
        }
        internal static Pullenti.Ner.Core.IntOntologyCollection m_TerrOntology;
        internal static Pullenti.Ner.Core.TerminCollection m_GeoAbbrs;
        static Pullenti.Ner.Core.IntOntologyItem m_RussiaRU;
        static Pullenti.Ner.Core.IntOntologyItem m_RussiaUA;
        static Pullenti.Ner.Core.IntOntologyItem m_MosRegRU;
        static Pullenti.Ner.Core.IntOntologyItem m_LenRegRU;
        static Pullenti.Ner.Core.IntOntologyItem m_Belorussia;
        static Pullenti.Ner.Core.IntOntologyItem m_Kazahstan;
        static Pullenti.Ner.Core.IntOntologyItem m_TamogSous;
        static Pullenti.Ner.Core.IntOntologyItem m_Tatarstan;
        static Pullenti.Ner.Core.IntOntologyItem m_Udmurtia;
        static Pullenti.Ner.Core.IntOntologyItem m_Dagestan;
        internal static Pullenti.Ner.Core.TerminCollection m_TerrAdjs;
        internal static Pullenti.Ner.Core.TerminCollection m_MansByState;
        internal static Pullenti.Ner.Core.TerminCollection m_UnknownRegions;
        internal static Pullenti.Ner.Core.TerminCollection m_TerrNounAdjectives;
        internal static Pullenti.Ner.Core.TerminCollection m_CapitalsByState;
        internal static Dictionary<string, Pullenti.Ner.Core.IntOntologyItem> m_Alpha2State;
        static Pullenti.Ner.Core.TerminCollection m_SpecNames;
        internal static List<Pullenti.Ner.Referent> m_AllStates = new List<Pullenti.Ner.Referent>();
        static void LoadState(XmlNode xml, Pullenti.Morph.MorphLang lang)
        {
            Pullenti.Ner.Geo.GeoReferent state = new Pullenti.Ner.Geo.GeoReferent();
            Pullenti.Ner.Core.IntOntologyItem c = new Pullenti.Ner.Core.IntOntologyItem(state);
            List<string> acrs = null;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.Name == "n") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, null);
                    c.Termins.Add(te);
                    state.AddName(x.InnerText);
                }
                else if (x.Name == "acr") 
                {
                    c.Termins.Add(new Pullenti.Ner.Core.Termin() { Acronym = x.InnerText, Lang = lang });
                    state.AddName(x.InnerText);
                    if (acrs == null) 
                        acrs = new List<string>();
                    acrs.Add(x.InnerText);
                }
                else if (x.Name == "a") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, lang);
                    te.Tag = c;
                    c.Termins.Add(te);
                    m_TerrAdjs.Add(te);
                }
                else if (x.Name == "a2") 
                    state.Alpha2 = x.InnerText;
                else if (x.Name == "m") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, lang);
                    te.Tag = state;
                    te.Gender = Pullenti.Morph.MorphGender.Masculine;
                    m_MansByState.Add(te);
                }
                else if (x.Name == "w") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, lang);
                    te.Tag = state;
                    te.Gender = Pullenti.Morph.MorphGender.Feminie;
                    m_MansByState.Add(te);
                }
                else if (x.Name == "cap") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, lang);
                    te.Tag = state;
                    m_CapitalsByState.Add(te);
                }
            }
            c.SetShortestCanonicalText(true);
            if (c.CanonicText == "ГОЛЛАНДИЯ" || c.CanonicText.StartsWith("КОРОЛЕВСТВО НИДЕР")) 
                c.CanonicText = "НИДЕРЛАНДЫ";
            else if (c.CanonicText == "ГОЛЛАНДІЯ" || c.CanonicText.StartsWith("КОРОЛІВСТВО НІДЕР")) 
                c.CanonicText = "НІДЕРЛАНДИ";
            if (state.Alpha2 == "RU") 
            {
                if (lang.IsUa) 
                    m_RussiaUA = c;
                else 
                    m_RussiaRU = c;
            }
            else if (state.Alpha2 == "BY") 
            {
                if (!lang.IsUa) 
                    m_Belorussia = c;
            }
            else if (state.Alpha2 == "KZ") 
            {
                if (!lang.IsUa) 
                    m_Kazahstan = c;
            }
            else if (c.CanonicText == "ТАМОЖЕННЫЙ СОЮЗ") 
            {
                if (!lang.IsUa) 
                    m_TamogSous = c;
            }
            if (state.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, null, true) == null) 
            {
                if (lang.IsUa) 
                    state.AddTypState(lang);
                else 
                {
                    state.AddTypState(Pullenti.Morph.MorphLang.RU);
                    state.AddTypState(Pullenti.Morph.MorphLang.EN);
                }
            }
            m_TerrOntology.AddItem(c);
            if (lang.IsRu) 
                m_AllStates.Add(state);
            string a2 = state.Alpha2;
            if (a2 != null) 
            {
                if (!m_Alpha2State.ContainsKey(a2)) 
                    m_Alpha2State.Add(a2, c);
                string a3;
                if (MiscLocationHelper.m_Alpha2_3.TryGetValue(a2, out a3)) 
                {
                    if (!m_Alpha2State.ContainsKey(a3)) 
                        m_Alpha2State.Add(a3, c);
                }
            }
            if (acrs != null) 
            {
                foreach (string a in acrs) 
                {
                    if (!m_Alpha2State.ContainsKey(a)) 
                        m_Alpha2State.Add(a, c);
                }
            }
        }
        static void LoadRegion(XmlNode xml, Pullenti.Morph.MorphLang lang)
        {
            Pullenti.Ner.Geo.GeoReferent reg = new Pullenti.Ner.Geo.GeoReferent();
            Pullenti.Ner.Core.IntOntologyItem r = new Pullenti.Ner.Core.IntOntologyItem(reg);
            Pullenti.Ner.Core.Termin aTerm = null;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.Name == "n") 
                {
                    string v = x.InnerText;
                    if (v.StartsWith("ЦЕНТРАЛ")) 
                    {
                    }
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(v, lang);
                    if (lang.IsRu && m_MosRegRU == null && v == "ПОДМОСКОВЬЕ") 
                    {
                        m_MosRegRU = r;
                        te.AddAbridge("МОС.ОБЛ.");
                        te.AddAbridge("МОСК.ОБЛ.");
                        te.AddAbridge("МОСКОВ.ОБЛ.");
                        te.AddAbridge("МОС.ОБЛАСТЬ");
                        te.AddAbridge("МОСК.ОБЛАСТЬ");
                        te.AddAbridge("МОСКОВ.ОБЛАСТЬ");
                    }
                    else if (lang.IsRu && m_LenRegRU == null && v == "ЛЕНОБЛАСТЬ") 
                    {
                        te.Acronym = "ЛО";
                        te.AddAbridge("ЛЕН.ОБЛ.");
                        te.AddAbridge("ЛЕН.ОБЛАСТЬ");
                        m_LenRegRU = r;
                    }
                    r.Termins.Add(te);
                    reg.AddName(v);
                }
                else if (x.Name == "t") 
                    reg.AddTyp(x.InnerText);
                else if (x.Name == "a") 
                {
                    Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin();
                    te.InitByNormalText(x.InnerText, lang);
                    te.Tag = r;
                    r.Termins.Add(te);
                }
                else if (x.Name == "ab") 
                {
                    if (aTerm == null) 
                        aTerm = new Pullenti.Ner.Core.Termin(reg.GetStringValue(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME), lang) { Tag = reg };
                    aTerm.AddAbridge(x.InnerText);
                }
            }
            if (aTerm != null) 
                m_GeoAbbrs.Add(aTerm);
            r.SetShortestCanonicalText(true);
            if (r.CanonicText.StartsWith("КАРАЧАЕВО")) 
                r.CanonicText = "КАРАЧАЕВО - ЧЕРКЕССИЯ";
            if (r.CanonicText.Contains("ТАТАРСТАН")) 
                m_Tatarstan = r;
            else if (r.CanonicText.Contains("УДМУРТ")) 
                m_Udmurtia = r;
            else if (r.CanonicText.Contains("ДАГЕСТАН")) 
                m_Dagestan = r;
            if (reg.IsState && reg.IsRegion) 
                reg.AddTypReg(lang);
            m_TerrOntology.AddItem(r);
        }
        public static Pullenti.Ner.Core.IntOntologyToken CheckOntoItem(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> li = m_TerrOntology.TryAttach(t, null, false);
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
        public static Pullenti.Ner.MetaToken CheckKeyword(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            List<Pullenti.Ner.Core.IntOntologyToken> li = m_TerrOntology.TryAttach(t, null, false);
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
    }
}