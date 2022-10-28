using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;

namespace ZooIS.Models
{
    [Display(Name = "База")]
    public abstract class Base
    {
        /// <summary>
        /// A shortcut to field that describes entity.
        /// </summary>
        [NotMapped]
        [Display(Name = "Отображение")]
        public abstract string Display { get; }

        public Base() { }

        public string Enumerate<T>(ICollection<T> Collection) where T : Base
        {
            return String.Join(", ", Collection.Select(e => e.Display));
        }
    }

    [Display(Name = "Сущность")]
    public abstract class Entity : Base
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
    }
}
