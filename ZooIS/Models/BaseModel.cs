using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZooIS.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Humanizer;

namespace ZooIS.Models
{
    public interface IEntity
    {
        [NotMapped]
        [Display(Name ="Ключ")]
        /// <summary>
        /// A shortcut to primary key property.
        /// </summary>
        public object Key { get; }
        /// <summary>
        /// A shortcut to field that describes entity.
        /// </summary>
        [NotMapped]
        [Display(Name = "Отображение")]
        public string Display { get; }

        public string Enumerate(ICollection<IEntity> Collection)
        {
            return String.Join(", ", Collection.Select(e => e.Display));
        }
    }

    [Display(Name = "Ссылка")]
    public class Ref<T> where T : IEntity
    {
        [Display(Name = "Отображение")]
        public string Display { get; set; }
        [Display(Name = "Значение")]
        [Key]
        [Column(Order =2)]
        public object Id { get; set; }
        [Display(Name = "Тип")]
        [Key]
        [Column(Order =1)]
        public string Type { get => typeof(T).Name; }

        public Ref() { }

        public Ref(T Entity)
        {
            Id = Entity.Key;
            Display = Entity.Display;
        }
    }

    [Display(Name = "Сущность")]
    public abstract class Entity : IEntity
    {
        [Key]
        [Required]
        [Display(Name = "Guid")]
        public Guid Guid { get; set; }

        public Entity() : base()
        {
            this.Guid = Guid.NewGuid();
        }

        public Entity(Guid Guid) : base()
        {
            this.Guid = Guid;
        }

        public object Key { get => Guid; }

        public abstract string Display { get; }
    }

    [Display(Name = "Концепт")]
    public class Concept<TEnum> : Ref<Concept<TEnum>.EEnum> where TEnum : Enum
    {
        public Concept() : base(new EEnum()) { }

        public Concept(TEnum Enum) : base(new EEnum(Enum)) { }

        public static implicit operator Concept<TEnum>(TEnum Enum) => new(Enum);
        public static implicit operator TEnum(Concept<TEnum> Concept) => (TEnum)Concept.Id;

        /// <summary>
        /// Boxer for the enums, realizing IEntity interface. Used only to create Refs on Enums.
        /// </summary>
        [Display(Name = "Сущность - перечисление")]
        public class EEnum : IEntity
        {
            private object _Value;
            private string _Display;
            public object Key { get => _Value; }

            public string Display { get => _Display; }

            public EEnum() { }

            public EEnum(Enum Enum)
            {
                _Value = Enum.GetValue();
                _Display = Enum.GetDisplay();
            }
        }
    }

    /// <summary>
    /// Specifies that this DateTime property value cannot exceed current timestamp.
    /// </summary>
    public class NotFutureAttribute: ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not null && (DateTime)value > DateTime.Now)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Specifies that this DateTime property value cannot lag behind current timestamp.
    /// </summary>
    public class NotPastAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not null &&  (DateTime)value < DateTime.Now)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}
