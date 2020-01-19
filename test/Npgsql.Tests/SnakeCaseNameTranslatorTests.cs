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
            ("Hi!! This is text. Time to test.", "hi!! _this is text. _time to test.", "hi_this_is_text_time_to_test"),
            ("9999-12-31T23:59:59.9999999Z", "9999-12-31_t23:59:59.9999999_z", "9999_12_31t23_59_59_9999999z"),
            ("FK_post_simple_blog_BlogId", "f_k_post_simple_blog__blog_id", "fk_post_simple_blog_blog_id"),
            ("already_snake_case_ ", "already_snake_case_ ", "already_snake_case_"),
            ("SHOUTING_CASE", "s_h_o_u_t_i_n_g__c_a_s_e", "shouting_case"),
            ("IsJSONProperty", "is_j_s_o_n_property", "is_json_property"),
            ("SnA__ kEcAsE", "sn_a__ k_ec_as_e", "sn_a__k_ec_as_e"),
            ("SnA__kEcAsE", "sn_a__k_ec_as_e", "sn_a__k_ec_as_e"),
            ("SnAkEcAsE", "sn_ak_ec_as_e", "sn_ak_ec_as_e"),
            ("URLValue", "u_r_l_value", "url_value"),
            ("Xml2Json", "xml2_json", "xml2json"),
            (" IPhone ", " _i_phone ", "i_phone"),
            ("I  Phone", "i  _phone", "i_phone"),
            (" IPhone", " _i_phone", "i_phone"),
            ("I Phone", "i _phone", "i_phone"),
            ("IPhone", "i_phone", "i_phone"),
            ("iPhone", "i_phone", "i_phone"),
            ("IsCIA", "is_c_i_a", "is_cia"),
            ("Person", "person", "person"),
            ("ABC123", "a_b_c123", "abc123"),
            ("VmQ", "vm_q", "vm_q"),
            ("URL", "u_r_l", "url"),
            ("AB1", "a_b1", "ab1"),
            ("ID", "i_d", "id"),
            ("I", "i", "i"),
            ("", "", "")
        }.SelectMany(x => new[]
        {
            new TestCaseData(x.value, true).Returns(x.legacyResult),
            new TestCaseData(x.value, false).Returns(x.result), 
        });
    }
}
