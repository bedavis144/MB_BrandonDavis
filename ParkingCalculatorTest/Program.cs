using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;


//Created by Brandon Davis 
//For MINDBODY, Inc Software Test Automation Engineer Application
//March-April 2016


namespace ParkingCalculatorTest
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        [SetUp]
        public void Initialize()
        {
            //Create the reference for our browser
            TestProperties.driver = new ChromeDriver();

            //Initialize the test result as PASS
            TestProperties.initPassFail();

            //Navigate to Parking Calculator Web Page
            TestProperties.driver.Navigate().GoToUrl("http://adam.goucher.ca/parkcalc/index.php");

            //Write to output
            Console.WriteLine("Opened URL");
        }

        [Test]
        public void TestCase1()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form with Test Case 1 values
            page.CompleteForm("Short-Term Parking", "10:00", timeAMPM.PM, "01/01/2014", "11:00", timeAMPM.PM, "01/01/2014");

            //Write Entries to output
            page.ReadFormEntries();

            //Calculate and Test against expected strings
            page.Submit();
            page.TestCost("$ 2.00");
            page.TestDuration("(0 Days, 1 Hours, 0 Minutes)");
        }

        [Test]
        public void TestCase2()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form with Test Case 2 values
            page.ddlLot.SelectEnumerator("Long-Term Surface Parking");       
            page.SelectCalendar(visit.entry, month.January, "01", "2014");
            page.SelectCalendar(visit.exit, month.February, "01", "2014");

            //Write Entries to output
            page.ReadFormEntries();

            //Calculate and Test against expected strings
            page.Submit();
            page.TestCost("$ 270.00");
            page.TestDuration("(31 Days, 0 Hours, 0 Minutes)");
        }

        [Test]
        public void TestCase3()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form with Test Case 3 values
            page.ddlLot.SelectEnumerator("Short-Term Parking");
            page.txtEntryDate.EnterText("01/02/2014");
            page.txtExitDate.EnterText("01/01/2014");

            //Write Entries to outpout
            page.ReadFormEntries();

            //Calculate and Test against expected string
            page.Submit();
            page.TestCost("ERROR! YOUR EXIT DATE OR TIME IS BEFORE YOUR ENTRY DATE OR TIME");

        }


        //Test Case 4 removes the text from all text box inputs.
        //ASSUMPTION: The calculator should result in an error when no date or times are entered.
        [Test]
        public void TestCase4()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form, leaving dates and times empty
            page.CompleteForm("Economy Parking", "", timeAMPM.PM, "", "", timeAMPM.PM, "");

            //Write Entries to output
            page.ReadFormEntries();

            //Calculate and Test for a calculation error
            page.Submit();
            page.TestCostError();

        }

        //Test Case 5 sets entry time later than the exit time for the same day.
        //ASSUMPTION: The calculator should result in an error because the exit time cannot be before entry time.
        [Test]
        public void TestCase5()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form, Making entry time later than exit time
            page.CompleteForm("Long-Term Garage Parking", "3:00", timeAMPM.PM, "03/19/1991", "3:00", timeAMPM.AM, "03/19/1991");

            //Write Entries to output
            page.ReadFormEntries();

            //Calculate and Test for a calculation error
            page.Submit();
            page.TestCostError();

        }

        //Test Case 6 verifies that the correct military time conversion occurs when standard time is entered
        //ASSUMPTION: True military time should appear, meaning 0:00 hours for 12 midnight and 12:00 hours for 12noon
        //ASSUMPTION: AM/PM radio buttons are not considered in the result because military time doesn't use AM/PM
        [Test]
        public void TestCase6()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Initialize form at 0:00 hours
            page.CompleteForm("Short-Term Parking", "12:00", timeAMPM.AM, "03/19/1991", "12:00", timeAMPM.AM, "03/19/1991");

            //initialize strings to write to output
            string time = string.Empty;
            string amPm = string.Empty;
            string militaryTime = string.Empty;

            //Cycle through all 24 hours and enter standard time
            for (int i=0; i<24; i++)
            {
                if (i == 0)
                {
                    time = "12:00";
                    amPm = "AM";
                    militaryTime =  "0:00";
                    page.rbtnEntryTimeAMPM.rbtnClick(timeAMPM.AM);
                    page.rbtnExitTimeAMPM.rbtnClick(timeAMPM.AM);
                }
                else if (i < 12)
                {
                    time = i.ToString() + ":00";
                    amPm = "AM";
                    militaryTime = time;
                    page.rbtnEntryTimeAMPM.rbtnClick(timeAMPM.AM);
                    page.rbtnExitTimeAMPM.rbtnClick(timeAMPM.AM);

                }
                else if (i == 12)
                {
                    time = "12:00";
                    amPm = "PM";
                    militaryTime = time;
                    page.rbtnEntryTimeAMPM.rbtnClick(timeAMPM.AM);
                    page.rbtnExitTimeAMPM.rbtnClick(timeAMPM.AM);
                }
                else if (i > 12)
                {
                    time = (i - 12).ToString() + ":00";
                    amPm = "PM";
                    militaryTime = i.ToString() + ":00";
                    page.rbtnEntryTimeAMPM.rbtnClick(timeAMPM.PM);
                    page.rbtnExitTimeAMPM.rbtnClick(timeAMPM.PM);
                }
                else
                {
                    time = string.Empty;
                    amPm = string.Empty;
                }
                    
                //Enter standard time into the Entry and Exit text inputs
                page.txtEntryTime.EnterText(time);
                page.txtExitTime.EnterText(time);
                Console.WriteLine("Entered Entry and Exit time: " + time + amPm);

                //Calculate and test for the correct military time equivalent
                page.Submit();
                page.TestTime(visit.entry, militaryTime);
                page.TestTime(visit.exit, militaryTime);
            }
        }

        //Test Case 7 enters military time and checks that the correct military time stays.
        //Entry and Exit times are one hour apart.  Checks the cost of $2:00 and duration of (0 Days, 1 Hours, 0 Minutes)
        //ASSUMPTION: If military time is entered, and AM/PM are the same for both entry and exit time,
        //the calculator should result in the correct duration and cost.
        [Test]
        public void TestCase7()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Initialize variables for test
            string entryTime = string.Empty;
            string exitTime = string.Empty;
            timeAMPM entryTimeAMPM = timeAMPM.AM;
            timeAMPM exitTimeAMPM = timeAMPM.AM;

            //Cycle through all 24 hours twice, for AM and PM entries
            for (int j=0; j<2; j++)
            {
                if (j == 0)
                {
                    entryTimeAMPM = timeAMPM.AM;
                    exitTimeAMPM = timeAMPM.AM;
                }
                else
                {
                    entryTimeAMPM = timeAMPM.PM;
                    exitTimeAMPM = timeAMPM.PM;
                }

                for (int i = 0; i < 24; i++)
                {
                    //entry time and exit time 1 hour difference
                    entryTime = i.ToString() + ":00";
                    exitTime = (i + 1).ToString() + ":00";

                    //Enter entry and exit times into form
                    page.CompleteForm("Short-Term Parking", entryTime, entryTimeAMPM, "02/01/2011", exitTime, exitTimeAMPM, "02/01/2011");

                    //Calculate and test for correct times, cost, and duration
                    page.Submit();
                    page.TestTime(visit.entry, entryTime);
                    page.TestTime(visit.exit, exitTime);
                    page.TestCost("$ 2.00");
                    page.TestDuration("(0 Days, 1 Hours, 0 Minutes)");
                }
            }            
        }

        //Test Case 8 tests for incorrect Date formatting: enter 'hello' in the Entry and Exit Dates
        //ASSUMPTION: The calculator should result in an error when date is incorrectly entered.
        [Test]
        public void TestCase8()
        {
            //Create page object for Parking Calculator
            ParkingCalcPageObject page = new ParkingCalcPageObject();

            //Complete form with incorrect date entry for exit date
            page.CompleteForm("Long-Term Garage Parking", "3:00", timeAMPM.AM, "hello", "3:00", timeAMPM.AM, "hello");

            //Write entries to outpout
            page.ReadFormEntries();

            //Calculate and test for a calculation error
            page.Submit();
            page.TestCostError();
        }

        [TearDown]
        public void TearDown()
        {
            //Close the Browser
            TestProperties.driver.Close();

            //Write to output
            Console.WriteLine("Closed Browser");

            //The any tests failed, indicate an NUnit failure
            if (!TestProperties.getPassFail())
            {
                Assert.Fail("See Output for details.");
            }
        }


    }
}
