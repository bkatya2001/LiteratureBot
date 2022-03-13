/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Org.Internal
{
    class OrgTokenData
    {
        public OrgTokenData(Pullenti.Ner.Token t)
        {
            Tok = t;
            t.Tag = this;
        }
        public Pullenti.Ner.Token Tok;
        public OrgItemTypeToken Typ;
        public OrgItemTypeToken TypLow;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(Tok.ToString());
            if (Typ != null) 
                tmp.AppendFormat(" \r\nTyp: {0}", Typ.ToString());
            if (TypLow != null) 
                tmp.AppendFormat(" \r\nTypLow: {0}", TypLow.ToString());
            return tmp.ToString();
        }
    }
}