using System;
using System.Collections.Generic;
using BatchRvtUtil;
using Xunit;


namespace BatchRvtUtil.Tests
{
    public class BatchRvtTests
    {
        
        [Fact]
        public void ConstructCommandLineArguments_ShouldReturnArguments()
        {
            IEnumerable<KeyValuePair<string, string>> arguments = new List<KeyValuePair<string, string>>(){ new(
                "test1", "test2")};
            var result = BatchRvt.ConstructCommandLineArguments(arguments);
            Assert.Equal(string.Join(" ", "--" + "test1" + " " + "test2"), result);

        }

        [Theory]
        [InlineData(null)]
        public void ConstructCommandLineArguments_ShouldFail(List<KeyValuePair<string, string>> arguments)
        {
            Assert.Throws<ArgumentException>(() => BatchRvt.ConstructCommandLineArguments(arguments));
        }

        [Theory]
        [InlineData("16:30:13")]
        [InlineData("16:30:13")]
        public void IsBatchRvtLine_ShouldReturnTrueOnCorrectLine(string line)
        {
            Assert.True(BatchRvt.IsBatchRvtLine(line));
        }
        
        [Theory]
        [InlineData("x:x:x")]
        [InlineData("")]
        
        public void IsBatchRvtLine_ShouldReturnFalseOnIncorrectLine(string line)
        {
            Assert.False(BatchRvt.IsBatchRvtLine(line));
        }
        [Theory]
        [InlineData(null)]
        public void IsBatchRvtLine_ShouldReturnExceptionOnNullArgument(string line)
        {
            Assert.Throws<ArgumentException>(() => BatchRvt.IsBatchRvtLine(line));
        }
    }
}