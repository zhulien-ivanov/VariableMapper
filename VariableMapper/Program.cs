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
            string lessInputs = @"..\..\Inputs\";
            string lessInputsMapped = @"..\..\Inputs\Mapped\";
            string lessOutput = @"..\..\Outputs\";

            string singleLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+ {$";
            string multiLineSelector = @"^[a-zA-Z0-9-_#>., :&*]+,$";

            string selectorPattern = @"^[a-zA-Z0-9-_#>., :&*]+( {|,)$";
            string closingSelectorPattern = @"\s*}$";
            string propertyPattern = @"[a-zA-Z0-9-_]+: .*@.+;";

            string variablePattern = @"(@[a-zA-Z0-9-_]+): (.*);";
            string colorVariablePattern = @"(@[a-zA-Z0-9-_]+): .*(?:(@color_[a-zA-Z0-9-_]+)|(#[0-9a-fA-F]{3,})|(rgb[a]?[(].*[)])).*;";

            var singleLineSelectorRegex = new Regex(singleLineSelector);
            var multiLineSelectorRegex = new Regex(multiLineSelector);

            var selectorRegex = new Regex(selectorPattern);
            var closingSelectorRegex = new Regex(closingSelectorPattern);
            var propertyRegex = new Regex(propertyPattern);

            var variableRegex = new Regex(variablePattern);
            var colorVariableRegex = new Regex(colorVariablePattern);

            var directoryFiles = Directory.GetFiles(lessInputs);

            string normalizedName;

            string line;

            bool endOfFileReached = false;

            int bracketCounter = 0;

            var sb = new StringBuilder();

            // CREATE COLLECTION WITH FLAT VARIABLES STARTS
            var mappedVariables = new Dictionary<string, Dictionary<string, string>>();

            foreach (var fileName in directoryFiles)
            {
                normalizedName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(fileName))
                {
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        var match = variableRegex.Match(line);

                        if (match.Success)
                        {
                            if (!mappedVariables.ContainsKey(normalizedName))
                            {
                                mappedVariables[normalizedName] = new Dictionary<string, string>();
                            }

                            mappedVariables[normalizedName].Add(match.Groups[1].Value, match.Groups[2].Value);
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

            // MAP COLOR VARIABLES FOR EACH FILE STARTS
            var colorVariables = new Dictionary<string, HashSet<string>>();
            
            foreach (var fileName in directoryFiles)
            {
                normalizedName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(fileName))
                {
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        var match = colorVariableRegex.Match(line);

                        if (match.Success)
                        {
                            if (!colorVariables.ContainsKey(normalizedName))
                            {
                                colorVariables[normalizedName] = new HashSet<string>();
                            }

                            colorVariables[normalizedName].Add(match.Groups[1].Value);                 
                        }

                        line = sr.ReadLine();
                    }
                }
            }
            // MAP COLOR VARIABLES FOR EACH FILE ENDS
                  
            foreach (var fileName in directoryFiles)
            {
                endOfFileReached = false;

                normalizedName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                using (var sr = new StreamReader(fileName))
                {
                    using (var sw = new StreamWriter(lessInputsMapped + normalizedName))
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
                                        else if (propertyRegex.IsMatch(line))
                                        {
                                            sw.WriteLine(string.Format("/*{0}*/", line.Trim()));
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
        }
    }
}
