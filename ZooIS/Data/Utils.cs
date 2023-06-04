using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ZooIS.Models;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace ZooIS.Data
{
    //Extensions
    public static class EnumExtensions
    {
        public static Ref<IEntity> GetRef<TEnum> (this TEnum Enum) where TEnum: Enum => new Ref<IEntity>(new Concept<TEnum>.EEnum(Enum));

        public static object GetValue<T>(this T E) where T : Enum
        {
            Type underlyingType = Enum.GetUnderlyingType(E.GetType());
            return Convert.ChangeType(E, underlyingType);
        }

        public static string GetDisplay<T> (this T E) where T: Enum
        {
            return E.GetType()
                    .GetMember(E.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()
                    .GetName();
        }
    }

    //Attributes
    /// <summary>
    /// Tells filters to ignore this controller or action.
    /// </summary>
    public class Ignore : Attribute { }
}
