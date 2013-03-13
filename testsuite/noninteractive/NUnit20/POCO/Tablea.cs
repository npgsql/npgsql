using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NpgsqlTests
{
	[Table("tablea", Schema = "public")]
    public class Tablea
    {
        [Column("field_serial"), Key]
        public long Field_serial { get; set; }

        [Column("field_text")]
        public string Field_text { get; set; }

        [Column("field_int4")]
        public int? Field_int4 { get; set; }

        [Column("field_int8")]
        public int? Field_int8 { get; set; }
        
        [Column("field_bool")]
        public bool? Field_bool { get; set; }

    }
}
