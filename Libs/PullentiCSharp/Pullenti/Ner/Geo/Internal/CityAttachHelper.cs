/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Geo.Internal
{
    static class CityAttachHelper
    {
        public static Pullenti.Ner.ReferentToken TryDefine(List<CityItemToken> li, GeoAnalyzerData ad, bool always = false)
        {
            if (li == null) 
                return null;
            Pullenti.Ner.Core.IntOntologyItem oi;
            if (li.Count > 2 && li[0].Typ == CityItemToken.ItemType.Misc && li[1].Typ == CityItemToken.ItemType.Noun) 
            {
                li[1].Doubtful = false;
                li.RemoveAt(0);
            }
            Pullenti.Ner.ReferentToken res = null;
            if (res == null && li.Count > 1) 
            {
                res = Try4(li);
                if (res != null && res.EndChar <= li[1].EndChar) 
                    res = null;
            }
            if (res == null) 
                res = Try1(li, out oi, ad);
            if (res == null) 
                res = _tryNounName(li, out oi, false);
            if (res == null) 
                res = _tryNameExist(li, out oi, false);
            if (res == null) 
                res = Try4(li);
            if (res == null && always) 
                res = _tryNounName(li, out oi, true);
            if (res == null && always) 
            {
                if (OrgItemToken.TryParse(li[0].BeginToken, ad) != null) 
                {
                }
                else 
                    res = _tryNameExist(li, out oi, true);
            }
            if ((res == null && li.Count == 1 && li[0].Typ == CityItemToken.ItemType.Noun) && li[0].GeoObjectBefore && (li[0].WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.ReferentToken rt = li[0].EndToken.Next as Pullenti.Ner.ReferentToken;
                if ((rt != null && rt.BeginToken == rt.EndToken && !rt.BeginToken.Chars.IsAllLower) && (rt.BeginToken is Pullenti.Ner.TextToken)) 
                {
                    CityItemToken nam = CityItemToken.TryParse(rt.BeginToken, null, false, null);
                    if (nam != null && nam.EndToken == rt.EndToken && ((nam.Typ == CityItemToken.ItemType.ProperName || nam.Typ == CityItemToken.ItemType.City))) 
                    {
                        Pullenti.Ner.Token t = li[0].Kit.DebedToken(rt);
                        Pullenti.Ner.Geo.GeoReferent g = new Pullenti.Ner.Geo.GeoReferent();
                        g.AddTyp(li[0].Value);
                        g.AddName(nam.Value);
                        g.AddName(nam.AltValue);
                        return new Pullenti.Ner.ReferentToken(g, li[0].BeginToken, rt.EndToken);
                    }
                }
            }
            if (res == null) 
                return null;
            if (res != null && res.Morph != null) 
            {
            }
            if (res.BeginToken.Previous is Pullenti.Ner.TextToken) 
            {
                if (res.BeginToken.Previous.IsValue("ТЕРРИТОРИЯ", null)) 
                {
                    res.BeginToken = res.BeginToken.Previous;
                    res.Morph = res.BeginToken.Morph;
                }
                if ((Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(res.BeginToken.Previous, false, false) && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(res.EndToken.Next, false, null, false) && res.BeginToken.Previous.Previous != null) && res.BeginToken.Previous.Previous.IsValue("ТЕРРИТОРИЯ", null)) 
                {
                    res.BeginToken = res.BeginToken.Previous.Previous;
                    res.Morph = res.BeginToken.Morph;
                    res.EndToken = res.EndToken.Next;
                }
            }
            return res;
        }
        static Pullenti.Ner.ReferentToken Try1(List<CityItemToken> li, out Pullenti.Ner.Core.IntOntologyItem oi, Pullenti.Ner.Core.AnalyzerDataWithOntology ad)
        {
            oi = null;
            if (li == null || (li.Count < 1)) 
                return null;
            else if (li[0].Typ != CityItemToken.ItemType.City) 
            {
                if (li.Count == 1) 
                    return null;
                if (li[0].Typ != CityItemToken.ItemType.ProperName || li[1].Typ != CityItemToken.ItemType.Noun) 
                    return null;
                if (li.Count == 2) 
                {
                }
                else if (li.Count == 3 && li[2].Typ == CityItemToken.ItemType.ProperName) 
                {
                    if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(li[1].EndToken, false)) 
                    {
                    }
                    else 
                        return null;
                }
                else 
                    return null;
            }
            int i = 1;
            oi = li[0].OntoItem;
            bool ok = !li[0].Doubtful;
            if ((ok && li[0].OntoItem != null && li[0].OntoItem.MiscAttr == null) && ad != null) 
            {
                if (li[0].OntoItem.Owner != ad.LocalOntology && !li[0].OntoItem.Owner.IsExtOntology) 
                {
                    if (li[0].BeginToken.Previous != null && li[0].BeginToken.Previous.IsValue("В", null)) 
                    {
                    }
                    else 
                        ok = false;
                }
            }
            if (li.Count == 1 && li[0].BeginToken.Morph.Class.IsAdjective) 
            {
                List<Pullenti.Ner.Address.Internal.StreetItemToken> sits = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(li[0].BeginToken, 3, null);
                if (sits != null && sits.Count == 2 && sits[1].Typ == Pullenti.Ner.Address.Internal.StreetItemType.Noun) 
                    return null;
            }
            string typ = null;
            string alttyp = null;
            Pullenti.Ner.MorphCollection mc = li[0].Morph;
            if (i < li.Count) 
            {
                if (li[i].Typ == CityItemToken.ItemType.Noun) 
                {
                    Pullenti.Ner.Address.Internal.AddressItemToken at = null;
                    if (!li[i].Chars.IsAllLower && (li[i].WhitespacesAfterCount < 2)) 
                    {
                        if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(li[i].EndToken.Next)) 
                        {
                            at = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(li[i].BeginToken, false, null, null);
                            if (at != null) 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken at2 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(li[i].EndToken.Next, false, null, null);
                                if (at2 != null && at2.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                                    at = null;
                            }
                        }
                    }
                    if (at == null) 
                    {
                        typ = li[i].Value;
                        alttyp = li[i].AltValue;
                        if (li[i].BeginToken.IsValue("СТ", null) && li[i].BeginToken.Chars.IsAllUpper) 
                            return null;
                        if ((i + 1) == li.Count) 
                        {
                            if (li[i].Doubtful) 
                            {
                                if (li[0].Chars.IsLatinLetter) 
                                    return null;
                                Pullenti.Ner.ReferentToken rt1 = li[0].Kit.ProcessReferent("PERSON", li[0].BeginToken, null);
                                if (rt1 != null && rt1.Referent.TypeName == "PERSON") 
                                    return null;
                            }
                            ok = true;
                            if (!li[i].Morph.Case.IsUndefined) 
                                mc = li[i].Morph;
                            i++;
                        }
                        else if (ok) 
                            i++;
                        else 
                        {
                            Pullenti.Ner.Token tt0 = li[0].BeginToken.Previous;
                            if ((tt0 is Pullenti.Ner.TextToken) && (tt0.WhitespacesAfterCount < 3)) 
                            {
                                if (tt0.IsValue("МЭР", "МЕР") || tt0.IsValue("ГЛАВА", null) || tt0.IsValue("ГРАДОНАЧАЛЬНИК", null)) 
                                {
                                    ok = true;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            if (!ok && oi != null && (oi.CanonicText.Length < 4)) 
                return null;
            if (!ok && li[0].BeginToken.Morph.Class.IsProperName) 
                return null;
            if (!ok) 
            {
                if (!Pullenti.Ner.Core.MiscHelper.IsExistsInDictionary(li[0].BeginToken, li[0].EndToken, Pullenti.Morph.MorphClass.Adjective | Pullenti.Morph.MorphClass.Noun | Pullenti.Morph.MorphClass.Pronoun)) 
                {
                    ok = li[0].GeoObjectBefore || li[i - 1].GeoObjectAfter;
                    if (ok && li[0].BeginToken == li[0].EndToken) 
                    {
                        Pullenti.Morph.MorphClass mcc = li[0].BeginToken.GetMorphClassInDictionary();
                        if (mcc.IsProperName || mcc.IsProperSurname) 
                            ok = false;
                        else if (li[0].GeoObjectBefore && (li[0].WhitespacesAfterCount < 2)) 
                        {
                            Pullenti.Ner.Address.Internal.AddressItemToken ad1 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(li[0].BeginToken, false, null, null);
                            if (ad1 != null && ad1.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken ad2 = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(li[0].EndToken.Next, false, null, null);
                                if (ad2 == null || ad2.Typ != Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                                    ok = false;
                            }
                            else if (OrgItemToken.TryParse(li[0].BeginToken, null) != null) 
                                ok = false;
                        }
                    }
                }
                if (ok) 
                {
                    if (li[0].Kit.ProcessReferent("PERSON", li[0].BeginToken, null) != null) 
                        ok = false;
                }
            }
            if (!ok) 
                ok = CheckYearAfter(li[0].EndToken.Next);
            if (!ok && ((!li[0].BeginToken.Morph.Class.IsAdjective || li[0].BeginToken != li[0].EndToken))) 
                ok = CheckCityAfter(li[0].EndToken.Next);
            if (!ok) 
                return null;
            if (i < li.Count) 
                li.RemoveRange(i, li.Count - i);
            Pullenti.Ner.ReferentToken rt = null;
            if (oi == null) 
            {
                if (li[0].Value != null && li[0].HigherGeo != null) 
                {
                    Pullenti.Ner.Geo.GeoReferent cap = new Pullenti.Ner.Geo.GeoReferent();
                    cap.AddName(li[0].Value);
                    cap.AddTypCity(li[0].Kit.BaseLanguage);
                    cap.Higher = li[0].HigherGeo;
                    if (typ != null) 
                        cap.AddTyp(typ);
                    if (alttyp != null) 
                        cap.AddTyp(alttyp);
                    rt = new Pullenti.Ner.ReferentToken(cap, li[0].BeginToken, li[0].EndToken);
                }
                else 
                {
                    if (li[0].Value == null) 
                        return null;
                    if (typ == null) 
                    {
                        if ((li.Count == 1 && li[0].BeginToken.Previous != null && li[0].BeginToken.Previous.IsHiphen) && (li[0].BeginToken.Previous.Previous is Pullenti.Ner.ReferentToken) && (li[0].BeginToken.Previous.Previous.GetReferent() is Pullenti.Ner.Geo.GeoReferent)) 
                        {
                        }
                        else 
                            return null;
                    }
                    else 
                    {
                        if (!Pullenti.Morph.LanguageHelper.EndsWithEx(typ, "ПУНКТ", "ПОСЕЛЕНИЕ", "ПОСЕЛЕННЯ", "ПОСЕЛОК")) 
                        {
                            if (!Pullenti.Morph.LanguageHelper.EndsWith(typ, "CITY")) 
                            {
                                if (typ == "СТАНЦИЯ" && (MiscLocationHelper.CheckGeoObjectBefore(li[0].BeginToken, false))) 
                                {
                                }
                                else if (li.Count > 1 && li[1].Typ == CityItemToken.ItemType.Noun && li[0].Typ == CityItemToken.ItemType.City) 
                                {
                                }
                                else if ((li.Count == 2 && li[1].Typ == CityItemToken.ItemType.Noun && li[0].Typ == CityItemToken.ItemType.ProperName) && ((li[0].GeoObjectBefore || li[1].GeoObjectAfter))) 
                                {
                                }
                                else 
                                    return null;
                            }
                        }
                        if (li[0].BeginToken.Morph.Class.IsAdjective) 
                            li[0].Value = Pullenti.Ner.Core.ProperNameHelper.GetNameEx(li[0].BeginToken, li[0].EndToken, Pullenti.Morph.MorphClass.Adjective, li[1].Morph.Case, li[1].Morph.Gender, false, false);
                    }
                }
            }
            else if (oi.Referent is Pullenti.Ner.Geo.GeoReferent) 
            {
                Pullenti.Ner.Geo.GeoReferent city = oi.Referent.Clone() as Pullenti.Ner.Geo.GeoReferent;
                city.Occurrence.Clear();
                rt = new Pullenti.Ner.ReferentToken(city, li[0].BeginToken, li[li.Count - 1].EndToken) { Morph = mc };
            }
            else if (typ == null) 
                typ = oi.Typ;
            if (rt == null) 
            {
                if (li.Count == 1 && li[0].Typ == CityItemToken.ItemType.City && OrgItemToken.TryParse(li[0].BeginToken, null) != null) 
                    return null;
                Pullenti.Ner.Geo.GeoReferent city = new Pullenti.Ner.Geo.GeoReferent();
                city.AddName((oi == null ? li[0].Value : oi.CanonicText));
                if (typ != null) 
                    city.AddTyp(typ);
                else 
                    city.AddTypCity(li[0].Kit.BaseLanguage);
                if (alttyp != null) 
                    city.AddTyp(alttyp);
                rt = new Pullenti.Ner.ReferentToken(city, li[0].BeginToken, li[li.Count - 1].EndToken) { Morph = mc };
            }
            if ((rt.Referent is Pullenti.Ner.Geo.GeoReferent) && li.Count == 1 && (rt.Referent as Pullenti.Ner.Geo.GeoReferent).IsCity) 
            {
                if (rt.BeginToken.Previous != null && rt.BeginToken.Previous.IsValue("Г", null)) 
                    rt.BeginToken = rt.BeginToken.Previous;
                else if ((rt.BeginToken.Previous != null && rt.BeginToken.Previous.IsChar('.') && rt.BeginToken.Previous.Previous != null) && rt.BeginToken.Previous.Previous.IsValue("Г", null)) 
                    rt.BeginToken = rt.BeginToken.Previous.Previous;
                else if (rt.EndToken.Next != null && (rt.WhitespacesAfterCount < 2) && rt.EndToken.Next.IsValue("Г", null)) 
                {
                    rt.EndToken = rt.EndToken.Next;
                    if (rt.EndToken.Next != null && rt.EndToken.Next.IsChar('.')) 
                        rt.EndToken = rt.EndToken.Next;
                }
            }
            return rt;
        }
        static Pullenti.Ner.ReferentToken _tryNounName(List<CityItemToken> li, out Pullenti.Ner.Core.IntOntologyItem oi, bool always)
        {
            oi = null;
            if (li == null || (li.Count < 2) || ((li[0].Typ != CityItemToken.ItemType.Noun && li[0].Typ != CityItemToken.ItemType.Misc))) 
                return null;
            bool ok = !li[0].Doubtful;
            if (ok && li[0].Typ == CityItemToken.ItemType.Misc) 
                ok = false;
            string typ = (li[0].Typ == CityItemToken.ItemType.Misc ? null : li[0].Value);
            string typ2 = (li[0].Typ == CityItemToken.ItemType.Misc ? null : li[0].AltValue);
            string probAdj = null;
            int i1 = 1;
            if ((typ != null && li[i1].Typ == CityItemToken.ItemType.Noun && ((i1 + 1) < li.Count)) && li[0].WhitespacesAfterCount <= 1 && (((Pullenti.Morph.LanguageHelper.EndsWith(typ, "ПОСЕЛОК") || Pullenti.Morph.LanguageHelper.EndsWith(typ, "СЕЛИЩЕ") || typ == "ДЕРЕВНЯ") || typ == "СЕЛО"))) 
            {
                if (li[i1].BeginToken == li[i1].EndToken) 
                {
                    OrgItemToken ooo = OrgItemToken.TryParse(li[i1].BeginToken, null);
                    if (ooo != null) 
                        return null;
                }
                typ2 = li[i1].Value;
                if (typ2 == "СТАНЦИЯ" && li[i1].BeginToken.IsValue("СТ", null) && ((i1 + 1) < li.Count)) 
                {
                    Pullenti.Ner.MorphCollection m = li[i1 + 1].Morph;
                    if (m.Number == Pullenti.Morph.MorphNumber.Plural) 
                        probAdj = "СТАРЫЕ";
                    else if (m.Gender == Pullenti.Morph.MorphGender.Feminie) 
                        probAdj = "СТАРАЯ";
                    else if (m.Gender == Pullenti.Morph.MorphGender.Masculine) 
                        probAdj = "СТАРЫЙ";
                    else 
                        probAdj = "СТАРОЕ";
                }
                i1++;
            }
            string name = li[i1].Value ?? ((li[i1].OntoItem == null ? null : li[i1].OntoItem.CanonicText));
            string altName = li[i1].AltValue;
            if (name == null) 
                return null;
            Pullenti.Ner.MorphCollection mc = li[0].Morph;
            if (i1 == 1 && li[i1].Typ == CityItemToken.ItemType.City && ((((li[0].Value == "ГОРОД" && ((li[i1].OntoItem == null || li[i1].OntoItem.Referent == null || li[i1].OntoItem.Referent.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE, "город", true) != null)))) || li[0].Value == "МІСТО" || li[0].Typ == CityItemToken.ItemType.Misc))) 
            {
                if (typ == null && ((i1 + 1) < li.Count) && li[i1 + 1].Typ == CityItemToken.ItemType.Noun) 
                    return null;
                oi = li[i1].OntoItem;
                if (oi != null) 
                    name = oi.CanonicText;
                if (name.Length > 2 || oi.MiscAttr != null) 
                {
                    if (!li[1].Doubtful || ((oi != null && oi.MiscAttr != null))) 
                        ok = true;
                    else if (!ok && !li[1].IsNewlineBefore) 
                    {
                        if (li[0].GeoObjectBefore || li[1].GeoObjectAfter) 
                            ok = true;
                        else if (Pullenti.Ner.Address.Internal.StreetDefineHelper.CheckStreetAfter(li[1].EndToken.Next)) 
                            ok = true;
                        else if (li[1].EndToken.Next != null && (li[1].EndToken.Next.GetReferent() is Pullenti.Ner.Date.DateReferent)) 
                            ok = true;
                        else if ((li[1].WhitespacesBeforeCount < 2) && li[1].OntoItem != null) 
                        {
                            if (li[1].IsNewlineAfter) 
                                ok = true;
                            else 
                                ok = true;
                        }
                    }
                    if (li[1].Doubtful && li[1].EndToken.Next != null && li[1].EndToken.Chars.Equals(li[1].EndToken.Next.Chars)) 
                        ok = false;
                    if (li[0].BeginToken.Previous != null && li[0].BeginToken.Previous.IsValue("В", null)) 
                        ok = true;
                }
                if (!ok) 
                    ok = CheckYearAfter(li[1].EndToken.Next);
                if (!ok) 
                    ok = CheckCityAfter(li[1].EndToken.Next);
            }
            else if ((li[i1].Typ == CityItemToken.ItemType.ProperName || li[i1].Typ == CityItemToken.ItemType.City)) 
            {
                if (((li[0].Value == "АДМИНИСТРАЦИЯ" || li[0].Value == "АДМІНІСТРАЦІЯ")) && i1 == 1) 
                    return null;
                if (li[i1].IsNewlineBefore) 
                {
                    if (li.Count != 2) 
                        return null;
                }
                if (!li[0].Doubtful) 
                {
                    ok = true;
                    if (name.Length < 2) 
                        ok = false;
                    else if ((name.Length < 3) && li[0].Morph.Number != Pullenti.Morph.MorphNumber.Singular) 
                        ok = false;
                    if (li[i1].Doubtful && !li[i1].GeoObjectAfter && !li[0].GeoObjectBefore) 
                    {
                        if (li[i1].Morph.Case.IsGenitive) 
                        {
                            if (li[i1].EndToken.Next == null || MiscLocationHelper.CheckGeoObjectAfter(li[i1].EndToken.Next, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(li[i1].EndToken.Next, false, true)) 
                            {
                            }
                            else if (li[0].BeginToken.Previous == null || MiscLocationHelper.CheckGeoObjectBefore(li[0].BeginToken, false)) 
                            {
                            }
                            else 
                                ok = false;
                        }
                        if (ok) 
                        {
                            Pullenti.Ner.ReferentToken rt0 = li[i1].Kit.ProcessReferent("PERSONPROPERTY", li[0].BeginToken.Previous, null);
                            if (rt0 != null) 
                            {
                                Pullenti.Ner.ReferentToken rt1 = li[i1].Kit.ProcessReferent("PERSON", li[i1].BeginToken, null);
                                if (rt1 != null) 
                                    ok = false;
                            }
                        }
                    }
                    Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(li[i1].BeginToken);
                    if (npt != null && npt.EndToken.IsValue("КИЛОМЕТР", null)) 
                        npt = null;
                    if (npt != null) 
                    {
                        if (npt.EndToken.EndChar > li[i1].EndChar && npt.Adjectives.Count > 0 && !npt.Adjectives[0].EndToken.Next.IsComma) 
                        {
                            ok = false;
                            List<CityItemToken> li2 = new List<CityItemToken>(li);
                            li2.RemoveRange(0, i1 + 1);
                            if (li2.Count > 1) 
                            {
                                Pullenti.Ner.Core.IntOntologyItem oi2 = null;
                                if (_tryNounName(li2, out oi2, false) != null) 
                                    ok = true;
                                else if (li2.Count == 2 && li2[0].Typ == CityItemToken.ItemType.Noun && ((li2[1].Typ == CityItemToken.ItemType.ProperName || li2[1].Typ == CityItemToken.ItemType.City))) 
                                    ok = true;
                            }
                            if (!ok) 
                            {
                                if (OrgItemToken.TryParse(li[i1].EndToken.Next, null) != null) 
                                    ok = true;
                                else if (li2.Count == 1 && li2[0].Typ == CityItemToken.ItemType.Noun && OrgItemToken.TryParse(li2[0].EndToken.Next, null) != null) 
                                    ok = true;
                            }
                        }
                        if (ok && TerrItemToken.m_UnknownRegions.TryParse(npt.EndToken, Pullenti.Ner.Core.TerminParseAttr.FullwordsOnly) != null) 
                        {
                            bool ok1 = false;
                            if (li[0].BeginToken.Previous != null) 
                            {
                                Pullenti.Ner.Token ttt = li[0].BeginToken.Previous;
                                if (ttt.IsComma && ttt.Previous != null) 
                                    ttt = ttt.Previous;
                                Pullenti.Ner.Geo.GeoReferent geo = ttt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                                if (geo != null && !geo.IsCity) 
                                    ok1 = true;
                            }
                            if (npt.EndToken.Next != null) 
                            {
                                Pullenti.Ner.Token ttt = npt.EndToken.Next;
                                if (ttt.IsComma && ttt.Next != null) 
                                    ttt = ttt.Next;
                                Pullenti.Ner.Geo.GeoReferent geo = ttt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                                if (geo != null && !geo.IsCity) 
                                    ok1 = true;
                                else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(ttt, false, false)) 
                                    ok1 = true;
                                else if (CheckStreetAfter(ttt)) 
                                    ok1 = true;
                            }
                            else 
                                ok1 = true;
                            if (!ok1) 
                                return null;
                        }
                    }
                    if (li[0].Value == "ПОРТ") 
                    {
                        if (li[i1].Chars.IsAllUpper || li[i1].Chars.IsLatinLetter) 
                            return null;
                    }
                }
                else if (li[0].GeoObjectBefore) 
                    ok = true;
                else if (li[i1].GeoObjectAfter && !li[i1].IsNewlineAfter) 
                    ok = true;
                else 
                    ok = CheckYearAfter(li[i1].EndToken.Next);
                if (!ok) 
                    ok = CheckStreetAfter(li[i1].EndToken.Next);
                if (!ok && li[0].BeginToken.Previous != null && li[0].BeginToken.Previous.IsValue("В", null)) 
                    ok = true;
                if ((!ok && li.Count == 2 && li[1].Typ == CityItemToken.ItemType.City) && li[0].BeginToken != li[0].EndToken) 
                    ok = true;
            }
            else 
                return null;
            if (!ok && !always) 
            {
                if (MiscLocationHelper.CheckNearBefore(li[0].BeginToken, null) == null) 
                    return null;
            }
            if (li.Count > (i1 + 1)) 
                li.RemoveRange(i1 + 1, li.Count - i1 - 1);
            Pullenti.Ner.Geo.GeoReferent city = new Pullenti.Ner.Geo.GeoReferent();
            if (oi != null && oi.Referent != null) 
            {
                city = oi.Referent.Clone() as Pullenti.Ner.Geo.GeoReferent;
                city.Occurrence.Clear();
            }
            if (!li[0].Morph.Case.IsUndefined && li[0].Morph.Gender != Pullenti.Morph.MorphGender.Undefined) 
            {
                if (li[i1].EndToken.Morph.Class.IsAdjective && li[i1].BeginToken == li[i1].EndToken && li[i1].OntoItem == null) 
                {
                    string nam = Pullenti.Ner.Core.ProperNameHelper.GetNameEx(li[i1].BeginToken, li[i1].EndToken, Pullenti.Morph.MorphClass.Adjective, li[0].Morph.Case, li[0].Morph.Gender, false, false);
                    if (nam != null && nam != name) 
                    {
                        if (li[i1].EndToken.GetMorphClassInDictionary().IsUndefined) 
                            altName = name;
                        name = nam;
                    }
                }
            }
            if (typ == "НАСЕЛЕННЫЙ ПУНКТ" && name.EndsWith("КМ")) 
            {
                Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(li[1].BeginToken, false, null, null);
                if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                {
                    Pullenti.Ner.Address.StreetReferent ss = ait.Referent as Pullenti.Ner.Address.StreetReferent;
                    ss.Number = null;
                    name = string.Format("{0} {1}", name, ss.ToString().ToUpper());
                    li[1].EndToken = ait.EndToken;
                }
            }
            if (li[0].Morph.Case.IsNominative && li[0].Typ != CityItemToken.ItemType.Misc) 
            {
                if (altName != null) 
                    city.AddName(altName);
                altName = null;
            }
            city.AddName(name);
            if (probAdj != null) 
                city.AddName(probAdj + " " + name);
            if (altName != null) 
            {
                city.AddName(altName);
                if (probAdj != null) 
                    city.AddName(probAdj + " " + altName);
            }
            if (typ != null) 
                city.AddTyp(typ);
            else if (!city.IsCity) 
                city.AddTypCity(li[0].Kit.BaseLanguage);
            if (typ2 != null) 
                city.AddTyp(typ2.ToLower());
            if (li[0].HigherGeo != null && GeoOwnerHelper.CanBeHigher(li[0].HigherGeo, city, null, null)) 
                city.Higher = li[0].HigherGeo;
            if (li[0].Typ == CityItemToken.ItemType.Misc) 
                li.RemoveAt(0);
            Pullenti.Ner.ReferentToken res = new Pullenti.Ner.ReferentToken(city, li[0].BeginToken, li[li.Count - 1].EndToken) { Morph = mc };
            Pullenti.Ner.NumberToken num = null;
            if (res.EndToken.Next != null && res.EndToken.Next.IsHiphen && (res.EndToken.Next.Next is Pullenti.Ner.NumberToken)) 
                num = res.EndToken.Next.Next as Pullenti.Ner.NumberToken;
            else if ((res.EndToken.Next is Pullenti.Ner.NumberToken) && (res.WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Token tt1 = res.EndToken.Next.Next;
                bool ok1 = false;
                if (tt1 == null || tt1.IsNewlineBefore) 
                    ok1 = true;
                else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckStreetAfter(tt1, false)) 
                    ok1 = true;
                if (ok1) 
                    num = res.EndToken.Next as Pullenti.Ner.NumberToken;
            }
            if ((num != null && num.Typ == Pullenti.Ner.NumberSpellingType.Digit && !num.Morph.Class.IsAdjective) && num.IntValue != null && (num.IntValue.Value < 100)) 
            {
                foreach (Pullenti.Ner.Slot s in city.Slots) 
                {
                    if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_NAME) 
                        city.UploadSlot(s, string.Format("{0}-{1}", s.Value, num.Value));
                }
                res.EndToken = num;
            }
            if (li[0].BeginToken == li[0].EndToken && li[0].BeginToken.IsValue("ГОРОДОК", null)) 
            {
                if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(res.EndToken.Next, true, false)) 
                    return null;
            }
            if (li[0].Typ == CityItemToken.ItemType.Noun && li[0].LengthChar == 1 && !(li[1].BeginToken is Pullenti.Ner.TextToken)) 
                return null;
            return res;
        }
        static Pullenti.Ner.ReferentToken _tryNameExist(List<CityItemToken> li, out Pullenti.Ner.Core.IntOntologyItem oi, bool always)
        {
            oi = null;
            if (li == null || li[0].Typ != CityItemToken.ItemType.City) 
                return null;
            oi = li[0].OntoItem;
            Pullenti.Ner.TextToken tt = li[0].BeginToken as Pullenti.Ner.TextToken;
            if (tt == null) 
                return null;
            bool ok = false;
            string nam = (oi == null ? li[0].Value : oi.CanonicText);
            if (nam == null) 
                return null;
            if (nam == "РИМ") 
            {
                if (tt.Term == "РИМ") 
                {
                    if ((tt.Next is Pullenti.Ner.TextToken) && tt.Next.GetMorphClassInDictionary().IsProperSecname) 
                    {
                    }
                    else 
                        ok = true;
                }
                else if (tt.Previous != null && tt.Previous.IsValue("В", null) && tt.Term == "РИМЕ") 
                    ok = true;
                else if (MiscLocationHelper.CheckGeoObjectBefore(tt, false)) 
                    ok = true;
            }
            else if (oi != null && oi.Referent != null && oi.Owner.IsExtOntology) 
                ok = true;
            else if (Pullenti.Morph.LanguageHelper.EndsWithEx(nam, "ГРАД", "СК", "TOWN", null) || nam.StartsWith("SAN")) 
                ok = true;
            else if (li[0].Chars.IsLatinLetter && li[0].BeginToken.Previous != null && ((li[0].BeginToken.Previous.IsValue("IN", null) || li[0].BeginToken.Previous.IsValue("FROM", null)))) 
                ok = true;
            else 
            {
                for (Pullenti.Ner.Token tt2 = li[0].EndToken.Next; tt2 != null; tt2 = tt2.Next) 
                {
                    if (tt2.IsNewlineBefore) 
                        break;
                    if ((tt2.IsCharOf(",(") || tt2.Morph.Class.IsPreposition || tt2.Morph.Class.IsConjunction) || tt2.Morph.Class.IsMisc) 
                        continue;
                    if ((tt2.GetReferent() is Pullenti.Ner.Geo.GeoReferent) && tt2.Chars.IsCyrillicLetter == li[0].Chars.IsCyrillicLetter) 
                        ok = true;
                    break;
                }
                bool ok2 = false;
                if (!ok) 
                {
                    for (Pullenti.Ner.Token tt2 = li[0].BeginToken.Previous; tt2 != null; tt2 = tt2.Previous) 
                    {
                        if (tt2.IsNewlineAfter) 
                            break;
                        if ((tt2.IsCharOf(",)") || tt2.Morph.Class.IsPreposition || tt2.Morph.Class.IsConjunction) || tt2.Morph.Class.IsMisc) 
                            continue;
                        if ((tt2.GetReferent() is Pullenti.Ner.Geo.GeoReferent) && tt2.Chars.IsCyrillicLetter == li[0].Chars.IsCyrillicLetter) 
                            ok = (ok2 = true);
                        if (ok) 
                        {
                            List<Pullenti.Ner.Address.Internal.StreetItemToken> sits = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(li[0].BeginToken, 10, null);
                            if (sits != null && sits.Count > 1) 
                            {
                                Pullenti.Ner.Address.Internal.AddressItemToken ss = Pullenti.Ner.Address.Internal.StreetDefineHelper.TryParseStreet(sits, false, false, false);
                                if (ss != null) 
                                {
                                    sits.RemoveAt(0);
                                    if (Pullenti.Ner.Address.Internal.StreetDefineHelper.TryParseStreet(sits, false, false, false) == null) 
                                        ok = false;
                                }
                            }
                        }
                        if (ok) 
                        {
                            if (li.Count > 1 && li[1].Typ == CityItemToken.ItemType.ProperName && (li[1].WhitespacesBeforeCount < 3)) 
                                ok = false;
                            else if (!ok2) 
                            {
                                Pullenti.Morph.MorphClass mc = li[0].BeginToken.GetMorphClassInDictionary();
                                if (mc.IsProperName || mc.IsProperSurname || mc.IsAdjective) 
                                    ok = false;
                                else 
                                {
                                    Pullenti.Ner.Core.NounPhraseToken npt = MiscLocationHelper.TryParseNpt(li[0].BeginToken);
                                    if (npt != null && npt.EndChar > li[0].EndChar) 
                                        ok = false;
                                }
                            }
                        }
                        if (OrgItemToken.TryParse(li[0].BeginToken, null) != null) 
                        {
                            ok = false;
                            break;
                        }
                        if (li[0].Doubtful) 
                        {
                            Pullenti.Ner.ReferentToken rt1 = li[0].Kit.ProcessReferent("PERSON", li[0].BeginToken, null);
                            if (rt1 != null) 
                                return null;
                        }
                        break;
                    }
                }
            }
            if (always) 
            {
                if (li[0].WhitespacesBeforeCount > 3 && li[0].Doubtful && li[0].BeginToken.GetMorphClassInDictionary().IsProperSurname) 
                {
                    Pullenti.Ner.ReferentToken pp = li[0].Kit.ProcessReferent("PERSON", li[0].BeginToken, null);
                    if (pp != null) 
                        always = false;
                }
            }
            if (li[0].BeginToken.Chars.IsLatinLetter && li[0].BeginToken == li[0].EndToken) 
            {
                Pullenti.Ner.Token tt1 = li[0].EndToken.Next;
                if (tt1 != null && tt1.IsChar(',')) 
                    tt1 = tt1.Next;
                if (((tt1 is Pullenti.Ner.TextToken) && tt1.Chars.IsLatinLetter && (tt1.LengthChar < 3)) && !tt1.Chars.IsAllLower) 
                    ok = false;
            }
            if (!ok && !always) 
                return null;
            Pullenti.Ner.Geo.GeoReferent city = null;
            if (oi != null && (oi.Referent is Pullenti.Ner.Geo.GeoReferent) && !oi.Owner.IsExtOntology) 
            {
                city = oi.Referent.Clone() as Pullenti.Ner.Geo.GeoReferent;
                city.Occurrence.Clear();
            }
            else 
            {
                city = new Pullenti.Ner.Geo.GeoReferent();
                city.AddName(nam);
                if (oi != null && (oi.Referent is Pullenti.Ner.Geo.GeoReferent)) 
                    city.MergeSlots2(oi.Referent as Pullenti.Ner.Geo.GeoReferent, li[0].Kit.BaseLanguage);
                if (!city.IsCity) 
                    city.AddTypCity(li[0].Kit.BaseLanguage);
            }
            return new Pullenti.Ner.ReferentToken(city, li[0].BeginToken, li[0].EndToken) { Morph = li[0].Morph };
        }
        static Pullenti.Ner.ReferentToken Try4(List<CityItemToken> li)
        {
            if ((li.Count > 0 && li[0].Typ == CityItemToken.ItemType.Noun && ((li[0].Value != "ГОРОД" && li[0].Value != "МІСТО" && li[0].Value != "CITY"))) && ((!li[0].Doubtful || li[0].GeoObjectBefore || li[0].LengthChar > 2))) 
            {
                if (li.Count > 1 && li[1].OrgRef != null && (li[1].WhitespacesBeforeCount < 3)) 
                {
                    Pullenti.Ner.Geo.GeoReferent geo = new Pullenti.Ner.Geo.GeoReferent();
                    geo.AddTyp(li[0].Value);
                    geo.AddOrgReferent(li[1].OrgRef.Referent);
                    geo.AddExtReferent(li[1].OrgRef);
                    return new Pullenti.Ner.ReferentToken(geo, li[0].BeginToken, li[1].EndToken);
                }
                else if (li[0].WhitespacesAfterCount < 3) 
                {
                    OrgItemToken org = OrgItemToken.TryParse(li[0].EndToken.Next, null);
                    if (org != null) 
                    {
                        if (li.Count > 1 && OrgItemToken.TryParse(li[1].EndToken.Next, null) != null) 
                        {
                        }
                        else 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = new Pullenti.Ner.Geo.GeoReferent();
                            geo.AddTyp(li[0].Value);
                            geo.AddOrgReferent(org.Referent);
                            geo.AddExtReferent(org);
                            return new Pullenti.Ner.ReferentToken(geo, li[0].BeginToken, org.EndToken);
                        }
                    }
                }
            }
            return null;
        }
        public static bool CheckYearAfter(Pullenti.Ner.Token tt)
        {
            if (tt != null && ((tt.IsComma || tt.IsHiphen))) 
                tt = tt.Next;
            if (tt != null && tt.IsNewlineAfter) 
            {
                if ((tt is Pullenti.Ner.NumberToken) && (tt as Pullenti.Ner.NumberToken).IntValue != null) 
                {
                    int year = (tt as Pullenti.Ner.NumberToken).IntValue.Value;
                    if (year > 1990 && (year < 2100)) 
                        return true;
                }
                else if (tt.GetReferent() != null && tt.GetReferent().TypeName == "DATE") 
                    return true;
            }
            return false;
        }
        public static bool CheckStreetAfter(Pullenti.Ner.Token tt)
        {
            if (tt != null && ((tt.IsCommaAnd || tt.IsHiphen || tt.Morph.Class.IsPreposition))) 
                tt = tt.Next;
            if (tt == null) 
                return false;
            Pullenti.Ner.Address.Internal.AddressItemToken ait = Pullenti.Ner.Address.Internal.AddressItemToken.TryParse(tt, false, null, null);
            if (ait != null && ait.Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street) 
                return true;
            return false;
        }
        public static bool CheckCityAfter(Pullenti.Ner.Token tt)
        {
            while (tt != null && (((tt.IsCommaAnd || tt.IsHiphen || tt.Morph.Class.IsPreposition) || tt.IsChar('.')))) 
            {
                tt = tt.Next;
            }
            if (tt == null) 
                return false;
            List<CityItemToken> cits = CityItemToken.TryParseList(tt, 5, null);
            if (cits == null || cits.Count == 0) 
            {
                if (tt.LengthChar == 1 && tt.Chars.IsAllLower && ((tt.IsValue("Д", null) || tt.IsValue("П", null)))) 
                {
                    Pullenti.Ner.Token tt1 = tt.Next;
                    if (tt1 != null && tt1.IsChar('.')) 
                        tt1 = tt1.Next;
                    CityItemToken ci = CityItemToken.TryParse(tt1, null, false, null);
                    if (ci != null && ((ci.Typ == CityItemToken.ItemType.ProperName || ci.Typ == CityItemToken.ItemType.City))) 
                        return true;
                }
                return false;
            }
            if (TryDefine(cits, null, false) != null) 
                return true;
            if (cits[0].Typ == CityItemToken.ItemType.Noun) 
            {
                if (tt.Previous != null && tt.Previous.IsComma) 
                    return true;
                if (cits.Count > 1 && ((cits[1].Typ == CityItemToken.ItemType.City || cits[1].Typ == CityItemToken.ItemType.ProperName))) 
                    return true;
            }
            return false;
        }
    }
}