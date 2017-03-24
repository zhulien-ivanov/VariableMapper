using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var filePath = @"..\..\Tests\Inputs\Stripped\testFile.less";
            var resultFilePath = @"..\..\Tests\Inputs\Stripped\testFile-result.less";

            var expectedResult = File.ReadAllText(resultFilePath).Trim();

            var variables = new HashSet<string>();
            variables.Add("@search-container-background-color");
            variables.Add("@search-container-z-index");
            variables.Add("@search-box-input-1-z-index");
            variables.Add("@search-box-input-2-z-index");
            variables.Add("@search-box-input-icon-z-index");
            variables.Add("@search-mb-event-details-buttons-button-background-color");
            variables.Add("@search-mb-event-details-buttons-button-border-color");
            variables.Add("@search-mb-event-details-buttons-button-hover-background-color");
            variables.Add("@search-box-button-color");
            variables.Add("@search-box-button-hover-color");
            variables.Add("@search-z-index");
            variables.Add("@search-background-color");
            variables.Add("@search-border-color");
            variables.Add("@search-header-close-color");
            variables.Add("@search-header-close-hover-color");
            variables.Add("@search-group-header-color");
            variables.Add("@search-group-header-bottom-border-color");
            variables.Add("@search-group-header-text-sporticon-top");
            variables.Add("@search-footer-link-color");
            variables.Add("@search-footer-link-hover-color");
            variables.Add("@mb-option-button-search-results-color");
            variables.Add("@mb-option-button-search-results-hover-color");

            var dummyResult = vm.GetStrippedLessStringForComponent(filePath, variables);
            var result = dummyResult.Replace(dummyProperty, "").Trim();

            Assert.AreEqual(expectedResult, result);
        }
    }
}
