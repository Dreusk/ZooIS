using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ZooIS.Models
{
    [Display(Name ="Вид")]
    public class Species: Entity
    {
        [MaxLength(100)]
        [Required]
        [Display(Name = "Научное название")]
        public string ScientificName { get; set; }
        /// <summary>
        /// Also known as folk name.
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "Народное название")]
        public string? VernacularName { get; set; }
        [Display(Name = "Представители вида")]
        public virtual HashSet<Animal> Animals { get; set; } = new();

        public override string Display { get => VernacularName ?? ScientificName; }

        public Species(string ScientificName): base() => this.ScientificName = ScientificName;
    }

    public enum Sex {
        [Display(Name ="Неопределен")]
        Undetermined,
		[Display(Name ="Самец")]
        Male,
        [Display(Name ="Самка")]
        Female}

    [Display(Name = "Животное")]
    public class Animal : Entity
    {
        [MaxLength(60)]
        [Display(Name = "Кличка")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Возраст")]
        public uint? Age { get; set; }
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }
        public Guid SpeciesGuid { get; set; }
        [Required]
        [Display(Name = "Вид")]
        public virtual Species Species { get; set; }
        [Required]
        [Display(Name = "Пол")]
        public Sex Sex { get; set; }
        [Display(Name = "Состояние")]
        public Status Status { get; set; } = new();
        [Display(Name ="Родители")]
        public virtual HashSet<Animal> Parents { get; set; } = new();
        [Display(Name ="Дети")]
        public virtual HashSet<Animal> Children { get; set; }  = new();
		[Display(Name ="Любима еда")]
		public virtual HashSet<Food> FavouriteFood { get; set; }  = new();
		[Display(Name ="Характер")]
		public virtual HashSet<Characterization> Characterization { get; set; }  = new();

        public override string Display { get => this.Name; }

        public Animal(string Name) => this.Name = Name;
    }
	
	public enum Health {
		[Display(Name="Здоров(а)")]
		Healthy,
		[Display(Name="Болен(а)")]
		Sick,
		[Display(Name="Мертв(а)")]
		Dead }
	
	[ComplexType]
    [Display(Name = "Состояние")]
    public class Status {
        [Display(Name = "Здоровье")]
        public Health Health { get; set; }
        [Key]
        public Guid AnimalGuid { get; set; }
        [Required]
        [Display(Name = "Животное")]
        public virtual Animal Animal { get; set; }
	}

    [Display(Name = "Характеризация")]
    public class Characterization : Entity {
        [Required]
        [Display(Name = "Текст")]
        [MaxLength(1000)]
        public string Text { get; set; }
        [Display(Name = "Метка времени")]
        public DateTime ts { get; set; } = DateTime.Now;
        public Guid AuthorGuid { get; set; }
        [ForeignKey("AuthorGuid")]
        [Required]
        [Display(Name = "Автор")]
        public virtual Employee Author { get; set; }
        [Display(Name = "Тэги")]
        public virtual HashSet<CharacterTag> Tags { get; set; } = new();

        public override string Display => throw new NotImplementedException();

        public Characterization(): base() { }
	}
	
	[Display(Name="Тег характера")]
	public class CharacterTag: Base {
		[Key]
		[Display(Name="Тэг")]
        [MaxLength(20)]
		public string Word { get; set; }
		[Display(Name="Описание")]
        [MaxLength(250)]
		public string? Description { get; set; }
        public virtual HashSet<Characterization> Characterizations { get; set; } = new();

        public override string Display { get => Word; }

        public CharacterTag(): base() { }
	}
}
