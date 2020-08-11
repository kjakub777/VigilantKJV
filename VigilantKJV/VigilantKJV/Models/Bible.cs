using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigilantKJV.Models
{    
 public enum Testament { Old,New,Both}
    public class Bible
    {
        public Bible()
        {
            //Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        // [One]
        //[ForeignKey("FK_Bible_TestamentId_Old")]
        //public Guid OldTestamentId { get; set; }
        //public Testament OldTestament  { get; set; }

        //[ForeignKey("FK_Bible_TestamentId_New")]
        //public Guid NewTestamentId { get; set; }
        //public Testament NewTestament  { get; set; }
      
        public string Label { get; set; }
        public string Version { get; set; }
 
        public string Information { get; set; }
    }
}
