using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZooIS.Data;

namespace ZooIS.Models
{
    public enum TaxonRank
    {
        [Display(Name = "Вид")]
        Species,
        [Display(Name = "Род")]
        Genus,
        [Display(Name = "Семейство")]
        Family,
        [Display(Name = "Порядок")]
        Order,
        [Display(Name = "Класс")]
        Class,
        [Display(Name = "Тип")]
        Phylum,
        [Display(Name = "Царство")]
        Kingdom,
        [Display(Name = "Домен")]
        Domain,
        [Display(Name ="Жизнь")]
        Life
    }

    [Display(Name = "Таксон")]
    public class Taxon : Entity
    {
        [MaxLength(100)]
        [Required]
        [Display(Name = "Научное название")]
        public string ScientificName { get; set; }
        /// <summary>
        /// Also known as folk-name.
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "Народное название")]
        public string? VernacularName { get; set; }
        
        public object RankId { get; set; }
        [Display(Name = "Ранг")]
        public virtual Concept<TaxonRank> Rank { get; set; } = new(TaxonRank.Species);

        public Guid? ParentGuid { get; set; }

        [Display(Name = "Представители вида")]
        public virtual HashSet<Animal> Animals { get; set; } = new();
        [Display(Name = "Включающий такон")]
        [ForeignKey("ParentGuid")]
        public virtual Taxon? Parent { get; set; }
        [Display(Name = "Включенные таксоны")]
        public virtual HashSet<Taxon> Taxons { get; set; } = new();

        public override string Display { get => VernacularName ?? ScientificName; }

        public Taxon(string ScientificName): base() => this.ScientificName = ScientificName;

        public async Task<IEnumerable<Taxon>> GetSpecies(ZooISContext context)
        {
            TaxonRank CurrentLevel = Rank;
            HashSet<Taxon> Species = new() { this };
			//What is this shit?!
            while (CurrentLevel > TaxonRank.Species)
            {
                await context.Taxons.AsQueryable()
                    .Where(e => Species.Select(e => e.Guid)
                                       .ToHashSet()
                                    .Contains((Guid)e.ParentGuid))
                    .LoadAsync();
                Species = Species.SelectMany(e => e.Taxons).ToHashSet();
                CurrentLevel--;
            }
            return Species;
        }

        public async Task<Taxon> GetParentAt(ZooISContext context, TaxonRank Rank)
        {
            Taxon? tmp = this;
            while (tmp is not null && tmp.Rank < Rank)
            {
                tmp = await context.Taxons.AsQueryable()
                    .FirstOrDefaultAsync(e => e.Guid == tmp.ParentGuid);
            }
            return tmp;
        }
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
        [Required(ErrorMessage = "Обязательное поле.")]
        public string Name { get; set; }
        [Display(Name = "Возраст")]
        public DateTime? Age { get => BirthDate is not null ? new DateTime((DateTime.Now.AddYears(-1) - BirthDate)?.Ticks ?? default(long)) : null; }
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }
        public Guid SpeciesGuid { get; set; }
        [Required(ErrorMessage ="Обязательное поле.")]
        [Display(Name = "Вид")]
        public virtual Taxon Species { get; set; }
        [Required(ErrorMessage = "Обязательное поле.")]

        public object SexId { get; set; }
        [Display(Name = "Пол")]
        public virtual Concept<Sex> Sex { get; set; }
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

        [MaxLength(255)]
        [Display(Name ="Путь до фото")]
        public string? PicturePath { get; set; }

        public override string Display { get => this.Name; }
    }
	
	public enum Health {
		[Display(Name="Здоров(а)")]
		Healthy,
		[Display(Name="Болен(а)")]
		Sick,
		[Display(Name="Мертв(а)")]
		Dead }
	
	[Owned]
    [Display(Name = "Состояние")]
    public class Status {
        public object HealthId { get; set; }
        [Display(Name = "Здоровье")]
        public virtual Concept<Health> Health { get; set; }
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
	public class CharacterTag: IEntity
    {
		[Key]
		[Display(Name="Тэг")]
        [MaxLength(20)]
		public string Word { get; set; }
		[Display(Name="Описание")]
        [MaxLength(250)]
		public string? Description { get; set; }
        public virtual HashSet<Characterization> Characterizations { get; set; } = new();

        public object Key { get => Word; }

        public string Display { get => Word; }

        public CharacterTag(): base() { }
	}
}
