using System;
using System.Text.RegularExpressions;

namespace Fabio.SharpTools.String
{

    /// <summary>
    /// Provides ways for validation of Strings by using Regular Expressions. If you do not want to validate by Regular Expressions, do not use
    /// this class. Instead, use the TryParse methods available in the .NET Framework. ie: int.TryParse()
    /// </summary>
    public sealed class StringRegexValidator
    {
        private StringRegexValidator() { }

        /// <summary>s
        /// Function to test for Positive Integers.
        /// </summary>
        public static bool IsNaturalNumber(char cNumber)
        {
            if (string.IsNullOrEmpty(cNumber.ToString()))
                return false;

            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(cNumber.ToString()) && objNaturalPattern.IsMatch(cNumber.ToString());
        }

        /// <summary>s
        /// Function to test for Positive Integers.
        /// </summary>
        public static bool IsNaturalNumber(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) && objNaturalPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to Test for Integers both Positive & Negative
        /// </summary>
        public static bool IsInteger(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            Regex objNotIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
            return !objNotIntPattern.IsMatch(strNumber) &&
            objIntPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test for Positive Integers with zero inclusive
        /// </summary>
        public static bool IsPositiveInteger(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test whether the string is valid number or not
        /// </summary>
        public static bool IsNumber(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) && !objTwoDotPattern.IsMatch(strNumber) && !objTwoMinusPattern.IsMatch(strNumber) && objNumberPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to Test for Positive Number both Integer & Real
        /// </summary>
        public static bool IsPositiveNumber(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
                return false;

            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
            objPositivePattern.IsMatch(strNumber) && !objTwoDotPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function To test for Alphabets.
        /// </summary>
        public static bool IsAlpha(string strToCheck)
        {
            if (string.IsNullOrEmpty(strToCheck))
                return false;

            Regex objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Function To test for Alphabets.
        /// </summary>
        public static bool IsAlpha(char strToCheck)
        {
            return IsAlpha(strToCheck.ToString());
        }

        /// <summary>
        /// Function To test for Alphabets.
        /// </summary>
        public static bool IsCapitalAlpha(char charToCheck)
        {
            if (string.IsNullOrEmpty(charToCheck.ToString()))
                return false;

            Regex objAlphaPattern = new Regex("[^A-Z]");
            return !objAlphaPattern.IsMatch(charToCheck.ToString());
        }

        /// <summary>
        /// Function To test for Alphabets.
        /// </summary>
        public static bool IsCapitalAlpha(string strToCheck)
        {
            if (string.IsNullOrEmpty(strToCheck))
                return false;

            Regex objAlphaPattern = new Regex("[^A-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Function to Check for AlphaNumeric.
        /// </summary>
        public static bool IsAlphaNumeric(string strToCheck)
        {
            if (string.IsNullOrEmpty(strToCheck))
                return false;

            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Function to Check for AlphaNumeric.
        /// </summary>
        public static bool IsAlphaNumeric(char charToCheck)
        {
            if (string.IsNullOrEmpty(charToCheck.ToString()))
                return false;

            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(charToCheck.ToString());
        }

        /// <summary>
        /// Checks if a given string can be parsed to a DateTime value.
        /// </summary>
        public static bool IsDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return false;

            try
            {
                DateTime dt;
                return DateTime.TryParse(date, out dt);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a given string is a valid Email
        /// </summary>
        /// <param name="Email">Email address</param>
        /// <param name="strong">If strong is true, checks also by IP number emails</param>
        public static bool IsEmail(string Email, bool strong)
        {
            if (strong)
            {
                 string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

                if (Email != null)
                    return Regex.IsMatch(Email, MatchEmailPattern);
                else
                    return false;
            }
            else
            {
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

                Regex re = new Regex(strRegex);
                if (re.IsMatch(Email))
                    return (true);
                else
                    return (false);
            }
        }

        /// <summary>
        /// Checks if a given string is a valid Email
        /// </summary>
        /// <param name="Email">Email address</param>
        public static bool IsEmail(string Email)
        {
            return IsEmail(Email, false);
        }

        /// <summary>
        /// Checks if a given string is a valid URL
        /// </summary>
        public static bool IsUrl(string Url)
        {
            string strRegex = "^(https?://)"
            + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
            + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
            + "|" // allows either IP or domain
            + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
            + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain
            + "[a-z]{2,6})" // first level domain- .com or .museum
            + "(:[0-9]{1,4})?" // port number- :80
            + "((/?)|" // a slash isn't required if there is no file name
            + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";

            Regex re = new Regex(strRegex);

            if (re.IsMatch(Url))
                return (true);
            else
                return (false);
        }

    }
}
