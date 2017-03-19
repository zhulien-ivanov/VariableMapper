namespace VariableMapper
{
    public class Program
    {
        private static string lessInputs = @"..\..\Inputs\";
        private static string lessInputsMapped = @"..\..\Inputs\Mapped\";
        private static string lessOutput = @"..\..\Outputs\";
        private static string mappingsOutput = @"..\..\Outputs\VariableMappings\";

        public static void Main()
        {
            var variableMapper = new VariableMapper();

            variableMapper.GenerateStrippedLessFiles(lessInputs, lessInputsMapped);
            variableMapper.ParseStrippedLessFilesToCss(lessInputsMapped, lessOutput);
            variableMapper.ConvertParsedCssFilesToVariableMappings(lessOutput, mappingsOutput);
        }
    }
}
