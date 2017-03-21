using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VariableMapper.Common;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetVariableMappingsForComponentTests
    {
        [TestMethod]
        public void GetVariableMappingsForComponentWorksCorrectly1()
        {
            var variablePlaceholder = "{var}";

            var vm = new VariableMapper();

            var fileName = "testFile1";
            var resultFilePath = @"..\..\Tests\Outputs\VariableMappings\" + fileName + ".txt";

            var mappingTable = new Dictionary<string, List<PropertyUsage>>();
            mappingTable["@var1"] = new List<PropertyUsage>();
            mappingTable["@var1"].Add(new PropertyUsage(".select1", $"color: {variablePlaceholder}"));
            mappingTable["@var2"] = new List<PropertyUsage>();
            mappingTable["@var2"].Add(new PropertyUsage(".select1", $"background-color: {variablePlaceholder}"));
            mappingTable["@var4"] = new List<PropertyUsage>();
            mappingTable["@var4"].Add(new PropertyUsage(".select1", $"border-top: 1px solid {variablePlaceholder}"));

            mappingTable["@var1"].Add(new PropertyUsage("#select2", $"color: {variablePlaceholder}"));
            mappingTable["@var2"].Add(new PropertyUsage("#select2", $"border: 1px solid {variablePlaceholder}"));
            mappingTable["@var3"] = new List<PropertyUsage>();
            mappingTable["@var3"].Add(new PropertyUsage("#select2", $"background-color: {variablePlaceholder}"));
            mappingTable["@var4"].Add(new PropertyUsage("#select2", $"border-top: 1px solid {variablePlaceholder} !important"));

            mappingTable["@var1"].Add(new PropertyUsage("select3 > a", $"border: 1px solid {variablePlaceholder}"));
            mappingTable["@var3"].Add(new PropertyUsage("select3 > a", $"background-color: {variablePlaceholder}"));
            mappingTable["@var3"].Add(new PropertyUsage("select3 > a", $"color: {variablePlaceholder}"));            

            var expectedResult = File.ReadAllText(resultFilePath);
            var result = vm.GetVariableMappingsForComponent(mappingTable, fileName);

            Assert.AreEqual(4, result.VariablesCounts);
            Assert.AreEqual(expectedResult, result.VariableMappings);
        }

        [TestMethod]
        public void GetVariableMappingsForComponentWorksCorrectly2()
        {
            var variablePlaceholder = "{var}";

            var vm = new VariableMapper();

            var fileName = "testFile2";
            var resultFilePath = @"..\..\Tests\Outputs\VariableMappings\" + fileName + ".txt";

            var mappingTable = new Dictionary<string, List<PropertyUsage>>();
            mappingTable["@search-box-button-color"] = new List<PropertyUsage>();
            mappingTable["@search-box-button-color"].Add(new PropertyUsage(".search-box__button", $"color: {variablePlaceholder} !important"));

            mappingTable["@search-box-button-hover-color"] = new List<PropertyUsage>();
            mappingTable["@search-box-button-hover-color"].Add(new PropertyUsage(".search-box__button.hover", $"color: {variablePlaceholder} !important"));

            mappingTable["@search-container-background-color"] = new List<PropertyUsage>();
            mappingTable["@search-container-background-color"].Add(new PropertyUsage(".search-container", $"background-color: {variablePlaceholder}"));

            mappingTable["@search-background-color"] = new List<PropertyUsage>();
            mappingTable["@search-background-color"].Add(new PropertyUsage(".search", $"background-color: {variablePlaceholder}"));

            mappingTable["@search-border-color"] = new List<PropertyUsage>();
            mappingTable["@search-border-color"].Add(new PropertyUsage(".search", $"border: 1px solid {variablePlaceholder}"));
            mappingTable["@search-border-color"].Add(new PropertyUsage(".search::before", $"border-top: 1px solid {variablePlaceholder}"));
            mappingTable["@search-border-color"].Add(new PropertyUsage(".search::before", $"border-left: 1px solid {variablePlaceholder}"));
            mappingTable["@search-background-color"].Add(new PropertyUsage(".search::before", $"background-color: {variablePlaceholder}"));
            mappingTable["@search-background-color"].Add(new PropertyUsage(".search::before", $"color: {variablePlaceholder}"));

            mappingTable["@search-mb-event-details-buttons-button-background-color"] = new List<PropertyUsage>();
            mappingTable["@search-mb-event-details-buttons-button-background-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button", $"background-color: {variablePlaceholder}"));

            mappingTable["@search-mb-event-details-buttons-button-border-color"] = new List<PropertyUsage>();
            mappingTable["@search-mb-event-details-buttons-button-border-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button", $"border: 1px solid {variablePlaceholder}"));

            mappingTable["@search-mb-event-details-buttons-button-hover-background-color"] = new List<PropertyUsage>();
            mappingTable["@search-mb-event-details-buttons-button-hover-background-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button:hover", $"background-color: {variablePlaceholder}"));

            mappingTable["@search-group-header-color"] = new List<PropertyUsage>();
            mappingTable["@search-group-header-color"].Add(new PropertyUsage(".search-group-header", $"color: {variablePlaceholder}"));

            mappingTable["@search-group-header-bottom-border-color"] = new List<PropertyUsage>();
            mappingTable["@search-group-header-bottom-border-color"].Add(new PropertyUsage(".search-group-header", $"border-bottom: 1px solid {variablePlaceholder}"));

            mappingTable["@search-background-color"].Add(new PropertyUsage(".search-header__close", $"background-color: {variablePlaceholder}"));
            mappingTable["@search-background-color"].Add(new PropertyUsage(".search-header__close", $"border: 1px solid {variablePlaceholder}"));
            mappingTable["@search-header-close-color"] = new List<PropertyUsage>();
            mappingTable["@search-header-close-color"].Add(new PropertyUsage(".search-header__close", $"color: {variablePlaceholder}"));

            mappingTable["@search-header-close-hover-color"] = new List<PropertyUsage>();
            mappingTable["@search-header-close-hover-color"].Add(new PropertyUsage(".search-header__close:hover", $"color: {variablePlaceholder}"));

            mappingTable["@mb-option-button-search-results-color"] = new List<PropertyUsage>();
            mappingTable["@mb-option-button-search-results-color"].Add(new PropertyUsage(".mb-option-button-search-results", $"color: {variablePlaceholder}"));

            mappingTable["@mb-option-button-search-results-hover-color"] = new List<PropertyUsage>();
            mappingTable["@mb-option-button-search-results-hover-color"].Add(new PropertyUsage(".mb-option-button-search-results:hover", $"color: {variablePlaceholder} !important"));

            var expectedResult = File.ReadAllText(resultFilePath);
            var result = vm.GetVariableMappingsForComponent(mappingTable, fileName);

            Assert.AreEqual(14, result.VariablesCounts);
            Assert.AreEqual(expectedResult, result.VariableMappings);
        }
    }
}
