using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace SeleniumTests.Helpers
{
  public class CommonHelpers
  {
    public FindElement findElement = new FindElement();

    /// <summary>
    /// This method enters city name.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="cityName">City name, string value (e.g. Toronto)</param>
    public void EnterCity(IWebDriver driver, string cityName)
    {
      //If the location of system is enabled then the City field populate current location after few seconds.
      //To clear the City field as soon as it populate the current location, we need some wait here.
      Thread.Sleep(3000);
      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(Keys.Control + "a");
      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(Keys.Delete);
      findElement.WebElement(driver, By.Id("City"), 2).SendKeys(cityName);
    }

    /// <summary>
    /// This method enters postal code.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="postalCode">postal code, string value</param>
    public void EnterPostalCode(IWebDriver driver, string postalCode)
    {
      findElement.WebElement(driver, By.Id("PostalCode"), 5).Clear();
      findElement.WebElement(driver, By.Id("PostalCode"), 5).SendKeys(postalCode);
    }

    /// <summary>
    /// This method enters value in Residency field.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="residency">string value</param>
    public void EnterResidency(IWebDriver driver, string residency)
    {
      findElement.WebElement(driver, By.Id("PropertyName"), 2).SendKeys(Keys.Control + "a");
      findElement.WebElement(driver, By.Id("PropertyName"), 2).SendKeys(Keys.Delete);
      findElement.WebElement(driver, By.Id("PropertyName"), 5).SendKeys(residency);
    }

    /// <summary>
    /// This method verifies if the web element presents or not.
    /// </summary>
    /// <param name="webDriver">Web Driver<param>
    /// <param name="by">By method</param>
    /// <returns>Bool value (e.g. true, false)</returns>
    public bool VerifyElementPresent(IWebDriver webDriver, By by, int timeOut = 5)
    {
      try
      {
        if (findElement.WebElement(webDriver, by, timeOut) != null)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      catch (NoSuchElementException)
      {
        return false;
      }
    }

    /// <summary>
    /// This method is to switch between English and French on both Desktop and Mobile browsers.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="browserType">Type of Browser (e.g. Desktop, Mobile)</param>
    /// <param name="language">Language (e.g English, French)</param>
    public void SwitchBetweenENAndFR(IWebDriver driver, string browserType, Language language)
    {
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      switch (browserType)
      {
        case "Desktop":
          switch (language.ToString())
          {
            case "English":
              int attempts = 0;
              while (attempts < 2)
              {
                try
                {
                  findElement.WebElement(driver, By.Id("lnkEnglish"), 10).Click();
                  break;
                }
                catch (StaleElementReferenceException)
                {
                }
                attempts++;
              }
              break;
            case "French":
              attempts = 0;
              while (attempts < 2)
              {
                try
                {
                  findElement.WebElement(driver, By.Id("lnkFrench"), 10).Click();
                  break;
                }
                catch (StaleElementReferenceException)
                {
                }
                attempts++;
              }
              break;
          }
          break;
        case "Mobile":
          switch (language.ToString())
          {
            case "English":
              findElement.WebElement(driver, By.CssSelector("#mobileNavToggle button"), 2).Click();
              js.ExecuteScript("document.getElementById('lnkEnglish').click();");
              break;
            case "French":
              findElement.WebElement(driver, By.CssSelector("#mobileNavToggle button"), 2).Click();
              js.ExecuteScript("document.getElementById('lnkFrench').click();");
              break;
          }
          break;

        default:
          break;
      }
    }

    public void ClickElementByJS(IWebDriver driver, string cssSelector)
    {
      string query = "document.querySelector('" + cssSelector + "').click();";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      js.ExecuteScript(query);
    }

    /// <summary>
    /// This method returns the text of the element.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="cssSelector">CSS Selector</param>
    /// <returns>string value</returns>
    public string GetTextByJS(IWebDriver driver, string cssSelector)
    {
      string query = "return document.querySelector('" + cssSelector + "').innerText;";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      return js.ExecuteScript(query).ToString().Trim();
    }

    /// <summary>
    /// This method returns the text of the element.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="cssSelector">CSS Selector</param>
    /// <returns>string value</returns>
    public string GetTextsByJS(IWebDriver driver, string jsQuery)
    {
      string query = jsQuery;
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      return js.ExecuteScript(query).ToString().Trim();
    }

    public bool VerifyElementPresentByJS(IWebDriver driver, string cssSelector)
    {
      string query = "return document.querySelector(\"" + cssSelector + "\");";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      var element = js.ExecuteScript(query);
      if (element != null)
      {
        return true;
      }
      else return false;
    }

    /// <summary>
    /// This method returns selected value from All Properties dropdown.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>string value</returns>
    public string GetSelectedValueFromAllPropertiesDropdown(IWebDriver driver)
    {
      return findElement.WebElement(driver, By.CssSelector("option[selected='selected']"), 2).Text;
    }

    /// <summary>
    /// This metod return the heading of Search Result.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>string value</returns>
    public string GetSearchResultHeading(IWebDriver driver)
    {
      return findElement.WebElement(driver, By.CssSelector("#CityLandingPageList h2"), 2).Text;
    }

    /// <summary>
    /// This method returns list of options displayed in opened dropdown. First open the dropdown to use this method.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>string list</returns>
    public List<string> GetListOfAvailableOptionsFromDropdown(IWebDriver driver)
    {
      List<string> listOfItems = new List<string>();
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      var length = js.ExecuteScript("return document.querySelectorAll('.tt-suggestion.tt-selectable').length;");
      for (int i = 0; i < Convert.ToInt32(length); i++)
      {
        string query = "return document.querySelectorAll('.tt-suggestion.tt-selectable')[" + i + "].innerText";
        listOfItems.Add(js.ExecuteScript(query).ToString());
      }
      return listOfItems;
    }

    /// <summary>
    /// This method returns all available options from All Properties dropdown.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>List of string</returns>
    public List<string> GetOptionsFromAllPropertiesDropdown(IWebDriver driver)
    {
      List<string> listOfOptions = new List<string>();
      IList<IWebElement> listOfElements = findElement.WebElements(driver, By.CssSelector("select[class='form-control regionDropDown'] option"));
      foreach (IWebElement element in listOfElements)
      {
        listOfOptions.Add(element.Text);
      }
      return listOfOptions;
    }

    /// <summary>
    /// This method returns source of thumbnails from search result page.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>List of string</returns>
    public List<string> GetSourceOfThumbnailsOnResultPage(IWebDriver driver)
    {
      List<string> imageSources = new List<string>();
      IList<IWebElement> listOfThumbnails = findElement.WebElements(driver, By.CssSelector(".panel.panel-primary a img"));
      foreach (IWebElement thumbnail in listOfThumbnails)
      {
        imageSources.Add(thumbnail.GetAttribute("src"));
      }
      return imageSources;
    }

    /// <summary>
    /// This method returns provinces displayed on newest properties page.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <returns>List<string></returns>
    public List<string> GetProvincesDisplayedOnNewestPropertiesPage(IWebDriver driver, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector(".text-center h3"), 20);
      List<string> provinces = new List<string>();
      IList<IWebElement> elements = findElement.WebElements(driver, By.CssSelector(".text-center h3"));
      foreach (IWebElement element in elements)
      {
        provinces.Add(element.Text);
      }
      return provinces;
    }

    /// <summary>
    /// This method clicks on submenu under Getting Started menu.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="gettinStartedMenu">Sub Menu option</param>
    /// <param name="timeOut">int TimeOut in seconds to wait until the element found.</param>
    public void ClickMenuInGettingStarted(IWebDriver driver, string browserType, GettingStartedMenu gettinStartedMenu, int timeOut)
    {
      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.CssSelector("#mobileNavToggle"), timeOut).Click();
      }

      findElement.WebElement(driver, By.Id("MainNav-GettingStarted"), timeOut).Click();
      switch (gettinStartedMenu.ToString())
      {
        case "UnderstandingTheBenefits":
          findElement.WebElement(driver, By.Id("MainNav-UnderstandingtheBenefits"), 5).Click();
          break;
        case "MisconceptionsAboutRetirementLiving":
          findElement.WebElement(driver, By.Id("MainNav-MisconceptionsaboutRetirementLiving"), 5).Click();
          break;
        case "ADayInTheLifeOfAResident":
          findElement.WebElement(driver, By.Id("MainNav-ADayintheLifeofaResident"), 5).Click();
          break;
        case "SearchingForSelf":
          findElement.WebElement(driver, By.Id("MainNav-SearchingforSelf"), 5).Click();
          break;
        case "AmIReadyQuestionnaire":
          findElement.WebElement(driver, By.Id("MainNav-AmIReadyQuestionnaire"), 5).Click();
          break;
        case "BenefitsOfRetirementLiving":
          findElement.WebElement(driver, By.Id("MainNav-BenefitsofRetirementLiving"), 5).Click();
          break;
        case "ExploringRetirementLiving":
          findElement.WebElement(driver, By.Id("MainNav-ExploringRetirementLiving"), 5).Click();
          break;
        case "HowToMakeTheTransition":
          ClickElementByJS(driver, "#MainNav-HowtoMaketheTransition");
          break;
        case "SearchingForALovedOne":
          findElement.WebElement(driver, By.Id("MainNav-SearchingforaLovedOne"), 5).Click();
          break;
        case "IsItTimeQuestionnaire":
          ClickElementByJS(driver, "#MainNav-IsItTimeQuestionnaire");
          break;
        case "PeaceOfMind":
          findElement.WebElement(driver, By.Id("MainNav-PeaceofMind"), 5).Click();
          break;
        case "SupportingALovedOne":
          findElement.WebElement(driver, By.Id("MainNav-SupportingaLovedOne"), 5).Click();
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// This method clicks on submenu under Help Me Choose menu.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="helpMeChooseMenu">Sub Menu option</param>
    /// <param name="timeOut">int TimeOut in seconds to wait until the element found.</param>
    public void ClickMenuInHelpMeChoose(IWebDriver driver, string browserType, HelpMeChooseMenu helpMeChooseMenu, int timeOut)
    {
      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.CssSelector("#mobileNavToggle"), timeOut).Click();
      }

      findElement.WebElement(driver, By.Id("MainNav-HelpMeChoose"), timeOut).Click();
      switch (helpMeChooseMenu.ToString())
      {
        case "ExploringYourOptions":
          ClickElementByJS(driver, "#MainNav-ExploringYourOptions");
          break;
        case "IndependentLiving":
          findElement.WebElement(driver, By.Id("MainNav-IndependentLiving"), 5).Click();
          break;
        case "IndependentSupportiveLiving":
          findElement.WebElement(driver, By.Id("MainNav-IndependentSupportiveLiving"), 5).Click();
          break;
        case "AssistedLiving":
          findElement.WebElement(driver, By.Id("MainNav-AssistedLiving"), 5).Click();
          break;
        case "MemoryCare":
          findElement.WebElement(driver, By.Id("MainNav-MemoryCare"), 5).Click();
          break;
        case "LongTermCare":
          findElement.WebElement(driver, By.Id("MainNav-LongTermCare"), 5).Click();
          break;
        case "RetirementVersusLongTermCare":
          findElement.WebElement(driver, By.Id("MainNav-RetirementversusLongTermCare"), 5).Click();
          break;
        case "RetirementLivingVsHomecare":
          ClickElementByJS(driver, "#MainNav- RetirementLivingvsHomecare");
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// This method clicks on submenu under Our Services menu.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="ourServicesMenu">Sub Menu option</param>
    /// <param name="timeOut">int TimeOut in seconds to wait until the element found.</param>
    public void ClickMenuInOurServices(IWebDriver driver, string browserType, OurServicesMenu ourServicesMenu, int timeOut)
    {
      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.CssSelector("#mobileNavToggle"), timeOut).Click();
      }

      findElement.WebElement(driver, By.Id("MainNav-OurServices"), timeOut).Click();
      switch (ourServicesMenu.ToString())
      {
        case "DiningExperience":
          ClickElementByJavaScriptQuery(driver, "document.getElementById('MainNav-DiningExperience').click();");
          break;
        case "ActiveLiving":
          findElement.WebElement(driver, By.Id("MainNav-ActiveLiving"), 5).Click();
          break;
        case "LiveNow":
          findElement.WebElement(driver, By.Id("MainNav-LiveNow"), 5).Click();
          break;
        case "Recreation":
          findElement.WebElement(driver, By.Id("MainNav-Recreation"), 5).Click();
          break;
        case "BenefitsOfSocialization":
          findElement.WebElement(driver, By.Id("MainNav-BenefitsofSocialization"), 5).Click();
          break;
        case "WellnessAndSupportServices":
          findElement.WebElement(driver, By.Id("MainNav-WellnessandSupportServices"), 5).Click();
          break;
        case "HealthAndWellness":
          findElement.WebElement(driver, By.Id("MainNav-HealthandWellness"), 5).Click();
          break;
        case "SuiteTypesAndAmenities":
          ClickElementByJS(driver, "#MainNav-SuiteTypesandAmenities");
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// This method verifies if the section exists on left side on the Benefits page.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="sectionName">Section Name</param>
    /// <param name="timeOut">Wait time until the element found.</param>
    /// <returns></returns>
    public bool VerifyLeftSectionOnUnderstandingTheBenefitsPage(IWebDriver driver, string sectionName, int timeOut)
    {
      findElement.WaitUntilElementExists(driver, By.CssSelector("ul[class='nav navbar-nav quickLinksNav']"), timeOut);
      return findElement.WebElements(driver, By.CssSelector(".quickLinksFirstLevel a")).Any(s => s.Text.Trim() == sectionName);
    }

    /// <summary>
    /// This method fills out the 'AM I Ready? Questionnaire'.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="AmIReadyQuestionnaireAnswers">List of string (answers)</param>
    /// <param name="timeOut">Wait time until the element found</param>
    public void FilloutAmIReadyQuestionnaire(IWebDriver driver, List<string> AmIReadyQuestionnaireAnswers, int timeOut)
    {
      int questionNo = 0;
      foreach (string answer in AmIReadyQuestionnaireAnswers)
      {
        questionNo = questionNo + 1;
        SelectAnswerOfAMIReadyQuestionnaire(driver, questionNo, answer, timeOut);

        if (questionNo == 5 || questionNo == 10)
        {
          ClickElementByJS(driver, ".action.next.btn.btn-default");
        }
      }
    }

    /// <summary>
    /// This method select answer of individual question on 'AM I Ready? Questionnaire'
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="questionNo">Question Number</param>
    /// <param name="answer">Answer</param>
    /// <param name="timeOut">Wait time until the element to be found.</param>
    public void SelectAnswerOfAMIReadyQuestionnaire(IWebDriver driver, int questionNo, string answer, int timeOut)
    {
      findElement.WaitUntilElementExists(driver, By.CssSelector(".question.listReport"), timeOut);
      int answerNo = 0;
      int pageNo = 1;
      IList<IWebElement> answers = findElement.WebElements(driver, By.CssSelector(".question.listReport"));
      IList<IWebElement> ans = answers[questionNo - 1].FindElements(By.TagName("label"));
      for (int i = 0; i < ans.Count; i++)
      {
        if (ans[i].Text.Trim() == answer)
        {
          answerNo = i + 1;
          break;
        }
      }

      int counter = questionNo * 2;

      if (questionNo > 5)
      {
        counter = counter - 10;
        pageNo = 2;
        if (questionNo > 10)
        {
          counter = counter - 10;
          pageNo = 3;
        }
      }
      string query = "document.querySelector('.step:nth-of-type(" + pageNo + ") .question.listReport:nth-of-type(" + counter + ") label:nth-of-type(" + answerNo + ") input').click();";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      js.ExecuteScript(query);
    }

    /// <summary>
    /// This method select answer of individual question on 'Is It Time Questionnaire'
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="questionNo">Question Number</param>
    /// <param name="answer">Answer</param>
    /// <param name="timeOut">Wait time until the element to be found.</param>
    public void SelectAnswerOfIsItTimeQuestionnaire(IWebDriver driver, int questionNo, string answer, int timeOut)
    {
      findElement.WaitUntilElementExists(driver, By.CssSelector(".question.listReport"), timeOut);
      int answerNo = 0;
      int pageNo = 1;
      IList<IWebElement> answers = findElement.WebElements(driver, By.CssSelector(".question.listReport"));
      IList<IWebElement> ans = answers[questionNo - 1].FindElements(By.TagName("label"));
      for (int i = 0; i < ans.Count; i++)
      {
        if (ans[i].Text.Trim() == answer)
        {
          answerNo = i + 1;
          break;
        }
      }

      int counter = questionNo * 2;

      if (questionNo > 5)
      {
        counter = counter - 10;
        pageNo = 2;
      }
      string query = "document.querySelector('.step:nth-of-type(" + pageNo + ") .question.listReport:nth-of-type(" + counter + ") label:nth-of-type(" + answerNo + ") input').click();";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      js.ExecuteScript(query);
    }

    /// <summary>
    /// This method fills out the 'Is It Time Questionnaire'.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="AmIReadyQuestionnaireAnswers">List of string (answers)</param>
    /// <param name="timeOut">Wait time until the element found</param>
    public void FilloutIsItTimeQuestionnaire(IWebDriver driver, List<string> AmIReadyQuestionnaireAnswers, int timeOut)
    {
      int questionNo = 0;
      foreach (string answer in AmIReadyQuestionnaireAnswers)
      {
        questionNo = questionNo + 1;
        SelectAnswerOfIsItTimeQuestionnaire(driver, questionNo, answer, timeOut);

        if (questionNo == 5)
        {
          ClickElementByJS(driver, ".action.next.btn.btn-default");
        }
      }
    }

    public IList<IWebElement> GetElementsByJS(IWebDriver driver, string cssSelector)
    {
      var query = "return document.querySelectorAll('" + cssSelector + "');";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      return (IList<IWebElement>)js.ExecuteScript(query);
    }

    public IWebElement GetElementByJS(IWebDriver driver, string cssSelector)
    {
      var query = "return document.querySelector('" + cssSelector + "');";
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      return (IWebElement)js.ExecuteScript(query);
    }

    public void ClickElementByJavaScriptQuery(IWebDriver driver, string query)
    {
      IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
      js.ExecuteScript(query);
    }

    public void OpenOrCloseAccordioPanelOnDiningExperiencePage(IWebDriver driver, DiningExperienceAccordionPanels diningExperienceAccordionPanels, bool isOpen, int timeOut)
    {
      Thread.Sleep(2000);
      findElement.Wait(driver, By.Id("accordionDining"), timeOut);
      string description = GetDescription(diningExperienceAccordionPanels);
      switch (description)
      {
        case "High-Quality Food":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingQualityFood").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingQualityFood a");
          }
          break;
        case "Diverse Menu Choice":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingMenuChoice").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingMenuChoice a");
          }
          break;
        case "Warm and Professional Service":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingProService").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingProService a");
          }
          break;
        case "Welcome Social Ambiance":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingSocialAmbiance").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingSocialAmbiance a");
          }
          break;
        case "Unique Amenities & Programs":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingUniquePrograms").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingUniquePrograms a");
          }
          break;
        default:
          break;
      }
      Thread.Sleep(1000);
    }

    public bool VerifyIfAccordionPanelIsOpenOnDiningExperiencePage(IWebDriver driver, DiningExperienceAccordionPanels diningExperienceAccordionPanels, int timeOut)
    {
      findElement.Wait(driver, By.Id("accordionDining"), timeOut);
      string description = GetDescription(diningExperienceAccordionPanels);
      switch (description)
      {
        case "High-Quality Food":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingQualityFood").GetAttribute("aria-expanded"));
        case "Diverse Menu Choice":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingMenuChoice").GetAttribute("aria-expanded"));
        case "Warm and Professional Service":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingProService").GetAttribute("aria-expanded"));
        case "Welcome Social Ambiance":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingSocialAmbiance").GetAttribute("aria-expanded"));
        case "Unique Amenities & Programs":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingUniquePrograms").GetAttribute("aria-expanded"));
        default:
          return false;
      }
    }

    public void OpenOrCloseAccordioPanelOnLiveNowPage(IWebDriver driver, LiveNowAccordionPanels liveNowAccordionPanels, bool isOpen, int timeOut)
    {
      Thread.Sleep(2000);
      findElement.Wait(driver, By.Id("accordionDining"), timeOut);
      switch (liveNowAccordionPanels.ToString())
      {
        case "Social":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingMenuChoice").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingMenuChoice a");
          }
          break;
        case "Vocational":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingProService").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingProService a");
          }
          break;
        case "Spiritual":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingSocialAmbiance").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingSocialAmbiance a");
          }
          break;
        case "Emotional":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingUniquePrograms").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingUniquePrograms a");
          }
          break;
        case "Intellectual":
          if (Convert.ToBoolean(GetElementByJS(driver, "#headingIntellectual").GetAttribute("aria-expanded")) != isOpen)
          {
            ClickElementByJS(driver, "#headingIntellectual a");
          }
          break;
        default:
          break;
      }
      Thread.Sleep(1000);
    }

    public bool VerifyIfAccordionPanelIsOpenOnLiveNowPage(IWebDriver driver, LiveNowAccordionPanels liveNowAccordionPanels, int timeOut)
    {
      findElement.Wait(driver, By.Id("accordionDining"), timeOut);
      switch (liveNowAccordionPanels.ToString())
      {
        case "Social":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingMenuChoice").GetAttribute("aria-expanded"));
        case "Vocational":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingProService").GetAttribute("aria-expanded"));
        case "Spiritual":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingSocialAmbiance").GetAttribute("aria-expanded"));
        case "Emotional":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingUniquePrograms").GetAttribute("aria-expanded"));
        case "Intellectual":
          return Convert.ToBoolean(GetElementByJS(driver, "#headingIntellectual").GetAttribute("aria-expanded"));
        default:
          return false;
      }
    }

    /// <summary>
    /// This method clicks on submenu under Learn menu.
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="ourServicesMenu">Sub Menu option</param>
    /// <param name="timeOut">int TimeOut in seconds to wait until the element found.</param>
    public void ClickMenuInLearn(IWebDriver driver, string browserType, LearnMenu learnMenu, int timeOut)
    {
      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.CssSelector("#mobileNavToggle"), timeOut).Click();
      }

      findElement.WebElement(driver, By.Id("MainNav-Learn"), timeOut).Click();
      switch (learnMenu.ToString())
      {
        case "StepByStepResources":
          ClickElementByJavaScriptQuery(driver, "document.getElementById('MainNav-Step-by-StepResources').click();");
          break;
        case "BeginningYourResearch":
          findElement.WebElement(driver, By.Id("MainNav-BeginningYourResearch"), 5).Click();
          break;
        case "HavingTheConversation":
          findElement.WebElement(driver, By.Id("MainNav-HavingTheConversation"), 5).Click();
          break;
        case "FindingTheRightResidence":
          findElement.WebElement(driver, By.Id("MainNav-FindingTheRightResidence"), 5).Click();
          break;
        case "PlanningYourMove":
          findElement.WebElement(driver, By.Id("MainNav-PlanningYourMove"), 5).Click();
          break;
        case "ExpertAdvice":
          findElement.WebElement(driver, By.Id("MainNav-ExpertAdvice"), 5).Click();
          break;
        case "EssentialConversationsWithDrAmy":
          findElement.WebElement(driver, By.Id("Essential Conversations with Dr Amy"), 5).Click();
          break;
        case "FinancesWithKelleyKeehn":
          ClickElementByJS(driver, "#MainNav-FinanceswithKelleyKeehn");
          break;
        case "BrowseTopics":
          ClickElementByJS(driver, ".dropdown.show a[href=\"/en/learn/browse-topics\"]");
          break;
        case "BudgetAssistant":
          ClickElementByJS(driver, "a[href=\"/en/learn/budget-assistant\"]");
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// This method click on links under Supporting Aging Parent section
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="supportingAgingParentLinks"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderSupportingAgingParent(IWebDriver driver, SupportingAgingParentLinks supportingAgingParentLinks, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf']"), 5);

      switch (supportingAgingParentLinks.ToString())
      {
        case "SupportingALovedOneGuidePDF":
          ClickElementByJS(driver, "a[href=\"/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf\"]");
          break;
        case "IsItTimeQuestionnaire":
          ClickElementByJS(driver, "a[href=\"/getting-started/searching-for-a-loved-one/is-it-time\"]");
          break;
        case "Articles":
          ClickElementByJS(driver, "#headingBlogSupport a");
          break;
        case "AskOurExperts":
          ClickElementByJS(driver, "#headingAskExpert a");
          break;
      }
    }

    /// <summary>
    /// This method clicks on links under Supporting Aging Parent >> Articles
    /// </summary>
    /// <param name="driver">Web Driver</param>
    /// <param name="supportingAgingParentArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderSupportingAgingParentArticles(IWebDriver driver, SupportingAgingParentArticles supportingAgingParentArticles, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf']"), 5);

      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingBlogSupport"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingBlogSupport a");
      }

      switch (supportingAgingParentArticles.ToString())
      {
        case "AChecklistOfImportantQuestionsToAskYourAgingParent":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/01/a-checklist-of-important-questions-to-ask-your-aging-parent\"]");
          break;
        case "FourFinancialQuestionsToAskYourAgingParent":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/05/4-financial-questions-to-ask-your-aging-parent\"]");
          break;
        case "AnHonestConversationExploringRetirementLivingWithALovedOne":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2017/01/an-honest-conversation-exploring-retirement-living-with-a-loved-one\"]");
          break;
        case "TellingSignsItMayBeTimeToConsiderARetirementResidence":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/08/telling-signs-it-may-be-time-to-consider-a-retirement-residence\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on links under Supporting Aging Parent >> Ask Our Experts
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="supportingAgingParentAskOurExperts"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderSupportingAgingParentAskOurExperts(IWebDriver driver, SupportingAgingParentAskOurExperts supportingAgingParentAskOurExperts, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/-/media/Files/guides/loved-one/chartwell-supporting-a-loved-one-guide.pdf']"), 5);

      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingAskExpert"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingAskExpert a");
      }

      switch (supportingAgingParentAskOurExperts.ToString())
      {
        case "VideoDrAmyGettingOnTheSamePageAsYourSiblings":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/8CwoAqih7XI\"]");
          break;
        case "VideoDrAmyTalkingToYourParentsAboutKeepingSociallyActive":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/Oy7oKJPTrzg\"]");
          break;
        case "VideoDrAmyCommunicatingWithYourSiblingsAboutYourParentsCareNeeds":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/YIlFGbPC4TY\"]");
          break;
        case "VideoDrAmyUnderstandingThePerspectivesOfAParent":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/NkVmfoqHFrU\"]");
          break;
        case "EssentialConversationsWithDrAmyUnderstandingAParentsDifficultReactionsToABigLifeTransition":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/02/understanding-a-parents-difficult-reactions-to-a-big-life-transition\"]");
          break;
        case "EssentialConversationsWithDrAmyWhatDoIDoIfMySiblingWontHelpMeCareForOurAgingParents":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2018/10/essential-conversations-with-dr-amy-what-should-i-do-if-my-sibling-wont-help-me-care-for-our-parents\"]");
          break;
        case "HowDoIStartTheConversationAboutRetirementLivingWithMyLovedOne":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/03/how-do-i-start-the-conversation-about-retirement-living-with-my-loved-one\"]");
          break;
        case "WhenIsTheRightTimeForRetirementLiving":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/11/ask-our-experts-when-is-the-right-time-for-retirement-living\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on links under Finance section.
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="financeLinks"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderFinance(IWebDriver driver, FinanceLinks financeLinks, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/learn/budget-assistant']"), 5);
      switch (financeLinks.ToString())
      {
        case "BudgetAssistant":
          ClickElementByJS(driver, "a[href=\"/learn/budget-assistant\"]");
          break;
        case "Articles":
          ClickElementByJS(driver, "#headingBlogFinance a");
          break;
        case "AskOurExperts":
          ClickElementByJS(driver, "#headingFinanceAskExpert a");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Finance >> Articles
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="financeArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderFinanceArticle(IWebDriver driver, FinanceArticles financeArticles, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/learn/budget-assistant']"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingBlogFinance"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingBlogFinance a");
      }
      switch (financeArticles.ToString())
      {
        case "FourFactorsToConsiderWhenExploringRetirementLivingAffordability":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/05/4-factors-to-consider-when-exploring-retirement-living-affordability\"]");
          break;
        case "ExploringAndPlanningForTheCostOfRetirementLiving":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/05/exploring-and-planning-for-the-cost-of-retirement-living\"]");
          break;
        case "IsRetirementLivingAnAffordablePption":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/04/ask-our-residents-is-retirement-living-an-affordable-option\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Finance >> Ask Our Experts
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="financeArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderFinanceAskOurExperts(IWebDriver driver, FinanceAskOurExperts financeAskOurExperts, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='/learn/budget-assistant']"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingFinanceAskExpert"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingFinanceAskExpert a");
      }
      switch (financeAskOurExperts.ToString())
      {
        case "VideoDrAmyTalkingToYourParentsAboutTheManagementOfTheirFinances":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/8CwoAqih7XI\"]");
          break;
        case "VideoDrAmyGivingYourselfPermissionToLiveComfortablyInYourLaterYears":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/Oy7oKJPTrzg\"]");
          break;
        case "AskOurResidentsIsRetirementLivingAnAffordableOption":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/04/ask-our-residents-is-retirement-living-an-affordable-option\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Care And Services section
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="careAndServicesLinks"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderCareAndServices(IWebDriver driver, CareAndServicesLinks careAndServicesLinks, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingExploringOptions"), 5);
      switch (careAndServicesLinks.ToString())
      {
        case "ExploringYourOptions":
          ClickElementByJS(driver, "#headingExploringOptions a");
          break;
        case "Articles":
          ClickElementByJS(driver, "#headingCare a");
          break;
        case "AskOurExperts":
          ClickElementByJS(driver, "#headingCareAskExpert a");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Care And Services >> Exploring Your Options
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="careAndServicesExploringYourOptions"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderCareAndServicesExploringYourOptions(IWebDriver driver, CareAndServicesExploringYourOptions careAndServicesExploringYourOptions, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingExploringOptions"), 5);

      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingExploringOptions"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingExploringOptions a");
      }

      switch (careAndServicesExploringYourOptions.ToString())
      {
        case "VideoExploringYourOptionsWhatIsIndependentLiving":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/UeZayR1WMng\"]");
          break;
        case "VideoExploringYourOptionsWhatIsIndependentSupportiveLiving":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/TzeuGivaAF0\"]");
          break;
        case "VideoExploringYourOptionsWhatIsAssistedLiving":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/PZ3zOcXwFtI\"]");
          break;
        case "VideoExploringYourOptionsWhatIsMemoryCare":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/hgoemHjoqj4\"]");
          break;
        case "VideoExploringYourOptionsWhatIsLongTermCare":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/bRXNnpavtTQ\"]");
          break;
        case "VideoWhatIsTheDifferenceBetweenARetirementCommunityAndALongTermCareResidence":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/pi8uouPx0Ms\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Care And Services >> Articles
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="careAndServicesArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderCareAndServicesArticles(IWebDriver driver, CareAndServicesArticles careAndServicesArticles, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingCare"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingCare"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingCare a");
      }

      switch (careAndServicesArticles.ToString())
      {
        case "RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/07/retirement-living-options-which-care-level-is-right-for-your-loved-one\"]");
          break;
        case "CareAndLivingOptionsOfferedByChartwellRetirementResidences":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/11/care-and-living-options-offered-by-chartwell-retirement-residences\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Care And Services >> Our Experts
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="careAndServicesAskOurExperts"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderCareAndServicesAskOurExperts(IWebDriver driver, CareAndServicesAskOurExperts careAndServicesAskOurExperts, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingCareAskExpert"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingCareAskExpert"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingCareAskExpert a");
      }
      switch (careAndServicesAskOurExperts.ToString())
      {
        case "HowDoIknowWhichRetirementLivingSupportOptionsIsRightForMyNeeds":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/04/ask-our-experts-how-do-i-know-which-retirement-living-support-option-is-right-for-my-needs\"]");
          break;
        case "CaringForAnAgingSpouse":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2017/02/ask-our-experts-caring-for-a-spouse-during-your-retirement-years\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Active Living And Health & Wellness
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="activeLivingAndHealthAndWellnessLinks"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderActiveLivingAndHealthAndWellness(IWebDriver driver, ActiveLivingAndHealthAndWellnessLinks activeLivingAndHealthAndWellnessLinks, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='https://youtu.be/RVmYf_xsl6o']"), 5);
      switch (activeLivingAndHealthAndWellnessLinks.ToString())
      {
        case "VideoRhythmAndMovesExerciseClassVideoEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/RVmYf_xsl6o\"]");
          break;
        case "VideoRhythmAndMovesVideoFromHamptonHouseFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/8lxN0-VG97g\"]");
          break;
        case "VideoChartwellCruiseEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/zzsI5SIxnoE\"]");
          break;
        case "VideoHONOUREnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/L7uuFirvvy8\"]");
          break;
        case "VideoSeniorsDreamsComingTrueChartwellsPartnershipwithWishOfALifetimeEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/L7uuFirvvy8\"]");
          break;
        case "MomentsThatMatterVideosAndPhotos":
          ClickElementByJS(driver, "a[href=\"/about-us/moments-that-matter\"]");
          break;
        case "Articles":
          ClickElementByJS(driver, "#headingActiveLivingBlog a");
          break;
        case "AskOurExperts":
          ClickElementByJS(driver, "#headingActiveLivingAskExpert a");
          break;
        case "Infographics":
          ClickElementByJS(driver, "#headingActiveLivingInfo a");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Active Living And Health & Wellness >> Articles
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="activeLivingAndHealthAndWellnessArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderActiveLivingAndHealthAndWellnessArticles(IWebDriver driver, ActiveLivingAndHealthAndWellnessArticles activeLivingAndHealthAndWellnessArticles, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='https://youtu.be/RVmYf_xsl6o']"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingActiveLivingBlog"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingActiveLivingBlog a");
      }
      switch (activeLivingAndHealthAndWellnessArticles.ToString())
      {
        case "ForSeniorsFriendsComeWithHealthyBenefits":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/02/for-seniors-friends-come-with-healthy-benefits\"]");
          break;
        case "AgingWellIsMoreThanJustGoodPhysicalHealthForSeniors":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/04/aging-well-is-more-than-just-good-physical-health-for-seniors\"]");
          break;
        case "HowSeniorsCanBoostTheirHealthSpanForAMoreVitalRetirement":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/04/how-seniors-can-boost-their-health-span-for-a-more-vital-retirement\"]");
          break;
        case "WhyAPositiveOutlookOnLifeAndAgingIsGoodForYourHealth":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2018/09/why-a-positive-outlook-on-life-and-aging-is-good-for-your-health\"]");
          break;
        case "SevenWaysToPromoteActiveAgingAndHealthyLongevity":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2018/10/7-ways-to-promote-active-aging-and-healthy-longevity\"]");
          break;
        case "EngagingInTheArtsBoostsSeniorsPhysicalAndEmotionalHealth":
          string cssSelector = "a[href^=\"https://chartwell.com/en/blog/2018/11/engaging-in-the-arts-boosts-seniors\"]";
          ClickElementByJS(driver, cssSelector);
          break;
      }
    }

    /// <summary>
    /// This method click on link under Active Living And Health & Wellness >> Our Experts
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="activeLivingAndHealthAndWellnessAskOurExperts"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderActiveLivingAndHealthAndWellnessAskOurExperts(IWebDriver driver, ActiveLivingAndHealthAndWellnessAskOurExperts activeLivingAndHealthAndWellnessAskOurExperts, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='https://youtu.be/RVmYf_xsl6o']"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingActiveLivingAskExpert"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingActiveLivingAskExpert a");
      }
      switch (activeLivingAndHealthAndWellnessAskOurExperts.ToString())
      {
        case "AskOurExperts":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog\"]");
          break;
        case "VideoDrAmyAssessingWhetherYourLifestyleIsHealthyAndEnjoyable":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/17RveT02kxQ\"]");
          break;
        case "ExploreChartwellsRhythmAndMovesExerciseClassWithLifestyleAndProgramManagerTraceyMcDonald":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/06/explore-chartwells-rhythm-n-moves-exercise-class-with-lifestyle-and-program-manager-tracey-mcdonald\"]");
          break;
        case "TheRoleOfSocializationInSeniorsHealthAndWellness":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2018/04/ask-our-experts-the-importance-of-a-social-life-in-our-retirement-years\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Active Living And Health & Wellness >> Infographics
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="activeLivingAndHealthAndWellnessInfographics"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderActiveLivingAndHealthAndWellnessInfographics(IWebDriver driver, ActiveLivingAndHealthAndWellnessInfographics activeLivingAndHealthAndWellnessInfographics, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href='https://youtu.be/RVmYf_xsl6o']"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingActiveLivingInfo"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingActiveLivingInfo a");
      }
      switch (activeLivingAndHealthAndWellnessInfographics.ToString())
      {
        case "BenefitsOfSocializationPDF":
          ClickElementByJS(driver, "a[href=\"/-/media/Files/infographics/chartwell-benefits-of-socialization-infographic.pdf\"]");
          break;
        case "HealthAndWellnessPDF":
          ClickElementByJS(driver, "a[href=\"/-/media/Files/infographics/chartwell-health-and-wellness-infographic.pdf\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Help Me Choose section.
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="helpMeChooseLinks"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinksUnderHelpMeChooseSection(IWebDriver driver, HelpMeChooseLinks helpMeChooseLinks, int timeOut)
    {
      findElement.Wait(driver, By.CssSelector("a[href=\"/-/media/Files/guides/self/chartwell-exploring-retirement-living-guide.pdf\"]"), 5);
      switch (helpMeChooseLinks.ToString())
      {
        case "ExploringRetirementLivingGuidePDF":
          ClickElementByJS(driver, "a[href=\"/-/media/Files/guides/self/chartwell-exploring-retirement-living-guide.pdf\"]");
          break;
        case "AmIReadyQuestionnaire":
          ClickElementByJS(driver, "a[href=\"/getting-started/searching-for-self/am-i-ready\"]");
          break;
        case "InfographicsBenefitsOfRetirementLiving":
          ClickElementByJS(driver, "a[href=\"/-/media/Files/infographics/chartwell-the-benefits-of-retirement-living-infographic.pdf\"]");
          break;
        case "AskOurExperts":
          ClickElementByJS(driver, "#headingHelpMeChoose a");
          break;
        case "Articles":
          ClickElementByJS(driver, "#headingHelpMeChooseBlog a");
          break;
        case "StaffStories":
          ClickElementByJS(driver, "#headingStaffStories a");
          break;
        case "ResidentStories":
          ClickElementByJS(driver, "#headingResidentStories a");
          break;
      }
    }

    /// <summary>
    /// This method click on link under Help Me choose >> Ask Our Experts
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="helpMeChooseAskOurExperts"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderHelpMeChooseAskOurExperts(IWebDriver driver, HelpMeChooseAskOurExperts helpMeChooseAskOurExperts, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingHelpMeChoose"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingHelpMeChoose"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingHelpMeChoose a");
      }
      switch (helpMeChooseAskOurExperts.ToString())
      {
        case "AskOurExperts":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/\"]");
          break;
        case "HowShouldIFurnishAndDecorateMyNewSuite":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2017/04/ask-our-experts-how-should-i-furnish-and-decorate-my-new-suite\"]");
          break;
        case "HowDoIKnowIfImReadyForRetirementLiving":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/02/ask-our-experts-how-do-i-know-if-im-ready-for-retirement-living\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on Help Me Choose >> Articles
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="helpMeChooseArticles"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderHelpMeChooseArticles(IWebDriver driver, HelpMeChooseArticles helpMeChooseArticles, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingHelpMeChooseBlog"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingHelpMeChooseBlog"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingHelpMeChooseBlog a");
      }
      switch (helpMeChooseArticles.ToString())
      {
        case "Blog":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog\"]");
          break;
        case "WhatToLookForWhileTakingATourOfARetirement":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/05/what-to-look-for-while-taking-a-tour-of-a-retirement-residence\"]");
          break;
        case "ThreeImportantQuestionsToAskWhileOnATourOfARetirementResidence":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/11/3-important-questions-to-ask-while-on-a-tour-of-a-retirement-residence\"]");
          break;
        case "TipsForFindingTheRightRetirementResidence":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/11/tips-for-finding-the-right-retirement-residence\"]");
          break;
        case "RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2015/07/retirement-living-options-which-care-level-is-right-for-your-loved-one\"]");
          break;
        case "SevenQuestionsToDetermineIfYouAreRetirementResidenceReady":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/04/7-questions-to-determine-if-youre-retirement-residence-ready\"]");
          break;
        case "WhyRetirementLivingMaySuitYouBetterThanReceivingSupportAtHome":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2019/03/Why-retirement-living-may-suit-you-better-than-receiving-support-at-home\"]");
          break;
        case "ThreeHealthyReasonsToConsiderRetirementLiving":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2018/09/three-healthy-reasons-to-consider-retirement-living\"]");
          break;
        case "WhyRetirementResidenceLivingCanBeAHealthyChoicePart2":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2018/10/why-retirement-residence-living-can-be-a-healthy-choice-part-2\"]");
          break;
        case "ResearchingRetirementLivingHereIsWhereToStart":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/05/researching-retirement-living-heres-where-to-start\"]");
          break;
        case "TheBenefitsOfConsideringARetirementLifestyleBeforeExperiencingAHealthScare":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/en/blog/2016/03/the-benefits-of-considering-a-retirement-lifestyle-before-experiencing-a-health-scare\"]");
          break;
        case "TheBenefitsOfAnIndependentLivingRetirementLifestyle":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/11/the-benefits-of-an-independent-living-retirement-lifestyle\"]");
          break;
        case "ThreeCompellingReasonsToMoveIntoARetirementCommunity":
          ClickElementByJS(driver, "a[href=\"https://chartwell.com/blog/2016/06/3-compelling-reasons-to-move-into-a-retirement-community\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Help Me Choose >> Staff Stories
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="helpMeChooseStaffStories"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderHelpMeChooseStaffStories(IWebDriver driver, HelpMeChooseStaffStories helpMeChooseStaffStories, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingStaffStories"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingStaffStories"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingStaffStories a");
      }
      switch (helpMeChooseStaffStories.ToString())
      {
        case "PeterEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/VaDI5mSrllQ\"]");
          break;
        case "MichelFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/0jD-UJ70eO0\"]");
          break;
        case "StephanieFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/ai94-doHqiY\"]");
          break;
      }
    }

    /// <summary>
    /// This method clicks on link under Help Me Choose >> Resident Stories
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="helpMeChooseResidentStories"></param>
    /// <param name="timeOut"></param>
    public void ClickOnLinkUnderHelpMeChooseResidentStories(IWebDriver driver, HelpMeChooseResidentStories helpMeChooseResidentStories, int timeOut)
    {
      findElement.Wait(driver, By.Id("headingResidentStories"), 5);
      if (!Convert.ToBoolean(findElement.WebElement(driver, By.Id("headingResidentStories"), 5).GetAttribute("aria-expanded")))
      {
        ClickElementByJS(driver, "#headingResidentStories a");
      }
      switch (helpMeChooseResidentStories.ToString())
      {
        case "ResidentStories":
          ClickElementByJS(driver, "a[href=\"https://www.youtube.com/playlist?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "LoreenAndCecilEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/ZcWluXxebSc?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "AurelAndMajaEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/VSJgsiRdjfE?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "PierretteFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/4M0T5Wku9O0?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "LamarcheCoupleFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/NX-00uJE-P4?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "JenniferEnglish":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/GmBlNyBnQq4?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
        case "FlorenceFrench":
          ClickElementByJS(driver, "a[href=\"https://youtu.be/Lvn1qXhufs8?list=PLHKGXZndC1HelGm02_iSz6BXI3IMS9FM9\"]");
          break;
      }
    }

    /// <summary>
    /// This method click on menu under About us.
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="browserType"></param>
    /// <param name="aboutUsMenu"></param>
    /// <param name="timeOut"></param>
    public void ClickMenuInAboutUs(IWebDriver driver, string browserType, AboutUsMenu aboutUsMenu, int timeOut)
    {
      if (browserType != "Desktop")
      {
        findElement.WebElement(driver, By.CssSelector("#mobileNavToggle"), timeOut).Click();
      }

      findElement.WebElement(driver, By.Id("MainNav-AboutUs"), timeOut).Click();
      switch (aboutUsMenu.ToString())
      {
        case "Welcome":
          //ClickElementByJavaScriptQuery(driver, "document.getElementById('MainNav-Step-by-StepResources').click();");
          findElement.WebElement(driver, By.Id("MainNav- Welcome"), 5).Click();
          break;
        case "OurVisionMissionAndValues":
          findElement.WebElement(driver, By.Id("MainNav- OurVisionMissionandValues"), 5).Click();
          break;
        case "OurLeadershipTeam":
          findElement.WebElement(driver, By.Id("MainNav-OurLeadershipTeam"), 5).Click();
          break;
        case "WishOfALifetimeCanada":
          findElement.WebElement(driver, By.Id("MainNav-WishofaLifetimeCanada"), 5).Click();
          break;
        case "MomentsThatMatter":
          findElement.WebElement(driver, By.Id("MainNav-MomentsthatMatter"), 5).Click();
          break;
        case "CorporateSocialResponsibility":
          findElement.WebElement(driver, By.Id("MainNav-CorporateSocialResponsibility"), 5).Click();
          break;
        case "CorporateSocialResponsibilityProgram":
          findElement.WebElement(driver, By.Id("MainNav-CorporateSocialResponsibilityProgram"), 5).Click();
          break;
        case "HonourOurVeterans":
          //ClickElementByJS(driver, ".dropdown.show a[href=\"/en/learn/browse-topics\"]");
          findElement.WebElement(driver, By.Id("MainNav-HonourOurVeterans"), 5).Click();
          break;
        default:
          break;
      }
    }


    public void DoCityEntry()
    {

    }


    /// <summary>
    /// Types of browser
    /// </summary>
    public enum BrowserType
    {
      Desktop,
      Mobile
    }

    /// <summary>
    /// Languages
    /// </summary>
    public enum Language
    {
      English,
      French
    }

    public enum GettingStartedMenu
    {
      UnderstandingTheBenefits,
      MisconceptionsAboutRetirementLiving,
      ADayInTheLifeOfAResident,
      SearchingForSelf,
      AmIReadyQuestionnaire,
      BenefitsOfRetirementLiving,
      ExploringRetirementLiving,
      HowToMakeTheTransition,
      SearchingForALovedOne,
      IsItTimeQuestionnaire,
      PeaceOfMind,
      SupportingALovedOne
    }

    public enum HelpMeChooseMenu
    {
      ExploringYourOptions,
      IndependentLiving,
      IndependentSupportiveLiving,
      AssistedLiving,
      MemoryCare,
      LongTermCare,
      RetirementVersusLongTermCare,
      RetirementLivingVsHomecare
    }

    public enum OurServicesMenu
    {
      DiningExperience,
      ActiveLiving,
      LiveNow,
      Recreation,
      BenefitsOfSocialization,
      WellnessAndSupportServices,
      HealthAndWellness,
      SuiteTypesAndAmenities
    }

    public enum DiningExperienceAccordionPanels
    {
      [Description("High-Quality Food")]
      HighQualityFood = 1,
      [Description("Diverse Menu Choice")]
      DiverseMenuChoice = 2,
      [Description("Warm and Professional Service")]
      WarmAndProfessionalService = 3,
      [Description("Welcome Social Ambiance")]
      WelcomeSocialAmbiance = 4,
      [Description("Unique Amenities & Programs")]
      UniqueAmenitiesAndPrograms = 5
    }

    public enum LiveNowAccordionPanels
    {
      Social,
      Vocational,
      Spiritual,
      Emotional,
      Intellectual
    }

    public enum LearnMenu
    {
      [Description("Step-By-Step Resources")]
      StepByStepResources = 1,
      [Description("Beginning Your Research")]
      BeginningYourResearch = 2,
      [Description("Having The Conversation")]
      HavingTheConversation = 3,
      [Description("Finding The Right Residence")]
      FindingTheRightResidence = 4,
      [Description("Planning Your Move")]
      PlanningYourMove = 5,
      [Description("Expert Advice")]
      ExpertAdvice = 6,
      [Description("Essential Conversations With Dr Amy")]
      EssentialConversationsWithDrAmy = 7,
      [Description("Finances With Kelley Keehn")]
      FinancesWithKelleyKeehn = 8,
      [Description("Browse Topics")]
      BrowseTopics = 9,
      [Description("Budget Assistant")]
      BudgetAssistant = 10
    }

    public enum Topics
    {
      SupportingAgingParent,
      Finance,
      CareAndServices,
      ActiveLivingAndHealthAndWellness,
      HelpMeChoose
    }

    public enum SupportingAgingParentLinks
    {
      SupportingALovedOneGuidePDF,
      IsItTimeQuestionnaire,
      Articles,
      AskOurExperts
    }

    public enum FinanceLinks
    {
      BudgetAssistant,
      Articles,
      AskOurExperts
    }

    public enum CareAndServicesLinks
    {
      ExploringYourOptions,
      Articles,
      AskOurExperts
    }

    public enum ActiveLivingAndHealthAndWellnessLinks
    {
      VideoRhythmAndMovesExerciseClassVideoEnglish,
      VideoRhythmAndMovesVideoFromHamptonHouseFrench,
      VideoChartwellCruiseEnglish,
      VideoHONOUREnglish,
      VideoSeniorsDreamsComingTrueChartwellsPartnershipwithWishOfALifetimeEnglish,
      MomentsThatMatterVideosAndPhotos,
      Articles,
      AskOurExperts,
      Infographics
    }

    public enum HelpMeChooseLinks
    {
      ExploringRetirementLivingGuidePDF,
      AmIReadyQuestionnaire,
      InfographicsBenefitsOfRetirementLiving,
      AskOurExperts,
      Articles,
      StaffStories,
      ResidentStories
    }

    public enum SupportingAgingParentArticles
    {
      AChecklistOfImportantQuestionsToAskYourAgingParent,
      FourFinancialQuestionsToAskYourAgingParent,
      AnHonestConversationExploringRetirementLivingWithALovedOne,
      TellingSignsItMayBeTimeToConsiderARetirementResidence
    }

    public enum SupportingAgingParentAskOurExperts
    {
      VideoDrAmyGettingOnTheSamePageAsYourSiblings,
      VideoDrAmyTalkingToYourParentsAboutKeepingSociallyActive,
      VideoDrAmyCommunicatingWithYourSiblingsAboutYourParentsCareNeeds,
      VideoDrAmyUnderstandingThePerspectivesOfAParent,
      EssentialConversationsWithDrAmyUnderstandingAParentsDifficultReactionsToABigLifeTransition,
      EssentialConversationsWithDrAmyWhatDoIDoIfMySiblingWontHelpMeCareForOurAgingParents,
      HowDoIStartTheConversationAboutRetirementLivingWithMyLovedOne,
      WhenIsTheRightTimeForRetirementLiving
    }

    public enum FinanceArticles
    {
      FourFactorsToConsiderWhenExploringRetirementLivingAffordability,
      ExploringAndPlanningForTheCostOfRetirementLiving,
      IsRetirementLivingAnAffordablePption
    }

    public enum FinanceAskOurExperts
    {
      VideoDrAmyTalkingToYourParentsAboutTheManagementOfTheirFinances,
      VideoDrAmyGivingYourselfPermissionToLiveComfortablyInYourLaterYears,
      AskOurResidentsIsRetirementLivingAnAffordableOption
    }

    public enum CareAndServicesExploringYourOptions
    {
      VideoExploringYourOptionsWhatIsIndependentLiving,
      VideoExploringYourOptionsWhatIsIndependentSupportiveLiving,
      VideoExploringYourOptionsWhatIsAssistedLiving,
      VideoExploringYourOptionsWhatIsMemoryCare,
      VideoExploringYourOptionsWhatIsLongTermCare,
      VideoWhatIsTheDifferenceBetweenARetirementCommunityAndALongTermCareResidence
    }

    public enum CareAndServicesArticles
    {
      RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne,
      CareAndLivingOptionsOfferedByChartwellRetirementResidences
    }

    public enum CareAndServicesAskOurExperts
    {
      HowDoIknowWhichRetirementLivingSupportOptionsIsRightForMyNeeds,
      CaringForAnAgingSpouse
    }

    public enum ActiveLivingAndHealthAndWellnessArticles
    {
      ForSeniorsFriendsComeWithHealthyBenefits,
      AgingWellIsMoreThanJustGoodPhysicalHealthForSeniors,
      HowSeniorsCanBoostTheirHealthSpanForAMoreVitalRetirement,
      WhyAPositiveOutlookOnLifeAndAgingIsGoodForYourHealth,
      SevenWaysToPromoteActiveAgingAndHealthyLongevity,
      EngagingInTheArtsBoostsSeniorsPhysicalAndEmotionalHealth
    }

    public enum ActiveLivingAndHealthAndWellnessAskOurExperts
    {
      AskOurExperts,
      VideoDrAmyAssessingWhetherYourLifestyleIsHealthyAndEnjoyable,
      ExploreChartwellsRhythmAndMovesExerciseClassWithLifestyleAndProgramManagerTraceyMcDonald,
      TheRoleOfSocializationInSeniorsHealthAndWellness
    }

    public enum ActiveLivingAndHealthAndWellnessInfographics
    {
      BenefitsOfSocializationPDF,
      HealthAndWellnessPDF
    }

    public enum HelpMeChooseAskOurExperts
    {
      AskOurExperts,
      HowShouldIFurnishAndDecorateMyNewSuite,
      HowDoIKnowIfImReadyForRetirementLiving
    }

    public enum HelpMeChooseArticles
    {
      Blog,
      WhatToLookForWhileTakingATourOfARetirement,
      ThreeImportantQuestionsToAskWhileOnATourOfARetirementResidence,
      TipsForFindingTheRightRetirementResidence,
      RetirementLivingOptionsWhichCareLevelIsRightForYourLovedOne,
      SevenQuestionsToDetermineIfYouAreRetirementResidenceReady,
      WhyRetirementLivingMaySuitYouBetterThanReceivingSupportAtHome,
      ThreeHealthyReasonsToConsiderRetirementLiving,
      WhyRetirementResidenceLivingCanBeAHealthyChoicePart2,
      ResearchingRetirementLivingHereIsWhereToStart,
      TheBenefitsOfConsideringARetirementLifestyleBeforeExperiencingAHealthScare,
      TheBenefitsOfAnIndependentLivingRetirementLifestyle,
      ThreeCompellingReasonsToMoveIntoARetirementCommunity
    }

    public enum HelpMeChooseStaffStories
    {
      PeterEnglish,
      MichelFrench,
      StephanieFrench
    }

    public enum HelpMeChooseResidentStories
    {
      ResidentStories,
      LoreenAndCecilEnglish,
      AurelAndMajaEnglish,
      PierretteFrench,
      LamarcheCoupleFrench,
      JenniferEnglish,
      FlorenceFrench
    }

    public enum AboutUsMenu
    {
      Welcome,
      OurVisionMissionAndValues,
      OurLeadershipTeam,
      WishOfALifetimeCanada,
      MomentsThatMatter,
      CorporateSocialResponsibility,
      CorporateSocialResponsibilityProgram,
      HonourOurVeterans,
    }

    //--------------
    public string GetDescription(Enum GenericEnum)
    {
      Type genericEnumType = GenericEnum.GetType();
      MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
      if ((memberInfo != null && memberInfo.Length > 0))
      {
        var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        if ((_Attribs != null && _Attribs.Count() > 0))
        {
          return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;
        }
      }
      return GenericEnum.ToString();
    }
    //---------------
  }
}
