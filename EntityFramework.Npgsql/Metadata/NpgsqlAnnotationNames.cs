namespace EntityFramework.Npgsql.Extensions
{
    public static class NpgsqlAnnotationNames
    {
        public const string Prefix = "PostgreSQL:";
        public const string Clustered = "Clustered";
        public const string ValueGeneration = "ValueGeneration";
		public const string ColumnComputedExpression = "ColumnComputedExpression";
		public const string DefaultSequenceName = "DefaultSequenceName";
        public const string DefaultSequenceSchema = "DefaultSequenceSchema";
        public const string SequenceName = "SequenceName";
        public const string SequenceSchema = "SequenceSchema";
    }
}