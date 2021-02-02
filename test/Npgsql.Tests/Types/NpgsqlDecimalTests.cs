using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL NpgsqlDecimal type
    /// </summary>
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    public class NpgsqlDecimalTests : MultiplexingTestBase
    {
        #region Arithmetic Operations

        [Test, Description("Tests NpgsqlDecimal Ceiling operation")]
        public void NpgsqlCeiling()
        {
            //Without scale
            var num = new NpgsqlDecimal(1000000000001m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "1000000000001");

            //With scale (rounding tests)
            num = new NpgsqlDecimal(1000000000001.000000001m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "1000000000001");

            num = new NpgsqlDecimal(0.000000001m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "0");

            num = new NpgsqlDecimal(0.1m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "0");

            num = new NpgsqlDecimal(0.5m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "1");

            num = new NpgsqlDecimal(0.4m);
            Assert.That(NpgsqlDecimal.Ceiling(num).ToString() == "0");
        }

        [Test, Description("Tests NpgsqlDecimal Floor operation")]
        public void NpgsqlFloor()
        {
            //Without scale
            var num = new NpgsqlDecimal(1000000000001m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "1000000000001");

            //With scale (rounding tests)
            num = new NpgsqlDecimal(1000000000001.000000001m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "1000000000001");

            num = new NpgsqlDecimal(0.000000001m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "0");

            num = new NpgsqlDecimal(0.1m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "0");

            num = new NpgsqlDecimal(0.5m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "0");

            num = new NpgsqlDecimal(0.4m);
            Assert.That(NpgsqlDecimal.Floor(num).ToString() == "0");
        }

        [Test, Description("Tests NpgsqlDecimal Absolute value operation")]
        public void NpgsqlAbs()
        {
            //Without scale
            var num = new NpgsqlDecimal(-1000000000001m);
            Assert.That(NpgsqlDecimal.Abs(num).ToString() == "1000000000001");

            num = new NpgsqlDecimal(1000000000001m);
            Assert.That(NpgsqlDecimal.Abs(num).ToString() == "1000000000001");

            //With scale (rounding tests)
            num = new NpgsqlDecimal(1000000000001.000000001m);
            Assert.That(NpgsqlDecimal.Abs(num).ToString() == "1000000000001.000000001");
            num = new NpgsqlDecimal(-1000000000001.000000001m);
            Assert.That(NpgsqlDecimal.Abs(num).ToString() == "1000000000001.000000001");
        }

        [Test, Description("Tests NpgsqlDecimal Multi precision integer sum operation")]
        public void NpgsqlMpAdd()
        {

            var num = new NpgsqlDecimal(1000000m);
            num.MpAdd(20);

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(new uint[] { 1000020 }));
        }

        [Test, Description("Tests NpgsqlDecimal Multi precision integer divide operation")]
        public void NpgsqlMpDivide()
        {
            var num = new NpgsqlDecimal(10000000m);
            var remainder = num.MpDivide(10000);

            Assert.That(remainder == 0);
            Assert.That(num.ToDecimal() == 1000);
        }


        [Test, Description("Tests NpgsqlDecimal Multi precision integer multiplication")]
        public void NpgsqlMpMultiply()
        {

            var num = new NpgsqlDecimal(10000000000000000000000000m);
            num.MpMultiply(20);

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(new uint[] { 3355443200, 3113228709, 10842021 }));
        }

        [Test, Description("Tests NpgsqlDecimal > operator")]
        public void NpgsqlGreaterThanOperator()
        {
            //Without decimal part
            var operand1 = new NpgsqlDecimal(20000000000000000000000000m);
            var operand2 = new NpgsqlDecimal(19999999999999999999999999m);
            Assert.That(operand1 > operand2, "Greater than operator comparison failed for numbers without decimal part");

            //Just decimal part
            operand1 = new NpgsqlDecimal(0.9999999999999999999999m);
            operand2 = new NpgsqlDecimal(0.9999999999999999999998m);
            Assert.That(operand1 > operand2, "Greater than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999998m);
            Assert.That(operand1 > operand2, "Greater than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part, equal numbers
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999999m);
            Assert.IsFalse(operand1 > operand2, "Greater than operator comparison failed for numbers that are equal");
        }

        [Test, Description("Tests NpgsqlDecimal >= operator")]
        public void NpgsqlGreaterOrEqualThanOperator()
        {
            //Without decimal part
            var operand1 = new NpgsqlDecimal(20000000000000000000000000m);
            var operand2 = new NpgsqlDecimal(19999999999999999999999999m);
            Assert.That(operand1 >= operand2, "Greater or equal than operator comparison failed for numbers without decimal part");

            //Just decimal part
            operand1 = new NpgsqlDecimal(0.9999999999999999999999m);
            operand2 = new NpgsqlDecimal(0.9999999999999999999998m);
            Assert.That(operand1 >= operand2, "Greater or equal than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999998m);
            Assert.That(operand1 >= operand2, "Greater or equal than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part, equal numbers
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999999m);
            Assert.IsTrue(operand1 >= operand2, "Greater or equal than operator comparison failed for numbers that are equal");
        }

        [Test, Description("Tests NpgsqlDecimal < operator")]
        public void NpgsqlLessThanOperator()
        {
            //Without decimal part
            var operand1 = new NpgsqlDecimal(20000000000000000000000000m);
            var operand2 = new NpgsqlDecimal(19999999999999999999999999m);
            Assert.That(operand2 < operand1, "Less than operator comparison failed for numbers without decimal part");

            //Just decimal part
            operand1 = new NpgsqlDecimal(0.9999999999999999999999m);
            operand2 = new NpgsqlDecimal(0.9999999999999999999998m);
            Assert.That(operand2 < operand1, "Less than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999998m);
            Assert.That(operand2 < operand1, "Less than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part, equal numbers
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999999m);
            Assert.IsFalse(operand2 < operand1, "Less than operator comparison failed for numbers that are equal");
        }

        [Test, Description("Tests NpgsqlDecimal <= operator")]
        public void NpgsqlLessOrEqualThanOperator()
        {
            //Without decimal part
            var operand1 = new NpgsqlDecimal(20000000000000000000000000m);
            var operand2 = new NpgsqlDecimal(19999999999999999999999999m);
            Assert.That(operand2 <= operand1, "Less or equal than operator comparison failed for numbers without decimal part");

            //Just decimal part
            operand1 = new NpgsqlDecimal(0.9999999999999999999999m);
            operand2 = new NpgsqlDecimal(0.9999999999999999999998m);
            Assert.That(operand2 <= operand1, "Less or equal than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999998m);
            Assert.That(operand2 <= operand1, "Less or equal than operator comparison failed for numbers with decimal part");

            //With significant digits and decimal part, equal numbers
            operand1 = new NpgsqlDecimal(9999999999999.99999999999999m);
            operand2 = new NpgsqlDecimal(9999999999999.99999999999999m);
            Assert.IsTrue(operand2 <= operand1, "Less or equal than operator comparison failed for numbers that are equal");
        }


        [Test, Description("Tests NpgsqlDecimal + operator")]
        public void NpgsqlSumOperatorHighPrecision()
        {
            var operand1 = NpgsqlDecimal.Parse("530924859032485093248509283459083249058230495890285092384590834095832094582340985203948590234850923845239048520934850932485092384509832405823405890485902348502384509238459028340958234905823094850923485093248590823405982340958.001");
            var operand2 = NpgsqlDecimal.Parse("600000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000010000000000.001");
            var result = operand1 + operand2;
            var bits = new uint[] {
                                    1648879410
                                   ,3326410772
                                   ,3841557637
                                   ,2791007439
                                   ,1521403836
                                   ,2021754616
                                   ,3645313116
                                   ,2511060613
                                   ,3477521390
                                   ,3810089742
                                   ,3797290586
                                   ,3215693652
                                   ,3109225219
                                   ,284600426
                                   ,3563169865
                                   ,963530200
                                   ,2802493748
                                   ,3232850576
                                   ,1749935321
                                   ,418262482
                                   ,3234429662
                                   ,1948742115
                                   ,3061963145
                                   ,3128649
            };

            Assert.That(NpgsqlDecimal.GetBits(result), Is.EqualTo(bits));
            Assert.That(result.Scale == 3);
            Assert.That(result.Positive);
        }



        [Test, Description("Tests NpgsqlDecimal * operator")]
        public void NpgsqlMultiplyOperator()
        {
            var operand1 = new NpgsqlDecimal(300000000000001.000486734785m);
            var operand2 = new NpgsqlDecimal(2000.129384789237489237489237m);
            var result = operand1 * operand2;
            //result should be 600038815436773247.874164106415061017127186137711009045

            var bits = new uint[] {
                2271370517, 2936620743, 2582976012, 1689395818, 936602412, 410563
            };

            Assert.That(result.Scale == 36);
            Assert.That(result.Positive);
            Assert.That(NpgsqlDecimal.GetBits(result), Is.EqualTo(bits));
        }


        [Test, Description("Tests NpgsqlDecimal /  operator")]
        public void NpgsqlDivideOperator()
        {
            var operand1 = new NpgsqlDecimal(1000000000000000000000000000m);
            var operand2 = new NpgsqlDecimal(2m);
            var result = operand1 / operand2;
            Assert.That(NpgsqlDecimal.GetBits(result), Is.EqualTo(new uint[] { 0, 2623581573, 3810674377, 6310 }));
            Assert.That(result.Positive);
            Assert.That(result.Scale == 6);

            operand1 = new NpgsqlDecimal(1000000000000000000000000000m);
            operand2 = new NpgsqlDecimal(200000000000000000000000000m);
            result = operand1 / operand2;
            Assert.That(NpgsqlDecimal.GetBits(result), Is.EqualTo(new uint[] { 1342177280, 918096869, 2710505431 }));
            Assert.That(result.Scale == 28);
        }

        [Test, Description("Tests NpgsqlDecimal Substract operation")]
        public void NpgsqlDecimalSubstractOperator()
        {
            var result = new NpgsqlDecimal(99999999999999999999999.99999m)
                         -
                         new NpgsqlDecimal(11111111111111111111111.11111m);

            Assert.That(NpgsqlDecimal.GetBits(result), Is.EqualTo(new uint[] { 3101920824, 926766962, 481867632 }));
            Assert.That(result.Scale == 5);
            Assert.That(result.Positive);
        }

        [Test, Description("Tests NpgsqlDecimal Overflow exceptions in arithmentic operations")]
        public void NpgsqlDecimalOverflowInArithmeticOperations()
        {
            var maxPrecisionNumber = new string('9', NpgsqlDecimal.NUMERIC_MAX_PRECISION);
            var maxScaleNumber = new string('9', NpgsqlDecimal.NUMERIC_MAX_SCALE);
            var num = maxPrecisionNumber + "." + maxScaleNumber;

            var operand1 = NpgsqlDecimal.Parse(num);
            var operand2 = new NpgsqlDecimal(1);
            Assert.That(() => operand1 + operand2,
                   Throws.Exception
                       .With.TypeOf<OverflowException>()
                       .With.Message.EqualTo("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type."));

            operand2 = new NpgsqlDecimal(10);
            Assert.That(() => operand1 * operand2,
                   Throws.Exception
                       .With.TypeOf<OverflowException>()
                       .With.Message.EqualTo("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type."));

            operand2 = new NpgsqlDecimal(-10);
            Assert.That(() => operand1 - operand2,
                   Throws.Exception
                       .With.TypeOf<OverflowException>()
                       .With.Message.EqualTo("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type."));

            operand2 = new NpgsqlDecimal(0.1);
            Assert.That(() => operand1 / operand2,
                   Throws.Exception
                       .With.TypeOf<OverflowException>()
                       .With.Message.EqualTo("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type."));
        }



        #endregion

        #region ValueType contract tests

        [Test, Description("Tests NpgsqlDecimal Equals function")]
        public void NpgsqlDecimalEqualsTest()
        {
            //With scale 0
            var num1 = new NpgsqlDecimal(1000200000001m);
            var num2 = new NpgsqlDecimal(1000200000001m);
            Assert.That(num1.Equals(num2));

            //same scale
            num1 = new NpgsqlDecimal(100000001.000001m);
            num2 = new NpgsqlDecimal(100000001.000001m);
            Assert.That(num1.Equals(num2));

            //different scale
            num1 = new NpgsqlDecimal(100000001.0000010000000m);
            num2 = new NpgsqlDecimal(100000001.00000100m);
            Assert.That(num1.Equals(num2));

            //high precision (decimal max precision)
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = new NpgsqlDecimal(10000000000000000000000001.010m);
            Assert.That(num1.Equals(num2));

            //very high precision more than decimal max precision
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = NpgsqlDecimal.Parse("10000000000000000000000001.010000000000000000000000000000");
            Assert.That(num1.Equals(num2));

            //negative tests

            //Without scale
            num1 = new NpgsqlDecimal(10000000000000012345m);
            num2 = new NpgsqlDecimal(10000000600000012345m);
            Assert.That(!num1.Equals(num2));

            //same scale
            num1 = new NpgsqlDecimal(100000001.000001m);
            num2 = new NpgsqlDecimal(100000001.000002m);
            Assert.That(!num1.Equals(num2));

            //different scale
            num1 = new NpgsqlDecimal(100000001.0000010000001m);
            num2 = new NpgsqlDecimal(100000001.00000100m);
            Assert.That(!num1.Equals(num2));

            //high precision (decimal max precision)
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = new NpgsqlDecimal(10000000000000000000000001.011m);
            Assert.That(!num1.Equals(num2));

            //very high precision more than decimal max precision
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = NpgsqlDecimal.Parse("10000000000000000000000001.010000000000000000000000000001");
            Assert.That(!num1.Equals(num2));
        }

        [Test, Description("Tests NpgsqlDecimal Equals function")]
        public void NpgsqlDecimalEqualsOperator()
        {
            //With scale 0
            var num1 = new NpgsqlDecimal(1000200000001m);
            var num2 = new NpgsqlDecimal(1000200000001m);
            Assert.That(num1 == num2);

            //same scale
            num1 = new NpgsqlDecimal(100000001.000001m);
            num2 = new NpgsqlDecimal(100000001.000001m);
            Assert.That(num1 == num2);

            //different scale
            num1 = new NpgsqlDecimal(100000001.0000010000000m);
            num2 = new NpgsqlDecimal(100000001.00000100m);
            Assert.That(num1 == num2);

            //Negative tests            

            //Without scale
            num1 = new NpgsqlDecimal(10000000000000012345m);
            num2 = new NpgsqlDecimal(10000000600000012345m);
            Assert.That(!(num1 == num2));

            //same scale
            num1 = new NpgsqlDecimal(100000001.000001m);
            num2 = new NpgsqlDecimal(100000001.000002m);
            Assert.That(!(num1 == num2));

            //different scale
            num1 = new NpgsqlDecimal(100000001.0000010000001m);
            num2 = new NpgsqlDecimal(100000001.00000100m);
            Assert.That(!(num1 == num2));

            //high precision (decimal max precision)
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = new NpgsqlDecimal(10000000000000000000000001.011m);
            Assert.That(!(num1 == num2));

            //very high precision more than decimal max precision
            num1 = new NpgsqlDecimal(10000000000000000000000001.01m);
            num2 = NpgsqlDecimal.Parse("10000000000000000000000001.010000000000000000000000000001");
            Assert.That(!(num1 == num2));
        }


        [Test, Description("Tests NpgsqlDecimal ToString method")]
        public void NpgsqlDecimalToString()
        {
            var originalNumb = "300000000000000.0004867347856347856384756";
            var npgsqlDecimal = NpgsqlDecimal.Parse(originalNumb);
            var str = npgsqlDecimal.ToString();
            Assert.That(str == originalNumb, "ToString failed for Positive number with 25 Scale and 40 Precision");

            originalNumb = "-300000000000000.0004867347856347856384756";
            npgsqlDecimal = NpgsqlDecimal.Parse(originalNumb);
            str = npgsqlDecimal.ToString();
            Assert.That(str == originalNumb, "ToString failed for Negative number with 25 Scale and 40 Precision");

            originalNumb = "300000000000000";
            npgsqlDecimal = NpgsqlDecimal.Parse(originalNumb);
            str = npgsqlDecimal.ToString();
            Assert.That(str == originalNumb, "ToString failed for Positive number with no Scale and 15 Precision");

            originalNumb = "-300000000000000";
            npgsqlDecimal = NpgsqlDecimal.Parse(originalNumb);
            str = npgsqlDecimal.ToString();
            Assert.That(str == originalNumb, "ToString failed for negative number with no Scale and 15 Precision");

        }
        #endregion

        #region Constructors

        [Test, Description("Tests NpgsqlDecimal decimal constructor")]
        public void NpgsqlDecimalDecimalConstructor()
        {
            var num = new NpgsqlDecimal(decimal.MaxValue);
            var numBits = NpgsqlDecimal.GetBits(num);

            var bits = new uint[] { 4294967295, 4294967295, 4294967295 };

            Assert.That(numBits.Length == 3);
            Assert.That(numBits, Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Positive);
        }

        [Test, Description("Tests NpgsqlDecimal double constructor")]
        public void NpgsqlDecimalDoubleConstructor()
        {
            //Max value
            var num = new NpgsqlDecimal(double.MaxValue);
            var numBits = NpgsqlDecimal.GetBits(num);
            var bits = new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 107126352, 4087786100, 866356121, 2275956310, 3174582669, 3640009423, 1013472480, 256755600, 656218509, 4246097956, 2450687296, 1359996789, 3916874882, 2924277660, 3106322741, 3546470216, 3649033932, 4189631022, 1434106335, 3039171059, 1797681499, 4294965164, 4294967295 };

            Assert.That(numBits.Length == 32);
            Assert.That(numBits, Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Positive);

            //Min value
            num = new NpgsqlDecimal(double.MinValue);
            numBits = NpgsqlDecimal.GetBits(num);

            Assert.That(numBits.Length == 32);
            Assert.That(numBits, Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Negative);
        }

        [Test, Description("Tests NpgsqlDecimal long constructor")]
        public void NpgsqlDecimalLongConstructor()
        {
            //Max Value
            var num = new NpgsqlDecimal(long.MaxValue);
            var bits = new uint[] { 4294967295, 2147483647 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Positive);

            //Min value
            num = new NpgsqlDecimal(long.MinValue);
            bits = new uint[] { 0, 2147483648 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Negative);
        }

        [Test, Description("Tests NpgsqlDecimal long constructor")]
        public void NpgsqlDecimalIntConstructor()
        {
            //Max Value
            var num = new NpgsqlDecimal(int.MaxValue);
            var bits = new uint[] { 2147483647 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Positive);

            //Min value
            num = new NpgsqlDecimal(int.MinValue);
            bits = new uint[] { 2147483648 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Negative);
        }

        [Test, Description("Tests NpgsqlDecimal copy constructor")]
        public void NpgsqlDecimalCopyConstructor()
        {
            //positive value
            var num1 = new NpgsqlDecimal(123456789123456789.123456789123456789);
            var num1Bits = NpgsqlDecimal.GetBits(num1);
            var num2 = new NpgsqlDecimal(num1);
            var num2Bits = NpgsqlDecimal.GetBits(num2);

            Assert.That(num1Bits, Is.EqualTo(num2Bits));
            Assert.That(num1.Scale == num2.Scale);
            Assert.That(num1.Positive == num2.Positive);

            //negative value
            num1 = new NpgsqlDecimal(-123456789123456789.123456789123456789);
            num1Bits = NpgsqlDecimal.GetBits(num1);
            num2 = new NpgsqlDecimal(num1);
            num2Bits = NpgsqlDecimal.GetBits(num2);

            Assert.That(num1Bits, Is.EqualTo(num2Bits));
            Assert.That(num1.Scale == num2.Scale);
            Assert.That(num1.Positive == num2.Positive);
        }

        [Test, Description("Tests NpgsqlDecimal constructor from internal representation")]
        public void NpgsqlDecimalInternalRepresentationConstructor()
        {
            //positive value
            var num1 = new NpgsqlDecimal(new uint[] { 1874920424, 2328306 }, 6, true);
            var realNumber = 10000000000.001000m;

            Assert.That(num1.ToDecimal() == realNumber);

            //negative value
            num1 = new NpgsqlDecimal(new uint[] { 1874920424, 2328306 }, 6, false);
            realNumber = -10000000000.001000m;

            Assert.That(num1.ToDecimal() == realNumber);
        }

        [Test, Description("Tests NpgsqlDecimal string parsing")]
        public void NpgsqlDecimalParseString()
        {
            var num = NpgsqlDecimal.Parse("200000000000000000000000000");

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(new uint[] { 3355443200, 3113228709, 10842021 }));
        }


        #endregion

        #region Conversions tests

        [Test, Description("Tests NpgsqlDecimal long implicit conversion")]
        public void NpgsqlLongImplicitConversion()
        {

            var num = (NpgsqlDecimal)((long)10000000000);

            var bits = new uint[] { 1410065408, 2 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Scale == 0);
            Assert.That(num.Positive);
        }

        [Test, Description("Tests NpgsqlDecimal long implicit conversion")]
        public void NpgsqlDoubleImplicitConversion()
        {

            var num = (NpgsqlDecimal)(3000000001.000003400001d);

            var bits = new uint[] { 1339791009, 6984919 };

            Assert.That(NpgsqlDecimal.GetBits(num), Is.EqualTo(bits));
            Assert.That(num.Positive);
        }

        #endregion

        #region Other methods
        [Test, Description("Tests NpgsqlDecimal IsZero predicate function")]
        public void NpgsqlDecimalIsZero()
        {
            //double
            var num = new NpgsqlDecimal(0.0d);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            num = new NpgsqlDecimal(-0.0d);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            //decimal
            num = new NpgsqlDecimal(0.0m);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            num = new NpgsqlDecimal(-0.0m);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            //long
            num = new NpgsqlDecimal((long)0);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            num = new NpgsqlDecimal((long)-0);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            //int
            num = new NpgsqlDecimal((int)0);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);

            num = new NpgsqlDecimal((int)-0);
            Assert.That(num.IsZero());
            Assert.That(num.Positive);
        }
        #endregion

        public NpgsqlDecimalTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }
    }
}
