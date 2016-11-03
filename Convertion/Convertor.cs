using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using Fabio.SharpTools.String;
using System.Globalization;
using System.Collections.Specialized;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Text;
using System.Web.Script.Serialization;
using Fabio.SharpTools.Extension;

namespace Fabio.SharpTools.Convertion
{
    /// <summary>
    /// Converts Anything to Anything safely!
    /// Always returns default values.
    /// Never throws Exception or returns null value with exception of ToDateTime().
    /// Use this class to reuse code in replace of Non-Nullable types with exception of DateTime which returns Nullable DateTime.
    /// </summary>
    public static class Convertor
    {
        #region byte[]
        /// <summary>
        /// Converts string into array of bytes
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(string strInput)
        {
            int intCounter; char[] arrChar;
            arrChar = strInput.ToCharArray();
            byte[] arrByte = new byte[arrChar.Length];
            for (intCounter = 0; intCounter <= arrByte.Length - 1; intCounter++)
                arrByte[intCounter] = Convert.ToByte(arrChar[intCounter]);

            return arrByte;
        }
        #endregion

        #region boolean
        /// <summary>
        /// Converts to Boolean.
        /// </summary>
        public static bool ToBoolean(string theValue)
        {
            if (theValue == null)
                return false;

            theValue = theValue.Trim();
            try
            {
                if (StringRegexValidator.IsInteger(theValue))
                    return Convert.ToBoolean(Convertor.ToInteger(theValue));
                else
                    return Convert.ToBoolean(theValue.ToLower());
            }
            catch { }

            return false;

        }
        /// <summary>
        /// Converts to Boolean.
        /// </summary>
        public static bool ToBoolean(object theValue)
        {
            if (theValue == null)
                return false;

            if (theValue.GetType() == typeof(string))
                return ToBoolean(Convert.ToString(theValue));

            try
            {
                if (theValue is IConvertible)
                    return Convert.ToBoolean(theValue);
            }
            catch { }

            return false;
        }
        #endregion

        #region string
        public static string ToJSON(string text)
        {
            StringBuilder json = new StringBuilder(text.Length * 6);
            Byte[] bytes = Encoding.Unicode.GetBytes(text);

            foreach (var b in bytes)
            {
                if (b == 0)
                    continue;

                if (b == 96)
                {
                    json.Append('\'');
                    continue;
                }

                if ((b > 64 && b < 126) || (b > 47 && b < 58))
                {
                    json.Append((char)b);
                    continue;
                }

                switch (b)
                {
                    case 59:
                    case 58:
                    case 32:
                    case 33:
                    case 36:
                    case 39:
                    case 34:
                    case 44:
                    case 46:
                        json.Append((char)b);
                        break;
                    default:
                        json.AppendFormat(@"\u{0:X4}", b);
                        break;
                }
            }

            return json.ToString();
        }

        public static string ToStringFromJSON(string json)
        {
            Regex regex = new Regex(@"\\[uU]([0-9A-F]{4})", RegexOptions.IgnoreCase);
            return regex.Replace(json, match => ((char)int.Parse(match.Groups[1].Value,
              NumberStyles.HexNumber)).ToString());

        }

        /// <summary>
        /// Converts to String. Returns empty string if cannot convert
        /// </summary>
        public static string ToString(bool theValue)
        {
            return Convert.ToString(theValue).ToLower();
        }
        /// <summary>
        /// Converts to String. Returns empty string if cannot convert
        /// </summary>
        public static string ToString(string theValue)
        {
            if (theValue != null)
            {
                return theValue;
            }
            else
                return "";
        }

        /// <summary>
        /// Converts any enum to string
        /// </summary>
        public static string ToString(Enum EnumValue)
        {
            FieldInfo fi = EnumValue.GetType().GetField(EnumValue.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return EnumValue.ToString();
            }
        }
        #endregion

        #region char
        /// <summary>
        /// Converts to Char. Returns ' ' if cannot convert
        /// </summary>
        public static char ToChar(object theValue)
        {
            if (theValue != null)
            {
                try
                {
                    return Convert.ToChar(theValue);
                }
                catch { return ' '; }
            }
            else
                return ' ';
        }

        /// <summary>
        /// Converts to Char. Returns ' ' if cannot convert
        /// </summary>
        public static char ToChar(string theValue)
        {
            if (theValue != null)
            {
                try
                {
                    return Convert.ToChar(theValue);
                }
                catch { return ' '; }
            }
            else
                return ' ';
        }
        #endregion

        #region long
        /// <summary>
        /// Converts to Long. Returns 0 if cannot convert
        /// </summary>
        public static long ToLong(string theValue)
        {
            long value = 0;
            long.TryParse(theValue, out value);
            return value;
        }

        /// <summary>
        /// Converts to Long. Returns 0 if cannot convert
        /// </summary>
        public static long ToLong(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                    return Convert.ToInt64(theValue);
            }
            catch { }
            return 0;
        }
        #endregion

        #region double
        /// <summary>
        /// Converts to Double. Returns 0.0 if cannot convert
        /// </summary>
        public static double ToDouble(string theValue)
        {
            double value = 0;
            double.TryParse(theValue, out value);
            return value;
        }

