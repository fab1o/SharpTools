using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fabio.SharpTools.String
{
    public sealed class StringRandomizer
    {
        private StringRandomizer() { }

        #region Generate Random String
        /// <summary>
        /// Returns a random string with lower caps the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <returns></returns>
        public static string Generate(int size)
        {
            StringBuilder randStr = new StringBuilder(size);

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < size; i++)
            {
                //int c = (((int)rnd.NextDouble() % 2) == 0) ? 97 : 65;
                int c = 97;
                randStr.Append((char)(26 * rnd.NextDouble() + c));
            }

            return randStr.ToString();
        }
        /// <summary>
        /// Returns a random string the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="upperCase">If true all UpperCase. If false, all LowerCase</param>
        /// <returns></returns>
        public static string Generate(int size, bool upperCase)
        {
            StringBuilder randStr = new StringBuilder(size);

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < size; i++)
            {
                int c = (upperCase) ? 65: 97;
                randStr.Append((char)(26 * rnd.NextDouble() + c));
            }

            return randStr.ToString();
        }
        #endregion

    }
}
