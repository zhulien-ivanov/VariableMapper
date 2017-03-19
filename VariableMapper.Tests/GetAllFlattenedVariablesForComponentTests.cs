using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetAllFlattenedVariablesForComponentTests
    {
        [TestMethod]
        public void GetAllFlattenedVariablesForComponentWorksCorrectly()
        {
            var vm = new VariableMapper();

            var inputDictionary = new Dictionary<string, string>();
            inputDictionary.Add("@variable1", "@color_k1");
            inputDictionary.Add("@variable2", "@variable1");
            inputDictionary.Add("@variable3", "@variable4");
            inputDictionary.Add("@var1", "@var3");
            inputDictionary.Add("@variable4", "@variable5");
            inputDictionary.Add("@variable5", "@color_w");
            inputDictionary.Add("@variable6", "fade(@color_w, 40%)");
            inputDictionary.Add("@variable7", "1px solid @variable1");
            inputDictionary.Add("@variable8", "@missingVariable");
            inputDictionary.Add("@var2", "10px");
            inputDictionary.Add("@var3", "@var2");
            inputDictionary.Add("@var4", "@inheritedVariable");
            inputDictionary.Add("@v1", "@v4");
            inputDictionary.Add("@v2", "fade(@v1, 40%)");
            inputDictionary.Add("@v3", "@v2");
            inputDictionary.Add("@v4", "fade(@color_g1, 40%)");
            inputDictionary.Add("@var5", "bold");

            var expectedMappings = new Dictionary<string, string>();
            expectedMappings.Add("@variable1", "@color_k1");
            expectedMappings.Add("@variable2", "@color_k1");
            expectedMappings.Add("@variable3", "@color_w");
            expectedMappings.Add("@var1", "10px");
            expectedMappings.Add("@variable4", "@color_w");
            expectedMappings.Add("@variable5", "@color_w");
            expectedMappings.Add("@variable6", "fade(@color_w, 40%)");
            expectedMappings.Add("@variable7", "1px solid @color_k1");
            expectedMappings.Add("@variable8", "@missingVariable");
            expectedMappings.Add("@var2", "10px");
            expectedMappings.Add("@var3", "10px");
            expectedMappings.Add("@var4", "@inheritedVariable");
            expectedMappings.Add("@v1", "fade(@color_g1, 40%)");
            expectedMappings.Add("@v2", "fade(fade(@color_g1, 40%), 40%)");
            expectedMappings.Add("@v3", "fade(fade(@color_g1, 40%), 40%)");
            expectedMappings.Add("@v4", "fade(@color_g1, 40%)");
            expectedMappings.Add("@var5", "bold");

            var resultMappings = vm.GetAllFlattenedVariablesForComponent(inputDictionary);

            CollectionAssert.AreEqual(expectedMappings, resultMappings);
        }
    }
}
