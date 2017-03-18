namespace VariableMapper
{
    internal class PropertyUsage
    {
        private string selector;
        private string propertyTemplate;

        public PropertyUsage(string selector, string propertyTemplate)
        {
            this.Selector = selector;
            this.PropertyTemplate = propertyTemplate;
        }

        public string Selector
        {
            get { return this.selector; }
            private set { this.selector = value; }
        }

        public string PropertyTemplate
        {
            get { return this.propertyTemplate; }
            private set { this.propertyTemplate = value; }
        }
    }
}
