using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Basic utilities Java/C# compatible.
    /// Use this methods when java code is transformed with Sharpen
    /// </summary>
    public static class JavaCs
    {
        public static bool EqualsIgnoreCase(string thisString, string anotherString)
        {
            return thisString.Equals(anotherString, System.StringComparison.CurrentCultureIgnoreCase);
        }
        public static string Substring(string fromString, int beginIndex)
        {
            return fromString.Substring(beginIndex);
        }
        public static string Substring(string fromString, int beginIndex, int endIndex)
        {
            return fromString.Substring(beginIndex, endIndex - beginIndex);
        }
        public static bool ContainsIgnoreCase(IList<string> values, string target)
        {
            for (int j = 0; j < values.Count; j++)
                if (EqualsIgnoreCase(values[j], target))
                    return true;
            return false;
        }

        public static string NumToString(long value)
        {
            return value.ToString();
        }
        public static int StringToInt(string value)
        {
            return int.Parse(value);
        }


        public static string[] ToArray(List<string> lst)
        {
            return lst.ToArray();
        }
        //algunas veces al traducir el argumento es IList que no tiene .ToArray, hace un cast
        public static string[] ToArray(IList<string> lst)
        {
            return ((List<string>)lst).ToArray();
        }
        public static IList<string> ToList(string[] array)
        {
            return new List<string>(array);
        }
        public static string DeepToString(string[] strArray)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
                sb.Append((i == 0 ? "" : ", ") + strArray[i]);
            return "[" + sb.ToString() + "]";
        }

        public static void PutAll(IDictionary<string, object> targetMap, IDictionary<string, object> mapToAdd)
        {
            foreach (string key in mapToAdd.Keys)
                targetMap.Add(key, mapToAdd[key]);
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        public static bool IsEmpty(IList<string> lst)
        {
            return lst == null || lst.Count == 0;   
        }

        /// <summary>
        /// Replacement using a regular expression (java replaceAll), needed because in
        /// C# replace does not uses regular expressions
        /// </summary>
        public static string ReplaceRegex(string str, string regex, string replacement)
        {
            return Regex.Replace(str, regex, replacement);
        }
        /// <summary>
        /// Split by a single char (needed to former netcore 2.0 compatibility), note
        /// that the char must not be special character in regular expressions.
        /// </summary>
        public static string[] SplitByChar(string str, char c)
        {
            return str.Split(c);
        }
        public static string[] SplitByDot(string str)
        {
            return str.Split('.');
        }
        public static string[] SplitByBar(string str)
        {
            return str.Split('|');
        }

        public static void AddAll(List<string> thisList, List<string> listToAdd)
        {
            foreach (string item in listToAdd)
                thisList.Add(item);
        }
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        public static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
        public static string GetTime(DateTime date)
        {
            return date.ToString("hh:mm:ss");
        }
        public static string GetIsoDate(DateTime date)
        {
            //if date comes from system date, it has more precision than milliseconds: truncate
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fff").Substring(0, 23);
        }
        public static DateTime ParseIsoDate(string dateString)
        {
            return DateTime.Parse(dateString);
        }
        public static long CurrentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static void Sleep(int millis)
        {
            System.Threading.Thread.Sleep(millis);
        }

        public static string GetUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
        public static string GetHash(string str)
        {
            SHA256 mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
            string hashString = String.Join(String.Empty, Array.ConvertAll(hashValue, x => x.ToString("X2")));
            return hashString.ToLower();
        }
        public static string GetHashMd5(string str)
        {
            throw new PortableException("GetHashMd5: Not implemented on netcore platform");
        }
    }
}
