using System;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuTestApplication
{
    public class OwinApplication : FubuRegistry, IApplicationSource
    {
        public OwinApplication()
        {
            IncludeDiagnostics(true);
            Actions.IncludeType<OwinActions>();
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(this).StructureMap(new Container());
        }
    }

    public class OwinActions
    {
        public string get_say_hello()
        {
            return "Hello, world!";
        }
    }
}