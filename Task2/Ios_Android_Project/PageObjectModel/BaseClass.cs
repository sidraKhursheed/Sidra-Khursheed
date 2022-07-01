using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;
using Ios_Android_Project.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Service.Options;

using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Net;

namespace Ios_Android_Project.PageObjectModel
{
    public class BaseClass
    {
        public AppiumDriver<AppiumWebElement> mobiledriver;
        //public IOSDriver<IOSElement> iosDriver;
       // public AndroidDriver<AppiumWebElement> androiddriver;
        public TimeSpan ts = TimeSpan.FromSeconds(2);
        AppiumLocalService appiumLocalService;
        public BasePageFindElement Base;
        public TestContext TestContext { get; set; }
        public ExtentReports extent = null;


        public BaseClass()
        {
        }

        [Obsolete]
        private void InvokeMethodWithConfig(IEnumerable<IConfigurationSection> config, Action TestMethod)
        {
            if (config !=null){

                foreach (var configItem in config?.Select(x => new { key = x.Key, value = x.Value })?.ToList())
                {
                    if (TestContext.Properties.Contains(configItem.key))
                    {
                        TestContext.Properties.Remove(configItem.key);
                    }
                    TestContext.Properties.Add(configItem.key, configItem.value);
                }

                try
                {
                    TestInit();
                    TestMethod.Invoke();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    TestClean();
                }
            }
        }

       

        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsetting.json").Build();
        protected string Selected_OS;
        protected string BoxName;
        protected string EnvName;
        protected string DBType;
        protected string UserType;

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        [Obsolete]
        protected void Run(Action TestMethod)
        {
            var methodBase = (new System.Diagnostics.StackTrace())?.GetFrame(1)?.GetMethod();
            var mode = (methodBase.GetCustomAttributes(typeof(TestAttribute), true).FirstOrDefault() as TestAttribute)?.UserMode;

            if (mode == null)
            {
                throw new Exception("Mode Not Defined in TestAttribute");
            }
            Console.WriteLine("Start Running " + methodBase.Name);
            var SelectedMode = config.GetSection("SelectedMode").Value;
            
                var Selectedmode = config.GetSection(SelectedMode)?.GetChildren()?.Select(x => new { key = x.Key, value = x.Value })?.ToList();

                var SelectedEnvs = Selectedmode.Where(x => x.key == SelectedMode+"_SelectedEnvs").Select(x => x.value).SingleOrDefault().Split(',').Select(e => e.ToLowerInvariant().Trim())?.ToList();
                var SelectedDevices = Selectedmode.Where(x => x.key == SelectedMode + "_SelectedDevice").Select(x => x.value).SingleOrDefault().Split(',').Select(e => e.ToLowerInvariant().Trim())?.ToList();
                var SelectedBoxes = Selectedmode.Where(x => x.key == SelectedMode + "_SelectedBoxes").Select(x => x.value).SingleOrDefault().Split(',').Select(e => e.ToLowerInvariant().Trim())?.ToList();
                var SelectedDB = Selectedmode.Where(x => x.key == SelectedMode + "_SelectedDB").Select(x => x.value).SingleOrDefault().Split(',').Select(e => e.ToLowerInvariant().Trim())?.ToList();

                foreach (var SelectedOS in SelectedDevices)
                {
                    Selected_OS = SelectedOS;

                    foreach (var SelectedEnv in SelectedEnvs)
                    {
                        EnvName = SelectedEnv;

                      //  string relativeFilePath;

                    string settingName;
                        switch (mode)
                        {
                            case UserMode.Normal:
                                foreach (var SelectedBox in SelectedBoxes)
                                {
                                    BoxName = SelectedBox;
                                    UserType = "Normal";
                                    foreach (var SelectedDBType in SelectedDB)
                                    {

                                     DBType = SelectedDBType;

                                     settingName = SelectedBox + "-" + SelectedDBType + "-" + SelectedEnv ;

                                    var Gerowersetting = config.GetSection(settingName)?.GetChildren();

                                      //relativeFilePath = "RunSettingFiles\\" + SelectedBox + "-" + SelectedDBType + "-" + SelectedEnv + ".runsettings";
                                        InvokeMethodWithConfig(Gerowersetting, TestMethod);
                                    }
                                }
                                break;
                            case UserMode.Admin:

                                break;
                            case UserMode.SuperAdmin:
                                UserType = "SuperAdmin";
                            var SuperAdminsetting = config.GetSection("SuperAdminFile-" + SelectedEnv )?.GetChildren();
                            //relativeFilePath = "RunSettingFiles\\SuperAdminFile-" + SelectedEnv + ".runsettings";
                            InvokeMethodWithConfig(SuperAdminsetting, TestMethod);

                                break;
                            case UserMode.SystemAdmin:
                                throw new Exception("Config not found");
                                break;
                            default:
                                throw new Exception("Config not found");
                                break;
                        }
                    }

                }
        }



