using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using VigilantKJV.Services;

namespace VigilantKJV.Models
{
    public class Book : IBibleObject
    {
        public Book()
        {
            this.Chapters = new HashSet<Chapter>();
        }
        public Book(BookName name)
        {
            this.bookName = name;
            this.ordinal = (int)name;
            this.Chapters = new HashSet<Chapter>();
        }

        [Key]
        public Guid Id { get; set; }


        public ICollection<Chapter> Chapters { get; set; }

        //  [EnumDataType(typeof(Testament))]
        [Required]
        public Testament Testament { get; set; }

        BookName bookName;

        public BookName BookName
        {
            get => this.bookName;
            set
            {
                this.bookName = value;
            }
        }

        public string Name { get => BookName.GetDescription(); }

        int ordinal;
        public int Ordinal
        {
            get => this.ordinal; set => this.ordinal = value;
        }
        void SetOrdinal()
        {

        }

        public string sTestament => Testament.ToString();
        public string sBookName => BookName.ToString();
        public string Information { get; set; }

        public string FriendlyLabel => Name;

        public int Number => Ordinal;

        public override string ToString()
        {
            return $"{Name}";
        }


    }

}
