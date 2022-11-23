using System;
using System.ComponentModel.DataAnnotations;

namespace ZooIS.Models {

	public class GeneologyParams {
		[Display(Name="Животное")]
		public Ref<Animal> Animal { get; set;}
	}

	public class SicknessParams {
		[Display(Name="Начало")]
		public DateTime Start { get; set; }
		[Display(Name="Начало")]
		public DateTime End { get; set; }
	}
}