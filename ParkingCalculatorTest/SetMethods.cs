using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingCalculatorTest
{
    public static class SetMethods
    {

        // Extended method for entering text
        public static void EnterText(this IWebElement element, string value)
        {
            //Clears text and writes new value
            element.Clear();
            if (value != "")
            element.SendKeys(value);
        }

        // Extended method for clicking a button
        public static void btnClick(this IWebElement element)
        {
            element.Submit();
        }

        //Extended method for selecting the radio button
        public static void rbtnClick(this IList<IWebElement> elements, timeAMPM value)
        {
            //clicks the radio button according to AM or PM
            int i;
            if (value == timeAMPM.AM)
                i = 0;
            else
                i = 1;
         
            elements.ElementAt(i).Click();
        }


        /// Exteneded method for selecting drop down control
        public static void SelectEnumerator(this IWebElement element, string value)
        {
            new SelectElement(element).SelectByText(value);
        }

        /// Extended method for selecting Calendar
        public static void CalendarClick(this IList<IWebElement> element, visit value)
        {
            //clicks the calendar icon according to the visit type
            int i;
            if (value == visit.entry)
                i = 0;
            else
                i = 1;

            element.ElementAt(i).Click();
        }
    }
}
