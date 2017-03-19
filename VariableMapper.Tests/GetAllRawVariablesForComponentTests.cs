using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetAllRawVariablesForComponentTests
    {
        [TestMethod]
        public void GetAllRawVariablesForComponentWorksCorrectly()
        {
            var vm = new VariableMapper();
            var filePath = @"..\..\Tests\Inputs\testFile.less";

            var expectedMappings = new Dictionary<string, string>();
            expectedMappings.Add("@variable1", "@color_k1");
            expectedMappings.Add("@variable-2", "@color_w");
            expectedMappings.Add("@variableNumber3", "@color_g1");
            expectedMappings.Add("@var1", "20px");
            expectedMappings.Add("@var2", "bold");
            expectedMappings.Add("@var3", "3em");
            expectedMappings.Add("@variable4", "#ccc");
            expectedMappings.Add("@variable5", "rgba(10, 10, 10, 0.4)");
            expectedMappings.Add("@variable6", "blue");
            expectedMappings.Add("@variable7", "1px solid red");
            expectedMappings.Add("@variable8", "#FAFAFA");

            var resultMappings = vm.GetAllRawVariablesForComponent(filePath);

            CollectionAssert.AreEqual(expectedMappings, resultMappings);
        }
    }
}
