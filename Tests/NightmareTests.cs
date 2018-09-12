using System;
using Production;
using Xunit;

namespace Tests
{
    public class NightmareTests
    {
        [Theory]
        [InlineData("30m", 30)]
        [InlineData("130m", 130)]
        [InlineData("1h", 60)]
        [InlineData("1h 30m", 90)]
        [InlineData("40", 2400)]
        public void GivenTextInput_ReturnsNumberOfMinutes(string input, int expectedMinutes)
        {
            // arrange

            // act
            var result = Nightmare.Parse(input);

            // assert
            Assert.Equal(expectedMinutes, result);
        }

        [Theory]
        [InlineData("20m 10m")]
        public void GivenUnitDuplication_ThrowsException(string input)
        {
            // arrange

            // act
            Action action = () => Nightmare.Parse(input);

            // assert
            Assert.Throws<Exception>(action);
        }
    }
}