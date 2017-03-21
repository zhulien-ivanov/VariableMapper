using System;

namespace VariableMapper
{
    public class Program
    {
        private static string lessInputs = @"..\..\Inputs\";
        private static string lessInputsColorFile = @"..\..\Inputs\Colors\";
        private static string lessInputsStripped = @"..\..\Inputs\Stripped\";
        private static string lessInputsWithColorsImported = @"..\..\Inputs\ColorsImported\";
        private static string lessInputsMapped = @"..\..\Inputs\Mapped\";
        private static string lessOutput = @"..\..\Outputs\";
        private static string mappingsOutput = @"..\..\Outputs\VariableMappings\";

        public static void Main()
        {
            var variableMapper = new VariableMapper();
            
            variableMapper.StripCommentBlocksFromLessFiles(lessInputs, lessInputsStripped);
            variableMapper.GenerateLessFilesWithImportedColors(lessInputsStripped, lessInputsColorFile, lessInputsWithColorsImported);
            variableMapper.GenerateStrippedLessFiles(lessInputsWithColorsImported, lessInputsMapped);
            variableMapper.ParseStrippedLessFilesToCss(lessInputsMapped, lessOutput);
            variableMapper.ConvertParsedCssFilesToVariableMappings(lessOutput, mappingsOutput);

            Console.WriteLine("Mapping finished.");
            Console.ReadLine();
        }
    }
}
