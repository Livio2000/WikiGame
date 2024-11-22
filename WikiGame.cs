using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WikiGame
{
    [TestFixture]
    public class WikiGame
    {
        private IWebDriver _driver;
        private readonly IDictionary<string, string> _startAndEndPages = new Dictionary<string, string>();

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            string projectDir =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            var path = Path.Combine(projectDir, "Words.csv");
            using (var reader = new StreamReader(path))
            {
                var isHeader = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    var values = line.Split(',');
                    _startAndEndPages.Add(values[0], values[1]);
                }
            }
        }

        [Test]
        public void PlayGame()
        {
            foreach (var startAndEndPage in _startAndEndPages)
            {
                OpenWikipediaPage_CheckTitle(startAndEndPage.Key, startAndEndPage.Value);
            }
        }

        private void OpenWikipediaPage_CheckTitle(string startPage, string endWord)
        {
            var path = new List<string>();
            _driver.Navigate().GoToUrl("https://de.wikipedia.org/wiki/Spezial:Zuf%C3%A4llige_Seite");
            var bodyContent = string.Empty;
            while (!bodyContent.Contains("Philosophie", StringComparison.OrdinalIgnoreCase))
            {
                bodyContent = _driver.FindElement(By.Id("mw-content-text")).GetAttribute("innerHTML");
                if (bodyContent.Contains("Philosophie", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("The word 'Philosophie' was found in bodyContent.");
                }
                else
                {
                    var links = _driver.FindElements(By.CssSelector("#mw-content-text a"));
                    IWebElement firstWikiLink = null;
                    
                    foreach (var link in links)
                    {
                        var href = link.GetAttribute("href");
                        if (href != null 
                            && href.Contains("/wiki/") 
                            && !href.Contains("#") 
                            && !href.Contains("Datei:")
                            && !href.Contains("Portal:")
                            && !href.Contains("Hilfe:")
                            && !href.Contains("Wikipedia:")
                            && !path.Contains(link.Text))
                        {
                            firstWikiLink = link;
                            break;
                        }
                    }

                    Assert.IsNotNull(firstWikiLink, "No valid Wikipedia link found in bodyContent.");
                    path.Add(firstWikiLink.Text);
                    firstWikiLink.Click();
                }
            }

            foreach (var pathItem in path)
            {
                Console.WriteLine(pathItem);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }   
}