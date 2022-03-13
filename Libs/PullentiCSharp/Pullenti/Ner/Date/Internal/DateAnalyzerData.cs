/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Date.Internal
{
    class DateAnalyzerData : Pullenti.Ner.Core.AnalyzerData
    {
        Dictionary<string, Pullenti.Ner.Referent> m_Hash = new Dictionary<string, Pullenti.Ner.Referent>();
        public override ICollection<Pullenti.Ner.Referent> Referents
        {
            get
            {
                return m_Hash.Values;
            }
            set
            {
                m_Referents.Clear();
                if (value != null) 
                    m_Referents.AddRange(value);
            }
        }
        public override Pullenti.Ner.Referent RegisterReferent(Pullenti.Ner.Referent referent)
        {
            string key = referent.ToString();
            Pullenti.Ner.Referent dr;
            if (m_Hash.TryGetValue(key, out dr)) 
                return dr;
            m_Hash.Add(key, referent);
            return referent;
        }
        public bool DRegime = false;
    }
}