using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VariableMapper.Common;

namespace VariableMapper.Tests
{
    [TestClass]
    public class ConstructVariableMappingsTableForComponentTests
    {
        [TestMethod]
        public void ConstructVariableMappingsTableForComponentWorksCorrectly1()
        {
            var dummyProperty = "width: 1;";
            var variablePlaceholder = "{var}";

            var vm = new VariableMapper(dummyProperty);

            var filePath = @"..\..\Tests\Outputs\testFile1.css";

            var expectedMappingTable = new Dictionary<string, List<PropertyUsage>>();
            expectedMappingTable["@var1"] = new List<PropertyUsage>();
            expectedMappingTable["@var1"].Add(new PropertyUsage(".select1", $"color: {variablePlaceholder}"));
            expectedMappingTable["@var2"] = new List<PropertyUsage>();
            expectedMappingTable["@var2"].Add(new PropertyUsage(".select1", $"background-color: {variablePlaceholder}"));
            expectedMappingTable["@var4"] = new List<PropertyUsage>();
            expectedMappingTable["@var4"].Add(new PropertyUsage(".select1", $"border-top: 1px solid {variablePlaceholder}"));

            expectedMappingTable["@var1"].Add(new PropertyUsage("#select2", $"color: {variablePlaceholder}"));
            expectedMappingTable["@var2"].Add(new PropertyUsage("#select2", $"border: 1px solid {variablePlaceholder}"));
            expectedMappingTable["@var3"] = new List<PropertyUsage>();
            expectedMappingTable["@var3"].Add(new PropertyUsage("#select2", $"background-color: {variablePlaceholder}"));
            expectedMappingTable["@var4"].Add(new PropertyUsage("#select2", $"border-top: 1px solid {variablePlaceholder} !important"));

            expectedMappingTable["@var1"].Add(new PropertyUsage("select3 > a", $"border: 1px solid {variablePlaceholder}"));
            expectedMappingTable["@var3"].Add(new PropertyUsage("select3 > a", $"background-color: {variablePlaceholder}"));
            expectedMappingTable["@var3"].Add(new PropertyUsage("select3 > a", $"color: {variablePlaceholder}"));

            var mappingTable = vm.ConstructVariableMappingsTableForComponent(filePath, variablePlaceholder);

            CollectionAssert.AreEqual(expectedMappingTable.Keys.ToList(), mappingTable.Keys.ToList());

            foreach (var pair in expectedMappingTable)
            {
                CollectionAssert.AreEqual(expectedMappingTable[pair.Key], mappingTable[pair.Key]);
            }
        }

        [TestMethod]
        public void ConstructVariableMappingsTableForComponentWorksCorrectly2()
        {
            var dummyProperty = "width: 1;";
            var variablePlaceholder = "{var}";

            var vm = new VariableMapper(dummyProperty);

            var filePath = @"..\..\Tests\Outputs\testFile2.css";

            var expectedMappingTable = new Dictionary<string, List<PropertyUsage>>();
            expectedMappingTable["@search-box-button-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-box-button-color"].Add(new PropertyUsage(".search-box__button", $"color: {variablePlaceholder} !important"));

            expectedMappingTable["@search-box-button-hover-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-box-button-hover-color"].Add(new PropertyUsage(".search-box__button.hover", $"color: {variablePlaceholder} !important"));

            expectedMappingTable["@search-container-background-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-container-background-color"].Add(new PropertyUsage(".search-container", $"background-color: {variablePlaceholder}"));

            expectedMappingTable["@search-background-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-background-color"].Add(new PropertyUsage(".search", $"background-color: {variablePlaceholder}"));

            expectedMappingTable["@search-border-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-border-color"].Add(new PropertyUsage(".search", $"border: 1px solid {variablePlaceholder}"));
            expectedMappingTable["@search-border-color"].Add(new PropertyUsage(".search::before", $"border-top: 1px solid {variablePlaceholder}"));
            expectedMappingTable["@search-border-color"].Add(new PropertyUsage(".search::before", $"border-left: 1px solid {variablePlaceholder}"));
            expectedMappingTable["@search-background-color"].Add(new PropertyUsage(".search::before", $"background-color: {variablePlaceholder}"));

            expectedMappingTable["@search-mb-event-details-buttons-button-background-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-mb-event-details-buttons-button-background-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button", $"background-color: {variablePlaceholder}"));

            expectedMappingTable["@search-mb-event-details-buttons-button-border-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-mb-event-details-buttons-button-border-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button", $"border: 1px solid {variablePlaceholder}"));

            expectedMappingTable["@search-mb-event-details-buttons-button-hover-background-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-mb-event-details-buttons-button-hover-background-color"].Add(new PropertyUsage(".search .mb-event-details-buttons__button:hover", $"background-color: {variablePlaceholder}"));

            expectedMappingTable["@search-group-header-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-group-header-color"].Add(new PropertyUsage(".search-group-header", $"color: {variablePlaceholder}"));

            expectedMappingTable["@search-group-header-bottom-border-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-group-header-bottom-border-color"].Add(new PropertyUsage(".search-group-header", $"border-bottom: 1px solid {variablePlaceholder}"));

            expectedMappingTable["@search-header-close-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-header-close-color"].Add(new PropertyUsage(".search-header__close", $"color: {variablePlaceholder}"));

            expectedMappingTable["@search-header-close-hover-color"] = new List<PropertyUsage>();
            expectedMappingTable["@search-header-close-hover-color"].Add(new PropertyUsage(".search-header__close:hover", $"color: {variablePlaceholder}"));

            expectedMappingTable["@mb-option-button-search-results-color"] = new List<PropertyUsage>();
            expectedMappingTable["@mb-option-button-search-results-color"].Add(new PropertyUsage(".mb-option-button-search-results", $"color: {variablePlaceholder}"));

            expectedMappingTable["@mb-option-button-search-results-hover-color"] = new List<PropertyUsage>();
            expectedMappingTable["@mb-option-button-search-results-hover-color"].Add(new PropertyUsage(".mb-option-button-search-results:hover", $"color: {variablePlaceholder} !important"));

            var mappingTable = vm.ConstructVariableMappingsTableForComponent(filePath, variablePlaceholder);

            CollectionAssert.AreEqual(expectedMappingTable.Keys.ToList(), mappingTable.Keys.ToList());

            foreach (var pair in expectedMappingTable)
            {
                CollectionAssert.AreEqual(expectedMappingTable[pair.Key], mappingTable[pair.Key]);
            }
        }
    }
}
