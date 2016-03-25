using System;

namespace Npgsql
{
    /// <summary>
    /// Specifies whether and how a document's length should impact its rank.
    /// This is used with the ranking functions in <see cref="NpgsqlTextFunctions" />.
    /// 
    /// See http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// for more information about the behaviors that are controlled by this value.
    /// </summary>
    [Flags]
    public enum NpgsqlRankingNormalization
    {
        /// <summary>
        /// Ignores the document length.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Divides the rank by 1 + the logarithm of the document length.
        /// </summary>
        DivideBy1PlusLogLength = 1,

        /// <summary>
        /// Divides the rank by the document length.
        /// </summary>
        DivideByLength = 2,

        /// <summary>
        /// Divides the rank by the mean harmonic distance between extents (this is implemented only by ts_rank_cd).
        /// </summary>
        DivideByMeanHarmonicDistanceBetweenExtents = 4,

        /// <summary>
        /// Divides the rank by the number of unique words in document.
        /// </summary>
        DivideByUniqueWordCount = 8,

        /// <summary>
        /// Divides the rank by 1 + the logarithm of the number of unique words in document.
        /// </summary>
        DividesBy1PlusLogUniqueWordCount = 16,

        /// <summary>
        /// Divides the rank by itself + 1.
        /// </summary>
        DivideByItselfPlusOne = 32
    }
}