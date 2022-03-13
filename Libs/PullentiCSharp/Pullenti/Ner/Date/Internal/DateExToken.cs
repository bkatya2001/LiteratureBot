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

namespace Pullenti.Ner.Date.Internal
{
    // ВСЁ, этот класс теперь используется внутренним робразом, а DateReferent поддерживает относительные даты-время
    // Используется для нахождения в тексте абсолютных и относительных дат и диапазонов,
    // например, "в прошлом году", "за первый квартал этого года", "два дня назад и т.п."
    public class DateExToken : Pullenti.Ner.MetaToken
    {
        public DateExToken(Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
        {
        }
        public bool IsDiap = false;
        public List<DateExItemToken> ItemsFrom = new List<DateExItemToken>();
        public List<DateExItemToken> ItemsTo = new List<DateExItemToken>();
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            foreach (DateExItemToken it in ItemsFrom) 
            {
                tmp.AppendFormat("{0}{1}; ", (IsDiap ? "(fr)" : ""), it.ToString());
            }
            foreach (DateExItemToken it in ItemsTo) 
            {
                tmp.AppendFormat("(to){0}; ", it.ToString());
            }
            return tmp.ToString();
        }
        public DateTime? GetDate(DateTime now, int tense = 0)
        {
            DateValues dvl = DateValues.TryCreate((ItemsFrom.Count > 0 ? ItemsFrom : ItemsTo), now, tense);
            try 
            {
                DateTime dt = dvl.GenerateDate(now, false);
                if (dt == DateTime.MinValue) 
                    return null;
                dt = this._correctHours(dt, (ItemsFrom.Count > 0 ? ItemsFrom : ItemsTo), now);
                return dt;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public bool GetDates(DateTime now, out DateTime from, out DateTime to, int tense = 0)
        {
            from = DateTime.MinValue;
            to = DateTime.MinValue;
            bool hasHours = false;
            foreach (DateExItemToken it in ItemsFrom) 
            {
                if (it.Typ == DateExItemTokenType.Hour || it.Typ == DateExItemTokenType.Minute) 
                    hasHours = true;
            }
            foreach (DateExItemToken it in ItemsTo) 
            {
                if (it.Typ == DateExItemTokenType.Hour || it.Typ == DateExItemTokenType.Minute) 
                    hasHours = true;
            }
            List<DateExItemToken> li = new List<DateExItemToken>();
            if (hasHours) 
            {
                foreach (DateExItemToken it in ItemsFrom) 
                {
                    if (it.Typ != DateExItemTokenType.Hour && it.Typ != DateExItemTokenType.Minute) 
                        li.Add(it);
                }
                foreach (DateExItemToken it in ItemsTo) 
                {
                    if (it.Typ != DateExItemTokenType.Hour && it.Typ != DateExItemTokenType.Minute) 
                    {
                        bool exi = false;
                        foreach (DateExItemToken itt in li) 
                        {
                            if (itt.Typ == it.Typ) 
                            {
                                exi = true;
                                break;
                            }
                        }
                        if (!exi) 
                            li.Add(it);
                    }
                }
                // PYTHON: sort(key=attrgetter('typ'))
                li.Sort();
                DateValues dvl = DateValues.TryCreate(li, now, tense);
                if (dvl == null) 
                    return false;
                try 
                {
                    from = dvl.GenerateDate(now, false);
                    if (from == DateTime.MinValue) 
                        return false;
                }
                catch(Exception ex) 
                {
                    return false;
                }
                to = from;
                from = this._correctHours(from, ItemsFrom, now);
                to = this._correctHours(to, (ItemsTo.Count == 0 ? ItemsFrom : ItemsTo), now);
                return true;
            }
            bool grYear = false;
            foreach (DateExItemToken f in ItemsFrom) 
            {
                if (f.Typ == DateExItemTokenType.Century || f.Typ == DateExItemTokenType.Decade) 
                    grYear = true;
            }
            if (ItemsTo.Count == 0 && !grYear) 
            {
                DateValues dvl = DateValues.TryCreate(ItemsFrom, now, tense);
                if (dvl == null) 
                    return false;
                try 
                {
                    from = dvl.GenerateDate(now, false);
                    if (from == DateTime.MinValue) 
                        return false;
                }
                catch(Exception ex) 
                {
                    return false;
                }
                try 
                {
                    to = dvl.GenerateDate(now, true);
                    if (to == DateTime.MinValue) 
                        to = from;
                }
                catch(Exception ex) 
                {
                    to = from;
                }
                return true;
            }
            li.Clear();
            foreach (DateExItemToken it in ItemsFrom) 
            {
                li.Add(it);
            }
            foreach (DateExItemToken it in ItemsTo) 
            {
                bool exi = false;
                foreach (DateExItemToken itt in li) 
                {
                    if (itt.Typ == it.Typ) 
                    {
                        exi = true;
                        break;
                    }
                }
                if (!exi) 
                    li.Add(it);
            }
            // PYTHON: sort(key=attrgetter('typ'))
            li.Sort();
            DateValues dvl1 = DateValues.TryCreate(li, now, tense);
            li.Clear();
            foreach (DateExItemToken it in ItemsTo) 
            {
                li.Add(it);
            }
            foreach (DateExItemToken it in ItemsFrom) 
            {
                bool exi = false;
                foreach (DateExItemToken itt in li) 
                {
                    if (itt.Typ == it.Typ) 
                    {
                        exi = true;
                        break;
                    }
                }
                if (!exi) 
                    li.Add(it);
            }
            // PYTHON: sort(key=attrgetter('typ'))
            li.Sort();
            DateValues dvl2 = DateValues.TryCreate(li, now, tense);
            try 
            {
                from = dvl1.GenerateDate(now, false);
                if (from == DateTime.MinValue) 
                    return false;
            }
            catch(Exception ex) 
            {
                return false;
            }
            try 
            {
                to = dvl2.GenerateDate(now, true);
                if (to == DateTime.MinValue) 
                    return false;
            }
            catch(Exception ex) 
            {
                return false;
            }
            return true;
        }
        DateTime _correctHours(DateTime dt, List<DateExItemToken> li, DateTime now)
        {
            bool hasHour = false;
            foreach (DateExItemToken it in li) 
            {
                if (it.Typ == DateExItemTokenType.Hour) 
                {
                    hasHour = true;
                    if (it.IsValueRelate) 
                    {
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, now.Hour, now.Minute, 0);
                        dt = dt.AddHours(it.Value);
                    }
                    else if (it.Value > 0 && (it.Value < 24)) 
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, it.Value, 0, 0);
                }
                else if (it.Typ == DateExItemTokenType.Minute) 
                {
                    if (!hasHour) 
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, now.Hour, 0, 0);
                    if (it.IsValueRelate) 
                    {
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                        dt = dt.AddMinutes(it.Value);
                        if (!hasHour) 
                            dt = dt.AddMinutes(now.Minute);
                    }
                    else if (it.Value > 0 && (it.Value < 60)) 
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, it.Value, 0);
                }
            }
            return dt;
        }
        class DateValues
        {
            public int Day1;
            public int Day2;
            public int Month1;
            public int Month2;
            public int Year1;
            public int Year2;
            public override string ToString()
            {
                StringBuilder tmp = new StringBuilder();
                if (Year1 > 0) 
                {
                    tmp.AppendFormat("Year:{0}", Year1);
                    if (Year2 > Year1) 
                        tmp.AppendFormat("..{0}", Year2);
                }
                if (Month1 > 0) 
                {
                    tmp.AppendFormat(" Month:{0}", Month1);
                    if (Month2 > Month1) 
                        tmp.AppendFormat("..{0}", Month2);
                }
                if (Day1 > 0) 
                {
                    tmp.AppendFormat(" Day:{0}", Day1);
                    if (Day2 > Day1) 
                        tmp.AppendFormat("..{0}", Day2);
                }
                return tmp.ToString().Trim();
            }
            public DateTime GenerateDate(DateTime today, bool endOfDiap)
            {
                int year = Year1;
                if (year == 0) 
                    year = today.Year;
                if (endOfDiap && Year2 > Year1) 
                    year = Year2;
                if (year < 0) 
                    return DateTime.MinValue;
                int mon = Month1;
                if (mon == 0) 
                    mon = (endOfDiap ? 12 : 1);
                else if (endOfDiap && Month2 > 0) 
                    mon = Month2;
                int day = Day1;
                if (day == 0) 
                {
                    day = (endOfDiap ? 31 : 1);
                    if (day > DateTime.DaysInMonth(year, mon)) 
                        day = DateTime.DaysInMonth(year, mon);
                }
                else if (Day2 > 0 && endOfDiap) 
                    day = Day2;
                if (day > DateTime.DaysInMonth(year, mon)) 
                    return DateTime.MinValue;
                return new DateTime(year, mon, day);
            }
            public static DateValues TryCreate(List<Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken> list, DateTime today, int tense)
            {
                bool oo = false;
                if (list != null) 
                {
                    foreach (Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken v in list) 
                    {
                        if (v.Typ != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Hour && v.Typ != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Minute) 
                            oo = true;
                    }
                }
                if (!oo) 
                    return new DateValues() { Year1 = today.Year, Month1 = today.Month, Day1 = today.Day };
                if (list == null || list.Count == 0) 
                    return null;
                for (int j = 0; j < list.Count; j++) 
                {
                    if (list[j].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek) 
                    {
                        if (j > 0 && list[j - 1].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week) 
                            break;
                        Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken we = new Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken(list[j].BeginToken, list[j].EndToken) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week, IsValueRelate = true };
                        if (list[j].IsValueRelate) 
                        {
                            list[j].IsValueRelate = false;
                            if (list[j].Value < 0) 
                            {
                                we.Value = -1;
                                list[j].Value = -list[j].Value;
                            }
                        }
                        list.Insert(j, we);
                        break;
                    }
                }
                DateValues res = new DateValues();
                Pullenti.Ner.Date.Internal.DateExToken.DateExItemToken it;
                int i = 0;
                bool hasRel = false;
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Century) 
                {
                    it = list[i];
                    if (!it.IsValueRelate) 
                        res.Year1 = (((today.Year / 1000)) * 1000) + (it.Value * 100);
                    else 
                        res.Year1 = (((today.Year / 100)) * 100) + (it.Value * 100);
                    res.Year2 = res.Year1 + 99;
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Decade) 
                {
                    it = list[i];
                    if ((i > 0 && list[i - 1].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Century && !it.IsValueRelate) && (res.Year1 + 99) == res.Year2) 
                    {
                        res.Year1 += (((it.Value - 1)) * 10);
                        res.Year2 = res.Year1 + 9;
                    }
                    else if (!it.IsValueRelate) 
                        res.Year1 = (((today.Year / 100)) * 100) + (it.Value * 10);
                    else 
                        res.Year1 = (((today.Year / 10)) * 10) + (it.Value * 10);
                    res.Year2 = res.Year1 + 9;
                    return res;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year) 
                {
                    it = list[i];
                    if (!it.IsValueRelate) 
                        res.Year1 = it.Value;
                    else 
                    {
                        if (res.Year1 > 0 && res.Year2 > res.Year1 && it.Value >= 0) 
                        {
                            res.Year1 += it.Value;
                            res.Year2 = res.Year1;
                        }
                        else 
                            res.Year1 = today.Year + it.Value;
                        hasRel = true;
                    }
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Halfyear) 
                {
                    it = list[i];
                    if (!it.IsValueRelate) 
                    {
                        if (it.IsLast || it.Value == 2) 
                        {
                            res.Month1 = 7;
                            res.Month2 = 12;
                        }
                        else 
                        {
                            res.Month1 = 1;
                            res.Month2 = 6;
                        }
                    }
                    else 
                    {
                        int v = (today.Month > 6 ? 2 : 1);
                        v += it.Value;
                        while (v > 2) 
                        {
                            res.Year1 += 1;
                            v -= 2;
                        }
                        while (v < 1) 
                        {
                            res.Year1 -= 1;
                            v += 2;
                        }
                        if (v == 1) 
                        {
                            res.Month1 = 1;
                            res.Month2 = 6;
                        }
                        else 
                        {
                            res.Month1 = 7;
                            res.Month2 = 12;
                        }
                        hasRel = true;
                    }
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Quartal) 
                {
                    it = list[i];
                    int v = 0;
                    if (!it.IsValueRelate) 
                    {
                        if (res.Year1 == 0) 
                        {
                            int v0 = 1 + ((((today.Month - 1)) / 3));
                            if (it.Value > v0 && (tense < 0)) 
                                res.Year1 = today.Year - 1;
                            else if ((it.Value < v0) && tense > 0) 
                                res.Year1 = today.Year + 1;
                            else 
                                res.Year1 = today.Year;
                        }
                        v = it.Value;
                    }
                    else 
                    {
                        if (res.Year1 == 0) 
                            res.Year1 = today.Year;
                        v = 1 + ((((today.Month - 1)) / 3)) + it.Value;
                    }
                    while (v > 3) 
                    {
                        v -= 3;
                        res.Year1++;
                    }
                    while (v <= 0) 
                    {
                        v += 3;
                        res.Year1--;
                    }
                    res.Month1 = (((v - 1)) * 3) + 1;
                    res.Month2 = res.Month1 + 2;
                    return res;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Season) 
                {
                    it = list[i];
                    int v = 0;
                    if (!it.IsValueRelate) 
                    {
                        if (res.Year1 == 0) 
                        {
                            int v0 = 1 + ((((today.Month - 1)) / 3));
                            if (it.Value > v0 && (tense < 0)) 
                                res.Year1 = today.Year - 1;
                            else if ((it.Value < v0) && tense > 0) 
                                res.Year1 = today.Year + 1;
                            else 
                                res.Year1 = today.Year;
                        }
                        v = it.Value;
                    }
                    else 
                    {
                        if (res.Year1 == 0) 
                            res.Year1 = today.Year;
                        v = it.Value;
                    }
                    if (v == 1) 
                    {
                        res.Month1 = 12;
                        res.Year2 = res.Year1;
                        res.Year1--;
                        res.Month2 = 2;
                    }
                    else if (v == 2) 
                    {
                        res.Month1 = 3;
                        res.Month2 = 5;
                    }
                    else if (v == 3) 
                    {
                        res.Month1 = 6;
                        res.Month2 = 8;
                    }
                    else if (v == 4) 
                    {
                        res.Month1 = 9;
                        res.Month2 = 11;
                    }
                    else 
                        return null;
                    return res;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month) 
                {
                    it = list[i];
                    if (!it.IsValueRelate) 
                    {
                        if (res.Year1 == 0) 
                        {
                            if (it.Value > today.Month && (tense < 0)) 
                                res.Year1 = today.Year - 1;
                            else if ((it.Value < today.Month) && tense > 0) 
                                res.Year1 = today.Year + 1;
                            else 
                                res.Year1 = today.Year;
                        }
                        res.Month1 = it.Value;
                    }
                    else 
                    {
                        hasRel = true;
                        if (res.Year1 == 0) 
                            res.Year1 = today.Year;
                        int v = today.Month + it.Value;
                        while (v > 12) 
                        {
                            v -= 12;
                            res.Year1++;
                        }
                        while (v <= 0) 
                        {
                            v += 12;
                            res.Year1--;
                        }
                        res.Month1 = v;
                    }
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Weekend && i == 0) 
                {
                    it = list[i];
                    hasRel = true;
                    if (res.Year1 == 0) 
                        res.Year1 = today.Year;
                    if (res.Month1 == 0) 
                        res.Month1 = today.Month;
                    if (res.Day1 == 0) 
                        res.Day1 = today.Day;
                    DateTime dt0 = new DateTime(res.Year1, res.Month1, res.Day1);
                    DayOfWeek dow = dt0.DayOfWeek;
                    if (dow == DayOfWeek.Monday) 
                        dt0 = dt0.AddDays(5);
                    else if (dow == DayOfWeek.Tuesday) 
                        dt0 = dt0.AddDays(4);
                    else if (dow == DayOfWeek.Wednesday) 
                        dt0 = dt0.AddDays(3);
                    else if (dow == DayOfWeek.Thursday) 
                        dt0 = dt0.AddDays(2);
                    else if (dow == DayOfWeek.Friday) 
                        dt0 = dt0.AddDays(1);
                    else if (dow == DayOfWeek.Saturday) 
                        dt0 = dt0.AddDays(-1);
                    else if (dow == DayOfWeek.Sunday) 
                    {
                    }
                    if (it.Value != 0) 
                        dt0 = dt0.AddDays(it.Value * 7);
                    res.Year1 = dt0.Year;
                    res.Month1 = dt0.Month;
                    res.Day1 = dt0.Day;
                    dt0 = dt0.AddDays(1);
                    res.Year1 = dt0.Year;
                    res.Month2 = dt0.Month;
                    res.Day2 = dt0.Day;
                    i++;
                }
                if (((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week && i == 0) && list[i].IsValueRelate) 
                {
                    it = list[i];
                    hasRel = true;
                    if (res.Year1 == 0) 
                        res.Year1 = today.Year;
                    if (res.Month1 == 0) 
                        res.Month1 = today.Month;
                    if (res.Day1 == 0) 
                        res.Day1 = today.Day;
                    DateTime dt0 = new DateTime(res.Year1, res.Month1, res.Day1);
                    DayOfWeek dow = dt0.DayOfWeek;
                    if (dow == DayOfWeek.Tuesday) 
                        dt0 = dt0.AddDays(-1);
                    else if (dow == DayOfWeek.Wednesday) 
                        dt0 = dt0.AddDays(-2);
                    else if (dow == DayOfWeek.Thursday) 
                        dt0 = dt0.AddDays(-3);
                    else if (dow == DayOfWeek.Friday) 
                        dt0 = dt0.AddDays(-4);
                    else if (dow == DayOfWeek.Saturday) 
                        dt0 = dt0.AddDays(-5);
                    else if (dow == DayOfWeek.Sunday) 
                        dt0 = dt0.AddDays(-6);
                    if (it.Value != 0) 
                        dt0 = dt0.AddDays(it.Value * 7);
                    res.Year1 = dt0.Year;
                    res.Month1 = dt0.Month;
                    res.Day1 = dt0.Day;
                    dt0 = dt0.AddDays(6);
                    res.Year1 = dt0.Year;
                    res.Month2 = dt0.Month;
                    res.Day2 = dt0.Day;
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day) 
                {
                    it = list[i];
                    if (!it.IsValueRelate) 
                    {
                        res.Day1 = it.Value;
                        if (res.Month1 == 0) 
                        {
                            if (res.Year1 == 0) 
                                res.Year1 = today.Year;
                            if (it.Value > today.Day && (tense < 0)) 
                            {
                                res.Month1 = today.Month - 1;
                                if (res.Month1 <= 0) 
                                {
                                    res.Month1 = 12;
                                    res.Year1--;
                                }
                            }
                            else if ((it.Value < today.Day) && tense > 0) 
                            {
                                res.Month1 = today.Month + 1;
                                if (res.Month1 > 12) 
                                {
                                    res.Month1 = 1;
                                    res.Year1++;
                                }
                            }
                            else 
                                res.Month1 = today.Month;
                        }
                    }
                    else 
                    {
                        hasRel = true;
                        if (res.Year1 == 0) 
                            res.Year1 = today.Year;
                        if (res.Month1 == 0) 
                            res.Month1 = today.Month;
                        int v = today.Day + it.Value;
                        while (v > DateTime.DaysInMonth(res.Year1, res.Month1)) 
                        {
                            v -= DateTime.DaysInMonth(res.Year1, res.Month1);
                            res.Month1++;
                            if (res.Month1 > 12) 
                            {
                                res.Month1 = 1;
                                res.Year1++;
                            }
                        }
                        while (v <= 0) 
                        {
                            res.Month1--;
                            if (res.Month1 <= 0) 
                            {
                                res.Month1 = 12;
                                res.Year1--;
                            }
                            v += DateTime.DaysInMonth(res.Year1, res.Month1);
                        }
                        res.Day1 = v;
                    }
                    i++;
                }
                if ((i < list.Count) && list[i].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek) 
                {
                    it = list[i];
                    if ((i > 0 && list[i - 1].Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week && it.Value >= 1) && it.Value <= 7) 
                    {
                        res.Day1 = (res.Day1 + it.Value) - 1;
                        while (res.Day1 > DateTime.DaysInMonth(res.Year1, res.Month1)) 
                        {
                            res.Day1 -= DateTime.DaysInMonth(res.Year1, res.Month1);
                            res.Month1++;
                            if (res.Month1 > 12) 
                            {
                                res.Month1 = 1;
                                res.Year1++;
                            }
                        }
                        res.Day2 = res.Day1;
                        res.Month2 = res.Month1;
                        i++;
                    }
                }
                return res;
            }
        }

        public static DateExToken TryParse(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            if (t.IsValue("ЗА", null) && t.Next != null && t.Next.IsValue("ПЕРИОД", null)) 
            {
                DateExToken ne = TryParse(t.Next.Next);
                if (ne != null && ne.IsDiap) 
                {
                    ne.BeginToken = t;
                    return ne;
                }
            }
            DateExToken res = null;
            bool toRegime = false;
            bool fromRegime = false;
            Pullenti.Ner.Token t0 = null;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                Pullenti.Ner.Date.DateRangeReferent drr = tt.GetReferent() as Pullenti.Ner.Date.DateRangeReferent;
                if (drr != null) 
                {
                    res = new DateExToken(t, tt) { IsDiap = true };
                    Pullenti.Ner.Date.DateReferent fr = drr.DateFrom;
                    if (fr != null) 
                    {
                        if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                            return null;
                        _addItems(fr, res.ItemsFrom, tt);
                    }
                    Pullenti.Ner.Date.DateReferent to = drr.DateTo;
                    if (to != null) 
                    {
                        if (to.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                            return null;
                        _addItems(to, res.ItemsTo, tt);
                    }
                    bool hasYear = false;
                    if (res.ItemsFrom.Count > 0 && res.ItemsFrom[0].Typ == DateExItemTokenType.Year) 
                        hasYear = true;
                    else if (res.ItemsTo.Count > 0 && res.ItemsTo[0].Typ == DateExItemTokenType.Year) 
                        hasYear = true;
                    if (!hasYear && (tt.WhitespacesAfterCount < 3)) 
                    {
                        DateExItemToken dit = DateExItemToken.TryParse(tt.Next, (res.ItemsTo.Count > 0 ? res.ItemsTo : res.ItemsFrom), 0, false);
                        if (dit != null && dit.Typ == DateExItemTokenType.Year) 
                        {
                            if (res.ItemsFrom.Count > 0) 
                                res.ItemsFrom.Insert(0, dit);
                            if (res.ItemsTo.Count > 0) 
                                res.ItemsTo.Insert(0, dit);
                            res.EndToken = dit.EndToken;
                        }
                    }
                    return res;
                }
                Pullenti.Ner.Date.DateReferent dr = tt.GetReferent() as Pullenti.Ner.Date.DateReferent;
                if (dr != null) 
                {
                    if (dr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                        return null;
                    if (res == null) 
                        res = new DateExToken(t, tt);
                    List<DateExItemToken> li = new List<DateExItemToken>();
                    _addItems(dr, li, tt);
                    if (li.Count == 0) 
                        continue;
                    if (toRegime) 
                    {
                        bool ok = true;
                        foreach (DateExItemToken v in li) 
                        {
                            foreach (DateExItemToken vv in res.ItemsTo) 
                            {
                                if (vv.Typ == v.Typ) 
                                    ok = false;
                            }
                        }
                        if (!ok) 
                            break;
                        res.ItemsTo.AddRange(li);
                        res.EndToken = tt;
                    }
                    else 
                    {
                        bool ok = true;
                        foreach (DateExItemToken v in li) 
                        {
                            foreach (DateExItemToken vv in res.ItemsFrom) 
                            {
                                if (vv.Typ == v.Typ) 
                                    ok = false;
                            }
                        }
                        if (!ok) 
                            break;
                        res.ItemsFrom.AddRange(li);
                        res.EndToken = tt;
                    }
                    bool hasYear = false;
                    if (res.ItemsFrom.Count > 0 && res.ItemsFrom[0].Typ == DateExItemTokenType.Year) 
                        hasYear = true;
                    else if (res.ItemsTo.Count > 0 && res.ItemsTo[0].Typ == DateExItemTokenType.Year) 
                        hasYear = true;
                    if (!hasYear && (tt.WhitespacesAfterCount < 3)) 
                    {
                        DateExItemToken dit = DateExItemToken.TryParse(tt.Next, null, 0, false);
                        if (dit != null && dit.Typ == DateExItemTokenType.Year) 
                        {
                            if (res.ItemsFrom.Count > 0) 
                                res.ItemsFrom.Insert(0, dit);
                            if (res.ItemsTo.Count > 0) 
                                res.ItemsTo.Insert(0, dit);
                            tt = (res.EndToken = dit.EndToken);
                        }
                    }
                    continue;
                }
                if (tt.Morph.Class.IsPreposition) 
                {
                    if (tt.IsValue("ПО", null) || tt.IsValue("ДО", null)) 
                    {
                        toRegime = true;
                        if (t0 == null) 
                            t0 = tt;
                    }
                    else if (tt.IsValue("С", null) || tt.IsValue("ОТ", null)) 
                    {
                        fromRegime = true;
                        if (t0 == null) 
                            t0 = tt;
                    }
                    continue;
                }
                DateExItemToken it = DateExItemToken.TryParse(tt, (res == null ? null : (toRegime ? res.ItemsTo : res.ItemsFrom)), 0, false);
                if (it == null) 
                    break;
                if (tt.IsValue("ДЕНЬ", null) && tt.Next != null && tt.Next.IsValue("НЕДЕЛЯ", null)) 
                    break;
                if (it.EndToken == tt && ((it.Typ == DateExItemTokenType.Hour || it.Typ == DateExItemTokenType.Minute))) 
                {
                    if (tt.Previous == null || !tt.Previous.Morph.Class.IsPreposition) 
                        break;
                }
                if (res == null) 
                {
                    if ((it.Typ == DateExItemTokenType.Day || it.Typ == DateExItemTokenType.Month || it.Typ == DateExItemTokenType.Week) || it.Typ == DateExItemTokenType.Quartal || it.Typ == DateExItemTokenType.Year) 
                    {
                        if (it.BeginToken == it.EndToken && !it.IsValueRelate && it.Value == 0) 
                            return null;
                    }
                    res = new DateExToken(t, tt);
                }
                if (toRegime) 
                    res.ItemsTo.Add(it);
                else 
                {
                    res.ItemsFrom.Add(it);
                    if (it.IsLast && it.Value != 0 && it.Value != -1) 
                    {
                        res.ItemsTo.Add(new DateExItemToken(it.BeginToken, it.EndToken) { Typ = it.Typ, IsValueRelate = true });
                        fromRegime = true;
                    }
                }
                tt = it.EndToken;
                res.EndToken = tt;
            }
            if (res != null) 
            {
                if (t0 != null && res.BeginToken.Previous == t0) 
                    res.BeginToken = t0;
                res.IsDiap = fromRegime || toRegime;
                // PYTHON: sort(key=attrgetter('typ'))
                res.ItemsFrom.Sort();
                // PYTHON: sort(key=attrgetter('typ'))
                res.ItemsTo.Sort();
                if ((res.ItemsFrom.Count == 1 && res.ItemsTo.Count == 0 && res.ItemsFrom[0].IsLast) && res.ItemsFrom[0].Value == 0) 
                    return null;
            }
            return res;
        }
        static void _addItems(Pullenti.Ner.Date.DateReferent fr, List<DateExItemToken> res, Pullenti.Ner.Token tt)
        {
            if (fr.Century > 0) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Century, Value = fr.Century, Src = fr });
            if (fr.Decade > 0) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Decade, Value = fr.Decade, Src = fr });
            if (fr.Year > 0) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Year, Value = fr.Year, Src = fr });
            else if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Year, Value = 0, IsValueRelate = true });
            if (fr.Month > 0) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Month, Value = fr.Month, Src = fr });
            else if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Month, Value = 0, IsValueRelate = true });
            if (fr.Day > 0) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Day, Value = fr.Day, Src = fr });
            else if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Day, Value = 0, IsValueRelate = true });
            if (fr.FindSlot(Pullenti.Ner.Date.DateReferent.ATTR_HOUR, null, true) != null) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Hour, Value = fr.Hour, Src = fr });
            else if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Hour, Value = 0, IsValueRelate = true });
            if (fr.FindSlot(Pullenti.Ner.Date.DateReferent.ATTR_MINUTE, null, true) != null) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Minute, Value = fr.Minute, Src = fr });
            else if (fr.Pointer == Pullenti.Ner.Date.DatePointerType.Today) 
                res.Add(new DateExItemToken(tt, tt) { Typ = DateExItemTokenType.Minute, Value = 0, IsValueRelate = true });
        }
        public class DateExItemToken : Pullenti.Ner.MetaToken, IComparable<DateExItemToken>
        {
            public DateExItemToken(Pullenti.Ner.Token begin, Pullenti.Ner.Token end) : base(begin, end, null)
            {
            }
            public Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Undefined;
            public int Value;
            public bool IsValueRelate;
            public bool IsLast;
            public bool IsValueNotstrict;
            public Pullenti.Ner.Date.DateReferent Src;
            public override string ToString()
            {
                StringBuilder tmp = new StringBuilder();
                tmp.AppendFormat("{0} ", Typ);
                if (IsValueNotstrict) 
                    tmp.Append("~");
                if (IsValueRelate) 
                    tmp.AppendFormat("{0}{1}{2}", (Value < 0 ? "" : "+"), Value, (IsLast ? " (last)" : ""));
                else 
                    tmp.Append(Value);
                return tmp.ToString();
            }
            public static DateExItemToken TryParse(Pullenti.Ner.Token t, List<DateExItemToken> prev, int level = 0, bool noCorrAfter = false)
            {
                if (t == null || level > 10) 
                    return null;
                if (t.IsValue("СЕГОДНЯ", "СЬОГОДНІ")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = 0, IsValueRelate = true };
                if (t.IsValue("ЗАВТРА", null)) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = 1, IsValueRelate = true };
                if (t.IsValue("ЗАВТРАШНИЙ", "ЗАВТРАШНІЙ") && t.Next != null && t.Next.IsValue("ДЕНЬ", null)) 
                    return new DateExItemToken(t, t.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = 1, IsValueRelate = true };
                if (t.IsValue("ПОСЛЕЗАВТРА", "ПІСЛЯЗАВТРА")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = 2, IsValueRelate = true };
                if (t.IsValue("ПОСЛЕЗАВТРАШНИЙ", "ПІСЛЯЗАВТРАШНІЙ") && t.Next != null && t.Next.IsValue("ДЕНЬ", null)) 
                    return new DateExItemToken(t, t.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = 2, IsValueRelate = true };
                if (t.IsValue("ВЧЕРА", "ВЧОРА")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = -1, IsValueRelate = true };
                if (t.IsValue("ВЧЕРАШНИЙ", "ВЧОРАШНІЙ") && t.Next != null && t.Next.IsValue("ДЕНЬ", null)) 
                    return new DateExItemToken(t, t.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = -1, IsValueRelate = true };
                if (t.IsValue("ПОЗАВЧЕРА", "ПОЗАВЧОРА")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = -2, IsValueRelate = true };
                if (t.IsValue("ПОЗАВЧЕРАШНИЙ", "ПОЗАВЧОРАШНІЙ") && t.Next != null && t.Next.IsValue("ДЕНЬ", null)) 
                    return new DateExItemToken(t, t.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = -2, IsValueRelate = true };
                if (t.IsValue("ПОЛЧАСА", "ПІВГОДИНИ")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Minute, Value = 30, IsValueRelate = true };
                if (t.IsValue("ЗИМА", null)) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Season, Value = 1 };
                if (t.IsValue("ВЕСНА", null)) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Season, Value = 2 };
                if (t.IsValue("ЛЕТО", "ЛІТО") && !t.IsValue("ЛЕТ", null)) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Season, Value = 3 };
                if (t.IsValue("ОСЕНЬ", "ОСЕНІ")) 
                    return new DateExItemToken(t, t) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Season, Value = 4 };
                if (prev != null && prev.Count > 0) 
                {
                    if (((t.IsValue("Т", null) && t.Next != null && t.Next.IsChar('.')) && t.Next.Next != null && t.Next.Next.IsValue("Г", null)) && t.Next.Next.Next != null && t.Next.Next.Next.IsChar('.')) 
                        return new DateExItemToken(t, t.Next.Next.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year, IsValueRelate = true };
                }
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(t, Pullenti.Ner.Core.NounPhraseParseAttr.ParseNumericAsAdjective | Pullenti.Ner.Core.NounPhraseParseAttr.ParsePreposition, 0, null);
                if (npt != null && npt.BeginToken == npt.EndToken) 
                {
                    if (npt.EndToken.IsValue("ПРОШЛЫЙ", "МИНУЛИЙ") || npt.EndToken.IsValue("БУДУЩИЙ", "МАЙБУТНІЙ")) 
                        npt = null;
                }
                if (npt == null) 
                {
                    if ((t is Pullenti.Ner.NumberToken) && (t as Pullenti.Ner.NumberToken).IntValue != null) 
                    {
                        DateExItemToken res0 = TryParse(t.Next, prev, level + 1, true);
                        if (res0 != null && ((res0.Value == 1 || res0.Value == 0))) 
                        {
                            res0.BeginToken = t;
                            res0.Value = (t as Pullenti.Ner.NumberToken).IntValue.Value;
                            if (t.Previous != null && ((t.Previous.IsValue("ЧЕРЕЗ", null) || t.Previous.IsValue("СПУСТЯ", null)))) 
                                res0.IsValueRelate = true;
                            else if (res0.EndToken.Next != null) 
                            {
                                if (res0.EndToken.Next.IsValue("СПУСТЯ", null)) 
                                {
                                    res0.IsValueRelate = true;
                                    res0.EndToken = res0.EndToken.Next;
                                }
                                else if (res0.EndToken.Next.IsValue("НАЗАД", null)) 
                                {
                                    res0.IsValueRelate = true;
                                    res0.Value = -res0.Value;
                                    res0.EndToken = res0.EndToken.Next;
                                }
                                else if (res0.EndToken.Next.IsValue("ТОМУ", null) && res0.EndToken.Next.Next != null && res0.EndToken.Next.Next.IsValue("НАЗАД", null)) 
                                {
                                    res0.IsValueRelate = true;
                                    res0.Value = -res0.Value;
                                    res0.EndToken = res0.EndToken.Next.Next;
                                }
                            }
                            return res0;
                        }
                        Pullenti.Ner.Date.Internal.DateItemToken dtt = Pullenti.Ner.Date.Internal.DateItemToken.TryParse(t, null, false);
                        if (dtt != null && dtt.Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Year) 
                            return new DateExItemToken(t, dtt.EndToken) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year, Value = dtt.IntValue };
                        if (t.Next != null && t.Next.IsValue("ЧИСЛО", null)) 
                        {
                            DateExItemToken ne = TryParse(t.Next.Next, prev, level + 1, false);
                            if (ne != null && ne.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month) 
                                return new DateExItemToken(t, t.Next) { Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day, Value = (t as Pullenti.Ner.NumberToken).IntValue.Value };
                        }
                    }
                    int delt = 0;
                    bool ok = true;
                    bool last = false;
                    Pullenti.Ner.Token t1 = t;
                    if (t.IsValue("СЛЕДУЮЩИЙ", "НАСТУПНИЙ") || t.IsValue("БУДУЩИЙ", "МАЙБУТНІЙ") || t.IsValue("БЛИЖАЙШИЙ", "НАЙБЛИЖЧИЙ")) 
                        delt = 1;
                    else if (t.IsValue("ПРЕДЫДУЩИЙ", "ПОПЕРЕДНІЙ") || t.IsValue("ПРОШЛЫЙ", "МИНУЛИЙ") || t.IsValue("ПРОШЕДШИЙ", null)) 
                        delt = -1;
                    else if (t.IsValue("ПОЗАПРОШЛЫЙ", "ПОЗАМИНУЛИЙ")) 
                        delt = -2;
                    else if (t.IsValue("ЭТОТ", "ЦЕЙ") || t.IsValue("ТЕКУЩИЙ", "ПОТОЧНИЙ")) 
                    {
                    }
                    else if (t.IsValue("ПОСЛЕДНИЙ", "ОСТАННІЙ")) 
                    {
                        last = true;
                        if (t.Next is Pullenti.Ner.NumberToken) 
                        {
                            delt = (t.Next as Pullenti.Ner.NumberToken).IntValue.Value;
                            t1 = t.Next;
                            DateExItemToken next = TryParse(t1.Next, null, 0, false);
                            if (next != null && next.Value == 0) 
                            {
                                next.BeginToken = t;
                                next.IsLast = true;
                                next.Value = -delt;
                                next.IsValueRelate = true;
                                return next;
                            }
                        }
                        else 
                        {
                            DateExItemToken next = TryParse(t.Next, null, 0, false);
                            if (next != null && next.Value == 0) 
                            {
                                next.BeginToken = t;
                                next.IsLast = true;
                                next.IsValueRelate = true;
                                if (next.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Halfyear) 
                                {
                                    next.Value = 2;
                                    next.IsValueRelate = false;
                                }
                                return next;
                            }
                        }
                    }
                    else 
                        ok = false;
                    if (ok) 
                    {
                        for (Pullenti.Ner.Token tt = t.Previous; tt != null; tt = tt.Previous) 
                        {
                            if (tt.IsNewlineAfter) 
                                break;
                            Pullenti.Ner.Date.DateReferent dr = tt.GetReferent() as Pullenti.Ner.Date.DateReferent;
                            if (dr != null && dr.IsRelative) 
                            {
                                Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Undefined;
                                foreach (Pullenti.Ner.Slot s in dr.Slots) 
                                {
                                    if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_MONTH) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_YEAR) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_DAY) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_WEEK) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_CENTURY) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Century;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_QUARTAL) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Quartal;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_HALFYEAR) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Halfyear;
                                    else if (s.TypeName == Pullenti.Ner.Date.DateReferent.ATTR_DECADE) 
                                        ty0 = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Decade;
                                }
                                if (ty0 != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Undefined) 
                                    return new DateExItemToken(t, t) { Typ = ty0, Value = delt, IsValueRelate = true };
                            }
                            if (Pullenti.Ner.Core.MiscHelper.CanBeStartOfSentence(tt)) 
                                break;
                        }
                    }
                    return null;
                }
                Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Hour;
                int val = 0;
                if (npt.Noun.IsValue("ГОД", "РІК") || npt.Noun.IsValue("ГОДИК", null) || npt.Noun.IsValue("ЛЕТ", null)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year;
                else if (npt.Noun.IsValue("ПОЛГОДА", "ПІВРОКУ") || npt.Noun.IsValue("ПОЛУГОДИЕ", "ПІВРІЧЧЯ")) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Halfyear;
                else if (npt.Noun.IsValue("ВЕК", null) || npt.Noun.IsValue("СТОЛЕТИЕ", "СТОЛІТТЯ")) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Century;
                else if (npt.Noun.IsValue("КВАРТАЛ", null)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Quartal;
                else if (npt.Noun.IsValue("ДЕСЯТИЛЕТИЕ", "ДЕСЯТИЛІТТЯ") || npt.Noun.IsValue("ДЕКАДА", null)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Decade;
                else if (npt.Noun.IsValue("МЕСЯЦ", "МІСЯЦЬ")) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month;
                else if (npt.Noun.IsValue("ДЕНЬ", null) || npt.Noun.IsValue("ДЕНЕК", null)) 
                {
                    if (npt.EndToken.Next != null && npt.EndToken.Next.IsValue("НЕДЕЛЯ", "ТИЖДЕНЬ")) 
                        return null;
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                }
                else if (npt.Noun.IsValue("ЧИСЛО", null) && npt.Adjectives.Count > 0 && (npt.Adjectives[0].BeginToken is Pullenti.Ner.NumberToken)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                else if (npt.Noun.IsValue("НЕДЕЛЯ", "ТИЖДЕНЬ") || npt.Noun.IsValue("НЕДЕЛЬКА", null)) 
                {
                    if (t.Previous != null && t.Previous.IsValue("ДЕНЬ", null)) 
                        return null;
                    if (t.Previous != null && ((t.Previous.IsValue("ЗА", null) || t.Previous.IsValue("НА", null)))) 
                        ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week;
                    else if (t.IsValue("ЗА", null) || t.IsValue("НА", null)) 
                        ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week;
                    else 
                        ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week;
                }
                else if (npt.Noun.IsValue("ВЫХОДНОЙ", "ВИХІДНИЙ")) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Weekend;
                else if (npt.Noun.IsValue("ЧАС", "ГОДИНА") || npt.Noun.IsValue("ЧАСИК", null) || npt.Noun.IsValue("ЧАСОК", null)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Hour;
                else if (npt.Noun.IsValue("МИНУТА", "ХВИЛИНА") || npt.Noun.IsValue("МИНУТКА", null)) 
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Minute;
                else if (npt.Noun.IsValue("ПОНЕДЕЛЬНИК", "ПОНЕДІЛОК")) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 1;
                }
                else if (npt.Noun.IsValue("ВТОРНИК", "ВІВТОРОК")) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 2;
                }
                else if (npt.Noun.IsValue("СРЕДА", "СЕРЕДА")) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 3;
                }
                else if (npt.Noun.IsValue("ЧЕТВЕРГ", "ЧЕТВЕР")) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 4;
                }
                else if (npt.Noun.IsValue("ПЯТНИЦЯ", null)) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 5;
                }
                else if (npt.Noun.IsValue("СУББОТА", "СУБОТА")) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 6;
                }
                else if (npt.Noun.IsValue("ВОСКРЕСЕНЬЕ", "НЕДІЛЯ") || npt.Noun.IsValue("ВОСКРЕСЕНИЕ", null)) 
                {
                    ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek;
                    val = 7;
                }
                else 
                {
                    Pullenti.Ner.Date.Internal.DateItemToken dti = Pullenti.Ner.Date.Internal.DateItemToken.TryParse(npt.EndToken, null, false);
                    if (dti != null && dti.Typ == Pullenti.Ner.Date.Internal.DateItemToken.DateItemType.Month) 
                    {
                        ty = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month;
                        val = dti.IntValue;
                    }
                    else 
                        return null;
                }
                DateExItemToken res = new DateExItemToken(t, npt.EndToken) { Typ = ty, Value = val };
                bool heg = false;
                for (int i = 0; i < npt.Adjectives.Count; i++) 
                {
                    Pullenti.Ner.MetaToken a = npt.Adjectives[i];
                    if (a.IsValue("СЛЕДУЮЩИЙ", "НАСТУПНИЙ") || a.IsValue("БУДУЩИЙ", "МАЙБУТНІЙ") || a.IsValue("БЛИЖАЙШИЙ", "НАЙБЛИЖЧИЙ")) 
                    {
                        if (res.Value == 0 && ty != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Weekend) 
                            res.Value = 1;
                        res.IsValueRelate = true;
                    }
                    else if (a.IsValue("ПРЕДЫДУЩИЙ", "ПОПЕРЕДНІЙ") || a.IsValue("ПРОШЛЫЙ", "МИНУЛИЙ") || a.IsValue("ПРОШЕДШИЙ", null)) 
                    {
                        if (res.Value == 0) 
                            res.Value = 1;
                        res.IsValueRelate = true;
                        heg = true;
                    }
                    else if (a.IsValue("ПОЗАПРОШЛЫЙ", "ПОЗАМИНУЛИЙ")) 
                    {
                        if (res.Value == 0) 
                            res.Value = 2;
                        res.IsValueRelate = true;
                        heg = true;
                    }
                    else if (a.BeginToken == a.EndToken && (a.BeginToken is Pullenti.Ner.NumberToken) && (a.BeginToken as Pullenti.Ner.NumberToken).IntValue != null) 
                    {
                        if (res.Typ != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek) 
                            res.Value = (a.BeginToken as Pullenti.Ner.NumberToken).IntValue.Value;
                    }
                    else if (a.IsValue("ЭТОТ", "ЦЕЙ") || a.IsValue("ТЕКУЩИЙ", "ПОТОЧНИЙ")) 
                        res.IsValueRelate = true;
                    else if (a.IsValue("ПЕРВЫЙ", "ПЕРШИЙ")) 
                        res.Value = 1;
                    else if (a.IsValue("ПОСЛЕДНИЙ", "ОСТАННІЙ")) 
                    {
                        res.IsValueRelate = true;
                        res.IsLast = true;
                        if (((i + 1) < npt.Adjectives.Count) && (npt.Adjectives[i + 1].BeginToken is Pullenti.Ner.NumberToken) && (npt.Adjectives[i + 1].BeginToken as Pullenti.Ner.NumberToken).IntValue != null) 
                        {
                            i++;
                            res.Value = -(npt.Adjectives[i].BeginToken as Pullenti.Ner.NumberToken).IntValue.Value;
                            res.IsLast = true;
                        }
                        else if (i > 0 && (npt.Adjectives[i - 1].BeginToken is Pullenti.Ner.NumberToken) && (npt.Adjectives[i - 1].BeginToken as Pullenti.Ner.NumberToken).IntValue != null) 
                        {
                            res.Value = -(npt.Adjectives[i - 1].BeginToken as Pullenti.Ner.NumberToken).IntValue.Value;
                            res.IsLast = true;
                        }
                    }
                    else if (a.IsValue("ПРЕДПОСЛЕДНИЙ", "ПЕРЕДОСТАННІЙ")) 
                    {
                        res.IsValueRelate = true;
                        res.IsLast = true;
                        res.Value = -1;
                    }
                    else if (a.IsValue("БЛИЖАЙШИЙ", "НАЙБЛИЖЧИЙ") && res.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.DayOfWeek) 
                    {
                    }
                    else 
                        return null;
                }
                if (npt.Anafor != null) 
                {
                    if (npt.Anafor.IsValue("ЭТОТ", "ЦЕЙ")) 
                    {
                        if (npt.Morph.Number != Pullenti.Morph.MorphNumber.Singular) 
                            return null;
                        if (res.Value == 0) 
                            res.IsValueRelate = true;
                        if (prev == null || prev.Count == 0) 
                        {
                            if (t.Previous != null && t.Previous.GetMorphClassInDictionary().IsPreposition) 
                            {
                            }
                            else if (t.GetMorphClassInDictionary().IsPreposition) 
                            {
                            }
                            else if (ty == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year || ty == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month || ty == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week) 
                            {
                            }
                            else 
                                return null;
                        }
                    }
                    else 
                        return null;
                }
                bool ch = false;
                if (!noCorrAfter && res.EndToken.Next != null) 
                {
                    Pullenti.Ner.Token tt = res.EndToken.Next;
                    Pullenti.Ner.Token tt0 = res.BeginToken;
                    if (tt.IsValue("СПУСТЯ", null) || tt0.IsValue("СПУСТЯ", null) || tt0.IsValue("ЧЕРЕЗ", null)) 
                    {
                        ch = true;
                        res.IsValueRelate = true;
                        if (res.Value == 0) 
                            res.Value = 1;
                        res.EndToken = tt;
                    }
                    else if (tt.IsValue("НАЗАД", null)) 
                    {
                        ch = true;
                        res.IsValueRelate = true;
                        if (res.Value == 0) 
                            res.Value = -1;
                        else 
                            res.Value = -res.Value;
                        res.EndToken = tt;
                    }
                    else if (tt.IsValue("ТОМУ", null) && tt.Next != null && tt.Next.IsValue("НАЗАД", null)) 
                    {
                        ch = true;
                        res.IsValueRelate = true;
                        if (res.Value == 0) 
                            res.Value = -1;
                        else 
                            res.Value = -res.Value;
                        res.EndToken = tt.Next;
                    }
                }
                if (heg) 
                    res.Value = -res.Value;
                if (t.Previous != null) 
                {
                    if (t.Previous.IsValue("ЧЕРЕЗ", null) || t.Previous.IsValue("СПУСТЯ", null)) 
                    {
                        res.IsValueRelate = true;
                        if (res.Value == 0) 
                            res.Value = 1;
                        res.BeginToken = t.Previous;
                        ch = true;
                    }
                    else if (t.Previous.IsValue("ЗА", null) && res.Value == 0) 
                    {
                        if (!npt.Morph.Case.IsAccusative) 
                            return null;
                        if (npt.EndToken.Next != null && npt.EndToken.Next.IsValue("ДО", null)) 
                            return null;
                        if (npt.BeginToken == npt.EndToken) 
                            return null;
                        if (!res.IsLast) 
                        {
                            res.IsValueRelate = true;
                            ch = true;
                        }
                    }
                }
                if (res.BeginToken == res.EndToken) 
                {
                    if (t.Previous != null && t.Previous.IsValue("ПО", null)) 
                        return null;
                }
                if (ch && res.Typ != Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day) 
                {
                    if (res.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Week) 
                    {
                        res.Value *= 7;
                        res.Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                    }
                    else if (res.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Month) 
                    {
                        res.Value *= 30;
                        res.Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                    }
                    else if (res.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Quartal) 
                    {
                        res.Value *= 91;
                        res.Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                    }
                    else if (res.Typ == Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Year) 
                    {
                        res.Value *= 365;
                        res.Typ = Pullenti.Ner.Date.Internal.DateExToken.DateExItemTokenType.Day;
                    }
                }
                return res;
            }
            public int CompareTo(DateExItemToken other)
            {
                if (((int)Typ) < ((int)other.Typ)) 
                    return -1;
                if (((int)Typ) > ((int)other.Typ)) 
                    return 1;
                return 0;
            }
        }

        public enum DateExItemTokenType : int
        {
            Undefined = 0,
            Century = 1,
            Decade = 2,
            Year = 3,
            Halfyear = 4,
            Quartal = 5,
            Season = 6,
            Month = 7,
            Week = 8,
            Day = 9,
            DayOfWeek = 10,
            Hour = 11,
            Minute = 12,
            Weekend = 13,
        }

    }
}