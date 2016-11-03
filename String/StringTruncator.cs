using System;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Fabio.SharpTools.Convertion;

namespace Fabio.SharpTools.String
{
    /// <summary>
    /// Provides an utility tool for Management of String
    /// </summary>
    public sealed class StringTruncator
    {
        private StringTruncator() { }

        #region Simple
        /// <summary>
        /// Truncates the Input string to 100 chars length, and appends "..." if necessary
        /// </summary>
        public static string Simple(object Input)
        {
            if (Input == null)
                return "";

            return Simple(Input, 40, "...");
        }
        /// <summary>
        /// Truncates the Input string to 100 chars length, and appends "..." if necessary
        /// </summary>
        public static string Simple(string Input)
        {
            if (Input == null)
                return "";

            Input = Input.Trim();

            if (string.IsNullOrEmpty(Input))
                return Input;

            return Simple(Input, 40, "...");
        }
        /// <summary>
        /// Truncates the Input string to 100 chars length, and appends "..." if necessary
        /// </summary>
        public static string Simple(object Input, int MaxLen)
        {
            if (Input == null)
                return "";

            if (Convertor.ToNaturalNumber(MaxLen) <= 0)
                MaxLen = 40;
            
            return Simple(Input, MaxLen, "...");
        }
        /// <summary>
        /// Truncates the Input string to the maxlen length, and appends "..." if necessary
        /// </summary>
        public static string Simple(string Input, int MaxLen)
        {
            if (Input == null)
                return "";

            Input = Input.Trim();

            if (string.IsNullOrEmpty(Input))
                return Input;

            if (Convertor.ToNaturalNumber(MaxLen) <= 0)
                MaxLen = 40;

            return Simple(Input, MaxLen, "...");
        }

        /// <summary>
        /// Returns the Input with a maximum length of MaxLen.  If Input exceeds MaxLen the string
        /// is truncated and <see cref="trailer"/> is appended to it.
        /// </summary>
        public static string Simple(object Input, int MaxLen, string Trailer)
        {
            if (Input == null)
                return "";

            return Simple(Convert.ToString(Input), MaxLen, Trailer);
        }
        /// <summary>
        /// Returns the Input with a maximum length of MaxLen.  If Input exceeds MaxLen the string
        /// is truncated and <see cref="trailer"/> is appended to it.
        /// </summary>
        public static string Simple(string Input, int MaxLen, string Trailer)
        {
            if (Input == null)
                return "";

            Input = Input.Trim();

            if (string.IsNullOrEmpty(Input))
                return Input;

            if (Convertor.ToNaturalNumber(MaxLen) == 0)
                MaxLen = 40;

            if (Input == null)
                return "";
            if (Input.Length > MaxLen)
                return Input.Substring(0, MaxLen - Trailer.Length - 1) + Trailer;
            else
                return Input;
        }
        #endregion

        #region Special
        /// <summary>
        /// Truncate plain text string without cutting words, and appends "..." if necessary
        /// </summary>
        public static string Special(object Input)
        {
            if (Input == null)
                return "";

            return Special(Input, 40, "...");
        }
        /// <summary>
        /// Truncate plain text string without cutting words, and appends "..." if necessary
        /// </summary>
        public static string Special(string Input)
        {
            if (Input == null)
                return "";

            Input = Input.Trim();

            if (string.IsNullOrEmpty(Input))
                return Input;

            return Special(Input, 40, "...");
        }

        /// <summary>
        /// Truncate plain text string without cutting words, and appends "..." if necessary
        /// </summary>
        public static string Special(object Input, int MaxLen)
        {
            if (Input == null)
                return "";

            if (Convertor.ToNaturalNumber(MaxLen) <= 0)
                MaxLen = 40;

            return Special(Input, MaxLen, "...");
        }
        /// <summary>
        /// Truncate plain text string without cutting words, and appends "..." if necessary
        /// </summary>
        public static string Special(string Input, int MaxLen)
        {
            if (Input == null)
                return "";

            if (string.IsNullOrEmpty(Input))
                return Input;

            if (Convertor.ToNaturalNumber(MaxLen) == 0)
                MaxLen = 40;

            return Special(Input, MaxLen, "...");
        }

        /// <summary>
        /// Truncate plain text string without cutting words appending <see cref="trailer"/> to it
        /// </summary>
        public static string Special(object Input, int MaxLen, string Trailer)
        {
            if (Input == null)
                return "";

            return Special(Convert.ToString(Input), MaxLen, Trailer);
        }
        /// <summary>
        /// Truncate plain text string without cutting words appending <see cref="trailer"/> to it
        /// </summary>
        public static string Special(string Input, int MaxLen, string Trailer)
        {
            if (Input == null)
                return "";

            Input = Input.Trim();

            if (string.IsNullOrEmpty(Input))
                return Input;

            string response = "";

            Regex tags = new Regex("<(.|\n)+?>");
            if (tags.IsMatch(Input))
            {
                Input = tags.Replace(Input, "");
            }

            response = Input;

            if (Input.Length > MaxLen)
            {
                do
                {
                    string c = Input.Substring(MaxLen, 1);
                    if (c == " ")
                    {
                        response = Input.Substring(0, MaxLen);
                        break;
                    }
                    MaxLen--;
                } while (MaxLen > 0);

                response = response + Trailer;
            }

            response = (new Regex("&nbsp;")).Replace(response, "");

            return response;

        }
        #endregion

    }

}
