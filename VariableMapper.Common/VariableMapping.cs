namespace VariableMapper.Common
{
    public class VariableMapping
    {
        private int variablesCount;
        private string variableMappings;

        public VariableMapping(int variablesCount, string variableMappings)
        {
            this.VariablesCounts = variablesCount;
            this.VariableMappings = variableMappings;
        }

        public int VariablesCounts
        {
            get { return this.variablesCount; }
            private set { this.variablesCount = value; }
        }

        public string VariableMappings
        {
            get { return this.variableMappings; }
            private set { this.variableMappings = value; }
        }
    }
}
