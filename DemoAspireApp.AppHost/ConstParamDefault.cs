using Aspire.Hosting.Publishing;

namespace DemoAspireApp.AppHost
{
    internal class ConstParamDefault : ParameterDefault
    {
        private readonly string _value;

        internal ConstParamDefault(string value)
        {
            _value = value;
        }

        public override string GetDefaultValue()
        {
            return _value;
        }

        public override void WriteToManifest(ManifestPublishingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