        //  [TestInitialize]

        [Obsolete]
        public void TestInit()
        {
            

            if (Selected_OS == "ios")
            {
                startupIos();
            }

            else
                StartAppAndroid();

            Base = new BasePageFindElement(mobiledriver);

            ExtentStart();

            LaunchApplication();
        }
         //[TestCleanup]
        public void TestClean()
        {
            mobiledriver.CloseApp();
            Console.WriteLine("Closing the Application");
            appiumLocalService.Dispose();
            Console.WriteLine("Closing Appium server");
            ExtentClose();
        }

        public void LaunchApplication()
        {
            IWebElement getStarted = mobiledriver.FindElementById("com.monefy.app.lite:id/buttonContinue");
            Console.WriteLine("Get started button found");
            getStarted.Click();
            Thread.Sleep(ts);
            IWebElement amazing = mobiledriver.FindElementById("com.monefy.app.lite:id/buttonContinue");
            Console.WriteLine("Amazing button found");
            amazing.Click();
            Thread.Sleep(ts);
            IWebElement iAmReady = mobiledriver.FindElementById("com.monefy.app.lite:id/buttonContinue");
            Console.WriteLine("I am ready button found");
            iAmReady.Click();
            Thread.Sleep(ts);
            IWebElement closeBtn = mobiledriver.FindElementById("com.monefy.app.lite:id/buttonClose");
            Console.WriteLine("Close button click on Claim pages");
            closeBtn.Click();
            Console.WriteLine("Main Dashboard loaded sucessfully");

        }

            public void startupIos()
        {
            var capabilties = new AppiumOptions();

            capabilties.AddAdditionalCapability(IOSMobileCapabilityType.BundleId, "com.example.calculator");
            capabilties.AddAdditionalCapability(MobileCapabilityType.PlatformName, "ios");
            capabilties.AddAdditionalCapability(MobileCapabilityType.AutoWebview, "true");//for webview
            capabilties.AddAdditionalCapability(MobileCapabilityType.DeviceName, "iphone 11 Pro Max");
            capabilties.AddAdditionalCapability(MobileCapabilityType.AutomationName, "XCUITest");
            capabilties.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "14.4");
            capabilties.AddAdditionalCapability(MobileCapabilityType.Udid, "926C5081-E748-4EB5-BDB7-FC4126F641D5");
            capabilties.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, "2000");
            capabilties.AddAdditionalCapability("connectHardwareKeyboard", "true");

            //Defing option for server
            var serveroptions = new OptionCollector();
            var relaxedSecurityOption = new KeyValuePair<string, string>("--relaxed-security", "");
            var crossOrigin = new KeyValuePair<string, string>("--allow-cors", "true");
            FileInfo log = new FileInfo("Log.txt");
            serveroptions.AddArguments(relaxedSecurityOption);
            serveroptions.AddArguments(crossOrigin);
            serveroptions.AddArguments(GeneralOptionList.OverrideSession());
            serveroptions.AddArguments(GeneralOptionList.PreLaunch());

            //Defining Appium server --need to pass the node path and appium path from mac machine
            appiumLocalService = new AppiumServiceBuilder().
            UsingDriverExecutable(new FileInfo("")).
            WithAppiumJS(new FileInfo("")). 
            UsingPort(4723).WithIPAddress("127.0.0.1").
            WithArguments(serveroptions).WithLogFile(log)
            .Build();

            //Starting appium server programatically
            appiumLocalService.Start();
            Console.WriteLine("Appium started");

