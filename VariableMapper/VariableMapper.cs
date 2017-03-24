using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using dotless.Core;

using VariableMapper.Common;

namespace VariableMapper
{
    public class VariableMapper
    {
        private string dummyProperty;

        public VariableMapper(string dummyProperty)
        {
            this.dummyProperty = dummyProperty;
        }
        public VariableMapper() : this("width: 1;") { }

        public void StripCommentBlocksFromLessFiles(string directoryInputPath, string directoryOuputPath)
        {
            if (!Directory.Exists(directoryOuputPath))
            {
                Directory.CreateDirectory(directoryOuputPath);
            }

            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                var strippedLessString = this.StripCommentBlocksFromLessFile(filePath);
                File.WriteAllText(directoryOuputPath + fileName, strippedLessString);
            }
        }

        public void GenerateLessFilesWithImportedColors(string directoryInputPath, string colorsInputPath, string directoryOuputPath)
        {
            if (!Directory.Exists(directoryOuputPath))
            {
                Directory.CreateDirectory(directoryOuputPath);
            }

            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                var lessWithImportedColors = this.GetLessFileWithImportedColors(filePath, colorsInputPath);
                File.WriteAllText(directoryOuputPath + fileName, lessWithImportedColors);
            }
        }

        public void GenerateStrippedLessFiles(string strippedFilesInputPath, string colorsImportedFilesInputPath, string directoryOuputPath)
        {
            if (!Directory.Exists(directoryOuputPath))
            {
                Directory.CreateDirectory(directoryOuputPath);
            }

            var directoryFiles = Directory.GetFiles(colorsImportedFilesInputPath);

            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                Console.Write("Start of mapping file <");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(fileName);
                Console.ResetColor();
                Console.WriteLine(">:");

                var originalRawVariables = new HashSet<string>(this.GetAllRawVariablesForComponent(strippedFilesInputPath + fileName).Keys);

                var strippedLessString = this.GetStrippedLessStringForComponent(filePath, originalRawVariables);
                File.WriteAllText(directoryOuputPath + fileName, strippedLessString);

                Console.WriteLine();
            }
        }

        public void ParseStrippedLessFilesToCss(string directoryInputPath, string directoryOuputPath)
        {
            if (!Directory.Exists(directoryOuputPath))
            {
                Directory.CreateDirectory(directoryOuputPath);
            }

            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileNameWithExtension;
            string fileName;

            string parsedCss;

            foreach (var filePath in directoryFiles)
            {
                fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.Length - 5);

                parsedCss = Less.Parse(File.ReadAllText(filePath));

                if (parsedCss != string.Empty)
                {
                    File.WriteAllText(directoryOuputPath + fileName + ".css", parsedCss);
                }
            }
        }

        public void ConvertParsedCssFilesToVariableMappings(string directoryInputPath, string directoryOuputPath)
        {
            if (!Directory.Exists(directoryOuputPath))
            {
                Directory.CreateDirectory(directoryOuputPath);
            }

            var directoryFiles = Directory.GetFiles(directoryInputPath);

            int totalVariablesMapped = 0;

            string fileNameWithExtension;
            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.Length - 4);

                var mappingTable = this.ConstructVariableMappingsTableForComponent(filePath);
                var mappings = this.GetVariableMappingsForComponent(mappingTable, fileName);

                File.WriteAllText(directoryOuputPath + fileName + ".txt", mappings.VariableMappings);

                Console.Write("    [");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(String.Format("{0, 2}", mappings.VariablesCounts.ToString().PadLeft(2, '0')));
                Console.ResetColor();
                Console.Write("] variables mapped for <");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(fileNameWithExtension);
                Console.ResetColor();
                Console.WriteLine(">.");

                totalVariablesMapped += mappings.VariablesCounts;
            }

            Console.WriteLine();
            Console.Write("   [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(String.Format("{0, 3}", totalVariablesMapped.ToString().PadLeft(3, '0')));
            Console.ResetColor();
            Console.Write("] variables mapped in total for [");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(directoryFiles.Length);
            Console.ResetColor();
            Console.WriteLine("] files.");
            Console.WriteLine();
        }

        public void ConcatenteParsedVariableMappings(string directoryInputPath, string outputFilePath, string fileName)
        {
            if (!Directory.Exists(outputFilePath))
            {
                Directory.CreateDirectory(outputFilePath);
            }

            var directoryFiles = Directory.GetFiles(directoryInputPath);

            var mappings = new List<string>();

            foreach (var filePath in directoryFiles)
            {
                mappings.Add(File.ReadAllText(filePath).TrimEnd());
            }

            File.WriteAllText(outputFilePath + fileName, string.Join($",{Environment.NewLine}", mappings));
        }

        internal string StripCommentBlocksFromLessFile(string filePath)
        {
            var multilineCommentPattern = @"\/\*[\s\S]*?\*\/";

            var multilineCommentRegex = new Regex(multilineCommentPattern);

            var lessContent = File.ReadAllText(filePath);
            var strippedContent = multilineCommentRegex.Replace(lessContent, string.Empty);

            return strippedContent;
        }

        private string GetLessFileWithImportedColors(string fileInputPath, string colorsDirectoryPath)
        {
            var fileContent = File.ReadAllText(fileInputPath);

            var sb = new StringBuilder();

            var directoryFiles = Directory.GetFiles(colorsDirectoryPath);

            foreach (var filePath in directoryFiles)
            {
                sb.AppendLine(File.ReadAllText(filePath));
            }

            var finalContent = sb.ToString() + fileContent;

            return finalContent;
        }

        internal Dictionary<string, string> GetAllRawVariablesForComponent(string filePath)
        {
            string variablePattern = @"^(@[a-zA-Z0-9-_]+):\s*(.*);";
            var variableRegex = new Regex(variablePattern);

            var mappedVariables = new Dictionary<string, string>();

            string line;

            using (var sr = new StreamReader(filePath))
            {
                line = sr.ReadLine();

                while (line != null)
                {
                    line = line.Trim();

                    var match = variableRegex.Match(line);

                    if (match.Success)
                    {
                        mappedVariables[match.Groups[1].Value] = match.Groups[2].Value;
                    }

                    line = sr.ReadLine();
                }
            }

            return mappedVariables;
        }

        internal Dictionary<string, string> GetAllFlattenedVariablesForComponent(Dictionary<string, string> rawVariables)
        {
            string variableValue;

            var flattenedMappedVariables = new Dictionary<string, string>();

            // Handle simple variable flattening
            foreach (var variableMapping in rawVariables)
            {
                variableValue = variableMapping.Value;

                while (rawVariables.ContainsKey(variableValue))
                {
                    variableValue = rawVariables[variableValue];
                }

                flattenedMappedVariables[variableMapping.Key] = variableValue;
            }

            // Handle nested variables flattening
            foreach (var dictKey in flattenedMappedVariables.Keys.ToList())
            {
                flattenedMappedVariables[dictKey] = this.ResolveVariable(dictKey, flattenedMappedVariables);
            }

            return flattenedMappedVariables;
        }

        internal string ResolveVariable(string variableKey, Dictionary<string, string> flattenedVariables)
        {
            if (!flattenedVariables.ContainsKey(variableKey))
            {
                return variableKey;
            }

            string variableValue = flattenedVariables[variableKey];

            string result = variableValue;
            string finalValue;

            string includesVariablePattern = @"(@[a-zA-Z0-9-_]+)";
            var includesVariableRegex = new Regex(includesVariablePattern);

            var includesVariableMatch = includesVariableRegex.Matches(variableValue);

            foreach (var match in includesVariableMatch)
            {
                var matchedString = match.ToString();

                if (flattenedVariables.ContainsKey(matchedString))
                {
                    finalValue = this.ResolveVariable(matchedString, flattenedVariables);

                    var matchedStringRegex = new Regex(matchedString);
                    result = matchedStringRegex.Replace(result, finalValue, 1);
                }
            }

            return result;
        }

        internal HashSet<string> GetColorVariablesForComponent(Dictionary<string, string> flattenedVariables)
        {            
            string colorVariablePattern = @"^(?:(?:@color_[a-zA-Z0-9-_]+)|(?:transparent)|(?:#[a-fA-F0-9]{3}\b)|(?:#[a-fA-F0-9]{6}\b)|(?:(?:(?:[a]?rgb[a]?)|(?:hsl[a]?)|(?:hsv[a]?))\(.*?\))|(?:[a-zA-Z0-0-_]+\(.*(?:(?:@color_[a-zA-Z0-9-_]+)|(?:transparent)|(?:#[a-fA-F0-9]{3}?)|(?:#[a-fA-F0-9]{6})|(?:(?:(?:[a]?rgb[a]?)|(?:hsl[a]?)|(?:hsv[a]?))\(.*?\))).*\)))$";

            var colorVariableRegex = new Regex(colorVariablePattern);

            var colorVariables = new HashSet<string>();

            foreach (var pair in flattenedVariables)
            {
                if (colorVariableRegex.IsMatch(pair.Value))
                {
                    colorVariables.Add(pair.Key);
                }
            }

            return colorVariables;
        }        

        internal string GetStrippedLessStringForComponent(string filePath, HashSet<string> originalVariables)
        {
            string containsFunctionsPattern = @"(?:(:?.*@.*)|(?:.*[(][)].*)){";

            string selectorPattern = @"^[a-zA-Z0-9-_#>., :&*+~\[\]=\(\)]+(?:\s*{|,)$";
            string closingSelectorPattern = @"\s*}$";
            string singleLineSelector = @"^[a-zA-Z0-9-_#>., :&*+~\[\]=\(\)]+\s*{$";
            string multiLineSelector = @"^[a-zA-Z0-9-_#>., :&*+~\[\]=\(\)]+,$";

            string propertyPattern = @"^[a-zA-Z0-9-_]+: ([\s\S]+);$";
            string variablePattern = @"(@[a-zA-Z0-9-_]+)";

            var containsFunctionsRegex = new Regex(containsFunctionsPattern);

            var selectorRegex = new Regex(selectorPattern);
            var closingSelectorRegex = new Regex(closingSelectorPattern);
            var singleLineSelectorRegex = new Regex(singleLineSelector);
            var multiLineSelectorRegex = new Regex(multiLineSelector);

            var propertyRegex = new Regex(propertyPattern);
            var variableRegex = new Regex(variablePattern);

            var rawVariablesForComponent = this.GetAllRawVariablesForComponent(filePath);
            var flattenedVariablesForComponent = this.GetAllFlattenedVariablesForComponent(rawVariablesForComponent);
            var colorVariablesForComponent = this.GetColorVariablesForComponent(flattenedVariablesForComponent);

            var sb = new StringBuilder();

            string line;
            string lineTrimmed;

            int bracketCounter = 0;
            bool endOfFileReached = false;

            var variableUsage = new Dictionary<string, bool>();
            var originalColorVariables = new HashSet<string>();                              

            foreach (var variable in originalVariables)
            {
                variableUsage[variable] = false;

                if (colorVariablesForComponent.Contains(variable))
                {
                    originalColorVariables.Add(variable);
                }
            }

            using (var sr = new StreamReader(filePath))
            {
                line = sr.ReadLine();

                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        // Exclude functions(mixins, media queries, etc..)
                        if (containsFunctionsRegex.IsMatch(line))
                        {
                            bracketCounter++;

                            while (bracketCounter != 0)
                            {
                                line = sr.ReadLine();

                                if (line == null)
                                {
                                    endOfFileReached = true;
                                    break;
                                }

                                if (line.Contains("{"))
                                {
                                    bracketCounter++;
                                }
                                else if (line.Contains("}"))
                                {
                                    bracketCounter--;
                                }
                            }
                        }
                        else if (selectorRegex.IsMatch(line))
                        {
                            while (multiLineSelectorRegex.IsMatch(line))
                            {
                                sb.Append(line + " ");

                                line = sr.ReadLine();
                            }

                            if (singleLineSelectorRegex.IsMatch(line))
                            {
                                sb.AppendLine(line);

                                bracketCounter++;

                                while (bracketCounter != 0)
                                {
                                    line = sr.ReadLine();

                                    if (line == null)
                                    {
                                        endOfFileReached = true;
                                        break;
                                    }

                                    lineTrimmed = line.Trim();

                                    var propertyMatch = propertyRegex.Match(lineTrimmed);

                                    if (propertyMatch.Success)
                                    {
                                        var lineCopy = lineTrimmed;
                                        var properties = propertyMatch.Groups[1].Value;

                                        var matchedVariables = variableRegex.Matches(properties);

                                        foreach (var match in matchedVariables)
                                        {
                                            var matchedString = match.ToString();

                                            if (!originalColorVariables.Contains(matchedString))
                                            {
                                                if (flattenedVariablesForComponent.ContainsKey(matchedString))
                                                {
                                                    var matchedStringRegex = new Regex(matchedString);
                                                    lineCopy = matchedStringRegex.Replace(lineCopy, flattenedVariablesForComponent[matchedString], 1);
                                                }
                                            }
                                        }

                                        matchedVariables = variableRegex.Matches(lineCopy);

                                        var uniqueVariables = new HashSet<string>();

                                        foreach (var match in matchedVariables)
                                        {
                                            var matchedString = match.ToString();

                                            if (!uniqueVariables.Contains(matchedString))
                                            {
                                                uniqueVariables.Add(matchedString);
                                            }
                                        }

                                        if (uniqueVariables.Count == 1)
                                        {
                                            var variable = uniqueVariables.First();

                                            if (originalColorVariables.Contains(variable))
                                            {
                                                sb.AppendLine(string.Format("/*{0}*/{1}", lineCopy, dummyProperty));

                                                variableUsage[variable] = true;
                                            }
                                        }
                                        else if (uniqueVariables.Count > 1)
                                        {
                                            // Complex style involving either more than one color variable or inherited variables which cant be resolved.
                                            Console.Write("    Line <");
                                            Console.ForegroundColor = ConsoleColor.Magenta;
                                            Console.Write(lineCopy);
                                            Console.ResetColor();
                                            Console.WriteLine("> skipped due to complex mapping.");
                                        }
                                    }
                                    else if (selectorRegex.IsMatch(lineTrimmed))
                                    {
                                        while (multiLineSelectorRegex.IsMatch(lineTrimmed))
                                        {
                                            sb.Append(line + " ");

                                            line = sr.ReadLine();
                                            lineTrimmed = line.Trim();
                                        }

                                        if (singleLineSelectorRegex.IsMatch(lineTrimmed))
                                        {
                                            sb.AppendLine(line);

                                            bracketCounter++;
                                        }
                                    }
                                    else if (closingSelectorRegex.IsMatch(line))
                                    {
                                        bracketCounter--;

                                        sb.AppendLine(line);
                                    }
                                }
                            }
                        }
                    }

                    if (!endOfFileReached)
                    {
                        line = sr.ReadLine();
                    }
                }
            }

            foreach (var pair in variableUsage)
            {
                if (!originalColorVariables.Contains(pair.Key) || !pair.Value)
                {
                    Console.Write("    Property <");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(pair.Key);
                    Console.ResetColor();
                    Console.WriteLine("> is excluded because it is either inherited, not used or not a simple colour property.");
                }
            }

            return sb.ToString();
        }

        internal Dictionary<string, List<PropertyUsage>> ConstructVariableMappingsTableForComponent(string filePath, string variablePlaceholder = "{var}")
        {
            string cssSingleLineSelector = @"^([a-zA-Z0-9-_#>., :*+~\[\]=\(\)]+) {$";
            string cssMultiLineSelector = @"^([a-zA-Z0-9-_#>., :*+~\[\]=\(\)]+,)$";
            string cssPropertySelector = @"^\/\*(.*: .*(@[a-zA-Z0-9-_]+).*);\*\/$";

            var cssSingleLineSelectorRegex = new Regex(cssSingleLineSelector);
            var cssMultiLineSelectorRegex = new Regex(cssMultiLineSelector);
            var cssPropertyRegex = new Regex(cssPropertySelector);

            string line;
            var sb = new StringBuilder();

            string propertyValue;
            string propertyVariableName;
            string propertyTemplate;

            var variableMappings = new Dictionary<string, List<PropertyUsage>>();

            using (var sr = new StreamReader(filePath))
            {
                line = sr.ReadLine();

                while (line != null)
                {
                    var selectorMatch = cssMultiLineSelectorRegex.Match(line);

                    while (selectorMatch.Success)
                    {
                        sb.Append(selectorMatch.Groups[1].Value + " ");

                        line = sr.ReadLine();

                        selectorMatch = cssMultiLineSelectorRegex.Match(line);
                    }

                    selectorMatch = cssSingleLineSelectorRegex.Match(line);

                    if (selectorMatch.Success)
                    {
                        sb.Append(selectorMatch.Groups[1].Value);

                        line = sr.ReadLine().Trim();

                        selectorMatch = cssPropertyRegex.Match(line);

                        while (selectorMatch.Success)
                        {
                            propertyValue = selectorMatch.Groups[1].Value;
                            propertyVariableName = selectorMatch.Groups[2].Value;
                            propertyTemplate = propertyValue.Replace(propertyVariableName, variablePlaceholder);

                            if (!variableMappings.ContainsKey(propertyVariableName))
                            {
                                variableMappings[propertyVariableName] = new List<PropertyUsage>();
                            }

                            variableMappings[propertyVariableName].Add(new PropertyUsage(sb.ToString(), propertyTemplate));

                            line = sr.ReadLine().Trim();

                            if (line == dummyProperty)
                            {
                                line = sr.ReadLine().Trim();
                            }

                            selectorMatch = cssPropertyRegex.Match(line);
                        }

                        sb.Clear();
                    }

                    line = sr.ReadLine();
                }
            }

            return variableMappings;
        }

        internal VariableMapping GetVariableMappingsForComponent(Dictionary<string, List<PropertyUsage>> mappingTable, string fileName)
        {
            var spacingCount = 1;
            var spacingSymbol = '\t';

            var sb = new StringBuilder();

            var mappedByPropertyDict = new Dictionary<string, List<string>>();
            var mappeByPropertyDictFlatten = new Dictionary<string, string>();
            var mappedByPropertyAndSelectorDict = new Dictionary<string, List<string>>();
            var mappedByPropertyAndSelectorDictFlatten = new Dictionary<string, string>();

            var propertyStrings = new List<string>();
            var selectorStrings = new List<string>();

            sb.AppendLine(string.Format("{0}{1}", new string(spacingSymbol, spacingCount), "{"));
            sb.AppendLine(string.Format("{0}{1}: '{2}',", new string(spacingSymbol, spacingCount + 1), "name", fileName));
            sb.AppendLine(string.Format("{0}{1}: '{2}',", new string(spacingSymbol, spacingCount + 1), "value", fileName));
            sb.AppendLine(string.Format("{0}{1}: [", new string(spacingSymbol, spacingCount + 1), "variables"));

            foreach (var variable in mappingTable)
            {
                sb.AppendLine(string.Format("{0}{1}", new string(spacingSymbol, spacingCount + 2), "{"));
                sb.AppendLine(string.Format("{0}{1}: '{2}',", new string(spacingSymbol, spacingCount + 3), "variableName", variable.Key));
                sb.AppendLine(string.Format("{0}{1}: '{2}',", new string(spacingSymbol, spacingCount + 3), "variableTitle", variable.Key));

                mappedByPropertyDict = new Dictionary<string, List<string>>();
                mappeByPropertyDictFlatten = new Dictionary<string, string>();
                mappedByPropertyAndSelectorDict = new Dictionary<string, List<string>>();
                mappedByPropertyAndSelectorDictFlatten = new Dictionary<string, string>();

                foreach (var templateUsage in variable.Value)
                {
                    if (!mappedByPropertyDict.ContainsKey(templateUsage.PropertyTemplate))
                    {
                        mappedByPropertyDict[templateUsage.PropertyTemplate] = new List<string>();
                    }

                    mappedByPropertyDict[templateUsage.PropertyTemplate].Add(templateUsage.Selector);
                }

                foreach (var pair in mappedByPropertyDict)
                {
                    mappeByPropertyDictFlatten[pair.Key] = string.Join(", ", pair.Value);
                }

                foreach (var pair in mappeByPropertyDictFlatten)
                {
                    if (!mappedByPropertyAndSelectorDict.ContainsKey(pair.Value))
                    {
                        mappedByPropertyAndSelectorDict[pair.Value] = new List<string>();
                    }

                    mappedByPropertyAndSelectorDict[pair.Value].Add(pair.Key);
                }

                foreach (var pair in mappedByPropertyAndSelectorDict)
                {
                    mappedByPropertyAndSelectorDictFlatten[pair.Key] = string.Join("', '", pair.Value);
                }

                foreach (var pair in mappedByPropertyAndSelectorDictFlatten)
                {
                    propertyStrings.Add(pair.Value);
                    selectorStrings.Add(pair.Key);
                }

                sb.AppendLine(string.Format("{0}{1}: ['{2}'],", new string(spacingSymbol, spacingCount + 3), "variableSelector", string.Join("', '", selectorStrings)));
                sb.AppendLine(string.Format("{0}{1}: [['{2}']],", new string(spacingSymbol, spacingCount + 3), "propertyTemplate", string.Join("'], ['", propertyStrings)));

                sb.AppendLine(string.Format("{0}{1}: '{2}{3}.png'", new string(spacingSymbol, spacingCount + 3), "imageSource", $"Images/{fileName}/", variable.Key));
                sb.AppendLine(string.Format("{0}{1}", new string(spacingSymbol, spacingCount + 2), "},"));

                mappedByPropertyDict.Clear();
                mappeByPropertyDictFlatten.Clear();
                mappedByPropertyAndSelectorDict.Clear();
                mappedByPropertyAndSelectorDictFlatten.Clear();

                propertyStrings.Clear();
                selectorStrings.Clear();
            }

            sb.AppendLine(string.Format("{0}{1}", new string(spacingSymbol, spacingCount + 1), "]"));
            sb.AppendLine(string.Format("{0}{1}", new string(spacingSymbol, spacingCount), "}"));

            var variableMapping = new VariableMapping(mappingTable.Count, sb.ToString());

            return variableMapping;
        }
    }
}
