using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Fabio.SharpTools.String
{
    public sealed class StringManipulator
    {
        private StringManipulator() { }

        private static List<int> charsAllowed;

        /// <summary>
        /// Sets chars that are going to be used in RemoveSpecialChars method. Set CharsAllowed to null/Nothing to reset it
        /// </summary>
        public static List<int> CharsAllowed
        {
            get
            {
                if (charsAllowed == null)
                {
                    charsAllowed = new List<int>(103);

                    charsAllowed.Add(32); //space
                    charsAllowed.Add(33); //!
                    charsAllowed.Add(34); //"
                    charsAllowed.Add(38); //&
                    charsAllowed.Add(37); //%
                    charsAllowed.Add(39); //'
                    charsAllowed.Add(40); //(
                    charsAllowed.Add(41); //)
                    charsAllowed.Add(43); //+
                    charsAllowed.Add(44); //,
                    charsAllowed.Add(45); //-
                    charsAllowed.Add(46); //.
                    charsAllowed.Add(47); // /
                    charsAllowed.Add(63); //?

                    for (int i = 48; i <= 57; i++) //0 to 9
                        charsAllowed.Add(i);

                    for (int i = 65; i <= 90; i++) //A to Z
                        charsAllowed.Add(i);

                    for (int i = 97; i <= 122; i++) //a to z
                        charsAllowed.Add(i);

                    for (int i = 224; i <= 253; i++) //accents
                    {
                        if (i != 240 && i != 247 && i != 248)
                            charsAllowed.Add(i);
                    }
                }

                return charsAllowed;
            }
            set
            {
                charsAllowed = value;
            }
        }

        /// <summary>
        /// Eliminates all chars that are not defined in property <tt>CharsAllowed</tt> (default: 0-9,A-z,a-z,accents,space,!,",&,%,',(,),+,,,-,.,/,?)
        /// </summary>
        /// <param name="text">Text to manipulate</param>
        /// <returns></returns>
        public static string RemoveSpecialChars(string text)
        {
            if (text == null)
                throw new ArgumentNullException();

            try
            {
                StringBuilder retorn = new StringBuilder(text.Length);

                foreach (char c in text)
                {
                    if (CharsAllowed.Contains((int)c))
                    {
                        retorn.Append(c);
                    }
                    else
                    {
                        switch ((int)c)
                        {
                            case 146:
                            case 145:
                            case 147:
                            case 8216:
                            case 8219:
                            case 8242:
                            case 8220:
                            case 8221:
                            case 8223:
                            case 148:
                            case 34:
                            case 8217:
                            case 8243:
                            case 8244:
                                retorn.Append('\'');
                                break;
                            case 91:
                            case 123:
                                retorn.Append('(');
                                break;
                            case 93:
                            case 125:
                                retorn.Append(')');
                                break;
                        }
                    }
                }

                return retorn.ToString();

            }
            catch (System.Exception ex)
            {
                throw new Exception("StringManipulator error: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Remove Accents from texts
        /// </summary>
        /// <returns></returns>
        public static string RemoveAccents(string text)
        {
            if (text == null)
                throw new ArgumentNullException();

            text = text.Trim();

            string normalized = text.Normalize(NormalizationForm.FormKD);
            StringBuilder builder = new StringBuilder();
            foreach (char c in normalized)
            {
                if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

    }
}
