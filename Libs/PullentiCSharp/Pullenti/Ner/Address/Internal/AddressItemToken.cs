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

namespace Pullenti.Ner.Address.Internal
{
    public class AddressItemToken : Pullenti.Ner.MetaToken
    {
        public AddressItemToken(AddressItemType typ, Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
            Typ = typ;
        }
        public AddressItemType Typ
        {
            get
            {
                return m_Typ;
            }
            set
            {
                m_Typ = value;
                if (value == AddressItemType.House) 
                {
                }
            }
        }
        AddressItemType m_Typ;
        public string Value;
        public Pullenti.Ner.Referent Referent;
        public Pullenti.Ner.ReferentToken RefToken;
        public bool RefTokenIsGsk;
        public bool IsDoubt;
        public Pullenti.Ner.Address.AddressDetailType DetailType = Pullenti.Ner.Address.AddressDetailType.Undefined;
        public Pullenti.Ner.Address.AddressBuildingType BuildingType = Pullenti.Ner.Address.AddressBuildingType.Undefined;
        public Pullenti.Ner.Address.AddressHouseType HouseType = Pullenti.Ner.Address.AddressHouseType.Undefined;
        public int DetailMeters = 0;
        public AddressItemToken Clone()
        {
            AddressItemToken res = new AddressItemToken(Typ, BeginToken, EndToken);
            res.Morph = Morph;
            res.Value = Value;
            res.Referent = Referent;
            res.RefToken = RefToken;
            res.RefTokenIsGsk = RefTokenIsGsk;
            res.IsDoubt = IsDoubt;
            res.DetailType = DetailType;
            res.BuildingType = BuildingType;
            res.HouseType = HouseType;
            res.DetailMeters = DetailMeters;
            return res;
        }
        public bool IsStreetRoad
        {
            get
            {
                if (Typ != AddressItemType.Street) 
                    return false;
                if (!(Referent is Pullenti.Ner.Address.StreetReferent)) 
                    return false;
                return (Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Road;
            }
        }
        public bool IsDigit
        {
            get
            {
                if (Value == "Б/Н") 
                    return true;
                if (string.IsNullOrEmpty(Value)) 
                    return false;
                if (char.IsDigit(Value[0])) 
                    return true;
                if (Value.Length > 1) 
                {
                    if (char.IsLetter(Value[0]) && char.IsDigit(Value[1])) 
                        return true;
                }
                if (Value.Length != 1 || !char.IsLetter(Value[0])) 
                    return false;
                if (!BeginToken.Chars.IsAllLower) 
                    return false;
                return true;
            }
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0} {1}", Typ.ToString(), Value ?? "");
            if (Referent != null) 
                res.AppendFormat(" <{0}>", Referent.ToString());
            if (DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined || DetailMeters > 0) 
                res.AppendFormat(" [{0}, {1}]", DetailType, DetailMeters);
            return res.ToString();
        }
        static AddressItemToken _findAddrTyp(Pullenti.Ner.Token t, int maxChar, int lev = 0)
        {
            if (t == null || t.EndChar > maxChar) 
                return null;
            if (lev > 5) 
                return null;
            if (t is Pullenti.Ner.ReferentToken) 
            {
                Pullenti.Ner.Geo.GeoReferent geo = t.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                if (geo != null) 
                {
                    foreach (Pullenti.Ner.Slot s in geo.Slots) 
                    {
                        if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE) 
                        {
                            string ty = (string)s.Value;
                            if (ty.Contains("район")) 
                                return null;
                        }
                    }
                }
                for (Pullenti.Ner.Token tt = (t as Pullenti.Ner.ReferentToken).BeginToken; tt != null && tt.EndChar <= t.EndChar; tt = tt.Next) 
                {
                    if (tt.EndChar > maxChar) 
                        break;
                    AddressItemToken ty = _findAddrTyp(tt, maxChar, lev + 1);
                    if (ty != null) 
                        return ty;
                }
            }
            else 
            {
                AddressItemToken ai = _tryAttachDetail(t, null);
                if (ai != null) 
                {
                    if (ai.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined || ai.DetailMeters > 0) 
                        return ai;
                }
            }
            return null;
        }
        public static AddressItemToken TryParse(Pullenti.Ner.Token t, bool prefixBefore = false, AddressItemToken prev = null, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.ALevel > 1) 
                return null;
            ad.ALevel++;
            AddressItemToken res = _TryParse(t, prefixBefore, prev, ad);
            ad.ALevel--;
            if (((res != null && !res.IsWhitespaceAfter && res.EndToken.Next != null) && res.EndToken.Next.IsHiphen && !res.EndToken.Next.IsWhitespaceAfter) && res.Value != null) 
            {
                if (res.Typ == AddressItemType.House || res.Typ == AddressItemType.Building || res.Typ == AddressItemType.Corpus) 
                {
                    Pullenti.Ner.Token tt = res.EndToken.Next.Next;
                    if (tt is Pullenti.Ner.NumberToken) 
                    {
                        res.Value = string.Format("{0}-{1}", res.Value, (tt as Pullenti.Ner.NumberToken).Value);
                        res.EndToken = tt;
                        if ((!tt.IsWhitespaceAfter && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.LengthChar == 1) && tt.Next.Chars.IsAllUpper) 
                        {
                            tt = tt.Next;
                            res.EndToken = tt;
                            res.Value += (tt as Pullenti.Ner.TextToken).Term;
                        }
                        if ((!tt.IsWhitespaceAfter && tt.Next != null && tt.Next.IsCharOf("\\/")) && (tt.Next.Next is Pullenti.Ner.NumberToken)) 
                        {
                            res.EndToken = (tt = tt.Next.Next);
                            res.Value = string.Format("{0}/{1}", res.Value, (tt as Pullenti.Ner.NumberToken).Value);
                        }
                        if ((!tt.IsWhitespaceAfter && tt.Next != null && tt.Next.IsHiphen) && (tt.Next.Next is Pullenti.Ner.NumberToken)) 
                        {
                            res.EndToken = (tt = tt.Next.Next);
                            res.Value = string.Format("{0}-{1}", res.Value, (tt as Pullenti.Ner.NumberToken).Value);
                            if ((!tt.IsWhitespaceAfter && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.LengthChar == 1) && tt.Next.Chars.IsAllUpper) 
                            {
                                tt = tt.Next;
                                res.EndToken = tt;
                                res.Value += (tt as Pullenti.Ner.TextToken).Term;
                            }
                        }
                    }
                    else if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && tt.Chars.IsAllUpper) 
                    {
                        res.Value = string.Format("{0}-{1}", res.Value, (tt as Pullenti.Ner.TextToken).Term);
                        res.EndToken = tt;
                    }
                }
            }
            return res;
        }
        static AddressItemToken _TryParse(Pullenti.Ner.Token t, bool prefixBefore, AddressItemToken prev, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad)
        {
            if (t == null) 
                return null;
            if (t is Pullenti.Ner.ReferentToken) 
            {
                Pullenti.Ner.ReferentToken rt = t as Pullenti.Ner.ReferentToken;
                AddressItemType ty;
                Pullenti.Ner.Geo.GeoReferent geo = rt.Referent as Pullenti.Ner.Geo.GeoReferent;
                if (geo != null) 
                {
                    if (geo.IsCity) 
                        ty = AddressItemType.City;
                    else if (geo.IsState) 
                        ty = AddressItemType.Country;
                    else 
                        ty = AddressItemType.Region;
                    AddressItemToken res = new AddressItemToken(ty, t, t) { Referent = rt.Referent };
                    if (ty != AddressItemType.City) 
                        return res;
                    for (Pullenti.Ner.Token tt = (t as Pullenti.Ner.ReferentToken).BeginToken; tt != null && tt.EndChar <= t.EndChar; tt = tt.Next) 
                    {
                        if (tt is Pullenti.Ner.ReferentToken) 
                        {
                            if (tt.GetReferent() == geo) 
                            {
                                AddressItemToken res1 = _TryParse(tt, false, prev, ad);
                                if (res1 != null && ((res1.DetailMeters > 0 || res1.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined))) 
                                {
                                    res1.BeginToken = (res1.EndToken = t);
                                    return res1;
                                }
                            }
                            continue;
                        }
                        AddressItemToken det = _tryParsePureItem(tt, false, null);
                        if (det != null) 
                        {
                            if (det.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && res.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                                res.DetailType = det.DetailType;
                            if (det.DetailMeters > 0) 
                                res.DetailMeters = det.DetailMeters;
                        }
                    }
                    return res;
                }
            }
            if (prev != null) 
            {
                if (t.IsValue("КВ", null) || t.IsValue("КВАРТ", null)) 
                {
                    if ((((prev.Typ == AddressItemType.House || prev.Typ == AddressItemType.Number || prev.Typ == AddressItemType.Building) || prev.Typ == AddressItemType.Floor || prev.Typ == AddressItemType.Potch) || prev.Typ == AddressItemType.Corpus || prev.Typ == AddressItemType.CorpusOrFlat) || prev.Typ == AddressItemType.Detail) 
                        return TryParsePureItem(t, prev, null);
                }
            }
            List<StreetItemToken> sli = StreetItemToken.TryParseList(t, 10, ad);
            if (sli != null) 
            {
                AddressItemToken rt = StreetDefineHelper.TryParseStreet(sli, prefixBefore, false, (prev != null && prev.Typ == AddressItemType.Street));
                if (rt == null && sli[0].Typ != StreetItemType.Fix) 
                {
                    Pullenti.Ner.Geo.Internal.OrgItemToken org = Pullenti.Ner.Geo.Internal.OrgItemToken.TryParse(t, null);
                    if (org != null) 
                    {
                        StreetItemToken si = new StreetItemToken(t, org.EndToken) { Typ = StreetItemType.Fix, Org = org };
                        sli.Clear();
                        sli.Add(si);
                        rt = StreetDefineHelper.TryParseStreet(sli, prefixBefore || prev != null, false, false);
                    }
                }
                if (rt != null) 
                {
                    if (sli.Count > 2) 
                    {
                    }
                    if (rt.BeginChar > sli[0].BeginChar) 
                        return null;
                    bool crlf = false;
                    for (Pullenti.Ner.Token ttt = rt.BeginToken; ttt != rt.EndToken && (ttt.EndChar < rt.EndChar); ttt = ttt.Next) 
                    {
                        if (ttt.IsNewlineAfter) 
                        {
                            crlf = true;
                            break;
                        }
                    }
                    if (crlf) 
                    {
                        for (Pullenti.Ner.Token ttt = rt.BeginToken.Previous; ttt != null; ttt = ttt.Previous) 
                        {
                            if (ttt.Morph.Class.IsPreposition || ttt.IsComma) 
                                continue;
                            if (ttt.GetReferent() is Pullenti.Ner.Geo.GeoReferent) 
                                crlf = false;
                            break;
                        }
                        if (sli[0].Typ == StreetItemType.Noun && sli[0].Termin.CanonicText.Contains("ДОРОГА")) 
                            crlf = false;
                    }
                    if (crlf) 
                    {
                        AddressItemToken aat = TryParsePureItem(rt.EndToken.Next, null, null);
                        if (aat == null) 
                            return null;
                        if (aat.Typ != AddressItemType.House) 
                            return null;
                    }
                    return rt;
                }
                if (sli.Count == 1 && sli[0].Typ == StreetItemType.Noun) 
                {
                    Pullenti.Ner.Token tt = sli[0].EndToken.Next;
                    if (tt != null && ((tt.IsHiphen || tt.IsChar('_') || tt.IsValue("НЕТ", null)))) 
                    {
                        Pullenti.Ner.Token ttt = tt.Next;
                        if (ttt != null && ttt.IsComma) 
                            ttt = ttt.Next;
                        AddressItemToken att = TryParsePureItem(ttt, null, null);
                        if (att != null) 
                        {
                            if (att.Typ == AddressItemType.House || att.Typ == AddressItemType.Corpus || att.Typ == AddressItemType.Building) 
                                return new AddressItemToken(AddressItemType.Street, t, tt);
                        }
                    }
                }
            }
            return TryParsePureItem(t, prev, ad);
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            ad.ARegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                AddressItemToken prev = null;
                int kk = 0;
                for (Pullenti.Ner.Token tt = t.Previous; tt != null && (kk < 10); tt = tt.Previous,kk++) 
                {
                    Pullenti.Ner.Geo.Internal.GeoTokenData dd = tt.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                    if (dd == null || dd.Street == null) 
                        continue;
                    if (dd.Street.EndToken.Next == t) 
                        prev = dd.Addr;
                    if (t.Previous != null && t.Previous.IsComma && dd.Street.EndToken.Next == t.Previous) 
                        prev = dd.Addr;
                }
                AddressItemToken str = TryParsePureItem(t, prev, null);
                if (str != null) 
                {
                    if (d == null) 
                        d = new Pullenti.Ner.Geo.Internal.GeoTokenData(t);
                    d.Addr = str;
                }
            }
            ad.ARegime = true;
        }
        public static AddressItemToken TryParsePureItem(Pullenti.Ner.Token t, AddressItemToken prev = null, Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = null)
        {
            if (t == null) 
                return null;
            if (t.IsChar(',')) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (SpeedRegime && ((ad.ARegime || ad.AllRegime)) && !(t is Pullenti.Ner.ReferentToken)) 
            {
                Pullenti.Ner.Geo.Internal.GeoTokenData d = t.Tag as Pullenti.Ner.Geo.Internal.GeoTokenData;
                if (d == null) 
                    return null;
                if (d.Addr == null) 
                    return null;
                return d.Addr;
            }
            if (ad.ALevel > 0) 
                return null;
            ad.Level++;
            AddressItemToken res = _tryParsePureItem(t, false, prev);
            if (res != null && res.Typ == AddressItemType.Detail) 
            {
            }
            else 
            {
                AddressItemToken det = _tryAttachDetail(t, null);
                if (res == null) 
                    res = det;
                else if (det != null && det.EndChar > res.EndChar) 
                    res = det;
            }
            ad.Level--;
            return res;
        }
        static AddressItemToken _tryParsePureItem(Pullenti.Ner.Token t, bool prefixBefore, AddressItemToken prev)
        {
            if (t is Pullenti.Ner.NumberToken) 
            {
                Pullenti.Ner.NumberToken n = t as Pullenti.Ner.NumberToken;
                if (((n.LengthChar == 6 || n.LengthChar == 5)) && n.Typ == Pullenti.Ner.NumberSpellingType.Digit && !n.Morph.Class.IsAdjective) 
                    return new AddressItemToken(AddressItemType.Zip, t, t) { Value = n.Value.ToString() };
                bool ok = false;
                if ((t.Previous != null && t.Previous.Morph.Class.IsPreposition && t.Next != null) && t.Next.Chars.IsLetter && t.Next.Chars.IsAllLower) 
                    ok = true;
                else if (t.Morph.Class.IsAdjective && !t.Morph.Class.IsNoun) 
                    ok = true;
                Pullenti.Ner.Core.TerminToken tok0 = m_Ontology.TryParse(t.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok0 != null && (tok0.Termin.Tag is AddressItemType)) 
                {
                    if (tok0.EndToken.Next == null || tok0.EndToken.Next.IsComma || tok0.EndToken.IsNewlineAfter) 
                        ok = true;
                    AddressItemType typ0 = (AddressItemType)tok0.Termin.Tag;
                    if (typ0 == AddressItemType.Flat) 
                    {
                        if ((t.Next is Pullenti.Ner.TextToken) && t.Next.IsValue("КВ", null)) 
                        {
                            if (t.Next.GetSourceText() == "кВ") 
                                return null;
                        }
                        if ((tok0.EndToken.Next is Pullenti.Ner.NumberToken) && (tok0.EndToken.WhitespacesAfterCount < 3)) 
                        {
                            if (prev != null && ((prev.Typ == AddressItemType.Street || prev.Typ == AddressItemType.City))) 
                                return new AddressItemToken(AddressItemType.Number, t, t) { Value = n.Value.ToString() };
                        }
                    }
                    if (tok0.EndToken.Next is Pullenti.Ner.NumberToken) 
                    {
                    }
                    else if ((typ0 == AddressItemType.Kilometer || typ0 == AddressItemType.Floor || typ0 == AddressItemType.Block) || typ0 == AddressItemType.Potch || typ0 == AddressItemType.Flat) 
                        return new AddressItemToken(typ0, t, tok0.EndToken) { Value = n.Value.ToString() };
                }
            }
            bool prepos = false;
            Pullenti.Ner.Core.TerminToken tok = null;
            if (t.Morph.Class.IsPreposition) 
            {
                if ((((tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No)))) == null) 
                {
                    if (t.BeginChar < t.EndChar) 
                        return null;
                    if (!t.IsCharOf("КСкс")) 
                        t = t.Next;
                    prepos = true;
                }
            }
            if (tok == null) 
                tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            Pullenti.Ner.Token t1 = t;
            AddressItemType typ = AddressItemType.Number;
            Pullenti.Ner.Address.AddressHouseType houseTyp = Pullenti.Ner.Address.AddressHouseType.Undefined;
            Pullenti.Ner.Address.AddressBuildingType buildTyp = Pullenti.Ner.Address.AddressBuildingType.Undefined;
            if (tok != null) 
            {
                if (t.IsValue("УЖЕ", null)) 
                    return null;
                if (tok.Termin.CanonicText == "ТАМ ЖЕ") 
                {
                    int cou = 0;
                    for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (cou > 1000) 
                            break;
                        Pullenti.Ner.Referent r = tt.GetReferent();
                        if (r == null) 
                            continue;
                        if (r is Pullenti.Ner.Address.AddressReferent) 
                        {
                            Pullenti.Ner.Geo.GeoReferent g = r.GetSlotValue(Pullenti.Ner.Address.AddressReferent.ATTR_GEO) as Pullenti.Ner.Geo.GeoReferent;
                            if (g != null) 
                                return new AddressItemToken(AddressItemType.City, t, tok.EndToken) { Referent = g };
                            break;
                        }
                        else if (r is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            Pullenti.Ner.Geo.GeoReferent g = r as Pullenti.Ner.Geo.GeoReferent;
                            if (!g.IsState) 
                                return new AddressItemToken(AddressItemType.City, t, tok.EndToken) { Referent = g };
                        }
                    }
                    return null;
                }
                if (tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType) 
                    return _tryAttachDetail(t, tok);
                t1 = tok.EndToken.Next;
                if (tok.Termin.Tag is AddressItemType) 
                {
                    if (tok.Termin.Tag2 is Pullenti.Ner.Address.AddressHouseType) 
                        houseTyp = (Pullenti.Ner.Address.AddressHouseType)tok.Termin.Tag2;
                    if (tok.Termin.Tag2 is Pullenti.Ner.Address.AddressBuildingType) 
                        buildTyp = (Pullenti.Ner.Address.AddressBuildingType)tok.Termin.Tag2;
                    typ = (AddressItemType)tok.Termin.Tag;
                    if (typ == AddressItemType.Prefix) 
                    {
                        for (; t1 != null; t1 = t1.Next) 
                        {
                            if (((t1.Morph.Class.IsPreposition || t1.Morph.Class.IsConjunction)) && t1.WhitespacesAfterCount == 1) 
                                continue;
                            if (t1.IsChar(':')) 
                            {
                                t1 = t1.Next;
                                break;
                            }
                            if (t1.IsChar('(')) 
                            {
                                Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t1, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                                if (br != null && (br.LengthChar < 50)) 
                                {
                                    t1 = br.EndToken;
                                    continue;
                                }
                            }
                            if (t1 is Pullenti.Ner.TextToken) 
                            {
                                if (t1.Chars.IsAllLower || (t1.WhitespacesBeforeCount < 3)) 
                                {
                                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(t1);
                                    if (npt != null) 
                                    {
                                        t1 = npt.EndToken;
                                        continue;
                                    }
                                }
                            }
                            if (t1.IsValue("УКАЗАННЫЙ", null) || t1.IsValue("ЕГРИП", null) || t1.IsValue("ФАКТИЧЕСКИЙ", null)) 
                                continue;
                            if (t1.IsComma) 
                            {
                                if (t1.Next != null && t1.Next.IsValue("УКАЗАННЫЙ", null)) 
                                    continue;
                            }
                            break;
                        }
                        if (t1 != null) 
                        {
                            Pullenti.Ner.Token t0 = t;
                            if (((t0.Previous != null && !t0.IsNewlineBefore && t0.Previous.IsChar(')')) && (t0.Previous.Previous is Pullenti.Ner.TextToken) && t0.Previous.Previous.Previous != null) && t0.Previous.Previous.Previous.IsChar('(')) 
                            {
                                t = t0.Previous.Previous.Previous.Previous;
                                if (t != null && t.GetMorphClassInDictionary().IsAdjective && !t.IsNewlineAfter) 
                                    t0 = t;
                            }
                            AddressItemToken res = new AddressItemToken(AddressItemType.Prefix, t0, t1.Previous);
                            for (Pullenti.Ner.Token tt = t0.Previous; tt != null; tt = tt.Previous) 
                            {
                                if (tt.NewlinesAfterCount > 3) 
                                    break;
                                if (tt.IsCommaAnd || tt.IsCharOf("().")) 
                                    continue;
                                if (!(tt is Pullenti.Ner.TextToken)) 
                                    break;
                                if (((tt.IsValue("ПОЧТОВЫЙ", null) || tt.IsValue("ЮРИДИЧЕСКИЙ", null) || tt.IsValue("ЮР", null)) || tt.IsValue("ФАКТИЧЕСКИЙ", null) || tt.IsValue("ФАКТ", null)) || tt.IsValue("ПОЧТ", null) || tt.IsValue("АДРЕС", null)) 
                                    res.BeginToken = tt;
                                else 
                                    break;
                            }
                            return res;
                        }
                        else 
                            return null;
                    }
                    else if ((typ == AddressItemType.CorpusOrFlat && !tok.IsWhitespaceBefore && !tok.IsWhitespaceAfter) && tok.BeginToken == tok.EndToken && tok.BeginToken.IsValue("К", null)) 
                        typ = AddressItemType.Corpus;
                    if (typ == AddressItemType.Detail && t.IsValue("У", null)) 
                    {
                        if (!Pullenti.Ner.Geo.Internal.MiscLocationHelper.CheckGeoObjectBefore(t, false)) 
                            return null;
                    }
                    if (typ == AddressItemType.Flat && t.IsValue("КВ", null)) 
                    {
                        if (t.GetSourceText() == "кВ") 
                            return null;
                    }
                    if (typ == AddressItemType.Kilometer || typ == AddressItemType.Floor || typ == AddressItemType.Potch) 
                        return new AddressItemToken(typ, t, tok.EndToken);
                    if ((typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus) || typ == AddressItemType.Plot) 
                    {
                        if (t1 != null && ((t1.Morph.Class.IsPreposition || t1.Morph.Class.IsConjunction)) && (t1.WhitespacesAfterCount < 2)) 
                        {
                            Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(t1.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                            if (tok2 != null && (tok2.Termin.Tag is AddressItemType)) 
                            {
                                AddressItemType typ2 = (AddressItemType)tok2.Termin.Tag;
                                if (typ2 != typ && ((typ2 == AddressItemType.Plot || ((typ2 == AddressItemType.House && typ == AddressItemType.Plot))))) 
                                {
                                    typ = typ2;
                                    if (tok.Termin.Tag2 is Pullenti.Ner.Address.AddressHouseType) 
                                        houseTyp = (Pullenti.Ner.Address.AddressHouseType)tok.Termin.Tag2;
                                    t1 = tok2.EndToken.Next;
                                    if (t1 == null) 
                                        return new AddressItemToken(typ, t, tok2.EndToken) { Value = "0", HouseType = houseTyp };
                                }
                            }
                        }
                    }
                    if (typ == AddressItemType.Field) 
                    {
                        AddressItemToken re = new AddressItemToken(typ, t, tok.EndToken);
                        StringBuilder nnn = new StringBuilder();
                        for (Pullenti.Ner.Token tt = tok.EndToken.Next; tt != null; tt = tt.Next) 
                        {
                            Pullenti.Ner.NumberToken ll = Pullenti.Ner.Core.NumberHelper.TryParseRoman(tt);
                            if (ll != null && ll.IntValue != null) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append(Pullenti.Ner.Core.NumberHelper.GetNumberRoman(ll.IntValue.Value));
                                re.EndToken = (tt = ll.EndToken);
                                continue;
                            }
                            if (tt.IsHiphen) 
                                continue;
                            if (tt.IsWhitespaceBefore) 
                                break;
                            if (tt is Pullenti.Ner.NumberToken) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append((tt as Pullenti.Ner.NumberToken).Value);
                                re.EndToken = tt;
                                continue;
                            }
                            if ((tt is Pullenti.Ner.TextToken) && tt.Chars.IsAllUpper) 
                            {
                                if (nnn.Length > 0) 
                                    nnn.Append("-");
                                nnn.Append((tt as Pullenti.Ner.TextToken).Term);
                                re.EndToken = tt;
                                continue;
                            }
                            break;
                        }
                        if (nnn.Length > 0) 
                        {
                            re.Value = nnn.ToString();
                            return re;
                        }
                    }
                    if (typ != AddressItemType.Number) 
                    {
                        if (t1 == null && t.LengthChar > 1) 
                            return new AddressItemToken(typ, t, tok.EndToken) { HouseType = houseTyp, BuildingType = buildTyp };
                        if ((t1 is Pullenti.Ner.NumberToken) && (t1 as Pullenti.Ner.NumberToken).Value == "0") 
                            return new AddressItemToken(typ, t, t1) { Value = "0", HouseType = houseTyp, BuildingType = buildTyp };
                    }
                }
            }
            if (t1 != null && t1.IsChar('.') && t1.Next != null) 
            {
                if (!t1.IsWhitespaceAfter) 
                    t1 = t1.Next;
                else if ((t1.Next is Pullenti.Ner.NumberToken) && (t1.Next as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit && (t1.WhitespacesAfterCount < 2)) 
                    t1 = t1.Next;
            }
            if ((t1 != null && !t1.IsWhitespaceAfter && ((t1.IsHiphen || t1.IsChar('_')))) && (t1.Next is Pullenti.Ner.NumberToken)) 
                t1 = t1.Next;
            tok = m_Ontology.TryParse(t1, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null && (tok.Termin.Tag is AddressItemType) && ((AddressItemType)tok.Termin.Tag) == AddressItemType.Number) 
                t1 = tok.EndToken.Next;
            else if (tok != null && (tok.Termin.Tag is AddressItemType) && ((AddressItemType)tok.Termin.Tag) == AddressItemType.NoNumber) 
            {
                AddressItemToken re0 = new AddressItemToken(typ, t, tok.EndToken) { Value = "0", HouseType = houseTyp, BuildingType = buildTyp };
                if (!re0.IsWhitespaceAfter && (re0.EndToken.Next is Pullenti.Ner.NumberToken)) 
                {
                    re0.EndToken = re0.EndToken.Next;
                    re0.Value = (re0.EndToken as Pullenti.Ner.NumberToken).Value.ToString();
                }
                return re0;
            }
            else if (t1 != null) 
            {
                if (typ == AddressItemType.Flat) 
                {
                    Pullenti.Ner.Core.TerminToken tok2 = m_Ontology.TryParse(t1, Pullenti.Ner.Core.TerminParseAttr.No);
                    if (tok2 != null && ((AddressItemType)tok2.Termin.Tag) == AddressItemType.Flat) 
                        t1 = tok2.EndToken.Next;
                }
                if (t1.IsValue("СТРОИТЕЛЬНЫЙ", null) && t1.Next != null) 
                    t1 = t1.Next;
                Pullenti.Ner.Token ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t1);
                if (ttt != null) 
                {
                    t1 = ttt;
                    if (t1.IsHiphen || t1.IsChar('_')) 
                        t1 = t1.Next;
                }
            }
            if (t1 == null) 
                return null;
            StringBuilder num = new StringBuilder();
            Pullenti.Ner.NumberToken nt = t1 as Pullenti.Ner.NumberToken;
            AddressItemToken re11;
            if (nt != null) 
            {
                if (nt.IntValue == null || nt.IntValue.Value == 0) 
                    return null;
                num.Append(nt.Value);
                if (nt.Typ == Pullenti.Ner.NumberSpellingType.Digit || nt.Typ == Pullenti.Ner.NumberSpellingType.Words) 
                {
                    if (((nt.EndToken is Pullenti.Ner.TextToken) && (nt.EndToken as Pullenti.Ner.TextToken).Term == "Е" && nt.EndToken.Previous == nt.BeginToken) && !nt.EndToken.IsWhitespaceBefore) 
                        num.Append("Е");
                    bool drob = false;
                    bool hiph = false;
                    bool lit = false;
                    Pullenti.Ner.Token et = nt.Next;
                    if (et != null && ((et.IsCharOf("\\/") || et.IsValue("ДРОБЬ", null)))) 
                    {
                        drob = true;
                        et = et.Next;
                        if (et != null && et.IsCharOf("\\/")) 
                            et = et.Next;
                        t1 = et;
                    }
                    else if (et != null && ((et.IsHiphen || et.IsChar('_')))) 
                    {
                        hiph = true;
                        et = et.Next;
                    }
                    else if ((et != null && et.IsChar('.') && (et.Next is Pullenti.Ner.NumberToken)) && !et.IsWhitespaceAfter) 
                        return null;
                    if (et is Pullenti.Ner.NumberToken) 
                    {
                        if (drob) 
                        {
                            num.AppendFormat("/{0}", (et as Pullenti.Ner.NumberToken).Value);
                            drob = false;
                            t1 = et;
                            et = et.Next;
                            if (et != null && et.IsCharOf("\\/") && (et.Next is Pullenti.Ner.NumberToken)) 
                            {
                                t1 = et.Next;
                                num.AppendFormat("/{0}", (t1 as Pullenti.Ner.NumberToken).Value);
                                et = t1.Next;
                            }
                        }
                        else if ((hiph && !t1.IsWhitespaceAfter && (et is Pullenti.Ner.NumberToken)) && !et.IsWhitespaceBefore) 
                        {
                            AddressItemToken numm = TryParsePureItem(et, null, null);
                            if (numm != null && numm.Typ == AddressItemType.Number) 
                            {
                                bool merge = false;
                                if (typ == AddressItemType.Flat || typ == AddressItemType.Plot) 
                                    merge = true;
                                else if (typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus) 
                                {
                                    Pullenti.Ner.Token ttt = numm.EndToken.Next;
                                    if (ttt != null && ttt.IsComma) 
                                        ttt = ttt.Next;
                                    AddressItemToken numm2 = TryParsePureItem(ttt, null, null);
                                    if (numm2 != null) 
                                    {
                                        if ((numm2.Typ == AddressItemType.Flat || numm2.Typ == AddressItemType.Building || ((numm2.Typ == AddressItemType.CorpusOrFlat && numm2.Value != null))) || numm2.Typ == AddressItemType.Corpus) 
                                            merge = true;
                                    }
                                }
                                if (merge) 
                                {
                                    num.AppendFormat("/{0}", numm.Value);
                                    t1 = numm.EndToken;
                                    et = t1.Next;
                                }
                            }
                        }
                    }
                    else if (et != null && ((et.IsHiphen || et.IsChar('_') || et.IsValue("НЕТ", null))) && drob) 
                        t1 = et;
                    Pullenti.Ner.Token ett = et;
                    if ((ett != null && ett.IsCharOf(",.") && (ett.WhitespacesAfterCount < 2)) && (ett.Next is Pullenti.Ner.TextToken) && Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next, false)) 
                        ett = ett.Next;
                    if (((Pullenti.Ner.Core.BracketHelper.IsBracket(ett, false) && (ett.Next is Pullenti.Ner.TextToken) && ett.Next.LengthChar == 1) && ett.Next.IsLetters && Pullenti.Ner.Core.BracketHelper.IsBracket(ett.Next.Next, false)) && !ett.IsWhitespaceAfter && !ett.Next.IsWhitespaceAfter) 
                    {
                        string ch = CorrectCharToken(ett.Next);
                        if (ch == null) 
                            return null;
                        num.Append(ch);
                        t1 = ett.Next.Next;
                    }
                    else if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(ett, true, false) && (ett.WhitespacesBeforeCount < 2)) 
                    {
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(ett, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null && (br.BeginToken.Next is Pullenti.Ner.TextToken) && br.BeginToken.Next.Next == br.EndToken) 
                        {
                            string s = CorrectCharToken(br.BeginToken.Next);
                            if (s != null) 
                            {
                                num.Append(s);
                                t1 = br.EndToken;
                            }
                        }
                    }
                    else if ((et is Pullenti.Ner.TextToken) && (et as Pullenti.Ner.TextToken).LengthChar == 1) 
                    {
                        string s = CorrectCharToken(et);
                        if (s != null) 
                        {
                            if (((s == "К" || s == "С")) && (et.Next is Pullenti.Ner.NumberToken) && !et.IsWhitespaceAfter) 
                            {
                            }
                            else if ((s == "Б" && et.Next != null && et.Next.IsCharOf("/\\")) && (et.Next.Next is Pullenti.Ner.TextToken) && et.Next.Next.IsValue("Н", null)) 
                                t1 = (et = et.Next.Next);
                            else 
                            {
                                bool ok = false;
                                if (drob || hiph || lit) 
                                    ok = true;
                                else if (!et.IsWhitespaceBefore || ((et.WhitespacesBeforeCount == 1 && ((et.Chars.IsAllUpper || ((et.IsNewlineAfter || ((et.Next != null && et.Next.IsComma))))))))) 
                                {
                                    ok = true;
                                    if (et.Next is Pullenti.Ner.NumberToken) 
                                    {
                                        if (!et.IsWhitespaceBefore && et.IsWhitespaceAfter) 
                                        {
                                        }
                                        else 
                                            ok = false;
                                    }
                                }
                                else if (((et.Next == null || et.Next.IsComma)) && (et.WhitespacesBeforeCount < 2)) 
                                    ok = true;
                                else if (et.IsWhitespaceBefore && et.Chars.IsAllLower && et.IsValue("В", "У")) 
                                {
                                }
                                else 
                                {
                                    AddressItemToken aitNext = TryParsePureItem(et.Next, null, null);
                                    if (aitNext != null) 
                                    {
                                        if ((aitNext.Typ == AddressItemType.Corpus || aitNext.Typ == AddressItemType.Flat || aitNext.Typ == AddressItemType.Building) || aitNext.Typ == AddressItemType.Office) 
                                            ok = true;
                                    }
                                }
                                if (ok) 
                                {
                                    num.Append(s);
                                    t1 = et;
                                    if (et.Next != null && et.Next.IsCharOf("\\/") && et.Next.Next != null) 
                                    {
                                        if (et.Next.Next is Pullenti.Ner.NumberToken) 
                                        {
                                            num.AppendFormat("/{0}", (et.Next.Next as Pullenti.Ner.NumberToken).Value);
                                            t1 = (et = et.Next.Next);
                                        }
                                        else if (et.Next.Next.IsHiphen || et.Next.Next.IsChar('_') || et.Next.Next.IsValue("НЕТ", null)) 
                                            t1 = (et = et.Next.Next);
                                    }
                                }
                            }
                        }
                    }
                    else if ((et is Pullenti.Ner.TextToken) && !et.IsWhitespaceBefore) 
                    {
                        string val = (et as Pullenti.Ner.TextToken).Term;
                        if (val == "КМ" && typ == AddressItemType.House) 
                        {
                            t1 = et;
                            num.Append("КМ");
                        }
                        else if (val == "БН") 
                            t1 = et;
                        else if (((val.Length == 2 && val[1] == 'Б' && et.Next != null) && et.Next.IsCharOf("\\/") && et.Next.Next != null) && et.Next.Next.IsValue("Н", null)) 
                        {
                            num.Append(val[0]);
                            t1 = (et = et.Next.Next);
                        }
                    }
                }
            }
            else if ((((re11 = _tryAttachVCH(t1, typ)))) != null) 
            {
                re11.BeginToken = t;
                re11.HouseType = houseTyp;
                re11.BuildingType = buildTyp;
                return re11;
            }
            else if (((t1 is Pullenti.Ner.TextToken) && t1.LengthChar == 2 && t1.IsLetters) && !t1.IsWhitespaceBefore && (t1.Previous is Pullenti.Ner.NumberToken)) 
            {
                string src = t1.GetSourceText();
                if ((src != null && src.Length == 2 && ((src[0] == 'к' || src[0] == 'k'))) && char.IsUpper(src[1])) 
                {
                    char ch = CorrectChar(src[1]);
                    if (ch != ((char)0)) 
                        return new AddressItemToken(AddressItemType.Corpus, t1, t1) { Value = string.Format("{0}", ch) };
                }
            }
            else if ((t1 is Pullenti.Ner.TextToken) && t1.LengthChar == 1 && t1.IsLetters) 
            {
                string ch = CorrectCharToken(t1);
                if (ch != null) 
                {
                    if (typ == AddressItemType.Number) 
                        return null;
                    if (ch == "К" || ch == "С") 
                    {
                        if (!t1.IsWhitespaceAfter && (t1.Next is Pullenti.Ner.NumberToken)) 
                            return null;
                    }
                    if (ch == "Д" && typ == AddressItemType.Plot) 
                    {
                        AddressItemToken rrr = TryParsePureItem(t1, null, null);
                        if (rrr != null) 
                        {
                            rrr.Typ = AddressItemType.Plot;
                            rrr.BeginToken = t;
                            return rrr;
                        }
                    }
                    if (t1.Chars.IsAllLower && ((t1.Morph.Class.IsPreposition || t1.Morph.Class.IsConjunction))) 
                    {
                        if ((t1.WhitespacesAfterCount < 2) && t1.Next.Chars.IsLetter) 
                            return null;
                    }
                    if (t.Chars.IsAllUpper && t.LengthChar == 1 && t.Next.IsChar('.')) 
                        return null;
                    num.Append(ch);
                    if ((t1.Next != null && ((t1.Next.IsHiphen || t1.Next.IsChar('_'))) && !t1.IsWhitespaceAfter) && (t1.Next.Next is Pullenti.Ner.NumberToken) && !t1.Next.IsWhitespaceAfter) 
                    {
                        num.Append((t1.Next.Next as Pullenti.Ner.NumberToken).Value);
                        t1 = t1.Next.Next;
                    }
                    else if ((t1.Next is Pullenti.Ner.NumberToken) && !t1.IsWhitespaceAfter && t1.Chars.IsAllUpper) 
                    {
                        num.Append((t1.Next as Pullenti.Ner.NumberToken).Value);
                        t1 = t1.Next;
                    }
                    if (num.Length == 1 && typ == AddressItemType.Office) 
                        return null;
                }
                if (typ == AddressItemType.Box && num.Length == 0) 
                {
                    Pullenti.Ner.NumberToken rom = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t1);
                    if (rom != null) 
                        return new AddressItemToken(typ, t, rom.EndToken) { Value = rom.Value.ToString() };
                }
            }
            else if (((Pullenti.Ner.Core.BracketHelper.IsBracket(t1, false) && (t1.Next is Pullenti.Ner.TextToken) && t1.Next.LengthChar == 1) && t1.Next.IsLetters && Pullenti.Ner.Core.BracketHelper.IsBracket(t1.Next.Next, false)) && !t1.IsWhitespaceAfter && !t1.Next.IsWhitespaceAfter) 
            {
                string ch = CorrectCharToken(t1.Next);
                if (ch == null) 
                    return null;
                num.Append(ch);
                t1 = t1.Next.Next;
            }
            else if ((t1 is Pullenti.Ner.TextToken) && ((((t1.LengthChar == 1 && ((t1.IsHiphen || t1.IsChar('_'))))) || t1.IsValue("НЕТ", null) || t1.IsValue("БН", null))) && (((typ == AddressItemType.Corpus || typ == AddressItemType.CorpusOrFlat || typ == AddressItemType.Building) || typ == AddressItemType.House || typ == AddressItemType.Flat))) 
            {
                while (t1.Next != null && ((t1.Next.IsHiphen || t1.Next.IsChar('_'))) && !t1.IsWhitespaceAfter) 
                {
                    t1 = t1.Next;
                }
                string val = null;
                if (!t1.IsWhitespaceAfter && (t1.Next is Pullenti.Ner.NumberToken)) 
                {
                    t1 = t1.Next;
                    val = (t1 as Pullenti.Ner.NumberToken).Value.ToString();
                }
                if (t1.IsValue("БН", null)) 
                    val = "0";
                return new AddressItemToken(typ, t, t1) { Value = val };
            }
            else 
            {
                if (((typ == AddressItemType.Floor || typ == AddressItemType.Kilometer || typ == AddressItemType.Potch)) && (t.Previous is Pullenti.Ner.NumberToken)) 
                    return new AddressItemToken(typ, t, t1.Previous);
                if ((t1 is Pullenti.Ner.ReferentToken) && (t1.GetReferent() is Pullenti.Ner.Date.DateReferent)) 
                {
                    AddressItemToken nn = TryParsePureItem((t1 as Pullenti.Ner.ReferentToken).BeginToken, null, null);
                    if (nn != null && nn.EndChar == t1.EndChar && nn.Typ == AddressItemType.Number) 
                    {
                        nn.BeginToken = t;
                        nn.EndToken = t1;
                        nn.Typ = typ;
                        return nn;
                    }
                }
                if ((t1 is Pullenti.Ner.TextToken) && ((typ == AddressItemType.House || typ == AddressItemType.Building || typ == AddressItemType.Corpus))) 
                {
                    string ter = (t1 as Pullenti.Ner.TextToken).Term;
                    if (ter == "АБ" || ter == "АБВ" || ter == "МГУ") 
                        return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                    if (t1.Chars.IsAllUpper) 
                    {
                        if (prev != null && ((prev.Typ == AddressItemType.Street || prev.Typ == AddressItemType.City))) 
                            return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                        if (typ == AddressItemType.Corpus && (t1.LengthChar < 4)) 
                            return new AddressItemToken(typ, t, t1) { Value = ter, HouseType = houseTyp, BuildingType = buildTyp };
                    }
                }
                if (typ == AddressItemType.Box) 
                {
                    Pullenti.Ner.NumberToken rom = Pullenti.Ner.Core.NumberHelper.TryParseRoman(t1);
                    if (rom != null) 
                        return new AddressItemToken(typ, t, rom.EndToken) { Value = rom.Value.ToString() };
                }
                if (typ == AddressItemType.Plot && t1 != null) 
                {
                    if ((t1.IsValue("ОКОЛО", null) || t1.IsValue("РЯДОМ", null) || t1.IsValue("НАПРОТИВ", null)) || t1.IsValue("БЛИЗЬКО", null) || t1.IsValue("НАВПАКИ", null)) 
                        return new AddressItemToken(typ, t, t1) { Value = t1.GetSourceText().ToLower() };
                }
                return null;
            }
            if (typ == AddressItemType.Number && prepos) 
                return null;
            if (t1 == null) 
            {
                t1 = t;
                while (t1.Next != null) 
                {
                    t1 = t1.Next;
                }
            }
            for (Pullenti.Ner.Token tt = t.Next; tt != null && tt.EndChar <= t1.EndChar; tt = tt.Next) 
            {
                if (tt.IsNewlineBefore) 
                    return null;
            }
            return new AddressItemToken(typ, t, t1) { Value = num.ToString(), Morph = t.Morph, HouseType = houseTyp, BuildingType = buildTyp };
        }
        static AddressItemToken _tryAttachVCH(Pullenti.Ner.Token t, AddressItemType ty)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Token tt = t;
            if ((((tt.IsValue("В", null) || tt.IsValue("B", null))) && tt.Next != null && tt.Next.IsCharOf("./\\")) && (tt.Next.Next is Pullenti.Ner.TextToken) && tt.Next.Next.IsValue("Ч", null)) 
            {
                tt = tt.Next.Next;
                if (tt.Next != null && tt.Next.IsChar('.')) 
                    tt = tt.Next;
                Pullenti.Ner.Token tt2 = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tt.Next);
                if (tt2 != null) 
                    tt = tt2;
                if (tt.Next != null && (tt.Next is Pullenti.Ner.NumberToken) && (tt.WhitespacesAfterCount < 2)) 
                    tt = tt.Next;
                return new AddressItemToken(ty, t, tt) { Value = "В/Ч" };
            }
            if (((tt.IsValue("ВОЙСКОВОЙ", null) || tt.IsValue("ВОИНСКИЙ", null))) && tt.Next != null && tt.Next.IsValue("ЧАСТЬ", null)) 
            {
                tt = tt.Next;
                Pullenti.Ner.Token tt2 = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(tt.Next);
                if (tt2 != null) 
                    tt = tt2;
                if (tt.Next != null && (tt.Next is Pullenti.Ner.NumberToken) && (tt.WhitespacesAfterCount < 2)) 
                    tt = tt.Next;
                return new AddressItemToken(ty, t, tt) { Value = "В/Ч" };
            }
            if (ty == AddressItemType.Flat) 
            {
                if (tt.WhitespacesBeforeCount > 1) 
                    return null;
                if (!(tt is Pullenti.Ner.TextToken)) 
                    return null;
                if ((tt as Pullenti.Ner.TextToken).Term.StartsWith("ОБЩ")) 
                {
                    if (tt.Next != null && tt.Next.IsChar('.')) 
                        tt = tt.Next;
                    AddressItemToken re = _tryAttachVCH(tt.Next, ty);
                    if (re != null) 
                        return re;
                    return new AddressItemToken(ty, t, tt) { Value = "ОБЩ" };
                }
                if (tt.Chars.IsAllUpper && tt.LengthChar > 1) 
                {
                    AddressItemToken re = new AddressItemToken(ty, t, tt) { Value = (tt as Pullenti.Ner.TextToken).Term };
                    if ((tt.WhitespacesAfterCount < 2) && (tt.Next is Pullenti.Ner.TextToken) && tt.Next.Chars.IsAllUpper) 
                    {
                        tt = tt.Next;
                        re.EndToken = tt;
                        re.Value += (tt as Pullenti.Ner.TextToken).Term;
                    }
                    return re;
                }
            }
            return null;
        }
        static AddressItemToken _tryAttachDetail(Pullenti.Ner.Token t, Pullenti.Ner.Core.TerminToken tok)
        {
            if (t == null || (t is Pullenti.Ner.ReferentToken)) 
                return null;
            Pullenti.Ner.Token tt = t;
            if (t.Chars.IsCapitalUpper && !t.Morph.Class.IsPreposition) 
                return null;
            if (tok == null) 
                tok = m_Ontology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok == null && t.Morph.Class.IsPreposition && t.Next != null) 
            {
                tt = t.Next;
                if (tt is Pullenti.Ner.NumberToken) 
                {
                }
                else 
                {
                    if (tt.Chars.IsCapitalUpper && !tt.Morph.Class.IsPreposition) 
                        return null;
                    tok = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                }
            }
            AddressItemToken res = null;
            bool firstNum = false;
            if (tok == null) 
            {
                if (tt is Pullenti.Ner.NumberToken) 
                {
                    firstNum = true;
                    Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                    if (nex != null && ((nex.ExTyp == Pullenti.Ner.Core.NumberExType.Meter || nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer))) 
                    {
                        res = new AddressItemToken(AddressItemType.Detail, t, nex.EndToken);
                        Pullenti.Ner.Core.NumberExType tyy = Pullenti.Ner.Core.NumberExType.Meter;
                        res.DetailMeters = (int)nex.NormalizeValue(ref tyy);
                    }
                }
                if (res == null) 
                    return null;
            }
            else 
            {
                if (!(tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType)) 
                    return null;
                res = new AddressItemToken(AddressItemType.Detail, t, tok.EndToken) { DetailType = (Pullenti.Ner.Address.AddressDetailType)tok.Termin.Tag };
            }
            for (tt = res.EndToken.Next; tt != null; tt = tt.Next) 
            {
                if (tt is Pullenti.Ner.ReferentToken) 
                    break;
                if (!tt.Morph.Class.IsPreposition) 
                {
                    if (tt.Chars.IsCapitalUpper || tt.Chars.IsAllUpper) 
                        break;
                }
                tok = m_Ontology.TryParse(tt, Pullenti.Ner.Core.TerminParseAttr.No);
                if (tok != null && (tok.Termin.Tag is Pullenti.Ner.Address.AddressDetailType)) 
                {
                    Pullenti.Ner.Address.AddressDetailType ty = (Pullenti.Ner.Address.AddressDetailType)tok.Termin.Tag;
                    if (ty != Pullenti.Ner.Address.AddressDetailType.Undefined) 
                    {
                        if (ty == Pullenti.Ner.Address.AddressDetailType.Near && res.DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && res.DetailType != ty) 
                        {
                        }
                        else 
                            res.DetailType = ty;
                    }
                    res.EndToken = (tt = tok.EndToken);
                    continue;
                }
                if (tt.IsValue("ОРИЕНТИР", null) || tt.IsValue("НАПРАВЛЕНИЕ", null) || tt.IsValue("ОТ", null)) 
                {
                    res.EndToken = tt;
                    continue;
                }
                if (tt.IsComma || tt.Morph.Class.IsPreposition) 
                    continue;
                if ((tt is Pullenti.Ner.NumberToken) && tt.Next != null) 
                {
                    Pullenti.Ner.Core.NumberExToken nex = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(tt);
                    if (nex != null && ((nex.ExTyp == Pullenti.Ner.Core.NumberExType.Meter || nex.ExTyp == Pullenti.Ner.Core.NumberExType.Kilometer))) 
                    {
                        res.EndToken = (tt = nex.EndToken);
                        Pullenti.Ner.Core.NumberExType tyy = Pullenti.Ner.Core.NumberExType.Meter;
                        res.DetailMeters = (int)nex.NormalizeValue(ref tyy);
                        continue;
                    }
                }
                break;
            }
            if (firstNum && res.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                return null;
            if (res != null && res.EndToken.Next != null && res.EndToken.Next.Morph.Class.IsPreposition) 
            {
                if (res.EndToken.WhitespacesAfterCount == 1 && res.EndToken.Next.WhitespacesAfterCount == 1) 
                    res.EndToken = res.EndToken.Next;
            }
            return res;
        }
        public static bool CheckStreetAfter(Pullenti.Ner.Token t, bool checkThisAndNotNext = false)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.IsHiphen || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            if (t.IsNewlineBefore) 
                return false;
            AddressItemToken ait = TryParse(t, false, null, null);
            if (ait == null || ait.Typ != AddressItemType.Street) 
                return false;
            if (ait.RefToken != null) 
            {
                if (!ait.RefTokenIsGsk) 
                    return false;
                Pullenti.Ner.Geo.Internal.OrgItemToken oo = ait.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken;
                if (oo != null && oo.IsDoubt) 
                    return false;
            }
            if (!checkThisAndNotNext) 
                return true;
            if (t.Next == null || ait.EndChar <= t.EndChar) 
                return true;
            AddressItemToken ait2 = TryParse(t.Next, false, null, null);
            if (ait2 == null) 
                return true;
            List<AddressItemToken> aits1 = TryParseList(t, 20);
            List<AddressItemToken> aits2 = TryParseList(t.Next, 20);
            if (aits1 != null && aits2 != null) 
            {
                if (aits2[aits2.Count - 1].EndChar >= aits1[aits1.Count - 1].EndChar) 
                    return false;
            }
            return true;
        }
        public static bool CheckHouseAfter(Pullenti.Ner.Token t, bool leek = false, bool pureHouse = false)
        {
            if (t == null) 
                return false;
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            if (t.IsNewlineBefore) 
                return false;
            AddressItemToken ait = TryParsePureItem(t, null, null);
            if (ait != null) 
            {
                if (pureHouse) 
                    return ait.Typ == AddressItemType.House || ait.Typ == AddressItemType.Plot;
                if ((ait.Typ == AddressItemType.House || ait.Typ == AddressItemType.Floor || ait.Typ == AddressItemType.Office) || ait.Typ == AddressItemType.Flat || ait.Typ == AddressItemType.Plot) 
                {
                    if (((t is Pullenti.Ner.TextToken) && t.Chars.IsAllUpper && t.Next != null) && t.Next.IsHiphen && (t.Next.Next is Pullenti.Ner.NumberToken)) 
                        return false;
                    if ((t is Pullenti.Ner.TextToken) && t.Next == ait.EndToken && t.Next.IsHiphen) 
                        return false;
                    return true;
                }
                if (leek) 
                {
                    if (ait.Typ == AddressItemType.Number) 
                        return true;
                }
                if (ait.Typ == AddressItemType.Number) 
                {
                    Pullenti.Ner.Token t1 = t.Next;
                    while (t1 != null && t1.IsCharOf(".,")) 
                    {
                        t1 = t1.Next;
                    }
                    ait = TryParsePureItem(t1, null, null);
                    if (ait != null && (((ait.Typ == AddressItemType.Building || ait.Typ == AddressItemType.Corpus || ait.Typ == AddressItemType.Flat) || ait.Typ == AddressItemType.Floor || ait.Typ == AddressItemType.Office))) 
                        return true;
                }
            }
            return false;
        }
        public static bool CheckKmAfter(Pullenti.Ner.Token t)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Next,cou++) 
            {
                if (t.IsCharOf(",.") || t.Morph.Class.IsPreposition) 
                {
                }
                else 
                    break;
            }
            if (t == null) 
                return false;
            AddressItemToken km = TryParsePureItem(t, null, null);
            if (km != null && km.Typ == AddressItemType.Kilometer) 
                return true;
            if (!(t is Pullenti.Ner.NumberToken) || t.Next == null) 
                return false;
            if (t.Next.IsValue("КИЛОМЕТР", null) || t.Next.IsValue("МЕТР", null) || t.Next.IsValue("КМ", null)) 
                return true;
            return false;
        }
        public static bool CheckKmBefore(Pullenti.Ner.Token t)
        {
            int cou = 0;
            for (; t != null && (cou < 4); t = t.Previous,cou++) 
            {
                if (t.IsCharOf(",.")) 
                {
                }
                else if (t.IsValue("КМ", null) || t.IsValue("КИЛОМЕТР", null) || t.IsValue("МЕТР", null)) 
                    return true;
            }
            return false;
        }
        public static char CorrectChar(char v)
        {
            if (v == 'A' || v == 'А') 
                return 'А';
            if (v == 'Б' || v == 'Г') 
                return v;
            if (v == 'B' || v == 'В') 
                return 'В';
            if (v == 'C' || v == 'С') 
                return 'С';
            if (v == 'D' || v == 'Д') 
                return 'Д';
            if (v == 'E' || v == 'Е') 
                return 'Е';
            if (v == 'H' || v == 'Н') 
                return 'Н';
            if (v == 'K' || v == 'К') 
                return 'К';
            return (char)0;
        }
        static string CorrectCharToken(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.TextToken tt = t as Pullenti.Ner.TextToken;
            if (tt == null) 
                return null;
            string v = tt.Term;
            if (v.Length != 1) 
                return null;
            char corr = CorrectChar(v[0]);
            if (corr != ((char)0)) 
                return string.Format("{0}", corr);
            if (t.Chars.IsCyrillicLetter) 
                return v;
            return null;
        }
        public static void Initialize()
        {
            if (m_Ontology != null) 
                return;
            StreetItemToken.Initialize();
            m_Ontology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t;
            t = new Pullenti.Ner.Core.Termin("ДОМ") { Tag = AddressItemType.House };
            t.AddAbridge("Д.");
            t.AddVariant("КОТТЕДЖ", false);
            t.AddAbridge("КОТ.");
            t.AddVariant("ДАЧА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БУДИНОК") { Tag = AddressItemType.House, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("Б.");
            t.AddVariant("КОТЕДЖ", false);
            t.AddAbridge("БУД.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВЛАДЕНИЕ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.Estate };
            t.AddAbridge("ВЛАД.");
            t.AddAbridge("ВЛД.");
            t.AddAbridge("ВЛ.");
            m_Ontology.Add(t);
            m_Owner = t;
            t = new Pullenti.Ner.Core.Termin("ДОМОВЛАДЕНИЕ") { Tag = AddressItemType.House, Tag2 = Pullenti.Ner.Address.AddressHouseType.HouseEstate };
            t.AddVariant("ДОМОВЛАДЕНИЕ", false);
            t.AddAbridge("ДВЛД.");
            t.AddAbridge("ДМВЛД.");
            t.AddVariant("ДОМОВЛ", false);
            t.AddVariant("ДОМОВА", false);
            t.AddVariant("ДОМОВЛАД", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДЪЕЗД ДОМА") { Tag = AddressItemType.House };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДВАЛ ДОМА") { Tag = AddressItemType.House };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КРЫША ДОМА") { Tag = AddressItemType.House };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЭТАЖ") { Tag = AddressItemType.Floor };
            t.AddAbridge("ЭТ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДЪЕЗД") { Tag = AddressItemType.Potch };
            t.AddAbridge("ПОД.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОРПУС") { Tag = AddressItemType.Corpus };
            t.AddAbridge("КОРП.");
            t.AddAbridge("КОР.");
            t.AddAbridge("Д.КОРП.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("К") { Tag = AddressItemType.CorpusOrFlat };
            t.AddAbridge("К.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОЕНИЕ") { Tag = AddressItemType.Building };
            t.AddAbridge("СТРОЕН.");
            t.AddAbridge("СТР.");
            t.AddAbridge("СТ.");
            t.AddAbridge("ПОМ.СТР.");
            t.AddAbridge("Д.СТР.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СООРУЖЕНИЕ") { Tag = AddressItemType.Building, Tag2 = Pullenti.Ner.Address.AddressBuildingType.Construction };
            t.AddAbridge("СООР.");
            t.AddAbridge("СООРУЖ.");
            t.AddAbridge("СООРУЖЕН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИТЕРА") { Tag = AddressItemType.Building, Tag2 = Pullenti.Ner.Address.AddressBuildingType.Liter };
            t.AddAbridge("ЛИТ.");
            t.AddVariant("ЛИТЕР", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("УЧАСТОК") { Tag = AddressItemType.Plot };
            t.AddAbridge("УЧАСТ.");
            t.AddAbridge("УЧ.");
            t.AddAbridge("УЧ-К");
            t.AddVariant("ЗЕМЕЛЬНЫЙ УЧАСТОК", false);
            t.AddAbridge("ЗЕМ.УЧ.");
            t.AddAbridge("ЗЕМ.УЧ-К");
            t.AddAbridge("З/У");
            t.AddAbridge("ПОЗ.");
            m_Ontology.Add(t);
            m_Plot = t;
            t = new Pullenti.Ner.Core.Termin("ПОЛЕ") { Tag = AddressItemType.Field };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТИРА") { Tag = AddressItemType.Flat };
            t.AddAbridge("КВАРТ.");
            t.AddAbridge("КВАР.");
            t.AddAbridge("КВ.");
            t.AddAbridge("KB.");
            t.AddAbridge("КВ-РА");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОФИС") { Tag = AddressItemType.Office };
            t.AddAbridge("ОФ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОФІС") { Tag = AddressItemType.Office, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("ОФ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАВИЛЬОН") { Tag = AddressItemType.Pavilion };
            t.AddAbridge("ПАВ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПАВІЛЬЙОН") { Tag = AddressItemType.Pavilion, Lang = Pullenti.Morph.MorphLang.UA };
            t.AddAbridge("ПАВ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БЛОК") { Tag = AddressItemType.Block };
            t.AddVariant("РЯД", false);
            t.AddVariant("СЕКТОР", false);
            t.AddAbridge("СЕК.");
            t.AddVariant("МАССИВ", false);
            t.AddVariant("ОЧЕРЕДЬ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БОКС") { Tag = AddressItemType.Box };
            t.AddVariant("ГАРАЖ", false);
            t.AddVariant("САРАЙ", false);
            t.AddAbridge("ГАР.");
            t.AddVariant("МАШИНОМЕСТО", false);
            t.AddVariant("ПОМЕЩЕНИЕ", false);
            t.AddAbridge("ПОМ.");
            t.AddVariant("НЕЖИЛОЕ ПОМЕЩЕНИЕ", false);
            t.AddAbridge("Н.П.");
            t.AddAbridge("НП");
            t.AddVariant("ПОДВАЛ", false);
            t.AddVariant("ПОГРЕБ", false);
            t.AddVariant("ПОДВАЛЬНОЕ ПОМЕЩЕНИЕ", false);
            t.AddVariant("ПОДЪЕЗД", false);
            t.AddAbridge("ГАРАЖ-БОКС");
            t.AddVariant("ГАРАЖНЫЙ БОКС", false);
            t.AddAbridge("ГБ.");
            t.AddAbridge("Г.Б.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОМНАТА") { Tag = AddressItemType.Office };
            t.AddAbridge("КОМ.");
            t.AddAbridge("КОМН.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КАБИНЕТ") { Tag = AddressItemType.Office };
            t.AddAbridge("КАБ.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НОМЕР") { Tag = AddressItemType.Number };
            t.AddAbridge("НОМ.");
            t.AddAbridge("№");
            t.AddAbridge("N");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БЕЗ НОМЕРА") { CanonicText = "Б/Н", Tag = AddressItemType.NoNumber };
            t.AddAbridge("Б.Н.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АБОНЕНТСКИЙ ЯЩИК") { Tag = AddressItemType.PostOfficeBox };
            t.AddAbridge("А.Я.");
            t.AddVariant("ПОЧТОВЫЙ ЯЩИК", false);
            t.AddAbridge("П.Я.");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГОРОДСКАЯ СЛУЖЕБНАЯ ПОЧТА") { Tag = AddressItemType.CSP, Acronym = "ГСП" };
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АДРЕС") { Tag = AddressItemType.Prefix };
            t.AddVariant("ЮРИДИЧЕСКИЙ АДРЕС", false);
            t.AddVariant("ФАКТИЧЕСКИЙ АДРЕС", false);
            t.AddAbridge("ЮР.АДРЕС");
            t.AddAbridge("ПОЧТ.АДРЕС");
            t.AddAbridge("ФАКТ.АДРЕС");
            t.AddAbridge("П.АДРЕС");
            t.AddVariant("ЮРИДИЧЕСКИЙ/ФАКТИЧЕСКИЙ АДРЕС", false);
            t.AddVariant("ПОЧТОВЫЙ АДРЕС", false);
            t.AddVariant("АДРЕС ПРОЖИВАНИЯ", false);
            t.AddVariant("МЕСТО НАХОЖДЕНИЯ", false);
            t.AddVariant("МЕСТОНАХОЖДЕНИЕ", false);
            t.AddVariant("МЕСТОПОЛОЖЕНИЕ", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АДРЕСА") { Tag = AddressItemType.Prefix };
            t.AddVariant("ЮРИДИЧНА АДРЕСА", false);
            t.AddVariant("ФАКТИЧНА АДРЕСА", false);
            t.AddVariant("ПОШТОВА АДРЕСА", false);
            t.AddVariant("АДРЕСА ПРОЖИВАННЯ", false);
            t.AddVariant("МІСЦЕ ПЕРЕБУВАННЯ", false);
            t.AddVariant("ПРОПИСКА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КИЛОМЕТР") { Tag = AddressItemType.Kilometer };
            t.AddAbridge("КИЛОМ.");
            t.AddAbridge("КМ.");
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПЕРЕСЕЧЕНИЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Cross });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НА ПЕРЕСЕЧЕНИИ") { Tag = Pullenti.Ner.Address.AddressDetailType.Cross });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПЕРЕКРЕСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.Cross });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НА ПЕРЕКРЕСТКЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Cross });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НА ТЕРРИТОРИИ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕРЕДИНА") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИМЫКАТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ГРАНИЧИТЬ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near });
            t = new Pullenti.Ner.Core.Termin("ВБЛИЗИ") { Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddVariant("У", false);
            t.AddAbridge("ВБЛ.");
            t.AddVariant("ВОЗЛЕ", false);
            t.AddVariant("ОКОЛО", false);
            t.AddVariant("НЕДАЛЕКО ОТ", false);
            t.AddVariant("РЯДОМ С", false);
            t.AddVariant("ГРАНИЦА", false);
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("РАЙОН") { Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddAbridge("Р-Н");
            m_Ontology.Add(t);
            t = new Pullenti.Ner.Core.Termin("В РАЙОНЕ") { CanonicText = "РАЙОН", Tag = Pullenti.Ner.Address.AddressDetailType.Near };
            t.AddAbridge("В Р-НЕ");
            m_Ontology.Add(t);
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИМЕРНО") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПОРЯДКА") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ПРИБЛИЗИТЕЛЬНО") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("НАПРАВЛЕНИЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Undefined });
            t = new Pullenti.Ner.Core.Termin("ОБЩЕЖИТИЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.Hostel };
            t.AddAbridge("ОБЩ.");
            t.AddAbridge("ПОМ.ОБЩ.");
            m_Ontology.Add(t);
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = true;
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.North });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕР") { Tag = Pullenti.Ner.Address.AddressDetailType.North });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮЖНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.South });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГ") { Tag = Pullenti.Ner.Address.AddressDetailType.South });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.West });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.West });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.East });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.East });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("СЕВЕРО-ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.NorthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ЗАПАДНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ЗАПАД") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthWest });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ВОСТОЧНЕЕ") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthEast });
            m_Ontology.Add(new Pullenti.Ner.Core.Termin("ЮГО-ВОСТОК") { Tag = Pullenti.Ner.Address.AddressDetailType.SouthEast });
            t = new Pullenti.Ner.Core.Termin("ТАМ ЖЕ");
            t.AddAbridge("ТАМЖЕ");
            m_Ontology.Add(t);
            Pullenti.Ner.Core.Termin.AssignAllTextsAsNormal = false;
        }
        static Pullenti.Ner.Core.TerminCollection m_Ontology;
        public static Pullenti.Ner.Core.Termin m_Plot;
        public static Pullenti.Ner.Core.Termin m_Owner;
        public static List<AddressItemToken> TryParseList(Pullenti.Ner.Token t, int maxCount = 20)
        {
            if (t == null) 
                return null;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad != null) 
            {
                if (ad.Level > 0) 
                    return null;
                ad.Level++;
            }
            List<AddressItemToken> res = TryParseListInt(t, maxCount);
            if (ad != null) 
                ad.Level--;
            if (res != null && res.Count == 0) 
                return null;
            return res;
        }
        static List<AddressItemToken> TryParseListInt(Pullenti.Ner.Token t, int maxCount = 20)
        {
            if (t is Pullenti.Ner.NumberToken) 
            {
                if ((t as Pullenti.Ner.NumberToken).IntValue == null) 
                    return null;
                int v = (t as Pullenti.Ner.NumberToken).IntValue.Value;
                if ((v < 100000) || v >= 10000000) 
                {
                    if ((t as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit && !t.Morph.Class.IsAdjective) 
                    {
                        if (t.Next == null || (t.Next is Pullenti.Ner.NumberToken)) 
                        {
                            if (t.Previous == null || !t.Previous.Morph.Class.IsPreposition) 
                                return null;
                        }
                    }
                }
            }
            AddressItemToken it = TryParse(t, false, null, null);
            if (it == null) 
                return null;
            if (it.Typ == AddressItemType.Number) 
                return null;
            if (it.Typ == AddressItemType.Kilometer && (it.BeginToken.Previous is Pullenti.Ner.NumberToken)) 
            {
                it = it.Clone();
                it.BeginToken = it.BeginToken.Previous;
                it.Value = (it.BeginToken as Pullenti.Ner.NumberToken).Value.ToString();
                if (it.BeginToken.Previous != null && it.BeginToken.Previous.Morph.Class.IsPreposition) 
                    it.BeginToken = it.BeginToken.Previous;
            }
            List<AddressItemToken> res = new List<AddressItemToken>();
            res.Add(it);
            bool pref = it.Typ == AddressItemType.Prefix;
            for (t = it.EndToken.Next; t != null; t = t.Next) 
            {
                if (maxCount > 0 && res.Count >= maxCount) 
                    break;
                AddressItemToken last = res[res.Count - 1];
                if (res.Count > 1) 
                {
                    if (last.IsNewlineBefore && res[res.Count - 2].Typ != AddressItemType.Prefix) 
                    {
                        int i;
                        for (i = 0; i < (res.Count - 1); i++) 
                        {
                            if (res[i].Typ == last.Typ) 
                            {
                                if (i == (res.Count - 2) && ((last.Typ == AddressItemType.City || last.Typ == AddressItemType.Region))) 
                                {
                                    int jj;
                                    for (jj = 0; jj < i; jj++) 
                                    {
                                        if ((res[jj].Typ != AddressItemType.Prefix && res[jj].Typ != AddressItemType.Zip && res[jj].Typ != AddressItemType.Region) && res[jj].Typ != AddressItemType.Country) 
                                            break;
                                    }
                                    if (jj >= i) 
                                        continue;
                                }
                                break;
                            }
                        }
                        if ((i < (res.Count - 1)) || last.Typ == AddressItemType.Zip) 
                        {
                            res.Remove(last);
                            break;
                        }
                    }
                }
                if (t.IsTableControlChar) 
                    break;
                if (t.IsChar(',')) 
                    continue;
                if (Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(t, true, null, false) && last.Typ == AddressItemType.Street) 
                    continue;
                if (t.IsChar('.')) 
                {
                    if (t.IsNewlineAfter) 
                        break;
                    if (t.Previous != null && t.Previous.IsChar('.')) 
                        break;
                    continue;
                }
                if (t.IsHiphen || t.IsChar('_')) 
                {
                    if (((it.Typ == AddressItemType.Number || it.Typ == AddressItemType.Street)) && (t.Next is Pullenti.Ner.NumberToken)) 
                        continue;
                }
                if (it.Typ == AddressItemType.Detail && it.DetailType == Pullenti.Ner.Address.AddressDetailType.Cross) 
                {
                    AddressItemToken str1 = TryParse(t, true, null, null);
                    if (str1 != null && str1.Typ == AddressItemType.Street) 
                    {
                        if (str1.EndToken.Next != null && ((str1.EndToken.Next.IsAnd || str1.EndToken.Next.IsHiphen))) 
                        {
                            AddressItemToken str2 = TryParse(str1.EndToken.Next.Next, true, null, null);
                            if (str2 == null || str2.Typ != AddressItemType.Street) 
                            {
                                str2 = StreetDefineHelper.TryParseSecondStreet(str1.BeginToken, str1.EndToken.Next.Next);
                                if (str2 != null && str2.IsDoubt) 
                                {
                                    str2 = str2.Clone();
                                    str2.IsDoubt = false;
                                }
                            }
                            if (str2 != null && str2.Typ == AddressItemType.Street) 
                            {
                                res.Add(str1);
                                res.Add(str2);
                                t = str2.EndToken;
                                it = str2;
                                continue;
                            }
                        }
                    }
                }
                bool pre = pref;
                if (it.Typ == AddressItemType.Kilometer || it.Typ == AddressItemType.House) 
                {
                    if (!t.IsNewlineBefore) 
                        pre = true;
                }
                AddressItemToken it0 = TryParse(t, pre, it, null);
                if (it0 == null) 
                {
                    if (t.NewlinesBeforeCount > 2) 
                        break;
                    if (it.Typ == AddressItemType.PostOfficeBox) 
                        break;
                    if (t.Morph.Class.IsPreposition) 
                    {
                        it0 = TryParse(t.Next, false, it, null);
                        if (it0 != null && it0.Typ == AddressItemType.Building && it0.BeginToken.IsValue("СТ", null)) 
                        {
                            it0 = null;
                            break;
                        }
                        if (it0 != null) 
                        {
                            if ((it0.Typ == AddressItemType.Detail && it.Typ == AddressItemType.City && it.DetailMeters > 0) && it.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            {
                                it.DetailType = it0.DetailType;
                                t = (it.EndToken = it0.EndToken);
                                continue;
                            }
                            if ((it0.Typ == AddressItemType.House || it0.Typ == AddressItemType.Building || it0.Typ == AddressItemType.Corpus) || it0.Typ == AddressItemType.Street || it0.Typ == AddressItemType.Detail) 
                            {
                                res.Add((it = it0));
                                t = it.EndToken;
                                continue;
                            }
                        }
                    }
                    if (it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Number) 
                    {
                        if ((!t.IsWhitespaceBefore && t.LengthChar == 1 && t.Chars.IsLetter) && !t.IsWhitespaceAfter && (t.Next is Pullenti.Ner.NumberToken)) 
                        {
                            string ch = CorrectCharToken(t);
                            if (ch == "К" || ch == "С") 
                            {
                                it0 = new AddressItemToken((ch == "К" ? AddressItemType.Corpus : AddressItemType.Building), t, t.Next) { Value = (t.Next as Pullenti.Ner.NumberToken).Value.ToString() };
                                it = it0;
                                res.Add(it);
                                t = it.EndToken;
                                Pullenti.Ner.Token tt = t.Next;
                                if (((tt != null && !tt.IsWhitespaceBefore && tt.LengthChar == 1) && tt.Chars.IsLetter && !tt.IsWhitespaceAfter) && (tt.Next is Pullenti.Ner.NumberToken)) 
                                {
                                    ch = CorrectCharToken(tt);
                                    if (ch == "К" || ch == "С") 
                                    {
                                        it = new AddressItemToken((ch == "К" ? AddressItemType.Corpus : AddressItemType.Building), tt, tt.Next) { Value = (tt.Next as Pullenti.Ner.NumberToken).Value.ToString() };
                                        res.Add(it);
                                        t = it.EndToken;
                                    }
                                }
                                continue;
                            }
                        }
                    }
                    if (t.Morph.Class.IsPreposition) 
                    {
                        if ((((t.IsValue("У", null) || t.IsValue("ВОЗЛЕ", null) || t.IsValue("НАПРОТИВ", null)) || t.IsValue("НА", null) || t.IsValue("В", null)) || t.IsValue("ВО", null) || t.IsValue("ПО", null)) || t.IsValue("ОКОЛО", null)) 
                            continue;
                    }
                    if (t.Morph.Class.IsNoun) 
                    {
                        if ((t.IsValue("ДВОР", null) || t.IsValue("ПОДЪЕЗД", null) || t.IsValue("КРЫША", null)) || t.IsValue("ПОДВАЛ", null)) 
                            continue;
                    }
                    if (t.IsValue("ТЕРРИТОРИЯ", "ТЕРИТОРІЯ")) 
                        continue;
                    if (t.IsChar('(') && t.Next != null) 
                    {
                        it0 = TryParse(t.Next, pre, null, null);
                        if (it0 != null && it0.EndToken.Next != null && it0.EndToken.Next.IsChar(')')) 
                        {
                            it0 = it0.Clone();
                            it0.BeginToken = t;
                            it0.EndToken = it0.EndToken.Next;
                            it = it0;
                            res.Add(it);
                            t = it.EndToken;
                            continue;
                        }
                        Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
                        if (br != null && (br.LengthChar < 100)) 
                        {
                            if (t.Next.IsValue("БЫВШИЙ", null) || t.Next.IsValue("БЫВШ", null)) 
                            {
                                it = new AddressItemToken(AddressItemType.Detail, t, br.EndToken);
                                res.Add(it);
                            }
                            t = br.EndToken;
                            continue;
                        }
                    }
                    bool checkKv = false;
                    if (t.IsValue("КВ", null) || t.IsValue("KB", null)) 
                    {
                        if (it.Typ == AddressItemType.Number && res.Count > 1 && res[res.Count - 2].Typ == AddressItemType.Street) 
                            checkKv = true;
                        else if ((it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Corpus) || it.Typ == AddressItemType.CorpusOrFlat) 
                        {
                            for (int jj = res.Count - 2; jj >= 0; jj--) 
                            {
                                if (res[jj].Typ == AddressItemType.Street || res[jj].Typ == AddressItemType.City) 
                                    checkKv = true;
                            }
                        }
                        if (checkKv) 
                        {
                            Pullenti.Ner.Token tt2 = t.Next;
                            if (tt2 != null && tt2.IsChar('.')) 
                                tt2 = tt2.Next;
                            AddressItemToken it22 = TryParsePureItem(tt2, null, null);
                            if (it22 != null && it22.Typ == AddressItemType.Number) 
                            {
                                it22 = it22.Clone();
                                it22.BeginToken = t;
                                it22.Typ = AddressItemType.Flat;
                                res.Add(it22);
                                t = it22.EndToken;
                                continue;
                            }
                        }
                    }
                    if (res[res.Count - 1].Typ == AddressItemType.City) 
                    {
                        if (((t.IsHiphen || t.IsChar('_') || t.IsValue("НЕТ", null))) && t.Next != null && t.Next.IsComma) 
                        {
                            AddressItemToken att = TryParsePureItem(t.Next.Next, null, null);
                            if (att != null) 
                            {
                                if (att.Typ == AddressItemType.House || att.Typ == AddressItemType.Building || att.Typ == AddressItemType.Corpus) 
                                {
                                    it = new AddressItemToken(AddressItemType.Street, t, t);
                                    res.Add(it);
                                    continue;
                                }
                            }
                        }
                    }
                    if (t.LengthChar == 2 && (t is Pullenti.Ner.TextToken) && t.Chars.IsAllUpper) 
                    {
                        string term = (t as Pullenti.Ner.TextToken).Term;
                        if (!string.IsNullOrEmpty(term) && term[0] == 'Р') 
                            continue;
                    }
                    break;
                }
                if (t.WhitespacesBeforeCount > 15) 
                {
                    if (it0.Typ == AddressItemType.Street && last.Typ == AddressItemType.City) 
                    {
                    }
                    else 
                        break;
                }
                if (t.IsNewlineBefore && it0.Typ == AddressItemType.Street && it0.RefToken != null) 
                {
                    if (!it0.RefTokenIsGsk) 
                        break;
                }
                if (it0.Typ == AddressItemType.Street && t.IsValue("КВ", null)) 
                {
                    if (it != null) 
                    {
                        if (it.Typ == AddressItemType.House || it.Typ == AddressItemType.Building || it.Typ == AddressItemType.Corpus) 
                        {
                            AddressItemToken it2 = TryParsePureItem(t, null, null);
                            if (it2 != null && it2.Typ == AddressItemType.Flat) 
                                it0 = it2;
                        }
                    }
                }
                if (it0.Typ == AddressItemType.Prefix) 
                    break;
                if (it0.Typ == AddressItemType.Number) 
                {
                    if (string.IsNullOrEmpty(it0.Value)) 
                        break;
                    if (!char.IsDigit(it0.Value[0])) 
                        break;
                    int cou = 0;
                    for (int i = res.Count - 1; i >= 0; i--) 
                    {
                        if (res[i].Typ == AddressItemType.Number) 
                            cou++;
                        else 
                            break;
                    }
                    if (cou > 5) 
                        break;
                    if (it.IsDoubt && t.IsNewlineBefore) 
                        break;
                }
                if (it0.Typ == AddressItemType.CorpusOrFlat && it != null && it.Typ == AddressItemType.Flat) 
                    it0.Typ = AddressItemType.Office;
                if (((((it0.Typ == AddressItemType.Floor || it0.Typ == AddressItemType.Potch || it0.Typ == AddressItemType.Block) || it0.Typ == AddressItemType.Kilometer)) && string.IsNullOrEmpty(it0.Value) && it.Typ == AddressItemType.Number) && it.EndToken.Next == it0.BeginToken) 
                {
                    it = it.Clone();
                    res[res.Count - 1] = it;
                    it.Typ = it0.Typ;
                    it.EndToken = it0.EndToken;
                }
                else if ((((it.Typ == AddressItemType.Floor || it.Typ == AddressItemType.Potch)) && string.IsNullOrEmpty(it.Value) && it0.Typ == AddressItemType.Number) && it.EndToken.Next == it0.BeginToken) 
                {
                    it = it.Clone();
                    res[res.Count - 1] = it;
                    it.Value = it0.Value;
                    it.EndToken = it0.EndToken;
                }
                else 
                {
                    it = it0;
                    res.Add(it);
                }
                t = it.EndToken;
            }
            if (res.Count > 0) 
            {
                it = res[res.Count - 1];
                AddressItemToken it0 = (res.Count > 1 ? res[res.Count - 2] : null);
                if (it.Typ == AddressItemType.Number && it0 != null && it0.RefToken != null) 
                {
                    foreach (Pullenti.Ner.Slot s in it0.RefToken.Referent.Slots) 
                    {
                        if (s.TypeName == "TYPE") 
                        {
                            string ss = s.Value as string;
                            if (ss.Contains("гараж") || ((ss[0] == 'Г' && ss[ss.Length - 1] == 'К'))) 
                            {
                                it.Typ = AddressItemType.Box;
                                break;
                            }
                        }
                    }
                }
                if (it.Typ == AddressItemType.Number || it.Typ == AddressItemType.Zip) 
                {
                    bool del = false;
                    if (it.BeginToken.Previous != null && it.BeginToken.Previous.Morph.Class.IsPreposition) 
                        del = true;
                    else if (it.Morph.Class.IsNoun) 
                        del = true;
                    if ((!del && it.EndToken.WhitespacesAfterCount == 1 && it.WhitespacesBeforeCount > 0) && it.Typ == AddressItemType.Number) 
                    {
                        Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Geo.Internal.MiscLocationHelper.TryParseNpt(it.EndToken.Next);
                        if (npt != null) 
                            del = true;
                    }
                    if (del) 
                        res.RemoveAt(res.Count - 1);
                    else if ((it.Typ == AddressItemType.Number && it0 != null && it0.Typ == AddressItemType.Street) && it0.RefToken == null) 
                    {
                        if (it.BeginToken.Previous.IsChar(',') || it.IsNewlineAfter) 
                        {
                            it = it.Clone();
                            res[res.Count - 1] = it;
                            it.Typ = AddressItemType.House;
                            it.IsDoubt = true;
                        }
                    }
                }
            }
            if (res.Count == 0) 
                return null;
            foreach (AddressItemToken r in res) 
            {
                if (r.Typ == AddressItemType.City) 
                {
                    AddressItemToken ty = _findAddrTyp(r.BeginToken, r.EndChar, 0);
                    if (ty != null) 
                    {
                        if (r.DetailType == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            r.DetailType = ty.DetailType;
                        if (ty.DetailMeters > 0) 
                            r.DetailMeters = ty.DetailMeters;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 2); i++) 
            {
                if (res[i].Typ == AddressItemType.Street && res[i + 1].Typ == AddressItemType.Number) 
                {
                    if ((res[i + 2].Typ == AddressItemType.Building || res[i + 2].Typ == AddressItemType.Corpus || res[i + 2].Typ == AddressItemType.Office) || res[i + 2].Typ == AddressItemType.Flat) 
                    {
                        res[i + 1] = res[i + 1].Clone();
                        res[i + 1].Typ = AddressItemType.House;
                    }
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i].Typ == AddressItemType.Street && res[i + 1].Typ == AddressItemType.Kilometer && (res[i].Referent is Pullenti.Ner.Address.StreetReferent)) && (res[i].Referent as Pullenti.Ner.Address.StreetReferent).Number == null) 
                {
                    res[i] = res[i].Clone();
                    (res[i].Referent as Pullenti.Ner.Address.StreetReferent).Number = res[i + 1].Value + "км";
                    res[i].EndToken = res[i + 1].EndToken;
                    res.RemoveAt(i + 1);
                }
            }
            for (int i = 0; i < (res.Count - 1); i++) 
            {
                if ((res[i + 1].Typ == AddressItemType.Street && res[i].Typ == AddressItemType.Kilometer && (res[i + 1].Referent is Pullenti.Ner.Address.StreetReferent)) && (res[i + 1].Referent as Pullenti.Ner.Address.StreetReferent).Number == null) 
                {
                    res[i + 1] = res[i + 1].Clone();
                    (res[i + 1].Referent as Pullenti.Ner.Address.StreetReferent).Number = res[i].Value + "км";
                    res[i + 1].BeginToken = res[i].BeginToken;
                    res.RemoveAt(i);
                    break;
                }
            }
            while (res.Count > 0) 
            {
                AddressItemToken last = res[res.Count - 1];
                if (last.Typ != AddressItemType.Street || !(last.RefToken is Pullenti.Ner.Geo.Internal.OrgItemToken)) 
                    break;
                if ((last.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken).IsGsk || (last.RefToken as Pullenti.Ner.Geo.Internal.OrgItemToken).HasTerrKeyword) 
                    break;
                res.RemoveAt(res.Count - 1);
            }
            return res;
        }
    }
}