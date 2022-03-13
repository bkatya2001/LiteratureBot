/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Org.Internal
{
    public class OrgAnalyzerData : Pullenti.Ner.Core.AnalyzerDataWithOntology
    {
        public override Pullenti.Ner.Referent RegisterReferent(Pullenti.Ner.Referent referent)
        {
            if (referent is Pullenti.Ner.Org.OrganizationReferent) 
                (referent as Pullenti.Ner.Org.OrganizationReferent).FinalCorrection();
            int slots = referent.Slots.Count;
            Pullenti.Ner.Referent res = base.RegisterReferent(referent);
            if (!LargeTextRegim && (res is Pullenti.Ner.Org.OrganizationReferent) && (res == referent)) 
            {
                Pullenti.Ner.Core.IntOntologyItem ioi = (res as Pullenti.Ner.Org.OrganizationReferent).CreateOntologyItemEx(2, true, false);
                if (ioi != null) 
                    LocOrgs.AddItem(ioi);
                List<string> names = (res as Pullenti.Ner.Org.OrganizationReferent)._getPureNames();
                if (names != null) 
                {
                    foreach (string n in names) 
                    {
                        OrgPureNames.Add(new Pullenti.Ner.Core.Termin(n));
                    }
                }
            }
            return res;
        }
        public Pullenti.Ner.Core.IntOntologyCollection LocOrgs = new Pullenti.Ner.Core.IntOntologyCollection();
        public Pullenti.Ner.Core.TerminCollection OrgPureNames = new Pullenti.Ner.Core.TerminCollection();
        public Pullenti.Ner.Core.TerminCollection Aliases = new Pullenti.Ner.Core.TerminCollection();
        public bool LargeTextRegim = false;
        public bool TRegime = false;
        public int TLevel;
    }
}