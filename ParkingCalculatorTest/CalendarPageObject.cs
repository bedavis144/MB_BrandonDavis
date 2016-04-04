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
    class CalendarPageObject
    {
        //Initialize all web elements on this page
        public CalendarPageObject()
        {
            PageFactory.InitElements(TestProperties.driver, this);
        }

        [FindsBy(How = How.Name, Using = "MonthSelector")]
        public IWebElement ddlMonth { get; set; }

        //All links on the page have tag name "a"
        [FindsBy(How = How.TagName, Using = "a")]
        public IList<IWebElement> links { get; set; }

        //Find all bold elements, this is used for reading the year
        [FindsBy(How = How.TagName, Using = "b")]
        public IList<IWebElement> boldElements { get; set; }



        public void SelectCalendarDate (month m, string d, string y)
        {
            int yInt = Convert.ToInt32(y);
            int dInt = Convert.ToInt32(d);
            int i=0;

            //year is displayed as the second bold element in the code
            IWebElement year = boldElements.ElementAt(1);

            //Select month
            ddlMonth.SelectEnumerator(m.ToString());

            //Re-initialize page due to page refresh
            PageFactory.InitElements(TestProperties.driver, this);
            year = boldElements.ElementAt(1);

            //Click the arrows in either direction according to the desired date.
            int diff = yInt - Convert.ToInt32(year.Text);
            if (diff < 0)
            {
                for (i = 0; i > diff; i--)
                {
                    GetLink("DecYear").Click();
                }
            }
            else if (diff > 0)
            {
                for (i = 0; i < diff; i++)
                {
                    GetLink("IncYear").Click();
                }
            }
            else
            { }

            string datestring = (Convert.ToUInt32(m).ToString() + "/" + dInt.ToString() + "/" + yInt.ToString());

            //Click the correct day of the month
            GetLink(datestring).Click();
        }


        //Searches through all link elements for the element that contains a string pattern in its href attribute
        public IWebElement GetLink (string pattern)
        {
            foreach (IWebElement link in links)
            {               
                if (Regex.IsMatch(link.GetAttribute("href"), pattern))
                {
                    return link;
                }
            }
            return links.ElementAt(links.Count - 1);
        }
    }
}
