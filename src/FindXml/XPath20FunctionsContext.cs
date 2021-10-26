using System;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace FindXml
{
    /// <summary>
    /// This Adds XPath support for 2.0 matches() and lower-case() functions.
    /// matches(string,'patternToMatch','i') allows matching regular expressions and an easy way to do case-insensitive matching.
    /// </summary>
    class XPath20FunctionsContext : XsltContext
    {
        public override bool Whitespace => true;
        public override int CompareDocument(string baseUri, string nextbaseUri) => throw new NotImplementedException();
        public override bool PreserveWhitespace(XPathNavigator node) => true;

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
        {
            if (name == "lower-case")
            {
                return new LowerCaseFunction(argTypes);
            }
            else if (name == "matches")
            {
                return new MatchesFunction(argTypes);
            }
            else
            {
                return null;
            }
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name) => throw new NotImplementedException();

        class LowerCaseFunction : IXsltContextFunction
        {
            XPathResultType[] _argTypes;

            public LowerCaseFunction(XPathResultType[] argTypes)
            {
                _argTypes = argTypes;

                foreach (XPathResultType t in argTypes)
                {
                    if (t != XPathResultType.NodeSet && t != XPathResultType.Navigator)
                    {
                        throw new Exception("incorrect argument type: Navigator or NodeSet expected");
                    }
                }
            }

            public int Minargs { get { return 1; } }
            public int Maxargs { get { return 1; } }
            public XPathResultType ReturnType { get { return XPathResultType.String; } }
            public XPathResultType[] ArgTypes { get { return _argTypes; } }

            public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                string value = FunctionUtility.CoerceToString(args[0]);
                return value.ToLowerInvariant();
            }
        }

        class MatchesFunction : IXsltContextFunction
        {
            XPathResultType[] _argTypes;

            public MatchesFunction(XPathResultType[] argTypes)
            {
                _argTypes = argTypes;
                foreach (XPathResultType t in argTypes)
                    if (t != XPathResultType.NodeSet && t != XPathResultType.Navigator) throw new Exception("incorrect argument type: Navigator or NodeSet expected");
            }

            public int Minargs { get { return 2; } }
            public int Maxargs { get { return 3; } }
            public XPathResultType ReturnType { get { return XPathResultType.Boolean; } }
            public XPathResultType[] ArgTypes { get { return _argTypes; } }

            public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
                string inputString = FunctionUtility.CoerceToString(args[0]);
                string patternString = FunctionUtility.CoerceToString(args[1]);
                var regexOptions = RegexOptions.None;
                if (args.Length > 2 && string.Equals(FunctionUtility.CoerceToString(args[2]), "i", StringComparison.OrdinalIgnoreCase))
                {
                    regexOptions |= RegexOptions.IgnoreCase;
                }

                return Regex.Match(inputString, patternString, regexOptions);
            }
        }

        static class FunctionUtility
        {
            public static string CoerceToString(object value)
            {
                if (value is string stringValue)
                {
                    return stringValue;
                }
                else if (value is XPathNodeIterator xpathIterator)
                {
                    foreach (XPathItem item in xpathIterator)
                    {
                        return item.Value;
                    }

                    return null;
                }
                else
                {
                    throw new NotImplementedException($"Handling objects of type {value.GetType()} is not yet implemented");
                }
            }
        }
    }
}