using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AVOSCloud.Internal
{
    internal class Json
    {
        private static readonly string startOfString = "\\G";
        private static readonly string whitespace = "[ \t\r\n]*";
        private static readonly Regex beginArray = new Regex(Json.startOfString + Json.whitespace + "\\[" + Json.whitespace);
        private static readonly Regex beginObject = new Regex(Json.startOfString + Json.whitespace + "\\{" + Json.whitespace);
        private static readonly Regex endArray = new Regex(Json.startOfString + Json.whitespace + "\\]" + Json.whitespace);
        private static readonly Regex endObject = new Regex(Json.startOfString + Json.whitespace + "\\}" + Json.whitespace);
        private static readonly Regex nameSeparator = new Regex(Json.startOfString + Json.whitespace + ":" + Json.whitespace);
        private static readonly Regex valueSeparator = new Regex(Json.startOfString + Json.whitespace + "," + Json.whitespace);
        private static readonly Regex falseValue = new Regex(Json.startOfString + "false");
        private static readonly Regex trueValue = new Regex(Json.startOfString + "true");
        private static readonly Regex nullValue = new Regex(Json.startOfString + "null");
        private static readonly Regex numberValue = new Regex(Json.startOfString + "-?(?:0|[1-9]\\d*)(?<frac>\\.\\d+)?(?<exp>(?:e|E)(?:-|\\+)?\\d+)?");
        private static readonly Regex stringValue = new Regex(Json.startOfString + "\"(?<content>(?:[^\\\\\"]|(?<escape>\\\\(?:[\\\\\"/bfnrt]|u[0-9a-fA-F]{4})))*)\"", RegexOptions.Multiline);
        private static readonly Regex escapePattern = new Regex("\\\\|\"|[\0-\x001F]");

        private static bool Accept(string input, int start, Regex matcher, out int consumed, out Match match)
        {
            match = matcher.Match(input, start);
            consumed = match.Length;
            return match.Success;
        }

        public static string Encode(IDictionary<string, object> dict)
        {
            if (dict == null)
                throw new ArgumentNullException();
            if (dict.Count == 0)
                return "{}";
            StringBuilder stringBuilder = new StringBuilder("{");
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)dict)
            {
                stringBuilder.Append(Json.Encode((object)keyValuePair.Key));
                stringBuilder.Append(":");
                stringBuilder.Append(Json.Encode(keyValuePair.Value));
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = '}';
            return ((object)stringBuilder).ToString();
        }

        public static string Encode(IList<object> list)
        {
            if (list == null)
                throw new ArgumentNullException();
            if (list.Count == 0)
                return "[]";
            StringBuilder stringBuilder = new StringBuilder("[");
            foreach (object obj in (IEnumerable<object>)list)
            {
                stringBuilder.Append(Json.Encode(obj));
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = ']';
            return ((object)stringBuilder).ToString();
        }

        public static string Encode(IList<string> strList)
        {
            if (strList == null)
                throw new ArgumentNullException();
            if (strList.Count == 0)
                return "[]";
            StringBuilder stringBuilder = new StringBuilder("[");
            foreach (object obj in (IEnumerable<string>)strList)
            {
                stringBuilder.Append(Json.Encode(obj));
                stringBuilder.Append(",");
            }
            stringBuilder[stringBuilder.Length - 1] = ']';
            return ((object)stringBuilder).ToString();
        }

        public static string Encode(object obj)
        {
            IDictionary<string, object> dict = obj as IDictionary<string, object>;
            if (dict != null)
                return Json.Encode(dict);
            IList<object> list = obj as IList<object>;
            if (list != null)
                return Json.Encode(list);
            IList<string> strList = obj as IList<string>;
            if (strList != null)
                return Json.Encode(strList);
            string input = obj as string;
            if (input != null)
                return "\"" + Json.escapePattern.Replace(input, (MatchEvaluator)(m =>
                {
                    switch (m.Value[0])
                    {
                        case '\b':
                            return "\\b";
                        case '\t':
                            return "\\t";
                        case '\n':
                            return "\\n";
                        case '\v':
                            return "\\u" + ((ushort)m.Value[0]).ToString("x4");
                        case '\f':
                            return "\\f";
                        case '\r':
                            return "\\r";
                        case '"':
                            return "\\\"";
                        case '\\':
                            return "\\\\";
                        default:
                            return "\\u" + ((ushort)m.Value[0]).ToString("x4");
                    }
                })) + "\"";
            if (obj == null)
                return "null";
            if (obj is bool)
                return (bool)obj ? "true" : "false";
            else if (!ReflectionHelpers.IsPrimitive(obj.GetType()))
                throw new ArgumentException("Unable to encode objects of type " + (object)obj.GetType());
            else
                return Convert.ToString(obj, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public static object Parse(string input)
        {
            int consumed;
            object output;
            if (!Json.AVObject(input, 0, out consumed, out output) && !Json.ParseArray(input, 0, out consumed, out output) || consumed != input.Length)
                throw new ArgumentException("Input JSON was invalid.");
            else
                return output;
        }

        private static bool ParseArray(string input, int start, out int consumed, out object output)
        {
            output = (object)null;
            consumed = 0;
            int consumed1;
            Match match;
            if (!Json.Accept(input, start, Json.beginArray, out consumed1, out match))
                return false;
            consumed = consumed + consumed1;
            List<object> list = new List<object>();
            object output1;
            while (Json.ParseValue(input, start + consumed, out consumed1, out output1))
            {
                list.Add(output1);
                consumed = consumed + consumed1;
                if (Json.Accept(input, start + consumed, Json.valueSeparator, out consumed1, out match))
                    consumed = consumed + consumed1;
                else
                    break;
            }
            if (!Json.Accept(input, start + consumed, Json.endArray, out consumed1, out match))
                return false;
            consumed = consumed + consumed1;
            output = (object)list;
            return true;
        }

        private static bool ParseMember(string input, int start, out int consumed, out object output)
        {
            output = (object)null;
            consumed = 0;
            int consumed1;
            object output1;
            if (!Json.ParseString(input, start, out consumed1, out output1))
                return false;
            consumed = consumed + consumed1;
            Match match;
            if (!Json.Accept(input, start + consumed, Json.nameSeparator, out consumed1, out match))
                return false;
            consumed = consumed + consumed1;
            object output2;
            if (!Json.ParseValue(input, start + consumed, out consumed1, out output2))
                return false;
            consumed = consumed + consumed1;
            output = (object)new Tuple<string, object>((string)output1, output2);
            return true;
        }

        private static bool ParseNumber(string input, int start, out int consumed, out object output)
        {
            output = (object)null;
            Match match;
            if (!Json.Accept(input, start, Json.numberValue, out consumed, out match))
                return false;
            if (match.Groups["frac"].Length <= 0 && match.Groups["exp"].Length <= 0)
            {
                output = (object)long.Parse(match.Value, (IFormatProvider)CultureInfo.InvariantCulture);
                return true;
            }
            else
            {
                output = (object)double.Parse(match.Value, (IFormatProvider)CultureInfo.InvariantCulture);
                return true;
            }
        }

        private static bool AVObject(string input, int start, out int consumed, out object output)
        {
            output = (object)null;
            consumed = 0;
            int consumed1;
            Match match;
            if (!Json.Accept(input, start, Json.beginObject, out consumed1, out match))
                return false;
            consumed = consumed + consumed1;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            object output1;
            while (Json.ParseMember(input, start + consumed, out consumed1, out output1))
            {
                Tuple<string, object> tuple = output1 as Tuple<string, object>;
                dictionary[tuple.Item1] = tuple.Item2;
                consumed = consumed + consumed1;
                if (Json.Accept(input, start + consumed, Json.valueSeparator, out consumed1, out match))
                    consumed = consumed + consumed1;
                else
                    break;
            }
            if (!Json.Accept(input, start + consumed, Json.endObject, out consumed1, out match))
                return false;
            consumed = consumed + consumed1;
            output = (object)dictionary;
            return true;
        }

        private static bool ParseString(string input, int start, out int consumed, out object output)
        {
            output = (object)null;
            Match match;
            if (!Json.Accept(input, start, Json.stringValue, out consumed, out match))
                return false;
            int num = 0;
            Group group = match.Groups["content"];
            StringBuilder stringBuilder = new StringBuilder(group.Value);
            IEnumerator enumerator = match.Groups["escape"].Captures.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Capture capture = (Capture)enumerator.Current;
                    int index = capture.Index - group.Index - num;
                    num += capture.Length - 1;
                    stringBuilder.Remove(index + 1, capture.Length - 1);
                    char ch = capture.Value[1];
                    if ((int)ch <= 92)
                    {
                        if ((int)ch == 34)
                        {
                            stringBuilder[index] = '"';
                            continue;
                        }
                        else if ((int)ch == 47)
                        {
                            stringBuilder[index] = '/';
                            continue;
                        }
                        else if ((int)ch == 92)
                        {
                            stringBuilder[index] = '\\';
                            continue;
                        }
                    }
                    else if ((int)ch > 102)
                    {
                        switch (ch)
                        {
                            case 'n':
                                stringBuilder[index] = '\n';
                                continue;
                            case 'r':
                                stringBuilder[index] = '\r';
                                continue;
                            case 't':
                                stringBuilder[index] = '\t';
                                continue;
                            case 'u':
                                stringBuilder[index] = (char)ushort.Parse(capture.Value.Substring(2), NumberStyles.AllowHexSpecifier);
                                continue;
                            default:
                                continue;
                        }
                    }
                    else if ((int)ch == 98)
                    {
                        stringBuilder[index] = '\b';
                        continue;
                    }
                    else if ((int)ch == 102)
                    {
                        stringBuilder[index] = '\f';
                        continue;
                    }
                    throw new ArgumentException("Unexpected escape character in string: " + capture.Value);
                }
                output = (object)((object)stringBuilder).ToString();
                return true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                output = (object)((object)stringBuilder).ToString();
            }
        }

        private static bool ParseValue(string input, int start, out int consumed, out object output)
        {
            Match match;
            if (Json.Accept(input, start, Json.falseValue, out consumed, out match))
            {
                output = (object)false;
                return true;
            }
            else if (Json.Accept(input, start, Json.nullValue, out consumed, out match))
            {
                output = (object)null;
                return true;
            }
            else if (Json.Accept(input, start, Json.trueValue, out consumed, out match))
            {
                output = (object)true;
                return true;
            }
            else if (Json.AVObject(input, start, out consumed, out output) || Json.ParseArray(input, start, out consumed, out output) || Json.ParseNumber(input, start, out consumed, out output))
                return true;
            else
                return Json.ParseString(input, start, out consumed, out output);
        }
    }
}