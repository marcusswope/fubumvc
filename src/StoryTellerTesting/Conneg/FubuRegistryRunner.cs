using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using FubuCore;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.OwinHost;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Serenity.Endpoints;
using StructureMap;

namespace IntegrationTesting.Conneg
{


    public abstract class FubuRegistryHarness
    {
        private Harness theHarness;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theHarness = Harness.Run(configure);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theHarness.Dispose();
        }

        protected EndpointDriver endpoints
        {
            get
            {
                return theHarness.Endpoints;
            }
        }

        protected abstract void configure(FubuRegistry registry);
    }



    public class SimpleSource : IApplicationSource
    {
        private readonly FubuRegistry _registry;

        public SimpleSource(FubuRegistry registry)
        {
            _registry = registry;
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(_registry).StructureMap(new Container());
        }
    }

    public class Harness : IDisposable
    {
        private static int _port = 5500;

        public static Harness Run(Action<FubuRegistry> configure)
        {
            var registry = new FubuRegistry();
            configure(registry);

            return Run(registry);
        }

        public static Harness Run(FubuRegistry registry)
        {
            var applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

            FubuMvcPackageFacility.PhysicalRootPath = applicationDirectory;


            var application = new FubuKayakApplication(new SimpleSource(registry));
            var port = PortFinder.FindPort(_port++);

            var reset = new ManualResetEvent(false);
            FubuRuntime runtime = null;

            ThreadPool.QueueUserWorkItem(o =>
            {
                // Need to make this capture the package registry failures cleanly
                application.RunApplication(port, r =>
                {
                    runtime = r;
                    reset.Set();
                });
            });

            reset.WaitOne();

            var root = "http://localhost:" + port;

            return new Harness(runtime, application, root);


            // Need to get the runtime
        }

        private readonly FubuRuntime _runtime;
        private readonly FubuKayakApplication _application;
        private readonly EndpointDriver _endpoints;

        public Harness(FubuRuntime runtime, FubuKayakApplication application, string root)
        {
            _runtime = runtime;
            _application = application;

            var urls = _runtime.Facility.Get<IUrlRegistry>();
            urls.As<UrlRegistry>().RootAt(root);

            _endpoints = new EndpointDriver(urls);
        }

        public EndpointDriver Endpoints
        {
            get { return _endpoints; }
        }


        public void Dispose()
        {
            _application.Stop();
        }
    }

    public static class HttpResponseExtensions
    {
        public static HttpResponse ContentShouldBe(this HttpResponse response, MimeType mimeType, string content)
        {
            response.ContentType.ShouldEqual(mimeType.Value);
            response.ReadAsText().ShouldEqual(content);

            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, string mimeType, string content)
        {
            response.ContentType.ShouldEqual(mimeType);
            response.ReadAsText().ShouldEqual(content);

            return response;
        }

        public static HttpResponse StatusCodeShouldBe(this HttpResponse response, HttpStatusCode code)
        {
            response.StatusCode.ShouldEqual(code);

            return response;
        }
    }
}