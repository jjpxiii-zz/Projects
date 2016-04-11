using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApplication1
{
    class Program
    {
        private static FirefoxDriver _driver;
        private static WebDriverWait _wait;
        private static readonly int MaxTimeout = 5;
        static void Main(string[] args)
        {
            var urlList = ConfigurationManager.AppSettings["urlList"].Split(';');
            FirefoxProfile profile = new FirefoxProfile(@"C:\Users\jjpantalacci\AppData\Local\Mozilla\Firefox\Profiles\1f6it2ug.default") { AcceptUntrustedCertificates = true };

            foreach (var url in urlList)
            {
                _driver = new FirefoxDriver(profile);
                _driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMinutes(5));
                _driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMinutes(5));
                _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(5));
                var baseUrl = string.Format($@"http://www.{url}/");
                var secureUrl = string.Format($@"https://secure.{url}/");
                _driver.Navigate().GoToUrl(baseUrl);

                OpenTab(baseUrl + "a1234");
                OpenTab(baseUrl + "a7714492");

                OpenTab(baseUrl + "a7283995");
                OpenTab(baseUrl + "a7638959");
                OpenTab(baseUrl + "a5017064");
                OpenTab(baseUrl + "s20/Roman-et-Nouvelles");
                OpenTab(baseUrl + "l826/Nouveautes-musique");
                OpenTab(baseUrl + "ia183958/Napalm-Death");
                OpenTab(baseUrl + "Maurice-et-Patapon/si5380");
                OpenTab(baseUrl + "Poussettes-par-type/Poussette-combinee/pl371872");

                // Ajout panier
                OpenTab(baseUrl + "SearchResult/ResultList.aspx?SCat=0!1&Search=tintin&sft=1&sa=0");
                Click(By.CssSelector(".FnacBtnAddBasket"));

                Click(By.ClassName("mfp-close"));
                Click(By.CssSelector(".mosaicButton"));
                Actions actions = new Actions(_driver);
                var menuHoverLink = _driver.FindElementByCssSelector(".itemosaic");
                actions.MoveToElement(menuHoverLink).Build().Perform();

                Click(By.CssSelector(".thumbnail-btn,.thumbnail-btnCart,.js-ProductBuy-add,.js-AddAndTrackProductThumbnailBasket"));
                Click(By.ClassName("mfp-close"));

                OpenTab(baseUrl + "a4051043/En-vivo-Blu-Ray-Edition-limitee-Blu-Ray");
                Click(By.CssSelector(".FnacBtnAddBasket"));

                Click(By.ClassName("mfp-close")); OpenTab(baseUrl + "a5679836");
                Click(By.CssSelector(".FnacBtnAddBasket"));


                Click(By.ClassName("mfp-close"));

                // secure
                OpenTab(secureUrl + @"Account/Logon/Logon.aspx");
                _driver.FindElementById("LogonAccountSteamRollPlaceHolder_ctl00_txtEmail").SendKeys("testh.hywill@yahoo.frX");
                _driver.FindElementById("LogonAccountSteamRollPlaceHolder_ctl00_txtPassword").SendKeys("fnac");
                Click(By.Id("LogonAccountSteamRollPlaceHolder_ctl00_btnPoursuivre"));


                OpenTab(baseUrl);
                _driver.FindElementById("Fnac_Search").SendKeys("canon");
                _wait.Until(d => ExpectedConditions.ElementIsVisible(By.CssSelector(".a-c_results,.a-c_groupresults,.modeBillet")));

            }
        }

        private static void Click(By by, int count = 0)
        {
            try
            {
                _driver.FindElement(by).Click();
            }
            catch (Exception e)
            {
                if (++count <= MaxTimeout)
                    Click(by, count);
            }
        }

        private static void OpenTab(string url, int count = 0)
        {
            try
            {
                IWebElement body = _driver.FindElement(By.TagName("body"));
                body.SendKeys(Keys.Control + 't');
                _driver.Navigate().GoToUrl(url);
                _wait.Until(d => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception e)
            {
                if (++count <= MaxTimeout)
                    OpenTab(url, count);
            }
        }
    }
}
