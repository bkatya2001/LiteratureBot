/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Address.Internal
{
    class MetaStreet : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaStreet();
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_TYP, "Тип", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_KIND, "Класс", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "Наименование", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_NUMBER, "Номер", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_SECNUMBER, "Доп.номер", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_HIGHER, "Вышележащая улица", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, "Географический объект", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_REF, "Ссылка на связанную сущность", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_FIAS, "Объект ФИАС", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_BTI, "Объект БТИ", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Address.StreetReferent.ATTR_OKM, "Код ОКМ УМ", 0, 1);
        }
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Address.StreetReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Улица";
            }
        }
        public static string ImageId = "street";
        public static string ImageTerrId = "territory";
        public static string ImageTerrOrgId = "terrorg";
        public static string ImageTerrSpecId = "terrspec";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            Pullenti.Ner.Address.StreetReferent s = obj as Pullenti.Ner.Address.StreetReferent;
            if (s != null) 
            {
                if (s.Kind == Pullenti.Ner.Address.StreetKind.Org) 
                    return ImageTerrOrgId;
                if (s.Kind == Pullenti.Ner.Address.StreetKind.Area) 
                    return ImageTerrId;
                if (s.Kind == Pullenti.Ner.Address.StreetKind.Spec) 
                    return ImageTerrSpecId;
            }
            return ImageId;
        }
        internal static MetaStreet GlobalMeta;
    }
}