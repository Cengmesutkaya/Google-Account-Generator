using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main()
    {
        IWebDriver driver = new ChromeDriver();
        GmailSignupAutomation automation = new GmailSignupAutomation();
        automation.StartGmailSignup(driver);
    }
    public class GmailSignupAutomation
    {
        public void StartGmailSignup(IWebDriver driver)
        {
            try
            {
                //driver = ChangeIP();
                Random random = new Random();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Manage().Cookies.DeleteAllCookies();
                driver.Navigate().GoToUrl("https://accounts.google.com/signup");
                Thread.Sleep(3000); // Wait for page to load

                // Ad
                var (firstName, lastName) = GenerateRandomName();

                driver.FindElement(By.Id("firstName")).SendKeys(firstName);
                Thread.Sleep(3000);
                // Soyad
                driver.FindElement(By.Id("lastName")).SendKeys(lastName);
                Thread.Sleep(4000);
                // Next
                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(4000);

                // Doğum Ayı

                string[] months = {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };
                string randomMonth = months[random.Next(months.Length)];
                var monthElement = driver.FindElement(By.Id("month"));
                Thread.Sleep(3000);
                new SelectElement(monthElement).SelectByText(randomMonth);

                // Gün
                string day = random.Next(1, 28).ToString();
                Thread.Sleep(2000);
                driver.FindElement(By.Id("day")).SendKeys(day);

                // Yıl

                string year = random.Next(1970, 2000).ToString();
                Thread.Sleep(5000);
                driver.FindElement(By.Id("year")).SendKeys(year);

                // Cinsiyet
                string[] genderOptions = { "Rather not say", "Male", "Female" };

                var genderElement = driver.FindElement(By.Id("gender"));
                string selectedGender = genderOptions[random.Next(genderOptions.Length)];
                Thread.Sleep(6000);
                new SelectElement(genderElement).SelectByText(selectedGender);

                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(4000);

                if (IsErrorPresent(driver))
                {
                    IWebElement elementOne = driver.FindElement(By.Id("selectionc1"));
                    IWebElement elementTwo = driver.FindElement(By.Id("selectionc2"));

                    // Choose one randomly
                    IWebElement selectedElement = random.Next(2) == 0 ? elementOne : elementTwo;
                    Thread.Sleep(5000);
                    // Öğeyi tıklıyoruz
                    selectedElement.Click();
                    string elementText = selectedElement.Text;

                    driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                    Thread.Sleep(7000);

                }
                else
                {
                    // Kullanıcı adı
                    IWebElement usernameField = driver.FindElement(By.CssSelector("input[name='Username']"));
                    usernameField.SendKeys(GenerateUniqueUsername(firstName, lastName));

                    driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                    Thread.Sleep(5000);
                }

                string password = ResetPassword();
                // Şifre
                IWebElement passwordField = driver.FindElement(By.CssSelector("input[name='Passwd']"));
                Thread.Sleep(5000);
                passwordField.SendKeys(password);


                IWebElement passwordAgainField = driver.FindElement(By.CssSelector("input[name='PasswdAgain']"));
                Thread.Sleep(5000);
                passwordAgainField.SendKeys(password);

                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(8000);

                // Hata kontrolü
                if (IsErrorPresent(driver))
                {
                    Console.WriteLine("Error detected, restarting from the beginning...");
                    RestartSignupFlow(driver);
                    driver.Manage().Cookies.DeleteAllCookies();

                    return;
                }

                // Telefon Numarası
                driver.FindElement(By.Id("phoneNumberId")).SendKeys("phoneNumber");
                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(2000);

                Console.WriteLine("SMS doğrulama ve CAPTCHA ekranındasın.");
                Console.ReadLine(); // Manuel müdahale için beklet
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        private ChromeDriver ChangeIP()
        {
            //Create a chrome options object
            var chromeOptions = new ChromeOptions();
            //Create a new proxy object
            var proxy = new Proxy();
            //Set the http proxy value, host and port.
            proxy.HttpProxy = "196.43.97.114:5678";
            //Set the proxy to the Chrome options
            chromeOptions.Proxy = proxy;
            //Then create a new ChromeDriver passing in the options
            //ChromeDriver path isn't required if its on your path
            //If it now downloaded it and put the path here
            //var Driver = new ChromeDriver(@"C:\Users\Mesut Kaya\Desktop\", chromeOptions);
            var driver = new ChromeDriver(chromeOptions); // yol vermene gerek kalmaz

            //Navigation to a url and a look at the traffic logged in fiddler
            return driver;
        }

        static string GenerateUniqueUsername(string firstName, string lastName)
        {
            Random random = new Random();

            // Normalize: lowercase and remove whitespace
            firstName = firstName.Trim().ToLower();
            lastName = lastName.Trim().ToLower();

            // Generate 3-5 digit random number
            int randomDigits = random.Next(100, 10000); // e.g., 3 or 4 digits

            // Option 1: Combine name, surname and digits
            string username = $"{firstName}.{lastName}{randomDigits}";

            // Option 2 (alternative pattern): Use first letter of name + surname + digits
            // string username = $"{firstName[0]}{lastName}{randomDigits}";

            return username;
        }
        public static string ResetPassword()
        {
            Random random = new Random();

            int digit = random.Next(8, 10);

            string words = "9ABCD2EF3G3HI1JK1LM4N5OPR4ST7U6VY7ZWX8";
            string password = "";
            for (int i = 0; i < digit; i++)
            {
                password += words[random.Next(words.Length)];
            }
            return password;
        }
        private bool IsErrorPresent(IWebDriver driver)
        {
            try
            {
                var errorElement = driver.FindElement(By.XPath("//span[@jsslot='']"));
                return errorElement.Text.Contains("Error") || errorElement.Text.Contains("Choose your Gmail address");
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private void RestartSignupFlow(IWebDriver driver)
        {
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Navigate().GoToUrl("https://accounts.google.com/signup");
            Thread.Sleep(2000);
            StartGmailSignup(driver); // Recursive restart
        }

        static (string firstName, string lastName) GenerateRandomName()
        {
            // Predefined lists of first names and last names
            string[] firstNames = { "Liam", "Ava", "Ethan", "Sophie", "Maxwell", "Isla", "Oliver", "Maya", "Lucas", "Clara" };
            string[] lastNames = { "Harrington", "Windham", "Carver", "McAllister", "Prescott", "Fitzgerald", "Hawthorne", "Blackburn", "Vance", "Montgomery" };

            // Initialize random object
            Random random = new Random();

            // Get a random first name and last name
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];

            // Return as a tuple
            return (firstName, lastName);
        }
    }

}