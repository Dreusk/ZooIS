using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ZooIS.Data
{
    public struct Page
    {
        public string Controller { get; set; }
        /// <summary>
        /// A localized name
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// A list of roles that have access to this page.
        /// </summary>
        [Display(Name = "Доступ")]
        public List<string> Access { get; set; }

        /// <summary>
        /// List of all pages currently present in project.
        /// </summary>
        public static readonly List<Page> AllPages = new(){
        new(){
            Controller = "Animals",
            Display = "Животные",
            Access = new List<string>{"Caretaker"}
        }};
    }
}
