using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VariableMapper.Tests
{
    [TestClass]
    public class ResolveVariableTests
    {
        [TestMethod]
        public void ResolveVariableWorksCorrectly()
        {
            var vm = new VariableMapper();

            var flattenedVariables = new Dictionary<string, string>();
            flattenedVariables.Add("@color_k1", "#ccc");
            flattenedVariables.Add("@color_w", "#fff");
            flattenedVariables.Add("@test5", "@borderProps @borderColor");
            flattenedVariables.Add("@borderWidth", "2px");
            flattenedVariables.Add("@borderColor", "@color_k1");
            flattenedVariables.Add("@borderType", "solid");
            flattenedVariables.Add("@borderProps", "@borderWidth @borderType");
            flattenedVariables.Add("@test1", "fade(@color_k1, 40%)");
            flattenedVariables.Add("@test2", "mix(@color_k1, @color_w, 10%)");
            flattenedVariables.Add("@test3", "@borderWidth solid @borderColor");
            flattenedVariables.Add("@test4", "@missingVariable solid @borderColor");
            flattenedVariables.Add("@genericBorder", "@genericBorderType @genericBorderColor");
            flattenedVariables.Add("@genericBorderColor", "@borderColor");
            flattenedVariables.Add("@genericBorderType", "@borderWidth @anotherMissingVariable");
            flattenedVariables.Add("@test6", "@test5");

            var expected = "2px solid #ccc";
            var result = vm.ResolveVariable("@test6", flattenedVariables);

            Assert.AreEqual(expected, result);

            expected = "2px @anotherMissingVariable";
            result = vm.ResolveVariable("@genericBorderType", flattenedVariables);

            Assert.AreEqual(expected, result);
        }
    }
}
