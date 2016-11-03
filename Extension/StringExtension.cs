using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Fabio.SharpTools.String;
using System.Web.Script.Serialization;

namespace Fabio.SharpTools.Extension
{
    public static class Extension
    {
        public static string ToJSON(this object source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(source);
            return output;
        }

        // for generic interface IEnumerable<T>     
        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null) throw new ArgumentException("Parameter source can not be null.");
            if (string.IsNullOrEmpty(separator)) throw new ArgumentException("Parameter separator can not be null or empty.");
            string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();
            return string.Join(separator, array);
        }     // for interface IEnumerable     
        public static string ToString(this IEnumerable source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null."); if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");
            string[] array = source.Cast<object>().Where(n => n != null).Select(n => n.ToString()).ToArray();
            return string.Join(separator, array);
        }

        /// <summary>
        /// Returns text into Canonical form, used for the URL
        /// </summary>
        /// <returns></returns>
        public static string ToCanonical(this string source)
        {
            source = source.Trim();

            string normalized = source.Normalize(NormalizationForm.FormKD);
            StringBuilder builder = new StringBuilder();
            foreach (char c in normalized)
            {
                if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }
            source = builder.ToString();

            Regex re = new Regex(@"[,.;\s]+");
            source = re.Replace(source, "-");

            re = new Regex(@"['""]+");
            source = re.Replace(source, "");

            StringManipulator.CharsAllowed.Remove(33); //!
            StringManipulator.CharsAllowed.Remove(34); //"
            StringManipulator.CharsAllowed.Remove(38); //&
            StringManipulator.CharsAllowed.Remove(37); //%
            StringManipulator.CharsAllowed.Remove(39); //'
            StringManipulator.CharsAllowed.Remove(40); //(
            StringManipulator.CharsAllowed.Remove(41); //)
            StringManipulator.CharsAllowed.Remove(43); //+
            StringManipulator.CharsAllowed.Remove(44); //,
            StringManipulator.CharsAllowed.Remove(46); //.
            StringManipulator.CharsAllowed.Remove(47); // /
            StringManipulator.CharsAllowed.Remove(63); //?

            source = StringManipulator.RemoveSpecialChars(source);

            StringManipulator.CharsAllowed = null;

            source = StringManipulator.RemoveAccents(source);

            Regex reTraco = new Regex(@"-{2,}");
            source = reTraco.Replace(source, "-");

            return source;

        }

        /// <summary>
        /// Returns text into Title Case. Improved version of CultureInfo.CurrentCulture.TextInfo.ToTitleCase
        /// </summary>
        /// <returns></returns>
        public static string ToTitleCase(this string source)
        {
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            string ret = "";

            if (string.IsNullOrWhiteSpace(source))
                return ret;

            source = source.Trim();

            string[] words = source.Split(' ');
            foreach (string word in words)
            {
                if (word.Length >= 3)
                {
                    if (word[1] == '\'')
                    {
                        ret += word[0].ToString().ToUpper() + "'" +
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.Substring(2, word.Length - 2).ToLower()) + " ";
                    }
                    else if (word.Substring(0, 2).ToLower() == "mc" && char.IsLetter(word[2]))
                    {
                        ret += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.Substring(0, 2)) + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.Substring(2, word.Length - 2).ToLower()) + " ";
                    }
                    else
                    {
                        ret += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower()) + " ";
                    }
                }
                else if (word.ToLower() == "my")
                {
                    ret += "My ";
                }
                else if (word.ToLower() == "or")
                {
                    ret += "or ";
                }
                else if (word.ToLower() == "of")
                {
                    ret += "of ";
                }
                else
                {
                    Regex regexp = new Regex(@"^\d+\s?in$", RegexOptions.IgnoreCase);
                    if (regexp.IsMatch(word))
                        ret += word + " ";
                    else
                        ret += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word) + " ";
                }
            }

            ret = ret.Trim();

            return ret;
        }

        /// <summary>
        /// Pluralize this string
        /// </summary>
        public static string ToPlural(this string source)
        {
            string plural = source;

            //ignore plurals
            if (plural.EndsWith("es", true, CultureInfo.InvariantCulture) ||
                plural.EndsWith("ies", true, CultureInfo.InvariantCulture))
                return plural;

            Regex g = new Regex(@"s\b|z\b|x\b|sh\b|ch\b");
            MatchCollection matches = g.Matches(plural);
            if (matches.Count > 0)
                plural += "es";
            else
                if (plural.EndsWith("y", true, CultureInfo.InvariantCulture))
                {
                    Regex g2 = new Regex(@"(ay|ey|iy|oy|uy)\b");
                    if (g2.Matches(plural).Count <= 0) //e.g. cities
                        plural = plural.Substring(0, source.Length - 1) + "ies";
                    else
                        plural += "s";
                }
                else
                    plural += "s";

            return plural;
        }

        /// <summary>
        /// Separates words together. Those words are distinguished by uppercase and lowercase. "ThisIsTest" will be returned as "This Is Test"
        /// </summary>
        /// <param name="text">Text to separate words</param>
        /// <returns>"ThisIsTest" will be returned as "This Is Test"</returns>
        public static string SeparateWords(this string source)
        {
            if (source == null)
                throw new ArgumentNullException();

            try
            {
                source = source.Trim();

                StringBuilder retorn = new StringBuilder(source.Length);

                foreach (char c in source)
                {
                    if ((c >= 65 && c <= 90) || c == 45 || c == 95 || c == 46 || c == 43) //A to Z | - | _ | 
                        retorn.Append(" ");
                    retorn.Append(c);
                }

                source = retorn.ToString();
                source = source.Trim();
            }
            catch { }

            return source;

        }

        /// <summary>
        /// Returns the number of words. Words are broken by punctuation, a space, or by being at the start or the end of string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int CountWords(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return 0;

            MatchCollection collection = Regex.Matches(source.Trim(), @"[\S]+");
            return collection.Count;
        }

        /// <summary>
        /// Splits this string and returns an array of words separated by any whitespace char
        /// </summary>
        /// <param name="WordsOnly">Remove non-word chars from results.</param>
        /// <param name="DontIncludeThese">List of custom chars to remove from results</param>
        public static string[] ToWords(this string source, bool WordsOnly = true,
            params char[] DontIncludeThese)
        {
            source = source.Trim();

            if (source == "")
                return new string[] { };

            if (WordsOnly) //only letter, number and underscores
            {
                Regex re = new Regex(@"[\W]+");
                string[] words = re.Split(source).ToArray();
                return words;
            }
            else //split by whitespace character
            {
                string symbols = "";
                foreach (char s in DontIncludeThese) //custom list of chars to add into the split
                    symbols += s;

                Regex re = new Regex("[" + symbols + @"\s]+");
                string[] words = re.Split(source).ToArray();
                return words;
            }

        }

    }


}
