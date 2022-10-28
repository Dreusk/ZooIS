using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;

namespace ZooIS.Models
{
    [Display(Name ="Пища")]
    public class Food: Entity
    {
        [Required]
        [MaxLength(60)]
        [Display(Name="Название")]
        public string Name { get; set; }
        [MaxLength(250)]
        [Display(Name="Доп. инфомация")]
        public string? AdditionalInfo { get; set; }
        public uint? Supply { get; set; }
        public uint Demand { get; set; } = 0;
        [Display(Name="Едоки")]
        public HashSet<Animal> Eaters { get; set; } = new();

        public Food(): base() { }

        public override string Display { get => Name; }
    }
}
