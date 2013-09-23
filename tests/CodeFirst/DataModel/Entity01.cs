#if CODE_FIRST
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.DataModel
{
    [Table("entity01", Schema = "code_first")]
	public class Entity01 : IComparable
	{
        [Column("id"), Key]
        public Int64 Id { get; set; }

        [Column("field_timestamp")]
        public DateTime TimeStamp { get; set; }

        [Column("field_time")]
        public DateTime Time { get; set; }

        [Column("field_interval")]
        public TimeSpan Interval { get; set; }

        public int CompareTo(object obj)
        {
            var other = obj as Entity01;
            if (other == null) return 0;

            var x = Interval.CompareTo(other.Interval);
            if (x != 0) return x;
            x = TimeStamp.CompareTo(other.TimeStamp);
            if (x != 0) return x;
            x = Time.CompareTo(other.Time);
            return x;
        }
	}
}
#endif