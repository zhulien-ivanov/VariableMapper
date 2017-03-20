using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class StripCommentBlocksFromLessFileTests
    {
        [TestMethod]
        public void StripCommentBlocksFromLessFileWorksCorrectly()
        {
            var vm = new VariableMapper();

            var filePath = @"..\..\Tests\Inputs\testFile.less";
            var resultFilePath = @"..\..\Tests\Inputs\testFile-result.less";

            var expectedResult = File.ReadAllText(resultFilePath);

            var result = vm.StripCommentBlocksFromLessFile(filePath);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
