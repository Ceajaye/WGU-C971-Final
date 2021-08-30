using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TermTracker.Classes
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TermName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }


    }
}

