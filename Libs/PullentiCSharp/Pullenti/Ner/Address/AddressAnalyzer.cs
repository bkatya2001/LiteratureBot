/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pullenti.Ner.Address
{
    /// <summary>
    /// Анализатор адресов
    /// </summary>
    public class AddressAnalyzer : Pullenti.Ner.Analyzer
    {
        /// <summary>
        /// Имя анализатора ("ADDRESS")
        /// </summary>
        public const string ANALYZER_NAME = "ADDRESS";
        public override string Name
        {
            get
            {
                return ANALYZER_NAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Адреса";
            }
        }
        public override string Description
        {
            get
            {
                return "Адреса (улицы, дома ...)";
            }
        }
        public override Pullenti.Ner.Analyzer Clone()
        {
            return new AddressAnalyzer();
        }
        public override ICollection<Pullenti.Ner.Metadata.ReferentClass> TypeSystem
        {
            get
            {
                return new Pullenti.Ner.Metadata.ReferentClass[] {Pullenti.Ner.Address.Internal.MetaAddress.GlobalMeta, Pullenti.Ner.Address.Internal.MetaStreet.GlobalMeta};
            }
        }
        public override Dictionary<string, byte[]> Images
        {
            get
            {
                Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
                res.Add(Pullenti.Ner.Address.Internal.MetaAddress.AddressImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("address.png"));
                res.Add(Pullenti.Ner.Address.Internal.MetaStreet.ImageId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("street.png"));
                res.Add(Pullenti.Ner.Address.Internal.MetaStreet.ImageTerrOrgId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("terrorg.png"));
                res.Add(Pullenti.Ner.Address.Internal.MetaStreet.ImageTerrSpecId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("terrspec.png"));
                res.Add(Pullenti.Ner.Address.Internal.MetaStreet.ImageTerrId, Pullenti.Ner.Core.Internal.ResourceHelper.GetBytes("territory.png"));
                return res;
            }
        }
        public override int ProgressWeight
        {
            get
            {
                return 4;
            }
        }
        public override Pullenti.Ner.Referent CreateReferent(string type)
        {
            if (type == AddressReferent.OBJ_TYPENAME) 
                return new AddressReferent();
            if (type == StreetReferent.OBJ_TYPENAME) 
                return new StreetReferent();
            return null;
        }
        public override IEnumerable<string> UsedExternObjectTypes
        {
            get
            {
                return new string[] {Pullenti.Ner.Geo.GeoReferent.OBJ_TYPENAME, "PHONE", "URI"};
            }
        }
        class AddressAnalyzerData : Pullenti.Ner.Core.AnalyzerData
        {
            Pullenti.Ner.Core.AnalyzerData m_Addresses = new Pullenti.Ner.Core.AnalyzerData();
            public Pullenti.Ner.Core.AnalyzerDataWithOntology Streets = new Pullenti.Ner.Core.AnalyzerDataWithOntology();
            public override Pullenti.Ner.Referent RegisterReferent(Pullenti.Ner.Referent referent)
            {
                if (referent is Pullenti.Ner.Address.StreetReferent) 
                {
                    (referent as Pullenti.Ner.Address.StreetReferent).Correct();
                    return Streets.RegisterReferent(referent);
                }
                else 
                    return m_Addresses.RegisterReferent(referent);
            }
            public override ICollection<Pullenti.Ner.Referent> Referents
            {
                get
                {
                    if (Streets.Referents.Count == 0) 
                        return m_Addresses.Referents;
                    else if (m_Addresses.Referents.Count == 0) 
                        return Streets.Referents;
                    List<Pullenti.Ner.Referent> res = new List<Pullenti.Ner.Referent>(Streets.Referents);
                    res.AddRange(m_Addresses.Referents);
                    return res;
                }
                set
                {
                    m_Referents.Clear();
                    if (value != null) 
                        m_Referents.AddRange(value);
                }
            }
        }

        public override Pullenti.Ner.Core.AnalyzerData CreateAnalyzerData()
        {
            return new AddressAnalyzerData();
        }
        public override void Process(Pullenti.Ner.Core.AnalysisKit kit)
        {
            AddressAnalyzerData ad = kit.GetAnalyzerData(this) as AddressAnalyzerData;
            Pullenti.Ner.Geo.Internal.GeoAnalyzerData gad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(kit.FirstToken);
            if (gad == null) 
                return;
            gad.AllRegime = true;
            int steps = 1;
            int max = steps;
            int delta = 100000;
            int parts = (((kit.Sofa.Text.Length + delta) - 1)) / delta;
            if (parts == 0) 
                parts = 1;
            max *= parts;
            int cur = 0;
            int nextPos = delta;
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = t.Next) 
            {
                if (t.BeginChar > nextPos) 
                {
                    nextPos += delta;
                    cur++;
                    if (!this.OnProgress(cur, max, kit)) 
                        return;
                }
                List<Pullenti.Ner.Address.Internal.AddressItemToken> li = Pullenti.Ner.Address.Internal.AddressItemToken.TryParseList(t, 20);
                if (li == null || li.Count == 0) 
                    continue;
                if ((li.Count == 1 && li[0].Typ == Pullenti.Ner.Address.Internal.AddressItemType.Street && (li[0].Referent as StreetReferent).Kind == StreetKind.Railway) && (li[0].Referent as StreetReferent).Number == null) 
                {
                    t = li[0].EndToken;
                    continue;
                }
                Pullenti.Ner.Token tt = Pullenti.Ner.Address.Internal.AddressDefineHelper.TryDefine(li, t, ad);
                if (tt != null) 
                    t = tt;
            }
            sw.Stop();
            List<StreetReferent> sli = new List<StreetReferent>();
            for (Pullenti.Ner.Token t = kit.FirstToken; t != null; t = (t == null ? null : t.Next)) 
            {
                StreetReferent sr = t.GetReferent() as StreetReferent;
                if (sr == null) 
                    continue;
                if (t.Next == null || !t.Next.IsCommaAnd) 
                    continue;
                sli.Clear();
                sli.Add(sr);
                for (t = t.Next; t != null; t = t.Next) 
                {
                    if (t.IsCommaAnd) 
                        continue;
                    if ((((sr = t.GetReferent() as StreetReferent))) != null) 
                    {
                        sli.Add(sr);
                        continue;
                    }
                    AddressReferent adr = t.GetReferent() as AddressReferent;
                    if (adr == null) 
                        break;
                    if (adr.Streets.Count == 0) 
                        break;
                    foreach (Pullenti.Ner.Referent ss in adr.Streets) 
                    {
                        if (ss is StreetReferent) 
                            sli.Add(ss as StreetReferent);
                    }
                }
                if (sli.Count < 2) 
                    continue;
                bool ok = true;
                Pullenti.Ner.Geo.GeoReferent hi = null;
                foreach (StreetReferent s in sli) 
                {
                    if (s.Geos.Count == 0) 
                        continue;
                    else if (s.Geos.Count == 1) 
                    {
                        if (hi == null || hi == s.Geos[0]) 
                            hi = s.Geos[0];
                        else 
                        {
                            ok = false;
                            break;
                        }
                    }
                    else 
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok && hi != null) 
                {
                    foreach (StreetReferent s in sli) 
                    {
                        if (s.Geos.Count == 0) 
                            s.AddSlot(StreetReferent.ATTR_GEO, hi, false, 0);
                    }
                }
            }
            foreach (Pullenti.Ner.Referent a in ad.Referents) 
            {
                if (a is AddressReferent) 
                    (a as AddressReferent).Correct();
            }
            gad.AllRegime = false;
        }
        public override Pullenti.Ner.ReferentToken ProcessOntologyItem(Pullenti.Ner.Token begin)
        {
            List<Pullenti.Ner.Address.Internal.StreetItemToken> li = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(begin, 10, null);
            if (li == null || (li.Count < 2)) 
                return null;
            Pullenti.Ner.Address.Internal.AddressItemToken rt = Pullenti.Ner.Address.Internal.StreetDefineHelper.TryParseStreet(li, true, false, false);
            if (rt == null) 
                return null;
            StreetReferent street = rt.Referent as StreetReferent;
            for (Pullenti.Ner.Token t = rt.EndToken.Next; t != null; t = t.Next) 
            {
                if (!t.IsChar(';')) 
                    continue;
                t = t.Next;
                if (t == null) 
                    break;
                li = Pullenti.Ner.Address.Internal.StreetItemToken.TryParseList(begin, 10, null);
                Pullenti.Ner.Address.Internal.AddressItemToken rt1 = Pullenti.Ner.Address.Internal.StreetDefineHelper.TryParseStreet(li, true, false, false);
                if (rt1 != null) 
                {
                    t = (rt.EndToken = rt1.EndToken);
                    street.MergeSlots(rt1.Referent, true);
                }
                else 
                {
                    Pullenti.Ner.Token tt = null;
                    for (Pullenti.Ner.Token ttt = t; ttt != null; ttt = ttt.Next) 
                    {
                        if (ttt.IsChar(';')) 
                            break;
                        else 
                            tt = ttt;
                    }
                    if (tt != null) 
                    {
                        string str = Pullenti.Ner.Core.MiscHelper.GetTextValue(t, tt, Pullenti.Ner.Core.GetTextAttr.No);
                        if (str != null) 
                            street.AddSlot(StreetReferent.ATTR_NAME, Pullenti.Ner.Core.MiscHelper.ConvertFirstCharUpperAndOtherLower(str), false, 0);
                        t = (rt.EndToken = tt);
                    }
                }
            }
            return new Pullenti.Ner.ReferentToken(street, rt.BeginToken, rt.EndToken);
        }
        static bool m_Initialized = false;
        public static void Initialize()
        {
            if (m_Initialized) 
                return;
            m_Initialized = true;
            try 
            {
                Pullenti.Ner.Address.Internal.AddressItemToken.Initialize();
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
            Pullenti.Ner.ProcessorService.RegisterAnalyzer(new AddressAnalyzer());
        }
    }
}