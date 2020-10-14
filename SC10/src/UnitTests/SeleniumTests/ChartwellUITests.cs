using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using SeleniumTests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;

namespace SeleniumTests
{
  [TestClass]
  public class ChartwellUITests
  {
    private TestContext testContextInstance;
    private IWebDriver driver;
    private string appURL;
    private FindElement findElement = new FindElement();
    private CommonHelpers commonHelpers = new CommonHelpers();
    private string propertyType;
    private string browserType;
    private string testEmailRecipient;

    public ChartwellUITests()
    {
      //Select property type either RetirementResidence Or LongTermCare
      propertyType = PropertyType.RetirementResidence.ToString();

      //browserType = BrowserType.iPhone678.ToString();
      browserType = BrowserType.Desktop.ToString();

      //set email recipients for test contact forms
      testEmailRecipient = "ergonzales@chartwell.com";
    }

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    [TestInitialize()]
    public void SetupTest()
    {
      appURL = (propertyType == "RetirementResidence") ?
          "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/overview" :
          "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/overview";

      string browser = "Chrome";
      switch (browser)
      {
        case "Chrome":
          ChromeOptions options = new ChromeOptions();
          switch (browserType)
          {
            case "Desktop":
              options.AddArgument("--start-maximized");
              break;
            case "iPhone678":
              options.EnableMobileEmulation("iPhone 6/7/8");
              break;
          }
          driver = new ChromeDriver(options);
          break;
        case "Firefox":
          driver = new FirefoxDriver();
          break;
        case "IE":
          driver = new InternetExplorerDriver();
          break;
        default:
          driver = new ChromeDriver();
          break;
      }

      driver.Navigate().GoToUrl(appURL + "/");
    }

    [TestCleanup()]
    public void MyTestCleanup()
    {
      driver.Quit();
    }

    #region Search Module
    [TestMethod, Description("Test Case 1.1, 2.11")]
    public void EnterCityNameHitEnter()
    {
      string cityName = "Toronto";
      string url = "https://chartwell.com/search-results/?City=" + cityName;
      commonHelpers.EnterCity(driver, cityName);
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify the correct page shows up after entering city name.
      Assert.AreEqual(url, driver.Url, "The search result page does not open.");
      var results = driver.FindElements(By.TagName("address"));

      //Verify the results on page for the city entered.
      foreach (IWebElement element in results)
      {
        Assert.IsTrue(City(cityName, propertyType).Any(s => element.Text.Contains(s)), "The Search results do not match with expected result.");
      }

      //Verify RR and LTC buttons are displayed.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.Id("test")), "Retirement Residences button is not displayed on Search Result Page.");
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.Id("test1")), "Long Term Care Residences button is not displayed on Search Result Page.");

      //Verify there are 9 results displayed.
      Assert.IsTrue(results.Count == 9, "There are more/less than 9 records displayed on Search Result Page.");

