using PINDomain.Interfaces;
using PINDomain.Security;
using PINDomain.Shared;
using System;
using Xunit;

namespace PINTest
{
    public class PINTestGenerator
    {
        readonly ISpreadSheet spreadSheet;
        readonly IPINGenerator pinGenerator;

        public PINTestGenerator()
        {
            spreadSheet = new SpreadSheet();
            pinGenerator = new PINGenerator(spreadSheet);
        }

        [Theory]
        [InlineData(1, 30, 52)]
        [InlineData(5, 15, 85)]
        [InlineData(7, 50, 131)]
        [InlineData(8, 5, 69)]
        public void TestGetSpreedSheetNumber(int sequence, int second, int valueExpected)
        {
            var valueResult = spreadSheet.GetValueSpreadSheet(sequence, second);
            Assert.Equal(valueResult, valueExpected);
        }

        [Fact]
        public void TestGetPIN()
        {
            var pinValue = pinGenerator.GetPIN();
            Assert.NotNull(pinValue);
        }

        [Theory]
        [InlineData("")] // PIN is empty
        [InlineData("jTPxffqsdpffdf6vffnkuB2KwFC9Y4bZsQwmYJmlFgA=")] // Incorrect Cryptography
        [InlineData("jTPCOpSqzgvpsa6vxlnkuB2KwFC9Y4bZsQwmYJmlFgA=")] // PIN Expired
        [InlineData("sFympE8ZoOGgKr9fdv9jXoE407xTX7esDeFOD2t9sCM=")] // PIN Table number invalid
        [InlineData("SNzidQ9g1M7ROWbviSIEkfMOBRd/VQ91sC125Xz0Bbg=")] // PIN Digit Verificator invalid
        public void TestPINIsInvalid(string pin)
        {
            var pinResult = pinGenerator.PINIsValid(pin);
            Assert.False(pinResult);
        }

        [Fact]
        public void TestPINIsValid()
        {
            var pinValue = pinGenerator.GetPIN();
            var pinResult = pinGenerator.PINIsValid(pinValue);
            Assert.True(pinResult);
        }
    }
}
