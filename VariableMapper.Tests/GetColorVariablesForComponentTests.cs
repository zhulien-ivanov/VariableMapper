using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetColorVariablesForComponentTests
    {
        [TestMethod]
        public void GetColorVariablesForComponentWorksCorrectly()
        {
            var vm = new VariableMapper();

            var inputDictionary = new Dictionary<string, string>();
            inputDictionary.Add("@variable1", "@color_k1");
            inputDictionary.Add("@variable2", "@color_w");
            inputDictionary.Add("@variable3", "10px");
            inputDictionary.Add("@var1", "fade(@color_w, 40%)");
            inputDictionary.Add("@variable4", "rgb(10, 10, 10)");
            inputDictionary.Add("@variable5", "bold");
            inputDictionary.Add("@variable6", "rgba(15, 15, 15, 0.5)");
            inputDictionary.Add("@variable7", "1px solid @color_g1");
            inputDictionary.Add("@variable8", "@missingVariable");
            inputDictionary.Add("@var2", "3em");
            inputDictionary.Add("@var3", "#ccc");
            inputDictionary.Add("@var4", "1px solid #FAFAFA");
            inputDictionary.Add("@var5", "none");
            inputDictionary.Add("@v1", "fade(fade(@color_k1, 40%), 50%)");
            inputDictionary.Add("@v2", "hsl(90, 100%, 100%)");
            inputDictionary.Add("@v3", "hsv(90, 100%, 100%)");

            var expectedMappings = new HashSet<string>();
            expectedMappings.Add("@variable1");
            expectedMappings.Add("@variable2");
            expectedMappings.Add("@var1");
            expectedMappings.Add("@variable4");
            expectedMappings.Add("@variable6");
            expectedMappings.Add("@var3");
            expectedMappings.Add("@v1");
            expectedMappings.Add("@v2");
            expectedMappings.Add("@v3");

            var resultMappings = vm.GetColorVariablesForComponent(inputDictionary);

            Assert.IsTrue(expectedMappings.SetEquals(resultMappings));
        }
    }
}
