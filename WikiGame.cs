using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WikiGame
{
    [TestFixture]
    public class WikiGame
    {
        private IWebDriver _driver;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void OpenWikipediaPage_CheckTitle()
        {
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
                        if (href != null && href.Contains("/wiki/") && !href.Contains("#"))
                        {
                            firstWikiLink = link;
                            break;
                        }
                    }

                    Assert.IsNotNull(firstWikiLink, "No valid Wikipedia link found in bodyContent.");
                    firstWikiLink.Click();
                }
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