        /// <summary>
        /// Converts to Double. Returns 0.0 if cannot convert
        /// </summary>
        public static double ToDouble(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                    return Convert.ToDouble(theValue);
            }
            catch { }
            return 0;
        }
        #endregion

        #region datetime
        /// <summary>
        /// Converts to DateTime. Returns null if cannot convert
        /// </summary>
        public static DateTime? ToDateTime(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                    return Convert.ToDateTime(theValue);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Converts to DateTime. Returns null if cannot convert
        /// </summary>
        public static DateTime? ToDateTime(object theValue, IFormatProvider provider)
        {
            try
            {
                if (theValue is IConvertible)
                    return Convert.ToDateTime(theValue, provider);
            }
            catch { }
            return null;
        }


        /// <summary>
        /// Converts to DateTime. Returns null if cannot convert
        /// </summary>
        public static DateTime? ToDateTime(string theValue)
        {
            try
            {
                theValue = theValue.Trim();
                return Convert.ToDateTime(theValue);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Converts to DateTime. Returns null if cannot convert
        /// </summary>
        public static DateTime? ToDateTime(string theValue, IFormatProvider provider)
        {
            try
            {
                theValue = theValue.Trim();
                return Convert.ToDateTime(theValue, provider);
            }
            catch { }
            return null;
        }
        #endregion

        #region int
        /// <summary>
        /// Converts to Integer. Returns 0 if cannot convert
        /// </summary>
        public static int[] ToInteger(string[] array)
        {
            try
            {
                if (array == null || array.Length == 0)
                    return new int[] { };

                int[] values = new int[array.Length];

                //Array.Copy(array, values, array.Length); doesnt work for strings

                for (int i = 0; i < array.Length; i++)
                    values[i] = Convertor.ToInteger(array[i]);

                return values;
            }
            catch
            {
                return new int[] { };
            }
        }
        /// <summary>
        /// Converts to Integer. Returns 0 if cannot convert
        /// </summary>
        public static int ToInteger(string theValue)
        {
            if (theValue == null)
                return 0;

            theValue = theValue.Trim();

            if (theValue.Contains(".") || theValue.Contains(","))
            {
                double Value = ToDouble(theValue);
                return (int)(Value >= 0 ? Value + 0.5 : Value - 0.5);
            }
            else
            {
                int value = 0;
                int.TryParse(theValue, out value);
                return value;
            }
        }
        /// <summary>
        /// Converts to Integer. Returns 0 if cannot convert
        /// </summary>
        public static short ToShort(string theValue)
        {
            if (theValue == null)
                return 0;

            theValue = theValue.Trim();

            if (theValue.Contains(".") || theValue.Contains(","))
            {
                double Value = ToDouble(theValue);
                return (short)(Value >= 0 ? Value + 0.5 : Value - 0.5);
            }
            else
            {
                short value = 0;
                short.TryParse(theValue, out value);
                return value;
            }
        }

        /// <summary>
        /// Converts to Integer. Returns 0 if cannot convert
        /// </summary>
        public static int ToInteger(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                    return Convert.ToInt32(theValue);
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Converts to Integer. Returns 0 if cannot convert
        /// </summary>
        public static int ToInteger(bool theValue)
        {
            if (theValue)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Converts to Natural Number. Returns 1 if cannot convert
        /// </summary>
        public static int ToNaturalNumber(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                {
                    int x = Convert.ToInt32(theValue);
                    if (x <= 0)
                        x = 1;
                    return x;
                }

            }
            catch { }
            return 1;
        }

        /// <summary>
        /// Converts to Natural Number. Returns 1 if cannot convert
        /// </summary>
        public static int ToNaturalNumber(string theValue)
        {
            int x = ToInteger(theValue);
            if (x <= 0)
                x = 1;
            return x;
        }

        /// <summary>
        /// Converts to Positive Integer. Returns 0 if cannot convert
        /// </summary>
        public static int ToPositiveInteger(object theValue)
        {
            try
            {
                if (theValue is IConvertible)
                {
                    int x = Convert.ToInt32(theValue);
                    if (x < 0)
                        x = 0;
                    return x;
                }

            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Converts to Positive Integer. Returns 0 if cannot convert
        /// </summary>
        public static int ToPositiveInteger(string theValue)
        {
            int x = ToInteger(theValue);
            if (x <= 0)
                x = 0;
            return x;
        }
        #endregion

        #region enum
        /// <summary>
        /// Converts any string to the specified enum type, returns ArgumentException if not found.
        /// </summary>
        public static object ToEnum(string StringValue, Type EnumType)
        {
            if (EnumType == null)
                throw new Exception("StringToEnum(): EnumType is null");

            if (StringValue == null)
                throw new Exception("StringToEnum(): StringValue is null");

            string[] names = Enum.GetNames(EnumType);
            foreach (string name in names)
            {
                if (ToString((Enum)Enum.Parse(EnumType, name)).Equals(StringValue))
                {
                    return Enum.Parse(EnumType, name);
                }
            }

            throw new ArgumentException("The StringValue is not a description or value of the specified EnumType.");
        }
        #endregion

        #region decimal
        public static decimal ToDecimal(object value)
        {
            if (value == null)
                return 0;

            decimal theValue = 0;
            try
            {
                if (value is IConvertible)
                {
                    theValue = Convert.ToDecimal(value);
                }
            }
            catch { }
            {
                theValue = 0;
            }

            return theValue;
        }

        public static decimal ToDecimal(string theValue)
        {
            if (theValue == null)
                return 0;

            theValue = theValue.Trim();

            theValue = theValue.Replace(",", ".");

            decimal value = 0;

            decimal.TryParse(theValue, out value);

            return value;

        }
        #endregion

        #region Guid
        public static Guid ToGuid(string guid)
        {
            Guid g = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(guid))
            {
                try
                {
                    g = new Guid(guid);
                }
                catch { }
            }

            return g;
        }
        #endregion

        #region DataSet_DataTable
        public static string GetJSONString(DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(table.ToDictionary());

        }

        public static string GetJSONString(DataSet data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(data.ToDictionary());

        }
        #endregion

    }

}

