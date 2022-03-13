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

namespace Pullenti.Ner.Geo.Internal
{
    public class OrgItemToken : Pullenti.Ner.ReferentToken
    {
        public OrgItemToken(Pullenti.Ner.Referent r, Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(r, b, e, null)
        {
        }
        public bool IsDoubt;
        public bool HasTerrKeyword;
        public bool KeywordAfter;
        public bool IsGsk;
        public void SetGsk()
        {
            IsGsk = false;
            foreach (Pullenti.Ner.Slot s in Referent.Slots) 
            {
                if (s.TypeName == "TYPE" && (s.Value is string)) 
                {
                    string ty = s.Value as string;
                    if (((((ty.Contains("товарищество") || ty.Contains("кооператив") || ty.Contains("коллектив")) || Pullenti.Morph.LanguageHelper.EndsWithEx(ty, "поселок", " отдыха", " часть", "хозяйство") || ty.Contains("партнерство")) || ty.Contains("объединение") || ty.Contains("бизнес")) || ((ty.Contains("станция") && !ty.Contains("заправоч"))) || ty.Contains("аэропорт")) || ty.Contains("пансионат") || ty.Contains("санаторий")) 
                    {
                        IsGsk = true;
                        return;
                    }
                    if (ty == "АОЗТ") 
                    {
                        IsGsk = true;
                        return;
                    }
                }
                else if (s.TypeName == "NAME" && (s.Value is string)) 
                {
                    string nam = s.Value as string;
                    if (Pullenti.Morph.LanguageHelper.EndsWithEx(nam, "ГЭС", "АЭС", "ТЭС", null)) 
                    {
                        IsGsk = true;
                        return;
                    }
                }
            }
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (IsDoubt) 
                tmp.Append("? ");
            if (HasTerrKeyword) 
                tmp.Append("Terr ");
            if (IsGsk) 
                tmp.Append("Gsk ");
            tmp.Append(Referent.ToString());
            return tmp.ToString();
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.ORegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                OrgItemToken org = TryParse(t, ad);
                if (org != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.Org = org;
                    if (org.HasTerrKeyword || ((org.IsGsk && !org.KeywordAfter))) 
                    {
                        for (Pullenti.Ner.Token tt = org.BeginToken; tt != null && tt.EndChar <= org.EndChar; tt = tt.Next) 
                        {
                            GeoTokenData dd = tt.Tag as GeoTokenData;
                            if (dd == null) 
                                dd = new GeoTokenData(tt);
                            dd.NoGeo = true;
                        }
                        if (!org.HasTerrKeyword) 
                            t = org.EndToken;
                    }
                }
            }
            ad.ORegime = true;
        }
        public static OrgItemToken TryParse(Pullenti.Ner.Token t, GeoAnalyzerData ad = null)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (SpeedRegime && ((ad.ORegime || ad.AllRegime))) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d != null) 
                    return d.Org;
                return null;
            }
            if (ad.OLevel > 1) 
                return null;
            ad.OLevel++;
            OrgItemToken res = _TryParse(t, false, 0, ad);
            ad.OLevel--;
            return res;
        }
        static OrgItemToken _TryParse(Pullenti.Ner.Token t, bool afterTerr, int lev, GeoAnalyzerData ad)
        {
            if (lev > 3 || t == null || t.IsComma) 
                return null;
            Pullenti.Ner.Token tt2 = MiscLocationHelper.CheckTerritory(t);
            if (tt2 != null && tt2.Next != null) 
            {
                tt2 = tt2.Next;
                bool br = false;
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt2, true)) 
                {
                    br = true;
                    tt2 = tt2.Next;
                }
                if (tt2 == null || lev > 3) 
                    return null;
                OrgItemToken re2 = _TryParse(tt2, true, lev + 1, ad);
                if (re2 != null) 
                {
                    Pullenti.Ner.Analyzer a = t.Kit.Processor.FindAnalyzer("GEO");
                    if (a != null) 
                    {
                        Pullenti.Ner.ReferentToken rt = a.ProcessReferent(tt2, null);
                        if (rt != null) 
                            return null;
                    }
                    re2.BeginToken = t;
                    if (br && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(re2.EndToken.Next, false, null, false)) 
                        re2.EndToken = re2.EndToken.Next;
                    re2.HasTerrKeyword = true;
                    return re2;
                }
                else if ((t is Pullenti.Ner.TextToken) && (t as Pullenti.Ner.TextToken).Term.StartsWith("ТЕР") && (tt2.WhitespacesBeforeCount < 3)) 
                {
                    NameToken nam1 = NameToken.TryParse(tt2, NameTokenType.Org, 0);
                    if (nam1 != null && nam1.Name != null) 
                    {
                        if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(tt2)) 
                            return null;
                        if (t.Next != nam1.EndToken && Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(nam1.EndToken)) 
                            return null;
                        if (TerrItemToken.CheckKeyword(tt2) != null) 
                            return null;
                        if (t.Next != nam1.EndToken && TerrItemToken.CheckKeyword(nam1.EndToken) != null) 
                            return null;
                        Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(tt2);
                        if (ter != null) 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = ter.Item.Referent as Pullenti.Ner.Geo.GeoReferent;
                            if (geo.IsCity || geo.IsState) 
                                return null;
                        }
                        if (CityItemToken.CheckKeyword(tt2) != null) 
                            return null;
                        if (CityItemToken.CheckOntoItem(tt2) != null) 
                            return null;
                        Pullenti.Ner.Token tt = nam1.EndToken;
                        bool ok = false;
                        if (tt.IsNewlineAfter) 
                            ok = true;
                        else if (tt.Next != null && tt.Next.IsComma) 
                            ok = true;
                        else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt2, false, false)) 
                            ok = true;
                        else 
                        {
                            Pullenti.Ner.Address.Internal.AddressItemToken a2 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(nam1.EndToken.Next, false, null, ad);
                            if (a2 != null) 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken a1 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt2, false, null, ad);
                                if (a1 == null || (a1.EndChar < a2.EndChar)) 
                                    ok = true;
                            }
                        }
                        if (ok) 
                        {
                            Pullenti.Ner.Referent org1 = t.Kit.CreateReferent("ORGANIZATION");
                            org1.AddSlot("NAME", nam1.Name, false, 0);
                            if (nam1.Number != null) 
                                org1.AddSlot("NUMBER", nam1.Number, false, 0);
                            OrgItemToken res1 = new OrgItemToken(org1, t, nam1.EndToken);
                            res1.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
                            res1.HasTerrKeyword = true;
                            return res1;
                        }
                    }
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("NAMEDENTITY", tt2, null);
                    if (rt != null) 
                    {
                        OrgItemToken res1 = new OrgItemToken(rt.Referent, t, rt.EndToken);
                        res1.Data = t.Kit.GetAnalyzerDataByAnalyzerName("NAMEDENTITY");
                        res1.HasTerrKeyword = true;
                        return res1;
                    }
                }
                if (!t.IsValue("САД", null)) 
                    return null;
            }
            bool typAfter = false;
            bool doubt0 = false;
            OrgTypToken tokTyp = OrgTypToken.TryParse(t, afterTerr, ad);
            NameToken nam = null;
            if (tokTyp == null) 
            {
                int ok = 0;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                    ok = 2;
                else if (t.IsValue("ИМ", null)) 
                    ok = 2;
                else if ((t is Pullenti.Ner.TextToken) && !t.Chars.IsAllLower && t.LengthChar > 1) 
                    ok = 1;
                else if (afterTerr) 
                    ok = 1;
                if (ok == 0) 
                    return null;
                if ((t.LengthChar > 5 && (t is Pullenti.Ner.TextToken) && !t.Chars.IsAllUpper) && !t.Chars.IsAllLower && !t.Chars.IsCapitalUpper) 
                {
                    string namm = (t as Pullenti.Ner.TextToken).GetSourceText();
                    if (char.IsUpper(namm[0]) && char.IsUpper(namm[1])) 
                    {
                        for (int i = 0; i < namm.Length; i++) 
                        {
                            if (char.IsLower(namm[i]) && i > 2) 
                            {
                                string abbr = namm.Substring(0, i - 1);
                                Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(abbr) { Acronym = abbr };
                                List<Pullenti.Ner.Core.Termin> li = OrgTypToken.FindTerminByAcronym(abbr);
                                if (li != null && li.Count > 0) 
                                {
                                    nam = new NameToken(t, t);
                                    nam.Name = (t as Pullenti.Ner.TextToken).Term.Substring(i - 1);
                                    tokTyp = new OrgTypToken(t, t);
                                    tokTyp.Vals.Add(li[0].CanonicText.ToLower());
                                    tokTyp.Vals.Add(abbr);
                                    nam.TryAttachNumber();
                                    break;
                                }
                            }
                        }
                    }
                }
                if (nam == null) 
                {
                    if (afterTerr) 
                        ok = 2;
                    if (ok < 2) 
                    {
                        int kk = 0;
                        for (Pullenti.Ner.Token tt = t.Next; tt != null && (kk < 5); tt = tt.Next,kk++) 
                        {
                            OrgTypToken ty22 = OrgTypToken.TryParse(tt, false, ad);
                            if (ty22 == null || ty22.IsDoubt) 
                                continue;
                            ok = 2;
                            break;
                        }
                    }
                    if (ok < 2) 
                        return null;
                    typAfter = true;
                    nam = NameToken.TryParse(t, NameTokenType.Org, 0);
                    if (nam == null) 
                        return null;
                    tokTyp = OrgTypToken.TryParse(nam.EndToken.Next, afterTerr, ad);
                    if (nam.Name == null) 
                    {
                        if (nam.Number != null && tokTyp != null) 
                        {
                        }
                        else 
                            return null;
                    }
                    if (tokTyp != null) 
                    {
                        if (nam.BeginToken == nam.EndToken) 
                        {
                            Pullenti.Morph.MorphClass mc = nam.GetMorphClassInDictionary();
                            if (mc.IsConjunction || mc.IsPreposition || mc.IsPronoun) 
                                return null;
                        }
                        NameToken nam2 = NameToken.TryParse(tokTyp.EndToken.Next, NameTokenType.Org, 0);
                        OrgItemToken rt2 = TryParse(tokTyp.BeginToken, null);
                        if (rt2 != null && rt2.EndChar > tokTyp.EndChar) 
                        {
                            if ((nam.Number == null && nam2 != null && nam2.Name == null) && nam2.Number != null && nam2.EndToken == rt2.EndToken) 
                            {
                                nam.Number = nam2.Number;
                                tokTyp = tokTyp.Clone();
                                tokTyp.EndToken = nam2.EndToken;
                            }
                            else 
                                return null;
                        }
                        else if ((nam.Number == null && nam2 != null && nam2.Name == null) && nam2.Number != null) 
                        {
                            nam.Number = nam2.Number;
                            tokTyp = tokTyp.Clone();
                            tokTyp.EndToken = nam2.EndToken;
                        }
                        nam.EndToken = tokTyp.EndToken;
                        doubt0 = true;
                    }
                    else 
                    {
                        if (nam.Name.EndsWith("ПЛАЗА") || nam.Name.StartsWith("БИЗНЕС")) 
                        {
                        }
                        else if (nam.BeginToken == nam.EndToken) 
                            return null;
                        else if ((((tokTyp = OrgTypToken.TryParse(nam.EndToken, false, ad)))) == null) 
                            return null;
                        else if (nam.Morph.Case.IsGenitive && !nam.Morph.Case.IsNominative) 
                            nam.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nam, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominativeSingle).Replace("-", " ");
                        if (tokTyp == null) 
                        {
                            tokTyp = new OrgTypToken(t, t);
                            tokTyp.Vals.Add("бизнес центр");
                            tokTyp.Vals.Add("БЦ");
                        }
                        nam.IsDoubt = false;
                    }
                }
            }
            else 
            {
                if (tokTyp.WhitespacesAfterCount > 3) 
                    return null;
                Pullenti.Ner.Token tt3 = MiscLocationHelper.CheckTerritory(tokTyp.EndToken.Next);
                if (tt3 != null) 
                {
                    tokTyp = tokTyp.Clone();
                    tokTyp.EndToken = tt3;
                    afterTerr = true;
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(tokTyp.EndToken.Next, true, ad);
                    if (tokTyp2 != null && !tokTyp2.IsDoubt) 
                        tokTyp.MergeWith(tokTyp2);
                }
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(tokTyp.EndToken.Next, true, false)) 
                {
                    OrgTypToken tokTyp2 = OrgTypToken.TryParse(tokTyp.EndToken.Next.Next, afterTerr, ad);
                    if (tokTyp2 != null && !tokTyp2.IsDoubt) 
                    {
                        tokTyp = tokTyp.Clone();
                        tokTyp.IsDoubt = false;
                        nam = NameToken.TryParse(tokTyp2.EndToken.Next, NameTokenType.Org, 0);
                        if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken.Next, false, null, false)) 
                        {
                            tokTyp.MergeWith(tokTyp2);
                            nam.EndToken = nam.EndToken.Next;
                        }
                        else if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken, false, null, false)) 
                            tokTyp.MergeWith(tokTyp2);
                        else 
                            nam = null;
                    }
                }
            }
            if (nam == null) 
                nam = NameToken.TryParse(tokTyp.EndToken.Next, NameTokenType.Org, 0);
            if (nam == null) 
                return null;
            if (tokTyp.IsDoubt && nam.IsDoubt) 
                return null;
            if ((tokTyp.LengthChar < 3) && nam.Name == null && nam.Pref == null) 
                return null;
            Pullenti.Ner.Referent org = t.Kit.CreateReferent("ORGANIZATION");
            OrgItemToken res = new OrgItemToken(org, t, nam.EndToken);
            res.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
            res.HasTerrKeyword = afterTerr;
            res.IsDoubt = doubt0 || tokTyp.IsDoubt;
            res.KeywordAfter = typAfter;
            foreach (string ty in tokTyp.Vals) 
            {
                org.AddSlot("TYPE", ty, false, 0);
            }
            bool ignoreNext = false;
            if ((res.WhitespacesAfterCount < 3) && res.EndToken.Next != null && res.EndToken.Next.IsValue("ТЕРРИТОРИЯ", null)) 
            {
                if (_TryParse(res.EndToken.Next.Next, true, lev + 1, ad) == null) 
                {
                    res.EndToken = res.EndToken.Next;
                    ignoreNext = true;
                }
            }
            if (res.WhitespacesAfterCount < 3) 
            {
                Pullenti.Ner.Token tt = res.EndToken.Next;
                OrgItemToken next = _TryParse(tt, false, lev + 1, ad);
                if (next != null) 
                {
                    if (next.IsGsk) 
                        next = null;
                    else 
                        res.EndToken = next.EndToken;
                    ignoreNext = true;
                }
                else 
                {
                    if (tt != null && tt.IsValue("ПРИ", null)) 
                        tt = tt.Next;
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", tt, null);
                    if (rt != null) 
                    {
                    }
                    if (rt != null) 
                    {
                        res.EndToken = rt.EndToken;
                        Pullenti.Ner.Core.IntOntologyToken ter = TerrItemToken.CheckOntoItem(res.EndToken.Next);
                        if (ter != null) 
                            res.EndToken = ter.EndToken;
                        ignoreNext = true;
                    }
                }
            }
            string suffName = null;
            if (!ignoreNext && (res.WhitespacesAfterCount < 2)) 
            {
                OrgTypToken tokTyp2 = OrgTypToken.TryParse(res.EndToken.Next, true, ad);
                if (tokTyp2 != null) 
                {
                    res.EndToken = tokTyp2.EndToken;
                    if (tokTyp2.IsDoubt && nam.Name != null) 
                        suffName = tokTyp2.Vals[0];
                    else 
                        foreach (string ty in tokTyp2.Vals) 
                        {
                            org.AddSlot("TYPE", ty, false, 0);
                        }
                    if (nam.Number == null) 
                    {
                        NameToken nam2 = NameToken.TryParse(res.EndToken.Next, NameTokenType.Org, 0);
                        if ((nam2 != null && nam2.Number != null && nam2.Name == null) && nam2.Pref == null) 
                        {
                            nam.Number = nam2.Number;
                            res.EndToken = nam2.EndToken;
                        }
                    }
                }
            }
            if (nam.Name != null) 
            {
                if (nam.Pref != null) 
                {
                    org.AddSlot("NAME", string.Format("{0} {1}", nam.Pref, nam.Name), false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1} {2}", nam.Pref, nam.Name, suffName), false, 0);
                }
                else 
                {
                    org.AddSlot("NAME", nam.Name, false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1}", nam.Name, suffName), false, 0);
                }
            }
            else if (nam.Pref != null) 
                org.AddSlot("NAME", nam.Pref, false, 0);
            else if (nam.Number != null && (res.WhitespacesAfterCount < 2)) 
            {
                NameToken nam2 = NameToken.TryParse(res.EndToken.Next, NameTokenType.Org, 0);
                if (nam2 != null && nam2.Name != null && nam2.Number == null) 
                {
                    res.EndToken = nam2.EndToken;
                    org.AddSlot("NAME", nam2.Name, false, 0);
                }
            }
            if (nam.Number != null) 
                org.AddSlot("NUMBER", nam.Number, false, 0);
            bool ok1 = false;
            int cou = 0;
            for (Pullenti.Ner.Token tt = res.BeginToken; tt != null && tt.EndChar <= res.EndChar; tt = tt.Next) 
            {
                if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar > 1) 
                {
                    if (nam != null && tt.BeginChar >= nam.BeginChar && tt.EndChar <= nam.EndChar) 
                    {
                        if (tokTyp != null && tt.BeginChar >= tokTyp.BeginChar && tt.EndChar <= tokTyp.EndChar) 
                        {
                        }
                        else 
                            cou++;
                    }
                    if (!tt.Chars.IsAllLower) 
                        ok1 = true;
                }
                else if (tt is Pullenti.Ner.ReferentToken) 
                    ok1 = true;
            }
            res.SetGsk();
            if (!ok1) 
            {
                if (!res.IsGsk && !res.HasTerrKeyword) 
                    return null;
            }
            if (cou > 4) 
                return null;
            return res;
        }
        public static Pullenti.Ner.Address.Internal.StreetItemToken TryParseRailway(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken) || !t.Chars.IsLetter) 
                return null;
            if (t.IsValue("ДОРОГА", null) && (t.WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken next = TryParseRailway(t.Next);
                if (next != null) 
                {
                    next.BeginToken = t;
                    return next;
                }
            }
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.OLevel > 0) 
                return null;
            ad.OLevel++;
            Pullenti.Ner.Address.Internal.StreetItemToken res = _tryParseRailway(t);
            ad.OLevel--;
            return res;
        }
        static Pullenti.Ner.ReferentToken _tryParseRailwayOrg(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            int cou = 0;
            bool ok = false;
            for (Pullenti.Ner.Token tt = t; tt != null && (cou < 4); tt = tt.Next,cou++) 
            {
                if (tt is Pullenti.Ner.TextToken) 
                {
                    string val = (tt as Pullenti.Ner.TextToken).Term;
                    if (val == "Ж" || val.StartsWith("ЖЕЛЕЗ")) 
                    {
                        ok = true;
                        break;
                    }
                    if (Pullenti.Morph.LanguageHelper.EndsWith(val, "ЖД")) 
                    {
                        ok = true;
                        break;
                    }
                }
            }
            if (!ok) 
                return null;
            Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", t, null);
            if (rt == null) 
                return null;
            foreach (string ty in rt.Referent.GetStringValues("TYPE")) 
            {
                if (ty.EndsWith("дорога")) 
                    return rt;
            }
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRailway(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
            if (rt0 != null) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(t, rt0.EndToken) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true };
                res.Value = rt0.Referent.GetStringValue("NAME");
                t = res.EndToken.Next;
                if (t != null && t.IsComma) 
                    t = t.Next;
                Pullenti.Ner.Address.Internal.StreetItemToken next = _tryParseRzdDir(t);
                if (next != null) 
                {
                    res.EndToken = next.EndToken;
                    res.Value = string.Format("{0} {1}", res.Value, next.Value);
                }
                else if ((t is Pullenti.Ner.TextToken) && t.Morph.Class.IsAdjective && !t.Chars.IsAllLower) 
                {
                    bool ok = false;
                    if (t.IsNewlineAfter || t.Next == null) 
                        ok = true;
                    else if (t.Next.IsCharOf(".,")) 
                        ok = true;
                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t.Next, false, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckKmAfter(t.Next)) 
                        ok = true;
                    if (ok) 
                    {
                        res.Value = string.Format("{0} {1} НАПРАВЛЕНИЕ", res.Value, (t as Pullenti.Ner.TextToken).Term);
                        res.EndToken = t;
                    }
                }
                if (res.Value == "РОССИЙСКИЕ ЖЕЛЕЗНЫЕ ДОРОГИ") 
                    res.NounIsDoubtCoef = 2;
                return res;
            }
            Pullenti.Ner.Address.Internal.StreetItemToken dir = _tryParseRzdDir(t);
            if (dir != null && dir.NounIsDoubtCoef == 0) 
                return dir;
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRzdDir(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Token napr = null;
            Pullenti.Ner.Token tt0 = null;
            Pullenti.Ner.Token tt1 = null;
            string val = null;
            int cou = 0;
            for (Pullenti.Ner.Token tt = t; tt != null && (cou < 4); tt = tt.Next,cou++) 
            {
                if (tt.IsCharOf(",.")) 
                    continue;
                if (tt.IsNewlineBefore) 
                    break;
                if (tt.IsValue("НАПРАВЛЕНИЕ", null)) 
                {
                    napr = tt;
                    continue;
                }
                if (tt.IsValue("НАПР", null)) 
                {
                    if (tt.Next != null && tt.Next.IsChar('.')) 
                        tt = tt.Next;
                    napr = tt;
                    continue;
                }
                Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(tt);
                if (npt != null && npt.Adjectives.Count > 0 && npt.Noun.IsValue("КОЛЬЦО", null)) 
                {
                    tt0 = tt;
                    tt1 = npt.EndToken;
                    val = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false);
                    break;
                }
                if ((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && ((tt.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = (tt1 = tt);
                    continue;
                }
                if ((((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && tt.Next != null) && tt.Next.IsHiphen && (tt.Next.Next is Pullenti.Ner.TextToken)) && ((tt.Next.Next.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = tt;
                    tt = tt.Next.Next;
                    tt1 = tt;
                    continue;
                }
                break;
            }
            if (tt0 == null) 
                return null;
            Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(tt0, tt1) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true, NounIsDoubtCoef = 1 };
            if (val != null) 
                res.Value = val;
            else 
            {
                res.Value = tt1.GetNormalCaseText(Pullenti.Morph.MorphClass.Adjective, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Neuter, false);
                if (tt0 != tt1) 
                    res.Value = string.Format("{0} {1}", (tt0 as Pullenti.Ner.TextToken).Term, res.Value);
                res.Value += " НАПРАВЛЕНИЕ";
            }
            if (napr != null && napr.EndChar > res.EndChar) 
                res.EndToken = napr;
            t = res.EndToken.Next;
            if (t != null && t.IsComma) 
                t = t.Next;
            if (t != null) 
            {
                Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
                if (rt0 != null) 
                {
                    res.Value = string.Format("{0} {1}", rt0.Referent.GetStringValue("NAME"), res.Value);
                    res.EndToken = rt0.EndToken;
                    res.NounIsDoubtCoef = 0;
                }
            }
            return res;
        }
    }
}