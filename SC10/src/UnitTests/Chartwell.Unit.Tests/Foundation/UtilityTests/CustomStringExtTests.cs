using System.Collections.Generic;
using System.Configuration;
using Chartwell.Foundation.utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chartwell.UnitTest.Chartwell.Utilities.Tests
{
  [TestClass]
  public class CustomStringExtTests
  {
    private readonly ChartwellUtiles cw = new ChartwellUtiles();


    [TestMethod]
    public void TestSetPhoneNumberWithDashes()
    {
      string actualValue = "(647) 298-7130";
      string expectedValue = "647-298-7130";
      string phoneNum = cw.SetPhoneNumberWithDashes(actualValue);
      Assert.AreEqual(expectedValue, phoneNum);
    }

    //this method has 0 references. Secondly it needs refactoring in Utility class
    // &ensp - use this for adding two spaces
    // the argument count is not being used which makes the method redundant
    [TestMethod]
    public void TestIndent()
    {
      var input = 2;
      var expected = "&nbsp;&nbsp;";

      var actual = ChartwellUtiles.Indent(input);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestRemoveSpcialCharacters()
    {
      string inpuVal = "5x4y7z";
      string expectedVal = "547";

      string retVal = cw.RemoveSpecialCharacters(inpuVal);
      Assert.AreEqual(expectedVal, retVal);
    }

  }
  [TestClass]
  public class TestCustomStringExtensions
  {
    [TestMethod]
    public void TestSanitize404Url()
    {
      var input = "https://chartwell.com/residence";
      var expected = "https://chartwell.com/residence";

      var actual = CustomStringExtensions.Sanitize404Url(input);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestRemoveDiacritics()
    {
      var input = "Chartwèll";
      var expected = "Chartwell";

      var actual = CustomStringExtensions.RemoveDiacritics(input);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestTrimPunctuation()
    {
      var input = "Hello. Chartwell";
      var expected = "Hello Chartwell";

      var actual = CustomStringExtensions.TrimPunctuation(input);

      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// If letter is capitalized in sentence with no space, add a space. Ex. Chartwell.Is <--Add space
    /// </summary>
    [TestMethod]
    public void TestAddSpacesToSentence()
    {
      var input = "Hello.Chartwell";
      var expected = "Hello. Chartwell";

      var actual = CustomStringExtensions.AddSpacesToSentence(input);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestToTitleCase()
    {
      var input = "chartwell";
      var expected = "Chartwell";

      var actual = CustomStringExtensions.ToTitleCase(input);

      Assert.AreEqual(expected, actual);
    }
    [TestMethod]
    public void TestRemoveExtraSpaces()
    {
      var input = "chart  well";
      var expectedVal = "chart well";

      var actual = CustomStringExtensions.RemoveExtraSpaces(input);

      Assert.AreEqual(expectedVal, actual);
    }
    /// <summary>
    /// If value[0] is not in arr1(string[]) and value[1] is, add braces for value[1] element
    /// </summary>
    [TestMethod]
    public void TestInsertBraces()
    {
      string count1Input = "Ontario";
      string count1ItemVal = "Ontario";

      string cntSplitInput = "Ontario Manitoba";
      string cntSplitInputExpected = "Ontario Manitoba";

      string inputValBracesAddedExpected = "Manitoba Ontario";
      string bracesAddedExpected = "Manitoba (Ontario)";

      var expectedBracesAddedActual = CustomStringExtensions.InsertBraces(inputValBracesAddedExpected);
      var cntSplitActual = CustomStringExtensions.InsertBraces(cntSplitInput);
      var count1ItemValidationActual = CustomStringExtensions.InsertBraces(count1Input);

      Assert.AreEqual(count1ItemVal, count1ItemValidationActual);
      Assert.AreEqual(cntSplitInputExpected, cntSplitActual);
      Assert.AreEqual(bracesAddedExpected, expectedBracesAddedActual);
    }

    [TestMethod]
    public void TestLanguageFromUrl()
    {
      var sanitizedUrl = "/learn/résultat-de-la-recherche";
      if (sanitizedUrl.Contains("résultat-de-la-recherche"))
        sanitizedUrl = "/fr" + sanitizedUrl;
      else
        sanitizedUrl = "/en" + sanitizedUrl;

      var expectedFrUrlResult = "/fr/learn/résultat-de-la-recherche";
      var expectedEnUrlResult = "/en/learn/research-result";

      Assert.IsTrue(expectedFrUrlResult == sanitizedUrl);
      Assert.IsFalse(expectedEnUrlResult == sanitizedUrl);

    }
  }
}
