using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZooIS.Data;

namespace ZooIS.Models {
	//Params
	public class GeneologyParams {
		[Required]
		[Display(Name="Животное")]
		public Guid Animal { get; set;}
	}

	public class SicknessParams
	{
		[Required]
		[Display(Name="Начало")]
		public DateTime Start { get; set; }
		[Required]
		[Display(Name="Начало")]
		public DateTime End { get; set; }
	}

	//Results
    public class GeneologyResult
    {
        public Guid ChildrenGuid { get; set; }
		public virtual Animal Children { get; set; }
        public Guid ParentsGuid { get; set; }
		public virtual Animal Parent { get; set; }

		public GeneologyResult(SqlDataReader Row)
		{
			ChildrenGuid = (Guid)Row["ChildrenGuid"];
			ParentsGuid = (Guid)Row["ParentsGuid"];
		}
    }

	public class SicknessResult
	{
		public SicknessResult(SqlDataReader Row)
		{

		}
	}
}