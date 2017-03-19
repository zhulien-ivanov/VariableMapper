using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using dotless.Core;

namespace VariableMapper
{
    public class Program
    {
        private static string dummyProperty = "width: 1;";

        private static string lessInputs = @"..\..\Inputs\";
        private static string lessInputsMapped = @"..\..\Inputs\Mapped\";
        private static string lessOutput = @"..\..\Outputs\";
        private static string mappingsOutput = @"..\..\Outputs\VariableMappings\";

        public static void Main()
        {
            GenerateStrippedLessFiles(lessInputs, lessInputsMapped);
            ParseStrippedLessFilesToCss(lessInputsMapped, lessOutput);
            ConvertParsedCssFilesToVariableMappings(lessOutput, mappingsOutput);
        }

        private static Dictionary<string, string> GetAllRawVariablesForComponent(string filePath)
        {
            string variablePattern = @"(@[a-zA-Z0-9-_]+): (.*);";
            var variableRegex = new Regex(variablePattern);

            var mappedVariables = new Dictionary<string, string>();

            string line;

            using (var sr = new StreamReader(filePath))
            {
                line = sr.ReadLine();

                while (line != null)
                {
                    var match = variableRegex.Match(line);

                    if (match.Success)
                    {
                        mappedVariables.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }

                    line = sr.ReadLine();
                }
            }

            return mappedVariables;
        }

        private static Dictionary<string, string> GetAllFlattenedVariablesForComponent(Dictionary<string, string> rawVariables)
        {
            string variableValue;

            var flattenedMappedVariables = new Dictionary<string, string>();

            foreach (var variableMapping in rawVariables)
            {
                variableValue = variableMapping.Value;

                while (rawVariables.ContainsKey(variableValue))
                {
                    variableValue = rawVariables[variableValue];
                }

                flattenedMappedVariables[variableMapping.Key] = variableValue;
            }

            return flattenedMappedVariables;
        }

