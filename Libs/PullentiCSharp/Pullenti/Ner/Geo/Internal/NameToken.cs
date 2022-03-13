/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Geo.Internal
{
    class NameToken : Pullenti.Ner.MetaToken
    {
        public NameToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public string Name;
        public string Number;
        public string Pref;
        public bool IsDoubt;
        int m_lev;
        NameTokenType m_typ;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (IsDoubt) 
                res.Append("? ");
            if (Pref != null) 
                res.AppendFormat("{0} ", Pref);
            if (Name != null) 
                res.AppendFormat("\"{0}\"", Name);
            if (Number != null) 
                res.AppendFormat(" N{0}", Number);
            return res.ToString();
        }
        public static NameToken TryParse(Pullenti.Ner.Token t, NameTokenType ty, int lev)
        {
            if (t == null || lev > 3) 
                return null;
            Pullenti.Ner.Core.BracketSequenceToken br = Pullenti.Ner.Core.BracketHelper.TryParse(t, Pullenti.Ner.Core.BracketParseAttr.No, 100);
            NameToken res = null;
            Pullenti.Ner.Token ttt;
            Pullenti.Ner.NumberToken num;
            Pullenti.Ner.Core.TerminToken ttok;
            if (br != null) 
            {
                NameToken nam = TryParse(t.Next, ty, lev + 1);
                if (nam != null && nam.EndToken.Next == br.EndToken) 
                {
                    res = nam;
                    nam.BeginToken = t;
                    nam.EndToken = br.EndToken;
                    res.IsDoubt = false;
                }
                else 
                {
                    res = new NameToken(t, br.EndToken);
                    Pullenti.Ner.Token tt = br.EndToken.Previous;
                    if (tt is Pullenti.Ner.NumberToken) 
                    {
                        res.Number = (tt as Pullenti.Ner.NumberToken).Value;
                        tt = tt.Previous;
                        if (tt != null && tt.IsHiphen) 
                            tt = tt.Previous;
                    }
                    if (tt != null && tt.BeginChar > br.BeginChar) 
                        res.Name = Pullenti.Ner.Core.MiscHelper.GetTextValue(t.Next, tt, Pullenti.Ner.Core.GetTextAttr.No);
                }
            }
            else if ((t is Pullenti.Ner.ReferentToken) && (t as Pullenti.Ner.ReferentToken).BeginToken == (t as Pullenti.Ner.ReferentToken).EndToken && !(t as Pullenti.Ner.ReferentToken).BeginToken.Chars.IsAllLower) 
            {
                res = new NameToken(t, t) { IsDoubt = true };
                res.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(t as Pullenti.Ner.ReferentToken, Pullenti.Ner.Core.GetTextAttr.No);
            }
            else if ((((ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(t)))) is Pullenti.Ner.NumberToken) 
            {
                res = new NameToken(t, ttt) { Number = (ttt as Pullenti.Ner.NumberToken).Value };
                if (ttt.WhitespacesAfterCount < 2) 
                {
                    NameToken nam = TryParse(ttt.Next, ty, lev + 1);
                    if (nam != null && nam.Name != null && nam.Number == null) 
                    {
                        res.Name = nam.Name;
                        res.EndToken = nam.EndToken;
                    }
                }
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAge(t)))) != null) 
                res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(t)))) != null) 
                res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
            else if (t is Pullenti.Ner.NumberToken) 
            {
                Pullenti.Ner.Core.NumberExToken nn = Pullenti.Ner.Core.NumberHelper.TryParseNumberWithPostfix(t);
                if (nn != null) 
                {
                    if (nn.ExTyp != Pullenti.Ner.Core.NumberExType.Undefined) 
                        return null;
                }
                res = new NameToken(t, t) { Number = (t as Pullenti.Ner.NumberToken).Value };
            }
            else if (t.IsHiphen && (t.Next is Pullenti.Ner.NumberToken)) 
            {
                num = Pullenti.Ner.Core.NumberHelper.TryParseAge(t.Next);
                if (num == null) 
                    num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(t.Next);
                if (num != null) 
                    res = new NameToken(t, num.EndToken) { Pref = (num as Pullenti.Ner.NumberToken).Value + " ЛЕТ" };
                else 
                    res = new NameToken(t, t.Next) { Number = (t.Next as Pullenti.Ner.NumberToken).Value, IsDoubt = true };
            }
            else if ((t is Pullenti.Ner.ReferentToken) && t.GetReferent().TypeName == "DATE") 
            {
                string year = t.GetReferent().GetStringValue("YEAR");
                if (year != null) 
                    res = new NameToken(t, t) { Pref = year + " ГОДА" };
                else 
                {
                    string mon = t.GetReferent().GetStringValue("MONTH");
                    string day = t.GetReferent().GetStringValue("DAY");
                    if (day != null && mon == null && t.GetReferent().ParentReferent != null) 
                        mon = t.GetReferent().ParentReferent.GetStringValue("MONTH");
                    if (mon != null) 
                        res = new NameToken(t, t) { Name = t.GetReferent().ToString().ToUpper() };
                }
            }
            else if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            else if (t.LengthChar == 1) 
            {
                if ((((ty != NameTokenType.Org && ty != NameTokenType.Strong)) || !t.Chars.IsAllUpper || t.IsWhitespaceAfter) || !t.Chars.IsLetter) 
                    return null;
                NameToken next = TryParse(t.Next, ty, lev + 1);
                if (next != null && next.Number != null && next.Name == null) 
                {
                    res = next;
                    res.BeginToken = t;
                    res.Name = (t as Pullenti.Ner.TextToken).Term;
                }
                else if (t.Next != null && t.Next.IsChar('.')) 
                {
                    StringBuilder nam = new StringBuilder();
                    nam.Append((t as Pullenti.Ner.TextToken).Term);
                    Pullenti.Ner.Token t1 = t.Next;
                    for (Pullenti.Ner.Token tt = t1.Next; tt != null; tt = tt.Next) 
                    {
                        if (!(tt is Pullenti.Ner.TextToken) || tt.LengthChar != 1 || !tt.Chars.IsLetter) 
                            break;
                        if (tt.Next == null || !tt.Next.IsChar('.')) 
                            break;
                        nam.Append((tt as Pullenti.Ner.TextToken).Term);
                        tt = tt.Next;
                        t1 = tt;
                    }
                    if (nam.Length >= 3) 
                        res = new NameToken(t, t1) { Name = nam.ToString() };
                    else 
                    {
                        Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("PERSON", t, null);
                        if (rt != null) 
                        {
                            res = new NameToken(t, rt.EndToken) { Name = rt.Referent.GetStringValue("LASTNAME") };
                            if (res.Name == null) 
                                res.Name = rt.Referent.ToString(false, null, 0).ToUpper();
                            else 
                                for (Pullenti.Ner.Token tt = t; tt != null && tt.EndChar <= rt.EndChar; tt = tt.Next) 
                                {
                                    if ((tt is Pullenti.Ner.TextToken) && tt.IsValue(res.Name, null)) 
                                    {
                                        res.Name = (tt as Pullenti.Ner.TextToken).Term;
                                        break;
                                    }
                                }
                        }
                    }
                }
            }
            else if ((t as Pullenti.Ner.TextToken).Term == "ИМЕНИ" || (t as Pullenti.Ner.TextToken).Term == "ИМ") 
            {
                Pullenti.Ner.Token tt = t.Next;
                if (t.IsValue("ИМ", null) && tt != null && tt.IsChar('.')) 
                    tt = tt.Next;
                NameToken nam = TryParse(tt, NameTokenType.Strong, lev + 1);
                if (nam != null) 
                {
                    nam.BeginToken = t;
                    nam.IsDoubt = false;
                    res = nam;
                }
            }
            else if ((((ttok = m_Onto.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No)))) != null) 
                res = new NameToken(t, ttok.EndToken) { Name = ttok.Termin.CanonicText };
            else 
            {
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null && npt.BeginToken == npt.EndToken) 
                    npt = null;
                if (npt != null && npt.EndToken.Chars.IsAllLower) 
                {
                    if (t.Chars.IsAllLower) 
                        npt = null;
                    else if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(npt.EndToken)) 
                        npt = null;
                }
                if (npt != null) 
                    res = new NameToken(t, npt.EndToken) { Morph = npt.Morph, Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(npt, Pullenti.Ner.Core.GetTextAttr.No).Replace("-", " ") };
                else if (!t.Chars.IsAllLower || t.IsValue("МЕСТНОСТЬ", null)) 
                {
                    if (TerrItemToken.CheckKeyword(t) != null) 
                        return null;
                    res = new NameToken(t, t) { Name = (t as Pullenti.Ner.TextToken).Term, Morph = t.Morph };
                    if ((((Pullenti.Morph.LanguageHelper.EndsWith(res.Name, "ОВ") || Pullenti.Morph.LanguageHelper.EndsWith(res.Name, "ВО"))) && (t.Next is Pullenti.Ner.TextToken) && !t.Next.Chars.IsAllLower) && t.Next.LengthChar > 1 && !t.Next.GetMorphClassInDictionary().IsUndefined) 
                    {
                        if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t.Next)) 
                        {
                        }
                        else if (OrgTypToken.TryParse(t.Next, false, null) != null) 
                        {
                        }
                        else 
                        {
                            res.EndToken = t.Next;
                            res.Name = string.Format("{0} {1}", res.Name, (t.Next as Pullenti.Ner.TextToken).Term);
                            res.Morph = t.Next.Morph;
                        }
                    }
                    if ((t.WhitespacesAfterCount < 2) && (t.Next is Pullenti.Ner.TextToken) && t.Next.Chars.IsLetter) 
                    {
                        bool ok = false;
                        if (t.Next.LengthChar >= 3 && t.Next.GetMorphClassInDictionary().IsUndefined) 
                            ok = true;
                        else 
                        {
                            bool ok1 = false;
                            if ((((t.Next.LengthChar < 4) || t.GetMorphClassInDictionary().IsUndefined)) && t.Next.Chars.Equals(t.Chars)) 
                                ok1 = true;
                            else if (t.IsValue("МЕСТНОСТЬ", null) && !t.Next.Chars.IsAllLower) 
                                ok = true;
                            if (ok1) 
                            {
                                NameToken next = TryParse(t.Next, ty, lev + 1);
                                if (next == null || next.BeginToken == next.EndToken) 
                                    ok = true;
                            }
                        }
                        if (!ok && t.Next.GetMorphClassInDictionary().IsAdjective) 
                        {
                            Pullenti.Morph.MorphClass mc = t.GetMorphClassInDictionary();
                            if (mc.IsNoun || mc.IsProperGeo) 
                            {
                                if (((t.Morph.Gender & t.Next.Morph.Gender)) != Pullenti.Morph.MorphGender.Undefined) 
                                {
                                    Pullenti.Ner.Token tt = t.Next.Next;
                                    if (tt == null) 
                                        ok = true;
                                    else if (tt.IsComma || tt.IsNewlineAfter) 
                                        ok = true;
                                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(tt, false, false)) 
                                        ok = true;
                                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(tt, false)) 
                                        ok = true;
                                }
                            }
                        }
                        if (ok) 
                        {
                            if (OrgTypToken.TryParse(t.Next, false, null) != null) 
                                ok = false;
                        }
                        if (ok) 
                        {
                            res.EndToken = t.Next;
                            res.Name = string.Format("{0} {1}", res.Name, (t.Next as Pullenti.Ner.TextToken).Term);
                        }
                    }
                }
                if (res != null && res.EndToken.IsValue("УСАДЬБА", null) && (res.WhitespacesAfterCount < 2)) 
                {
                    NameToken res1 = TryParse(res.EndToken.Next, ty, lev + 1);
                    if (res1 != null && res1.Name != null) 
                    {
                        res.EndToken = res1.EndToken;
                        res.Name = string.Format("{0} {1}", res.Name, res1.Name);
                    }
                }
            }
            if (res == null || res.WhitespacesAfterCount > 2) 
                return res;
            ttt = res.EndToken.Next;
            if (ttt != null && ttt.IsHiphen) 
            {
                num = Pullenti.Ner.Core.NumberHelper.TryParseAge(ttt.Next);
                if (num == null) 
                    num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(ttt.Next);
                if (num != null) 
                {
                    res.Pref = num.Value + " ЛЕТ";
                    res.EndToken = num.EndToken;
                }
                else if ((ttt.Next is Pullenti.Ner.NumberToken) && res.Number == null) 
                {
                    res.Number = (ttt.Next as Pullenti.Ner.NumberToken).Value;
                    res.EndToken = ttt.Next;
                }
                if ((ttt.Next is Pullenti.Ner.TextToken) && !ttt.IsWhitespaceAfter && res.Name != null) 
                {
                    res.Name = string.Format("{0} {1}", res.Name, (ttt.Next as Pullenti.Ner.TextToken).Term);
                    res.EndToken = ttt.Next;
                }
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAge(ttt)))) != null) 
            {
                res.Pref = num.Value + " ЛЕТ";
                res.EndToken = num.EndToken;
            }
            else if ((((num = Pullenti.Ner.Core.NumberHelper.TryParseAnniversary(ttt)))) != null) 
            {
                res.Pref = num.Value + " ЛЕТ";
                res.EndToken = num.EndToken;
            }
            else if (ttt is Pullenti.Ner.NumberToken) 
            {
                bool ok = false;
                if (ty == NameTokenType.Org) 
                    ok = true;
                if (ok) 
                {
                    if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(ttt.Next)) 
                        ok = false;
                    else if (ttt.Next != null) 
                    {
                        if (ttt.Next.IsValue("КМ", null) || ttt.Next.IsValue("КИЛОМЕТР", null)) 
                            ok = false;
                    }
                }
                if (ok) 
                {
                    res.Number = (ttt as Pullenti.Ner.NumberToken).Value;
                    res.EndToken = ttt;
                }
            }
            if (res.Number == null) 
            {
                ttt = Pullenti.Ner.Core.MiscHelper.CheckNumberPrefix(res.EndToken.Next);
                if (ttt is Pullenti.Ner.NumberToken) 
                {
                    res.Number = (ttt as Pullenti.Ner.NumberToken).Value;
                    res.EndToken = ttt;
                }
            }
            if ((res.WhitespacesAfterCount < 3) && res.Name == null && Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(res.EndToken.Next, false, false)) 
            {
                NameToken nam = TryParse(res.EndToken.Next, ty, lev + 1);
                if (nam != null) 
                {
                    res.Name = nam.Name;
                    res.EndToken = nam.EndToken;
                    res.IsDoubt = false;
                }
            }
            if (res.Pref != null && res.Name == null && res.Number == null) 
            {
                NameToken nam = TryParse(res.EndToken.Next, ty, lev + 1);
                if (nam != null && nam.Name != null && nam.Pref == null) 
                {
                    res.Name = nam.Name;
                    res.Number = nam.Number;
                    res.EndToken = nam.EndToken;
                }
            }
            res.m_lev = lev;
            res.m_typ = ty;
            if (res.WhitespacesAfterCount < 3) 
            {
                Pullenti.Ner.Core.TerminToken nn = m_Onto.TryParse(res.EndToken.Next, Pullenti.Ner.Core.TerminParseAttr.No);
                if (nn != null) 
                {
                    res.EndToken = nn.EndToken;
                    res.Name = string.Format("{0} {1}", res.Name, Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nn, Pullenti.Ner.Core.GetTextAttr.No));
                }
            }
            res.TryAttachNumber();
            return res;
        }
        public void TryAttachNumber()
        {
            if (WhitespacesAfterCount > 2) 
                return;
            if (Number == null) 
            {
                NameToken nam2 = TryParse(EndToken.Next, m_typ, m_lev + 1);
                if ((nam2 != null && nam2.Number != null && nam2.Name == null) && nam2.Pref == null) 
                {
                    if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(nam2.EndToken.Next)) 
                    {
                    }
                    else 
                    {
                        Number = nam2.Number;
                        EndToken = nam2.EndToken;
                    }
                }
            }
            if ((m_typ == NameTokenType.Org && (EndToken is Pullenti.Ner.NumberToken) && Number == (EndToken as Pullenti.Ner.NumberToken).Value) && !IsWhitespaceAfter) 
            {
                StringBuilder tmp = new StringBuilder(Number);
                string delim = null;
                for (Pullenti.Ner.Token tt = EndToken.Next; tt != null; tt = tt.Next) 
                {
                    if (tt.IsWhitespaceBefore) 
                        break;
                    if (tt.IsCharOf(",.") || tt.IsTableControlChar) 
                        break;
                    if (tt.IsCharOf("\\/")) 
                    {
                        delim = "/";
                        continue;
                    }
                    else if (tt.IsHiphen) 
                    {
                        delim = "-";
                        continue;
                    }
                    if ((tt is Pullenti.Ner.NumberToken) && (tt as Pullenti.Ner.NumberToken).Typ == Pullenti.Ner.NumberSpellingType.Digit) 
                    {
                        if (delim != null && char.IsDigit(tmp[tmp.Length - 1])) 
                            tmp.Append(delim);
                        delim = null;
                        tmp.Append((tt as Pullenti.Ner.NumberToken).Value);
                        EndToken = tt;
                        continue;
                    }
                    if ((tt is Pullenti.Ner.TextToken) && tt.LengthChar == 1 && tt.Chars.IsLetter) 
                    {
                        if (delim != null && char.IsLetter(tmp[tmp.Length - 1])) 
                            tmp.Append(delim);
                        delim = null;
                        tmp.Append((tt as Pullenti.Ner.TextToken).Term);
                        EndToken = tt;
                        continue;
                    }
                    break;
                }
                Number = tmp.ToString();
            }
        }
        static Pullenti.Ner.Core.TerminCollection m_Onto;
        public static void Initialize()
        {
            m_Onto = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("СОВЕТСКОЙ АРМИИ И ВОЕННО МОРСКОГО ФЛОТА");
            t.AddVariant("СА И ВМФ", false);
            m_Onto.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВЕТСКОЙ АРМИИ") { Acronym = "СА" };
            m_Onto.Add(t);
            t = new Pullenti.Ner.Core.Termin("МИНИСТЕРСТВО ОБОРОНЫ") { Acronym = "МО" };
            m_Onto.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОЕННО МОРСКОЙ ФЛОТ") { Acronym = "ВМФ" };
            m_Onto.Add(t);
            m_Onto.Add(new Pullenti.Ner.Core.Termin("МОЛОДАЯ ГВАРДИЯ"));
            m_Onto.Add(new Pullenti.Ner.Core.Termin("ЗАЩИТНИКИ БЕЛОГО ДОМА"));
        }
    }
}