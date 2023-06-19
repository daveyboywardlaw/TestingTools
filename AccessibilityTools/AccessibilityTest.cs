using System.Xml.Linq;
using System.Xml;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Selenium.Axe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace AccessibilityTools
{
    public class AccessibilityTest
    {
        private static readonly HttpClient Client = new HttpClient();

        private IWebDriver _webDriver;

        [SetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArguments("window-size=1920,1080");

            _webDriver = new ChromeDriver(chromeOptions);
        }

        [TearDown]
        public void TearDown()
        {
            _webDriver.Dispose();
        }

        [Test]
        public async Task WebsiteAccessibilityTest()
        {
            var baseUrl = "http://www.mkdocs.org";

            var results = new List<AxeResult>();

            try
            {
                var defaultWait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));

                using var response = await Client.GetAsync(baseUrl + "/sitemap.xml");
                var responseBody = await response.Content.ReadAsStringAsync();

                var xDoc = XDocument.Parse(responseBody);

                var nsMgr = new XmlNamespaceManager(new NameTable());
                nsMgr.AddNamespace("sm", "http://www.sitemaps.org/schemas/sitemap/0.9");

                var documentLinks = xDoc.XPathSelectElements("/sm:urlset/sm:url/sm:loc", nsMgr);
                foreach (var link in documentLinks.Select(l => new Uri(l.Value)))
                {
                    await TestContext.Out.WriteLineAsync($"Testing site page: {link}");
                    _webDriver.Url = link.ToString();

                    results.Add(_webDriver.Analyze());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            var baseOutputDirectory = @"C:\AccessibilityReport";
            Directory.CreateDirectory(baseOutputDirectory);

            foreach (var result in results)
                _webDriver.CreateAxeHtmlReport(result, Path.Combine(baseOutputDirectory, $"{new Uri(result.Url).AbsolutePath.Trim('/').Replace('/', '_')}.report.html"), ReportTypes.Violations);

            var violations = results.SelectMany(r => r.Violations).GroupBy(v => v.Id);

            foreach (var violation in violations)
                TestContext.WriteLine($"{violation.Key} - {violation.Count()} occurrences");

            var details = results.ToArray();

            foreach (var detail in details)
            {
                TestContext.WriteLine("URL: " + detail.Url);

                foreach (var violationDetail in detail.Violations)
                {
                    TestContext.WriteLine("IMPACT: " + violationDetail.Impact);
                    TestContext.WriteLine("------------");
                    TestContext.WriteLine("DESCRIPTION: " + violationDetail.Description);
                    TestContext.WriteLine("------------");
                    TestContext.WriteLine("HELP: " + violationDetail.Help);
                    TestContext.WriteLine("------------");
                }
            }
        }
    }
}