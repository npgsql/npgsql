using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.SqlGenerators
{
    /// <summary>
    /// Represents an InputExpression and what alias it will have when used in a FROM clause
    /// </summary>
    internal class NameAndInputExpression
    {
        public string AsName { get; set; }
        public InputExpression Exp { get; set; }

        public NameAndInputExpression(string asName, InputExpression exp)
        {
            AsName = asName;
            Exp = exp;
        }
    }

    /// <summary>
    /// A tree of subqueries, used when evaluating SQL text for DbPropertyExpressions in SqlSelectGenerator.
    /// See SqlSelectGenerator.Visit(DbPropertyExpression) for more information.
    /// </summary>
    internal class PendingProjectsNode
    {
        public readonly List<NameAndInputExpression> Selects = new List<NameAndInputExpression>();
        public PendingProjectsNode JoinParent { get; set; }
        public string TopName
        {
            get
            {
                return Selects[0].AsName;
            }
        }

        public PendingProjectsNode(string asName, InputExpression exp)
        {
            Selects.Add(new NameAndInputExpression(asName, exp));
        }
        public void Add(string asName, InputExpression exp)
        {
            Selects.Add(new NameAndInputExpression(asName, exp));
        }

        public NameAndInputExpression Last { get { return Selects[Selects.Count - 1]; } }
    }
}
