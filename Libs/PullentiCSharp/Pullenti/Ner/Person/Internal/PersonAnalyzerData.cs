/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Person.Internal
{
    class PersonAnalyzerData : Pullenti.Ner.Core.AnalyzerDataWithOntology
    {
        public bool NominativeCaseAlways = false;
        public bool TextStartsWithLastnameFirstnameMiddlename = false;
        public bool NeedSecondStep = false;
        public override Pullenti.Ner.Referent RegisterReferent(Pullenti.Ner.Referent referent)
        {
            if (referent is Pullenti.Ner.Person.PersonReferent) 
            {
                List<Pullenti.Ner.Person.PersonPropertyReferent> existProps = null;
                for (int i = 0; i < referent.Slots.Count; i++) 
                {
                    Pullenti.Ner.Slot a = referent.Slots[i];
                    if (a.TypeName == Pullenti.Ner.Person.PersonReferent.ATTR_ATTR) 
                    {
                        PersonAttrToken pat = a.Value as PersonAttrToken;
                        if (pat == null || pat.PropRef == null) 
                        {
                            if (a.Value is Pullenti.Ner.Person.PersonPropertyReferent) 
                            {
                                if (existProps == null) 
                                    existProps = new List<Pullenti.Ner.Person.PersonPropertyReferent>();
                                existProps.Add(a.Value as Pullenti.Ner.Person.PersonPropertyReferent);
                            }
                            continue;
                        }
                        if (pat.PropRef != null) 
                        {
                            foreach (Pullenti.Ner.Slot ss in pat.PropRef.Slots) 
                            {
                                if (ss.TypeName == Pullenti.Ner.Person.PersonPropertyReferent.ATTR_REF) 
                                {
                                    if (ss.Value is Pullenti.Ner.ReferentToken) 
                                    {
                                        if ((ss.Value as Pullenti.Ner.ReferentToken).Referent == referent) 
                                        {
                                            pat.PropRef.Slots.Remove(ss);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (existProps != null) 
                        {
                            foreach (Pullenti.Ner.Person.PersonPropertyReferent pp in existProps) 
                            {
                                if (pp.CanBeEquals(pat.PropRef, Pullenti.Ner.Core.ReferentsEqualType.WithinOneText)) 
                                {
                                    if (pat.PropRef.CanBeGeneralFor(pp)) 
                                    {
                                        pat.PropRef.MergeSlots(pp, true);
                                        break;
                                    }
                                }
                            }
                        }
                        pat.Data = this;
                        pat.SaveToLocalOntology();
                        if (pat.PropRef != null) 
                        {
                            if (referent.FindSlot(a.TypeName, pat.PropRef, true) != null) 
                            {
                                if (i >= 0 && (i < referent.Slots.Count)) 
                                {
                                    referent.Slots.RemoveAt(i);
                                    i--;
                                }
                            }
                            else 
                                referent.UploadSlot(a, pat.Referent);
                        }
                    }
                }
            }
            if (referent is Pullenti.Ner.Person.PersonPropertyReferent) 
            {
                for (int i = 0; i < referent.Slots.Count; i++) 
                {
                    Pullenti.Ner.Slot a = referent.Slots[i];
                    if (a.TypeName == Pullenti.Ner.Person.PersonPropertyReferent.ATTR_REF || a.TypeName == Pullenti.Ner.Person.PersonPropertyReferent.ATTR_HIGHER) 
                    {
                        Pullenti.Ner.ReferentToken pat = a.Value as Pullenti.Ner.ReferentToken;
                        if (pat != null) 
                        {
                            pat.Data = this;
                            pat.SaveToLocalOntology();
                            if (pat.Referent != null) 
                                referent.UploadSlot(a, pat.Referent);
                        }
                        else if (a.Value is Pullenti.Ner.Person.PersonPropertyReferent) 
                        {
                            if (a.Value == referent) 
                            {
                                referent.Slots.RemoveAt(i);
                                i--;
                                continue;
                            }
                            referent.UploadSlot(a, this.RegisterReferent(a.Value as Pullenti.Ner.Person.PersonPropertyReferent));
                        }
                    }
                }
            }
            Pullenti.Ner.Referent res = base.RegisterReferent(referent);
            return res;
        }
        public Dictionary<int, bool> CanBePersonPropBeginChars = new Dictionary<int, bool>();
        public bool ARegime = false;
    }
}