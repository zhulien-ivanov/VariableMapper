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

            var singleLineSelectorRegex = new Regex(singleLineSelector);
            var multiLineSelectorRegex = new Regex(multiLineSelector);

            var selectorRegex = new Regex(selectorPattern);
            var closingSelectorRegex = new Regex(closingSelectorPattern);
            var propertyRegex = new Regex(propertyPattern);

            var directoryFiles = Directory.GetFiles(lessInputs);

            string normalizedName;

            string line;

            bool endOfFileReached = false;

            int bracketCounter = 0;

            var sb = new StringBuilder();

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
