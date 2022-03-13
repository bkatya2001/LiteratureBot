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
    class GeoTokenData
    {
        public GeoTokenData(Pullenti.Ner.Token t)
        {
            Tok = t;
            t.Tag = this;
        }
        public Pullenti.Ner.Token Tok;
        public Pullenti.Ner.Core.NounPhraseToken Npt;
        public TerrItemToken Terr;
        public CityItemToken Cit;
        public OrgTypToken OrgTyp;
        public OrgItemToken Org;
        public Pullenti.Ner.Address.Internal.StreetItemToken Street;
        public Pullenti.Ner.Address.Internal.AddressItemToken Addr;
        public bool NoGeo = false;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(Tok.ToString());
            if (Npt != null) 
                tmp.AppendFormat(" \r\nNpt: {0}", Npt.ToString());
            if (Terr != null) 
                tmp.AppendFormat(" \r\nTerr: {0}", Terr.ToString());
            if (Cit != null) 
                tmp.AppendFormat(" \r\nCit: {0}", Cit.ToString());
            if (Org != null) 
                tmp.AppendFormat(" \r\nOrg: {0}", Org.ToString());
            if (OrgTyp != null) 
                tmp.AppendFormat(" \r\nOrgTyp: {0}", OrgTyp.ToString());
            if (Street != null) 
                tmp.AppendFormat(" \r\nStreet: {0}", Street.ToString());
            if (Addr != null) 
                tmp.AppendFormat(" \r\nAddr: {0}", Addr.ToString());
            if (NoGeo) 
                tmp.Append(" \r\nNO GEO!!!");
            return tmp.ToString();
        }
    }
}