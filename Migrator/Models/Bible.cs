using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigilantKJV.Models
{

    public enum BookName
    {
        [Description("Genesis")]
        Genesis = 1,
        [Description("Exodus")]
        Exodus,
        [Description("Leviticus")]
        Leviticus,
        [Description("Numbers")]
        Numbers,
        [Description("Deuteronomy")]
        Deuteronomy,
        [Description("Joshua")]
        Joshua,
        [Description("Judges")]
        Judges,
        [Description("Ruth")]
        Ruth,
        [Description("1 Samuel")]
        ISamuel,
        [Description("2 Samuel")]
        IISamuel,
        [Description("1 Kings")]
        IKings,
        [Description("2 Kings")]
        IIKings,
        [Description("1 Chronicles")]
        IChronicles,
        [Description("2 Chronicles")]
        IIChronicles,
        [Description("Ezra")]
        Ezra,
        [Description("Nehemiah")]
        Nehemiah,
        [Description("Esther")]
        Esther,
        [Description("Job")]
        Job,
        [Description("Psalm")]
        Psalm,
        [Description("Proverbs")]
        Proverbs,
        [Description("Ecclesiastes")]
        Ecclesiastes,
        [Description("Song Of Solomon")]
        SongOfSolomon,
        [Description("Isaiah")]
        Isaiah,
        [Description("Jeremiah")]
        Jeremiah,
        [Description("Lamentations")]
        Lamentations,
        [Description("Ezekiel")]
        Ezekiel,
        [Description("Daniel")]
        Daniel,
        [Description("Hosea")]
        Hosea,
        [Description("Joel")]
        Joel,
        [Description("Amos")]
        Amos,
        [Description("Obadiah")]
        Obadiah,
        [Description("Jonah")]
        Jonah,
        [Description("Micah")]
        Micah,
        [Description("Nahum")]
        Nahum,
        [Description("Habakkuk")]
        Habakkuk,
        [Description("Zephaniah")]
        Zephaniah,
        [Description("Haggai")]
        Haggai,
        [Description("Zechariah")]
        Zechariah,
        [Description("Malachi")]
        Malachi,
        [Description("Matthew")]
        Matthew,
        [Description("Mark")]
        Mark,
        [Description("Luke")]
        Luke,
        [Description("John")]
        John,
        [Description("Acts")]
        Acts,
        [Description("Romans")]
        Romans,
        [Description("1 Corinthians")]
        ICorinthians,
        [Description("2 Corinthians")]
        IICorinthians,
        [Description("Galatians")]
        Galatians,
        [Description("Ephesians")]
        Ephesians,
        [Description("Philippians")]
        Philippians,
        [Description("Colossians")]
        Colossians,
        [Description("1 Thessalonians")]
        IThessalonians,
        [Description("2 Thessalonians")]
        IIThessalonians,
        [Description("1 Timothy")]
        ITimothy,
        [Description("2 Timothy")]
        IITimothy,
        [Description("Titus")]
        Titus,
        [Description("Philemon")]
        Philemon,
        [Description("Hebrews")]
        Hebrews,
        [Description("James")]
        James,
        [Description("1 Peter")]
        IPeter,
        [Description("2 Peter")]
        IIPeter,
        [Description("1 John")]
        IJohn,
        [Description("2 John")]
        IIJohn,
        [Description("3 John")]
        IIIJohn,
        [Description("Jude")]
        Jude,
        [Description("Revelation")]
        Revelation
    }
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
