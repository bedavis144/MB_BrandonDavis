using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingCalculatorTest
{
    public enum timeAMPM
    {
        AM,
        PM
    }

    public enum month
    {
        January = 1,
        February = 2,
        March = 3,
        April=  4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum visit
    {
        entry,
        exit
    }
    class TestProperties
    {
        public static IWebDriver driver { get; set; }

        private static bool passFail { get; set; }

        public static void initPassFail ()
        {
            passFail = true;
        }

        public static bool getPassFail()
        {
            return passFail;
        }

        public static void setPassFail (bool testResult)
        {
            //Only write passFail if the test hasn't already failed
            if (passFail)
            {
                passFail = testResult;
            }     
        }        
    }
}