      //Verify the button 'View More Retirement Residences' is displayed on Search Result Page.
      var buttons = findElement.WebElements(driver, By.CssSelector("#CityLandingPageList a.btn.btn-default.selected"));
      Assert.IsTrue(buttons.Any(b => b.Text == "View More Retirement Residences"), "The button 'View More Retirement Residences' is not displayed on Search Result Page.");
    }

    [TestMethod, Description("Test Case 1.2")]
    public void EnterCityNameTabOut()
    {
      string cityName = "Toronto";
      commonHelpers.EnterCity(driver, cityName);
      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(Keys.Tab);

      //Verify Postal Code field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Postal Code field is not inactive.");

      //Verify Residence field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Residence field is not inactive.");

      //Verify Search button is selected.
      Assert.IsTrue(driver.SwitchTo().ActiveElement().GetAttribute("Id") == "ChartwellSearch", "The Search button is not selected.");
    }

    [TestMethod, Description("Test Cases 1.3, 1.11, 2.3")]
    public void SelectCityFromAutoPopulatedList()
    {
      string[] places = new string[2] { "Brampton", "Huron" };
      string[] placesInitials = new string[2] { "Bram", "Huron" };

      int index = 0;
      foreach (string city in places)
      {
        string cityNameInitials = placesInitials[index];
        string cityName = city;
        string url = "https://chartwell.com/search-results/?City=" + cityName;
        commonHelpers.EnterCity(driver, cityNameInitials);
        findElement.Wait(driver, By.CssSelector(".tt-menu.tt-open"), 5);
        findElement.WebElements(driver, By.CssSelector(".tt-suggestion.tt-selectable")).FirstOrDefault(s => s.Text == cityName).Click();
        findElement.WebElement(driver, By.Id("ChartwellSearch"), 2).Click();
        findElement.Wait(driver, By.ClassName("gridCity"), 5);

        //Verify the correct page shows up after entering city name.
        Assert.AreEqual(url, driver.Url, "The search result page does not open.");
        var results = driver.FindElements(By.TagName("address"));
        var distanceResults = driver.FindElements(By.CssSelector(".panel-footer p span.glyphicon-map-marker"));

        //Verify the results on page for the city entered.
        foreach (IWebElement element in results)
        {
          Assert.IsTrue(City(cityName, propertyType).Any(s => element.Text.Contains(s)), "The Search results do not match with expected result.");
        }
        Assert.AreEqual(results.Count, distanceResults.Count, "Not all results have distance displayed");
        index++;
      }
    }




    [TestMethod, Description("Test Cases 1.4, 1.5")]
    public void EnterRandomTextAndUSCity()
    {
      commonHelpers.EnterCity(driver, "quexoie");
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 2);
      string message = commonHelpers.GetSearchResultHeading(driver);
      Assert.AreEqual("Search results not found for Quexoie", message, "The message displayed does not match with expected message.");
      string dropdownOption = commonHelpers.GetSelectedValueFromAllPropertiesDropdown(driver);
      Assert.AreEqual("All Properties", dropdownOption, "The selected dropdown option does not match with expected value.");
      commonHelpers.EnterCity(driver, "Hollywood");
      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 2);
      message = commonHelpers.GetSearchResultHeading(driver);
      Assert.AreEqual("Search results not found for Hollywood", message, "The message displayed does not match with expected message.");
      dropdownOption = commonHelpers.GetSelectedValueFromAllPropertiesDropdown(driver);
      Assert.AreEqual("All Properties", dropdownOption, "The selected dropdown option does not match with expected value.");
    }

    [TestMethod, Description("Test Case 1.6")]
    public void ReviewTrouvezunerésidenceSearchFields()
    {
      if (browserType == "Desktop")
      {
        commonHelpers.SwitchBetweenENAndFR(driver, browserType, CommonHelpers.Language.French);
        string city = findElement.WebElement(driver, By.Id("City"), 5).GetAttribute("placeholder");
        string postalCode = findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("placeholder");
        string residence = findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("placeholder");
        Assert.AreEqual("Nom de la ville", city, "City field name is not " + city);
        Assert.AreEqual("Code postal (A0A 0A0)", postalCode, "Postal Code field name is not " + postalCode);
        Assert.AreEqual("Nom de la résidence", residence, "Residence field name is not " + residence);
      }
      else
      {
        commonHelpers.SwitchBetweenENAndFR(driver, browserType, CommonHelpers.Language.French);
        string city = findElement.WebElement(driver, By.Id("City"), 5).GetAttribute("placeholder");
        string residence = findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("placeholder");
        Assert.AreEqual("Nom de la ville", city, "City field name is not " + city);
        Assert.AreEqual("Nom de la résidence", residence, "Residence field name is not " + residence);
      }

    }

    [TestMethod, Description("Test Case 1.7, 2.17")]
    public void VerifyActiveInactiveFields()
    {
      //Click in City field.
      findElement.WebElement(driver, By.Id("City"), 2).Click();

      //Verify City field is active.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("City"), 2).GetAttribute("class").Contains("activeSearchField"), "City field is not active.");

      //Verify Postal Code field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Postal Code field is not inactive.");

      //Verify Residence field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Residence field is not inactive.");

      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(Keys.Tab);

      //Verify Search button is selected.
      Assert.IsTrue(driver.SwitchTo().ActiveElement().GetAttribute("Id") == "ChartwellSearch", "The Search button is not selected.");

      //Click in Postal Code field.
      findElement.WebElement(driver, By.Id("PostalCode"), 2).Click();

      //Verify Postal Code field is active.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("class").Contains("activeSearchField"), "Postal Code field is not active.");

      //Verify City field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("City"), 2).GetAttribute("class").Contains("inactiveSearchField"), "City field is not inactive.");

      //Verify Residence field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Residence field is not inactive.");

      findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Tab);

      //Verify Search button is selected.
      Assert.IsTrue(driver.SwitchTo().ActiveElement().GetAttribute("Id") == "ChartwellSearch", "The Search button is not selected.");

      //Click in Residency field.
      findElement.WebElement(driver, By.Id("PropertyName"), 2).Click();

      //Verify Residence field is active.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PropertyName"), 2).GetAttribute("class").Contains("activeSearchField"), "Residence field is not active.");

      //Verify City field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("City"), 2).GetAttribute("class").Contains("inactiveSearchField"), "City field is not inactive.");

      //Verify Postal Code field is inactive.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("class").Contains("inactiveSearchField"), "Postal Code field is not inactive.");

      findElement.WebElement(driver, By.Id("PropertyName"), 2).SendKeys(Keys.Tab);

      //Verify Search button is selected.
      Assert.IsTrue(driver.SwitchTo().ActiveElement().GetAttribute("Id") == "ChartwellSearch", "The Search button is not selected.");
    }

    /// <summary>
    /// This test method will be executed only when Browser Type is Desktop since the postal code feature is not there for mobile browsers..
    /// </summary>
    [TestMethod, Description("Test Case 1.8, 2.10")]
    public void ValidPostalCodeSearch()
    {
      if (browserType == "Desktop")
      {
        string postalCode = "m4l";
        string url = "https://chartwell.com/search-results/?PostalCode=m4l-1c3";

        commonHelpers.EnterPostalCode(driver, postalCode);
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys("1");
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys("c");
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys("3");

        Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("value") == "m4l 1c3", "The postal code in Postal Code field does not have space.");

        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);
        findElement.Wait(driver, By.ClassName("gridCity"), 5);

        IWebElement ele = findElement.WebElement(driver, By.Id("PostalCode"), 2);

        Assert.IsTrue(findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("value") == "m4l 1c3", "The postal code in Postal Code field does not have space.");

        //Verify the correct page shows up after entering postal code.
        Assert.AreEqual(url, driver.Url, "The search result page does not open.");

        //Verify Result message.
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("#CityLandingPageList h1"), 2).Text == "Retirement Homes Near M4L 1C3", "The Result message does not have Uppercase Postal Code.");

        //Verify RR and LTC buttons are displayed.
        Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.Id("test")), "Retirement Residences button is not displayed on Search Result Page.");
        Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.Id("test1")), "Long Term Care Residences button is not displayed on Search Result Page.");

        //Display residence details which includes: image, name, USP, distance in KM, address(street no., city, prov, postal code), phone no., and "View Residence" button.
        Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity img")).Count == 9, "Not All 9 properties have image displayed on Search Result Page.");
        Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-title span")).Count == 9, "Not all 9 properties have name displayed on Search Result Page.");
        Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-footer p")).All(d => d.Text.Contains("KM")), "Not all 9 properties have distance in KM displayed on Search Result Page.");
        Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-body p")).All(d => d.Text.Length > 0), "Not all 9 properties have USP displayed on Search Result Page.");

        var elements = findElement.WebElements(driver, By.CssSelector("address:nth-child(2)"));
        //Verify Street Numbers are displayed for the properties displayed on Search Result Page.
        Assert.IsTrue(elements.Any(s => Regex.Match(s.Text.Split(',')[0].Split(' ')[0].Trim(), @".*([\d]+).*").Success), "Street Number is not displayed for any of the properties.");

        //Verify City Names are displayed for the properties displayed on Search Result Page.
        Assert.IsTrue(elements.Any(s => Regex.Match(s.Text.Split(',')[1].Trim(), @"^[A-Z].*").Success), "City Name is not displayed for any of the properties.");

        //Verify Province Name is displayed for the properties displayed on Search Result Page.
        Assert.IsTrue(elements.Any(s => Regex.Match(s.Text.Split(',')[2].Substring(0, s.Text.Split(',')[2].Trim().Length - 7).Trim(), @"^[A-Z].*").Success), "Province Name is not displayed for any of the properties.");

        //Verify Postal Codes are displayed for the properties displayed on Search Result Page.
        Assert.IsTrue(elements.Any(s => Regex.Match(s.Text.Split(',')[2].Trim().Substring(s.Text.Split(',')[2].Trim().Length - 7, 7).Trim(), @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$").Success), "Postal Code is not displayed for any of the properties.");

        //Verity telephone numbers are displayed on Search Result Page.
        var telephoneNumbers = findElement.WebElements(driver, By.CssSelector(".gridCity .panel-footer h4 a"));
        Assert.IsTrue(telephoneNumbers.All(t => Regex.Match(t.Text.Trim(), @"([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})").Success));

        //Verify View Residence button.
        var viewResidenceButtons = findElement.WebElements(driver, By.CssSelector(".viewResBtn.text-right a"));
        Assert.IsTrue(viewResidenceButtons.Any(b => b.Text == "View Residence"), "The button 'View Residence' is not displayed on Search Result Page.");

        //verify that distances are displayed
        /*
        var distanceResults = driver.FindElements(By.XPath("//p[contains(text(),'Distance: ')]"));
        Assert.IsTrue(distanceResults.Any(s => Regex.Match(s.Text.Split(' ')[1], @".*([\d]+).*").Success), "Distances are not displayed for any of the properties.");
        Assert.IsTrue(distanceResults.All(s => s.Text.Contains("KM")));
        */
      }
    }

    [TestMethod, Description("Test Cases 1.9, 1.10")]
    public void RandomTextAndInvalidCodeInPostalCodeField()
    {
      if (browserType == "Desktop")
      {
        commonHelpers.EnterPostalCode(driver, RanStr(6));
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);

        //Verify the search result does not show up.
        Assert.AreEqual(appURL + "/", driver.Url, "Search results show up.");

        //Verify Search result grid does not show up.
        Assert.AreEqual(driver.FindElements(By.ClassName("gridCity")).Count, 0, "Search results show up.");
        //Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.ClassName("gridCity")), "Search results show up.");

        commonHelpers.EnterPostalCode(driver, "90210");
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);

        Assert.AreEqual("Postal Code (A0A 0A0)", findElement.WebElement(driver, By.Id("PostalCode"), 2).GetAttribute("placeholder"), "Postal code field takes invalid format.");

        commonHelpers.EnterPostalCode(driver, "A0A 0A0");
        findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);

        Assert.AreEqual("Search results not found for A0A 0A0", commonHelpers.GetSearchResultHeading(driver), "Search result heading does not match with expected message.");

        Assert.AreEqual("All Properties", commonHelpers.GetSelectedValueFromAllPropertiesDropdown(driver), "All Properties dropdown does not have expected selected value.");
      }
    }

    [TestMethod, Description("Test Case 1.9a")]
    public void PostalCodeLength()
    {
      string postalCode = "M4L";
      string url = "https://chartwell.com/search-results/?PostalCode=" + postalCode;

      commonHelpers.EnterPostalCode(driver, "M4L");
      findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify the correct page shows up after entering 3 chars of postal code.
      Assert.AreEqual(url, driver.Url, "The search result page does not open.");

      //Verify Search result grid shows up.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.ClassName("gridCity")), "Search results does not show up.");

      commonHelpers.EnterPostalCode(driver, "M4L1");
      findElement.WebElement(driver, By.Id("PostalCode"), 2).SendKeys(Keys.Enter);

      //Verify Error message.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("div[role='tooltip']"), 2).Text == "Please enter a 3 or 6-character Postal Code", "The Error message does not show up.");
    }

    [TestMethod, Description("Test Case 1.12")]
    public void EnterRegionInCityName()
    {
      string regionName = "Region-of-Durham";
      commonHelpers.EnterCity(driver, regionName);
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);
      var results = driver.FindElements(By.TagName("address"));
      //Verify the results on page for the city entered.
      foreach (IWebElement element in results)
      {
        Assert.IsTrue(City(regionName, propertyType).Any(s => element.Text.Contains(s)), "The Search results do not match with expected result.");
      }
    }

    [TestMethod, Description("Test Case 1.13, 1.14, 1.15, 1.16, 1.18")]
    public void VerifyResidence()
    {
      #region Variables
      List<string> expectedOptions = new List<string>();
      expectedOptions.Add("Chartwell Avondale Retirement Residence");
      expectedOptions.Add("Chartwell Jardins Laviolette résidence pour retraités");
      expectedOptions.Add("Chartwell Notre-Dame Victoriaville résidence pour retraités");
      expectedOptions.Add("Chartwell Stonehaven Retirement Residence");
      expectedOptions.Add("Chartwell Stonehaven Senior Apartments");
      expectedOptions.Add("Chartwell Woodhaven Long Term Care Residence");
      string residencyName = "Heritage";
      string expectedURL = "https://chartwell.com/en/continuum-of-care/chartwell-greenfield-park-retirement-community";
      #endregion

      #region Test Case 1.13
      commonHelpers.EnterResidency(driver, "ee");
      findElement.Wait(driver, By.CssSelector(".tt-menu.tt-open"), 5);
      findElement.WebElements(driver, By.CssSelector(".tt-suggestion.tt-selectable")).FirstOrDefault(s => s.Text == "Chartwell Greenfield Park Retirement Residence").Click();
      findElement.WebElement(driver, By.Id("ChartwellSearch"), 2).Click();
      findElement.Wait(driver, By.ClassName("gridCity"), 5);
      //Verify the correct page shows up after entering city name.
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");
      #endregion

      #region Test Case 1.14, 1.15
      commonHelpers.EnterResidency(driver, "AV");
      findElement.Wait(driver, By.CssSelector(".tt-menu.tt-open"), 5);
      List<string> actualAvailableOptions = commonHelpers.GetListOfAvailableOptionsFromDropdown(driver);
      Assert.IsTrue(!expectedOptions.Except(actualAvailableOptions).Any() && !actualAvailableOptions.Except(expectedOptions).Any(), "The available options in Residencey dropdown do not match with the expected values.");
      findElement.WebElements(driver, By.CssSelector(".tt-suggestion.tt-selectable")).FirstOrDefault(s => s.Text == "Chartwell Avondale Retirement Residence").Click();
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);

      expectedURL = "https://chartwell.com/en/retirement-residences/chartwell-avondale-retirement-residence/overview";
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");
      #endregion

      #region Test Case 1.16
      commonHelpers.EnterResidency(driver, residencyName);
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      var results = driver.FindElements(By.TagName("address"));

      //Verify the results on page for the city entered.
      foreach (IWebElement element in results)
      {
        Assert.IsTrue(City(residencyName, propertyType).Any(s => element.Text.Contains(s)), "The Search results do not match with expected result.");
      }
      #endregion

      #region Test Case 1.18
      expectedURL = "https://chartwell.com/en/continuum-of-care/chartwell-duke-of-devonshire-retirement-community";
      commonHelpers.EnterResidency(driver, "Duke");
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");
      #endregion
    }

    [TestMethod, Description("Test Case 1.17")]
    public void VerifyResidencyFieldWithNotFoundName()
    {
      #region Variables
      string expectedHeading = "Search results not found for Banana";
      #endregion
      commonHelpers.EnterResidency(driver, "Banana");
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);
      //Assert.IsTrue(!commonHelpers.VerifyElementPresent(driver, By.CssSelector("option[selected='selected']")));
      Assert.AreEqual("All Properties", findElement.WebElements(driver, By.CssSelector("option"))[0].Text);
      Assert.AreEqual(expectedHeading, commonHelpers.GetSearchResultHeading(driver), "The heading of search page is not " + expectedHeading);
    }
    #endregion

    #region Search Results (Grid)
    [TestMethod, Description("Test Case 2.1")]
    public void VerifyCountrySearchByHittingEnter()
    {
      #region Variables
      string countryName = "Canada";
      string expectedURL = "https://chartwell.com/search-results/?City=Canada";
      string pageTitleInEnglish = "Retirement Homes in and Around Canada";
      string pageTitleInFrench = "Résidences près de Canada";
      List<string> expectedOptionsInDropdown = new List<string>();
      expectedOptionsInDropdown.Add("All Properties");
      expectedOptionsInDropdown.Add("Alberta");
      expectedOptionsInDropdown.Add("British Columbia");
      expectedOptionsInDropdown.Add("Ontario");
      expectedOptionsInDropdown.Add("Quebec");
      List<string> expectedOptionsInDropdownFR = new List<string>();
      expectedOptionsInDropdownFR.Add("Toutes les résidences");
      expectedOptionsInDropdownFR.Add("Alberta");
      expectedOptionsInDropdownFR.Add("Colombie Britannique");
      expectedOptionsInDropdownFR.Add("Ontario");
      expectedOptionsInDropdownFR.Add("Québec");
      List<string> expectedThumbnails = new List<string>();
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Lord-Lansdowne/Lord_Lansdowne_Exterior_4.jpg?h=675&mw=1280&w=1200&hash=3749BD122C889456BB7444FA18F7EA3D");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Lord-Lansdowne/Lord_Lansdowne_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=2997DD369B02D0C41032A742CD6A6A5B");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Duke-of-Devonshire/Duke_of_Devonshire_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=DDE91E86C2EF3C145ABAF57FD5D56C1B");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Duke-of-Devonshire/Duke_of_Devonshire_Exterior_1.jpg?h=675&mw=1280&w=1200&hash=EDB720E2FF93E8BB75D629BD6DB85507");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rideau-Place/Rideau_Place_Exterior_3.jpg?h=675&mw=1280&w=1200&hash=64E75A50891BC22D5DDC2A8C8E802EFC");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rockcliffe/Rockcliffe_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=CB0873E05FE0052E010067128C69FD2A");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rockcliffe/Rockcliffe_Exterior_4.jpg?h=675&mw=1280&w=1200&hash=D4152BDC501F4AFCC2013A8ABD8443A4");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/New-Edinburgh-Square/New_Edinburgh_Square_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=DC8DD6F86347B85D33917BE93F4176C2");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Jardins-Notre-Dame/Jardins_Notre_Dame_Exterior_1.jpg?h=675&mw=1280&w=1200&hash=54351C7F4EF826595018B2BA9DE319CE");
      #endregion

      //Enter Country name in City field.
      commonHelpers.EnterCity(driver, countryName);
      //Hit Enter
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);
      //Verify the correct page shows up after entering country name.
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");

      //Verify page title in English
      Assert.AreEqual(pageTitleInEnglish, commonHelpers.GetSearchResultHeading(driver), "Page title in English is not as expected.");

      //Verity the All Properties dropdown is available.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.TagName("select")), "The All Properties dropdown is not available.");

      //Verify all available options in 'All Properties' dropdown.
      List<string> actualOptionsInDropdown = commonHelpers.GetOptionsFromAllPropertiesDropdown(driver);
      Assert.IsTrue(!expectedOptionsInDropdown.Except(actualOptionsInDropdown).Any() && !actualOptionsInDropdown.Except(expectedOptionsInDropdown).Any(), "The available options in All Properties dropdown do not match with the expected values.");

      //Verify Thumbnails are visible for Canada.
      List<string> actualThumbnails = commonHelpers.GetSourceOfThumbnailsOnResultPage(driver);
      Assert.IsTrue(expectedThumbnails.Any(x => actualThumbnails.Any(y => y == x)) && expectedThumbnails.Count == actualThumbnails.Count, "The available thumbnails do not match with the expected thumbnails.");

      //Verify View Residence Control is displayed.
      IList<IWebElement> viewResidenceElements = findElement.WebElements(driver, By.CssSelector(".viewResBtn.text-right a"));
      foreach (IWebElement element in viewResidenceElements)
      {
        Assert.IsTrue(element.Text == "View Residence", "One of the View Residence controls is not displayed in English.");
      }

      string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";

      //Change Language to French
      commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);

      //Verify page title in French
      Assert.AreEqual(pageTitleInFrench, commonHelpers.GetSearchResultHeading(driver), "Page title in French is not as expected.");

      //Verify all available options in 'All Properties' dropdown in French.
      List<string> actualOptionsInDropdownFR = commonHelpers.GetOptionsFromAllPropertiesDropdown(driver);
      Assert.IsTrue(!expectedOptionsInDropdownFR.Except(actualOptionsInDropdownFR).Any() && !actualOptionsInDropdownFR.Except(expectedOptionsInDropdownFR).Any(), "The available options in All Properties dropdown do not match with the expected values.");

      //Verify View Residence Control is displayed.
      IList<IWebElement> viewResidenceElementsFR = findElement.WebElements(driver, By.CssSelector(".viewResBtn.text-right a"));
      foreach (IWebElement element in viewResidenceElementsFR)
      {
        Assert.IsTrue(element.Text == "Voir la résidence", "One of the View Residence controls is not displayed in French.");
      }
    }

    [TestMethod, Description("Test Case 2.1")]
    public void VerifyCountrySearchBySelectingFromDropdown()
    {
      #region Variables
      string countryName = "Canada";
      string expectedURL = "https://chartwell.com/search-results/?City=Canada";
      string pageTitleInEnglish = "Retirement Homes in and Around Canada";
      string pageTitleInFrench = "Résidences près de Canada";
      List<string> expectedOptionsInDropdown = new List<string>();
      expectedOptionsInDropdown.Add("All Properties");
      expectedOptionsInDropdown.Add("Alberta");
      expectedOptionsInDropdown.Add("British Columbia");
      expectedOptionsInDropdown.Add("Ontario");
      expectedOptionsInDropdown.Add("Quebec");
      List<string> expectedOptionsInDropdownFR = new List<string>();
      expectedOptionsInDropdownFR.Add("Toutes les résidences");
      expectedOptionsInDropdownFR.Add("Alberta");
      expectedOptionsInDropdownFR.Add("Colombie Britannique");
      expectedOptionsInDropdownFR.Add("Ontario");
      expectedOptionsInDropdownFR.Add("Québec");
      List<string> expectedThumbnails = new List<string>();
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Lord-Lansdowne/Lord_Lansdowne_Exterior_4.jpg?h=675&mw=1280&w=1200&hash=3749BD122C889456BB7444FA18F7EA3D");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Lord-Lansdowne/Lord_Lansdowne_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=2997DD369B02D0C41032A742CD6A6A5B");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Duke-of-Devonshire/Duke_of_Devonshire_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=DDE91E86C2EF3C145ABAF57FD5D56C1B");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Duke-of-Devonshire/Duke_of_Devonshire_Exterior_1.jpg?h=675&mw=1280&w=1200&hash=EDB720E2FF93E8BB75D629BD6DB85507");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rideau-Place/Rideau_Place_Exterior_3.jpg?h=675&mw=1280&w=1200&hash=64E75A50891BC22D5DDC2A8C8E802EFC");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rockcliffe/Rockcliffe_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=CB0873E05FE0052E010067128C69FD2A");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Rockcliffe/Rockcliffe_Exterior_4.jpg?h=675&mw=1280&w=1200&hash=D4152BDC501F4AFCC2013A8ABD8443A4");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/New-Edinburgh-Square/New_Edinburgh_Square_Exterior_2.jpg?h=675&mw=1280&w=1200&hash=DC8DD6F86347B85D33917BE93F4176C2");
      expectedThumbnails.Add("https://chartwell.com/-/media/Images/photo-gallery/Jardins-Notre-Dame/Jardins_Notre_Dame_Exterior_1.jpg?h=675&mw=1280&w=1200&hash=54351C7F4EF826595018B2BA9DE319CE");
      #endregion

      //Enter Country name in City field.
      commonHelpers.EnterCity(driver, countryName);

      //Select Country from City dropdown.
      findElement.Wait(driver, By.CssSelector(".tt-menu.tt-open"), 5);
      findElement.WebElements(driver, By.CssSelector(".tt-suggestion.tt-selectable")).FirstOrDefault(s => s.Text == countryName).Click();

      //Click on Search button.
      findElement.WebElement(driver, By.Id("ChartwellSearch"), 2).Click();
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify the correct page shows up after entering country name.
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");

      //Verify page title in English
      Assert.AreEqual(pageTitleInEnglish, commonHelpers.GetSearchResultHeading(driver), "Page title in English is not as expected.");

      //Verity the All Properties dropdown is available.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.TagName("select")), "The All Properties dropdown is not available.");

      //Verify all available options in 'All Properties' dropdown.
      List<string> actualOptionsInDropdown = commonHelpers.GetOptionsFromAllPropertiesDropdown(driver);
      Assert.IsTrue(!expectedOptionsInDropdown.Except(actualOptionsInDropdown).Any() && !actualOptionsInDropdown.Except(expectedOptionsInDropdown).Any(), "The available options in All Properties dropdown do not match with the expected values.");

      //Verify Thumbnails are visible for Canada.
      List<string> actualThumbnails = commonHelpers.GetSourceOfThumbnailsOnResultPage(driver);
      Assert.IsTrue(expectedThumbnails.Any(x => actualThumbnails.Any(y => y == x)) && expectedThumbnails.Count == actualThumbnails.Count, "The available thumbnails do not match with the expected thumbnails.");

      //Verify View Residence Control is displayed.
      IList<IWebElement> viewResidenceElements = findElement.WebElements(driver, By.CssSelector(".viewResBtn.text-right a"));
      foreach (IWebElement element in viewResidenceElements)
      {
        Assert.IsTrue(element.Text == "View Residence", "One of the View Residence controls is not displayed in English.");
      }

      string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";

      //Change Language to French
      commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);

      //Verify page title in French
      Assert.AreEqual(pageTitleInFrench, commonHelpers.GetSearchResultHeading(driver), "Page title in French is not as expected.");

      //Verify all available options in 'All Properties' dropdown in French.
      List<string> actualOptionsInDropdownFR = commonHelpers.GetOptionsFromAllPropertiesDropdown(driver);
      Assert.IsTrue(!expectedOptionsInDropdownFR.Except(actualOptionsInDropdownFR).Any() && !actualOptionsInDropdownFR.Except(expectedOptionsInDropdownFR).Any(), "The available options in All Properties dropdown do not match with the expected values.");

      //Verify View Residence Control is displayed.
      IList<IWebElement> viewResidenceElementsFR = findElement.WebElements(driver, By.CssSelector(".viewResBtn.text-right a"));
      foreach (IWebElement element in viewResidenceElementsFR)
      {
        Assert.IsTrue(element.Text == "Voir la résidence", "One of the View Residence controls is not displayed in French.");
      }
    }

    [TestMethod, Description("Test Case 2.2")]
    public void VerifyProviceSearchByHittingEnter()
    {
      #region Variables
      string provinceName = "British Columbia";
      string expectedURL = "https://chartwell.com/search-results/?City=British-Columbia";
      #endregion
      commonHelpers.EnterCity(driver, provinceName);

      //Hit Enter
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify the correct page shows up after entering provice name.
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");

      //Verity the All Properties dropdown is not displayed.
      Assert.IsTrue(driver.FindElements(By.CssSelector("select[class='form-control regionDropDown']")).Count == 0, "The All Properties dropdown is displayed.");
    }

    [TestMethod, Description("Test Case 2.2")]
    public void VerifyProviceSearchBySelectingFromDropdown()
    {
      #region Variables
      string provinceName = "British Columbia";
      string expectedURL = "https://chartwell.com/search-results/?City=British-Columbia";
      #endregion
      commonHelpers.EnterCity(driver, provinceName);

      //Select Country from City dropdown.
      findElement.Wait(driver, By.CssSelector(".tt-menu.tt-open"), 5);
      findElement.WebElements(driver, By.CssSelector(".tt-suggestion.tt-selectable")).FirstOrDefault(s => s.Text == provinceName).Click();

      //Click on Search button.
      findElement.WebElement(driver, By.Id("ChartwellSearch"), 2).Click();
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify the correct page shows up after entering provice name.
      Assert.AreEqual(expectedURL, driver.Url, "The search result page does not open.");

      //Verity the All Properties dropdown is not displayed.
      Assert.IsTrue(driver.FindElements(By.CssSelector("select[class='form-control regionDropDown']")).Count == 0, "The All Properties dropdown is displayed.");
    }

    [TestMethod, Description("Test Cases 2.5, 2.6")]
    public void VerifyNewestResidences()
    {
      #region Variables
      List<string> expectedProvincesEN = new List<string>();
      expectedProvincesEN.Add("British Columbia");
      expectedProvincesEN.Add("Ontario");
      expectedProvincesEN.Add("Alberta");
      expectedProvincesEN.Add("Quebec");
      #endregion

      //Click on Logo to go to home page.
      findElement.WebElement(driver, By.CssSelector(".logoEn"), 3).Click();

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("homeVideo"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("VideoContainerRelative"), 30);
      }

      //Click on 'Read More' button for Newest Properties.
      commonHelpers.ClickElementByJS(driver, "a#btnTopNavRead.btn.btn-info.viewMoreBtn");

      //Verify the provinces names are displayed on newest properties page.
      List<string> actualProvinces = commonHelpers.GetProvincesDisplayedOnNewestPropertiesPage(driver, 20);
      Assert.IsTrue(!expectedProvincesEN.Except(actualProvinces).Any() && !actualProvinces.Except(expectedProvincesEN).Any(), "The procinces displayed on newest properties page does not match with expected values.");

      var results = driver.FindElements(By.TagName("address"));

      //Verify the results on page for the city entered.
      foreach (IWebElement element in results)
      {
        Assert.IsTrue(expectedProvincesEN.Any(s => element.Text.Contains(s)), "The Search results do not match with expected result.");
      }

      //Click on Logo to go to home page.
      findElement.WebElement(driver, By.CssSelector(".logoEn"), 3).Click();
      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("homeVideo"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("VideoContainerRelative"), 30);
      }

      string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";
      //Click on FR to change language to French.
      commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("homeVideo"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("VideoContainerRelative"), 30);
      }

      //Click on 'Voir plus' button for Nos nouvelles résidences.
      commonHelpers.ClickElementByJS(driver, "a#btnTopNavRead.btn.btn-info.viewMoreBtn");

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.CssSelector("#mobileNavToggle"), 30);
      }
      else
      {
        findElement.Wait(driver, By.Id("chartwellContactForm"), 30);
      }

      //Verify the provinces other than Quebec are not displayed on newest properties page.
      //Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.CssSelector(".text-center h3")), "There are provinces other than Quebec are also displayed on la-vie-en-résidence-à-québec page.");

      //Verify the province name Quebec is displayed.
      Assert.AreEqual("Nos nouvelles résidences", findElement.WebElement(driver, By.CssSelector("#mainBlock h1"), 5).Text);

      results = driver.FindElements(By.TagName("address"));

      //Verify the results on page.
      /*foreach (IWebElement element in results)
      {
          Assert.IsTrue(element.Text.Contains("Québec"));
      }*/
    }

    [TestMethod, Description("Test Case 2.6")]
    public void VerifyNewestResidencesViewResidence()
    {
      //Click on Logo to go to home page.
      findElement.WebElement(driver, By.CssSelector(".logoEn"), 3).Click();

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("homeVideo"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("VideoContainerRelative"), 30);
      }

      //Click on 'Read More' button for Newest Properties.
      commonHelpers.ClickElementByJS(driver, "a#btnTopNavRead.btn.btn-info.viewMoreBtn");

      findElement.Wait(driver, By.CssSelector(".panel.panel-primary"), 10);

      var properties = driver.FindElements(By.CssSelector(".panel.panel-primary"));
      string propertyName = properties[0].Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].Replace(' ', '-').ToLower();
      string expectedUrl = "https://chartwell.com/en/retirement-residences/" + propertyName + "/overview";

      //Click on Image of property displayed first.
      properties[0].FindElement(By.TagName("img")).Click();

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("lnk-overview"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("carouselNewDevelopment"), 20);
      }

      //Verify the property page is opened.
      Assert.AreEqual(expectedUrl, HttpUtility.UrlDecode(driver.Url, System.Text.Encoding.UTF8), "Clicking on New Residency does not land on Property page.");
    }

    [TestMethod, Description("Test Case 2.7")]
    public void VerifyClickOnResidenceImage()
    {
      //Enter City Name in City Field.
      commonHelpers.EnterCity(driver, "Toronto");

      //Hit Enter button.
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      var properties = driver.FindElements(By.CssSelector(".panel.panel-primary"));
      string propertyName = properties[0].Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].Replace(' ', '-').ToLower();
      string expectedUrl = "https://chartwell.com/en/retirement-residences/" + propertyName + "/overview";

      //Click on Image of property displayed first.
      properties[0].FindElement(By.TagName("img")).Click();

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("lnk-overview"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("carouselNewDevelopment"), 20);
      }

      //Verify the property page is opened.
      Assert.AreEqual(expectedUrl, HttpUtility.UrlDecode(driver.Url, System.Text.Encoding.UTF8), "Clicking on property image on Search Result Grid does not open Property page.");
    }

    [TestMethod, Description("Test Case 2.8")]
    public void VerifyClickOnResidenceName()
    {
      //Enter City Name in City Field.
      commonHelpers.EnterCity(driver, "Toronto");

      //Hit Enter button.
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      var properties = driver.FindElements(By.CssSelector(".panel.panel-primary .panel-title a"));
      string propertyName = properties[0].Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].Replace(' ', '-').ToLower();
      string expectedUrl = "https://chartwell.com/en/retirement-residences/" + propertyName + "/overview";

      //Click on first property's title on Search Result grid.
      properties[0].Click();

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("lnk-overview"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("carouselNewDevelopment"), 20);
      }

      //Verify the property page is opened.
      Assert.AreEqual(expectedUrl, HttpUtility.UrlDecode(driver.Url, System.Text.Encoding.UTF8), "Clicking on Property's name on Search Result Grid does not open Property page.");
    }

    [TestMethod, Description("Test Case 2.9")]
    public void VerifyClickOnViewResidenceBtn()
    {
      //Enter City Name in City Field.
      commonHelpers.EnterCity(driver, "Toronto");

      //Hit Enter button.
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      var properties = driver.FindElements(By.CssSelector(".panel.panel-primary"));
      string propertyName = properties[0].Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0].Replace(' ', '-').ToLower();
      string expectedUrl = "https://chartwell.com/en/retirement-residences/" + propertyName + "/overview";

      //Click on first property's View Property button on Search Result grid.
      commonHelpers.ClickElementByJS(driver, ".panel.panel-primary .viewResBtn.text-right a:nth-of-type(1)");

      if (browserType != "Desktop")
      {
        findElement.Wait(driver, By.Id("lnk-overview"), 20);
      }
      else
      {
        findElement.Wait(driver, By.Id("carouselNewDevelopment"), 20);
      }

      //Verify the property page is opened.
      Assert.AreEqual(expectedUrl, HttpUtility.UrlDecode(driver.Url, System.Text.Encoding.UTF8), "Clicking on View Property button Search Result Grid does not open Property page.");
    }

    [TestMethod, Description("Test Case 2.12, 2.13")]
    public void VerifyClickOnViewMoreRetirementHomesOrLTCBtn()
    {
      string[] expectedButtonText = new string[2] { "View More Retirement Residences", "View More Long Term Care Residences" };
      for (var x = 0; x < 2; x++)
      {
        commonHelpers.EnterCity(driver, "Toronto");
        findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
        findElement.Wait(driver, By.ClassName("gridCity"), 5);

        //Click on 'View More Retirement Residences' or 'View More Long Term Care Residences' button.
        driver.FindElements(By.CssSelector("#CityLandingPageList a.btn.btn-default"))[x].Click();

        findElement.Wait(driver, By.ClassName("gridCity"), 5);
        //Verify there are 18 results displayed.
        var results = driver.FindElements(By.TagName("address"));
        Assert.IsTrue(results.Count == 9, "There are more/less than 9 records displayed on Search Result Page.");

        //Verify the button 'View More Retirement Residences' or 'View More Long Term Care Residences' is displayed on Search Result Page.
        //Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("#CityLandingPageList > a.btn.btn-default.selected")), "View More Retirement Residences or View More Long Term Care Residences button is not displayed.");

        //Verify that the button text matches the expected
        if (x == 1)
        {
          var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
          wait.Until(driver => !driver.FindElement(By.Id("searchoverlay")).Displayed);
        }
        Assert.AreEqual(driver.FindElements(By.CssSelector("#CityLandingPageList a.btn.btn-default"))[2].Text, expectedButtonText[x]);
      }
    }

    [TestMethod, Description("Test Case 2.14")]
    public void VerifyEnteringPartOfResidenceNameAndHitEnter()
    {
      #region Variables
      string partOfResidenceName = "Heritage";
      string expectedURL = "https://chartwell.com/search-results/?PropertyName=" + partOfResidenceName;
      #endregion
      //Enter Paft of Residence Name in Residence Field.
      commonHelpers.EnterResidency(driver, partOfResidenceName);

      //Hit Enter.
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);

      //Verify URL.
      Assert.AreEqual(expectedURL, driver.Url, "The URL does not contain property name");
    }

    [TestMethod, Description("Test Case 2.15")]
    public void VerifyBothRetirementResidenceAndLTC()
    {
      commonHelpers.EnterCity(driver, "Cannington");
      findElement.WebElement(driver, By.Id("City"), 5).SendKeys(Keys.Enter);
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      //Verify Retirement Residence Properties are displayed.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("#CityLandingPageList h1"), 5).Text.Contains("Retirement"), "The Search Result page does not display title for Retirement Residencies.");
      Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-title span")).Any(e => e.Text.Contains("Retirement Residence")), "Retirement Residence properties are not displayed.");

      findElement.WebElement(driver, By.Id("test1"), 5).Click();
      findElement.Wait(driver, By.ClassName("gridCity"), 5);

      System.Threading.Thread.Sleep(5000);

      //Verify Retirement Residence Properties are displayed.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("#CityLandingPageList h1"), 5).Text.Contains("Long Term Care"), "The Search Result page does not display title for Long Term Care.");
      Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-title span")).Any(e => e.Text.Contains("Long Term Care")), "Long Term Care properties are not displayed.");

      //Verify that Bon Air is first
      Assert.IsTrue(findElement.WebElements(driver, By.CssSelector(".gridCity .panel-title span"))[0].Text.Contains("Bon Air"), "Bon Air is not displayed first");
    }

    [TestMethod, Description("Test Case 2.16")]
    public void VerifyResidenceFieldWithBothPropertiesRetirementAndLTC()
    {
      #region Variables
      string partOfResidenceName = "Waterford";
      string expectedURL = "https://chartwell.com/en/continuum-of-care/chartwell-waterford-retirement-community";
      #endregion
      //Enter Paft of Residence Name in Residence Field.
      commonHelpers.EnterResidency(driver, partOfResidenceName);

      //Hit Enter.
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);

      //Verify URL.
      Assert.AreEqual(expectedURL, driver.Url, "The splitter page does not open.");
    }

    #endregion

    #region Getting Started
    [TestMethod, Description("Test Case 3.1.1")]
    public void VerifyUnderstandingTheBenefits()
    {
      #region Variables
      string introBlurbContent = "Today’s retirement residences are so much more than the old-fashioned concept of “your grandmother’s nursing home.” No longer are seniors choosing retirement living just because they need support with health or mobility challenges, but because they want to lead a worry-free lifestyle that provides the freedom to live each day as they choose.";
      string closingBlurbContent1 = "Delicious and nutritious dining options; access to exercise classes, leisure opportunities and outings; housekeeping, laundry and transportation; personal support to help you lead each day comfortably—these are all contemporary services that you or a loved one can benefit from in a retirement residence. Yet, embracing retirement living also brings with it intangible benefits like an increased sense of security, an enriched social life and overall peace of mind.";
      string closingBlurbContent2 = "Life doesn’t have to change when you move into a senior living community. Rather, you’ll find the services and amenities Chartwell residences offer are designed to complement your existing lifestyle and empower you to decide what aspects of daily life you want support with, allowing you to focus on what brings you the most happiness in life.";
      #endregion

      //Click on 'Understanding the Benefits' menu under 'Getting Started' menu.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType.ToString(), CommonHelpers.GettingStartedMenu.UnderstandingTheBenefits, 5);

      if (browserType == "Desktop")
      {
        //Verify 'Understanding the Benefits' section exists.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Understanding the Benefits", 2), "The section 'Understanding the Benefits' does not exit.");

        //Verify 'Misconceptions about Retirement Living' section exists.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Misconceptions about Retirement Living", 2), "The section 'Misconceptions about Retirement Living' does not exist.");

        //Verify 'A Day in the Life of a Resident' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "A Day in the Life of a Resident", 2), "The section 'A Day in the Life of a Resident' does not exist.");

        //Verify 'Searching for Self' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Searching for Self", 2), "The section 'Searching for Self' does not exist.");

        //Verify 'Am I Ready Questionnaire' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Am I Ready Questionnaire", 2), "The section 'Am I Ready Questionnaire' does not exist.");

        //Verify 'Benefits of Retirement Living' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Benefits of Retirement Living", 2), "The section 'Benefits of Retirement Living' does not exist.");

        //Verify 'Exploring Retirement Living' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Exploring Retirement Living", 2), "The section 'Exploring Retirement Living' does not exist.");

        //Verify 'How to Make the Transition' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "How to Make the Transition", 2), "The section 'How to Make the Transition' does not exist.");

        //Verify 'Searching for a Loved One' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Searching for a Loved One", 2), "The section 'Searching for a Loved One' does not exist.");

        //Verify 'Is It Time Questionnaire' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Is It Time Questionnaire", 2), "The section 'Is It Time Questionnaire' does not exist.");

        //Verify 'Peace of Mind' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Peace of Mind", 2), "The section 'Peace of Mind' does not exist.");

        //Verify 'Supporting a Loved One' section exits.
        Assert.IsTrue(commonHelpers.VerifyLeftSectionOnUnderstandingTheBenefitsPage(driver, "Supporting a Loved One", 2), "The section 'Supporting a Loved One' does not exist.");
      }

      //Verify Breadcrumb exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("ol.breadcrumb")), "Breadcrumb does not exist.");

      //Verify title 'Why Retirement Living?' exists.
      Assert.IsTrue(findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim() == "Why Retirement Living?", "The tile 'Why Retirement Living? does not exist.'");

      //Verify Embedded Video with title 'Independence' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Independence']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Indepence' or the embedded video does not exists.");

      //Verify Embedded Video with title 'Security' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Security']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Security' or the embedded video does not exists.");

      //Verify Embedded Video with title 'Social Life' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Social Life']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Social Life' or the embedded video does not exists.");

      //Verify Embedded Video with title 'Nutrition' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Nutrition']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Nutrition' or the embedded video does not exists.");

      //Verify Embedded Video with title 'Worry-Free Living' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Worry-Free Living']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Worry-Free Living' or the embedded video does not exists.");

      //Verify Embedded Video with title 'Active Living' exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.XPath("//h3[.='Active Living']/following-sibling::a[contains(@href,'https://youtu.be/')]")), "Either the title 'Active Living' or the embedded video does not exists.");

      //Verify intro blurb content exists.
      Assert.AreEqual(introBlurbContent, findElement.WebElements(driver, By.CssSelector(".col-md-12 p"))[1].Text.Trim(), "Intro blurb content is not as expected.");

      //Verify closing blurb content exists.
      Assert.AreEqual(closingBlurbContent1, findElement.WebElements(driver, By.CssSelector(".col-md-12 p"))[2].Text.Trim(), "Closing blurb content is not as expected.");
      Assert.AreEqual(closingBlurbContent2, findElement.WebElements(driver, By.CssSelector(".col-md-12 p"))[3].Text.Trim(), "Closing blurb content is not as expected.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }
    }

    [TestMethod, Description("Test Case 3.1.2")]
    public void VerifyADayInTheLifeOfResidentPage()
    {
      //Click on menu 'A Day In The Life Of A Resident'
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.ADayInTheLifeOfAResident, 5);

      //Verify Page Title
      Assert.IsTrue(driver.FindElements(By.TagName("h1"))[0].Text.Contains("A Day in the Life of a Resident"), "The heading does not contain 'A Day in the Life ...'");

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/Understanding-Benefits/day-of-life.jpg']")), "The thumbnail image does not display.");

      //Click on 'A day in the life of a resident' thumbnail.
      findElement.WebElement(driver, By.CssSelector("a[href='/-/media/Files/infographics/chartwell-a-day-in-the-life-of-a-resident.pdf']"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/infographics/chartwell-a-day-in-the-life-of-a-resident.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.1.3, 3.1.4, 3.1.5")]
    public void VerifyConsiderIndependentLivingRecommendation()
    {
      //Click on menu 'AM I Ready Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.AmIReadyQuestionnaire, 5);

      //Verify "Am I Ready?" Questionnaire page displays.
      Assert.AreEqual("https://chartwell.com/en/getting-started/searching-for-self/am-i-ready-questionnaire", driver.Url, "'AM I Ready? Questionnaire page does not open.'");

      //Fill out questions 1-5.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 1, "Never", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 2, "Never", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 3, "Yes, I can take care of my outdoor housework", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 4, "Often", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 5, "No", 5);

      //Verify progress.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector(".progress"), 5).Text == "36%", "After filling the questions 1-5, the progess is not 36%.");

      //Click on Next button.
      commonHelpers.ClickElementByJS(driver, ".action.next.btn.btn-default");

      //Fill out questions 5-10.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 6, "No, I don't take medication or I'm always good at remembering", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 7, "Yes, frequently", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 8, "Never", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 9, "Yes, using the stairs is an easy task for me", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 10, "Never", 5);

      //Verify progress.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector(".progress"), 5).Text == "71%", "After filling the questions 5-10, the progess is not 71%.");

      //Click on Next button.
      commonHelpers.ClickElementByJS(driver, ".action.next.btn.btn-default");

      //Fill out questions 11-14.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 11, "Often", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 12, "Yes, I participate in enough physical activities to meet my needs", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 13, "No, I'm very comfortable driving myself around", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 14, "No", 5);

      //Verify progress.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector(".progress"), 5).Text == "100%", "After filling the questions 11-14, the progess is not 100%.");

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Click on Click here link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"http://chartwell.com/subscribe\"]");

      //Verify subscribe page opens.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual("https://chartwell.com/subscribe", newDriver.Url, "The subscribe page does not open.");


    }

    [TestMethod, Description("Test Case 3.1.5")]
    public void VerifyNotTimeForRCButImportantToConsiderYourOptions()
    {
      #region Variables
      List<string> AmIReadyQuestionnaireAnswers = new List<string>();
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Sometimes, I could use some additional help");
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Not recently, but I have before");
      AmIReadyQuestionnaireAnswers.Add("Yes, sometimes");
      AmIReadyQuestionnaireAnswers.Add("Periodically");
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Sometimes, but I wish I didn't have to use them at all");
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Sometimes");
      AmIReadyQuestionnaireAnswers.Add("Some, but I could benefit from more fitness options");
      AmIReadyQuestionnaireAnswers.Add("Sometimes, but having someone drive for me would help a lot");
      AmIReadyQuestionnaireAnswers.Add("Not at the moment");
      #endregion

      //Click on menu 'AM I Ready Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.AmIReadyQuestionnaire, 5);

      //Fill out AM I Ready? Questionnaire.
      commonHelpers.FilloutAmIReadyQuestionnaire(driver, AmIReadyQuestionnaireAnswers, 5);

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Click on Chartwell.com link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"https://chartwell.com/\"]");

      //Verify home page opens.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual("https://chartwell.com/", newDriver.Url, "The home page does not open.");
    }

    [TestMethod, Description("Test Case 3.1.5")]
    public void VerifyRightTimeToConsiderARC()
    {
      #region Variables
      List<string> AmIReadyQuestionnaireAnswers = new List<string>();
      AmIReadyQuestionnaireAnswers.Add("Often");
      AmIReadyQuestionnaireAnswers.Add("Often");
      AmIReadyQuestionnaireAnswers.Add("No, I have trouble");
      AmIReadyQuestionnaireAnswers.Add("Rarely");
      AmIReadyQuestionnaireAnswers.Add("Yes");
      AmIReadyQuestionnaireAnswers.Add("Yes, often");
      AmIReadyQuestionnaireAnswers.Add("No, rarely");
      AmIReadyQuestionnaireAnswers.Add("Often");
      AmIReadyQuestionnaireAnswers.Add("No, I find it very difficult to use the stairs");
      AmIReadyQuestionnaireAnswers.Add("Often");
      AmIReadyQuestionnaireAnswers.Add("Often");
      AmIReadyQuestionnaireAnswers.Add("Yes, I participate in enough physical activities to meet my needs");
      AmIReadyQuestionnaireAnswers.Add("Sometimes, but having someone drive for me would help a lot");
      AmIReadyQuestionnaireAnswers.Add("Yes");
      #endregion

      //Click on menu 'AM I Ready Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.AmIReadyQuestionnaire, 5);

      //Fill out AM I Ready? Questionnaire.
      commonHelpers.FilloutAmIReadyQuestionnaire(driver, AmIReadyQuestionnaireAnswers, 5);

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Click on Chartwell.com link
      commonHelpers.ClickElementByJS(driver, "a[href=\"//chartwell.com/\"]");

      //Verify home page opens.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual("https://chartwell.com/", newDriver.Url, "The home page does not open.");

      driver.SwitchTo().Window(tabs[0]);

      //Click on Help Me Choose link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"//chartwell.com/Help-Me-Choose/Exploring-Your-Options\"]");

      Assert.AreEqual("https://chartwell.com/Help-Me-Choose/Exploring-Your-Options", newDriver.Url, "The home page does not open.");
    }

    [TestMethod, Description("Test Case 3.1.6")]
    public void VerifyErrorMessageOnIncompleteQuestions()
    {
      //Click on menu 'AM I Ready Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.AmIReadyQuestionnaire, 5);

      //Fill out questions 1-3-5.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 1, "Never", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 3, "Yes, I can take care of my outdoor housework", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 5, "No", 5);

      //Click on Next button.
      commonHelpers.ClickElementByJS(driver, ".action.next.btn.btn-default");

      //Verify Error message.
      Assert.AreEqual("Please select one of these options", findElement.WebElement(driver, By.Id("q2-error"), 3).Text, "The error message for question 2 is not as expected.");
      Assert.AreEqual("Please select one of these options", findElement.WebElement(driver, By.Id("q4-error"), 3).Text, "The error message for question 4 is not as expected.");
    }

    [TestMethod, Description("Test Case 3.1.8")]
    public void VerifyBenefitsOfRetirementLivingPage()
    {
      #region Variables
      string openingStatement = "Making the move to a seniors’ residence can be an exciting new chapter in your life. With convenient services and support available, a retirement lifestyle can enable you to pursue your interests, enjoy an active lifestyle, and develop new friendships.";
      #endregion
      //Click on menu 'Benefits of Retirement Living'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.BenefitsOfRetirementLiving, 5);

      //Verify title
      Assert.AreEqual("The Benefits of Retirement Living", findElement.WebElement(driver, By.TagName("h1"), 3).Text, "The title does not display.");

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits/1325_Benefits-Retirement-Living_Download_But_ENG.jpg']")), "The thumbnail image does not display.");

      //Verify opening statement.
      Assert.AreEqual(openingStatement, findElement.WebElements(driver, By.CssSelector(".staticPage p"))[0].Text.Trim(), "Opening Statement is not as expected.");

      //Verify image on center of the page.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits/think-about.png']")), "The image does not display on center of the page.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits/1325_Benefits-Retirement-Living_Download_But_ENG.jpg']"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/infographics/chartwell-the-benefits-of-retirement-living-infographic.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.1.9")]
    public void VerifyExploringRetirementLivingPage()
    {
      #region Variables
      string openingStatement = "Coming to the decision that a move to a retirement residence would be a beneficial lifestyle change isn’t always easy, and you may wonder whether the timing is right. It’s natural to feel hesitant about embarking on a new chapter in life, as well as have questions if it’s unfamiliar territory.";
      string pointOne = "Understand the benefits of a retirement living lifestyle";
      string pointTwo = "Assess whether you are ready to consider retirement living";
      string pointThree = "Determine how to find the right residence for your needs and preferences";
      string pointFour = "Explain accommodation and care options";
      string pointFive = "Learn what questions to ask on a residence tour";
      string pointSix = "Discover what you can do to make the transition to retirement living easier";
      #endregion
      //Click on menu 'Exploring Retirement Living'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.ExploringRetirementLiving, 5);

      //Verify title
      Assert.AreEqual("Exploring Retirement Living", findElement.WebElement(driver, By.TagName("h1"), 3).Text, "The title does not display.");

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/Chartwell_ExploringRetirement_Thumbnail_Eng.jpg']")), "The thumbnail image does not display.");

      //Verify opening statement.
      Assert.AreEqual(openingStatement, findElement.WebElements(driver, By.CssSelector(".staticPage p"))[0].Text.Trim(), "Opening Statement is not as expected.");

      //Verify bullet points.
      Assert.AreEqual(pointOne, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[0].Text.Trim(), "1st bullet point is not as expected.");
      Assert.AreEqual(pointTwo, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[1].Text.Trim(), "Second bullet point is not as expected.");
      Assert.AreEqual(pointThree, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[2].Text.Trim(), "Third bullet point is not as expected.");
      Assert.AreEqual(pointFour, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[3].Text.Trim(), "Fourth bullet point is not as expected.");
      Assert.AreEqual(pointFive, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[4].Text.Trim(), "Fifth bullet point is not as expected.");
      Assert.AreEqual(pointSix, findElement.WebElements(driver, By.CssSelector(".staticPage li"))[5].Text.Trim(), "Sixth bullet point is not as expected.");

      //Verify hyperlink.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/-/media/Files/guides/self/chartwell-exploring-retirement-living-guide.pdf']")), "The link does not exists.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/Chartwell_ExploringRetirement_Thumbnail_Eng.jpg']"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/guides/self/chartwell-exploring-retirement-living-guide.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.1.10")]
    public void VerifyMakingTheTransitionIntoRetirementLivingPage()
    {
      #region Variables
      string openingStatement = "Transitioning into retirement living requires advance planning as well as an openness to change. Ultimately, the benefits of downsizing into the space you need, with the amenities, services and support you want, will be smoother if you follow a few simple steps before making a final decision:";
      #endregion
      //Click on menu 'How To Make The Transition'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.HowToMakeTheTransition, 5);

      //Verify title
      Assert.AreEqual("Making the Transition into Retirement Living", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //Verify opening statement.
      Assert.AreEqual(openingStatement, findElement.WebElements(driver, By.CssSelector(".row.staticPage p"))[0].Text.Trim(), "Opening Statement is not as expected.");

      //Verify hyperlink.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options']")), "The link does not exists.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-getting-started-checklist_download_but_ENG.jpg']")), "The thumbnail image does not display.");

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-getting-started-checklist_download_but_ENG.jpg']"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/guides/generic/chartwell-getting-started-checklist-EN.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("3.1.11, 3.1.12, 3.1.13")]
    public void VerifyIsItTimeQuestionnaireGoodFitForRetirementRL()
    {
      //Click on menu 'Is It Time Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.IsItTimeQuestionnaire, 5);

      //Verify page.
      Assert.AreEqual("https://chartwell.com/en/getting-started/searching-for-a-loved-one/is-it-time-questionnaire", driver.Url, "The is-it-time-questionnaire page does not open.");

      //Fill out questions 1-5.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 1, "Never", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 2, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 3, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 4, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 5, "No", 5);

      //Verify progress.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector(".progress"), 5).Text == "50%", "After filling the questions 1-5, the progess is not 50%.");

      //Click on Next button.
      commonHelpers.ClickElementByJS(driver, ".action.next.btn.btn-default");

      //Fill out questions 5-10.
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 6, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 7, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 8, "My loved one has access to and participates in physical activity on a regular basis", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 9, "No", 5);
      commonHelpers.SelectAnswerOfAMIReadyQuestionnaire(driver, 10, "No", 5);

      //Verify progress.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector(".progress"), 5).Text == "100%", "After filling the questions 1-5, the progess is not 100%.");

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Click on Click here link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"//chartwell.com/subscribe\"]");

      //Verify subscribe page opens.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual("https://chartwell.com/subscribe", newDriver.Url, "The subscribe page does not open.");
    }

    [TestMethod, Description("Test Case 3.1.13")]
    public void VerifyTimeToConsiderYourFutureOptions()
    {
      #region Variables
      List<string> IsItTimeQuestionnaireAnswers = new List<string>();
      IsItTimeQuestionnaireAnswers.Add("Sometimes");
      IsItTimeQuestionnaireAnswers.Add("No, but I am worried this may occur");
      IsItTimeQuestionnaireAnswers.Add("No, but I fear I may in the future");
      IsItTimeQuestionnaireAnswers.Add("A little bit");
      IsItTimeQuestionnaireAnswers.Add("Maybe");
      IsItTimeQuestionnaireAnswers.Add("Not yet, but expect they will in our future");
      IsItTimeQuestionnaireAnswers.Add("Maybe within the foreseeable future");
      IsItTimeQuestionnaireAnswers.Add("None, and he or she would benefit from a community that provided this.");
      IsItTimeQuestionnaireAnswers.Add("No, but this is a strong possibility for the future");
      IsItTimeQuestionnaireAnswers.Add("No, but I may in the future");
      #endregion

      //Click on menu 'Is It Time Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.IsItTimeQuestionnaire, 5);

      //Fill out Is It Ready Questionnaire.
      commonHelpers.FilloutIsItTimeQuestionnaire(driver, IsItTimeQuestionnaireAnswers, 5);

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Verify link exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresentByJS(driver, "a[href='tel:18554610685']"), "The phone link does not exists.");
    }

    [TestMethod, Description("Test Case 3.1.13")]
    public void VerifyTimeToSearchForThePerfectResidence()
    {
      #region Variables
      List<string> IsItTimeQuestionnaireAnswers = new List<string>();
      IsItTimeQuestionnaireAnswers.Add("Often");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Some, but he or she could benefit from more options.");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      IsItTimeQuestionnaireAnswers.Add("Yes");
      #endregion

      //Click on menu 'Is It Time Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.IsItTimeQuestionnaire, 5);

      //Fill out Is It Ready Questionnaire.
      commonHelpers.FilloutIsItTimeQuestionnaire(driver, IsItTimeQuestionnaireAnswers, 5);

      //Click on View Recommendations button.
      commonHelpers.ClickElementByJS(driver, "#surveyReady");

      //Verify Recommendation page opens.
      Assert.IsTrue(findElement.WebElement(driver, By.Id("myModalLabel"), 5).Text == "Recommendations", "Recommendation page does not open.");

      //Verify link exists.
      Assert.IsTrue(commonHelpers.VerifyElementPresentByJS(driver, "a[href='tel:18554610685']"), "The phone link does not exists.");

      //Click on Chartwell.com link
      commonHelpers.ClickElementByJS(driver, "a[href=\"//chartwell.com/Help-Me-Choose/Exploring-Your-Options\"]");

      //Verify home page opens.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual("https://chartwell.com/Help-Me-Choose/Exploring-Your-Options", newDriver.Url, "The Exploring-Your-Options page does not open.");
    }

    [TestMethod, Description("Test Case 3.1.14")]
    public void VerifyErrorMessageOnIncompleteQuestionsOnIsItTimeQuestionnaire()
    {
      //Click on menu 'Is It Time Questionnaire'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.IsItTimeQuestionnaire, 5);

      //Fill out questions 1-3-5.
      commonHelpers.SelectAnswerOfIsItTimeQuestionnaire(driver, 1, "Never", 5);
      commonHelpers.SelectAnswerOfIsItTimeQuestionnaire(driver, 3, "Yes", 5);
      commonHelpers.SelectAnswerOfIsItTimeQuestionnaire(driver, 5, "No", 5);

      //Click on Next button.
      commonHelpers.ClickElementByJS(driver, ".action.next.btn.btn-default");

      //Verify Error message.
      Assert.AreEqual("Please select one of these options", findElement.WebElement(driver, By.Id("q2-error"), 3).Text, "The error message for question 2 is not as expected.");
      Assert.AreEqual("Please select one of these options", findElement.WebElement(driver, By.Id("q4-error"), 3).Text, "The error message for question 4 is not as expected.");
    }

    [TestMethod, Description("Test Case 3.1.16")]
    public void VerifyPeaceOfMindPage()
    {
      #region Variables
      string openingStatement = "Friendly staff, comfortable suites and a welcoming atmosphere are the hallmarks of every Chartwell residence.";
      #endregion
      //Click on menu 'Peace of Mind'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.PeaceOfMind, 5);

      //Verify title
      Assert.AreEqual("Peace of Mind", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //Verify opening statement.
      Assert.AreEqual(openingStatement, findElement.WebElements(driver, By.CssSelector(".col-md-12 blockquote> p"))[0].Text.Trim(), "Opening Statement is not as expected.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-getting-started-checklist_download_but_ENG.jpg")), "The thumbnail image does not display.");

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-getting-started-checklist_download_but_ENG.jpg"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/guides/generic/chartwell-getting-started-checklist-EN.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.1.17")]
    public void VerifySupportingALovedOnePage()
    {
      #region Variables
      string openingStatement = "Once a decision has been made to explore a retirement home, the most frequent questions are, where do we begin and how do we choose the right retirement community? Chartwell’s “Supporting a Loved One” guide helps identify:";
      #endregion
      //Click on menu 'Supporting a Loved One'.
      commonHelpers.ClickMenuInGettingStarted(driver, browserType, CommonHelpers.GettingStartedMenu.SupportingALovedOne, 5);

      //Verify title
      Assert.AreEqual("Supporting a Loved One", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //Verify opening statement.
      Assert.AreEqual(openingStatement, findElement.WebElements(driver, By.CssSelector(".staticPage p"))[0].Text.Trim(), "Opening Statement is not as expected.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-supporting-a-loved-one-thumb-en.jpg")), "The thumbnail image does not display.");

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/getting-started/chartwell-supporting-a-loved-one-thumb-en.jpg"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf", newDriver.Url, "The pdf does not open.");
    }
    #endregion

    #region Help Me Choose
    [TestMethod, Description("Test Case 3.2.1")]
    public void VerifyExploringYourOptions()
    {
      //Click on menu 'Exploring Your Options'.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.ExploringYourOptions, 5);

      //Verify title displays.
      Assert.AreEqual("Exploring Your Options", findElement.WebElement(driver, By.CssSelector(".moveUp h1"), 5).Text.Trim(), "The title does not display.");

      //Verify Icons displays for Independent Living.
      Assert.AreEqual("Independent Living", findElement.WebElement(driver, By.CssSelector("#gtm-independent-living"), 5).Text.Trim(), "The sevice Independent Living does not display.");
      Assert.AreEqual("https://chartwell.com/-/media/Images/icons/exploring-your-options/independent-living.svg", findElement.WebElement(driver, By.CssSelector("#gtm-independent-living img"), 5).GetAttribute("src"), "The image for sevice Independent Living does not display.");

      //Verify Icons displays for Independent Suppotive Living.
      Assert.AreEqual("Independent Supportive Living", findElement.WebElement(driver, By.CssSelector("#gtm-independent-supportive-living"), 5).Text.Trim(), "The sevice Independent Supportive Living does not display.");
      Assert.AreEqual("https://chartwell.com/-/media/Images/icons/exploring-your-options/independent-supportive-living.svg", findElement.WebElement(driver, By.CssSelector("#gtm-independent-supportive-living img"), 5).GetAttribute("src"), "The image for sevice Independent Supportive Living does not display.");

      //Verify Icons displays for Assisted Living.
      Assert.AreEqual("Assisted Living", findElement.WebElement(driver, By.CssSelector("#gtm-assisted-living"), 5).Text.Trim(), "The sevice Assisted Living does not display.");
      Assert.AreEqual("https://chartwell.com/-/media/Images/icons/exploring-your-options/assisted-living.svg", findElement.WebElement(driver, By.CssSelector("#gtm-assisted-living img"), 5).GetAttribute("src"), "The image for sevice Assisted Living does not display.");

      //Verify Icons displays for Memory Living.
      Assert.AreEqual("Memory Living", findElement.WebElement(driver, By.CssSelector("#gtm-memory-living"), 5).Text.Trim(), "The sevice Memory Living does not display.");
      Assert.AreEqual("https://chartwell.com/-/media/Images/icons/exploring-your-options/memory-living.svg", findElement.WebElement(driver, By.CssSelector("#gtm-memory-living img"), 5).GetAttribute("src"), "The image for sevice Memory Living does not display.");

      //Verify Icons displays for Long Term Care.
      Assert.AreEqual("Long Term Care", findElement.WebElement(driver, By.CssSelector("#gtm-long-term-care"), 5).Text.Trim(), "The sevice Long Term Care does not display.");
      Assert.AreEqual("https://chartwell.com/-/media/Images/icons/exploring-your-options/long-term-care.svg", findElement.WebElement(driver, By.CssSelector("#gtm-long-term-care img"), 5).GetAttribute("src"), "The image for sevice Long Term Care does not display.");

      //Verify tabs toggle between one to another displaying Read More button.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options/Independent-Living']")), "The Read More button does not display for Independent Living.");

      commonHelpers.ClickElementByJS(driver, "#gtm-independent-supportive-living");
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options/independent-supportive-living']")), "The Read More button does not display for Independent Supportive Living.");

      commonHelpers.ClickElementByJS(driver, "#gtm-assisted-living");
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options/assisted-living']")), "The Read More button does not display for Assisted Living.");

      commonHelpers.ClickElementByJS(driver, "#gtm-memory-living");
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options/memory-care']")), "The Read More button does not display for Memory Living.");

      commonHelpers.ClickElementByJS(driver, "#gtm-long-term-care");
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("a[href='/help-me-choose/exploring-your-options/long-term-care']")), "The Read More button does not display for Long Term Care.");

      //Verify clicking Read More button opens Independent Living page.
      findElement.WebElement(driver, By.CssSelector("#gtm-independent-living"), 5).Click();
      commonHelpers.ClickElementByJS(driver, "a[href=\"/help-me-choose/exploring-your-options/Independent-Living\"]");
      Thread.Sleep(3000);
      Assert.AreEqual("https://chartwell.com/help-me-choose/exploring-your-options/Independent-Living", driver.Url, "Indepnedent Living page does not open.");
      driver.Navigate().Back();

      //Verify clicking Read More button opens Independent Supportive Living page.
      Thread.Sleep(3000);
      commonHelpers.ClickElementByJS(driver, "#gtm-independent-supportive-living");
      commonHelpers.ClickElementByJS(driver, "a[href=\"/help-me-choose/exploring-your-options/independent-supportive-living\"]");
      Thread.Sleep(3000);
      Assert.AreEqual("https://chartwell.com/help-me-choose/exploring-your-options/independent-supportive-living", driver.Url, "Indepnedent Supportive Living page does not open.");
      driver.Navigate().Back();

      //Verify clicking Read More button opens Assisted Living page.
      Thread.Sleep(3000);
      commonHelpers.ClickElementByJS(driver, "#gtm-assisted-living");
      commonHelpers.ClickElementByJS(driver, "a[href=\"/help-me-choose/exploring-your-options/assisted-living\"]");
      Thread.Sleep(3000);
      Assert.AreEqual("https://chartwell.com/help-me-choose/exploring-your-options/assisted-living", driver.Url, "Assisted Living page does not open.");
      driver.Navigate().Back();

      //Verify clicking Read More button opens Memory Living page.
      Thread.Sleep(3000);
      commonHelpers.ClickElementByJS(driver, "#gtm-memory-living");
      commonHelpers.ClickElementByJS(driver, "a[href=\"/help-me-choose/exploring-your-options/memory-care\"]");
      Thread.Sleep(3000);
      Assert.AreEqual("https://chartwell.com/help-me-choose/exploring-your-options/memory-care", driver.Url, "Memory Living page does not open.");
      driver.Navigate().Back();

      //Verify clicking Read More button opens Long Term Care page.
      Thread.Sleep(3000);
      commonHelpers.ClickElementByJS(driver, "#gtm-long-term-care");
      commonHelpers.ClickElementByJS(driver, "a[href=\"/help-me-choose/exploring-your-options/long-term-care\"]");
      Thread.Sleep(3000);
      Assert.AreEqual("https://chartwell.com/help-me-choose/exploring-your-options/long-term-care", driver.Url, "Long Term page does not open.");
    }

    [TestMethod, Description("Test Case 3.2.2")]
    public void VerifyIndependentLiving()
    {
      #region Variables
      string heading = "Independent Living";
      string youTubeLink = "https://www.youtube.com/embed/UeZayR1WMng?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "What is Independent Living?";
      string title2 = "Benefits of Independent Living";
      string title3 = "How do I move into a Chartwell retirement community that offers Independent Living?";
      string para1 = "Independent living is a lifestyle option offered at Chartwell that can include a range of convenient services, such as housekeeping, leisure opportunities and outings, and even the availability of à la carte dining options at an additional fee. Typically, seniors who choose independent living do not require any personal care services, and are seeking social engagement, convenience and the peace of mind of a secure community.";
      string para2 = "Independent living accommodations at Chartwell retirement communities can vary—from retirement living suites with kitchenettes, to seniors apartments with full kitchens, to townhomes, cottages and bungalows.";
      string para3 = "Are you seeking a worry-free lifestyle uninhibited by some of the tasks associated with homeownership, such as general maintenance, yard work or even snow shovelling? Would you benefit from living in a community setting where you can socialize with friends whenever you choose? When you have retirement living conveniences like housekeeping, peace of mind services like 24-hour security and emergency response, and even entertainment and outings at your disposal, you’ll find it frees up your time to live your retirement exactly how you want to, and in a setting that can adapt to your needs as they change over time.";
      string para4 = "We invite you to reach out to us about availability at the Chartwell retirement community you are interested in! When you visit, staff may conduct an assessment to ensure independent living will provide you with the lifestyle you desire, and may recommend another option if required.";
      string para5 = "To find the residence nearest you, or to learn more about independent living, call 1-855-461-0685 today to speak with a Chartwell representative who can help.";
      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.IndependentLiving, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeLink, findElement.WebElement(driver, By.Id("genvid"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title2, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title3, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, "em"), "The text content on web page is not as expected.");
    }

    [TestMethod, Description("Test Case 3.2.3")]
    public void VerifyIndependentSupportiveLiving()
    {
      #region Variables
      string heading = "Independent Supportive Living";
      string youTubeLink = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "What is Independent Supportive Living?";
      string title2 = "Benefits of Independent Supportive Living";
      string title3 = "How do I move into a retirement community that offers Independent Supportive Living?";
      string para1 = "Independent supportive living may combine all of the benefits of an independent lifestyle in a retirement residence—housekeeping and laundry, leisure and social opportunities and 24-hour security—with delicious daily meals and the ability to take advantage of personal support services like medication administration or assistance with your daily routine.";
      string para2 = "Personal support services may be offered by Chartwell staff, through government-funded home care agencies or private pay services, depending on your province of residence and the services in the retirement community you are interested in.";
      string para3 = "Are you an active and independent senior who could benefit from some additional daily support? Many people who choose independent supportive living appreciate not having to grocery shop or do all the cooking, and feel peace of mind having services like 24-hour emergency response and personal support options like medication administration at hand. An active social life and a complement of recreational opportunities like exercise classes, themed events and outings can also help you lead your worry-free retirement years to the fullest.";
      string para4 = "We invite you to inquire about availability at the Chartwell retirement community you are interested in! When you visit, staff may conduct an assessment to ensure independent supportive living will provide you with the right amount of support you need, and may recommend another lifestyle option if required.";
      string para5 = "To find the residence nearest you, or to learn more about independent supportive living, call 1-855-461-0685 today to speak with a Chartwell representative who can help.";

      string independeSupportiveLivingSrc = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string assistedLivingSrc = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string memoryCareSrc = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string longTermCareSrc = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string retirementVsLongTermCareSrc = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";

      string independentLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-living";
      string assistedLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/assisted-living";
      string memoryCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/memory-care";
      string longTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/long-term-care";
      string retirementVsLongTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/retirement-versus-long-term-care";
      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.IndependentSupportiveLiving, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeLink, findElement.WebElement(driver, By.TagName("iframe"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title2, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title3, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, "em"), "The text content on web page is not as expected.");

      if (browserType == "Desktop")
      {
        //Able to toggle between pages.
        findElement.WebElement(driver, By.Id("lnk-Independent Living"), 3).Click();
        findElement.Wait(driver, By.Id("genvid"), 5);
        Assert.AreEqual(independentLivingURL, driver.Url, "The Indepedent Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Assisted Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);
        Assert.AreEqual(assistedLivingURL, driver.Url, "The Assisted Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Memory Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);
        Assert.AreEqual(memoryCareURL, driver.Url, "The Memory Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);
        Assert.AreEqual(longTermCareURL, driver.Url, "The Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Retirement versus Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);
        Assert.AreEqual(retirementVsLongTermCareURL, driver.Url, "The Retirement Vs Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);
      }
    }

    [TestMethod, Description("Test Case 3.2.4")]
    public void VerifyAssistedLiving()
    {
      #region Variables
      string heading = "Assisted Living";
      string youTubeVideoLink = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "What is Assisted Living?";
      string title2 = "Benefits of Assisted Living";
      string title3 = "How do I move into a retirement community that offers Assisted Living?";
      string para1 = "Chartwell’s Assisted Living program is specially-designed to support individuals who would benefit from daily personalized care to lead a better quality of life. Staff are there to ensure your days are spent in comfort and satisfaction, and aim to maximize independence and peace of mind through convenient service, such as medication administration and supervision, assistance with aspects of your daily routine, or an escort to meals and activities.";
      string para2 = "Many Chartwell retirement residences that offer assisted living services have dedicated neighbourhoods or floors with their own dining rooms and activity lounges.";
      string para3 = "Assisted living is beneficial for older adults who need highly-personalized support in order to live comfortably and with peace of mind. Delicious and nutritious meals, specially-designed activities, housekeeping and laundry, individualized exercise and rehabilitation services and even dementia support all complement a care plan that is reflective of your unique needs, preferences and choices.";
      string para4 = "For residents of Ontario, Quebec and British Columbia, we invite you to inquire about availability at the Chartwell retirement community you are interested in. When you visit, staff will conduct an assessment to ensure assisted living will provide you with the right amount of support you need, and may recommend another lifestyle option if required.";
      string para5 = "Known as Designated Supportive Living in Alberta, eligibility for assisted living in the province requires an assessment by a Home Care Registered Nurse. Information on Designated Supportive Living spaces can be obtained by calling HEALTHLink Alberta toll-free at 1-866-408-5465.";
      string para6 = "To find the residence nearest you, or to learn more about assisted living, call 1-855-461-0685 today to speak with a Chartwell representative who can help.";

      string independeSupportiveLivingSrc = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string assistedLivingSrc = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string memoryCareSrc = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string longTermCareSrc = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string retirementVsLongTermCareSrc = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";

      string independentLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-living";
      string independentSupportiveLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-supportive-living";
      string memoryCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/memory-care";
      string longTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/long-term-care";
      string retirementVsLongTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/retirement-versus-long-term-care";
      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.AssistedLiving, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeVideoLink, findElement.WebElement(driver, By.TagName("iframe"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title2, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title3, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(5)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para6, commonHelpers.GetTextByJS(driver, "em"), "The text content on web page is not as expected.");

      if (browserType == "Desktop")
      {
        //Able to toggle between pages.
        findElement.WebElement(driver, By.Id("lnk-Independent Living"), 3).Click();
        findElement.Wait(driver, By.Id("genvid"), 5);
        Assert.AreEqual(independentLivingURL, driver.Url, "The Indepedent Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Independent Supportive Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);
        Assert.AreEqual(independentSupportiveLivingURL, driver.Url, "The Independent Supportive Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Memory Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);
        Assert.AreEqual(memoryCareURL, driver.Url, "The Memory Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);
        Assert.AreEqual(longTermCareURL, driver.Url, "The Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Retirement versus Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);
        Assert.AreEqual(retirementVsLongTermCareURL, driver.Url, "The Retirement Vs Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);
      }
    }

    [TestMethod, Description("Test Case 3.2.5")]
    public void VerifyMemoryCare()
    {
      #region Variables
      string heading = "Memory Care";
      string youTubeVideoLink = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "What is Memory Care?";
      string title2 = "What is Chartwell’s Memory Living program?";
      string title3 = "How does my loved one move into a retirement community that offers Memory Care or Memory Living?";
      string para1 = "Chartwell’s memory care services are offered to seniors living with dementia or Alzheimer’s. Caring staff are trained on how to support individuals living with cognitive impairment.";
      string para2 = "Chartwell retirement residences offering memory care services have dedicated, secure neighbourhoods or floors with their own dining rooms and activity lounges for added security and peace of mind.";
      string para3 = "Chartwell also offers a unique Memory Living program in select locations that focuses on the social impact of dementia and offers seniors living with cognitive impairment a safe, supportive and independent environment with an emphasis on activities of daily living. Trained and dedicated staff build strong relationships with residents, as well as engage with and support their family members. Residents also benefit from specially-designed lifestyle activities and dining meant to help each individual lead a good day, every day.";
      string para4 = "Though Chartwell offers memory care services in many of our residences, only a few of our retirement communities across Canada have dedicated Memory Living Neighbourhoods, featuring secure outdoor space, family-style dining and resources like Memory Living Managers and Dementia Counsellors for families. We also have a number of new Memory Living Neighbourhoods currently under development.";
      string para5 = "To learn more about Chartwell’s Memory Living program, download our brochure.";
      string para6 = "For residents of Ontario, Quebec and British Columbia, you are welcome to inquire about availability at the Chartwell retirement community you are interested in for your loved one. When you visit, staff will conduct an assessment to ensure memory care or memory living will provide your loved one with the right amount of support they need and may recommend another lifestyle option such as long term care if required.";
      string para7 = "For residents of Alberta, Chartwell offers private-pay Memory Living Neighbourhoods in a few of our retirement communities. You are welcome to directly inquire about the availability in these Neighbourhoods at the retirement residence you are interested in.";
      string para8 = "Additionally, some of our residences in Alberta offer partially-funded memory care services coordinated by Alberta Health Services. To contact them about moving your loved one into a memory care suite, call HEALTHLink Alberta toll-free at 1-866-408-5465.";
      string para9 = "To find the residence nearest you or your loved one, or to learn more about memory care or memory living, call 1-855-461-0685 today to speak with a Chartwell representative who can help.";

      string independeSupportiveLivingSrc = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string assistedLivingSrc = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string memoryCareSrc = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string longTermCareSrc = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string retirementVsLongTermCareSrc = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";

      string independentLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-living";
      string independentSupportiveLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-supportive-living";
      string assistedLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/assisted-living";
      string longTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/long-term-care";
      string retirementVsLongTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/retirement-versus-long-term-care";
      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.MemoryCare, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeVideoLink, findElement.WebElement(driver, By.TagName("iframe"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title2, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title3, commonHelpers.GetTextByJS(driver, ".moveUp h3:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(5)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para6, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(6)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para7, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(7)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para8, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(8)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para9, commonHelpers.GetTextsByJS(driver, "return document.querySelectorAll('em')[1].innerText"), "The text content on web page is not as expected.");

      if (browserType == "Desktop")
      {
        //Able to toggle between pages.
        findElement.WebElement(driver, By.Id("lnk-Independent Living"), 3).Click();
        findElement.Wait(driver, By.Id("genvid"), 5);
        Assert.AreEqual(independentLivingURL, driver.Url, "The Indepedent Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Independent Supportive Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);
        Assert.AreEqual(independentSupportiveLivingURL, driver.Url, "The Independent Supportive Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Assisted Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);
        Assert.AreEqual(assistedLivingURL, driver.Url, "The Assisted Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);
        Assert.AreEqual(longTermCareURL, driver.Url, "The Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Retirement versus Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);
        Assert.AreEqual(retirementVsLongTermCareURL, driver.Url, "The Retirement Vs Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);
      }
    }

    [TestMethod, Description("Test Case 3.2.6")]
    public void VerifyLongTermCare()
    {
      #region Variables
      string heading = "Long Term Care";
      string youTubeVideoLink = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "What is Long Term Care?";
      string subTitle1 = "Ontario";
      string subTitle2 = "Quebec";
      string subTitle3 = "British Columbia";
      string para1 = "Long term care residences—historically also referred to as nursing homes—are licensed and authorized as government-regulated and funded homes. They are designed for individuals who require the availability of 24-hour nursing care or advanced dementia support within a secure setting.";
      string para2 = "Depending on the availability of the Long Term Care residence you are interested in for your loved one, you may have the option of private, semi-private or ward room accommodation.";
      string para3 = "For residents of Ontario, all applications to Long Term Care homes are coordinated by individual community Local Health Integration Networks (LHIN), who plan, integrate and fund local health care. There are 14 LHINs in Ontario. To locate the LHIN in your region, please visit their website.";
      string para4 = "Known in Quebec as Centre d'hébergement de soins de longue durée (CHSLD), government-funded and private long term care in the province are available options. Private CHSLDs offer private and semi-private rooms and provide a housing alternative for seniors, given long waiting lists for accommodation within the public health and social services network.";
      string para5 = "Chartwell does not currently operate any CHSLDs in the province; however, many of our retirement residences maintain strong relationships with local CHSLDs in both the private and public health network, should any of our residents eventually require long term care services.";
      string para6 = "Also known as Residential Care, Chartwell offers both government-funded and private pay long term care in Nanaimo, Langley, Surrey and Burnaby. Applicable Regional Health Authorities in those locations coordinate admission and will perform an assessment to determine eligibility. Please reach out to your local health authority for more information.";
      string para7 = "To find the residence nearest you or your loved one, or to learn more about long term care, call 1-855-461-0685 today to speak with a Chartwell representative who can help.";

      string independeSupportiveLivingSrc = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string assistedLivingSrc = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string memoryCareSrc = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string longTermCareSrc = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string retirementVsLongTermCareSrc = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";

      string independentLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-living";
      string independentSupportiveLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-supportive-living";
      string assistedLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/assisted-living";
      string memoryCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/memory-care";
      string retirementVsLongTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/retirement-versus-long-term-care";
      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.LongTermCare, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeVideoLink, findElement.WebElement(driver, By.TagName("iframe"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".moveUp h3"), "The text content on web page is not as expected.");
      Assert.AreEqual(subTitle1, commonHelpers.GetTextByJS(driver, ".moveUp h4:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(subTitle2, commonHelpers.GetTextByJS(driver, ".moveUp h4:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(subTitle3, commonHelpers.GetTextByJS(driver, ".moveUp h4:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(5)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para6, commonHelpers.GetTextByJS(driver, ".moveUp p:nth-of-type(6)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para7, commonHelpers.GetTextByJS(driver, "em"), "The text content on web page is not as expected.");

      if (browserType == "Desktop")
      {
        //Able to toggle between pages.
        findElement.WebElement(driver, By.Id("lnk-Independent Living"), 3).Click();
        findElement.Wait(driver, By.Id("genvid"), 5);
        Assert.AreEqual(independentLivingURL, driver.Url, "The Indepedent Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Independent Supportive Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);
        Assert.AreEqual(independentSupportiveLivingURL, driver.Url, "The Independent Supportive Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Assisted Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);
        Assert.AreEqual(assistedLivingURL, driver.Url, "The Assisted Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Memory Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);
        Assert.AreEqual(memoryCareURL, driver.Url, "The Memory Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Retirement versus Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);
        Assert.AreEqual(retirementVsLongTermCareURL, driver.Url, "The Retirement Vs Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);
      }
    }

    [TestMethod, Description("Test Case 3.2.7")]
    public void VerifyRetirementVersusLongTermCare()
    {
      #region Variables
      string heading = "What is the difference between a retirement community and a long term care residence?";
      string youTubeVideoLink = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";
      string title1 = "Care Support";
      string title2 = "Security";
      string title3 = "Private Pay vs. Funded Care";
      string title4 = "Move-in Process";
      string para1 = "Chartwell owns and operates over 200 residences across the country, which includes a combination of retirement residences and long term care homes. If you are trying to determine what type of residence is best suited for your lifestyle, or that of a loved one, here are some key points to consider:";
      string para2 = "A retirement residence is best suited for individuals who still consider themselves active and independent, but may also desire some help with things like meals, housekeeping and some personal support services like medication administration or assistance with their daily routine.";
      string para3 = "One of the main differentiating factors of a long term care residence is that the older adults who call them home usually require 24-hour nursing support to help manage complex medical needs or advanced stages of dementia and Alzheimer’s.";
      string para4 = "In a retirement residence, older adults can come and go as they please, though they benefit from 24-hour security and emergency response should they ever require assistance.";
      string para5 = "In long term care, many residences provide secure environments in order to keep seniors living with dementia safe when not accompanied outside of the residence by family or staff.";
      string para6 = "Retirement residences are private-pay—meaning residents are responsible for paying for their monthly rent.";
      string para7 = "The majority of the long term care residences that Chartwell owns and operates are partially funded by provincial governments—meaning the residents living in them have a portion of their monthly rent subsidized by the government.";
      string para8 = "To move into a Chartwell retirement residence, individuals are welcome to inquire about availability at the retirement community they are interested in. A Health & Wellness Manager will meet with the individual inquiring about moving in to ensure the lifestyle option they have selected (i.e. independent living, independent supportive living, or assisted living) will adequately meet their unique needs and preferences.";
      string para9 = "Admission into long term care is coordinated by local health authorities in each province, not directly by Chartwell long term care homes. Individuals trying to move a loved one into a long term care residence may find that once they engage with their local health authority, they may have to be put on a waiting list until a suite becomes available.";
      string para10 = "Want to learn more about the lifestyle options offered in Chartwell retirement and long term care residences? Visit our Exploring Your Options page.";

      string independeSupportiveLivingSrc = "https://www.youtube.com/embed/TzeuGivaAF0?rel=0&iv_load_policy=3&enablejsapi=1";
      string assistedLivingSrc = "https://www.youtube.com/embed/PZ3zOcXwFtI?rel=0&iv_load_policy=3&enablejsapi=1";
      string memoryCareSrc = "https://www.youtube.com/embed/hgoemHjoqj4?rel=0&iv_load_policy=3&enablejsapi=1";
      string longTermCareSrc = "https://www.youtube.com/embed/bRXNnpavtTQ?rel=0&iv_load_policy=3&enablejsapi=1";
      string retirementVsLongTermCareSrc = "https://www.youtube.com/embed/pi8uouPx0Ms?rel=0&iv_load_policy=3&enablejsapi=1";

      string independentLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-living";
      string independentSupportiveLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/independent-supportive-living";
      string assistedLivingURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/assisted-living";
      string memoryCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/memory-care";
      string longTermCareURL = "https://chartwell.com/en/help-me-choose/exploring-your-options/long-term-care";

      #endregion

      //Click on menu Independent Living.
      commonHelpers.ClickMenuInHelpMeChoose(driver, browserType, CommonHelpers.HelpMeChooseMenu.RetirementVersusLongTermCare, 5);

      //Verify title displays.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.CssSelector("h1"), 5).Text.Trim(), "The title does not display.");

      //Verify You Tube video displays.
      Assert.AreEqual(youTubeVideoLink, findElement.WebElement(driver, By.TagName("iframe"), 5).GetAttribute("src"), "The You Tube video does not display.");

      //Verify Text Contents.
      Assert.AreEqual(title1, commonHelpers.GetTextByJS(driver, ".col-md-12 h3:nth-of-type(1)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title2, commonHelpers.GetTextByJS(driver, ".col-md-12 h3:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title3, commonHelpers.GetTextByJS(driver, ".col-md-12 h3:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(title4, commonHelpers.GetTextByJS(driver, ".col-md-12 h3:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para1, commonHelpers.GetTextsByJS(driver, "return document.querySelectorAll('.col-md-12 p')[1].innerText"), "The text content on web page is not as expected.");
      Assert.AreEqual(para2, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(2)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para3, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(3)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para4, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(4)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para5, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(5)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para6, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(6)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para7, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(7)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para8, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(8)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para9, commonHelpers.GetTextByJS(driver, ".col-md-12 p:nth-of-type(9)"), "The text content on web page is not as expected.");
      Assert.AreEqual(para10, commonHelpers.GetTextByJS(driver, "em"), "The text content on web page is not as expected.");

      if (browserType == "Desktop")
      {
        //Able to toggle between pages.
        findElement.WebElement(driver, By.Id("lnk-Independent Living"), 3).Click();
        findElement.Wait(driver, By.Id("genvid"), 5);
        Assert.AreEqual(independentLivingURL, driver.Url, "The Indepedent Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Independent Supportive Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + independeSupportiveLivingSrc + "']"), 5);
        Assert.AreEqual(independentSupportiveLivingURL, driver.Url, "The Independent Supportive Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Assisted Living"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + assistedLivingSrc + "']"), 5);
        Assert.AreEqual(assistedLivingURL, driver.Url, "The Assisted Living page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Memory Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + memoryCareSrc + "']"), 5);
        Assert.AreEqual(memoryCareURL, driver.Url, "The Memory Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);

        findElement.WebElement(driver, By.Id("lnk-Long Term Care"), 3).Click();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + longTermCareSrc + "']"), 5);
        Assert.AreEqual(longTermCareURL, driver.Url, "The Long Term Care page does not open.");
        driver.Navigate().Back();
        findElement.Wait(driver, By.CssSelector("iframe[src='" + retirementVsLongTermCareSrc + "']"), 5);
      }
    }
    #endregion

    #region Our Services
    [TestMethod, Description("Test Case 3.3.1")]
    public void VerifyDiningExperience()
    {
      //Click on menu Dining Experience under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.DiningExperience, 5);

      //verify slideshow exists
      Assert.IsTrue(driver.FindElements(By.CssSelector("#gallery .ug-slide-wrapper")).Count > 0, "Slideshow does not have slides");

      //Verify Accordion Functions.
      bool isHighQualityFoodOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.HighQualityFood, 5);
      bool isDiverseMenuChoiceOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.DiverseMenuChoice, 5);
      bool isWarmAndProfessionalServiceOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WarmAndProfessionalService, 5);
      bool isWelcomeSocialAmbianceOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WelcomeSocialAmbiance, 5);
      bool isUniqueAmenitiesAndProgramsOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.UniqueAmenitiesAndPrograms, 5);

      commonHelpers.OpenOrCloseAccordioPanelOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.HighQualityFood, !isHighQualityFoodOpen, 5);
      bool isHighQualityFoodOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.HighQualityFood, 5);
      Assert.AreNotEqual(isHighQualityFoodOpen, isHighQualityFoodOpenNow, "The panel 'High-Quality Food' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.DiverseMenuChoice, !isDiverseMenuChoiceOpen, 5);
      bool isDiverseMenuChoiceOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.DiverseMenuChoice, 5);
      Assert.AreNotEqual(isDiverseMenuChoiceOpen, isDiverseMenuChoiceOpenNow, "The panel 'Diverse Menu Choice' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WarmAndProfessionalService, !isWarmAndProfessionalServiceOpen, 5);
      bool isWarmAndProfessionalServiceOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WarmAndProfessionalService, 5);
      Assert.AreNotEqual(isWarmAndProfessionalServiceOpen, isWarmAndProfessionalServiceOpenNow, "The panel 'Warm And Professional Service' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WelcomeSocialAmbiance, !isWelcomeSocialAmbianceOpen, 5);
      bool isWelcomeSocialAmbianceOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.WelcomeSocialAmbiance, 5);
      Assert.AreNotEqual(isWelcomeSocialAmbianceOpen, isWelcomeSocialAmbianceOpenNow, "The panel 'Welcome Social Ambiance' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.UniqueAmenitiesAndPrograms, !isUniqueAmenitiesAndProgramsOpen, 5);
      bool isUniqueAmenitiesAndProgramsOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(driver, CommonHelpers.DiningExperienceAccordionPanels.UniqueAmenitiesAndPrograms, 5);
      Assert.AreNotEqual(isUniqueAmenitiesAndProgramsOpen, isUniqueAmenitiesAndProgramsOpenNow, "The panel 'Unique Amenities & Programs' is not clicked.");
    }

    [TestMethod, Description("Test Case 3.3.2")]
    public void VerifyActiveLiving()
    {
      //Click on menu Active Living under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.ActiveLiving, 5);

      //Verify Title displays.
      Assert.AreEqual("Active Living", findElement.WebElement(driver, By.CssSelector("h1"), 10).Text.Trim(), "The title for Active Living does not display.");

      //Verify You tube video displays.
      string cssSelector = "iframe[src='https://www.youtube.com/embed/17RveT02kxQ?rel=0&iv_load_policy=3&enablejsapi=1\']";
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector(cssSelector), 5));

      //Click on Live Now link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"/en/our-services/active-living/livenow\"]");
      findElement.Wait(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/Oy7oKJPTrzg?rel=0&iv_load_policy=3&enablejsapi=1']"), 5);
      //Verify Live Now page opens.
      Assert.IsTrue(driver.Url == "https://chartwell.com/en/our-services/active-living/livenow", "Live Now page does not open.");
      driver.Navigate().Back();

      findElement.Wait(driver, By.CssSelector(cssSelector), 5);

      //Click on Recreation link.
      commonHelpers.ClickElementByJS(driver, "a[href=\"/en/our-services/active-living/recreation\"]");
      findElement.Wait(driver, By.CssSelector("h1"), 5);
      //Verify Recreation page opens.
      Assert.IsTrue(driver.Url == "https://chartwell.com/en/our-services/active-living/recreation", "Recreation page does not open.");
    }

    [TestMethod, Description("Test Case 3.3.3")]
    public void VerifyLiveNow()
    {
      #region Variables
      string youTubeLinkSelector = "iframe[src='https://www.youtube.com/embed/Oy7oKJPTrzg?rel=0&iv_load_policy=3&enablejsapi=1']";
      #endregion
      //Click on menu Live Now under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.LiveNow, 5);

      //Verify Title displays.
      Assert.AreEqual("LiveNow", findElement.WebElement(driver, By.CssSelector("h1"), 10).Text.Trim(), "The title for LiveNow does not display.");

      //Verify You Tube video displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector(youTubeLinkSelector), 5));

      //Verify Accordion Functions.
      bool isSocialOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Social, 5);
      bool isVocationalOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, 5);
      bool isSpiritualOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Spiritual, 5);
      bool isEmotionalOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Emotional, 5);
      bool isIntellectualOpen = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, 5);

      commonHelpers.OpenOrCloseAccordioPanelOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Social, !isSocialOpen, 5);
      bool isSocialOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Social, 5);
      Assert.AreNotEqual(isSocialOpen, isSocialOpenNow, "The panel 'Social' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, !isVocationalOpen, 5);
      bool isVocationalOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, 5);
      Assert.AreNotEqual(isVocationalOpen, isVocationalOpenNow, "The panel 'Vocational' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Spiritual, !isSpiritualOpen, 5);
      bool isSpiritualOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Spiritual, 5);
      Assert.AreNotEqual(isSpiritualOpen, isSpiritualOpenNow, "The panel 'Spiritual' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Emotional, !isEmotionalOpen, 5);
      bool isEmotionalOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Emotional, 5);
      Assert.AreNotEqual(isEmotionalOpen, isEmotionalOpenNow, "The panel 'Emotional' is not clicked.");

      commonHelpers.OpenOrCloseAccordioPanelOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, !isIntellectualOpen, 5);
      bool isIntellectualOpenNow = commonHelpers.VerifyIfAccordionPanelIsOpenOnLiveNowPage(driver, CommonHelpers.LiveNowAccordionPanels.Vocational, 5);
      Assert.AreNotEqual(isIntellectualOpen, isIntellectualOpenNow, "The panel 'Vocational' is not clicked.");
    }

    [TestMethod, Description("Test Case 3.3.4")]
    public void VerifyRecreation()
    {
      #region Variables
      string para1 = "At Chartwell, we believe that keeping active and socially engaged is the key to a happy, healthy and fulfilled life.";
      string para2 = "A variety of recreational programs and activities are offered in all Chartwell residences to meet the needs of everyone. Programs take place on-site and in the community, and are created with varying interests and abilities in mind. Residents are encouraged to lead active and engaged lives while enjoying an inclusive and social atmosphere where they can truly feel fulfilled. From interest clubs to excursions, dance classes to celebrations, there is always an array of programs and activities for residents to choose from.";
      string para3 = "We believe that physical activity can help improve your health, mental acuity and independence, and it can also help to enhance balance and prevent falls. Activities that offer a variety of endurance, strength-building and flexibility benefits should be top of mind for older adults, and at Chartwell, the options are limitless. In addition to our signature Rhythm ’n’ Moves program, our residences offer a wide range of physical activities to help keep you moving well into your retirement years. Walking clubs, yoga, line dancing, tai chi and gardening are just a few examples of physical activities enjoyed by our residents. Don’t feel like heading outside? You can take part in a game of tennis on the residence’s Nintendo Wii console to help get your heart rate up!";
      string para4 = "Research has shown that social interaction offers numerous benefits, especially for older adults. Staying socially active and maintaining close relationships can help contribute to good physical, emotional and cognitive health. Chartwell residences offer ample opportunities for residents to meet new friends and maintain close relationships. A variety of activities, including Game Nights, Bistro Socials, Book Clubs, Movies and Community Outings, provide fun ways for residents to meet one another and form friendships in a relaxed and enjoyable atmosphere. Residences are encouraged to host a variety of social events and activities that foster engagement between residents with varying interests and backgrounds.";
      #endregion
      //Click on menu Recreation under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.Recreation, 5);

      //Verify Title displays.
      Assert.AreEqual("Recreation", findElement.WebElement(driver, By.CssSelector("h1"), 10).Text.Trim(), "The title for Recreation does not display.");

      //Verify images display.
      commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/our-services/recreation/chartwell-recreation-social-coffee.jpg']"));
      commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/our-services/recreation/chartwell-recreation-physical-activity.jpg']"));
      commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/our-services/recreation/chartwell-recreation-social-activities.jpg']"));

      //Verify texts diplay.
      Assert.AreEqual(para1, findElement.WebElements(driver, By.CssSelector("#mainRow p"))[0].Text, "Not All texts displayed.");
      Assert.AreEqual(para2, findElement.WebElements(driver, By.CssSelector("#mainRow p"))[1].Text, "Not All texts displayed.");
      Assert.AreEqual(para3, findElement.WebElements(driver, By.CssSelector("#mainRow p"))[2].Text, "Not All texts displayed.");
      Assert.AreEqual(para4, findElement.WebElements(driver, By.CssSelector("#mainRow p"))[3].Text, "Not All texts displayed.");

      //Verify clicking on button 'Download Sample Activity Calendar' opens new tab.
      commonHelpers.ClickElementByJS(driver, "a[href=\"/-/media/Files/activity-calendars/chartwell-retirement-residence-activity-calendar-sample.pdf\"]");

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/activity-calendars/chartwell-retirement-residence-activity-calendar-sample.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.3.5")]
    public void VerifyBenefitsOfSocialization()
    {
      //Click on menu 'Benefits of Socialization' under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.BenefitsOfSocialization, 5);

      //Verify title
      Assert.AreEqual("Benefits Of Socialization", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //Verify image.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src='/-/media/Images/icons/benefits-socialization/think-about.png']")), "The infographic does not display.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits-socialization/1325_Benefits-Socialization_Download_But_ENG.jpg'")), "The thumbnail image does not display.");

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits-socialization/1325_Benefits-Socialization_Download_But_ENG.jpg'"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/infographics/chartwell-benefits-of-socialization-infographic.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.3.6")]
    public void VerifyWellnessAndSupportServices()
    {
      #region Variables
      string heading = "Wellness and Support Services";
      string para1 = "At Chartwell, we strive to provide a lifestyle that allows you or a loved one to lead a healthy and happy retirement in a great place to live. We offer a number of valuable services in our retirement residences across the country that can contribute to your overall wellness and comfort. Nutritious and delicious meals, active living and social opportunities, and conveniences like housekeeping and 24-security are all ingredients for a healthy retirement, and can allow you to spend your energy on what brings you contentment in life.";
      string para2 = "For added peace of mind, Chartwell offers a selection of optional support services meant to maximize independence and improve or maintain quality of life. If you will be benefitting from any of our support services, prior to move in a Health & Wellness Manager will meet with you to assess what you may require to live life comfortably and with confidence. Our caring and trained staff are there to partner with you or a loved one to provide what you want and need support with, giving you power and choice over your personalized care plan.";
      string para3 = "*Fee-for-service personal support is offered to help you or a loved one manage your needs in the comfort and privacy of a Chartwell residence. Please inquire which services are available in the residence you are considering, in order to evaluate your support needs.";
      string para4 = "**Ancillary services may be provided by the residence as part of our Assisted Living program, or by private vendors as a fee-for-service option. The availability of these services may vary depending on the residence. Please inquire which services are available in the residence you are considering, in order to evaluate your support needs.";
      string title1 = "Wellness services provided in our retirement residences include:";
      string title2 = "Fee-for-service personal support can include*:";
      string title3 = "Ancillary services offered at our residences may include**:";
      string bulletPoint1 = "24-hour emergency response";
      string bulletPoint2 = "Regular observation and supervision of well-being";
      string bulletPoint3 = "Access to exercise and recreational programming";
      string bulletPoint4 = "Medication administration and supervision";
      string bulletPoint5 = "Assistance with your daily routine, including activities like dressing, hygiene and bathing";
      string bulletPoint6 = "Escort to activities and meals";
      string bulletPoint7 = "Weight and blood pressure tracking";
      string bulletPoint8 = "Companionship";
      string bulletPoint9 = "Other care services as agreed";
      string bulletPoint10 = "Reflexologist";
      string bulletPoint11 = "Podiatrist";
      string bulletPoint12 = "Manicurist/Pedicurist";
      string bulletPoint13 = "Physiotherapist";
      string bulletPoint14 = "Beauty and Barber Services";
      string bulletPoint15 = "Pharmacy";
      #endregion
      //Click on menu 'Wellness And Support Services' under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.WellnessAndSupportServices, 5);

      //Verify title.
      Assert.AreEqual(heading, findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The heading does not display.");

      //Verify texts.
      IList<IWebElement> paragraphsAndTitles = findElement.WebElements(driver, By.CssSelector("#mainRow p"));
      IList<IWebElement> bulletPoints = findElement.WebElements(driver, By.CssSelector(".staticPage li"));
      Assert.AreEqual(para1, paragraphsAndTitles[1].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(para2, paragraphsAndTitles[2].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(title1, paragraphsAndTitles[3].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(title2, paragraphsAndTitles[4].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(title3, paragraphsAndTitles[5].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(para3, paragraphsAndTitles[6].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(para4, paragraphsAndTitles[7].Text.Trim(), "Not all texts are displayed.");
      Assert.AreEqual(bulletPoint1, bulletPoints[0].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint2, bulletPoints[1].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint3, bulletPoints[2].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint4, bulletPoints[3].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint5, bulletPoints[4].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint6, bulletPoints[5].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint7, bulletPoints[6].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint8, bulletPoints[7].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint9, bulletPoints[8].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint10, bulletPoints[9].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint11, bulletPoints[10].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint12, bulletPoints[11].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint13, bulletPoints[12].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint14, bulletPoints[13].Text.Trim(), "Not all bullet points are displayed.");
      Assert.AreEqual(bulletPoint15, bulletPoints[14].Text.Trim(), "Not all bullet points are displayed.");
    }

    [TestMethod, Description("Test Case 3.3.7")]
    public void VerifyHealthAndWellness()
    {
      //Click on menu 'Health And Wellness' under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.HealthAndWellness, 5);

      //Verify title
      Assert.AreEqual("The Benefits of Active Living", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //Verify image.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src='/-/media/Images/icons/active-living/think-about.png']")), "The infographic does not display.");

      if (browserType == "Desktop")
      {
        //Verify the header of Contact Form.
        Assert.AreEqual("Questions about living at Chartwell?", findElement.WebElements(driver, By.CssSelector("#chartwellContactForm h3"))[0].Text, "The header of Contact Form does not exists.");

        //Verify Phone number on Contact Form.
        Assert.AreEqual("1 855 461 0685", findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed on Contact Form is not as expected.");
      }

      //Verify the thumbnail displays.
      Assert.IsTrue(commonHelpers.VerifyElementPresent(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits-active-living/1325HealthWellnessDownloadButENG.jpg'")), "The thumbnail image does not display.");

      //Click on thumbnail.
      findElement.WebElement(driver, By.CssSelector("img[src = '/-/media/Images/icons/benefits-active-living/1325HealthWellnessDownloadButENG.jpg'"), 5).Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver newDriver = driver.SwitchTo().Window(tabs[1]);

      //Verify the pdf opens.
      Assert.AreEqual("https://chartwell.com/-/media/Files/infographics/chartwell-health-and-wellness-infographic.pdf", newDriver.Url, "The pdf does not open.");
    }

    [TestMethod, Description("Test Case 3.3.8")]
    public void VerifySuiteTypesAndAmenities()
    {
      //Click on menu 'Suite and Accommodation Types' under Our Services.
      commonHelpers.ClickMenuInOurServices(driver, browserType, CommonHelpers.OurServicesMenu.SuiteTypesAndAmenities, 5);

      //Verify title
      Assert.AreEqual("Suite and Accommodation Types", findElement.WebElement(driver, By.TagName("h1"), 5).Text, "The title does not display.");

      //verify slideshows exist
      Assert.IsTrue(driver.FindElements(By.CssSelector(".ug-gallery-wrapper.ug-theme-slider")).Count == 4, "There are no slideshows on Suites Types and Amenities");
    }
    #endregion

    #region Learn
    [TestMethod, Description("Test Case 3.4.1")]
    public void VerifyStepByStepResources()
    {
      #region Variables
      string hrefBeginningYourResearch = "https://chartwell.com/en/learn/step-by-step-resources/beginning-your-research";
      string hrefHavingTheConversation = "https://chartwell.com/en/learn/step-by-step-resources/having-the-conversation";
      string hrefFindingTheRightResidence = "https://chartwell.com/en/learn/step-by-step-resources/finding-the-right-residence";
      string hrefPlanningYourMove = "https://chartwell.com/en/learn/step-by-step-resources/planning-your-move";
      #endregion

      //Click on menu Step-By-Step Resources under Learn.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.StepByStepResources, 5);

      //Verify menu titles that links to other pages.
      Assert.AreEqual(hrefBeginningYourResearch, findElement.WebElement(driver, By.Id("tab-beginning-your-research"), 5).GetAttribute("href"));
      Assert.AreEqual(hrefHavingTheConversation, findElement.WebElement(driver, By.Id("tab-having-the-conversation"), 5).GetAttribute("href"));
      Assert.AreEqual(hrefFindingTheRightResidence, findElement.WebElement(driver, By.Id("tab-finding-the-right-residence"), 5).GetAttribute("href"));
      Assert.AreEqual(hrefPlanningYourMove, findElement.WebElement(driver, By.Id("tab-planning-your-move"), 5).GetAttribute("href"));
    }

    [TestMethod, Description("Test Case 3.4.2")]
    public void VerifyExpertAdvice()
    {
      //Click on menu Expert Advice under Learn.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.ExpertAdvice, 5);
      findElement.Wait(driver, By.Id("LeadershipTabs"), 10);

      //Verify title 'Family Conversations' is displayed.
      Assert.AreEqual("Family Conversations", findElement.WebElement(driver, By.CssSelector("#Conversations h2"), 5).Text.Trim(), "The title Family Conversations does not display.");

      if (browserType == "Desktop")
      {
        //Click on Finances tab.    
        findElement.WebElement(driver, By.Id("lnkfinance_en"), 5).Click();
        findElement.Wait(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/learn/Finances-with-Kelley-Keehn.jpg']"), 5);

        //Verify Finances tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("lnkfinance_en"), 5).GetAttribute("aria-selected")), "Finances tab is not opened.");

        //Click on Chartwell's Experts tab
        findElement.WebElement(driver, By.Id("lnkexpert_en"), 5).Click();
        findElement.Wait(driver, By.CssSelector("a[href='https://chartwell.com/blog/2017/02/ask-our-experts-caring-for-a-spouse-during-your-retirement-years']"), 5);

        //Verify Chartwell's Experts tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("lnkexpert_en"), 5).GetAttribute("aria-selected")), "Chartwell's Expert tab is not opened.");

        //Click on Family Conversations tab.
        findElement.WebElement(driver, By.Id("lnkfamily_en"), 5).Click();
        findElement.Wait(driver, By.CssSelector("a[href='/learn/expert-advice/essential-conversations-with-dr-amy']"), 5);

        //Verify Family Conversations tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("lnkfamily_en"), 5).GetAttribute("aria-selected")), "Family Conversations tab is not opened.");
      }

      else
      {
        //Click on Finances tab.
        commonHelpers.ClickElementByJS(driver, "#Finances button");
        findElement.Wait(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/learn/Finances-with-Kelley-Keehn.jpg']"), 5);

        //Verify Finances tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.CssSelector("#Finances button"), 5).GetAttribute("aria-expanded")), "Finances tab is not opened.");

        //Click on Chartwell's Experts tab..
        commonHelpers.ClickElementByJS(driver, "#Experts button");
        findElement.Wait(driver, By.CssSelector("a[href='https://chartwell.com/blog/2017/02/ask-our-experts-caring-for-a-spouse-during-your-retirement-years']"), 5);

        //Verify Chartwell's Expert tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.CssSelector("#Experts button"), 5).GetAttribute("aria-expanded")), "Chartwell's Expert tab is not opened.");

        //Click on Family Conversations tab.
        commonHelpers.ClickElementByJS(driver, "#Conversations button");
        findElement.Wait(driver, By.CssSelector("a[href='/learn/expert-advice/essential-conversations-with-dr-amy']"), 5);

        //Verify Family Conversations tab is opened.
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.CssSelector("#Conversations button"), 5).GetAttribute("aria-expanded")), "Family Conversations tab is not opened.");
      }

      //Click on library of videos and blogs.
      commonHelpers.ClickElementByJS(driver, "a[href=\"/learn/expert-advice/essential-conversations-with-dr-amy\"]");
      findElement.Wait(driver, By.CssSelector("div[data-src='https://www.youtube.com/embed/b9cIUaw9RjI']"), 10);

      //Verify Dr Amy's page is opened.
      Assert.AreEqual("https://chartwell.com/learn/expert-advice/essential-conversations-with-dr-amy", driver.Url, "Dr Amy's page does not open.");
    }

    [TestMethod, Description("Test Case 3.4.4")]
    public void VerifyBlogLinks()
    {
      string[] URLs = new string[2] { "https://chartwell.com/en/blog", "https://chartwell.com/fr/blogue" };
      string[] languages = new string[2] { "en", "fr" };
      string[] buttonIds = new string[2] { "MainNav-Blog", "MainNav-Blogue" };

      int x = 0;
      foreach (string lang in languages)
      {
        List<string> tabs = new List<string>(driver.WindowHandles);
        IWebDriver driver1;

        driver.FindElements(By.CssSelector(".langOption." + lang))[0].Click();
        findElement.Wait(driver, By.Id(buttonIds[x]), 10);

        driver.FindElement(By.Id(buttonIds[x])).Click();
        findElement.Wait(driver, By.Id(buttonIds[x]), 10);

        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);

        Assert.AreEqual(URLs[x], driver1.Url, "The " + lang + " blog link is incorrect ");
        x++;
      }
    }

    [TestMethod, Description("Test Case 3.4.5")]
    public void VerifyBrowseTopics()
    {
      #region Variables
      string title = "Topics";
      string supportingLovedOneGuidePdf = "https://chartwell.com/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf";
      string isItTimeQuestionnaire = "https://chartwell.com/getting-started/searching-for-a-loved-one/is-it-time";
      string supportingAgingParentArticle1 = "https://chartwell.com/blog/2016/01/a-checklist-of-important-questions-to-ask-your-aging-parent";
      string supportingAgingParentArticle2 = "https://chartwell.com/blog/2016/05/4-financial-questions-to-ask-your-aging-parent";
      string supportingAgingParentArticle3 = "https://chartwell.com/en/blog/2017/01/an-honest-conversation-exploring-retirement-living-with-a-loved-one";
      string supportingAgingParentArticle4 = "https://chartwell.com/blog/2016/08/telling-signs-it-may-be-time-to-consider-a-retirement-residence";
      string supportingAgingParentAskOurExperts5 = "https://chartwell.com/en/blog/2019/02/understanding-a-parents-difficult-reactions-to-a-big-life-transition";
      string supportingAgingParentAskOurExperts6 = "https://chartwell.com/en/blog/2018/10/essential-conversations-with-dr-amy-what-should-i-do-if-my-sibling-wont-help-me-care-for-our-parents";
      string supportingAgingParentAskOurExperts7 = "https://chartwell.com/blog/2015/03/how-do-i-start-the-conversation-about-retirement-living-with-my-loved-one";
      string supportingAgingParentAskOurExperts8 = "https://chartwell.com/blog/2015/11/ask-our-experts-when-is-the-right-time-for-retirement-living";
      string financeBudgetAssitant = "https://chartwell.com/learn/budget-assistant";
      string financeArticle1 = "https://chartwell.com/en/blog/2019/05/4-factors-to-consider-when-exploring-retirement-living-affordability";
      string financeArticle2 = "https://chartwell.com/en/blog/2019/05/exploring-and-planning-for-the-cost-of-retirement-living";
      string financeArticle3 = "https://chartwell.com/blog/2016/04/ask-our-residents-is-retirement-living-an-affordable-option";
      string financeAskOurExpert3 = "https://chartwell.com/blog/2016/04/ask-our-residents-is-retirement-living-an-affordable-option";
      string careAndServicesArticle1 = "https://chartwell.com/blog/2015/07/retirement-living-options-which-care-level-is-right-for-your-loved-one";
      string careAndServicesArticle2 = "https://chartwell.com/blog/2015/11/care-and-living-options-offered-by-chartwell-retirement-residences";
      string careAndServicesAskOurExpert1 = "https://chartwell.com/blog/2016/04/ask-our-experts-how-do-i-know-which-retirement-living-support-option-is-right-for-my-needs";
      string careAndServicesAskOurExpert2 = "https://chartwell.com/blog/2017/02/ask-our-experts-caring-for-a-spouse-during-your-retirement-years";
      string activeLivingAndHealthAndWellness6 = "https://chartwell.com/about-us/moments-that-matter";
      string activeLivingAndHealthAndWellnessArticle1 = "https://chartwell.com/en/blog/2019/02/for-seniors-friends-come-with-healthy-benefits";
      string activeLivingAndHealthAndWellnessArticle2 = "https://chartwell.com/en/blog/2019/04/aging-well-is-more-than-just-good-physical-health-for-seniors";
      string activeLivingAndHealthAndWellnessArticle3 = "https://chartwell.com/en/blog/2019/04/how-seniors-can-boost-their-health-span-for-a-more-vital-retirement";
      string activeLivingAndHealthAndWellnessArticle4 = "https://chartwell.com/en/blog/2018/09/why-a-positive-outlook-on-life-and-aging-is-good-for-your-health";
      string activeLivingAndHealthAndWellnessArticle5 = "https://chartwell.com/en/blog/2018/10/7-ways-to-promote-active-aging-and-healthy-longevity";
      string activeLivingAndHealthAndWellnessArticle6 = "https://chartwell.com/en/blog/2018/11/engaging-in-the-arts-boosts-seniors'-physical-and-emotional-health";
      string activeLivingAndHealthAndWellnessAskOurExpert1 = "https://chartwell.com/en/blog";
      string activeLivingAndHealthAndWellnessAskOurExpert3 = "https://chartwell.com/blog/2016/06/explore-chartwells-rhythm-n-moves-exercise-class-with-lifestyle-and-program-manager-tracey-mcdonald";
      string activeLivingAndHealthAndWellnessAskOurExpert4 = "https://chartwell.com/blog/2018/04/ask-our-experts-the-importance-of-a-social-life-in-our-retirement-years";
      string activeLivingAndHealthAndWellnessInfographics1 = "https://chartwell.com/-/media/Files/infographics/chartwell-benefits-of-socialization-infographic.pdf";
      string exploringRetirementLivingGuidePdf = "https://chartwell.com/-/media/Files/guides/self/chartwell-exploring-retirement-living-guide.pdf";
      string isItTimeQuestionnaireForHelpMeChoose = "https://chartwell.com/getting-started/searching-for-self/am-i-ready";
      string infographicsForHelpMeChoose = "https://chartwell.com/-/media/Files/infographics/chartwell-the-benefits-of-retirement-living-infographic.pdf";
      string helpMeChooseAskOurExperts1 = "https://chartwell.com/blog/";
      string helpMeChooseAskOurExperts2 = "https://chartwell.com/blog/2017/04/ask-our-experts-how-should-i-furnish-and-decorate-my-new-suite";
      string helpMeChooseAskOurExperts3 = "https://chartwell.com/blog/2016/02/ask-our-experts-how-do-i-know-if-im-ready-for-retirement-living";
      string helpMeChooseArticle1 = "https://chartwell.com/en/blog";
      string helpMeChooseArticle2 = "https://chartwell.com/blog/2016/05/what-to-look-for-while-taking-a-tour-of-a-retirement-residence";
      string helpMeChooseArticle3 = "https://chartwell.com/blog/2015/11/3-important-questions-to-ask-while-on-a-tour-of-a-retirement-residence";
      string helpMeChooseArticle4 = "https://chartwell.com/blog/2015/11/tips-for-finding-the-right-retirement-residence";
      string helpMeChooseArticle5 = "https://chartwell.com/blog/2015/07/retirement-living-options-which-care-level-is-right-for-your-loved-one";
      string helpMeChooseArticle6 = "https://chartwell.com/en/blog/2019/04/7-questions-to-determine-if-youre-retirement-residence-ready";
      string helpMeChooseArticle7 = "https://chartwell.com/en/blog/2019/03/Why-retirement-living-may-suit-you-better-than-receiving-support-at-home";
      string helpMeChooseArticle8 = "https://chartwell.com/en/blog/2018/09/three-healthy-reasons-to-consider-retirement-living";
      string helpMeChooseArticle9 = "https://chartwell.com/en/blog/2018/10/why-retirement-residence-living-can-be-a-healthy-choice-part-2";
      string helpMeChooseArticle10 = "https://chartwell.com/blog/2016/05/researching-retirement-living-heres-where-to-start";
      string helpMeChooseArticle11 = "https://chartwell.com/en/blog/2016/03/the-benefits-of-considering-a-retirement-lifestyle-before-experiencing-a-health-scare";
      string helpMeChooseArticle12 = "https://chartwell.com/blog/2016/11/the-benefits-of-an-independent-living-retirement-lifestyle";
      string helpMeChooseArticle13 = "https://chartwell.com/blog/2016/06/3-compelling-reasons-to-move-into-a-retirement-community";
      string helpMeChooseStaffStories1 = "https://youtu.be/VaDI5mSrllQ";
      string helpMeChooseStaffStories11 = "https://www.youtube.com/watch?v=VaDI5mSrllQ&feature=youtu.be";
      string helpMeChooseStaffStories111 = "https://m.youtube.com/watch?v=VaDI5mSrllQ&feature=youtu.be";
      string helpMeChooseStaffStories1111 = "https://m.youtube.com/watch?feature=youtu.be&v=VaDI5mSrllQ";
      string helpMeChooseStaffStories2 = "https://youtu.be/0jD-UJ70eO0";
      string helpMeChooseStaffStories21 = "https://www.youtube.com/watch?v=0jD-UJ70eO0&feature=youtu.be";
      string helpMeChooseStaffStories211 = "https://m.youtube.com/watch?v=0jD-UJ70eO0&feature=youtu.be";
      string helpMeChooseStaffStories2111 = "https://m.youtube.com/watch?feature=youtu.be&v=0jD-UJ70eO0";
      string helpMeChooseStaffStories3 = "https://youtu.be/ai94-doHqiY";
      string helpMeChooseStaffStories31 = "https://www.youtube.com/watch?v=ai94-doHqiY&feature=youtu.be";
      string helpMeChooseStaffStories311 = "https://m.youtube.com/watch?v=ai94-doHqiY&feature=youtu.be";
      string helpMeChooseStaffStories3111 = "https://m.youtube.com/watch?feature=youtu.be&v=ai94-doHqiY";
      #endregion
      //Click on menu Browse Topics under Learn.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.BrowseTopics, 5);
      findElement.Wait(driver, By.ClassName("topics"), 10);

      //Verify title is displayed.
      Assert.AreEqual(title, findElement.WebElement(driver, By.CssSelector("#mainRow h1"), 5).Text.Trim(), "The title does not display.");

      //- Accordions open where indicated
      //- Links to surveys function as expected
      //- PDF download open in another tab
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;

      #region Supporting Aging Parent
      commonHelpers.ClickOnLinkUnderSupportingAgingParent(driver, CommonHelpers.SupportingAgingParentLinks.SupportingALovedOneGuidePDF, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingLovedOneGuidePdf, driver1.Url, "The pdf for 'Supporting Loved One Guide PDF' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParent(driver, CommonHelpers.SupportingAgingParentLinks.IsItTimeQuestionnaire, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(isItTimeQuestionnaire, driver1.Url, "The questionnaire for 'Supporting Loved One Guide PDF' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentArticles(driver, CommonHelpers.SupportingAgingParentArticles.AChecklistOfImportantQuestionsToAskYourAgingParent, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentArticle1, driver1.Url, "The Article 1 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentArticles(driver, CommonHelpers.SupportingAgingParentArticles.FourFinancialQuestionsToAskYourAgingParent, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentArticle2, driver1.Url, "The Article 2 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentArticles(driver, CommonHelpers.SupportingAgingParentArticles.AnHonestConversationExploringRetirementLivingWithALovedOne, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentArticle3, driver1.Url, "The Article 3 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentArticles(driver, CommonHelpers.SupportingAgingParentArticles.TellingSignsItMayBeTimeToConsiderARetirementResidence, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentArticle4, driver1.Url, "The Article 4 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.VideoDrAmyGettingOnTheSamePageAsYourSiblings, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Experts link 1 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.VideoDrAmyTalkingToYourParentsAboutKeepingSociallyActive, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Experts link 2 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.VideoDrAmyCommunicatingWithYourSiblingsAboutYourParentsCareNeeds, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Experts link 3 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.VideoDrAmyUnderstandingThePerspectivesOfAParent, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Experts link 4 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.EssentialConversationsWithDrAmyUnderstandingAParentsDifficultReactionsToABigLifeTransition, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentAskOurExperts5, driver1.Url, "The Ask Our Experts link 5 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.EssentialConversationsWithDrAmyWhatDoIDoIfMySiblingWontHelpMeCareForOurAgingParents, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentAskOurExperts6, driver1.Url, "The Ask Our Experts link 6 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.HowDoIStartTheConversationAboutRetirementLivingWithMyLovedOne, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentAskOurExperts7, driver1.Url, "The Ask Our Experts link 7 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderSupportingAgingParentAskOurExperts(driver, CommonHelpers.SupportingAgingParentAskOurExperts.WhenIsTheRightTimeForRetirementLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(supportingAgingParentAskOurExperts8, driver1.Url, "The Ask Our Experts link 8 for 'Supporting Aging Parent' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);
      #endregion

      #region Finance
      commonHelpers.ClickOnLinkUnderFinance(driver, CommonHelpers.FinanceLinks.BudgetAssistant, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(financeBudgetAssitant, driver1.Url, "The questionnaire for 'Budget Assistant' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceArticle(driver, CommonHelpers.FinanceArticles.FourFactorsToConsiderWhenExploringRetirementLivingAffordability, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(financeArticle1, driver1.Url, "The Article 1 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceArticle(driver, CommonHelpers.FinanceArticles.ExploringAndPlanningForTheCostOfRetirementLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(financeArticle2, driver1.Url, "The Article 2 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceArticle(driver, CommonHelpers.FinanceArticles.IsRetirementLivingAnAffordablePption, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(financeArticle3, driver1.Url, "The Article 3 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceAskOurExperts(driver, CommonHelpers.FinanceAskOurExperts.VideoDrAmyTalkingToYourParentsAboutTheManagementOfTheirFinances, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Expert link 1 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceAskOurExperts(driver, CommonHelpers.FinanceAskOurExperts.VideoDrAmyGivingYourselfPermissionToLiveComfortablyInYourLaterYears, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Expert link 2 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderFinanceAskOurExperts(driver, CommonHelpers.FinanceAskOurExperts.AskOurResidentsIsRetirementLivingAnAffordableOption, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(financeAskOurExpert3, driver1.Url, "The Ask Our Expert link 3 for 'Finance' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);
      #endregion

      #region Care And Services
      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsIndependentLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 1 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsIndependentSupportiveLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 2 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsAssistedLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 3 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsMemoryCare, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 4 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsLongTermCare, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 5 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesExploringYourOptions(driver, CommonHelpers.CareAndServicesExploringYourOptions.VideoExploringYourOptionsWhatIsIndependentLiving, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Exploring Your Options link 6 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesArticles(driver, CommonHelpers.CareAndServicesArticles.RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == careAndServicesArticle1, "The Article link 1 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesArticles(driver, CommonHelpers.CareAndServicesArticles.CareAndLivingOptionsOfferedByChartwellRetirementResidences, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == careAndServicesArticle2, "The Article link 2 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesAskOurExperts(driver, CommonHelpers.CareAndServicesAskOurExperts.HowDoIknowWhichRetirementLivingSupportOptionsIsRightForMyNeeds, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == careAndServicesAskOurExpert1, "The Ask Our Experts link 1 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderCareAndServicesAskOurExperts(driver, CommonHelpers.CareAndServicesAskOurExperts.CaringForAnAgingSpouse, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == careAndServicesAskOurExpert2, "The Ask Our Experts link 2 for 'Care And Services' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);
      #endregion

      #region Active Living And Health & Wellness
      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.VideoRhythmAndMovesExerciseClassVideoEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The video link 1 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.VideoRhythmAndMovesVideoFromHamptonHouseFrench, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The video link 2 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.VideoChartwellCruiseEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The video link 3 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.VideoHONOUREnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The video link 4 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.VideoSeniorsDreamsComingTrueChartwellsPartnershipwithWishOfALifetimeEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The video link 5 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellness(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessLinks.MomentsThatMatterVideosAndPhotos, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellness6, "The video link 5 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.ForSeniorsFriendsComeWithHealthyBenefits, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle1, "The Article link 1 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.AgingWellIsMoreThanJustGoodPhysicalHealthForSeniors, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle2, "The Article link 2 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.HowSeniorsCanBoostTheirHealthSpanForAMoreVitalRetirement, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle3, "The Article link 3 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.WhyAPositiveOutlookOnLifeAndAgingIsGoodForYourHealth, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle4, "The Article link 4 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.SevenWaysToPromoteActiveAgingAndHealthyLongevity, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle5, "The Article link 5 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessArticles.EngagingInTheArtsBoostsSeniorsPhysicalAndEmotionalHealth, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessArticle6, "The Article link 6 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessAskOurExperts(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessAskOurExperts.AskOurExperts, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessAskOurExpert1, "The Ask Our Experts link 1 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessAskOurExperts(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessAskOurExperts.VideoDrAmyAssessingWhetherYourLifestyleIsHealthyAndEnjoyable, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Ask Our Experts link 2 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessAskOurExperts(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessAskOurExperts.ExploreChartwellsRhythmAndMovesExerciseClassWithLifestyleAndProgramManagerTraceyMcDonald, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessAskOurExpert3, "The Ask Our Experts link 3 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessAskOurExperts(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessAskOurExperts.TheRoleOfSocializationInSeniorsHealthAndWellness, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessAskOurExpert4, "The Ask Our Experts link 4 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessInfographics(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessInfographics.BenefitsOfSocializationPDF, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == activeLivingAndHealthAndWellnessInfographics1, "The Infographics link 1 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderActiveLivingAndHealthAndWellnessInfographics(driver, CommonHelpers.ActiveLivingAndHealthAndWellnessInfographics.HealthAndWellnessPDF, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Infographics link 2 for 'Active Living And Health & Wellness' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);
      #endregion

      #region Help Me Choose
      commonHelpers.ClickOnLinksUnderHelpMeChooseSection(driver, CommonHelpers.HelpMeChooseLinks.ExploringRetirementLivingGuidePDF, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(exploringRetirementLivingGuidePdf, driver1.Url, "The pdf for 'Exploring Retirement Living Guide PDF' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinksUnderHelpMeChooseSection(driver, CommonHelpers.HelpMeChooseLinks.AmIReadyQuestionnaire, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(isItTimeQuestionnaireForHelpMeChoose, driver1.Url, "The questionnaire for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinksUnderHelpMeChooseSection(driver, CommonHelpers.HelpMeChooseLinks.InfographicsBenefitsOfRetirementLiving, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(infographicsForHelpMeChoose, driver1.Url, "The infographics for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseAskOurExperts(driver, CommonHelpers.HelpMeChooseAskOurExperts.AskOurExperts, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseAskOurExperts1, "The Ask Our Experts link 1 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseAskOurExperts(driver, CommonHelpers.HelpMeChooseAskOurExperts.HowShouldIFurnishAndDecorateMyNewSuite, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseAskOurExperts2, "The Ask Our Experts link 2 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseAskOurExperts(driver, CommonHelpers.HelpMeChooseAskOurExperts.HowDoIKnowIfImReadyForRetirementLiving, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseAskOurExperts3, "The Ask Our Experts link 3 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.Blog, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle1, "The Article 1 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.WhatToLookForWhileTakingATourOfARetirement, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle2, "The Article 2 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.ThreeImportantQuestionsToAskWhileOnATourOfARetirementResidence, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle3, "The Article 3 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.TipsForFindingTheRightRetirementResidence, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle4, "The Article 4 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle5, "The Article 5 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.SevenQuestionsToDetermineIfYouAreRetirementResidenceReady, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle6, "The Article 6 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.WhyRetirementLivingMaySuitYouBetterThanReceivingSupportAtHome, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle7, "The Article 7 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.ThreeHealthyReasonsToConsiderRetirementLiving, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle8, "The Article 8 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.WhyRetirementResidenceLivingCanBeAHealthyChoicePart2, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle9, "The Article 9 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.ResearchingRetirementLivingHereIsWhereToStart, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle10, "The Article 10 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.TheBenefitsOfConsideringARetirementLifestyleBeforeExperiencingAHealthScare, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle11, "The Article 11 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.TheBenefitsOfAnIndependentLivingRetirementLifestyle, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle12, "The Article 12 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseArticles(driver, CommonHelpers.HelpMeChooseArticles.ThreeCompellingReasonsToMoveIntoARetirementCommunity, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseArticle13, "The Article 13 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseStaffStories(driver, CommonHelpers.HelpMeChooseStaffStories.PeterEnglish, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseStaffStories1 || driver1.Url == helpMeChooseStaffStories11 || driver1.Url == helpMeChooseStaffStories111 || driver1.Url == helpMeChooseStaffStories1111, "The Staff Story 1 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseStaffStories(driver, CommonHelpers.HelpMeChooseStaffStories.MichelFrench, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseStaffStories2 || driver1.Url == helpMeChooseStaffStories21 || driver1.Url == helpMeChooseStaffStories211 || driver1.Url == helpMeChooseStaffStories2111, "The Staff Story 2 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseStaffStories(driver, CommonHelpers.HelpMeChooseStaffStories.StephanieFrench, 5);
      Thread.Sleep(3000);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(driver1.Url == helpMeChooseStaffStories3 || driver1.Url == helpMeChooseStaffStories31 || driver1.Url == helpMeChooseStaffStories311 || driver1.Url == helpMeChooseStaffStories3111, "The Staff Story 3 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.ResidentStories, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 1 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.LoreenAndCecilEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 2 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.AurelAndMajaEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 3 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.PierretteFrench, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 4 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.LamarcheCoupleFrench, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 5 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.JenniferEnglish, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 6 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      commonHelpers.ClickOnLinkUnderHelpMeChooseResidentStories(driver, CommonHelpers.HelpMeChooseResidentStories.FlorenceFrench, 5);
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.IsTrue(!string.IsNullOrEmpty(driver1.Url), "The Resident Stories 7 for 'Help Me Choose' does not open on new tab.");
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);
      #endregion
    }

    [TestMethod, Description("Test Cases 3.4.6, 3.4.7, 3.4.8")]
    public void VerifyPrintPreviewAndResetForBudgetAssitant()
    {
      //Click on Learn >> Budget Assistant menu.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.BudgetAssistant, 5);

      #region Fill out Monthly Income Form.
      commonHelpers.GetElementByJS(driver, "#pension").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#oas").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#gis").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#benefitsOther").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#RRSP").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#RRIF").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#LRIF").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#investment").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#companyPension").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#employment").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#rental").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#otherIncome").SendKeys("5000");
      commonHelpers.ClickElementByJS(driver, "#nextIncome span");
      #endregion

      #region Fill out Your Home Equity Form
      commonHelpers.ClickElementByJS(driver, "#home_sell");
      commonHelpers.ClickElementByJS(driver, "#noSell");
      commonHelpers.ClickElementByJS(driver, "#nextEquity span");
      #endregion

      #region Fill out Housing Expenses Form
      commonHelpers.GetElementByJS(driver, "#mortgageCost").SendKeys("1000");
      commonHelpers.GetElementByJS(driver, "#rent").SendKeys("500");
      commonHelpers.GetElementByJS(driver, "#condo").SendKeys("500");
      commonHelpers.GetElementByJS(driver, "#propertyTaxes").SendKeys("500");

      commonHelpers.GetElementByJS(driver, "#utilitiesGas").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#utilitiesElectricity").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherUtilities").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#premium").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextHousingExpenses span");
      #endregion

      #region Fill out Home Maintenance Form
      commonHelpers.GetElementByJS(driver, "#lawnCare").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#landscaping").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#snowRemoval").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#security").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#windowClean").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#garbageCollection").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#roof").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#furnace").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#airConditioning").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#appliances").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#emergencyRepairs").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherMaintenance").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherCosts").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextHomeMaintenance span");
      #endregion

      #region Fill out Food And Entertainment Form
      commonHelpers.GetElementByJS(driver, "#foodExpense").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#entertainment").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextFoodExpenses span");
      #endregion

      #region Fill out Transportation Form
      commonHelpers.ClickElementByJS(driver, "#vehicleChoice");
      commonHelpers.ClickElementByJS(driver, "#vechicleNo");
      commonHelpers.ClickElementByJS(driver, "#nextTransportation span");
      #endregion

      string query = "document.querySelector('#btn_print').click();";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      try
      {
        js.ExecuteScript(query);
      }
      catch (WebDriverTimeoutException tE)
      {

      }

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      //IWebElement e = findElement.WebElement(driver1, By.Id("plugin"), 5);
      Assert.IsTrue(driver1.Url == "chrome://print/");
      driver1.Quit();
    }

    [TestMethod, Description("Test Case 3.4.9")]
    public void VerifyResetForBudgetAssitant()
    {
      #region Variables
      string expectedMessage = "Are you sure you want to re-calculate your results? You will need to enter all your information again.";
      #endregion

      //Click on Learn >> Budget Assistant menu.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.BudgetAssistant, 5);

      #region Fill out Monthly Income Form.
      commonHelpers.GetElementByJS(driver, "#pension").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#oas").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#gis").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#benefitsOther").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#RRSP").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#RRIF").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#LRIF").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#investment").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#companyPension").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#employment").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#rental").SendKeys("5000");
      commonHelpers.GetElementByJS(driver, "#otherIncome").SendKeys("5000");
      commonHelpers.ClickElementByJS(driver, "#nextIncome span");
      #endregion

      #region Fill out Your Home Equity Form
      commonHelpers.ClickElementByJS(driver, "#home_sell");
      commonHelpers.ClickElementByJS(driver, "#noSell");
      commonHelpers.ClickElementByJS(driver, "#nextEquity span");
      #endregion

      #region Fill out Housing Expenses Form
      commonHelpers.GetElementByJS(driver, "#mortgageCost").SendKeys("1000");
      commonHelpers.GetElementByJS(driver, "#rent").SendKeys("500");
      commonHelpers.GetElementByJS(driver, "#condo").SendKeys("500");
      commonHelpers.GetElementByJS(driver, "#propertyTaxes").SendKeys("500");

      commonHelpers.GetElementByJS(driver, "#utilitiesGas").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#utilitiesElectricity").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherUtilities").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#premium").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextHousingExpenses span");
      #endregion

      #region Fill out Home Maintenance Form
      commonHelpers.GetElementByJS(driver, "#lawnCare").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#landscaping").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#snowRemoval").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#security").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#windowClean").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#garbageCollection").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#roof").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#furnace").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#airConditioning").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#appliances").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#emergencyRepairs").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherMaintenance").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#otherCosts").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextHomeMaintenance span");
      #endregion

      #region Fill out Food And Entertainment Form
      commonHelpers.GetElementByJS(driver, "#foodExpense").SendKeys("50");
      commonHelpers.GetElementByJS(driver, "#entertainment").SendKeys("50");
      commonHelpers.ClickElementByJS(driver, "#nextFoodExpenses span");
      #endregion

      #region Fill out Transportation Form
      commonHelpers.ClickElementByJS(driver, "#vehicleChoice");
      commonHelpers.ClickElementByJS(driver, "#vechicleNo");
      commonHelpers.ClickElementByJS(driver, "#nextTransportation span");
      #endregion

      //Click on Reset The Form Button.
      commonHelpers.ClickElementByJS(driver, "#refresh");
      string message = commonHelpers.GetTextByJS(driver, "#refreshPage strong");
      Assert.AreEqual(expectedMessage, message);
      commonHelpers.ClickElementByJS(driver, "#btn_refresh_calculate");
    }

    [TestMethod, Description("Test Case 3.4.10")]
    public void VerifyErrorMessageOnBudgetAssistant()
    {
      #region Variables
      string errorMessage = "Please enter a numeric value.";
      #endregion

      //Click on Learn >> Budget Assistant menu.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.BudgetAssistant, 5);
      commonHelpers.GetElementByJS(driver, "#pension").SendKeys("abc");
      commonHelpers.GetElementByJS(driver, "#oas").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#pensionError").Trim());
      commonHelpers.GetElementByJS(driver, "#gis").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#oasError").Trim());
      commonHelpers.GetElementByJS(driver, "#benefitsOther").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#gisError").Trim());
      commonHelpers.GetElementByJS(driver, "#RRSP").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#benefitsOtherError").Trim());
      commonHelpers.GetElementByJS(driver, "#RRIF").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#rrspError").Trim());
      commonHelpers.GetElementByJS(driver, "#LRIF").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#rrifError").Trim());
      commonHelpers.GetElementByJS(driver, "#investment").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#lrifError").Trim());
      commonHelpers.GetElementByJS(driver, "#companyPension").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#investmentError").Trim());
      commonHelpers.GetElementByJS(driver, "#employment").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#companyPensionError").Trim());
      commonHelpers.GetElementByJS(driver, "#rental").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#employmentError").Trim());
      commonHelpers.GetElementByJS(driver, "#otherIncome").SendKeys("abc");
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#rentalError").Trim());
      commonHelpers.GetElementByJS(driver, "#otherIncome").SendKeys(Keys.Tab);
      Assert.AreEqual(errorMessage, commonHelpers.GetTextByJS(driver, "#otherIncome2Error").Trim());
    }

    [TestMethod, Description("Test Case 3.4.3")]
    public void VerifyFinancesWithKelleyKeehn()
    {
      #region Variables
      string expectedSource1 = "https://www.youtube.com/embed/WaZlUWDTAhw?modestbranding=1&showinfo=0&rel=0&iv_load_policy=3&enablejsapi=1";
      string expectedSource2 = "https://www.youtube.com/embed/Yi7NtSPIhok?modestbranding=1&showinfo=0&rel=0&iv_load_policy=3&enablejsapi=1";
      #endregion
      //Click on menu Finances with Kelley Keehn under Learn.
      commonHelpers.ClickMenuInLearn(driver, browserType, CommonHelpers.LearnMenu.FinancesWithKelleyKeehn, 5);
      findElement.Wait(driver, By.Id("ChartwellVideoGallery"), 10);

      //Verify list of videos
      IList<IWebElement> listOfVideos = findElement.WebElements(driver, By.CssSelector("div[data-target='#videoModal']"));
      Assert.IsTrue(listOfVideos.Count == 14);

      //Click on first video.
      listOfVideos[0].Click();

      //Verify video appears in modal.
      string source = findElement.WebElement(driver, By.CssSelector("#videoModal iframe"), 5).GetAttribute("src");
      Assert.AreEqual(expectedSource1, source);

      //Click on next button.
      findElement.WebElement(driver, By.Id("next-button"), 5).Click();
      Thread.Sleep(2000);

      //Verify video appear in modal
      source = findElement.WebElement(driver, By.CssSelector("#videoModal iframe"), 5).GetAttribute("src");
      Assert.AreEqual(expectedSource2, source);

      //Click on previous button.
      findElement.WebElement(driver, By.Id("previous-button"), 5).Click();
      Thread.Sleep(1000);

      //Verify video appear in modal
      source = findElement.WebElement(driver, By.CssSelector("#videoModal iframe"), 5).GetAttribute("src");
      Assert.AreEqual(expectedSource1, source);

      //Click on close button.
      findElement.WebElement(driver, By.Id("close-button"), 5).Click();

      //Click on first filter.
      IList<IWebElement> listOfFilters = findElement.WebElements(driver, By.CssSelector("#chips div"));
      listOfFilters[0].Click();
      listOfFilters[2].Click();
      listOfFilters[3].Click();
      listOfFilters[4].Click();
      Thread.Sleep(2000);

      //Verify list of videos
      listOfVideos = findElement.WebElements(driver, By.CssSelector(".col-md-6.col-lg-4.videoItem.aging-at-home"));
      Assert.IsTrue(listOfVideos.Count == 1);

      //Click on Clear filter button.
      findElement.WebElement(driver, By.Id("chipResetBtn"), 5).Click();

      //Verify video count
      listOfVideos = findElement.WebElements(driver, By.CssSelector("div[data-target='#videoModal']"));
      Assert.IsTrue(listOfVideos.Count == 14);
    }
    #endregion

    #region About Us
    [TestMethod, Description("Test Case 3.5.1")]
    public void VerifyWelcomeMenuUnderAboutUs()
    {
      #region Variables
      string title = "Welcome to Chartwell Retirement Residences";
      string pdfURL = "https://chartwell.com/-/media/Files/careers/chartwell-careers-infographic-en.pdf";
      #endregion
      //Click on Welcome menu under About Us.
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.Welcome, 5);

      //Verify title displays.
      Assert.AreEqual(title, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title + " does not display.");

      IWebElement pdfThumbnail = findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/welcome/careers-infographic-download-icon-en.png']"), 5);
      //Verify thumbnail for infographic dispalys.
      Assert.IsTrue(pdfThumbnail != null);

      //Click on PDF
      pdfThumbnail.Click();

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      //Verify pdf opens in new tab.
      Assert.AreEqual(pdfURL, driver1.Url);

      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      //Verify feature photo displays.
      IWebElement featurePhoto = findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/about-us/welcome/welcome.png']"), 3);
      Assert.IsTrue(featurePhoto != null);

      //Verify RESPECT text displays.
      IList<IWebElement> respectList = findElement.WebElements(driver, By.CssSelector(".RespectList li"));
      Assert.IsTrue(respectList[0].Text == "Respect");
      Assert.IsTrue(respectList[1].Text == "Empathy");
      Assert.IsTrue(respectList[2].Text == "Service Excellence");
      Assert.IsTrue(respectList[3].Text == "Performance");
      Assert.IsTrue(respectList[4].Text == "Education");
      Assert.IsTrue(respectList[5].Text == "Commitment");
      Assert.IsTrue(respectList[6].Text == "Trust");
    }

    [TestMethod, Description("Test Case 3.5.2")]
    public void VerifyOurVisionMissionAndValuesMenuUnderAboutUs()
    {
      #region Variables
      string title = "Our Vision, Mission and Values";
      #endregion
      //Click on Our Vision Mission And Values menu under About Us.
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.OurVisionMissionAndValues, 5);

      //Verify title displays.
      Assert.AreEqual(title, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title + " does not display.");

      //Verify Thumbnail for Youtube video displays.
      IWebElement youTubeThumbnail = findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/What-makes-you-happy.jpg']"), 5);
      Assert.IsTrue(youTubeThumbnail != null, "Youtube thumbnail does not display.");
      youTubeThumbnail.Click();

      //Verify Youtube Video opens.
      IWebElement youTubeVideo = findElement.WebElement(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/Kf4Ov9vxEp8?rel=0&iv_load_policy=3&enablejsapi=1']"), 5);
      Assert.IsTrue(youTubeVideo != null, "Youtube video does not open.");

      findElement.WebElement(driver, By.CssSelector(".modal-footer button"), 2).Click();

      //Verify RESPECT image displays.
      IWebElement respectImage = findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/respect-EN.jpg']"), 3);
      Assert.IsTrue(respectImage != null, "RESPECT image does not display.");
    }

    [TestMethod, Description("Test Case 3.5.3")]
    public void VerifyOurLeadershipTeamMenuUnderAboutUs()
    {
      #region Variables
      string title = "Our Leadership Team";
      string imageBrentBinions = "https://chartwell.com/-/media/Images/icons/about-us/our-leadership/BBinnions_headshot_Transparent.png";
      string imageVladVolodarski = "https://chartwell.com/-/media/Images/icons/about-us/our-leadership/VVoladarski_headshot_Transparent.png";
      string imageKarenSullivan = "https://chartwell.com/-/media/Images/icons/about-us/our-leadership/KSullivan_headshot_Transparent.png";
      string imageJonathanBoulakia = "https://chartwell.com/-/media/Images/icons/about-us/our-leadership/JBoulakia_headshot_Transparent.png";
      string imageSheriHarris = "https://chartwell.com/-/media/Images/icons/about-us/our-leadership/SHarris_headshot_Transparent.png";
      #endregion
      //Click on Our Leadership Team menu under About Us.
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.OurLeadershipTeam, 5);

      //Verify title displays.
      Assert.AreEqual(title, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title + " does not display.");

      if (browserType != "Desktop")
      {
        //Verify photo of SEC members display.
        Assert.AreEqual(imageBrentBinions, findElement.WebElement(driver, By.CssSelector("#independet img"), 5).GetAttribute("src"), "Image of BRENT BINIONS does not display.");
        Assert.AreEqual(imageVladVolodarski, findElement.WebElement(driver, By.CssSelector("#supportive img"), 5).GetAttribute("src"), "Image of VLAD VOLODARSKI does not display.");
        Assert.AreEqual(imageKarenSullivan, findElement.WebElement(driver, By.CssSelector("#assisted img"), 5).GetAttribute("src"), "Image of KAREN SULLIVAN does not display.");
        Assert.AreEqual(imageJonathanBoulakia, findElement.WebElement(driver, By.CssSelector("#memory img"), 5).GetAttribute("src"), "Image of JONATHAN BOULAKIA does not display.");
        Assert.AreEqual(imageSheriHarris, findElement.WebElement(driver, By.CssSelector("#long img"), 5).GetAttribute("src"), "Image of SHERI HARRIS does not display.");

        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("independet"), 3).GetAttribute("class").Contains("show")), "Tab for BRENT BINIONS is not expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("supportive"), 3).GetAttribute("class").Contains("show")), "Tab for VLAD VOLODARSKI is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("assisted"), 3).GetAttribute("class").Contains("show")), "Tab for KAREN SULLIVAN is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("memory"), 3).GetAttribute("class").Contains("show")), "Tab for JONATHAN BOULAKIA is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("long"), 3).GetAttribute("class").Contains("show")), "Tab for SHERI HARRIS is expanded.");

        commonHelpers.ClickElementByJS(driver, "#supportive button");
        Thread.Sleep(2000);
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("independet"), 3).GetAttribute("class").Contains("show")), "Tab for BRENT BINIONS is expanded.");
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("supportive"), 3).GetAttribute("class").Contains("show")), "Tab for VLAD VOLODARSKI is not expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("assisted"), 3).GetAttribute("class").Contains("show")), "Tab for KAREN SULLIVAN is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("memory"), 3).GetAttribute("class").Contains("show")), "Tab for JONATHAN BOULAKIA is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("long"), 3).GetAttribute("class").Contains("show")), "Tab for SHERI HARRIS is expanded.");

        commonHelpers.ClickElementByJS(driver, "#assisted button");
        Thread.Sleep(2000);
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("independet"), 3).GetAttribute("class").Contains("show")), "Tab for BRENT BINIONS is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("supportive"), 3).GetAttribute("class").Contains("show")), "Tab for VLAD VOLODARSKI is expanded.");
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("assisted"), 3).GetAttribute("class").Contains("show")), "Tab for KAREN SULLIVAN is not expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("memory"), 3).GetAttribute("class").Contains("show")), "Tab for JONATHAN BOULAKIA is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("long"), 3).GetAttribute("class").Contains("show")), "Tab for SHERI HARRIS is expanded.");

        commonHelpers.ClickElementByJS(driver, "#memory button");
        Thread.Sleep(2000);
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("independet"), 3).GetAttribute("class").Contains("show")), "Tab for BRENT BINIONS is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("supportive"), 3).GetAttribute("class").Contains("show")), "Tab for VLAD VOLODARSKI is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("assisted"), 3).GetAttribute("class").Contains("show")), "Tab for KAREN SULLIVAN is expanded.");
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("memory"), 3).GetAttribute("class").Contains("show")), "Tab for JONATHAN BOULAKIA is not expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("long"), 3).GetAttribute("class").Contains("show")), "Tab for SHERI HARRIS is expanded.");

        commonHelpers.ClickElementByJS(driver, "#long button");
        Thread.Sleep(2000);
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("independet"), 3).GetAttribute("class").Contains("show")), "Tab for BRENT BINIONS is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("supportive"), 3).GetAttribute("class").Contains("show")), "Tab for VLAD VOLODARSKI is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("assisted"), 3).GetAttribute("class").Contains("show")), "Tab for KAREN SULLIVAN is expanded.");
        Assert.IsFalse(Convert.ToBoolean(findElement.WebElement(driver, By.Id("memory"), 3).GetAttribute("class").Contains("show")), "Tab for JONATHAN BOULAKIA is expanded.");
        Assert.IsTrue(Convert.ToBoolean(findElement.WebElement(driver, By.Id("long"), 3).GetAttribute("class").Contains("show")), "Tab for SHERI HARRIS is not expanded.");
      }

      else
      {
        //Verify photo of SEC members display.
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/our-leadership/BBinnions_headshot_Transparent.png']"), 5) != null, "Image of BRENT BINIONS does not display.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/our-leadership/VVoladarski_headshot_Transparent.png']"), 5) != null, "Image of VLAD VOLODARSKI does not display.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/our-leadership/KSullivan_headshot_Transparent.png']"), 5) != null, "Image of KAREN SULLIVAN does not display.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/our-leadership/JBoulakia_headshot_Transparent.png']"), 5) != null, "Image of JONATHAN BOULAKIA does not display.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/our-leadership/SHarris_headshot_Transparent.png']"), 5) != null, "Image of SHERI HARRIS does not display.");

        //Toggling between tabs function as expected.
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("a[href='#independet']"), 2).GetAttribute("class").Contains("active"), "Tab for BRENT BINIONS is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).GetAttribute("class").Contains("active"), "Tab for VLAD VOLODARSKI is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).GetAttribute("class").Contains("active"), "Tab for KAREN SULLIVAN is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).GetAttribute("class").Contains("active"), "Tab for JONATHAN BOULAKIA is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).GetAttribute("class").Contains("active"), "Tab for SHERI HARRIS is selected.");

        findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).Click();
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#independet']"), 2).GetAttribute("class").Contains("active"), "Tab for BRENT BINIONS is selected.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).GetAttribute("class").Contains("active"), "Tab for VLAD VOLODARSKI is not selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).GetAttribute("class").Contains("active"), "Tab for KAREN SULLIVAN is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).GetAttribute("class").Contains("active"), "Tab for JONATHAN BOULAKIA is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).GetAttribute("class").Contains("active"), "Tab for SHERI HARRIS is selected.");

        findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).Click();
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#independet']"), 2).GetAttribute("class").Contains("active"), "Tab for BRENT BINIONS is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).GetAttribute("class").Contains("active"), "Tab for VLAD VOLODARSKI is selected.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).GetAttribute("class").Contains("active"), "Tab for KAREN SULLIVAN is not selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).GetAttribute("class").Contains("active"), "Tab for JONATHAN BOULAKIA is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).GetAttribute("class").Contains("active"), "Tab for SHERI HARRIS is selected.");

        findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).Click();
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#independet']"), 2).GetAttribute("class").Contains("active"), "Tab for BRENT BINIONS is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).GetAttribute("class").Contains("active"), "Tab for VLAD VOLODARSKI is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).GetAttribute("class").Contains("active"), "Tab for KAREN SULLIVAN is selected.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).GetAttribute("class").Contains("active"), "Tab for JONATHAN BOULAKIA is not selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).GetAttribute("class").Contains("active"), "Tab for SHERI HARRIS is selected.");

        findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).Click();
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#independet']"), 2).GetAttribute("class").Contains("active"), "Tab for BRENT BINIONS is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#supportive']"), 2).GetAttribute("class").Contains("active"), "Tab for VLAD VOLODARSKI is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#assisted']"), 2).GetAttribute("class").Contains("active"), "Tab for KAREN SULLIVAN is selected.");
        Assert.IsFalse(findElement.WebElement(driver, By.CssSelector("a[href='#memory']"), 2).GetAttribute("class").Contains("active"), "Tab for JONATHAN BOULAKIA is selected.");
        Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("a[href='#long']"), 2).GetAttribute("class").Contains("active"), "Tab for SHERI HARRIS is not selected.");
      }
    }

    [TestMethod, Description("Test Case 3.5.4")]
    public void VerifyWishOfALifetimeCanadaMenuUnderAboutUs()
    {
      #region Variables
      string nominateASeniorURL = "https://wishofalifetime.ca/what-we-do/submit-a-wish/";
      string makeADonationURL = "https://wishofalifetime.ca/donate/";
      #endregion
      //Click on Wish of a Lifetime Canada under About Us
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.WishOfALifetimeCanada, 5);

      //Verify title displays.
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("img[src=' /-/media/Images/partner-logos/WOL-Canada-Logo-Header2.png']"), 5) != null);

      //Verify Youtube video displays. rel=0 for related videos from same channel
      Assert.IsTrue(findElement.WebElement(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/lH2bSyJ0WNs?rel=0&iv_load_policy=3&enablejsapi=1']"), 5) != null);

      //Click on Nominate a Senior
      commonHelpers.ClickElementByJS(driver, "a[href=\"https://wishofalifetime.ca/what-we-do/submit-a-wish/\"]");

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(nominateASeniorURL, driver1.Url);
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      //Click on Make A Donation
      commonHelpers.ClickElementByJS(driver, "a[href=\"https://wishofalifetime.ca/donate/\"]");

      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(makeADonationURL, driver1.Url);
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      //Verify images load quickly.
      IList<IWebElement> images = findElement.WebElements(driver, By.CssSelector(".j-image img"));
      foreach (IWebElement image in images)
      {
        Assert.IsTrue(!string.IsNullOrEmpty(image.GetAttribute("src")));
      }
    }

    [TestMethod, Description("Test Case 3.5.5")]
    public void VerifyMomentsThatMatterMenuUnderAboutUs()
    {
      #region Variables
      string title = "Moments that Matter at Chartwell";
      #endregion

      //Click on Moments That Matter menu under About Us
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.MomentsThatMatter, 5);

      //Verify title displays.
      Assert.AreEqual(title, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title + " does not display.");
    }

    [TestMethod, Description("Test Case 3.5.6")]
    public void VerifyCorporateSocialResponsibilityMenuUnderAboutUs()
    {
      #region Variables
      string title1 = "What is CSR?";
      string title2 = "A Message from the CEO";
      #endregion

      //Click on Corporate Social Responsibility menu under About Us.
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.CorporateSocialResponsibility, 5);

      //Verify title displays.
      Assert.AreEqual(title1, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title1 + " does not display.");
      Assert.AreEqual(title2, findElement.WebElement(driver, By.TagName("h2"), 3).Text, "Title " + title2 + " does not display.");

      //Verify CEO's photo displays.
      Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/icons/about-us/csr/csr-brent-english.jpg']"), 3), "CEO's Photo does not display.");
      Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("a[href='/about-us/corporate-social-responsibility']"), 3), "One of the quick links does not display.");
      Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("a[href='/about-us/corporate-social-responsibility/corporate-social-responsibility-program']"), 3), "One of the quick links does not display.");
    }

    [TestMethod, Description("Test Case 3.5.7")]
    public void VerifyHonourOurVeteransMenuUnderAboutUs()
    {
      #region Variables
      string title = "Honour Our Veterans";
      string pdfURL = "https://chartwell.com/-/media/Files/books/chartwell-honour.pdf";
      #endregion

      //Click on Honour Our Veterans menu under About Us.
      commonHelpers.ClickMenuInAboutUs(driver, browserType, CommonHelpers.AboutUsMenu.HonourOurVeterans, 5);

      //Verify Title.
      Assert.AreEqual(title, findElement.WebElement(driver, By.TagName("h1"), 3).Text, "Title " + title + " does not display.");

      //Verify Download thumbnail displays.
      IWebElement pdfThumbnail = findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/living-at-chartwell/about-us/honour-our-veterans/HonourBook-web-eng.JPG']"), 3);
      Assert.IsNotNull(pdfThumbnail, "Download thumbnail does not display.");
      pdfThumbnail.Click();

      //Verify PDF opens in separate tab.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Assert.AreEqual(pdfURL, driver1.Url);
      driver1.Close();
      driver.SwitchTo().Window(tabs[0]);

      //Verify Youtube video displays.
      Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/L7uuFirvvy8?rel=0&iv_load_policy=3&enablejsapi=1']"), 4), "Youtube video does not display.");
    }
    #endregion

    #region Property Pages (Left Nav)
    [TestMethod, Description("Test Cases 4.1.1, 4.1.2, 4.1.3, 4.1.4, 4.1.5, 4.1.6, 4.1.7, 4.2.1, 4.2.2, 4.3.1, 4.3.2")]
    public void VerifyOverviewPage()
    {
      #region Variables
      string propertyNameRetirementResidence = "Chartwell Robert Speck Retirement Residence";
      string propertyNameLongTermCareResidence = "Chartwell Parkhill Long Term Care Residence";
      string addressRetirementResidence = "100 Robert Speck Parkway, Mississauga, Ontario L4Z 0A1";
      string addressLongTermCareResidence = "250 Tain Street, Parkhill, Ontario N0M 2K0";
      string taglineRetirementResidence = "\"Offering an active and urban retirement lifestyle in the heart of Mississauga.\"";
      string taglineLongTermCareResidence = "\"We treat our residents with dignity and respect\"";
      string propertyMarketingStory1RetirementResidence = "Chartwell Robert Speck Retirement Residence, nestled in the heart of Mississauga, close to Square One Shopping Centre, is a modern senior living community offering an independent supportive living environment. With one- and two-bedroom suites, including some with dens, you’ll find yourself settling in quickly to this well-appointed and bustling residence. A green sanctuary set back from the bustling surrounding streets, there’s a warmth and energy that radiates from Chartwell Robert Speck, both in our welcoming common areas and manicured gardens—and from our friendly, caring staff. The result is a retirement home where residents are active, independent members of a close-knit community, supported by dedicated staff and life-enriching activities.";
      string propertyMarketingStory2RetirementResidence = "Residents at Chartwell Robert Speck often remark that they’re enjoying new pursuits they never imagined themselves doing. With a full range of activities and outings, this is retirement living as it’s meant to be. The amenities are equally impressive: a full-service dining room, bistro, hair salon, spa and fitness centre, to name a few, and of course a gorgeous outdoor courtyard and greenhouse to enjoy or garden in. If needs change, we offer supportive personal care options, including activities of daily living and medication assistance—services that provide residents and families with a sense of security and peace of mind.";
      string propertyMarketingStoryLongTermCareResidence = "Located in rural Parkhill, Ontario, Chartwell Parkhill Long Term Care Residence offers residents a comfortable, friendly and home-like environment. Twenty-four-hour nursing support, assistance with daily living activities and high levels of personal care are provided by a team of highly dedicated and skilled professionals. Chartwell Parkhill is equipped to support varying health needs, and on-site supervision ensures the personal safety and well-being of residents at all times. At Chartwell Parkhill, you can feel confident that your loved one is well cared for and is receiving the highest standards of service. We treat our residents with dignity and respect, and offer them and their families the peace of mind they deserve.";
      string pdfURL = "https://chartwell.com/-/media/Files/brochures/chartwell-robert-speck-retirement-residence-brochure.pdf";
      string printPreviewURL = "chrome://print/";
      string googleReviewURL = "https://www.google.ca/search?q=Chartwell+Robert+Speck+Retirement+Residence+Mississauga";
      string chartwellReviewURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/reviews";
      #endregion
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;

      //Verify breadcrum displays.
      IList<IWebElement> breadcrumbs = findElement.WebElements(driver, By.ClassName("breadcrumb-item"));
      foreach (IWebElement breadcrumb in breadcrumbs)
      {
        Assert.IsNotNull(breadcrumb, breadcrumb.Text.Trim() + " does not display.");
      }

      if (propertyType == "RetirementResidence")
      {
        //Verify Property Name displays.
        Assert.AreEqual(propertyNameRetirementResidence, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Property Name of Retirement Residence does not match with expected name.");

        //Verify Address.
        Assert.AreEqual(addressRetirementResidence, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address for Retirement Residence is displayed.");

        //Verify tagline.
        Assert.AreEqual(taglineRetirementResidence, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim());

        //Verify Virtual tour thumbnail.
        Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/JRlYuLSs3B8?rel=0&iv_load_policy=3&enablejsapi=1']"), 5), "Virtual Thumbnail for Retirement Residence does not display.");

        //Verify Property Marketing Story.
        commonHelpers.ClickElementByJS(driver, "#btnPropRead");
        IList<IWebElement> stories = findElement.WebElements(driver, By.CssSelector("#ReadMoreContent p"));
        Assert.AreEqual(propertyMarketingStory1RetirementResidence, stories[1].Text.Trim(), "Story for Retirement Residence does not match with expected story.");
        Assert.AreEqual(propertyMarketingStory2RetirementResidence, stories[2].Text.Trim(), "Story for Retirement Residence does not match with expected story.");

        //Verify Download Property Brochure Button.
        IWebElement downloadPropertyBrochureBtn = findElement.WebElement(driver, By.CssSelector("a[href='/-/media/Files/brochures/chartwell-robert-speck-retirement-residence-brochure.pdf']"), 2);
        Assert.IsNotNull(downloadPropertyBrochureBtn, "Download Property Brochure button for Retirement Residency does not display.");

        //Verify Click on Download Property Brochure opens new tab with pdf.
        commonHelpers.ClickElementByJS(driver, "a[href=\"/-/media/Files/brochures/chartwell-robert-speck-retirement-residence-brochure.pdf\"]");
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(pdfURL, driver1.Url);
        driver1.Close();
        driver.SwitchTo().Window(tabs[0]);

        //Verify Google Review button.
        IWebElement googleReviewBtn = findElement.WebElement(driver, By.CssSelector("img[src='/Assets/Images/google-rate-button.png']"), 2);
        Assert.IsNotNull(googleReviewBtn, "Google Review button for Retirement Residency does not display.");

        //Verify click on Google Review button opens correct page.
        commonHelpers.ClickElementByJS(driver, "img[src=\"/Assets/Images/google-rate-button.png\"]");
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(googleReviewURL, driver1.Url);
        Assert.IsNotNull(driver1.FindElements(By.XPath("//*[contains(text(),'Google Reviews')]/ancestor::div[contains(@class, 'knowledge-panel')]")));
        string reviewNumber = string.Empty;

        //Verify Google Reviews on google page.
        if (browserType != "Desktop")
        {
          reviewNumber = findElement.WebElement(driver1, By.CssSelector(".Ob2kfd span.z5jxId"), 2).Text;
          reviewNumber = reviewNumber.Substring(1);
          reviewNumber = reviewNumber.Substring(0, reviewNumber.Length - 1);
        }
        else
        {
          reviewNumber = findElement.WebElements(driver1, By.CssSelector("a[data-sort_by='qualityScore'] span"))[0].Text.Trim();
        }
        Assert.IsTrue(Regex.Match(reviewNumber, @".*([\d]+).*").Success, "Number of Reviews does not display.");

        //Verify Ratings on Google page.
        var rating = findElement.WebElement(driver1, By.CssSelector("span.Aq14fc"), 2);
        Assert.IsTrue(Regex.Match(rating.Text.Trim(), @".*([\d]+).*").Success, "Number of Ratings does not display.");

        driver1.Close();
        driver.SwitchTo().Window(tabs[0]);

        //Verify Chartwell Review button.
        IWebElement chartwellReviewBtn = findElement.WebElement(driver, By.CssSelector("img[src='/Assets/Images/chartwell-rate-button.png']"), 2);
        Assert.IsNotNull(chartwellReviewBtn, "Chartwell Review button for Retirement Residency does not display.");

        //Verify clicking on Chartwell Review button opens correct page.
        commonHelpers.ClickElementByJS(driver, "img[src=\"/Assets/Images/chartwell-rate-button.png\"]");
        Thread.Sleep(5000);
        Assert.AreEqual(chartwellReviewURL, driver.Url);
        driver.Navigate().Back();
        Thread.Sleep(5000);

        //Verify Left Nav
        Assert.AreEqual("Overview", commonHelpers.GetTextByJS(driver, "#lnk-overview"), "Overview does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Photos", commonHelpers.GetTextByJS(driver, "#lnk-photos"), "Photos does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Virtual Tour", commonHelpers.GetTextByJS(driver, "#lnk-virtual-tour"), "Virtual Tour does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Service Levels", commonHelpers.GetTextByJS(driver, "#lnk-service-levels"), "Service Levels does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Dining Services", commonHelpers.GetTextByJS(driver, "#lnk-dining-services"), "Dinig Services does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Wellness Services", commonHelpers.GetTextByJS(driver, "#lnk-wellness-services"), "Wellness Services does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Activities", commonHelpers.GetTextByJS(driver, "#lnk-activities"), "Activities does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Amenities", commonHelpers.GetTextByJS(driver, "#lnk-amenities"), "Amenities does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Reviews & Ratings", commonHelpers.GetTextByJS(driver, "#lnk-reviews"), "Reviews & Ratings does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Map", commonHelpers.GetTextByJS(driver, "#lnk-map"), "Map does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Neighbourhood", commonHelpers.GetTextByJS(driver, "#lnk-neighbourhood"), "Neighbourhood does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Contact Us", commonHelpers.GetTextByJS(driver, "#lnk-contact"), "Contact Us does not display on Left Nav on Overview page for Retirement Residence.");

        if (browserType == "Desktop")
        {
          //Verify Contact Form on Right Nav
          Assert.IsNotNull(findElement.WebElement(driver, By.Id("chartwellResidencesContactForm"), 2), "Contact Form does not display on Retirement Residency page.");
        }

        //Verify Nearby Residency display.
        Assert.IsTrue((findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 img")).Count == 4) && (findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 a.panel.panel-info")).Count == 4), "There are less than four or none Nearby Retirement Residencies display.");

        var elements = findElement.WebElements(driver, By.CssSelector(".panel.panel-info address"));

        foreach (IWebElement element in elements)
        {
          //Verify Street Numbers are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[0].Trim(), @".*([\d]+).*").Success, "Street Number is not displayed for any of the properties.");

          //Verify City Names are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[1].Trim(), @"^[A-Z].*").Success, "City Name is not displayed for any of the properties.");

          //Verify Province Name is displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[2].Substring(0, element.Text.Split(',')[2].Trim().Length - 7).Trim(), @"^[A-Z].*").Success, "Province Name is not displayed for any of the properties.");

          //Verify Postal Codes are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[2].Trim().Substring(element.Text.Trim().Split(',')[2].Trim().Length - 7, 7).Trim(), @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$").Success, "Postal Code is not displayed for any of the properties.");
        }

        //Verity telephone numbers are displayed on Search Result Page.
        var telephoneNumbers = findElement.WebElements(driver, By.CssSelector(".panel-footer label"));
        Assert.IsTrue(telephoneNumbers.All(t => Regex.Match(t.Text.Trim(), @"([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})").Success));

        var propertyDistances = findElement.WebElements(driver, By.CssSelector(".panel-footer p"));

        foreach (IWebElement distance in propertyDistances)
        {
          string distanceText = propertyDistances[0].Text.Trim().Substring(10);
          string distanceNumber = distanceText.Substring(0, distanceText.Length - 3);
          decimal d1 = Convert.ToDecimal(distanceNumber);
          int compareResult = Decimal.Compare(0, d1);
          Assert.IsTrue(compareResult != 0, "0 KM is displayed.");
        }

        //Click on any near by residence.
        string nearbyResidenceURL = findElement.WebElements(driver, By.CssSelector(".container a"))[0].GetAttribute("href");
        commonHelpers.ClickElementByJS(driver, ".container a:nth-child(1)");
        Thread.Sleep(3000);
        Assert.AreEqual(nearbyResidenceURL, driver.Url);

        driver.Navigate().Back();
        Thread.Sleep(3000);

        //Verify Click Here To Print link displays.
        IWebElement clickHereToPrintBtn = findElement.WebElement(driver, By.CssSelector("a[href='javascript:window.print()']"), 2);
        Assert.IsNotNull(clickHereToPrintBtn, "Click Here To Print link does not display.");

        //Verify click on Click Here To Print button opens print preview page.
        commonHelpers.ClickElementByJS(driver, "a[href=\"javascript:window.print()\"]");
        Thread.Sleep(3000);
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(printPreviewURL, driver1.Url);
        driver1.Quit();
      }
      else
      {
        //Verify Property Name displays.
        Assert.AreEqual(propertyNameLongTermCareResidence, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Property Name of Long Term Care Residence does not match with expected name.");

        //Verify Address.
        Assert.AreEqual(addressLongTermCareResidence, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address for Long Term Care Residence is displayed.");

        //Verify tagline.
        Assert.AreEqual(taglineLongTermCareResidence, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim());

        //Verify image.
        Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("img[src='/-/media/Images/photo-gallery/parkhill-ltc/parkhill-exterior-02.jpg?h=155&mw=275&w=275&hash=6DC358ED8A331EDEB3F594CC7A79F202']"), 5), "Image for Long Term Care Residence does not display.");

        //Verify Property Marketing Story.
        IList<IWebElement> stories = findElement.WebElements(driver, By.CssSelector("#ReadMoreContent p"));
        Assert.AreEqual(propertyMarketingStoryLongTermCareResidence, stories[1].Text.Trim(), "Story for Long Term Residence does not match with expected story.");

        //Verify Left Nav
        Assert.AreEqual("Overview", commonHelpers.GetTextByJS(driver, "#lnk-overview"), "Overview does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Photos", commonHelpers.GetTextByJS(driver, "#lnk-photos"), "Photos does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Service Levels", commonHelpers.GetTextByJS(driver, "#lnk-service-levels"), "Service Levels does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Dining Services", commonHelpers.GetTextByJS(driver, "#lnk-dining-services"), "Dinig Services does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Wellness Services", commonHelpers.GetTextByJS(driver, "#lnk-wellness-services"), "Wellness Services does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Activities", commonHelpers.GetTextByJS(driver, "#lnk-activities"), "Activities does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Map", commonHelpers.GetTextByJS(driver, "#lnk-map"), "Map does not display on Left Nav on Overview page for Long Term Care Residence.");
        Assert.AreEqual("Contact Us", commonHelpers.GetTextByJS(driver, "#lnk-contact"), "Contact Us does not display on Left Nav on Overview page for Long Term Care Residence.");

        if (browserType == "Desktop")
        {
          //Verify Contact Form on Right Nav
          Assert.IsNotNull(findElement.WebElement(driver, By.Id("chartwellResidencesContactForm"), 2), "Contact Form does not display on Long Term Care Residency page.");
        }
        //Verify nearby residency display.
        Assert.IsTrue((findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 img")).Count == 4) && (findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 a.panel.panel-info")).Count == 4), "There are less than four or none Nearby Long Term Care Residencies display.");

        var elements = findElement.WebElements(driver, By.CssSelector(".panel.panel-info address"));

        foreach (IWebElement element in elements)
        {
          //Verify Street Numbers are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[0].Trim(), @".*([\d]+).*").Success, "Street Number is not displayed for any of the properties.");

          //Verify City Names are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[1].Trim(), @"^[A-Z].*").Success, "City Name is not displayed for any of the properties.");

          //Verify Province Name is displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[2].Substring(0, element.Text.Split(',')[2].Trim().Length - 7).Trim(), @"^[A-Z].*").Success, "Province Name is not displayed for any of the properties.");

          //Verify Postal Codes are displayed for the properties displayed on Search Result Page.
          Assert.IsTrue(Regex.Match(element.Text.Trim().Split(',')[2].Trim().Substring(element.Text.Trim().Split(',')[2].Trim().Length - 7, 7).Trim(), @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$").Success, "Postal Code is not displayed for any of the properties.");
        }

        //Verity telephone numbers are displayed on Search Result Page.
        var telephoneNumbers = findElement.WebElements(driver, By.CssSelector(".panel-footer label"));
        Assert.IsTrue(telephoneNumbers.All(t => Regex.Match(t.Text.Trim(), @"([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})").Success));

        var propertyDistances = findElement.WebElements(driver, By.CssSelector(".panel-footer p"));

        foreach (IWebElement distance in propertyDistances)
        {
          string distanceText = propertyDistances[0].Text.Trim().Substring(10);
          string distanceNumber = distanceText.Substring(0, distanceText.Length - 3);
          decimal d1 = Convert.ToDecimal(distanceNumber);
          int compareResult = Decimal.Compare(0, d1);
          Assert.IsTrue(compareResult != 0, "0 KM is displayed.");
        }

        //Click on any near by residence.
        string nearbyResidenceURL = findElement.WebElements(driver, By.CssSelector(".container a"))[0].GetAttribute("href");
        commonHelpers.ClickElementByJS(driver, ".container a:nth-child(1)");
        Thread.Sleep(3000);
        Assert.AreEqual(nearbyResidenceURL, driver.Url);

        driver.Navigate().Back();
        Thread.Sleep(3000);

        //Verify Click Here To Print link displays.
        IWebElement clickHereToPrintBtn = findElement.WebElement(driver, By.CssSelector("a[href='javascript:window.print()']"), 2);
        Assert.IsNotNull(clickHereToPrintBtn, "Click Here To Print link does not display.");

        //Verify click on Click Here To Print button opens print preview page.
        commonHelpers.ClickElementByJS(driver, "a[href=\"javascript:window.print()\"]");
        Thread.Sleep(3000);
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(printPreviewURL, driver1.Url);
        driver1.Quit();
      }
    }

    [TestMethod, Description("Test Case 4.1.8")]
    public void VerifyPhotosPage()
    {
      #region Variables
      string retirementResidencePhotosURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/photos";
      string longTermCareResidencePhotoURL = "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/photos";
      string propertyNameOnOverview = string.Empty;
      string addressOnOverview = string.Empty;
      string taglineOnOverview = string.Empty;
      #endregion

      //Get Property Name.
      propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

      //Get Address.
      addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

      //Get tagline.
      taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

      //Click on Photos link on Left Nav.
      findElement.WebElement(driver, By.Id("lnk-photos"), 2).Click();
      Thread.Sleep(3000);

      //Verify breadcrum displays.
      IList<IWebElement> breadcrumbs = findElement.WebElements(driver, By.ClassName("breadcrumb-item"));
      foreach (IWebElement breadcrumb in breadcrumbs)
      {
        Assert.IsNotNull(breadcrumb, breadcrumb.Text.Trim() + " does not display.");
      }

      if (propertyType == "RetirementResidence")
      {
        //Verify photos url
        Assert.AreEqual(retirementResidencePhotosURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Photos page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Photos page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Photos page for Retirement Residence.");

        //Verify images are displayed.
        IList<IWebElement> images = findElement.WebElements(driver, By.CssSelector(".col-md-6.col-lg-4.item.scale-on-hover img"));
        foreach (IWebElement image in images)
        {
          Assert.IsNotNull(image, "Some of the images are not displayed oh Photos page for Retirement Residence.");
        }

        //Verify Left Nav
        Assert.AreEqual("Overview", commonHelpers.GetTextByJS(driver, "#lnk-overview"), "Overview does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Photos", commonHelpers.GetTextByJS(driver, "#lnk-photos"), "Photos does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Virtual Tour", commonHelpers.GetTextByJS(driver, "#lnk-virtual-tour"), "Virtual Tour does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Service Levels", commonHelpers.GetTextByJS(driver, "#lnk-service-levels"), "Service Levels does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Dining Services", commonHelpers.GetTextByJS(driver, "#lnk-dining-services"), "Dinig Services does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Wellness Services", commonHelpers.GetTextByJS(driver, "#lnk-wellness-services"), "Wellness Services does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Activities", commonHelpers.GetTextByJS(driver, "#lnk-activities"), "Activities does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Amenities", commonHelpers.GetTextByJS(driver, "#lnk-amenities"), "Amenities does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Reviews & Ratings", commonHelpers.GetTextByJS(driver, "#lnk-reviews"), "Reviews & Ratings does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Map", commonHelpers.GetTextByJS(driver, "#lnk-map"), "Map does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Neighbourhood", commonHelpers.GetTextByJS(driver, "#lnk-neighbourhood"), "Neighbourhood does not display on Left Nav on Photos page for Retirement Residence.");
        Assert.AreEqual("Contact Us", commonHelpers.GetTextByJS(driver, "#lnk-contact"), "Contact Us does not display on Left Nav on Photos page for Retirement Residence.");

        //Verify Nearby Residency display.
        Assert.IsTrue((findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 img")).Count == 4) && (findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 a.panel.panel-info")).Count == 4), "There are less than four or none Nearby Retirement Residencies display on Photos page.");
      }
      else
      {
        //Verify photos url
        Assert.AreEqual(longTermCareResidencePhotoURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Photos page for Long Term Care Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Photos page for Long Term Care Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Photos page for Long Term Care Residence.");

        //Verify images are displayed.
        IList<IWebElement> images = findElement.WebElements(driver, By.CssSelector(".col-md-6.col-lg-4.item.scale-on-hover img"));
        foreach (IWebElement image in images)
        {
          Assert.IsNotNull(image, "Some of the images are not displayed oh Photos page for Long Term Care Residence.");
        }

        //Verify Left Nav
        Assert.AreEqual("Overview", commonHelpers.GetTextByJS(driver, "#lnk-overview"), "Overview does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Photos", commonHelpers.GetTextByJS(driver, "#lnk-photos"), "Photos does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Service Levels", commonHelpers.GetTextByJS(driver, "#lnk-service-levels"), "Service Levels does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Dining Services", commonHelpers.GetTextByJS(driver, "#lnk-dining-services"), "Dinig Services does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Wellness Services", commonHelpers.GetTextByJS(driver, "#lnk-wellness-services"), "Wellness Services does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Activities", commonHelpers.GetTextByJS(driver, "#lnk-activities"), "Activities does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Map", commonHelpers.GetTextByJS(driver, "#lnk-map"), "Map does not display on Left Nav on Photos page for Long Term Care Residence.");
        Assert.AreEqual("Contact Us", commonHelpers.GetTextByJS(driver, "#lnk-contact"), "Contact Us does not display on Left Nav on Photos page for Long Term Care Residence.");

        //Verify nearby residency display.
        Assert.IsTrue((findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 img")).Count == 4) && (findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 a.panel.panel-info")).Count == 4), "There are less than four or none Nearby Long Term Care Residencies display.");
      }
    }

    [TestMethod, Description("Test Cases 4.1.9, 4.1.10, 4.1.11")]
    public void VerifyNextPreviousAndCloseBtnsOnModal()
    {
      //Click on Photos link on Left Nav.
      findElement.WebElement(driver, By.Id("lnk-photos"), 2).Click();
      Thread.Sleep(5000);

      //Click on first photo.
      IList<IWebElement> listOfPhotos = findElement.WebElements(driver, By.CssSelector(".row.photoGallery a"));
      listOfPhotos[0].Click();
      Thread.Sleep(500);

      //Click on next button until last photo.
      for (int i = 0; i < listOfPhotos.Count; i++)
      {
        findElement.WebElement(driver, By.Id("next-button"), 2).Click();
      }
      for (int i = 0; i < listOfPhotos.Count; i++)
      {
        findElement.WebElement(driver, By.Id("previous-button"), 2).Click();
      }

      //Click on close button.
      findElement.WebElement(driver, By.Id("close-button"), 5).Click();
    }

    [TestMethod, Description("Test Cases 4.1.12, 4.1.13")]
    public void VerifyVirtualTourPage()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string virtualTourURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/virtual-tour";
        #endregion

        //Get Property Name.
        string propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

        //Get Address.
        string addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

        //Get tagline.
        string taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

        //Click on Photos link on Left Nav.
        findElement.WebElement(driver, By.Id("lnk-virtual-tour"), 2).Click();
        Thread.Sleep(3000);

        //Verify Virtual Tour page displays.
        Assert.AreEqual(virtualTourURL, driver.Url, "Virtual Tour page does not open.");

        //Verify breadcrum displays.
        IList<IWebElement> breadcrumbs = findElement.WebElements(driver, By.ClassName("breadcrumb-item"));
        foreach (IWebElement breadcrumb in breadcrumbs)
        {
          Assert.IsNotNull(breadcrumb, breadcrumb.Text.Trim() + " does not display.");
        }

        //Verify Youtube video display.
        Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("iframe[src='https://www.youtube.com/embed/JRlYuLSs3B8?rel=0&iv_load_policy=3&enablejsapi=1']"), 2), "Youtube video does not display on Virtual Tour page.");

        //Verify Left Nav
        Assert.AreEqual("Overview", commonHelpers.GetTextByJS(driver, "#lnk-overview"), "Overview does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Photos", commonHelpers.GetTextByJS(driver, "#lnk-photos"), "Photos does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Virtual Tour", commonHelpers.GetTextByJS(driver, "#lnk-virtual-tour"), "Virtual Tour does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Service Levels", commonHelpers.GetTextByJS(driver, "#lnk-service-levels"), "Service Levels does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Dining Services", commonHelpers.GetTextByJS(driver, "#lnk-dining-services"), "Dinig Services does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Wellness Services", commonHelpers.GetTextByJS(driver, "#lnk-wellness-services"), "Wellness Services does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Activities", commonHelpers.GetTextByJS(driver, "#lnk-activities"), "Activities does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Amenities", commonHelpers.GetTextByJS(driver, "#lnk-amenities"), "Amenities does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Reviews & Ratings", commonHelpers.GetTextByJS(driver, "#lnk-reviews"), "Reviews & Ratings does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Map", commonHelpers.GetTextByJS(driver, "#lnk-map"), "Map does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Neighbourhood", commonHelpers.GetTextByJS(driver, "#lnk-neighbourhood"), "Neighbourhood does not display on Left Nav on Overview page for Retirement Residence.");
        Assert.AreEqual("Contact Us", commonHelpers.GetTextByJS(driver, "#lnk-contact"), "Contact Us does not display on Left Nav on Overview page for Retirement Residence.");

        if (browserType == "Desktop")
        {
          //Verify Contact Form on Right Nav
          Assert.IsNotNull(findElement.WebElement(driver, By.Id("chartwellResidencesContactForm"), 2), "Contact Form does not display on Retirement Residency page.");
        }

        //Verify Nearby Residency display.
        Assert.IsTrue((findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 img")).Count == 4) && (findElement.WebElements(driver, By.CssSelector(".col-xs-12.col-sm-6.col-md-3 a.panel.panel-info")).Count == 4), "There are less than four or none Nearby Retirement Residencies display.");
      }
    }

    [TestMethod, Description("Test Cases 4.1.17, 4.1.18, 4.1.19, 4.1.20, 4.1.21, 4.1.22, 4.1.23")]
    public void VerifyServiceLevelsPage()
    {
      #region Variables
      string retirementResidenceServiceLevelsURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/service-levels";
      string longTermCareResidenceServiceLevelsURL = "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/service-levels";
      string propertyNameOnOverview = string.Empty;
      string addressOnOverview = string.Empty;
      string taglineOnOverview = string.Empty;
      string title1 = "Service Levels";
      string title2 = "Independent Supportive Living";
      string title3 = "Long Term Care";
      string para1 = "Chartwell offers a wide range of personal support services—delivered by friendly, dedicated staff—that can cater to your unique needs and lifestyle. The following care levels are offered at this residence. For questions regarding which care level may be the best fit for yourself or a loved one, please contact us.";
      string para2 = "Independent suites with the convenience of general household services such as dining, lifestyle programs and housekeeping but with the added availability of on-site personal care services such as medication administration or assistance with daily living activities. Care services can be offered by Chartwell staff or through government-funded home care agencies, depending on the jurisdiction and availability in each retirement residence.";
      string para3 = "Residences offering 24-hour nursing care, assistance with daily living activities and high levels of personal care. Suitable for residents with complex medical needs and/or advanced stages of Alzheimer’s or Dementia who require skilled nursing. Admission and funding is overseen by local government agencies in each province. Long Term Care homes, also known as Nursing Homes, Extended Care Homes or Residential Care Homes, offer both private or semi-private rooms and ward rooms. Private-pay Residential Care beds are available in the provinces of British Columbia and Quebec and offer an alternative to long waiting lists in government funded facilities.";
      string para4 = "All applications to Long Term Care Homes are coordinated by individual community Local Health Integration Networks (LHIN) who plan, integrate and fund local health care. You can learn more by visiting their website here or by locating a phone number for your local LHIN. We welcome the opportunity to offer personal visits as part of your decision making process. For general information on long term care in the province of Ontario, please click here.";
      string title1FR = "Services de soins";
      string title2FR = "Soins et services aux activités de la vie quotidienne";
      string title3FR = "Établissements de soins de longue durée";
      #endregion

      //Get Property Name.
      propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

      //Get Address.
      addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

      //Get tagline.
      taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

      //Click on Service Levels link on Left Nav.
      findElement.WebElement(driver, By.Id("lnk-service-levels"), 2).Click();
      Thread.Sleep(3000);

      if (propertyType == "RetirementResidence")
      {
        //Verify Service Levels url
        Assert.AreEqual(retirementResidenceServiceLevelsURL, driver.Url);
      }
      else
      {
        //Verify Service Levels url
        Assert.AreEqual(longTermCareResidenceServiceLevelsURL, driver.Url);
      }

      //Verify Property Name displays.
      Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Service Levels page.");

      //Verify Address.
      Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Service Levels page.");

      //Verify tagline.
      Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Service Levels page.");

      //Verify titles
      IList<IWebElement> titles = findElement.WebElements(driver, By.TagName("h2"));

      Assert.AreEqual(title1, titles[1].Text.Trim(), "Service Levels title does not display.");
      string actualPara1 = findElement.WebElements(driver, By.CssSelector("#mainRow p"))[0].Text.Trim();
      Assert.AreEqual(para1, actualPara1);
      if (propertyType == "RetirementResidence")
      {
        Assert.AreEqual(title2, titles[2].Text.Trim(), title2 + "  title does not display.");
        string actualPara2 = findElement.WebElements(driver, By.CssSelector("#mainRow p"))[1].Text.Trim();
        Assert.AreEqual(para2, actualPara2);
      }

      else
      {
        Assert.AreEqual(title3, titles[2].Text.Trim(), title3 + "  title does not display.");
        string actualPara3 = findElement.WebElements(driver, By.CssSelector("#mainRow p"))[2].Text.Trim();
        string actualPara4 = findElement.WebElements(driver, By.CssSelector("#mainRow p"))[3].Text.Trim();
        Assert.AreEqual(para3, actualPara3);
        Assert.AreEqual(para4, actualPara4);
      }

      //Verify image displays.
      Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Service Levels page.");

      //Verify Care option for FR
      string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";

      //Click on FR to change language to French.
      commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);
      Thread.Sleep(3000);

      //Verify titles
      titles = findElement.WebElements(driver, By.TagName("h2"));
      Assert.AreEqual(title1FR, titles[1].Text.Trim(), "Service Levels title does not display in FR.");

      if (propertyType == "RetirementResidence")
      {
        Assert.AreEqual(title2FR, titles[2].Text.Trim(), title2FR + "  title does not display in FR.");
      }
      else
      {
        Assert.AreEqual(title3FR, titles[2].Text.Trim(), title3FR + "  title does not display in FR.");
      }
    }

    [TestMethod, Description("Test Cases 4.1.24, 4.1.25")]
    public void VerifyDiningServicesPage()
    {
      #region Variables
      string retirementResidenceDiningServicesURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/dining-services";
      string longTermCareResidenceDiningServicesURL = "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/dining-services";
      string propertyNameOnOverview = string.Empty;
      string addressOnOverview = string.Empty;
      string taglineOnOverview = string.Empty;
      string genericDescriptionRetirementResidence = "Offering delicious and nutritious daily menus, our food service team exceeds expectations by crafting meals that cater to the tastes, preferences and dietary needs of residents, preparing everything from home-style favourites to themed meals to scrumptious desserts.";
      string genericDescriptionLongTermCareResidence = "Offering nutritious and delicious daily menus, our food service team prepares meals that are specially-designed to cater to the tastes, preferences and dietary needs of our residents. Meals may take place in dedicated dining rooms on each floor for the comfort and ease of long term care residents.";
      string retirementResidenceMenuURL = "https://chartwell.com/-/media/Files/dining-menus/chartwell-robert-speck-retirement-residence-dining-menu.pdf";
      string lTCMenuURL = "https://chartwell.com/-/media/Files/dining-menus/chartwell-long-term-care-dining-menu.pdf";
      #endregion

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;

      //Get Property Name.
      propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

      //Get Address.
      addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

      //Get tagline.
      taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

      //Click on Dining Services link on Left Nav.
      findElement.WebElement(driver, By.Id("lnk-dining-services"), 2).Click();
      Thread.Sleep(3000);

      if (propertyType == "RetirementResidence")
      {
        //Verify photos url
        Assert.AreEqual(retirementResidenceDiningServicesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Dining Services page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Dining Services page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Dining Services page for Retirement Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionRetirementResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description for Retirement Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Dining Services page for Retirement Residence.");

        //Verify food.
        IList<IWebElement> listOfFood = findElement.WebElements(driver, By.CssSelector(".treeList li"));
        Assert.IsTrue(listOfFood[0].Text.Trim() == "Country kitchen");
        Assert.IsTrue(listOfFood[1].Text.Trim() == "Diabetic diet");
        Assert.IsTrue(listOfFood[2].Text.Trim() == "Diversified menu");
        Assert.IsTrue(listOfFood[3].Text.Trim() == "Gluten-free diet");
        Assert.IsTrue(listOfFood[4].Text.Trim() == "Private dining room");
        Assert.IsTrue(listOfFood[5].Text.Trim() == "Special diets accommodated");
        Assert.IsTrue(listOfFood[6].Text.Trim() == "Two (2) meals included");
        Assert.IsTrue(listOfFood[7].Text.Trim() == "Vegetarian diet");

        //Verify Download Sample Dining Menu button.
        IWebElement downloadSampleDiningMenuBtnRetirement = findElement.WebElement(driver, By.CssSelector("a[href='/-/media/Files/dining-menus/chartwell-robert-speck-retirement-residence-dining-menu.pdf']"), 2);
        Assert.IsNotNull(downloadSampleDiningMenuBtnRetirement, "Download Sample Dining Menu button does not display on Dining Services page for Retirement Residence.");

        //Verify clicking on Download Sample Dining Menu button opens new tab.
        commonHelpers.ClickElementByJS(driver, "a[href=\"/-/media/Files/dining-menus/chartwell-robert-speck-retirement-residence-dining-menu.pdf\"]");
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(retirementResidenceMenuURL, driver1.Url);
        driver1.Close();
        driver.SwitchTo().Window(tabs[0]);
      }
      else
      {
        //Verify photos url
        Assert.AreEqual(longTermCareResidenceDiningServicesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Dining Services page for Long Term Care Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Dining Services page for Long Term Care Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Dining Services page for Long Term Care Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionLongTermCareResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description for Long Term Care Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Dining Services page for Long Term Care Residence.");

        //Verify Download Sample Dining Menu button.
        IWebElement downloadSampleDiningMenuBtnLTC = findElement.WebElement(driver, By.CssSelector("a[href='/-/media/Files/dining-menus/chartwell-long-term-care-dining-menu.pdf']"), 2);
        Assert.IsNotNull(downloadSampleDiningMenuBtnLTC, "Download Sample Dining Menu button does not display on Dining Services for Long Term Care Residence.");

        //Verify clicking on Download Sample Dining Menu button opens new tab.
        commonHelpers.ClickElementByJS(driver, "a[href=\"/-/media/Files/dining-menus/chartwell-long-term-care-dining-menu.pdf\"]");
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(lTCMenuURL, driver1.Url);
        driver1.Close();
        driver.SwitchTo().Window(tabs[0]);
      }
    }

    [TestMethod, Description("Test Case 4.1.26")]
    public void VerifyWellnessServicesPage()
    {
      #region Variables
      string retirementResidenceWellnessServicesURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/wellness-services";
      string longTermCareResidenceWellnessServicesURL = "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/wellness-services";
      string propertyNameOnOverview = string.Empty;
      string addressOnOverview = string.Empty;
      string taglineOnOverview = string.Empty;
      string genericDescriptionRetirementResidence = "In addition to convenient household services, our residents can take advantage of a host of personal support services geared to helping them maintain their health and independence with the peace of mind they deserve.";
      string genericDescriptionLongTermCareResidence = "Our residents benefit from access to 24-hour nursing care and a host of personal support services geared toward maintaining or improving their quality of life and positively benefitting their health and happiness. Our goal is to help provide residents and their families with the peace of mind they deserve.";
      #endregion

      //Get Property Name.
      propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

      //Get Address.
      addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

      //Get tagline.
      taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

      //Click on Dining Services link on Left Nav.
      findElement.WebElement(driver, By.Id("lnk-wellness-services"), 2).Click();
      Thread.Sleep(3000);

      if (propertyType == "RetirementResidence")
      {
        //Verify photos url
        Assert.AreEqual(retirementResidenceWellnessServicesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Wellness Services page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Wellness Services page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Wellness Services page for Retirement Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionRetirementResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description on Wellness Services page for Retirement Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Wellness Services page for Retirement Residence.");

        //Verify food.
        IList<IWebElement> listOfFood = findElement.WebElements(driver, By.CssSelector(".treeList li"));
        Assert.IsTrue(listOfFood[0].Text.Trim() == "Assistance with activities of daily living");
        Assert.IsTrue(listOfFood[1].Text.Trim() == "Audiology-hearing clinic");
        Assert.IsTrue(listOfFood[2].Text.Trim() == "Companionship");
        Assert.IsTrue(listOfFood[3].Text.Trim() == "Esthetician");
        Assert.IsTrue(listOfFood[4].Text.Trim() == "Individual service plans");
        Assert.IsTrue(listOfFood[5].Text.Trim() == "Medication supervision - administration");
        Assert.IsTrue(listOfFood[6].Text.Trim() == "On-call physician");
        Assert.IsTrue(listOfFood[7].Text.Trim() == "Oxygen service");
        Assert.IsTrue(listOfFood[8].Text.Trim() == "Pharmacy");
        Assert.IsTrue(listOfFood[9].Text.Trim() == "Physiotherapy");
        Assert.IsTrue(listOfFood[10].Text.Trim() == "Visiting dentist");
        Assert.IsTrue(listOfFood[11].Text.Trim() == "Vitals monitored");
        Assert.IsTrue(listOfFood[12].Text.Trim() == "Wheelchair accessible");
      }
      else
      {
        //Verify photos url
        Assert.AreEqual(longTermCareResidenceWellnessServicesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Wellness Services page for Long Term Care Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Wellness Services page for Long Term Care Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Wellness Services page for Long Term Care Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionLongTermCareResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description on Wellness Services page for Long Term Care Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Wellness Services page for Long Term Care Residence.");
      }
    }

    [TestMethod, Description("Test Cases 4.1.27, 4.1.28")]
    public void VerifyActivitiesPage()
    {
      #region Variables
      string retirementResidenceActivitesURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/activities";
      string longTermCareResidenceActivitiesURL = "https://chartwell.com/en/retirement-residences/chartwell-parkhill-long-term-care-residence/activities";
      string propertyNameOnOverview = string.Empty;
      string addressOnOverview = string.Empty;
      string taglineOnOverview = string.Empty;
      string genericDescriptionRetirementResidence = "To further enhance residents’ retirement living experience, many of our most popular programs, activities and social events take place on-site in our amenities, where they can pursue hobbies, socialize with friends, or simply sit back and relax.";
      string genericDescriptionLongTermCareResidence = "To further enhance residents’ daily experience, many of our most popular programs and social events take place on-site in our common spaces, where residents can benefit from activities specially-designed to reflect their individual interests and abilities.";
      string retirementResidenceActivityCalendarURL = "https://chartwell.com/-/media/Files/activity-calendars/chartwell-robert-speck-retirement-residence-activity-calendar.pdf";
      #endregion

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;

      //Get Property Name.
      propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

      //Get Address.
      addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

      //Get tagline.
      taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

      //Click on Dining Services link on Left Nav.
      commonHelpers.ClickElementByJS(driver, "#lnk-activities");
      Thread.Sleep(3000);

      if (propertyType == "RetirementResidence")
      {
        //Verify photos url
        Assert.AreEqual(retirementResidenceActivitesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Activities page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Activities page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Activities page for Retirement Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionRetirementResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description on Activities page for Retirement Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Activities page for Retirement Residence.");

        //Verify food.
        IList<IWebElement> listOfFood = findElement.WebElements(driver, By.CssSelector(".treeList li"));
        Assert.IsTrue(listOfFood[0].Text.Trim() == "Book Club");
        Assert.IsTrue(listOfFood[1].Text.Trim() == "Crafts");
        Assert.IsTrue(listOfFood[2].Text.Trim() == "Day trips");
        Assert.IsTrue(listOfFood[3].Text.Trim() == "Exercise Fitness facilities");
        Assert.IsTrue(listOfFood[4].Text.Trim() == "Garden Outdoor space");
        Assert.IsTrue(listOfFood[5].Text.Trim() == "Gardening");
        Assert.IsTrue(listOfFood[6].Text.Trim() == "Gym");
        Assert.IsTrue(listOfFood[7].Text.Trim() == "Library");
        Assert.IsTrue(listOfFood[8].Text.Trim() == "Movie Theatre");
        Assert.IsTrue(listOfFood[9].Text.Trim() == "Residents' computer room");
        Assert.IsTrue(listOfFood[10].Text.Trim() == "Social and recreational programs");

        //Verify Download Sample Dining Menu button.
        IWebElement downloadSampleActivityCalendarBtnRetirement = findElement.WebElement(driver, By.CssSelector("a[href='/-/media/Files/activity-calendars/chartwell-robert-speck-retirement-residence-activity-calendar.pdf']"), 2);
        Assert.IsNotNull(downloadSampleActivityCalendarBtnRetirement, "Download Sample Dining Menu button does not display on Dining Services page for Retirement Residence.");

        //Verify clicking on Download Sample Dining Menu button opens new tab.
        commonHelpers.ClickElementByJS(driver, "a[href=\"/-/media/Files/activity-calendars/chartwell-robert-speck-retirement-residence-activity-calendar.pdf\"]");
        tabs = new List<string>(driver.WindowHandles);
        driver1 = driver.SwitchTo().Window(tabs[1]);
        Assert.AreEqual(retirementResidenceActivityCalendarURL, driver1.Url);
        driver1.Close();
        driver.SwitchTo().Window(tabs[0]);
      }
      else
      {
        //Verify photos url
        Assert.AreEqual(longTermCareResidenceActivitiesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Activities page for Long Term Care Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Activities page for Long Term Care Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Activities page for Long Term Care Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionLongTermCareResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description on Activities page for Long Term Care Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Activities page for Long Term Care Residence.");
      }
    }

    [TestMethod, Description("Test Case 4.1.29")]
    public void VerifyAmenitiesPage()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string retirementResidenceActivitiesURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/amenities";
        string propertyNameOnOverview = string.Empty;
        string addressOnOverview = string.Empty;
        string taglineOnOverview = string.Empty;
        string genericDescriptionRetirementResidence = "Chartwell communities have a variety of onsite amenities that residents can enjoy when they aren’t relaxing in the privacy of their own suite. Every residence has a unique offering of common spaces that complement an active and fulfilling lifestyle.";
        #endregion

        //Get Property Name.
        propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

        //Get Address.
        addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

        //Get tagline.
        taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

        //Click on Dining Services link on Left Nav.
        commonHelpers.ClickElementByJS(driver, "#lnk-amenities");
        Thread.Sleep(3000);

        //Verify photos url
        Assert.AreEqual(retirementResidenceActivitiesURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Amenities page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Amenities page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Amenities page for Retirement Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionRetirementResidence, findElement.WebElement(driver, By.CssSelector("#mainRow p"), 2).Text.Trim(), "Different Generic Description on Amenities page for Retirement Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Amenities page for Retirement Residence.");

        //Verify food.
        IList<IWebElement> listOfFood = findElement.WebElements(driver, By.CssSelector(".treeList li"));
        Assert.IsTrue(listOfFood[0].Text.Trim() == "Barber-hairdresser");
        Assert.IsTrue(listOfFood[1].Text.Trim() == "Electricity");
        Assert.IsTrue(listOfFood[2].Text.Trim() == "Elevator");
        Assert.IsTrue(listOfFood[3].Text.Trim() == "Heating");
        Assert.IsTrue(listOfFood[4].Text.Trim() == "Housekeeping");
        Assert.IsTrue(listOfFood[5].Text.Trim() == "Indoor Parking");
        Assert.IsTrue(listOfFood[6].Text.Trim() == "ORCA member (Ontario)");
        Assert.IsTrue(listOfFood[7].Text.Trim() == "Personal emergency response system");
        Assert.IsTrue(listOfFood[8].Text.Trim() == "Pet friendly");
        Assert.IsTrue(listOfFood[9].Text.Trim() == "Scooter parking");
        Assert.IsTrue(listOfFood[10].Text.Trim() == "Security system");
        Assert.IsTrue(listOfFood[11].Text.Trim() == "Telephone");
      }
    }

    [TestMethod, Description("Test Case 4.1.30")]
    public void VerifyNeighbourhoodPage()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string retirementResidenceNeighbourURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/neighbourhood";
        string propertyNameOnOverview = string.Empty;
        string addressOnOverview = string.Empty;
        string taglineOnOverview = string.Empty;
        string genericDescriptionRetirementResidence = "Just a stone’s throw from Square One, Ontario’s largest shopping and dining mecca (with over 360 stores and services, including a new luxury wing), the Living Arts Centre (a cultural hub for theatre, art exhibits and festivals), as well as the Mississauga Civic Centre and library, Chartwell Robert Speck is at the centre of it all. Need a winter getaway? Pearson International Airport is a 20-minute drive. Visiting Toronto? Highways 401 and 403 are close by, as is public transit. Entertaining the grandchildren? Playdium, the emporium of fun, is minutes away. Local hospitals, including Trillium Health Centre and Credit Valley Hospital, are also less than a 10-minute drive away.";
        #endregion

        //Get Property Name.
        propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

        //Get Address.
        addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

        //Get tagline.
        taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

        //Click on Dining Services link on Left Nav.
        commonHelpers.ClickElementByJS(driver, "#lnk-neighbourhood");
        Thread.Sleep(3000);

        //Verify photos url
        Assert.AreEqual(retirementResidenceNeighbourURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Neighbourhood page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Neighbourhood page for Retirement Residence.");

        //Verify tagline.
        Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Neighbourhood page for Retirement Residence.");

        //Verify Generic Description.
        Assert.AreEqual(genericDescriptionRetirementResidence, findElement.WebElement(driver, By.CssSelector("#NeighbourhoodDescriptionContainer p:nth-child(2)"), 2).Text.Trim(), "Different Generic Description on Neighbourhood page for Retirement Residence display.");

        //Verify image displays.
        Assert.IsTrue(!string.IsNullOrEmpty(findElement.WebElement(driver, By.CssSelector(".moveUpArea img.large-image"), 2).GetAttribute("src")), "Image does not display on Neighbourhood page for Retirement Residence.");

        //Verify food.
        IList<IWebElement> listOfFood = findElement.WebElements(driver, By.CssSelector("#NeighbourhoodDescriptionContainer li"));
        Assert.IsTrue(listOfFood[0].Text.Trim() == "Square One Shopping Centre, including all major retailers, banks and restaurants");
        Assert.IsTrue(listOfFood[1].Text.Trim() == "Erin Mills Town Centre; Dixie Outlet Mall");
        Assert.IsTrue(listOfFood[2].Text.Trim() == "Living Arts Centre");
        Assert.IsTrue(listOfFood[3].Text.Trim() == "Mississauga Civic Centre, including Celebration Square");
        Assert.IsTrue(listOfFood[4].Text.Trim() == "Central Library");
        Assert.IsTrue(listOfFood[5].Text.Trim() == "Art Gallery of Mississauga");
        Assert.IsTrue(listOfFood[6].Text.Trim() == "Trillium Health Centre; Credit Valley Hospital");
        Assert.IsTrue(listOfFood[7].Text.Trim() == "Kariya Park, with a tranquil Japanese garden");
        Assert.IsTrue(listOfFood[8].Text.Trim() == "Sheridan College - Hazel McCallion campus");
        Assert.IsTrue(listOfFood[9].Text.Trim() == "Farmers' Market");
        Assert.IsTrue(listOfFood[10].Text.Trim() == "LCBO");
        Assert.IsTrue(listOfFood[11].Text.Trim() == "Places of worship");
      }
    }

    [TestMethod, Description("Test Cases 4.1.31, 4.1.32, 4.2.3, 4.2.4")]
    public void VerifyReviewsAndRatingsPage()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string retirementResidenceReviewsAndRatingsURL = "https://chartwell.com/en/retirement-residences/chartwell-robert-speck-retirement-residence/reviews";
        string propertyNameOnOverview = string.Empty;
        string addressOnOverview = string.Empty;
        //string taglineOnOverview = string.Empty;
        string lastStatement = "Chartwell encourages our stakeholders to review and rate our properties and services in full accordance with our Website Terms of Use.";
        #endregion

        //Get Property Name.
        propertyNameOnOverview = findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim();

        //Get Address.
        addressOnOverview = findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim();

        //Get tagline.
        //taglineOnOverview = findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim();

        //Click on Reviews & Ratings link on Left Nav.
        commonHelpers.ClickElementByJS(driver, "#lnk-reviews");
        Thread.Sleep(3000);

        //Verify photos url
        Assert.AreEqual(retirementResidenceReviewsAndRatingsURL, driver.Url);

        //Verify Property Name displays.
        Assert.AreEqual(propertyNameOnOverview, findElement.WebElement(driver, By.TagName("h1"), 2).Text.Trim(), "Different Property Name is displayed on Reviews And Ratings page for Retirement Residence.");

        //Verify Address.
        Assert.AreEqual(addressOnOverview, findElement.WebElement(driver, By.CssSelector("#mainRow address"), 2).Text.Trim(), "Different address is displayed on Reviews And Ratings page for Retirement Residence.");

        //Verify tagline.
        //Assert.AreEqual(taglineOnOverview, findElement.WebElement(driver, By.TagName("blockquote"), 3).Text.Trim(), "Different tagline is displayed on Reviews And Ratings page for Retirement Residence.");

        //Verify how many reviews, "Rreviews for this Residence - Overall Ratings -" average star rating
        IList<IWebElement> listOfElements = findElement.WebElements(driver, By.CssSelector("#ratingsForm h4 span"));
        string str1 = listOfElements[1].Text.Trim() + listOfElements[2].Text.Trim() + listOfElements[3].Text.Trim() + listOfElements[4].Text.Trim();
        Assert.IsTrue(str1.Contains("Reviews for this Residence-Overall Ratings"));

        //Verify 'Click to write a review for this residence' button.
        IWebElement clickToWriteReviewBtn = findElement.WebElement(driver, By.CssSelector("a[href = '#collapse1']"), 2);
        Assert.AreEqual("Click to write a review for this residence.", clickToWriteReviewBtn.Text.Trim());

        //Verify last statement.
        Assert.AreEqual(lastStatement, findElement.WebElement(driver, By.CssSelector("#ratingsForm p:last-child"), 10).Text.Trim());

        //Click on 'Click to write a review for this residence' button.
        clickToWriteReviewBtn.Click();

        //Verify review form.
        Assert.IsNotNull(findElement.WebElement(driver, By.Id("FirstName"), 2), "First Name field is not available in Review form.");
        Assert.IsNotNull(findElement.WebElement(driver, By.Id("LastName"), 2), "Last Name field is not available in Review form.");
        Assert.IsNotNull(findElement.WebElement(driver, By.Id("Email"), 2), "Email Id field is not available in Review form.");
        Assert.IsNotNull(findElement.WebElement(driver, By.CssSelector("button[type='submit']"), 2), "Add Comments button is not available in Review form.");
      }
    }
    #endregion

    #region Reviews & Ratings

    [TestMethod, Description("Test Cases 4.2.5, 4.2.6, 4.2.7")]
    public void VerifyRequiredFieldsAndErrorsOnChartwellReviewSection()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string ratingsError = "Ratings is required.";
        string firstNameError = "The First Name field is required";
        string lastNameError = "The Last Name field is required.";
        string emailError = "The Email field is required";
        string captchaError = "Please select the correct fruit";
        #endregion

        //Click on Reviews & Ratings link on Left Nav.
        commonHelpers.ClickElementByJS(driver, "#lnk-reviews");
        Thread.Sleep(3000);

        //Click on 'Click to write a review for this residence' button.
        IWebElement clickToWriteReviewBtn = findElement.WebElement(driver, By.CssSelector("a[href = '#collapse1']"), 2);
        clickToWriteReviewBtn.Click();

        //Click on Add Comment button.
        commonHelpers.ClickElementByJS(driver, "#collapse1 button");

        //Verify Error message.
        Assert.AreEqual(ratingsError, findElement.WebElement(driver, By.Id("Ratings-error"), 2).Text.Trim(), "Error does not display for empty Ratings.");
        Assert.AreEqual(firstNameError, findElement.WebElement(driver, By.Id("FirstName-error"), 2).Text.Trim(), "Error does not display for empty First Name.");
        Assert.AreEqual(lastNameError, findElement.WebElement(driver, By.Id("LastName-error"), 2).Text.Trim(), "Error does not display for empty Last Name.");
        Assert.AreEqual(emailError, findElement.WebElement(driver, By.Id("Email-error"), 2).Text.Trim(), "Error does not display for empty Email.");
        Assert.AreEqual(captchaError, commonHelpers.GetTextByJS(driver, "span[data-valmsg-for=\"CaptchaImagesList\"]").Trim(), "Error does not display for captcha.");

        //Enter First Name and Last Name
        findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");
        findElement.WebElement(driver, By.Id("LastName"), 2).SendKeys("LN");

        //Click on Add comment button.
        commonHelpers.ClickElementByJS(driver, "#collapse1 button");

        //Verify Error message.
        Assert.AreEqual(ratingsError, findElement.WebElement(driver, By.Id("Ratings-error"), 2).Text.Trim(), "Error does not display for empty Ratings.");
        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "Error message for First Name is displayed.");
        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#LastName-error"), "Error message for Last Name is displayed.");
        Assert.AreEqual(emailError, findElement.WebElement(driver, By.Id("Email-error"), 2).Text.Trim(), "Error does not display for empty Email.");
        Assert.AreEqual(captchaError, commonHelpers.GetTextByJS(driver, "span[data-valmsg-for=\"CaptchaImagesList\"]").Trim(), "Error does not display for captcha.");

        //Enter Email Address.
        findElement.WebElement(driver, By.Id("Email"), 2).SendKeys(testEmailRecipient);

        string captchaText = findElement.WebElement(driver, By.CssSelector(".contactRadioButtonRequiredRR"), 2).Text.Trim().Substring(18).Replace(System.Environment.NewLine, string.Empty);
        captchaText = captchaText.Substring(0, captchaText.Length - 67).ToLower();

        IList<IWebElement> listOfCaptcha = findElement.WebElements(driver, By.CssSelector(".contactRadioButtonRequiredRR input#CaptchaImagesList"));
        foreach (IWebElement captcha in listOfCaptcha)
        {
          if (captcha.GetAttribute("value") != captchaText)
          {
            string text = captcha.GetAttribute("value");
            commonHelpers.ClickElementByJS(driver, ".contactRadioButtonRequiredRR input#CaptchaImagesList[value=\"" + text + "\"]");
            break;
          }
        }

        //Click on Add comment button.
        commonHelpers.ClickElementByJS(driver, "#collapse1 button");

        Assert.AreEqual(captchaError, commonHelpers.GetTextByJS(driver, "span[data-valmsg-for=\"CaptchaImagesList\"]").Trim(), "Error does not display for captcha.");

        foreach (IWebElement captcha in listOfCaptcha)
        {
          if (captcha.GetAttribute("value") == captchaText)
          {
            commonHelpers.ClickElementByJS(driver, ".contactRadioButtonRequiredRR input#CaptchaImagesList[value=\"" + captchaText + "\"]");
            break;
          }
        }

        Assert.IsTrue(string.IsNullOrEmpty(commonHelpers.GetTextByJS(driver, "span[data-valmsg-for=\"CaptchaImagesList\"]")), "Error still displayed for captcha.");
      }
    }

    [TestMethod, Description("Test Cases 4.2.13")]
    public void VerifyReviewsInENAndFR()
    {
      if (propertyType == "RetirementResidence")
      {
        #region Variables
        string expectedReview = "Excellent staff! Very informative and full of compassion! I felt at home when we went to visit the home and feel that it is a comfortable choice for our aging parent.";
        #endregion
        //Click on Reviews & Ratings link on Left Nav.
        commonHelpers.ClickElementByJS(driver, "#lnk-reviews");
        Thread.Sleep(3000);

        string actualReview = findElement.WebElement(driver, By.CssSelector("#ratingsForm blockquote:last-child"), 10).Text.Trim();

        //Verify review for EN.
        Assert.AreEqual(expectedReview, actualReview, "The review is not as expected in EN.");

        //Switch to FR.
        if (browserType != "Desktop")
        {
          findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
          findElement.WebElement(driver, By.Id("lnkFrench"), 2).Click();
        }
        else
        {
          findElement.WebElement(driver, By.Id("lnkFrench"), 2).Click();
        }

        actualReview = findElement.WebElement(driver, By.CssSelector("#ratingsForm blockquote:last-child"), 10).Text.Trim();
        //Verify review for FR.
        Assert.AreEqual(expectedReview, actualReview, "The review is not as expected in FR.");
      }
    }
    #endregion

    //#region Property Contact Form
    //[TestMethod, Description("Test Cases 5.1.1, 5.1.2, 5.1.3, 5.1.4, 5.1.7, 5.1.8, 5.1.9, 7.8")]
    //public void VerifyPropertyContactForm()
    //{
    //    if (browserType != "Desktop")
    //    {
    //        findElement.WebElement(driver, By.Id("MobileFooterContactFormShowBtn"), 2).Click();
    //        Thread.Sleep(1000);
    //    }
    //    #region Variables
    //    string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.If you have submitted a request for a tour, we will contact you prior to your personalized visit to confirm the date and time.";
    //    string contactNoRR = "289-633-2153";
    //    string contactNoLTC = "519-294-6342";
    //    string firstNameError = "First Name is required";
    //    string emailError = "Email is required";
    //    string consentRadioBtnError = "Select Yes or No";
    //    string invalidEmailError = "Please enter a valid Email Address.";
    //    string invalidContactNoError = "Not a valid phone number";
    //    #endregion

    //    if (propertyType == "RetirementResidence")
    //    {
    //        //Verify Contact number for Retirement Residence.
    //        if (browserType == "Desktop")
    //        {
    //            Assert.AreEqual(contactNoRR, findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Retirement Residence does is not correct.");
    //        } else
    //        {
    //            Assert.AreEqual(contactNoRR, findElement.WebElement(driver, By.CssSelector("#MobileFooterContactFormPhoneNumber #phoneText"), 2).Text.Trim(), "The phone number displayed for Retirement Residence does is not correct.");
    //        }   

    //        //Click on Submit button.
    //        findElement.WebElement(driver, By.Id("PropertyContactUsFormSubmit"), 2).Click();
    //    }

    //    else
    //    {
    //        //Verify Contact number for Long Term Care
    //        if (browserType == "Desktop")
    //        {
    //            Assert.AreEqual(contactNoLTC, findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Long Term Care does is not correct.");
    //        }
    //        else
    //        {
    //            Assert.AreEqual(contactNoRR, findElement.WebElement(driver, By.CssSelector("#MobileFooterContactFormPhoneNumber #phoneText"), 2).Text.Trim(), "he phone number displayed for Long Term Care does is not correct.");
    //        }

    //        //Click on Submit button.
    //        findElement.WebElement(driver, By.Id("LTC_ContactFormSubmit"), 2).Click();
    //    }

    //    //Verify Error message for empty First Name field.
    //    Assert.AreEqual(firstNameError, findElement.WebElement(driver, By.Id("FirstName-error"), 1).Text, "Error message for empty First Name field does not show up.");

    //    //Verify Error message for empty Email Address field.
    //    Assert.AreEqual(emailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 1).Text, "Error message for empty Email Address field does not show up.");

    //    //Verify Error message for Consent Radio button.
    //    Assert.AreEqual(consentRadioBtnError, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 1).Text, "Error message for Consent Radio Button does not show up.");

    //    //Verify Error message for empty First Name field does not show up after adding data.
    //    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //    //Verify Error message for invalid Email Address.
    //    Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //    //Verify Error message for empty Email Address field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //    Thread.Sleep(2000);

    //    //Verify Error message for invalid phone number.
    //    Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //    //Verify Error message for invalid phone number does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //    if (propertyType == "RetirementResidence")
    //    {
    //        //Verify Calander opens.
    //        findElement.WebElement(driver, By.Id("visitdate"), 2).Click();
    //        Assert.IsTrue(commonHelpers.VerifyElementPresentByJS(driver, ".datepicker-days table"));
    //    }

    //    findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();

    //    //Verify Error message for Consent Radio button does not show up after selecting option.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //    if (propertyType == "RetirementResidence")
    //    {
    //        //Click on Submit button.
    //        findElement.WebElement(driver, By.Id("PropertyContactUsFormSubmit"), 2).Click();
    //    }
    //    else
    //    {
    //        //Click on Submit button.
    //        findElement.WebElement(driver, By.Id("LTC_ContactFormSubmit"), 2).Click();
    //    }
    //    Thread.Sleep(3000);

    //    //Verify thank you message.
    //    string actualThankYouText = findElement.WebElement(driver, By.CssSelector(".resFormConfirmation"), 2).Text.Trim().Replace(System.Environment.NewLine, string.Empty);
    //    Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");

    //}

    ////[TestMethod, Description("Test Case 5.1.2")]
    ////public void VerifyRequiredFieldsOnContactForm()
    ////{
    ////    string submitButtonId = (propertyType == "RetirementResidence") ? "PropertyContactUsFormSubmit" : "LTC_ContactFormSubmit";
    ////    findElement.WebElement(driver, By.Id(submitButtonId), 2).Click();

    ////    //Verify Error message for empty First Name field.
    ////    Assert.AreEqual("First Name is required", findElement.WebElement(driver, By.Id("FirstName-error"), 1).Text, "Error message for empty First Name field does not show up.");

    ////    //Verify Error message for empty Email Address field.
    ////    Assert.AreEqual("Email is required", findElement.WebElement(driver, By.Id("EmailAddress-error"), 1).Text, "Error message for empty Email Address field does not show up.");

    ////    //Verify Error message for Consent Radio button.
    ////    Assert.AreEqual("Select Yes or No", findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 1).Text, "Error message for Consent Radio Button does not show up.");
    ////}

    ////[TestMethod, Description("Test Case 5.1.3")]
    ////public void VerifyErrorMsgForRequiredFieldsGoneAfterAddingData()
    ////{
    ////    string submitButtonId = (propertyType == "RetirementResidence") ? "PropertyContactUsFormSubmit" : "LTC_ContactFormSubmit";
    ////    findElement.WebElement(driver, By.Id(submitButtonId), 2).Click();

    ////    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");

    ////    //Verify Error message for empty First Name field does not show up after adding data.
    ////    Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.Id("FirstName-error")), "The Error Message for empty First Name still displayed after adding data.");

    ////    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    ////    //Verify Error message for invalid Email Address.
    ////    Assert.AreEqual("Please enter a valid Email Address.", findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    ////    findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    ////    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    ////    //Verify Error message for empty Email Address field does not show up after adding data.
    ////    Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.Id("EmailAddress-error")), "The Error Message for empty Email Address still displayed after adding data.");

    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    ////    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);

    ////    //Verify Error message for invalid phone number.
    ////    Assert.AreEqual("Not a valid phone number", findElement.WebElement(driver, By.CssSelector("#ContactPhoneNo-error"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    ////    //Verify Error message for invalid phone number does not show up after adding data.
    ////    Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.Id("ContactPhoneNo-error")), "The Error Message for invalid phone number still displayed after adding data.");

    ////    findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();

    ////    //Verify Error message for Consent Radio button still shows up after selecting option.
    ////    Assert.IsFalse(commonHelpers.VerifyElementPresent(driver, By.Id("ConsentToConnect-error")), "The Error Message for Consent Radio button still displayed after selecting option.");
    ////}

    ////[TestMethod, Description("Test Case 5.1.4")]
    ////public void SubmitTheForm()
    ////{
    ////    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");
    ////    findElement.WebElement(driver, By.Id("LastName"), 2).SendKeys("testLN");
    ////    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);
    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    ////    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");
    ////    findElement.WebElement(driver, By.Id("ValidateTextBox"), 2).SendKeys("Test Question?");
    ////    findElement.WebElement(driver, By.CssSelector("#ConsentToConnect[value='false']"), 2).Click();
    ////    string submitButtonId = (propertyType == "RetirementResidence") ? "PropertyContactUsFormSubmit" : "LTC_ContactFormSubmit";
    ////    findElement.WebElement(driver, By.Id(submitButtonId), 2).Click();

    ////    //findElement.WebElement(driver, By.Id("PropertyContactUsFormSubmit"), 2).Click();

    ////    //Not implemented validation because
    ////    /*
    ////     Mail cannot be sent because of server problem:
    ////     Please Refresh the page and try again.
    ////    */
    ////}
    //#endregion

    //#region Photo Gallery Contact Form
    //[TestMethod, Description("Test Cases 5.2.1, 5.2.2, 5.2.3, 5.2.4")]
    //public void VerifyPhotoGalleryContactForm()
    //{
    //    if (browserType == "Desktop")
    //    {
    //        #region Variables
    //        string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.If you have submitted a request for a tour, we will contact you prior to your personalized visit to confirm the date and time.";
    //        string contactNoRR = "289-633-2153";
    //        string contactNoLTC = "519-294-6342";
    //        string firstNameError = "First Name is required";
    //        string emailError = "Email is required";
    //        string consentRadioBtnError = "Select Yes or No";
    //        string invalidEmailError = "Please enter a valid Email Address.";
    //        string invalidContactNoError = "Not a valid phone number";
    //        #endregion

    //        //Click on Photos link on Left Nav.
    //        findElement.WebElement(driver, By.Id("lnk-photos"), 2).Click();
    //        Thread.Sleep(3000);

    //        if (propertyType == "RetirementResidence")
    //        {
    //            //Verify Contact number for Retirement Residence.
    //            Assert.AreEqual(contactNoRR, findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Retirement Residence does is not correct.");

    //            //Click on Submit button.
    //            findElement.WebElement(driver, By.Id("PropertyContactUsFormSubmit"), 2).Click();
    //        }

    //        else
    //        {
    //            //Verify Contact number for Long Term Care
    //            Assert.AreEqual(contactNoLTC, findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Long Term Care does is not correct.");

    //            //Click on Submit button.
    //            findElement.WebElement(driver, By.Id("LTC_ContactFormSubmit"), 2).Click();
    //        }

    //        //Verify Error message for empty First Name field.
    //        Assert.AreEqual(firstNameError, findElement.WebElement(driver, By.Id("FirstName-error"), 1).Text, "Error message for empty First Name field does not show up.");

    //        //Verify Error message for empty Email Address field.
    //        Assert.AreEqual(emailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 1).Text, "Error message for empty Email Address field does not show up.");

    //        //Verify Error message for Consent Radio button.
    //        Assert.AreEqual(consentRadioBtnError, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 1).Text, "Error message for Consent Radio Button does not show up.");

    //        //Verify Error message for empty First Name field does not show up after adding data.
    //        findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //        //Verify Error message for invalid Email Address.
    //        Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //        //Verify Error message for empty Email Address field does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //        Thread.Sleep(2000);

    //        //Verify Error message for invalid phone number.
    //        Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //        //Verify Error message for invalid phone number does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();

    //        //Verify Error message for Consent Radio button does not show up after selecting option.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //        if (propertyType == "RetirementResidence")
    //        {
    //            //Click on Submit button.
    //            findElement.WebElement(driver, By.Id("PropertyContactUsFormSubmit"), 2).Click();
    //        }
    //        else
    //        {
    //            //Click on Submit button.
    //            findElement.WebElement(driver, By.Id("LTC_ContactFormSubmit"), 2).Click();
    //        }
    //        Thread.Sleep(3000);

    //        //Verify thank you message.
    //        string actualThankYouText = findElement.WebElement(driver, By.CssSelector(".resFormConfirmation"), 2).Text.Trim().Replace(System.Environment.NewLine, string.Empty);
    //        Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //    }
    //}
    //#endregion

    //#region General Contact Form
    //[TestMethod, Description("Test Cases 5.3.1, 5.3.2, 5.3.3, 5.3.4, 5.4.1")]
    //public void VerifyGeneralContactForm()
    //{
    //    #region Variables
    //    string contactNo = "1 855 461 0685";
    //    string errorFirstName = "First Name is required";
    //    string errorEmailAddress = "Email is required";
    //    string errorCity = "City is required";
    //    string errorConsentRadioBtn = "Select Yes or No";
    //    string invalidEmailError = "Please enter a valid Email Address.";
    //    string invalidContactNoError = "Not a valid phone number";
    //    string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.If you have submitted a request for a tour, we will contact you prior to your personalized visit to confirm the date and time.";
    //    #endregion
    //    if (browserType != "Desktop")
    //    {
    //        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
    //        Thread.Sleep(2000);
    //    }
    //    //Go to Static page.
    //    findElement.WebElement(driver, By.Id("MainNav-GettingStarted"), 2).Click();
    //    Thread.Sleep(2000);
    //    findElement.WebElement(driver, By.Id("MainNav-UnderstandingtheBenefits"), 2).Click();
    //    Thread.Sleep(3000);

    //    //Verify Contact number is displayed.
    //    if (browserType == "Desktop")
    //    {
    //        Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("#chartwellContactForm h3 a.phoneNumber"), 2).Text.Trim(), "Contact Number is not displayed.");
    //    } else
    //    {
    //        Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("#MobileFooterContactFormPhoneNumber #phoneText"), 2).Text.Trim(), "Contact Number is not displayed.");

    //        findElement.WebElement(driver, By.Id("MobileFooterContactFormShowBtn"), 2).Click();
    //        Thread.Sleep(1000);
    //    }

    //    //Click on Submit button.
    //    commonHelpers.ClickElementByJS(driver, "#GeneralContactSubmit");

    //    //Verify error for blank First Name field.
    //    Assert.AreEqual(errorFirstName, findElement.WebElement(driver, By.Id("FirstName-error"), 2).Text.Trim(), "Error for blank First Name field does not show up.");

    //    //Verify error for blank Email Address field.
    //    Assert.AreEqual(errorEmailAddress, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text.Trim(), "Error for blank Email Address field does not show up.");

    //    //Verify error for blank City field.
    //    Assert.AreEqual(errorCity, findElement.WebElement(driver, By.Id("ContactCity-error"), 2).Text.Trim(), "Error for blank City field does not show up.");

    //    //Verify error for unselected Consent Radio button field.
    //    Assert.AreEqual(errorConsentRadioBtn, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 2).Text.Trim(), "Error for unselected consent radio button does not show up.");

    //    //Enter First Name
    //    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("Test");

    //    //Verify there is no error showing up for First Name field.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //    //Enter invalid Email Address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //    //Verify Error message for invalid Email Address.
    //    Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //    //Enter valid email address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //    //Verify Error message for empty Email Address field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //    //Enter City.
    //    findElement.WebElement(driver, By.Id("ContactCity"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("ContactCity"), 2).SendKeys("Mississauga");

    //    //Verify Error message for empty City field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactCity-error"), "The Error Message for empty City still displayed after adding data.");


    //    //Enter invalid phone number
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //    Thread.Sleep(2000);

    //    //Verify Error message for invalid phone number.
    //    Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //    //Verify Error message for phone number does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //    //findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();
    //    commonHelpers.ClickElementByJS(driver, "#ConsentToConnect");

    //    //Verify Error message for Consent Radio button does not show up after selecting option.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //    //Commented Submit code.
    //    commonHelpers.ClickElementByJS(driver, "#GeneralContactSubmit");
    //    Thread.Sleep(3000);

    //    //Verify thank you message.
    //    string actualThankYouText = commonHelpers.GetTextByJS(driver, "#GeneralContactMsg").Trim().Replace(System.Environment.NewLine, string.Empty);
    //    Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //}
    //#endregion

    //#region Corporate Contact Form
    //[TestMethod, Description("Test Cases 5.4.1, 5.4.4")]
    //public void VerifyCorporateContactForm()
    //{
    //    #region Variables
    //    string contactNo = "1-855-461-0685";
    //    string errorFirstName = "First Name is required";
    //    string errorEmailAddress = "Email is required";
    //    string errorConsentRadioBtn = "Select Yes or No";
    //    string invalidEmailError = "Please enter a valid Email Address.";
    //    string invalidContactNoError = "Not a valid phone number";
    //    string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.Do you have questions? Call 1-855-461-0685 today to speak with a Chartwell representative who can help.";
    //    #endregion
    //    if (browserType != "Desktop")
    //    {
    //        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
    //        Thread.Sleep(2000);
    //    }
    //    //Go to Contact page.
    //    findElement.WebElement(driver, By.Id("MainNav-Contact"), 2).Click();

    //    //Verify Contact number is displayed.
    //    Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("#mainBlock h2 a"), 2).Text.Trim(), "Contact Number is not displayed.");

    //    //Click on Submit button.
    //    commonHelpers.ClickElementByJS(driver, "#CorporateContactSubmit");

    //    //Verify error for blank First Name field.
    //    Assert.AreEqual(errorFirstName, findElement.WebElement(driver, By.Id("FirstName-error"), 2).Text.Trim(), "Error for blank First Name field does not show up.");

    //    //Verify error for blank Email Address field.
    //    Assert.AreEqual(errorEmailAddress, findElement.WebElement(driver, By.Id("EMailAddress-error"), 2).Text.Trim(), "Error for blank Email Address field does not show up.");

    //    //Verify error for unselected Consent Radio button field.
    //    Assert.AreEqual(errorConsentRadioBtn, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 2).Text.Trim(), "Error for unselected consent radio button does not show up.");

    //    //Select Subject from dropdown.
    //    findElement.WebElement(driver, By.Id("Subject"), 2).Click();
    //    Thread.Sleep(1000);
    //    findElement.WebElement(driver, By.CssSelector("#Subject option[value='6']"), 2).Click();

    //    //Enter First Name
    //    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("Test");

    //    //Verify there is no error showing up for First Name field.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //    //Enter invalid Email Address.
    //    findElement.WebElement(driver, By.Id("EMailAddress"), 2).SendKeys("test");

    //    //Verify Error message for invalid Email Address.
    //    Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EMailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //    //Enter valid email address.
    //    findElement.WebElement(driver, By.Id("EMailAddress"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("EMailAddress"), 2).SendKeys(testEmailRecipient);

    //    //Verify Error message for empty Email Address field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EMailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //    //Enter invalid phone number
    //    findElement.WebElement(driver, By.Id("PhoneNo"), 2).SendKeys("321");
    //    findElement.WebElement(driver, By.Id("EMailAddress"), 2).SendKeys(Keys.Enter);
    //    Thread.Sleep(2000);

    //    //Verify Error message for invalid phone number.
    //    Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='PhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //    findElement.WebElement(driver, By.Id("PhoneNo"), 2).SendKeys("456");
    //    findElement.WebElement(driver, By.Id("PhoneNo"), 2).SendKeys("7891");

    //    //Verify Error message for phone number does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //    //findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();
    //    commonHelpers.ClickElementByJS(driver, "#ConsentToConnect");

    //    //Verify Error message for Consent Radio button does not show up after selecting option.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //    //Commented Submit code.
    //    commonHelpers.ClickElementByJS(driver, "#CorporateContactSubmit");
    //    Thread.Sleep(7000);

    //    //Verify thank you message.
    //    string actualThankYouText = commonHelpers.GetTextByJS(driver, "#SubmitMsg").Trim().Replace(System.Environment.NewLine, string.Empty);
    //    Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //}
    //#endregion

    //#region Regional
    //[TestMethod, Description("Test Cases 5.5.1, 5.5.2, 5.5.3, 5.5.4, 5.5.5")]
    //public void VerifyRegionalContactFormEN()
    //{
    //    #region Variables
    //    string url = "https://chartwell.com/regional/retirement-living-in-ottawa";
    //    string contactNo = "613-416-7862";
    //    string errorFirstName = "First Name is required";
    //    string errorEmailAddress = "Email is required";
    //    string errorConsentRadioBtn = "Select Yes or No";
    //    string invalidEmailError = "Please enter a valid Email Address.";
    //    string invalidContactNoError = "Not a valid phone number";
    //    string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.Do you have questions? Call 1-855-461-0685 today to speak with a Chartwell representative who can help.";
    //    #endregion

    //    driver.Navigate().GoToUrl(url);

    //    string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";
    //    //Click on FR to change language to French.
    //    commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);

    //    //Verify Contact number is displayed.
    //    Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("#RegionalContactForm h3 a"), 2).Text.Trim(), "Contact Number is not displayed.");

    //    //Click on Submit button.
    //    commonHelpers.ClickElementByJS(driver, "#RegionalContactSubmit");

    //    //Verify error for blank First Name field.
    //    Assert.AreEqual(errorFirstName, findElement.WebElement(driver, By.Id("FirstName-error"), 2).Text.Trim(), "Error for blank First Name field does not show up.");

    //    //Verify error for blank Email Address field.
    //    Assert.AreEqual(errorEmailAddress, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text.Trim(), "Error for blank Email Address field does not show up.");

    //    //Verify error for unselected Consent Radio button field.
    //    Assert.AreEqual(errorConsentRadioBtn, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 2).Text.Trim(), "Error for unselected consent radio button does not show up.");

    //    //Enter First Name
    //    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("Test");

    //    //Verify there is no error showing up for First Name field.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //    //Enter invalid Email Address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //    //Verify Error message for invalid Email Address.
    //    Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //    //Enter valid email address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //    //Verify Error message for empty Email Address field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //    //Enter invalid phone number
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //    Thread.Sleep(2000);

    //    //Verify Error message for invalid phone number.
    //    Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //    //Verify Error message for phone number does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //    //findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();
    //    commonHelpers.ClickElementByJS(driver, "#ConsentToConnect");

    //    //Verify Error message for Consent Radio button does not show up after selecting option.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //    commonHelpers.ClickElementByJS(driver, "#RegionalContactSubmit");
    //    Thread.Sleep(7000);

    //    //Verify thank you message.
    //    string actualThankYouText = commonHelpers.GetTextByJS(driver, "#SubmitMsg").Trim().Replace(System.Environment.NewLine, string.Empty);
    //    Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //}

    //[TestMethod, Description("Test Cases 5.5.1, 5.5.2, 5.5.3, 5.5.4, 5.5.5")]
    //public void VerifyRegionalContactFormFR()
    //{
    //    #region Variables
    //    string url = "https://chartwell.com/regional/retirement-living-in-ottawa";
    //    string contactNo = "613-416-7862";
    //    string errorFirstName = "Le prénom est requis";
    //    string errorEmailAddress = "Le courriel est requis";
    //    string errorConsentRadioBtn = "Sélectionnez Oui ou Non";
    //    string invalidEmailError = "S'il vous plaît, mettez une adresse email valide.";
    //    string invalidContactNoError = "Numéro de téléphone non valide";
    //    string expectedThankyouText = "";
    //    #endregion

    //    driver.Navigate().GoToUrl(url);

    //    string browser = (browserType == "Desktop") ? "Desktop" : "Mobile";
    //    //Click on FR to change language to French.
    //    commonHelpers.SwitchBetweenENAndFR(driver, browser, CommonHelpers.Language.French);

    //    //Verify Contact number is displayed.
    //    Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("#RegionalContactForm h3 a"), 2).Text.Trim(), "Contact Number is not displayed.");

    //    //Click on Submit button.
    //    commonHelpers.ClickElementByJS(driver, "#RegionalContactSubmit");

    //    //Verify error for blank First Name field.
    //    Assert.AreEqual(errorFirstName, findElement.WebElement(driver, By.Id("FirstName-error"), 2).Text.Trim(), "Error for blank First Name field does not show up.");

    //    //Verify error for blank Email Address field.
    //    Assert.AreEqual(errorEmailAddress, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text.Trim(), "Error for blank Email Address field does not show up.");

    //    //Verify error for unselected Consent Radio button field.
    //    Assert.AreEqual(errorConsentRadioBtn, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 2).Text.Trim(), "Error for unselected consent radio button does not show up.");

    //    //Enter First Name
    //    findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("Test");

    //    //Verify there is no error showing up for First Name field.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //    //Enter invalid Email Address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //    //Verify Error message for invalid Email Address.
    //    Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //    //Enter valid email address.
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //    //Verify Error message for empty Email Address field does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //    //Enter invalid phone number
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //    findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //    Thread.Sleep(2000);

    //    //Verify Error message for invalid phone number.
    //    Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //    findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //    //Verify Error message for phone number does not show up after adding data.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //    //findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();
    //    commonHelpers.ClickElementByJS(driver, "#ConsentToConnect");

    //    //Verify Error message for Consent Radio button does not show up after selecting option.
    //    Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //    commonHelpers.ClickElementByJS(driver, "#RegionalContactSubmit");
    //    Thread.Sleep(7000);

    //    //Verify thank you message.
    //    string actualThankYouText = commonHelpers.GetTextByJS(driver, "#SubmitMsg").Trim().Replace(System.Environment.NewLine, string.Empty);
    //    Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //}
    //#endregion

    //#region Splitter Contact Form
    //[TestMethod, Description("Test Cases 5.6.1, 5.6.2, 5.6.3, 5.6.4, 5.6.5")]
    //public void VerifySplitterContactForm()
    //{
    //    if (browserType == "Desktop")
    //    {
    //        #region Variables
    //        string partOfResidenceName = "Waterford";
    //        string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.If you have submitted a request for a tour, we will contact you prior to your personalized visit to confirm the date and time.";
    //        string contactNo = "289-644-2950";
    //        string firstNameError = "First Name is required";
    //        string emailError = "Email is required";
    //        string consentRadioBtnError = "Select Yes or No";
    //        string invalidEmailError = "Please enter a valid Email Address.";
    //        string invalidContactNoError = "Not a valid phone number";
    //        #endregion

    //        //Enter Paft of Residence Name in Residence Field.
    //        commonHelpers.EnterResidency(driver, partOfResidenceName);

    //        //Hit Enter.
    //        findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(Keys.Enter);

    //        //Verify Contact number for Retirement Residence.
    //        Assert.AreEqual(contactNo, findElement.WebElement(driver, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Retirement Residence does is not correct.");

    //        commonHelpers.ClickElementByJS(driver, "#PropertyContactUsFormSubmit");

    //        //Verify Error message for empty First Name field.
    //        Assert.AreEqual(firstNameError, findElement.WebElement(driver, By.Id("FirstName-error"), 1).Text, "Error message for empty First Name field does not show up.");

    //        //Verify Error message for empty Email Address field.
    //        Assert.AreEqual(emailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 1).Text, "Error message for empty Email Address field does not show up.");

    //        //Verify Error message for Consent Radio button.
    //        Assert.AreEqual(consentRadioBtnError, findElement.WebElement(driver, By.Id("ConsentToConnect-error"), 1).Text, "Error message for Consent Radio Button does not show up.");

    //        //Verify Error message for empty First Name field does not show up after adding data.
    //        findElement.WebElement(driver, By.Id("FirstName"), 2).SendKeys("test");
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys("test");

    //        //Verify Error message for invalid Email Address.
    //        Assert.AreEqual(invalidEmailError, findElement.WebElement(driver, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).Clear();
    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //        //Verify Error message for empty Email Address field does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //        findElement.WebElement(driver, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //        Thread.Sleep(2000);

    //        //Verify Error message for invalid phone number.
    //        Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //        findElement.WebElement(driver, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //        //Verify Error message for phone number does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //        findElement.WebElement(driver, By.Id("ConsentToConnect"), 2).Click();

    //        //Verify Error message for Consent Radio button does not show up after selecting option.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //        //Commented Submit code.
    //        commonHelpers.ClickElementByJS(driver, "#GeneralContactSubmit");
    //        Thread.Sleep(3000);

    //        //Verify thank you message.
    //        string actualThankYouText = findElement.WebElement(driver, By.CssSelector(".resFormConfirmation"), 2).Text.Trim().Replace(System.Environment.NewLine, string.Empty);
    //        Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");
    //    }
    //}
    //#endregion

    //#region Blog Contact Form
    //[TestMethod, Description("Test Cases 5.7.1, 5.7.2, 5.7.3, 5.7.4")]
    //public void VerifyBlogContactForm()
    //{
    //    if (browserType == "Desktop")
    //    {
    //        #region Variables
    //        string expectedThankyouText = "Thank you for your interest in Chartwell.A member of our team will be in touch with you shortly, usually within one business day.If you have submitted a request for a tour, we will contact you prior to your personalized visit to confirm the date and time.";
    //        string contactNo = "1 855 461 0685";
    //        string firstNameError = "First Name is required";
    //        string emailError = "Email is required";
    //        string cityError = "City is required";
    //        string consentRadioBtnError = "Select Yes or No";
    //        string invalidEmailError = "Please enter a valid Email Address.";
    //        string invalidContactNoError = "Not a valid phone number";
    //        #endregion

    //        //Click on Photos link on Left Nav.
    //        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();
    //        Thread.Sleep(3000);

    //        List<string> tabs = new List<string>(driver.WindowHandles);
    //        IWebDriver driver1;
    //        tabs = new List<string>(driver.WindowHandles);
    //        driver1 = driver.SwitchTo().Window(tabs[1]);

    //        //Verify Contact number for Retirement Residence.
    //        Assert.AreEqual(contactNo, findElement.WebElement(driver1, By.CssSelector("a.phoneNumber"), 5).Text, "The phone number displayed for Retirement Residence does is not correct.");

    //        commonHelpers.ClickElementByJS(driver, "#GeneralContactSubmit");

    //        //Verify Error message for empty First Name field.
    //        Assert.AreEqual(firstNameError, findElement.WebElement(driver1, By.Id("FirstName-error"), 1).Text, "Error message for empty First Name field does not show up.");

    //        //Verify Error message for empty Email Address field.
    //        Assert.AreEqual(emailError, findElement.WebElement(driver1, By.Id("EmailAddress-error"), 1).Text, "Error message for empty Email Address field does not show up.");

    //        //Verify Error message for city field.
    //        Assert.AreEqual(cityError, findElement.WebElement(driver, By.Id("ContactCity-error"), 1).Text, "Error message for empty City field does not show up.");

    //        //Verify Error message for Consent Radio button.
    //        Assert.AreEqual(consentRadioBtnError, findElement.WebElement(driver1, By.Id("ConsentToConnect-error"), 1).Text, "Error message for Consent Radio Button does not show up.");

    //        //Verify Error message for empty First Name field does not show up after adding data.
    //        findElement.WebElement(driver1, By.Id("FirstName"), 2).SendKeys("test");
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver1, "#FirstName-error"), "The Error Message for empty First Name still displayed after adding data.");

    //        findElement.WebElement(driver1, By.Id("EmailAddress"), 2).SendKeys("asdf2w5423");

    //        //Verify Error message for invalid Email Address.
    //        Assert.AreEqual(invalidEmailError, findElement.WebElement(driver1, By.Id("EmailAddress-error"), 2).Text, "The correct Error Message for invalid email address does not displayed.");

    //        findElement.WebElement(driver1, By.Id("EmailAddress"), 2).Clear();
    //        findElement.WebElement(driver1, By.Id("EmailAddress"), 2).SendKeys(testEmailRecipient);

    //        //Verify Error message for empty Email Address field does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver1, "#EmailAddress-error"), "The Error Message for empty Email Address still displayed after adding data.");

    //        //Enter City
    //        findElement.WebElement(driver, By.Id("ContactCity"), 2).SendKeys("Brampton");
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver, "#ContactCity-error"), "The Error Message for empty City field still displayed after adding data.");

    //        findElement.WebElement(driver1, By.Id("ContactPhoneNo"), 2).SendKeys("321");
    //        findElement.WebElement(driver1, By.Id("EmailAddress"), 2).SendKeys(Keys.Enter);
    //        Thread.Sleep(2000);

    //        //Verify Error message for invalid phone number.
    //        Assert.AreEqual(invalidContactNoError, findElement.WebElement(driver, By.CssSelector("span[data-valmsg-for='ContactPhoneNo']"), 2).Text, "The Error Message for invalid phone number does not displayed.");

    //        findElement.WebElement(driver1, By.Id("ContactPhoneNo"), 2).SendKeys("456");
    //        findElement.WebElement(driver1, By.Id("ContactPhoneNo"), 2).SendKeys("7891");

    //        //Verify Error message for phone number does not show up after adding data.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver1, "#ContactPhoneNo-error"), "The Error Message for invalid phone number still displayed after adding data.");

    //        findElement.WebElement(driver1, By.Id("ConsentToConnect"), 2).Click();

    //        //Verify Error message for Consent Radio button does not show up after selecting option.
    //        Assert.IsFalse(commonHelpers.VerifyElementPresentByJS(driver1, "#ConsentToConnect-error"), "The Error Message for Consent Radio button still displayed after selecting option.");

    //        //Commented Submit code.
    //        commonHelpers.ClickElementByJS(driver, "#GeneralContactSubmit");
    //        Thread.Sleep(3000);

    //        //Verify thank you message.
    //        string actualThankYouText = findElement.WebElement(driver1, By.CssSelector(".resFormConfirmation"), 2).Text.Trim().Replace(System.Environment.NewLine, string.Empty);
    //        Assert.AreEqual(expectedThankyouText, actualThankYouText, "Thank you text does not display.");

    //        driver1.Quit();
    //    }
    //}
    //#endregion

    #region Chartwell Blog
    [TestMethod, Description("Test Cases 6.1.1, 6.1.2")]
    public void VerifyBlogOnHomepage()
    {
      if (browserType == "Desktop")
      {
        //Go to home page.
        findElement.WebElement(driver, By.CssSelector("img[src='/Assets/Images/Layout/chart-logo-2014.png']"), 2).Click();
      }
      else
      {
        findElement.WebElement(driver, By.Id("logo"), 2).Click();
      }
      Thread.Sleep(5000);

      //Verify number of blogs.
      IList<IWebElement> listOfBlogs = findElement.WebElements(driver, By.CssSelector("#cwBlogRecentPosts a"));
      Assert.IsTrue(listOfBlogs.Count >= 3, "There are less than 3 blogs display.");

      string url = listOfBlogs[0].GetAttribute("href");

      //Click on any blog.
      commonHelpers.ClickElementByJS(driver, "#cwBlogRecentPosts a:nth-child(1)");

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      //Verify blog is opened in different tab.
      Assert.AreEqual(url, driver1.Url);
      driver1.Quit();
    }
    #endregion

    #region Category, Tags, and Search
    [TestMethod, Description("Test Cases 6.2.1")]
    public void VerifyBlogCategory()
    {
      if (browserType == "Desktop")
      {
        //Click on Blog menu.
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();
      }
      else
      {
        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
        Thread.Sleep(2000);
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();

      }
      Thread.Sleep(5000);

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      if (browserType != "Desktop")
      {
        findElement.WebElement(driver1, By.CssSelector("#MobileBlogHeader button"), 2).Click();
        Thread.Sleep(2000);
      }

      //Click on Advice For Seniors menu.
      findElement.WebElement(driver1, By.Id("cwb-Advice for Seniors"), 2).Click();
      Thread.Sleep(10000);

      IList<IWebElement> allBlogs = findElement.WebElements(driver1, By.CssSelector(".categories"));

      foreach (IWebElement blog in allBlogs)
      {
        Assert.IsTrue(blog.Text.Contains("ADVICE FOR SENIORS"));
      }
    }

    [TestMethod, Description("Test Cases 6.2.4, 7.9")]
    public void VerifySearchBlog()
    {
      if (browserType == "Desktop")
      {
        //Click on Blog menu.
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();
      }
      else
      {
        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
        Thread.Sleep(2000);
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();

      }
      Thread.Sleep(5000);

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      //verify we get results for valid search
      findElement.WebElement(driver1, By.Id("txtSearch"), 2).SendKeys("Active Living");
      findElement.WebElement(driver1, By.Id("blogSearchIcon"), 2).Click();
      Thread.Sleep(10000);
      IList<IWebElement> allBlogs = findElement.WebElements(driver1, By.CssSelector(".cwbPostsList .searchListingPost"));
      Assert.IsTrue(allBlogs.Count > 0);


      //verify we get zero results for garbage search
      findElement.WebElement(driver1, By.Id("txtSearch"), 2).SendKeys("asdf#Asdfasdr3d");
      findElement.WebElement(driver1, By.Id("blogSearchIcon"), 2).Click();
      Thread.Sleep(10000);
      allBlogs = findElement.WebElements(driver1, By.CssSelector(".cwbPostsList .searchListingPost"));
      Assert.IsTrue(allBlogs.Count == 0);
    }

    [TestMethod, Description("Test Case 6.2.2")]
    public void VerifyBlogTags()
    {
      if (browserType == "Desktop")
      {
        //Click on Blog menu.
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();
      }
      else
      {
        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
        Thread.Sleep(2000);
        findElement.WebElement(driver, By.Id("MainNav-Blog"), 2).Click();
      }
      Thread.Sleep(5000);

      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);

      if (browserType != "Desktop")
      {
        findElement.WebElement(driver1, By.CssSelector("#MobileBlogHeader button"), 2).Click();
        Thread.Sleep(2000);
      }

      //Click on the latest blog.
      IList<IWebElement> listOfBlogs = driver1.FindElements(By.CssSelector(".cwbListPost.cwbCentrePost"));
      listOfBlogs[0].Click();
      Thread.Sleep(10000);

      //Click on first tag of the latest blog.
      IList<IWebElement> listOfTags = findElement.WebElements(driver1, By.CssSelector(".articleTags a"));
      string expectedTag = listOfTags[0].Text;
      listOfTags[0].Click();
      Thread.Sleep(10000);

      //Click on other blog of the same tag.
      listOfBlogs = driver1.FindElements(By.CssSelector(".cwbListPost.cwbCentrePost"));
      if (listOfBlogs.Count > 1)
      {
        listOfBlogs[1].Click();
      }
      else
      {
        listOfBlogs[0].Click();
      }
      Thread.Sleep(10000);

      //Verify the other blog has the same tag.
      listOfTags = findElement.WebElements(driver1, By.CssSelector(".articleTags a"));
      Assert.IsTrue(listOfTags.Any(t => t.Text.Contains(expectedTag)));
    }
    #endregion

    #region Careers
    [TestMethod, Description("Test Case 9.2")]
    public void VerifyCareersLink()
    {
      #region Variables
      string expectedURL = "https://careersatchartwell.com/";
      #endregion

      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.Id("mobileNavToggle"), 2).Click();
        Thread.Sleep(2000);
      }

      //Click on Careers link.
      findElement.WebElement(driver, By.CssSelector("a[href='https://careersatchartwell.com/explore']"), 2).Click();

      //Verify careers page is opened.
      List<string> tabs = new List<string>(driver.WindowHandles);
      IWebDriver driver1;
      tabs = new List<string>(driver.WindowHandles);
      driver1 = driver.SwitchTo().Window(tabs[1]);
      Thread.Sleep(5000);
      Assert.AreEqual(expectedURL, driver1.Url);
    }
    #endregion

    #region //Private Methods
    private List<string> City(string cityName, string propertyType)
    {
      _ = new List<string>();
      List<string> cities;

      if (propertyType == "RetirementResidence")
        switch (cityName)
        {
          case "Toronto":
            cities = new List<string>() { "Toronto", "Etobicoke", "Thornhill", "North York" };
            return cities;
          case "Brampton":
            cities = new List<string>() { "Toronto", "Mississauga", "Etobicoke", "Woodbridge" };
            return cities;
          case "Mississauga":
            cities = new List<string>() { "Mississauga", "Toronto", "Etobicoke", "Oakville" };
            return cities;
          case "Huron":
            cities = new List<string>() { "Stratford", "Elmira", "Waterloo", "Collingwood", "Kitchener" };
            return cities;
          case "Region-of-Durham":
            cities = new List<string>() { "Whitby", "Oshawa", "Ajax", "Pickering", "Markham", "Scarborough" };
            return cities;
          case "Heritage":
            cities = new List<string>() { "Alberta", "Ottawa", "Mississauga", "Pembroke" };
            return cities;
          default:
            cities = new List<string>() { cityName };
            return cities;
        }
      else
      {
        switch (cityName)
        {
          case "Toronto":
            cities = new List<string>() { "Toronto", "Etobicoke", "Thornhill", "North York" };
            return cities;
          case "Brampton":
            cities = new List<string>() { "Toronto", "Mississauga", "Etobicoke", "Woodbridge" };
            return cities;
          case "Region-of-Durham":
            cities = new List<string>() { "Whitby", "Oshawa", "Ajax", "Pickering", "Markham", "Scarborough" };
            return cities;
          case "Heritage":
            cities = new List<string>() { "Alberta", "Ottawa", "Mississauga", "Pembroke" };
            return cities;
          default:
            cities = new List<string>() { cityName };
            return cities;
        }
      }
    }

    private static Random random = new Random();
    private static string RanStr(int len)
    {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    #endregion

    enum PropertyType
    {
      RetirementResidence,
      LongTermCare
    }

    enum BrowserType
    {
      Desktop,
      iPhone678
    }
  }
}