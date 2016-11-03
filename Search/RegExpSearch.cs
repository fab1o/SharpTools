using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Fabio.SharpTools.Extension;

namespace Fabio.SharpTools.Search
{
    public sealed class RegExpSearch
    {
        private Dictionary<string, int> dic;

        private Collection<string> keywords;

        //private string[] keywordsWithSpace;
        //private string[] keywordsWithNoSpace;

        private string match;

        public bool CheckForPlurals { get; set; }

        private string separators;

        public string Separators { get { return separators; } }

        public string Match
        {
            get { return match; }
        }

        public KeyValuePair<string, int> MatchKey
        {
            get {
                if (dic != null)
                {
                    return new KeyValuePair<string,int>(match,dic[match]);
                }
                else
                {
                    return new KeyValuePair<string, int>();
                }
            }
        }

        private Regex re;

        /// <summary>
        /// RegExpSearch using custom regular expression to separate words from a text 
        /// </summary>
        /// <param name="separateWordsPattern">Regular expression to separate words from a text</param>
        public RegExpSearch(string separateWordsPattern = @"[,.;\s]")
        {
            if (!string.IsNullOrWhiteSpace(separateWordsPattern))
                re = new Regex(separateWordsPattern + "+");
            else
                re = new Regex(@"[,.;\s]+");

            this.separators = separateWordsPattern;

            match = null;
            CheckForPlurals = true;
        }

        public Dictionary<string, int> Dic
        {
            get
            {
                if (dic == null)
                    dic = new Dictionary<string, int>();

                return dic;
            }
        }

        public void SetUpDictionary(IEnumerable<KeyValuePair<string[], int>> list)
        {
            foreach (var p in list)
            {
                foreach(var s in p.Key)
                {
                    if (!Dic.ContainsKey(s))
                        Dic.Add(s, p.Value);

                    KeywordsCollection.Add(s);
                }
            }
            
        }

        public Collection<string> KeywordsCollection
        {
            get {
                if (keywords == null)
                    keywords = new Collection<string>();

                return keywords;
            }
        }

        /// <summary>
        /// Keywords to search for (setting this property is slow, because
        /// it requieres rebuilding of keyword tree)
        /// </summary>
        public string[] Keywords
        {
            get { return KeywordsCollection.ToArray(); }
        }

        //private void resetAuxArrays()
        //{
        //    keywordsWithSpace = (from k in KeywordsCollection
        //                         where re.IsMatch(k)
        //                         select k).Distinct().ToArray();

        //    keywordsWithNoSpace = (from k in KeywordsCollection
        //                           where !re.IsMatch(k)
        //                           select k).Distinct().ToArray();
        //    match = null;
        //}

        private bool checkKeyword(string key, string text)
        {
            bool found = false;

            //verify if the keyword has a "space"
            bool withSpace = re.IsMatch(key);

            Regex regex = new Regex(@"('|"")");
            key = regex.Replace(key, "").Trim();

            if (withSpace)
            {
                string kPlural = (CheckForPlurals) ? key.ToPlural() : key;

                if (text.StartsWith(key, StringComparison.InvariantCultureIgnoreCase) || text.StartsWith(kPlural, StringComparison.InvariantCultureIgnoreCase))
                    found = true;
                else if (text.EndsWith(key, StringComparison.InvariantCultureIgnoreCase) || text.EndsWith(kPlural, StringComparison.InvariantCultureIgnoreCase))
                    found = true;
                else
                {
                    Regex specific = new Regex(separators + "{1}(" + key + "|" + kPlural + ")" + separators + "{1}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    if (specific.IsMatch(text))
                        found = true;
                }
            }
            else
            {
                string[] words = re.Split(text).Where(w => w.Length > 1).ToArray();

                string[] keys = new string[] { key };

                var query = words.Join<string, string, string, string>(keys, w => w, k => k, (w, k) => k, new KeywordComparer(CheckForPlurals));

                if (query.Count() > 0)
                    return true;
            }

            return found;

        }

        /// <summary>
        /// Returns true if all words match
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool ContainsAll(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            int count = 0;

            match = null;

            foreach (var key in KeywordsCollection)
            {
                bool found = checkKeyword(key, text);

                if (found)
                {
                    count++;

                    if (match == null)
                        match = key;
                }
            }

            return KeywordsCollection.Count == count;
        }

        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        public bool ContainsAny(IEnumerable<string[]> listKeywords)
        {
            Collection<string> keywords = new Collection<string>();

            foreach (var words in listKeywords)
            {
                foreach (var key in words)
                {
                    keywords.Add(key);
                }
            }

            int qtd = 0;
            return ContainsAny(keywords.Distinct().ToArray(), out qtd);
        }

        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        public bool ContainsAny(IEnumerable<string> keywords)
        {
            int qtd = 0;
            return ContainsAny(keywords, out qtd);
        }

        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        public bool ContainsAny(IEnumerable<string> keywords, out int quantity)
        {
            quantity = 0;

            if (keywords == null || keywords.Count() == 0)
                return false;


            var thisKeywords = this.keywords.Distinct().ToArray();

            var query = keywords.Join<string, string, string, string>(thisKeywords, w => w, k => k, (w, k) => k, new KeywordComparer(CheckForPlurals));

            quantity = query.Count();

            if (quantity > 0)
            {
                match = query.LastOrDefault();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        public bool ContainsAny(string text, out int quantity, bool returnQuantity = false)
        {
            quantity = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            match = null;

            Regex regex = new Regex(@"('|"")");
            text = regex.Replace(text, "").Trim();

            bool ok = false;

            foreach (var key in KeywordsCollection)
            {
                bool found = checkKeyword(key, text);

                if (found)
                {
                    ok = true;

                    quantity++;

                    if (match == null)
                        match = key;

                    if (!returnQuantity)
                        break;
                }
            }

            return ok;
        }
        /// <summary>
        /// Searches passed text and returns true if text contains any keyword
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns>True when text contains any keyword</returns>
        public bool ContainsAny(string text)
        {
            int qtd;
            return ContainsAny(text, out qtd);
        }

        private class KeywordComparer : IEqualityComparer<string>
        {
            private bool checkForPlurals;

            public KeywordComparer(bool checkForPlurals)
            {
                this.checkForPlurals = checkForPlurals;
            }

            public bool Equals(string x, string y)
            {
                if (Object.ReferenceEquals(x, y)) return true;

                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                Regex regex = new Regex(@"('|"")");
                x = regex.Replace(x, "").Trim();
                y = regex.Replace(y, "").Trim();

                if (checkForPlurals)
                    return (string.Compare(y, x, CultureInfo.InvariantCulture,
                        CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0 ||
                        string.Compare(y, x.ToPlural(), CultureInfo.InvariantCulture,
                        CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0 ||
                        string.Compare(y.ToPlural(), x, CultureInfo.InvariantCulture,
                        CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0);
                else
                    return (string.Compare(y, x, CultureInfo.InvariantCulture,
                        CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0);
            }

            public int GetHashCode(string str)
            {
                //if (Object.ReferenceEquals(str, null)) return 0;

                //int hash = str == null ? 0 : str.GetHashCode();

                //return hash;

                return 0;

            }

        }

    }
}
