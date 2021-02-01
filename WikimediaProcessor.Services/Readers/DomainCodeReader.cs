using System.Collections.Generic;
using System.Linq;

namespace WikimediaProcessor.Services.Readers
{
    public class DomainCodeReader : IDomainCodeReader
    {
        public Dictionary<string, string> WikimediaDatabases { get; }

        public List<string> WikimediaOrgDomains { get; }

        public DomainCodeReader()
        {
            WikimediaDatabases = LoadWikimediaDatabases();
            WikimediaOrgDomains = LoadWikimediaOrgDomains();
        }

        public (string, string) GetDomainAndLanguage(string domainCode)
        {
            var lastIndex = domainCode.LastIndexOf(".");
            if (lastIndex == -1)
            {
                // This means is a Wikipedia site in any language
                return ($"{domainCode}.wikipedia.org", domainCode);
            }
            else
            {
                var left = domainCode[0..lastIndex];
                var end = domainCode[lastIndex..];
                if (WikimediaDatabases.ContainsKey(end))
                {
                    var domainTrailingPart = WikimediaDatabases[end];
                    var wikimediaDomain = WikimediaOrgDomains.FirstOrDefault(d => d.StartsWith(left));
                    if (domainTrailingPart == ".wikimedia.org" && !string.IsNullOrEmpty(wikimediaDomain))
                    {
                        // This means this is a wikimedia.org domain with no language associated
                        return (wikimediaDomain, null);
                    }
                    else if (domainTrailingPart != ".wikimedia.org" && string.IsNullOrEmpty(wikimediaDomain))
                    {
                        // This means this is a mobile site from any of the other Wikimedia domains
                        return ($"{left}{domainTrailingPart}", domainCode[0..domainCode.IndexOf(".")]);
                    }
                    else
                    {
                        // This means this is a mobile site from Wikipedia
                        return ($"{domainCode}.wikipedia.org", left);
                    }
                }
            }
            return (null, null);
        }

        private static List<string> LoadWikimediaOrgDomains() =>
            new List<string>
            {
                "commons.wikimedia.org",
                "meta.wikimedia.org",
                "incubator.wikimedia.org",
                "species.wikimedia.org",
                "strategy.wikimedia.org",
                "outreach.wikimedia.org",
                "usability.wikimedia.org",
                "quality.wikimedia.org"
            };

        private static Dictionary<string, string> LoadWikimediaDatabases() =>
            new Dictionary<string, string>
            {
                { ".b", ".wikibooks.org" },
                { ".d", ".wiktionary.org" },
                { ".f", ".wikimediafoundation.org" },
                { ".m", ".wikimedia.org" },
                { ".n", ".wikinews.org" },
                { ".q", ".wikiquote.org" },
                { ".s", ".wikisource.org" },
                { ".v", ".wikiversity.org" },
                { ".voy", ".wikivoyage.org" },
                { ".w", ".mediawiki.org" },
                { ".wd", ".wikidata.org" }
            };
    }
}
