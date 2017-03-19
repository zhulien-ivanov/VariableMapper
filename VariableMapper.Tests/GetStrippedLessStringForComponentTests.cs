using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetStrippedLessStringForComponentTests
    {
        [TestMethod]
        public void GetStrippedLessStringForComponentWorksCorrectly()
        {
            var dummyProperty = "width: 1;";

            var vm = new VariableMapper(dummyProperty);

            var filePath = @"..\..\Tests\Inputs\testFile.less";
            var resultFilePath = @"..\..\Tests\Inputs\testFile-result.less";

            var expectedResult = File.ReadAllText(resultFilePath).Trim();

            var dummyResult = vm.GetStrippedLessStringForComponent(filePath);
            var result = dummyResult.Replace(dummyProperty, "").Trim();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
