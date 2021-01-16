using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace JSzpilaTest
{
    class SeleniumTest
    {
        IWebDriver driver;
        string DestinationChlodna = "Chłodna 51";
        string DestinationPlacDefilad = "Plac Defilad 1";

        int waittime = 1500; //wait for 1.5sec

        string SelectedBrowser = "ED"; // CR - Chrome, ED - Edge,

        //DriverFolderPath
        //string DriverPath = "C:\\Users\\JS\\Downloads\\BrowserDrivers";

        //XPathRower
        string XPathRower = "/html/body/jsl/div[3]/div[9]/div[3]/div[1]/div[2]/div/div[2]/div/div/div[2]/div[1]/div[1]/button";

        //XPathPieszo
        string XPathPieszo = "/html/body/jsl/div[3]/div[9]/div[3]/div[1]/div[2]/div/div[2]/div/div/div[1]/div[4]/button";

        private bool CheckResults(int TimeResult, int TimeMax, double DistanceResult, double DistanceMax) //True - pass, False - fail
        {
            if (TimeResult >= TimeMax) return false; //The task specifies "mniej niż" (less than) so to fail : bigger or equal  is enough
            if (DistanceResult >= DistanceMax) return false;

            return true; //Time and Dist within spec, pass
        }

        [SetUp]
        public void StartBrowser()
        {
            switch (SelectedBrowser)
            {
                case "CR":
                    driver = new ChromeDriver();
                    break;
                //case "ED": //doesnt work for some reason, the correct package is referenced. It works if the edges driver .exe is pointed directly (and renamed)
                //    driver = new EdgeDriver();
                //    break;

                default:
                    driver = new ChromeDriver();
                    break;
            }

        }


        [TestCase("Plac Defilad 1", "Chłodna 51", 40, 3.00)]
        [TestCase("Chłodna 51", "Plac Defilad 1", 40, 3.00)]
        //[TestCase("Chłodna 51", "Plac Zbawiciela 1", 40, 3.00)] Not needed, commented out
        public void TestPieszo(string Start, string Goal, int MaxTime, double MaxDistance)
        {
            int ResultTime;
            float ResultDist;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathPieszo));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)
            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[5]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            //Check if results pass the requirements
            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);


            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDistance))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test1.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);
        }

        [TestCase("Plac Defilad 1", "Chłodna 51", 15, 3.00)]
        [TestCase("Chłodna 51", "Plac Defilad 1", 15, 3.00)]
        //[TestCase("Chłodna 51", "Plac Zbawiciela 1", 15, 3.00)] Not needed, commented out
        public void TestRower(string Start, string Goal, int MaxTime, double MaxDistance)
        {
            int ResultTime;
            float ResultDist;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathRower));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)

            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[7]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);

            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDistance))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test3.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);
        }
        

        //Same Tests, before parametrization
        

        //[Test]
        public void Test1() //PlacDefilad-Chłodna-Pieszo
        {
            int MaxTime = 40;
            float MaxDist = (float)3.00;

            int ResultTime;
            float ResultDist;

            string Start = DestinationPlacDefilad;
            string Goal = DestinationChlodna;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathPieszo));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)
            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[5]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            //Check if results pass the requirements
            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);


            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDist))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                //Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test1.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);



            //Close driver, finish test
            //Thread.Sleep(100);
        }
        
        //[Test]
        public void Test2() //Chłodna-PlacDefilad-Pieszo
        {
            int MaxTime = 40;
            float MaxDist = (float)3.00;

            int ResultTime;
            float ResultDist;

            string Start = DestinationChlodna;
            string Goal = DestinationPlacDefilad;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathPieszo));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)
            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[5]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);

            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDist))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                //Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test2.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);

            //Close driver, finish test
            //Thread.Sleep(100);
        }
        
        //[Test]
        public void Test3() //PlacDefilad-Chłodna-Rower
        {
            int MaxTime = 15;
            float MaxDist = (float)3.00;

            int ResultTime;
            float ResultDist;

            string Start = DestinationPlacDefilad;
            string Goal = DestinationChlodna;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathRower));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)

            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[7]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);

            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDist))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                //Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test3.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);

            //Close driver, finish test
            //Thread.Sleep(waittime0);
        }

        //[Test]
        public void Test4() //Chłodna-PlacDefilad-Rower
        {
            int MaxTime = 15;
            float MaxDist = (float)3.00;

            int ResultTime;
            float ResultDist;

            string Start = DestinationChlodna;
            string Goal = DestinationPlacDefilad;

            driver.Url = "http://www.google.pl/maps"; //go to specified page
            driver.Manage().Window.Maximize(); //maximaze, just to be sure

            try
            {
                //Add try-catch on this?
                //Skip Googles privacy popup
                driver.SwitchTo().Frame(0);
                IWebElement PrivacyPopup = driver.FindElement(By.Id("introAgreeButton"));
                PrivacyPopup.Click();
            }
            catch (OpenQA.Selenium.NotFoundException e)
            {
                Console.WriteLine("No Google privacy popup found...\n");
            }

            Thread.Sleep(waittime); //Used to make sure that the page loads, and no 'element not found' error occurs 

            //Find the location search bar and fill it
            IWebElement DestinationSearchBox = driver.FindElement(By.Name("q"));
            IWebElement NavigateButton = driver.FindElement(By.Id("searchbox-directions"));
            DestinationSearchBox.SendKeys(Goal);
            NavigateButton.Click();

            Thread.Sleep(waittime);

            //When the dialog opens fill the start location
            IWebElement StartSearchBox = driver.FindElement(By.XPath(".//*[@id='sb_ifc51']//input"));
            StartSearchBox.SendKeys(Start);

            Thread.Sleep(waittime);

            //Find 'Walking' travel type, and click
            //forcing the maps to compute the route
            IWebElement TravelType = driver.FindElement(By.XPath(XPathRower));
            TravelType.Click();

            Thread.Sleep(waittime);

            //Find First Result (Usually the fastest/shortest one)
            IWebElement Results = driver.FindElement(By.XPath("/html/body/jsl/div[3]/div[9]/div[8]/div/div[1]/div/div/div[7]/div[1]/div/div[3]/div[1]"));
            //Get the text of this result : contains time and distance
            string Resstring = Results.Text;


            ResultTime = int.Parse(Resstring.Split('\n')[0].Split(' ')[0]);
            ResultDist = float.Parse(Resstring.Split('\n')[1].Split(' ')[0]);

            if (CheckResults(ResultTime, MaxTime, ResultDist, MaxDist))
            { //Passed, Time and Dist within spec.
                Resstring += "\nPassed";
            }
            else
            { //Time or Dist too long
                Resstring += "\nNot passed";

                /*
                 * In case the task meant that if the time or distance isnt within spec the test is supposed to fail, not just write appropriate info to the console
                 * un-comment next line of code.
                 */
                //Assert.Fail("Time or distance not within spec...");
            }

            //Write results to console
            Console.WriteLine(Resstring);
            //Construct final string with start, goal, and results
            Resstring = Start + "\n" + Goal + "\n" + Resstring;
            //Write it to file
            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\test4.txt";
            Console.WriteLine(filepath);
            System.IO.File.WriteAllText(filepath, Resstring);

            //Close driver, finish test
            //Thread.Sleep(waittime0);
        }
        
        

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
