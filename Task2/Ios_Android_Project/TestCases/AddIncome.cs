using System;
using System.Threading;
using AventStack.ExtentReports;
using Ios_Android_Project.PageObjectModel;
using Ios_Android_Project.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Ios_Android_Project
{
    [TestClass]
    public class AddIncome:BaseClass
    {
        
        public void AddIncomeTest()
        {
            string incomeButton = "com.monefy.app.lite:id/income_button_title";
            ExtentTest test = extent.CreateTest("DummyApplicationLaunch").Info("Test Started");
            IWebElement expnseButtonElement = Base.ElementbyId(incomeButton);
            test.Info("Add Income Button Found");
            expnseButtonElement.Click();
            test.Info("Add Income Button Clicked");

            string amountField = "com.monefy.app.lite:id/amount_text";
            IWebElement amountFieldElement = Base.ElementbyId(amountField);
            test.Info("Amount field Found");
            amountFieldElement.Click();
            test.Info("Amount field clicked");
            string amountValue = "com.monefy.app.lite:id/buttonKeyboard1";
            Base.ElementbyId(amountValue).Click();
            test.Info("Amount value added as 1");
            string noteField = "com.monefy.app.lite:id/textViewNote";
            IWebElement noteFieldElement = Base.ElementbyId(noteField);
            noteFieldElement.Click();
            noteFieldElement.SendKeys("test");
            test.Info("Note entered");
            mobiledriver.HideKeyboard();
            string chooseCategory = "com.monefy.app.lite:id/keyboard_action_button";
            IWebElement chooseCategoryElement = Base.ElementbyId(chooseCategory);
            chooseCategoryElement.Click();
            string selectCategoryDeposits = "//*[@class='android.widget.TextView' and @text='Deposits']";
            IWebElement selectCategoryDepositsElement = Base.ElementByXpath(selectCategoryDeposits);
            selectCategoryDepositsElement.Click();

            string successMessage = "com.monefy.app.lite:id/snackbar_text";
            IWebElement successMessageElement = Base.ElementbyId(successMessage);
            string Message = successMessageElement.Text;
            Assert.AreEqual(Message, "Deposits: $1.00 added");

            if (Message == "Deposits: $1.00 added")
            {
                test.Pass("Test case Pass", MediaEntityBuilder.CreateScreenCaptureFromPath(TakesScreenshot("ScreenShot")).Build());
                test.Info("Button assertion passed");

            }
            else
            {
                test.Fail("Test case failed", MediaEntityBuilder.CreateScreenCaptureFromPath(TakesScreenshot("ScreenShot")).Build());
                test.Info("Assertion failed");
            }
            test.Info("Test Finished");
        }

        [TestAttribute(UserMode = UserMode.Normal)]

        [TestCategory("Web TestCases Production New")]
        [Obsolete]
        public void AddIncomes()
        {
            Run(AddIncomeTest);
        }
    }
}
