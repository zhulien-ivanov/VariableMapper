using System;

namespace VariableMapper
{
    public class Program
    {
        private static string lessInputsDirectory = @"..\..\Inputs\";
        private static string lessInputsColorsDirectory = @"..\..\Inputs\Colors\";
        private static string strippedLessDirectory = @"..\..\Inputs\Stripped\";
        private static string colorsImportedLessDirectory = @"..\..\Inputs\ColorsImported\";
        private static string mappedLessDirectory = @"..\..\Inputs\Mapped\";
        private static string cssOutputDirectory = @"..\..\Outputs\";
        private static string variableMappingsDirectory = @"..\..\Outputs\VariableMappings\";
        private static string concatenatedVariableMappingsDirectory = @"..\..\Outputs\VariableMappings\All\";

        public static void Main()
        {
            var variableMapper = new VariableMapper();                        

            variableMapper.StripCommentBlocksFromLessFiles(lessInputsDirectory, strippedLessDirectory);
            variableMapper.GenerateLessFilesWithImportedColors(strippedLessDirectory, lessInputsColorsDirectory, colorsImportedLessDirectory);
            variableMapper.GenerateStrippedLessFiles(strippedLessDirectory, colorsImportedLessDirectory, mappedLessDirectory);
            variableMapper.ParseStrippedLessFilesToCss(mappedLessDirectory, cssOutputDirectory);
            variableMapper.ConvertParsedCssFilesToVariableMappings(cssOutputDirectory, variableMappingsDirectory);
            variableMapper.ConcatenteParsedVariableMappings(variableMappingsDirectory, concatenatedVariableMappingsDirectory, "all.txt");

            Console.WriteLine("Mapping finished.");
            Console.ReadLine();
        }
    }
}
