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
    class OrgTypToken : Pullenti.Ner.MetaToken
    {
        public OrgTypToken(Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(b, e, null)
        {
        }
        public bool IsDoubt;
        public List<string> Vals = new List<string>();
        public OrgTypToken Clone()
        {
            OrgTypToken res = new OrgTypToken(BeginToken, EndToken);
            res.Vals.AddRange(Vals);
            res.IsDoubt = IsDoubt;
            return res;
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (IsDoubt) 
                tmp.Append("? ");
            for (int i = 0; i < Vals.Count; i++) 
            {
                if (i > 0) 
                    tmp.Append(" / ");
                tmp.Append(Vals[i]);
            }
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
            ad.OTRegime = false;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                bool afterTerr = false;
                Pullenti.Ner.Token tt = MiscLocationHelper.CheckTerritory(t);
                if (tt != null && tt.Next != null) 
                {
                    afterTerr = true;
                    t = tt.Next;
                }
                GeoTokenData d = t.Tag as GeoTokenData;
                OrgTypToken ty = TryParse(t, afterTerr, ad);
                if (ty != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.OrgTyp = ty;
                    t = ty.EndToken;
                }
            }
            ad.OTRegime = true;
        }
        public static OrgTypToken TryParse(Pullenti.Ner.Token t, bool afterTerr, GeoAnalyzerData ad = null)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            if (t.LengthChar == 1 && !t.Chars.IsLetter) 
                return null;
            if (ad == null) 
                ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad != null && SpeedRegime && ((ad.OTRegime || ad.AllRegime))) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d != null) 
                    return d.OrgTyp;
                return null;
            }
            if (ad.OLevel > 2) 
                return null;
            ad.OLevel++;
            OrgTypToken res = _TryParse(t, afterTerr, 0);
            ad.OLevel--;
            return res;
        }
        static OrgTypToken _TryParse(Pullenti.Ner.Token t, bool afterTerr, int lev = 0)
        {
            if (t == null) 
                return null;
            if (t.IsValue("СП", null)) 
            {
                if (!afterTerr && t.Chars.IsAllLower) 
                    return null;
            }
            if (t.IsValue("НП", null)) 
            {
                if (!afterTerr && t.Chars.IsAllLower) 
                    return null;
            }
            if ((t.IsValue("ОФИС", null) || t.IsValue("ФАД", null) || t.IsValue("АД", null)) || t.IsValue("КОРПУС", null)) 
                return null;
            if (t.IsValue("ФЕДЕРАЦИЯ", null) || t.IsValue("СОЮЗ", null)) 
                return null;
            Pullenti.Ner.Token t1 = null;
            List<string> typs = null;
            bool doubt = false;
            Pullenti.Ner.MorphCollection morph = null;
            Pullenti.Ner.Core.TerminToken tok = m_OrgOntology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null) 
            {
                t1 = tok.EndToken;
                typs = new List<string>();
                morph = tok.Morph;
                typs.Add(tok.Termin.CanonicText.ToLower());
                if (tok.Termin.Acronym != null) 
                    typs.Add(tok.Termin.Acronym);
                if (tok.EndToken == t) 
                {
                    if ((t.LengthChar < 4) && (t is Pullenti.Ner.TextToken) && Pullenti.Morph.LanguageHelper.EndsWith((t as Pullenti.Ner.TextToken).Term, "К")) 
                    {
                        Pullenti.Ner.Core.IntOntologyToken oi = TerrItemToken.CheckOntoItem(t.Next);
                        if (oi != null) 
                        {
                            if (t.Next.GetMorphClassInDictionary().IsAdjective && oi.BeginToken == oi.EndToken) 
                            {
                            }
                            else 
                                return null;
                        }
                        if ((!afterTerr && t.Chars.IsAllUpper && t.Next != null) && t.Next.Chars.IsAllUpper && t.Next.LengthChar > 1) 
                            return null;
                    }
                }
                if (tok.Termin.CanonicText == "МЕСТОРОЖДЕНИЕ" && (tok.EndToken.Next is Pullenti.Ner.TextToken) && tok.EndToken.Next.Chars.IsAllLower) 
                {
                    Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tok.EndToken.Next, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                    if (npt != null && npt.Chars.IsAllLower) 
                        tok.EndToken = npt.EndToken;
                }
                if ((((t.Chars.IsAllUpper && t.LengthChar == 1 && t.Next != null) && t.Next.IsChar('.') && (t.Next.Next is Pullenti.Ner.TextToken)) && t.Next.Next.LengthChar == 1 && t.Next.Next.Chars.IsAllUpper) && t.Next.Next.Next == tok.EndToken && tok.EndToken.IsChar('.')) 
                    return null;
            }
            else 
            {
                if (Pullenti.Ner.Address.Internal.StreetItemToken.CheckKeyword(t)) 
                    return null;
                Pullenti.Ner.ReferentToken rtok = t.Kit.ProcessReferent("ORGANIZATION", t, "MINTYPE");
                if (rtok != null) 
                {
                    if (rtok.EndToken == t && t.IsValue("ТК", null)) 
                    {
                        if (TerrItemToken.CheckOntoItem(t.Next) != null) 
                            return null;
                        if (t.Chars.IsAllUpper && t.Next != null && t.Next.Chars.IsAllUpper) 
                            return null;
                    }
                    string prof = rtok.Referent.GetStringValue("PROFILE");
                    if (string.Compare(prof ?? "", "UNIT", true) == 0) 
                        doubt = true;
                    t1 = rtok.EndToken;
                    typs = rtok.Referent.GetStringValues("TYPE");
                    morph = rtok.Morph;
                }
            }
            if (((t1 == null && (t is Pullenti.Ner.TextToken) && t.LengthChar >= 2) && t.LengthChar <= 4 && t.Chars.IsAllUpper) && t.Chars.IsCyrillicLetter) 
            {
                if (Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null, null) != null) 
                    return null;
                if (t.LengthChar == 2) 
                    return null;
                if (TerrItemToken.CheckOntoItem(t) != null) 
                    return null;
                typs = new List<string>();
                typs.Add((t as Pullenti.Ner.TextToken).Term);
                t1 = t;
                doubt = true;
            }
            if (t1 == null && afterTerr) 
            {
                Pullenti.Ner.Core.TerminToken pt = Pullenti.Ner.Address.Internal.AddressItemToken.m_Plot.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
                if (pt != null) 
                {
                    typs = new List<string>();
                    typs.Add("участок");
                    t1 = pt.EndToken;
                    doubt = true;
                }
                else if ((((pt = Pullenti.Ner.Address.Internal.AddressItemToken.m_Owner.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No)))) != null) 
                {
                    typs = new List<string>();
                    typs.Add("владение");
                    t1 = pt.EndToken;
                    doubt = true;
                }
            }
            if (t1 == null) 
                return null;
            if (morph == null) 
                morph = t1.Morph;
            OrgTypToken res = new OrgTypToken(t, t1) { IsDoubt = doubt, Vals = typs, Morph = morph };
            if ((t == t1 && (t.LengthChar < 3) && t.Next != null) && t.Next.IsChar('.')) 
                res.EndToken = t1.Next;
            if ((lev < 2) && (res.WhitespacesAfterCount < 3)) 
            {
                OrgTypToken next = TryParse(res.EndToken.Next, afterTerr, null);
                if (next != null && !next.BeginToken.Chars.IsAllLower) 
                {
                    NameToken nam = NameToken.TryParse(next.EndToken.Next, NameTokenType.Org, 0);
                    if (nam == null || next.WhitespacesAfterCount > 3) 
                        next = null;
                    else if ((nam.Number != null && nam.Name == null && next.LengthChar > 2) && next.IsDoubt) 
                        next = null;
                }
                if (next != null) 
                {
                    if (!next.IsDoubt) 
                        res.IsDoubt = false;
                    res.MergeWith(next);
                }
            }
            return res;
        }
        public void MergeWith(OrgTypToken ty)
        {
            foreach (string v in ty.Vals) 
            {
                if (!Vals.Contains(v)) 
                    Vals.Add(v);
            }
            EndToken = ty.EndToken;
        }
        public static List<Pullenti.Ner.Core.Termin> FindTerminByAcronym(string abbr)
        {
            Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(abbr) { Acronym = abbr };
            return m_OrgOntology.FindTerminsByTermin(te);
        }
        public static void Initialize()
        {
            m_OrgOntology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("САДОВОЕ ТОВАРИЩЕСТВО") { Acronym = "СТ" };
            t.AddVariant("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СТ";
            t.AddAbridge("С/ТОВ");
            t.AddAbridge("ПК СТ");
            t.AddAbridge("САД.ТОВ.");
            t.AddAbridge("САДОВ.ТОВ.");
            t.AddAbridge("С/Т");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ ТОВАРИЩЕСТВО");
            t.AddAbridge("Д/Т");
            t.AddAbridge("ДАЧ/Т");
            t.Acronym = "ДТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЖИЛИЩНОЕ ТОВАРИЩЕСТВО");
            t.AddAbridge("Ж/Т");
            t.AddAbridge("ЖИЛ/Т");
            t.Acronym = "ЖТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВЫЙ КООПЕРАТИВ");
            t.AddAbridge("С/К");
            t.Acronym = "СК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ");
            t.AddVariant("ПОТРЕБКООПЕРАТИВ", false);
            t.Acronym = "ПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ ОБЩЕСТВО");
            t.AddAbridge("С/О");
            t.Acronym = "СО";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ДАЧНОЕ ТОВАРИЩЕСТВО");
            t.AddVariant("САДОВОЕ ДАЧНОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СДТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ");
            t.Acronym = "ДНО";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО");
            t.Acronym = "ДНП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО");
            t.Acronym = "ДНТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ");
            t.Acronym = "ДПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ");
            t.AddVariant("ДАЧНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            t.Acronym = "ДСК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ");
            t.Acronym = "СПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО");
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.AddVariant("ТСНСТ", false);
            t.Acronym = "СНТ";
            t.AcronymCanBeLower = true;
            t.AddAbridge("САДОВОЕ НЕКОМ-Е ТОВАРИЩЕСТВО");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "СНО", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "СНП", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СНТ", AcronymSmart = "СНТ", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СОТ", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ДНТ", AcronymCanBeLower = true };
            t.AddVariant("ДАЧНО НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "НСТ", AcronymCanBeLower = true };
            t.AddVariant("НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНСТ", AcronymCanBeLower = true };
            t.AddVariant("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ") { Acronym = "СПК", AcronymCanBeLower = true };
            t.AddVariant("САДОВАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ДСК", AcronymCanBeLower = true };
            t.AddVariant("ДАЧНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            m_OrgOntology.Add(t);
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ДСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ЖСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ЖСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ЖСКИЗ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "ОНО", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "ОНП", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНТ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ОПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СТСН", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "ТСН", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ ЖИЛЬЯ") { Acronym = "ТСЖ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("САДОВЫЕ ЗЕМЕЛЬНЫЕ УЧАСТКИ") { Acronym = "СЗУ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ТИЗ", AcronymCanBeLower = true });
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "КИЗ", AcronymCanBeLower = true };
            t.AddVariant("КИЗК", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СНТСН", AcronymCanBeLower = true };
            t.AddVariant("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ", false);
            t.AddVariant("СНТ СН", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО СОБСТВЕННИКОВ") { Acronym = "НПС", AcronymCanBeLower = true };
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЛИЧНОЕ ПОДСОБНОЕ ХОЗЯЙСТВО") { Acronym = "ЛПХ", AcronymCanBeLower = true };
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕНИЕ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВМЕСТНОЕ ПРЕДПРИЯТИЕ");
            t.Acronym = "СП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО");
            t.Acronym = "НП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОМОБИЛЬНЫЙ КООПЕРАТИВ");
            t.AddAbridge("А/К");
            t.Acronym = "АК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ КООПЕРАТИВ");
            t.AddAbridge("Г/К");
            t.AddAbridge("ГР.КОП.");
            t.AddAbridge("ГАР.КОП.");
            t.AddAbridge("ГАР.КООП.");
            t.AddVariant("ГАРАЖНЫЙ КООП", false);
            t.Acronym = "ГК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПРОИЗВОДСТВЕННЫЙ СЕЛЬСКОХОЗЯЙСТВЕННЫЙ КООПЕРАТИВ");
            t.AddVariant("ПРОИЗВОДСТВЕННО СЕЛЬСКОХОЗЯЙСТВЕННЫЙ КООПЕРАТИВ", false);
            t.Acronym = "ПСК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ГСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ЭКСПЛУАТАЦИОННЫЙ КООПЕРАТИВ") { Acronym = "ГЭК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ПГСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ СТРОИТЕЛЬНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНЫЙ КООПЕРАТИВ") { Acronym = "ПГК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ИНДИВИДУАЛЬНОЕ ЖИЛИЩНОЕ СТРОИТЕЛЬСТВО") { Acronym = "ИЖС", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКАЯ ТОЧКА"));
            t = new Pullenti.Ner.Core.Termin("СТАНЦИЯ ТЕХНИЧЕСКОГО ОБСЛУЖИВАНИЯ") { Acronym = "СТО", AcronymCanBeLower = true };
            t.AddVariant("СТАНЦИЯ ТЕХОБСЛУЖИВАНИЯ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТО ЗАПРАВОЧНАЯ СТАНЦИЯ") { Acronym = "АЗС", AcronymCanBeLower = true };
            t.AddVariant("АВТОЗАПРАВОЧНАЯ СТАНЦИЯ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНАЯ ЗАСТРОЙКА") { Acronym = "ДЗ", AcronymCanBeLower = true };
            t.AddVariant("КВАРТАЛ ДАЧНОЙ ЗАСТРОЙКИ", false);
            t.AddVariant("ЗОНА ДАЧНОЙ ЗАСТРОЙКИ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОТТЕДЖНЫЙ ПОСЕЛОК") { Acronym = "КП", AcronymCanBeLower = true };
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОСЕЛОК") { Acronym = "ДП", AcronymCanBeLower = true };
            t.AddAbridge("Д/П");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САНАТОРИЙ");
            t.AddAbridge("САН.");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОМ ОТДЫХА");
            t.AddAbridge("Д/О");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БАЗА ОТДЫХА");
            t.AddAbridge("Б/О");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ФЕРМЕРСКОЕ ХОЗЯЙСТВО") { Acronym = "ФХ", AcronymCanBeLower = true };
            t.AddAbridge("Ф/Х");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВХОЗ");
            t.AddAbridge("С-ЗА");
            t.AddAbridge("С/ЗА");
            t.AddAbridge("С/З");
            t.AddAbridge("СХ.");
            t.AddAbridge("С/Х");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПИОНЕРСКИЙ ЛАГЕРЬ");
            t.AddAbridge("П/Л");
            t.AddAbridge("П.Л.");
            t.AddAbridge("ПИОНЕР.ЛАГ.");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КУРОРТ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ВЛАДЕЛЬЦЕВ") { Acronym = "КИВ", AcronymCanBeLower = true };
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОДСОБНОЕ ХОЗЯЙСТВО");
            t.AddAbridge("ПОДСОБНОЕ Х-ВО");
            t.AddAbridge("ПОДСОБНОЕ ХОЗ-ВО");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БИЗНЕС ЦЕНТР") { Acronym = "БЦ", AcronymCanBeLower = true };
            t.AddVariant("БІЗНЕС ЦЕНТР", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЭРОПОРТ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АЭРОДРОМ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГИДРОУЗЕЛ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОДОЗАБОР");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОЛЕВОЙ СТАН");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ЧАБАНСКАЯ СТОЯНКА");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ВОЙСКОВАЯ ЧАСТЬ") { Acronym = "ВЧ", AcronymCanBeLower = true };
            t.AddVariant("ВОИНСКАЯ ЧАСТЬ", false);
            t.AddAbridge("В/Ч");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КВАРТИРНО ЭКСПЛУАТАЦИОННАЯ ЧАСТЬ") { Acronym = "КЭЧ", AcronymCanBeLower = true };
            m_OrgOntology.Add(t);
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("КАРЬЕР"));
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("РУДНИК"));
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ПРИИСК"));
            t = new Pullenti.Ner.Core.Termin("МЕСТОРОЖДЕНИЕ");
            t.AddAbridge("МЕСТОРОЖД.");
            m_OrgOntology.Add(t);
        }
        internal static Pullenti.Ner.Core.TerminCollection m_OrgOntology;
    }
}