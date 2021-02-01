using System.Linq;
using WikimediaProcessor.Services.Readers;
using Xunit;

namespace WikimediaProcessor.Tests.Readers
{
    public class DomainCodeReaderTests
    {
        [Fact]
        public void GetDomainAndLanguageEnglishWikipediaTest()
        {
            var domainCodeReader = new DomainCodeReader();
            var domainCode = "en";
            var expectedDomain = $"{domainCode}.wikipedia.org";
            var expectedLanguage = domainCode;
            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);

            Assert.Equal(expectedDomain, domain);
            Assert.Equal(expectedLanguage, language);
        }

        [Fact]
        public void GetDomainAndLanguageSpanishMobileWikipediaTest()
        {
            var domainCodeReader = new DomainCodeReader();
            var domainCode = "es.m";
            var expectedDomain = $"{domainCode}.wikipedia.org";
            var expectedLanguage = "es";
            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);

            Assert.Equal(expectedDomain, domain);
            Assert.Equal(expectedLanguage, language);
        }

        [Fact]
        public void GetDomainAndLanguageCommonWikimediaTest()
        {
            var domainCodeReader = new DomainCodeReader();
            var domainCode = "commons.m";
            var expectedDomain = domainCodeReader.WikimediaOrgDomains.First(d => d.StartsWith("commons"));
            string expectedLanguage = null;
            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);

            Assert.Equal(expectedDomain, domain);
            Assert.Equal(expectedLanguage, language);
        }

        [Fact]
        public void GetDomainAndLanguageEnglishMobileWikisourceTest()
        {
            var domainCodeReader = new DomainCodeReader();
            var domainCode = "en.m.s";
            var expectedDomain = $"en.m{domainCodeReader.WikimediaDatabases[".s"]}";
            var expectedLanguage = "en";
            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);

            Assert.Equal(expectedDomain, domain);
            Assert.Equal(expectedLanguage, language);
        }

        [Fact]
        public void GetDomainAndLanguageGermanMobileWikivoyageTest()
        {
            var domainCodeReader = new DomainCodeReader();
            var domainCode = "de.m.voy";
            var expectedDomain = $"de.m{domainCodeReader.WikimediaDatabases[".voy"]}";
            var expectedLanguage = "de";
            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);

            Assert.Equal(expectedDomain, domain);
            Assert.Equal(expectedLanguage, language);
        }
    }
}
