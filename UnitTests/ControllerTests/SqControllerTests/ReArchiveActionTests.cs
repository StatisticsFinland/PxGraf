using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PxGraf.Controllers;
using PxGraf.Language;
using PxGraf.Settings;
using System;
using System.Threading.Tasks;
using UnitTests.Fixtures;

namespace ControllerTests
{
    internal class ReArchiveActionTests
    {
        private const string TEST_SQ_ID = "abc-123";

        [OneTimeSetUp]
        public void DoSetup()
        {
            Localization.Load(TranslationFixture.DefaultLanguage, TranslationFixture.Translations);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(TestInMemoryConfiguration.Get())
                .Build();
            Configuration.Load(configuration);
        }

        private static SqController BuildController()
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        [Test]
        public async Task SqNotFoundTest_NotFoundResult()
        {
            // TODO: write test
        }

        [Test]
        public async Task SqNotFoundTest_WrongChartType()
        {
            // TODO write test
        }

        [Test]
        public async Task SqNotFoundTest_Success()
        {
            // TODO write test
        }
    }
}
