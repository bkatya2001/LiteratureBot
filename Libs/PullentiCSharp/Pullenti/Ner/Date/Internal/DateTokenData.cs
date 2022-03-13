/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Ner.Date.Internal
{
    class DateTokenData
    {
        public DateTokenData(Pullenti.Ner.Token t)
        {
            Tok = t;
            t.Tag = this;
        }
        public Pullenti.Ner.Token Tok;
        public DateItemToken Dat;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(Tok.ToString());
            if (Dat != null) 
                tmp.AppendFormat(" \r\nDat: {0}", Dat.ToString());
            return tmp.ToString();
        }
    }
}