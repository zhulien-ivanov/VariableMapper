using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VariableMapper.Common;
using System;

namespace VariableMapper.Tests
{
    [TestClass]
    public class GetVariableMappingsForComponentTests
    {
        [TestMethod]
        public void GetVariableMappingsForComponentWorksCorrectly()
        {
            var variablePlaceholder = "{var}";

            var vm = new VariableMapper();

            var resultFilePath = @"..\..\Tests\Outputs\VariableMappings\testFile.txt";

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
            var result = vm.GetVariableMappingsForComponent(mappingTable, "zhulien");

            Assert.AreEqual(expectedResult, result.Replace("\r", ""));
        }
    }
}
