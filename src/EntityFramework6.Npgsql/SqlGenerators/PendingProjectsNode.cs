#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
