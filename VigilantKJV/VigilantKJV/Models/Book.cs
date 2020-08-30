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
        #region Vars
        BookName bookName;

        int ordinal;

        public BookName BookName
        {
            get => this.bookName;
            set
            {
                this.bookName = value;
            }
        }



        public ICollection<Chapter> Chapters { get; set; }

        public string FriendlyLabel => Name;

        [Key]
        public int Id { get; set; }
        public string Information { get; set; }

        public string Name { get => BookName.GetDescription(); }

        public int Number => Ordinal;
        public int Ordinal
        {
            get => this.ordinal; set => this.ordinal = value;
        }
        public string sBookName => BookName.ToString();

        public string sTestament => Testament.ToString();
        public Testament Testament { get; set; }
        [ForeignKey("FK_Book_Testament")]
        public int TestamentId { get; set; }
        #endregion

        public Book()
        {
            // this.Chapters = new HashSet<Chapter>();
        }
        public Book(BookName name)
        {
            this.bookName = name;
            this.ordinal = (int)name;
            // this.Chapters = new HashSet<Chapter>();
        }

        #region Meths
        void SetOrdinal()
        {

        }

        public override string ToString()
        {
            return $"{Name}";
        }
        #endregion


    }

}
