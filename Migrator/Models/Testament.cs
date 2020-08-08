//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel.DataAnnotations;

//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Sqlite;
//using System.ComponentModel.DataAnnotations.Schema; 

//namespace VigilantKJV.Models
//{
//    public class Testament
//    {
//        public Testament()
//        { 
//        }
//        [Key]
//        public Guid Id { get; set; }
// 
//        public ICollection<Book> Books { get; set; }
//        [ForeignKey("FK_Testament_BibleId")]
//        public Guid BibleId { get;set;}
//        public Bible Bible { get;set;}
//        public string Name { get; set; }
// 
//        public string Information { get; set; }
//    }
//}
