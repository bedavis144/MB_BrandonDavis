using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParkingCalculatorTest
{

    class ParkingCalcPageObject
    {
        //Initialize all web elements on this page
        public ParkingCalcPageObject()
        {
            PageFactory.InitElements(TestProperties.driver, this);
        }

        [FindsBy(How = How.Id, Using = "Lot")]
        public IWebElement ddlLot { get; set; }

        [FindsBy(How = How.Id, Using = "EntryTime")]
        public IWebElement txtEntryTime { get; set; }

        [FindsBy(How = How.Id, Using = "ExitTime")]
        public IWebElement txtExitTime { get; set; }

        [FindsBy(How = How.Name, Using = "EntryTimeAMPM")]
        public IList<IWebElement> rbtnEntryTimeAMPM { get; set; }

        [FindsBy(How = How.Name, Using = "ExitTimeAMPM")]
        public IList<IWebElement> rbtnExitTimeAMPM { get; set; }

        [FindsBy(How = How.Id, Using = "EntryDate")]
        public IWebElement txtEntryDate { get; set; }

        [FindsBy(How = How.Id, Using = "ExitDate")]
        public IWebElement txtExitDate { get; set; }

        [FindsBy(How = How.Name, Using = "Submit")]
        public IWebElement btnSubmit { get; set; }

        //cost and duration have tag 'b' for bold
        [FindsBy(How = How.TagName, Using = "b")]
        public IList<IWebElement> calculatedOutputs { get; set; }

        //Entry and Exit calendars have Tag 'a' for link
        [FindsBy(How = How.TagName, Using = "a")]
        public IList<IWebElement> calendars { get; set; }



        public void CompleteForm (string lot, string entryTime, timeAMPM entryTimeAMPM, string entryDate, string exitTime, timeAMPM exitTimeAMPM, string exitDate)
        {
            //Enter all input elements on form         
            ddlLot.SelectEnumerator(lot);
            txtEntryTime.EnterText(entryTime);
            rbtnEntryTimeAMPM.rbtnClick(entryTimeAMPM);        
            txtEntryDate.EnterText(entryDate);
            txtExitTime.EnterText(exitTime);
            rbtnExitTimeAMPM.rbtnClick(exitTimeAMPM);
            txtExitDate.EnterText(exitDate);
            Console.WriteLine("Completed Form");                  
        }

        
        public void Submit()
        {
            btnSubmit.btnClick();
            Console.WriteLine("Clicked 'Calculate'");
        }
        


        public void SelectCalendar(visit svisit, month m, string d, string y)
        {
            //save current window handle
            string mainWindowHandle = TestProperties.driver.CurrentWindowHandle;
           
            //Click the calendar icon according to entry or exit
            //Create a page object for the popup window
            CalendarPageObject calPage = OpenCalendar(svisit);

            //click the date on the calendar popup
            calPage.SelectCalendarDate(m, d, y);

            //Go back to the main window handle
            TestProperties.driver.SwitchTo().Window(mainWindowHandle);
                
        }

        public CalendarPageObject OpenCalendar(visit v)
        {
            string calendarWindowHandle = string.Empty;
            
            //Obtain a list of all window handles before the icon was clicked
            ICollection<string> mainWindowHandles = TestProperties.driver.WindowHandles;

            //Click the caledar icon according to entry or exit
            calendars.CalendarClick(v);

            //Obtain a list of all handles after the icon was clicked, excluding the originally opened handles
            IEnumerable<string> newHandles = TestProperties.driver.WindowHandles.Except(mainWindowHandles);

            //Save new popup handle
            if (newHandles.Count() > 0)
            {
                calendarWindowHandle = newHandles.ElementAt(0);
            }

            //enable the popup handle as the window of operation
            TestProperties.driver.SwitchTo().Window(calendarWindowHandle);

            //initialize this page object and return its value
            return new CalendarPageObject();
        }

        public void ReadFormEntries ()
        {
            //Send all form entries to the output
            Console.WriteLine("Form Values:");
            Console.WriteLine("   Lot: " + ddlLot.GetTextfromDDL());
            Console.WriteLine("   Entry Time: " + txtEntryTime.GetText() + " " + rbtnEntryTimeAMPM.GetValueRadio());
            Console.WriteLine("   Entry Date: " + txtEntryDate.GetText());
            Console.WriteLine("   Exit Time: " + txtExitTime.GetText() + " " + rbtnExitTimeAMPM.GetValueRadio());
            Console.WriteLine("   Exit Date: " + txtExitDate.GetText());
        }

        public string ReadCost()
        {
            //Cost is the first of the bold elements on the form
            return calculatedOutputs.ElementAt(0).Text;
        }

        public string ReadDuration()
        {
            //Duration is the second of the bold elements on the form
            return calculatedOutputs.ElementAt(1).Text;
        }

        
        public int TestCost(string compare)
        {
            //read the web element and output value
            string cost = ReadCost();
            Console.WriteLine("Cost: " + cost);
            
            //Compare with expected string and output pass or fail
            if (cost == compare)
            {
                Console.WriteLine("PASS");
                TestProperties.setPassFail(true); ;
                return 1;
            }
            else
            {
                Console.WriteLine("FAIL");
                //If the test fails, indicates what value you were looking for
                Console.WriteLine("   Expected: " + compare);
                TestProperties.setPassFail(false); ;
                return 0;
            }
        }

        public int TestCostError()
        {
            //Same as TestCost(), except instead of looking for an exact string match, 
            //it looks for the string pattern 'ERROR!'

            string cost = ReadCost();                        
            Console.WriteLine("Cost: " + cost);

            if (Regex.IsMatch(cost, "ERROR!"))
            {
                Console.WriteLine("PASS");
                TestProperties.setPassFail(true); ;
                return 1;
            }
            else
            {
                Console.WriteLine("Duration: " + ReadDuration().Trim());
                Console.WriteLine("FAIL");
                Console.WriteLine("   Expected: " + "ERROR!...");
                TestProperties.setPassFail(false); ;
                return 0;
            }
        }

        public int TestDuration(string compare)
        {
            //Same as TestCost(), except checks duration instead of cost
            //also, removes whitespace from duration txt

            string duration = ReadDuration().Trim();
            Console.WriteLine("Duration: " + duration);
            
            if (duration == compare)
            {
                Console.WriteLine("PASS");
                TestProperties.setPassFail(true); ;
                return 1;
            }
            else
            {
                Console.WriteLine("FAIL");
                Console.WriteLine("   Expected: " + compare);
                TestProperties.setPassFail(false); ;
                return 0;
            }
        }

        public int TestTime(visit v, string compare)
        {
            string time = string.Empty;
            
            //Reads text from entry or exit            
            if (v == visit.entry)
            {
                time = txtEntryTime.GetText().Trim();
            }
            else
                time = txtExitTime.GetText().Trim();

            //tests text for string match
            if (time == compare)
            {
                Console.WriteLine(v.ToString() + " " + time + " " + ", PASS");
                TestProperties.setPassFail(true);
                return 1;
            }
            else
            {
                Console.WriteLine(v.ToString() + " " + time + " " + ", FAIL");
                Console.WriteLine("   Expected: " + compare);
                TestProperties.setPassFail(false);
                return 0;
            }
        }
    }
}
