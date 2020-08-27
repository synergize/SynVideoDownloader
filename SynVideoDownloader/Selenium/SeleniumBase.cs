using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;

namespace SynVideoDownloader.Selenium
{
    public abstract class SeleniumBase
    {
        public static IWebDriver WebDriver;
        private const string DriverName = "msedgedriver";

        protected SeleniumBase()
        {
            InitialStart();
        }

        private static IWebDriver CreateDriver()
        {
            try
            {
                var programDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var edgeService = EdgeDriverService.CreateChromiumService($"{programDirectory}\\Drivers", $"{DriverName}.exe");
                edgeService.SuppressInitialDiagnosticInformation = true;
                edgeService.HideCommandPromptWindow = true;

                var options = new EdgeOptions {UseChromium = true};
                options.AddArgument("headless");
                options.AddArgument("disable-gpu");

                return new EdgeDriver(edgeService, options);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Unable to download due to MicrosoftEdge not up to date. Please update it before proceeding.");
                throw;
            }
        }

        public static void TearDown()
        {
            WebDriver?.Quit();
            WebDriver = null;
        }

        private static void InitialStart()
        {
            var edgeDriver = Process.GetProcessesByName(DriverName);

            foreach (var edge in edgeDriver)
            {
                edge.Close();
            }

            if (WebDriver == null)
            {
                WebDriver = CreateDriver();
            }
        }
    }
}
