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
        public static Ref<Concept<TEnum>.EEnum> GetRef<TEnum> (this TEnum Enum) where TEnum: Enum => new Concept<TEnum>(Enum);

        public static string GetDisplay<T> (this T Enum) where T: Enum
        {
            return Enum.GetType()
                    .GetMember(Enum.ToString())
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
