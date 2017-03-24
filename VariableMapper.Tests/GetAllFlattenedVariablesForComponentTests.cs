using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetAllFlattenedVariablesForComponentTests
    {
        [TestMethod]
        public void GetAllFlattenedVariablesForComponentWorksCorrectly1()
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

        [TestMethod]
        public void GetAllFlattenedVariablesForComponentWorksCorrectly2()
        {
            var vm = new VariableMapper();

            var inputDictionary = new Dictionary<string, string>();
            inputDictionary.Add("@color_k1", "#ccc");
            inputDictionary.Add("@color_w", "#fff");
            inputDictionary.Add("@test5", "@borderProps @borderColor");
            inputDictionary.Add("@borderWidth", "2px");
            inputDictionary.Add("@borderColor", "@color_k1");
            inputDictionary.Add("@borderType", "solid");
            inputDictionary.Add("@borderProps", "@borderWidth @borderType");
            inputDictionary.Add("@test1", "fade(@color_k1, 40%)");
            inputDictionary.Add("@test2", "mix(@color_k1, @color_w, 10%)");
            inputDictionary.Add("@test3", "@borderWidth solid @borderColor");
            inputDictionary.Add("@test4", "@missingVariable solid @borderColor");
            inputDictionary.Add("@genericBorder", "@genericBorderType @genericBorderColor");
            inputDictionary.Add("@genericBorderColor", "@borderColor");
            inputDictionary.Add("@genericBorderType", "@borderWidth @anotherMissingVariable");

            var expectedMappings = new Dictionary<string, string>();
            expectedMappings.Add("@color_k1", "#ccc");
            expectedMappings.Add("@color_w", "#fff");
            expectedMappings.Add("@test5", "2px solid #ccc");
            expectedMappings.Add("@borderWidth", "2px");
            expectedMappings.Add("@borderColor", "#ccc");
            expectedMappings.Add("@borderType", "solid");
            expectedMappings.Add("@borderProps", "2px solid");
            expectedMappings.Add("@test1", "fade(#ccc, 40%)");
            expectedMappings.Add("@test2", "mix(#ccc, #fff, 10%)");
            expectedMappings.Add("@test3", "2px solid #ccc");
            expectedMappings.Add("@test4", "@missingVariable solid #ccc");
            expectedMappings.Add("@genericBorder", "2px @anotherMissingVariable #ccc");
            expectedMappings.Add("@genericBorderColor", "#ccc");
            expectedMappings.Add("@genericBorderType", "2px @anotherMissingVariable");

            var resultMappings = vm.GetAllFlattenedVariablesForComponent(inputDictionary);

            CollectionAssert.AreEqual(expectedMappings, resultMappings);
        }
    }
}