        private static HashSet<string> GetColorVariablesForComponent(Dictionary<string, string> flattenedVariables)
        {
            string colorVariablePattern = @".*(?:(@color_[a-zA-Z0-9-_]+)|(#[0-9a-fA-F]{3,})|(rgb[a]?[(].*[)])).*";
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

        private static string GetStrippedLessStringForComponent(string filePath)
        {
            string selectorPattern = @"^[a-zA-Z0-9-_#>., :&*]+(?: {|,)$";
            string closingSelectorPattern = @"\s*}$";
            string singleLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+ {$";
            string multiLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+,$";

            string propertyPattern = @"[a-zA-Z0-9-_]+: .*(@[a-zA-Z0-9-_]+).*;";

            var selectorRegex = new Regex(selectorPattern);
            var closingSelectorRegex = new Regex(closingSelectorPattern);
            var singleLineSelectorRegex = new Regex(singleLineSelector);
            var multiLineSelectorRegex = new Regex(multiLineSelector);

            var propertyRegex = new Regex(propertyPattern);

            var colorVariablesForComponent = GetColorVariablesForComponent(GetAllFlattenedVariablesForComponent(GetAllRawVariablesForComponent(filePath)));

            var sb = new StringBuilder();

            string line;

            int bracketCounter = 0;
            bool endOfFileReached = false;

            using (var sr = new StreamReader(filePath))
            {
                line = sr.ReadLine();

                while (line != null)
                {
                    if (selectorRegex.IsMatch(line))
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

                                var propertyMatch = propertyRegex.Match(line);

                                if (propertyMatch.Success)
                                {
                                    if (colorVariablesForComponent.Contains(propertyMatch.Groups[1].Value))
                                    {
                                        sb.AppendLine(string.Format("/*{0}*/{1}", line.Trim(), dummyProperty));
                                    }
                                    else
                                    {
                                        // ADD LOGGER
                                        Console.WriteLine(String.Format("   Property <{0}> is excluded because it is either inherited or not a colour property.", propertyMatch.Groups[1].Value));
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

                    if (!endOfFileReached)
                    {
                        line = sr.ReadLine();
                    }
                }
            }

            return sb.ToString();
        }

        private static Dictionary<string, List<PropertyUsage>> ConstructVariableMappingsTableForComponent(string filePath)
        {
            string cssSingleLineSelector = @"^([a-zA-Z0-9-_#>., :*]+) {$";
            string cssMultiLineSelector = @"^([a-zA-Z0-9-_#>., :*]+,)$";
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
                            propertyTemplate = propertyValue.Replace(propertyVariableName, "{var}");

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

        private static string GetVariableMappingsForComponent(Dictionary<string, List<PropertyUsage>> mappingTable, string fileName)
        {
            var propertyStrings = new List<string>();
            var selectorStrings = new List<string>();

            var sb = new StringBuilder();

            sb.AppendLine(string.Format("\t{0}", "{"));
            sb.AppendLine(string.Format("\t\t{0}: '{1}',", "name", "{NAME}"));
            sb.AppendLine(string.Format("\t\t{0}: '{1}',", "value", fileName));
            sb.AppendLine(string.Format("\t\t{0}: [", "variables"));

            foreach (var variable in mappingTable)
            {
                sb.AppendLine(string.Format("\t\t\t{0}", "{"));
                sb.AppendLine(string.Format("\t\t\t\t{0}: '{1}',", "variableName", variable.Key));
                sb.AppendLine(string.Format("\t\t\t\t{0}: '{1}',", "variableTitle", "{TITLE}"));

                var mappedByPropertyDict = new Dictionary<string, List<string>>();
                var mappeByPropertyDictFlatten = new Dictionary<string, string>();
                var mappedByPropertyAndSelectorDict = new Dictionary<string, List<string>>();
                var mappedByPropertyAndSelectorDictFlatten = new Dictionary<string, string>();

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

                sb.AppendLine(string.Format("\t\t\t\t{0}: ['{1}'],", "variableSelector", string.Join("', '", selectorStrings)));
                sb.AppendLine(string.Format("\t\t\t\t{0}: [['{1}']],", "propertyTemplate", string.Join("'], ['", propertyStrings)));

                sb.AppendLine(string.Format("\t\t\t\t{0}: '{1}{2}.png'", "imageSource", "Images/{NAME}/", variable.Key));
                sb.AppendLine(string.Format("\t\t\t{0}", "},"));
            }

            sb.AppendLine(string.Format("\t\t{0}", "]"));
            sb.AppendLine(string.Format("\t{0}", "}"));

            return sb.ToString();
        }

        private static void GenerateStrippedLessFiles(string directoryInputPath, string directoryOuputPath)
        {
            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                Console.Write("Start of mapping file <");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(fileName);
                Console.ResetColor();
                Console.WriteLine(">.");

                var strippedLessString = GetStrippedLessStringForComponent(filePath);
                File.WriteAllText(directoryOuputPath + fileName, strippedLessString);

                Console.WriteLine();
            }
        }

        private static void ParseStrippedLessFilesToCss(string directoryInputPath, string directoryOuputPath)
        {
            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileNameWithExtension;
            string fileName;

            string parsedCss;

            foreach (var filePath in directoryFiles)
            {
                fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.Length - 5);

                parsedCss = Less.Parse(File.ReadAllText(filePath));
                File.WriteAllText(directoryOuputPath + fileName + ".css", parsedCss);
            }
        }

        private static void ConvertParsedCssFilesToVariableMappings(string directoryInputPath, string directoryOuputPath)
        {
            var directoryFiles = Directory.GetFiles(directoryInputPath);

            string fileNameWithExtension;
            string fileName;

            foreach (var filePath in directoryFiles)
            {
                fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.Length - 4);

                var mappings = GetVariableMappingsForComponent(ConstructVariableMappingsTableForComponent(filePath), fileName);

                File.WriteAllText(directoryOuputPath + fileName + ".txt", mappings);
            }
        }
    }
}
