using System;
using System.Collections.Generic;
using System.Text; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigilantKJV.Models
{
    public class Book
    {
        public Book()
        {
            //Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

         
        public ICollection<Chapter> Chapters { get; set; }
 
      //  [EnumDataType(typeof(Testament))]
        [Required]
        public string Testament { get; set; }

        public string Name { get; set; }
       
        public int Ordinal { get; set; }
 
        public string Information { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }


    }

}