            //Initilazing Driver
            mobiledriver = new IOSDriver<AppiumWebElement>(appiumLocalService, capabilties, TimeSpan.FromMinutes(5));
             mobiledriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(90);

        }

        private void StartAppAndroid()
        {
            System.Environment.SetEnvironmentVariable("ANDROID_HOME", @"C:\Users\HP PROBOOK\AppData\Local\Android\Sdk");
            System.Environment.SetEnvironmentVariable("JAVA_HOME", @"C:\Program Files\Java\jdk1.8.0_291");

            Console.WriteLine("Application Started");
            var capabilities = new AppiumOptions();
            // automatic start of the emulator if not running
            capabilities.AddAdditionalCapability(AndroidMobileCapabilityType.Avd, "Pixel_5_API_28");
            capabilities.AddAdditionalCapability(AndroidMobileCapabilityType.AvdArgs, "-no-boot-anim -no-snapshot-load");
            capabilities.AddAdditionalCapability(MobileCapabilityType.NoReset, false);
            capabilities.AddAdditionalCapability(MobileCapabilityType.FullReset, true);
            

            // connecting to a device or emulator
            capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Pixel_5_API_28");
            capabilities.AddAdditionalCapability(MobileCapabilityType.AutomationName, "UiAutomator2");

            // specifyig which app we want to install and launch
            var packagePath = @"E:\GitProjects\Mobile-Android-Ios-autiomation-with-MStest-using-CSharp-N26\Mobile-Android-Ios-autiomation-with-MStest-using-CSharp\DemoApk\monefy-1-16-0.apk";
            packagePath = Path.GetFullPath(packagePath);
            Console.WriteLine($"Package path: {packagePath}");
            capabilities.AddAdditionalCapability(MobileCapabilityType.App, packagePath);

            capabilities.AddAdditionalCapability(AndroidMobileCapabilityType.AppPackage, "com.monefy.app.lite");
            capabilities.AddAdditionalCapability(AndroidMobileCapabilityType.AppActivity, "com.monefy.activities.main.MainActivity_");
           
            // specify startup flags appium server to execute adb shell commands
            var serveroptions = new OptionCollector();
            var relaxedSecurityOption = new KeyValuePair<string, string>("--relaxed-security", "");

            serveroptions.AddArguments(relaxedSecurityOption);
            appiumLocalService = new AppiumServiceBuilder().UsingAnyFreePort().WithArguments(serveroptions).Build();
            appiumLocalService.Start();
            mobiledriver = new AndroidDriver<AppiumWebElement>(appiumLocalService, capabilities);
            mobiledriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
        }

        [Obsolete]
        public void ExtentStart()
        {
            string pathProject = AppDomain.CurrentDomain.BaseDirectory;
            string pathScreen = pathProject.Replace("\\bin\\Debug", "");
            string path = pathScreen + "\\Android_IOS_Reports";

            Directory.CreateDirectory(path);
            string ReportName = Path.Combine(path, "ExtentReport" + "_");

            extent = new ExtentReports();
           
            ExtentV3HtmlReporter htmlReporter = new ExtentV3HtmlReporter(ReportName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html");

            extent.AttachReporter(htmlReporter);

            string hostname = Dns.GetHostName();
            OperatingSystem os = Environment.OSVersion;

            extent.AddSystemInfo("Operating System", os.ToString());
            extent.AddSystemInfo("HostName", hostname);
            extent.AddSystemInfo("Environment", "Production");
            extent.AddSystemInfo("Device", "Google Nexus Device");
           

        }

        public void ExtentClose()
        {
           extent.Flush();
        }

        public string TakesScreenshot(string FileName)
        {
            string pathProject = AppDomain.CurrentDomain.BaseDirectory;
            string pathScreen = pathProject.Replace("\\bin\\Debug", "");
            string path = pathScreen + "\\AndroidScreenShots\\";


            StringBuilder TimeAndDate = new StringBuilder(DateTime.Now.ToString());
            TimeAndDate.Replace("/", "_");
            TimeAndDate.Replace(":", "_");
            TimeAndDate.Replace(" ", "_");

            string imageName = FileName + TimeAndDate.ToString();

            Directory.CreateDirectory(path);
            string imageFileName = Path.Combine(path, imageName + "." + System.Drawing.Imaging.ImageFormat.Jpeg);

            ((ITakesScreenshot)mobiledriver).GetScreenshot().SaveAsFile(imageFileName);
            return imageFileName;
        }

    }
}
