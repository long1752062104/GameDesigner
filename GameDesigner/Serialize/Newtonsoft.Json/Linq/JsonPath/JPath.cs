using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class JPath
    {
        public List<PathFilter> Filters { get; private set; }

        public JPath(string expression)
        {
            ValidationUtils.ArgumentNotNull(expression, "expression");
            _expression = expression;
            Filters = new List<PathFilter>();
            ParseMain();
        }

        private void ParseMain()
        {
            int currentIndex = _currentIndex;
            EatWhitespace();
            if (_expression.Length == _currentIndex)
            {
                return;
            }
            if (_expression[_currentIndex] == '$')
            {
                if (_expression.Length == 1)
                {
                    return;
                }
                char c = _expression[_currentIndex + 1];
                if (c == '.' || c == '[')
                {
                    _currentIndex++;
                    currentIndex = _currentIndex;
                }
            }
            if (!ParsePath(Filters, currentIndex, false))
            {
                int currentIndex2 = _currentIndex;
                EatWhitespace();
                if (_currentIndex < _expression.Length)
                {
                    throw new JsonException("Unexpected character while parsing path: " + _expression[currentIndex2].ToString());
                }
            }
        }

        private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            while (_currentIndex < _expression.Length && !flag4)
            {
                char c = _expression[_currentIndex];
                if (c <= ')')
                {
                    if (c != ' ')
                    {
                        if (c != '(')
                        {
                            if (c != ')')
                            {
                                goto IL_1BF;
                            }
                            goto IL_E8;
                        }
                    }
                    else
                    {
                        if (_currentIndex < _expression.Length)
                        {
                            flag4 = true;
                            continue;
                        }
                        continue;
                    }
                }
                else
                {
                    if (c == '.')
                    {
                        if (_currentIndex > currentPartStartIndex)
                        {
                            string text = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex);
                            if (text == "*")
                            {
                                text = null;
                            }
                            PathFilter fieldFilter;
                            if (!flag)
                            {
                                fieldFilter = new FieldFilter() { Name = text };
                            }
                            else
                            {
                                fieldFilter = new ScanFilter() { Name = text };
                            }
                            PathFilter item = fieldFilter;
                            filters.Add(item);
                            flag = false;
                        }
                        if (_currentIndex + 1 < _expression.Length && _expression[_currentIndex + 1] == '.')
                        {
                            flag = true;
                            _currentIndex++;
                        }
                        _currentIndex++;
                        currentPartStartIndex = _currentIndex;
                        flag2 = false;
                        flag3 = true;
                        continue;
                    }
                    if (c != '[')
                    {
                        if (c != ']')
                        {
                            goto IL_1BF;
                        }
                        goto IL_E8;
                    }
                }
                if (_currentIndex > currentPartStartIndex)
                {
                    string text2 = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex);
                    if (text2 == "*")
                    {
                        text2 = null;
                    }
                    PathFilter fieldFilter2;
                    if (!flag)
                    {
                        fieldFilter2 = new FieldFilter() { Name = text2 };
                    }
                    else
                    {
                        fieldFilter2 = new ScanFilter() { Name = text2 };
                    }
                    PathFilter item2 = fieldFilter2;
                    filters.Add(item2);
                    flag = false;
                }
                filters.Add(ParseIndexer(c));
                _currentIndex++;
                currentPartStartIndex = _currentIndex;
                flag2 = true;
                flag3 = false;
                continue;
            IL_E8:
                flag4 = true;
                continue;
            IL_1BF:
                if (query && (c == '=' || c == '<' || c == '!' || c == '>' || c == '|' || c == '&'))
                {
                    flag4 = true;
                }
                else
                {
                    if (flag2)
                    {
                        throw new JsonException("Unexpected character following indexer: " + c.ToString());
                    }
                    _currentIndex++;
                }
            }
            bool flag5 = _currentIndex == _expression.Length;
            if (_currentIndex > currentPartStartIndex)
            {
                string text3 = _expression.Substring(currentPartStartIndex, _currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
                if (text3 == "*")
                {
                    text3 = null;
                }
                PathFilter fieldFilter3;
                if (!flag)
                {
                    fieldFilter3 = new FieldFilter() { Name = text3 };
                }
                else
                {
                    fieldFilter3 = new ScanFilter() { Name = text3 };
                }
                PathFilter item3 = fieldFilter3;
                filters.Add(item3);
            }
            else if (flag3 && (flag5 || query))
            {
                throw new JsonException("Unexpected end while parsing path.");
            }
            return flag5;
        }

        private PathFilter ParseIndexer(char indexerOpenChar)
        {
            _currentIndex++;
            char indexerCloseChar = (indexerOpenChar == '[') ? ']' : ')';
            EnsureLength("Path ended with open indexer.");
            EatWhitespace();
            if (_expression[_currentIndex] == '\'')
            {
                return ParseQuotedField(indexerCloseChar);
            }
            if (_expression[_currentIndex] == '?')
            {
                return ParseQuery(indexerCloseChar);
            }
            return ParseArrayIndexer(indexerCloseChar);
        }

        private PathFilter ParseArrayIndexer(char indexerCloseChar)
        {
            int currentIndex = _currentIndex;
            int? num = null;
            List<int> list = null;
            int num2 = 0;
            int? start = null;
            int? end = null;
            int? step = null;
            while (_currentIndex < _expression.Length)
            {
                char c = _expression[_currentIndex];
                if (c == ' ')
                {
                    num = new int?(_currentIndex);
                    EatWhitespace();
                }
                else if (c == indexerCloseChar)
                {
                    int num3 = (num ?? _currentIndex) - currentIndex;
                    if (list != null)
                    {
                        if (num3 == 0)
                        {
                            throw new JsonException("Array index expected.");
                        }
                        int item = Convert.ToInt32(_expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
                        list.Add(item);
                        return new ArrayMultipleIndexFilter
                        {
                            Indexes = list
                        };
                    }
                    else
                    {
                        if (num2 > 0)
                        {
                            if (num3 > 0)
                            {
                                int value = Convert.ToInt32(_expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
                                if (num2 == 1)
                                {
                                    end = new int?(value);
                                }
                                else
                                {
                                    step = new int?(value);
                                }
                            }
                            return new ArraySliceFilter
                            {
                                Start = start,
                                End = end,
                                Step = step
                            };
                        }
                        if (num3 == 0)
                        {
                            throw new JsonException("Array index expected.");
                        }
                        int value2 = Convert.ToInt32(_expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
                        return new ArrayIndexFilter
                        {
                            Index = new int?(value2)
                        };
                    }
                }
                else if (c == ',')
                {
                    int num4 = (num ?? _currentIndex) - currentIndex;
                    if (num4 == 0)
                    {
                        throw new JsonException("Array index expected.");
                    }
                    if (list == null)
                    {
                        list = new List<int>();
                    }
                    string value3 = _expression.Substring(currentIndex, num4);
                    list.Add(Convert.ToInt32(value3, CultureInfo.InvariantCulture));
                    _currentIndex++;
                    EatWhitespace();
                    currentIndex = _currentIndex;
                    num = null;
                }
                else if (c == '*')
                {
                    _currentIndex++;
                    EnsureLength("Path ended with open indexer.");
                    EatWhitespace();
                    if (_expression[_currentIndex] != indexerCloseChar)
                    {
                        throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                    }
                    return new ArrayIndexFilter();
                }
                else if (c == ':')
                {
                    int num5 = (num ?? _currentIndex) - currentIndex;
                    if (num5 > 0)
                    {
                        int value4 = Convert.ToInt32(_expression.Substring(currentIndex, num5), CultureInfo.InvariantCulture);
                        if (num2 == 0)
                        {
                            start = new int?(value4);
                        }
                        else if (num2 == 1)
                        {
                            end = new int?(value4);
                        }
                        else
                        {
                            step = new int?(value4);
                        }
                    }
                    num2++;
                    _currentIndex++;
                    EatWhitespace();
                    currentIndex = _currentIndex;
                    num = null;
                }
                else
                {
                    if (!char.IsDigit(c) && c != '-')
                    {
                        throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                    }
                    if (num != null)
                    {
                        throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
                    }
                    _currentIndex++;
                }
            }
            throw new JsonException("Path ended with open indexer.");
        }

        private void EatWhitespace()
        {
            while (_currentIndex < _expression.Length && _expression[_currentIndex] == ' ')
            {
                _currentIndex++;
            }
        }

        private PathFilter ParseQuery(char indexerCloseChar)
        {
            _currentIndex++;
            EnsureLength("Path ended with open indexer.");
            if (_expression[_currentIndex] != '(')
            {
                throw new JsonException("Unexpected character while parsing path indexer: " + _expression[_currentIndex].ToString());
            }
            _currentIndex++;
            QueryExpression expression = ParseExpression();
            _currentIndex++;
            EnsureLength("Path ended with open indexer.");
            EatWhitespace();
            if (_expression[_currentIndex] != indexerCloseChar)
            {
                throw new JsonException("Unexpected character while parsing path indexer: " + _expression[_currentIndex].ToString());
            }
            return new QueryFilter
            {
                Expression = expression
            };
        }

        private QueryExpression ParseExpression()
        {
            QueryExpression queryExpression = null;
            CompositeExpression compositeExpression = null;
            while (_currentIndex < _expression.Length)
            {
                EatWhitespace();
                if (_expression[_currentIndex] != '@')
                {
                    throw new JsonException("Unexpected character while parsing path query: " + _expression[_currentIndex].ToString());
                }
                _currentIndex++;
                List<PathFilter> list = new List<PathFilter>();
                if (ParsePath(list, _currentIndex, true))
                {
                    throw new JsonException("Path ended with open query.");
                }
                EatWhitespace();
                EnsureLength("Path ended with open query.");
                object value = null;
                QueryOperator queryOperator;
                if (_expression[_currentIndex] == ')' || _expression[_currentIndex] == '|' || _expression[_currentIndex] == '&')
                {
                    queryOperator = QueryOperator.Exists;
                }
                else
                {
                    queryOperator = ParseOperator();
                    EatWhitespace();
                    EnsureLength("Path ended with open query.");
                    value = ParseValue();
                    EatWhitespace();
                    EnsureLength("Path ended with open query.");
                }
                BooleanQueryExpression booleanQueryExpression = new BooleanQueryExpression
                {
                    Path = list,
                    Operator = queryOperator,
                    Value = ((queryOperator != QueryOperator.Exists) ? new JValue(value) : null)
                };
                if (_expression[_currentIndex] == ')')
                {
                    if (compositeExpression != null)
                    {
                        compositeExpression.Expressions.Add(booleanQueryExpression);
                        return queryExpression;
                    }
                    return booleanQueryExpression;
                }
                else
                {
                    if (_expression[_currentIndex] == '&' && Match("&&"))
                    {
                        if (compositeExpression == null || compositeExpression.Operator != QueryOperator.And)
                        {
                            CompositeExpression compositeExpression2 = new CompositeExpression
                            {
                                Operator = QueryOperator.And
                            };
                            if (compositeExpression != null)
                            {
                                compositeExpression.Expressions.Add(compositeExpression2);
                            }
                            compositeExpression = compositeExpression2;
                            if (queryExpression == null)
                            {
                                queryExpression = compositeExpression;
                            }
                        }
                        compositeExpression.Expressions.Add(booleanQueryExpression);
                    }
                    if (_expression[_currentIndex] == '|' && Match("||"))
                    {
                        if (compositeExpression == null || compositeExpression.Operator != QueryOperator.Or)
                        {
                            CompositeExpression compositeExpression3 = new CompositeExpression
                            {
                                Operator = QueryOperator.Or
                            };
                            if (compositeExpression != null)
                            {
                                compositeExpression.Expressions.Add(compositeExpression3);
                            }
                            compositeExpression = compositeExpression3;
                            if (queryExpression == null)
                            {
                                queryExpression = compositeExpression;
                            }
                        }
                        compositeExpression.Expressions.Add(booleanQueryExpression);
                    }
                }
            }
            throw new JsonException("Path ended with open query.");
        }

        private object ParseValue()
        {
            char c = _expression[_currentIndex];
            if (c == '\'')
            {
                return ReadQuotedString();
            }
            if (char.IsDigit(c) || c == '-')
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(c);
                _currentIndex++;
                while (_currentIndex < _expression.Length)
                {
                    c = _expression[_currentIndex];
                    if (c == ' ' || c == ')')
                    {
                        string text = stringBuilder.ToString();
                        if (text.IndexOfAny(new char[]
                        {
                            '.',
                            'E',
                            'e'
                        }) != -1)
                        {
                            if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out double num))
                            {
                                return num;
                            }
                            throw new JsonException("Could not read query value.");
                        }
                        else
                        {
                            if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long num2))
                            {
                                return num2;
                            }
                            throw new JsonException("Could not read query value.");
                        }
                    }
                    else
                    {
                        stringBuilder.Append(c);
                        _currentIndex++;
                    }
                }
            }
            else if (c == 't')
            {
                if (Match("true"))
                {
                    return true;
                }
            }
            else if (c == 'f')
            {
                if (Match("false"))
                {
                    return false;
                }
            }
            else if (c == 'n' && Match("null"))
            {
                return null;
            }
            throw new JsonException("Could not read query value.");
        }

        private string ReadQuotedString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            _currentIndex++;
            while (_currentIndex < _expression.Length)
            {
                char c = _expression[_currentIndex];
                if (c == '\\' && _currentIndex + 1 < _expression.Length)
                {
                    _currentIndex++;
                    if (_expression[_currentIndex] == '\'')
                    {
                        stringBuilder.Append('\'');
                    }
                    else
                    {
                        if (_expression[_currentIndex] != '\\')
                        {
                            throw new JsonException("Unknown escape chracter: \\" + _expression[_currentIndex].ToString());
                        }
                        stringBuilder.Append('\\');
                    }
                    _currentIndex++;
                }
                else
                {
                    if (c == '\'')
                    {
                        _currentIndex++;
                        return stringBuilder.ToString();
                    }
                    _currentIndex++;
                    stringBuilder.Append(c);
                }
            }
            throw new JsonException("Path ended with an open string.");
        }

        private bool Match(string s)
        {
            int num = _currentIndex;
            foreach (char c in s)
            {
                if (num >= _expression.Length || _expression[num] != c)
                {
                    return false;
                }
                num++;
            }
            _currentIndex = num;
            return true;
        }

        private QueryOperator ParseOperator()
        {
            if (_currentIndex + 1 >= _expression.Length)
            {
                throw new JsonException("Path ended with open query.");
            }
            if (Match("=="))
            {
                return QueryOperator.Equals;
            }
            if (Match("!=") || Match("<>"))
            {
                return QueryOperator.NotEquals;
            }
            if (Match("<="))
            {
                return QueryOperator.LessThanOrEquals;
            }
            if (Match("<"))
            {
                return QueryOperator.LessThan;
            }
            if (Match(">="))
            {
                return QueryOperator.GreaterThanOrEquals;
            }
            if (Match(">"))
            {
                return QueryOperator.GreaterThan;
            }
            throw new JsonException("Could not read query operator.");
        }

        private PathFilter ParseQuotedField(char indexerCloseChar)
        {
            List<string> list = null;
            while (_currentIndex < _expression.Length)
            {
                string text = ReadQuotedString();
                EatWhitespace();
                EnsureLength("Path ended with open indexer.");
                if (_expression[_currentIndex] == indexerCloseChar)
                {
                    if (list != null)
                    {
                        list.Add(text);
                        return new FieldMultipleFilter
                        {
                            Names = list
                        };
                    }
                    return new FieldFilter
                    {
                        Name = text
                    };
                }
                else
                {
                    if (_expression[_currentIndex] != ',')
                    {
                        throw new JsonException("Unexpected character while parsing path indexer: " + _expression[_currentIndex].ToString());
                    }
                    _currentIndex++;
                    EatWhitespace();
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(text);
                }
            }
            throw new JsonException("Path ended with open indexer.");
        }

        private void EnsureLength(string message)
        {
            if (_currentIndex >= _expression.Length)
            {
                throw new JsonException(message);
            }
        }

        internal IEnumerable<JToken> Evaluate(JToken t, bool errorWhenNoMatch)
        {
            return JPath.Evaluate(Filters, t, errorWhenNoMatch);
        }

        internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken t, bool errorWhenNoMatch)
        {
            IEnumerable<JToken> enumerable = new JToken[]
            {
                t
            };
            foreach (PathFilter pathFilter in filters)
            {
                enumerable = pathFilter.ExecuteFilter(enumerable, errorWhenNoMatch);
            }
            return enumerable;
        }

        private readonly string _expression;

        private int _currentIndex;
    }
}
