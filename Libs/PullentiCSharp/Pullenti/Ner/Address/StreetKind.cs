/*
 * SDK Pullenti Lingvo, version 4.11, january 2022. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Address
{
    /// <summary>
    /// Классы улиц
    /// </summary>
    public enum StreetKind : int
    {
        /// <summary>
        /// Обычная улица-переулок-площадь
        /// </summary>
        Undefined,
        /// <summary>
        /// Автодорога
        /// </summary>
        Road,
        /// <summary>
        /// Железная дорога
        /// </summary>
        Railway,
        /// <summary>
        /// Станция метро
        /// </summary>
        Metro,
        /// <summary>
        /// Районы, кварталы
        /// </summary>
        Area,
        /// <summary>
        /// Территория организации или иного объекта
        /// </summary>
        Org,
        /// <summary>
        /// Спецобъекты (будки, казармы)
        /// </summary>
        Spec,
    }
}