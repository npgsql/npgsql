#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using System.Collections.Generic;
using System.Linq;
using Npgsql.NameTranslation;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [TestFixture]
    public class SnakeCaseNameTranslatorTests
    {
        [Test, TestCaseSource(typeof(SnakeCaseNameTranslatorTests), nameof(TestCases))]
        public string TranslateTypeName(string value, bool legacyMode)
            => new NpgsqlSnakeCaseNameTranslator(legacyMode).TranslateTypeName(value);

        [Test, TestCaseSource(typeof(SnakeCaseNameTranslatorTests), nameof(TestCases))]
        public string TranslateMemberName(string value, bool legacyMode)
            => new NpgsqlSnakeCaseNameTranslator(legacyMode).TranslateMemberName(value);

        static IEnumerable<TestCaseData> TestCases => new (string value, string legacyResult, string result)[]
        {
            ("Hi!! This is text. Time to test.", "hi!! _this is text. _time to test.", "hi!!_this_is_text._time_to_test."),
            ("9999-12-31T23:59:59.9999999Z", "9999-12-31_t23:59:59.9999999_z", "9999-12-31_t23:59:59.9999999_z"),
            ("already_snake_case_ ", "already_snake_case_ ", "already_snake_case_"),
            ("SHOUTING_CASE", "s_h_o_u_t_i_n_g__c_a_s_e", "shouting_case"),
            ("IsJSONProperty", "is_j_s_o_n_property", "is_json_property"),
            ("SnA__ kEcAsE", "sn_a__ k_ec_as_e", "sn_a__k_ec_as_e"),
            ("SnA__kEcAsE", "sn_a__k_ec_as_e", "sn_a__k_ec_as_e"),
            ("SnAkEcAsE", "sn_ak_ec_as_e", "sn_ak_ec_as_e"),
            ("URLValue", "u_r_l_value", "url_value"),
            ("Xml2Json", "xml2_json", "xml2_json"),
            (" IPhone ", " _i_phone ", "i_phone"),
            ("I  Phone", "i  _phone", "i_phone"),
            (" IPhone", " _i_phone", "i_phone"),
            ("I Phone", "i _phone", "i_phone"),
            ("IPhone", "i_phone", "i_phone"),
            ("iPhone", "i_phone", "i_phone"),
            ("IsCIA", "is_c_i_a", "is_cia"),
            ("Person", "person", "person"),
            ("VmQ", "vm_q", "vm_q"),
            ("URL", "u_r_l", "url"),
            ("ID", "i_d", "id"),
            ("I", "i", "i"),
            ("", "", ""),
        }.SelectMany(x => new[]
        {
            new TestCaseData(x.value, true).Returns(x.legacyResult),
            new TestCaseData(x.value, false).Returns(x.result), 
        });
    }
}
