using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Npgsql.NameTranslation;
using NUnit.Framework;

namespace Npgsql.Tests;

public class SnakeCaseNameTranslatorTests
{
    static readonly CultureInfo trTRCulture = new("tr-TR");
    static readonly CultureInfo enUSCulture = new("en-US");

    [Test, TestCaseSource(typeof(SnakeCaseNameTranslatorTests), nameof(TestCases))]
    public string TranslateTypeName(CultureInfo? culture, string value, bool legacyMode)
        => new NpgsqlSnakeCaseNameTranslator(legacyMode, culture).TranslateTypeName(value);

    [Test, TestCaseSource(typeof(SnakeCaseNameTranslatorTests), nameof(TestCases))]
    public string TranslateMemberName(CultureInfo? culture, string value, bool legacyMode)
        => new NpgsqlSnakeCaseNameTranslator(legacyMode, culture).TranslateMemberName(value);

    static IEnumerable<TestCaseData> TestCases => new (CultureInfo? culture, string value, string legacyResult, string result)[]
    {
        (null, "Hi!! This is text. Time to test.", "hi!! _this is text. _time to test.", "hi_this_is_text_time_to_test"),
        (null, "9999-12-31T23:59:59.9999999Z", "9999-12-31_t23:59:59.9999999_z", "9999_12_31t23_59_59_9999999z"),
        (null, "FK_post_simple_blog_BlogId", "f_k_post_simple_blog__blog_id", "fk_post_simple_blog_blog_id"),
        (null, "already_snake_case_ ", "already_snake_case_ ", "already_snake_case_"),
        (null, "SHOUTING_CASE", "s_h_o_u_t_i_n_g__c_a_s_e", "shouting_case"),
        (null, "IsJSONProperty", "is_j_s_o_n_property", "is_json_property"),
        (null, "SnA__ kEcAsE", "sn_a__ k_ec_as_e", "sn_a__k_ec_as_e"),
        (null, "SnA__kEcAsE", "sn_a__k_ec_as_e", "sn_a__k_ec_as_e"),
        (null, "SnAkEcAsE", "sn_ak_ec_as_e", "sn_ak_ec_as_e"),
        (null, "URLValue", "u_r_l_value", "url_value"),
        (null, "Xml2Json", "xml2_json", "xml2json"),
        (null, " IPhone ", " _i_phone ", "i_phone"),
        (null, "I  Phone", "i  _phone", "i_phone"),
        (null, " IPhone", " _i_phone", "i_phone"),
        (null, "I Phone", "i _phone", "i_phone"),
        (null, "IPhone", "i_phone", "i_phone"),
        (null, "iPhone", "i_phone", "i_phone"),
        (null, "IsCIA", "is_c_i_a", "is_cia"),
        (null, "Person", "person", "person"),
        (null, "ABC123", "a_b_c123", "abc123"),
        (null, "VmQ", "vm_q", "vm_q"),
        (null, "URL", "u_r_l", "url"),
        (null, "AB1", "a_b1", "ab1"),
        (null, "ID", "i_d", "id"),
        (null, "I", "i", "i"),
        (null, "", "", ""),
        (trTRCulture, "IPhone", "ı_phone", "ı_phone"), // dotless I -> dotless ı
        (enUSCulture, "IPhone", "i_phone", "i_phone"),
        (CultureInfo.InvariantCulture, "IPhone", "i_phone", "i_phone"),
    }.SelectMany(x => new[]
    {
        new TestCaseData(x.culture, x.value, true).Returns(x.legacyResult),
        new TestCaseData(x.culture, x.value, false).Returns(x.result),
    });

    [Test, Description("Checks translating a name with letter 'I' in Turkish locale with default setting (Invariant Culture)")]
    [SetCulture("tr-TR")]
    public void TurkeyTest()
    {
        var translator = new NpgsqlSnakeCaseNameTranslator();
        var legacyTranslator = new NpgsqlSnakeCaseNameTranslator(true);

        const string clrName = "IPhone";
        const string expected = "i_phone";

        Assert.AreEqual(expected, translator.TranslateMemberName(clrName));
        Assert.AreEqual(expected, translator.TranslateTypeName(clrName));
        Assert.AreEqual(expected, legacyTranslator.TranslateMemberName(clrName));
        Assert.AreEqual(expected, legacyTranslator.TranslateTypeName(clrName));
    }
}
