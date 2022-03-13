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
    public class GeoAnalyzerData : Pullenti.Ner.Core.AnalyzerDataWithOntology
    {
        public bool AllRegime;
        public bool TRegime;
        public bool CRegime;
        public bool ORegime;
        public bool OTRegime;
        public bool SRegime;
        public bool ARegime;
        public bool CheckRegime;
        public int TLevel = 0;
        public int CLevel = 0;
        public int OLevel = 0;
        public int SLevel = 0;
        public int ALevel = 0;
        public int GeoBefore = 0;
        public int Step;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (AllRegime) 
                tmp.Append("AllRegime ");
            if (TRegime) 
                tmp.Append("TRegime ");
            if (CRegime) 
                tmp.Append("CRegime ");
            if (ORegime) 
                tmp.Append("ORegime ");
            if (OTRegime) 
                tmp.Append("OTRegime ");
            if (SRegime) 
                tmp.Append("SRegime ");
            if (ARegime) 
                tmp.Append("ARegime ");
            if (CheckRegime) 
                tmp.Append("CheckRegime ");
            if (TLevel > 0) 
                tmp.AppendFormat("TLev={0} ", TLevel);
            if (CLevel > 0) 
                tmp.AppendFormat("CLev={0} ", CLevel);
            if (OLevel > 0) 
                tmp.AppendFormat("OLev={0} ", OLevel);
            if (SLevel > 0) 
                tmp.AppendFormat("SLev={0} ", SLevel);
            if (ALevel > 0) 
                tmp.AppendFormat("ALev={0} ", ALevel);
            tmp.AppendFormat("{0} referents", Referents.Count);
            return tmp.ToString();
        }
        static string[] ends = new string[] {"КИЙ", "КОЕ", "КАЯ"};
        public override Pullenti.Ner.Referent RegisterReferent(Pullenti.Ner.Referent referent)
        {
            Pullenti.Ner.Geo.GeoReferent g = referent as Pullenti.Ner.Geo.GeoReferent;
            if (g != null) 
            {
                if (g.IsState) 
                {
                }
                else if (g.IsRegion || ((g.IsCity && !g.IsBigCity))) 
                {
                    List<string> names = new List<string>();
                    Pullenti.Morph.MorphGender gen = Pullenti.Morph.MorphGender.Undefined;
                    string basNam = null;
                    foreach (Pullenti.Ner.Slot s in g.Slots) 
                    {
                        if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_NAME) 
                            names.Add(s.Value as string);
                        else if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE) 
                        {
                            string typ = s.Value as string;
                            if (Pullenti.Morph.LanguageHelper.EndsWithEx(typ, "район", "край", "округ", "улус")) 
                                gen |= Pullenti.Morph.MorphGender.Masculine;
                            else if (Pullenti.Morph.LanguageHelper.EndsWithEx(typ, "область", "территория", null, null)) 
                                gen |= Pullenti.Morph.MorphGender.Feminie;
                        }
                    }
                    for (int i = 0; i < names.Count; i++) 
                    {
                        string n = names[i];
                        int ii = n.IndexOf(' ');
                        if (ii > 0) 
                        {
                            if (g.GetSlotValue(Pullenti.Ner.Geo.GeoReferent.ATTR_REF) is Pullenti.Ner.Referent) 
                                continue;
                            string nn = string.Format("{0} {1}", n.Substring(ii + 1), n.Substring(0, ii));
                            if (!names.Contains(nn)) 
                            {
                                names.Add(nn);
                                g.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, nn, false, 0);
                                continue;
                            }
                            continue;
                        }
                        foreach (string end in ends) 
                        {
                            if (Pullenti.Morph.LanguageHelper.EndsWith(n, end)) 
                            {
                                string nn = n.Substring(0, n.Length - 3);
                                foreach (string end2 in ends) 
                                {
                                    if (end2 != end) 
                                    {
                                        if (!names.Contains(nn + end2)) 
                                        {
                                            names.Add(nn + end2);
                                            g.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, nn + end2, false, 0);
                                        }
                                    }
                                }
                                if (gen == Pullenti.Morph.MorphGender.Masculine) 
                                {
                                    foreach (string na in names) 
                                    {
                                        if (Pullenti.Morph.LanguageHelper.EndsWith(na, "ИЙ")) 
                                            basNam = na;
                                    }
                                }
                                else if (gen == Pullenti.Morph.MorphGender.Feminie) 
                                {
                                    foreach (string na in names) 
                                    {
                                        if (Pullenti.Morph.LanguageHelper.EndsWith(na, "АЯ")) 
                                            basNam = na;
                                    }
                                }
                                else if (gen == Pullenti.Morph.MorphGender.Neuter) 
                                {
                                    foreach (string na in names) 
                                    {
                                        if (Pullenti.Morph.LanguageHelper.EndsWith(na, "ОЕ")) 
                                            basNam = na;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (basNam != null && names.Count > 0 && names[0] != basNam) 
                    {
                        Pullenti.Ner.Slot sl = g.FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, basNam, true);
                        if (sl != null) 
                        {
                            g.Slots.Remove(sl);
                            g.Slots.Insert(0, sl);
                        }
                    }
                }
            }
            return base.RegisterReferent(referent);
        }
    }
}