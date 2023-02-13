using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZooIS.Data;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Linq;

namespace ZooIS.Models {

    public enum ReportType
    {
        [Display(Name = "Готовые отчеты")]
        Ready,
        [Display(Name = "Построение генеалогического древа")]
        Geneology,
        [Display(Name = "Отчет о больных животных")]
        Sickness,
    }

    [Display(Name ="Отчет")]
	public class Report: Entity
	{
		public string RequesterId { get; set; }
		[Required]
		[Display(Name ="Заказчик")]
		public virtual User Requester { get; set; }
		[Required]
		[Display(Name = "Метка времени")]
		public DateTime ts { get; set; } = DateTime.Now;
		[Required]
		[Display(Name ="Тип")]
		public ReportType Type { get; set; }
        /// <summary>
        /// Used for sql mapping. Use XmlParams instead.
        /// </summary>
        [Column(TypeName = "xml")]
        public string Params { get; set; }
        [Required]
        [Display(Name = "Параметры")]
		[NotMapped]
        public XElement XmlParams { get => XElement.Parse(Params); set { Params = value.ToString(); } }
		/// <summary>
		/// Used for sql mapping. Use XmlResult instead.
		/// </summary>
		[Column(TypeName ="xml")]
		public string Result { get; set; }
        [Required]
        [Display(Name = "Результат")]
        [NotMapped]
		public XElement XmlResult { get => XElement.Parse(Result); set { Result = value.ToString(); } }

        public override string Display { get => Type.GetDisplay(); }

    }

    public static class ParamsHelpers
    {
        public static XElement ToXml<T>(this T Object)
        {
            using (MemoryStream memoryStream = new())
            {
                using (StreamWriter writer = new(memoryStream))
                {
                    XmlSerializer xmlSerializer = new(typeof(T));
                    xmlSerializer.Serialize(writer, Object);
                    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Deparses XElement into object of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static T Deparse<T>(this XElement Xml)
        {
            XmlSerializer xmlSerializer = new(typeof(T));
            return (T)xmlSerializer.Deserialize(Xml.CreateReader());
        }
    }

    public abstract class ReportParams
	{
        public XElement ToXml()
        {
            using (MemoryStream memoryStream = new())
            {
                using (StreamWriter writer = new(memoryStream))
                {
                    XmlSerializer xmlSerializer = new(this.GetType());
                    xmlSerializer.Serialize(writer, this);
                    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
            }
        }
    }

	public class GeneologyParams: ReportParams
	{
		[Required]
        [Display(Name = "Животное")]
        public Ref<Animal> Animal { get; set; }
    }

	public class SicknessParams: ReportParams
	{
		[Required(ErrorMessage ="Обязательное поле")]
		[Display(Name="Начало")]
		public DateTime Start { get; set; }
		[Required(ErrorMessage = "Обязательное поле")]
		[Display(Name="Конец")]
		public DateTime End { get; set; }
	}

    //Results
    public abstract class ReportResult
    {
        public abstract void InsertHeaders(ref ExcelWorksheet excel);
        public abstract void InsertRow(ref ExcelWorksheet excel);
    }

    public class GeneologyResult: ReportResult
    {
        [Display(Name = "Ребенок")]
        public Guid ChildrenGuid { get; set; }
		public virtual Animal Children { get; set; }
        public Guid ParentsGuid { get; set; }
        [Display(Name ="Родитель")]
		public virtual Animal Parent { get; set; }

        public GeneologyResult() { }

		public GeneologyResult(SqlDataReader Row)
		{
			ChildrenGuid = (Guid)Row["ChildrenGuid"];
			ParentsGuid = (Guid)Row["ParentsGuid"];
		}

        public override void InsertHeaders(ref ExcelWorksheet excel)
        {
            int CurrentRow = (excel.Dimension?.End.Row ?? 0) + 1;
            excel.Cells[CurrentRow, 1].Value = "Родитель";
            excel.Cells[CurrentRow, 1].Style.Font.Bold = true;
            excel.Cells[CurrentRow, 2].Value = "Ребенок";
            excel.Cells[CurrentRow, 2].Style.Font.Bold = true;
        }

        public override void InsertRow(ref ExcelWorksheet excel)
        {
            int CurrentRow = (excel.Dimension?.End.Row ?? 0) + 1;
            excel.Cells[CurrentRow, 1].Value = Parent.Display;
            excel.Cells[CurrentRow, 2].Value = Children.Display;
        }
    }

	public class SicknessResult: ReportResult
    {
        public SicknessResult() { }

		public SicknessResult(SqlDataReader Row)
		{

		}

        public override void InsertHeaders(ref ExcelWorksheet excel)
        {
            int CurrentRow = (excel.Dimension?.End.Row ?? 0) + 1;
            excel.Cells[CurrentRow, 1].Value = "";
            excel.Cells[CurrentRow, 1].Style.Font.Bold = true;
        }

        public override void InsertRow(ref ExcelWorksheet excel)
        {
            int CurrentRow = (excel.Dimension?.End.Row ?? 0) + 1;
            excel.Cells[CurrentRow, 1].Value = "";
        }
    }
}