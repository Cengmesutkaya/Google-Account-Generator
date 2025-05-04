using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main()
    {
        Random rnd = new Random();

        var options = new ChromeOptions();

        // 1. Proxy ayarı
        //var proxy = new Proxy
        //{
        //    Kind = ProxyKind.Manual,
        //    IsAutoDetect = false,
        //    HttpProxy = "87.248.129.32:80",
        //    SslProxy = "87.248.129.32:80"
        //};
        //options.Proxy = proxy;

        // 2. Rastgele user-agent
        string[] userAgents = new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.3 Safari/605.1.15",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
        };
        options.AddArgument($"--user-agent={userAgents[rnd.Next(userAgents.Length)]}");
        options.AddArgument("--incognito");
        options.AddArgument("--ignore-certificate-errors");

        using (IWebDriver driver = new ChromeDriver(options))
        {
            GmailSignupAutomation automation = new GmailSignupAutomation();
            automation.StartGmailSignup(driver);
        }

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

                var firstNameInput = driver.FindElement(By.Id("firstName"));
                SlowType(firstNameInput, firstName);
                Thread.Sleep(random.Next(800, 2000)); // Bekleme


                var lastNameInput = driver.FindElement(By.Id("lastName"));
                SlowType(lastNameInput, lastName);
                Thread.Sleep(random.Next(1000, 2000)); // Bekleme


                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(4000);

                // Doğum Ayı

                string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                string randomMonth = months[random.Next(months.Length)];
                var monthElement = driver.FindElement(By.Id("month"));
                Thread.Sleep(random.Next(700, 1000)); // Bekleme
                new SelectElement(monthElement).SelectByText(randomMonth);

                // Gün
                string day = random.Next(1, 28).ToString();
                Thread.Sleep(random.Next(500, 900)); // Bekleme

                var dayInput = driver.FindElement(By.Id("day"));
                SlowType(dayInput, day);


                // Yıl

                string year = random.Next(1970, 2000).ToString();
                Thread.Sleep(random.Next(800, 1400)); // Bekleme
                var yearInput = driver.FindElement(By.Id("year"));
                SlowType(yearInput, year);

                // Cinsiyet
                string[] genderOptions = { "Rather not say", "Male", "Female" };

                var genderElement = driver.FindElement(By.Id("gender"));
                string selectedGender = genderOptions[random.Next(genderOptions.Length)];
                var select = new SelectElement(genderElement);
                Thread.Sleep(random.Next(500, 1200));
                select.SelectByText(selectedGender);

                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(random.Next(100, 800));

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
                    var username = GenerateUniqueUsername(firstName, lastName);
                    SlowType(usernameField, username);

                    driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                    Thread.Sleep(random.Next(300, 900));
                }

                string password = CreatePassword();
                // Şifre
                var passwordField = driver.FindElement(By.CssSelector("input[name='Passwd']"));
                Thread.Sleep(random.Next(300, 900));
                //passwordField.SendKeys(password);
                SlowType(passwordField, password);


                IWebElement passwordAgainField = driver.FindElement(By.CssSelector("input[name='PasswdAgain']"));
                Thread.Sleep(random.Next(200, 700));
                //passwordAgainField.SendKeys(password);
                SlowType(passwordAgainField, password);

                driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                Thread.Sleep(random.Next(300, 900));

                // Hata kontrolü
                if (IsErrorPresent(driver))
                {
                    Console.WriteLine("Error detected, restarting from the beginning...");
                    RestartSignupFlow(driver);
                    driver.Manage().Cookies.DeleteAllCookies();

                    return;
                }

                // Telefon Numarası
                //driver.FindElement(By.Id("phoneNumberId")).SendKeys("phoneNumber");
                //driver.FindElement(By.XPath("//span[contains(text(),'Next')]")).Click();
                //Thread.Sleep(2000);

                //Console.WriteLine("SMS doğrulama ve CAPTCHA ekranındasın.");
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

        public static void SlowType(IWebElement element, string text)
        {
            Random rnd = new Random();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(rnd.Next(100, 200));
            }
        }
        public static string GenerateUniqueUsername(string firstName, string lastName)
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
        public static string CreatePassword()
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
        public static (string firstName, string lastName) GenerateRandomName()
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