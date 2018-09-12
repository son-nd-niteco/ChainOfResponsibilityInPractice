using System;
using Production;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class WorkLogParserTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WorkLogParserTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        [Theory]
        [InlineData("30m", 30)]
        [InlineData("130m", 130)]
        [InlineData("1h", 60)]
        [InlineData("1h 30m", 90)]
        public void GivenTextInput_ReturnsNumberOfMinutes(string input, int expectedMinutes)
        {
            // arrange
            IWorkLogParser parser = new NitecoParser(_testOutputHelper.WriteLine);

            // act
            var result = parser.Handle(input, null);

            // assert
            Assert.Equal(expectedMinutes, result);
        }

        [Theory]
        [InlineData("20m 10m")]
        public void GivenDuplicateUnit_ThrowsException(string input)
        {
            // arrange
            var parser = new NitecoParser(_testOutputHelper.WriteLine);

            // act
            Action action = () => parser.Handle(input, null);

            // assert
            Assert.Throws<Exception>(action);
        }
    }
}