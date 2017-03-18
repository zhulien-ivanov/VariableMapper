using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;

namespace VariableMapper
{
    public class Program
    {
        public static void Main()
        {
            const string dummyProperty = "width: 1;";

            string lessInputs = @"..\..\Inputs\";
            string lessInputsMapped = @"..\..\Inputs\Mapped\";
            string lessOutput = @"..\..\Outputs\";
            string mappingsOutput = @"..\..\Outputs\VariableMappings\";

            string selectorPattern = @"^[a-zA-Z0-9-_#>., :&*]+(?: {|,)$";
            string closingSelectorPattern = @"\s*}$";
            string singleLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+ {$";
            string multiLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+,$";
            
            string cssSingleLineSelector = @"^([a-zA-Z0-9-_#>., :*]+) {$";
            string cssMultiLineSelector = @"^([a-zA-Z0-9-_#>., :*]+,)$";            

            string propertyPattern = @"[a-zA-Z0-9-_]+: .*(@[a-zA-Z0-9-_]+).*;";
            string cssPropertySelector = @"^\/\*(.*: .*(@[a-zA-Z0-9-_]+).*);\*\/$";

            string variablePattern = @"(@[a-zA-Z0-9-_]+): (.*);";

            string colorVariablePattern = @".*(?:(@color_[a-zA-Z0-9-_]+)|(#[0-9a-fA-F]{3,})|(rgb[a]?[(].*[)])).*";

            var selectorRegex = new Regex(selectorPattern);
            var closingSelectorRegex = new Regex(closingSelectorPattern);
            var singleLineSelectorRegex = new Regex(singleLineSelector);
            var multiLineSelectorRegex = new Regex(multiLineSelector);

            var cssSingleLineSelectorRegex = new Regex(cssSingleLineSelector);
            var cssMultiLineSelectorRegex = new Regex(cssMultiLineSelector);            
            
            var propertyRegex = new Regex(propertyPattern);
            var cssPropertyRegex = new Regex(cssPropertySelector);

            var variableRegex = new Regex(variablePattern);
            var colorVariableRegex = new Regex(colorVariablePattern);

            var directoryFiles = Directory.GetFiles(lessInputs);

            string fileName;

            string line;

            bool endOfFileReached = false;

            int bracketCounter = 0;

            var sb = new StringBuilder();

            #region MappedVariables helper
            // CREATE COLLECTION WITH FLAT VARIABLES STARTS
            var mappedVariables = new Dictionary<string, Dictionary<string, string>>();

            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(filePath))
                {
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        var match = variableRegex.Match(line);

                        if (match.Success)
                        {
                            if (!mappedVariables.ContainsKey(fileName))
                            {
                                mappedVariables[fileName] = new Dictionary<string, string>();
                            }

                            mappedVariables[fileName].Add(match.Groups[1].Value, match.Groups[2].Value);
                        }

                        line = sr.ReadLine();
                    }
                }
            }

            string currentVariable;

            // START FLATTENING
            var flattenedMappedVariables = new Dictionary<string, Dictionary<string, string>>();

            foreach (var variableMappingPair in mappedVariables)
            {
                foreach (var variableMapping in variableMappingPair.Value)
                {
                    currentVariable = variableMapping.Value;

                    while (variableMappingPair.Value.ContainsKey(currentVariable))
                    {
                        currentVariable = variableMappingPair.Value[currentVariable];
                    }

                    if (!flattenedMappedVariables.ContainsKey(variableMappingPair.Key))
                    {
                        flattenedMappedVariables[variableMappingPair.Key] = new Dictionary<string, string>();
                    }

                    flattenedMappedVariables[variableMappingPair.Key][variableMapping.Key] = currentVariable;            
                }
            }
            // END FLATTENING
            // CREATE COLLECTION WITH FLAT VARIABLES ENDS
            #endregion

            #region ColorVariables helper
            // MAP COLOR VARIABLES FOR EACH FILE STARTS
            var colorVariables = new Dictionary<string, HashSet<string>>();

            string variableName;
            
            foreach (var filePath in directoryFiles)
            {
                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(filePath))
                {
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        var match = variableRegex.Match(line);

                        if (match.Success)
                        {
                            variableName = match.Groups[1].Value;

                            if (colorVariableRegex.IsMatch(flattenedMappedVariables[fileName][variableName]))
                            {
                                if (!colorVariables.ContainsKey(fileName))
                                {
                                    colorVariables[fileName] = new HashSet<string>();
                                }

                                colorVariables[fileName].Add(match.Groups[1].Value);
                            }              
                        }

                        line = sr.ReadLine();
                    }
                }
            }
            // MAP COLOR VARIABLES FOR EACH FILE ENDS
            #endregion

            #region Less Mapping
            foreach (var filePath in directoryFiles)
            {
                endOfFileReached = false;

                fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(filePath))
                {
                    using (var sw = new StreamWriter(lessInputsMapped + fileName))
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
                                    sb.Append(line);

                                    sw.WriteLine(sb.ToString());
                                    sb.Clear();

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

                                        if (selectorRegex.IsMatch(line))
                                        {
                                            while (multiLineSelectorRegex.IsMatch(line))
                                            {
                                                sb.Append(line + " ");

                                                line = sr.ReadLine();
                                            }

                                            if (singleLineSelectorRegex.IsMatch(line))
                                            {
                                                sb.Append(line);

                                                sw.WriteLine(sb.ToString());
                                                sb.Clear();

                                                bracketCounter++;
                                            }
                                        }
                                        else if (closingSelectorRegex.IsMatch(line))
                                        {
                                            bracketCounter--;

                                            sw.WriteLine(line);
                                        }
                                        else if (propertyMatch.Success)
                                        {
                                            if (colorVariables[fileName].Contains(propertyMatch.Groups[1].Value))
                                            {
                                                sw.WriteLine(string.Format("/*{0}*/{1}", line.Trim(), dummyProperty));
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
                }
            }
            #endregion

            #region Parsing of Less to Css
            // PARSING OF LESS TO CSS STARTS
            directoryFiles = Directory.GetFiles(lessInputsMapped);

            string fileNameWithExtension;
            string parsedCss;

            foreach (var filePath in directoryFiles)
            {
                fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.Length - 5);

                parsedCss = Less.Parse(File.ReadAllText(filePath));
                File.WriteAllText(lessOutput + fileName + ".css", parsedCss);
            }
            // PARSING OF LESS TO CSS ENDS
            #endregion
        }
    }
}
