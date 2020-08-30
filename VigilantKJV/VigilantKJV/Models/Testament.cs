using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VigilantKJV.Models
{
    public enum TestamentName { Old, New, Both }
    public class Testament
    {
        public Testament()
        {
            //Id = Guid.NewGuid();   
        }
        [Key]
        public int Id { get; set; } 
        public ICollection<Book> Books { get; set; }
       // [EnumDataType(typeof(TestamentName))]
        public TestamentName TestamentName { get; set; }

        public int? BookCount => Books?.Count;

        public string Information { get; internal set; }
    }
}