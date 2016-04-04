using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingCalculatorTest
{
    public static class GetMethods
    {

        // Extended command for reading text
        public static string GetText(this IWebElement element)
        {
            return element.GetAttribute("value");
        }


        // Extended command for reading DropDown Text
        public static string GetTextfromDDL(this IWebElement element)
        {
            return new SelectElement(element).AllSelectedOptions.SingleOrDefault().Text;
        }


        // Extended command for reading radio button value
        public static string GetValueRadio(this IList<IWebElement> elements)
        {
            for (int i = 0; i <= elements.Count(); i++)
            {
                if (elements.ElementAt(i).Selected)
                {
                    return elements.ElementAt(i).GetAttribute("value");
                }
            }
            return string.Empty;
        }
    }
}
