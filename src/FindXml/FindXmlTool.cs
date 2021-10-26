using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FindXml
{
    public class FindXmlSettings
    {
        public FindXmlSettings()
        {
            SearchSubtree = false;
            ShowFileNamesOnly = false;
            IgnoreNamespaces = true;
            Output = Console.Out;
        }

        public bool SearchSubtree { get; set; }
        public bool ShowFileNamesOnly { get; set; }
        public bool IgnoreNamespaces { get; set; }
        public string XPathToMatch { get; set; }
        public TextWriter Output { get; set; }
    }

    public class FindXmlTool
    {
        public FindXmlSettings FindSettings { get; }

        public FindXmlTool()
            : this(new FindXmlSettings())
        {
        }

        public FindXmlTool(FindXmlSettings findSettings)
        {
            this.FindSettings = findSettings;
        }

        public void FindInFiles(IEnumerable<string> filePatterns)
        {
            foreach (var filePattern in filePatterns)
            {
                this.FindInFile(filePattern);
            }
        }

        public void FindInFile(string filePattern)
        {
            string searchDirectory = Environment.CurrentDirectory;

            if (Path.IsPathRooted(filePattern))
            {
                searchDirectory = Path.GetDirectoryName(filePattern);
                filePattern = Path.GetFileName(filePattern);
            }
            else
            {
                string combined = Path.Combine(searchDirectory, filePattern);
                filePattern = Path.GetFileName(combined);
                searchDirectory = Path.GetDirectoryName(combined);
            }

            var searchOption = this.FindSettings.SearchSubtree ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(searchDirectory, filePattern, searchOption);

            foreach (var file in files)
            {
                try
                {
                    string relativeFilePath = file;
                    if (file.StartsWith(searchDirectory, StringComparison.CurrentCultureIgnoreCase))
                    {
                        relativeFilePath = file.Substring(searchDirectory.Length + 1);
                    }

                    using (var xmlReader = XmlReader.Create(file))
                    {
                        XPathNodeIterator nodeIterator = FindXmlInReader(xmlReader);
                        if (nodeIterator != null)
                        {
                            foreach (XPathNavigator xmlNode in nodeIterator)
                            {
                                OutputMatchingNode(xmlNode, relativeFilePath);
                                if (this.FindSettings.ShowFileNamesOnly)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    this.FindSettings.Output.WriteLine($"ERROR processing file {file}: {e.GetType().Name}: {e.Message}");
                }
            }
        }

        public XPathNodeIterator FindXmlInReader(XmlReader xmlReader)
        {
            if (this.FindSettings.IgnoreNamespaces)
            {
                xmlReader = new NoNamespaceXmlReader(xmlReader);
            }

            XPathDocument xPathDoc = new XPathDocument(xmlReader, XmlSpace.Preserve);
            var nav = xPathDoc.CreateNavigator();
            XPathExpression expr = nav.Compile(this.FindSettings.XPathToMatch);
            expr.SetContext(new XPath20FunctionsContext());
            XPathNodeIterator nodeIterator = nav.Select(expr);
            return nodeIterator;
        }

        public void OutputMatchingNode(XPathNavigator xmlNode, string fileName)
        {
            StringBuilder output = new StringBuilder();
            if (!string.IsNullOrEmpty(fileName))
            {
                output.Append($"{fileName}");
            }

            if (!this.FindSettings.ShowFileNamesOnly)
            {
                if (xmlNode is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo())
                {
                    output.Append($"(@{xmlLineInfo.LineNumber})");
                    output.AppendLine();
                    if (xmlLineInfo.LinePosition > 1)
                    {
                        // Indent the first line as it was in its original source document
                        output.Append(new string(' ', xmlLineInfo.LinePosition - 2));
                    }
                }
                else
                {
                    output.AppendLine();
                }

                output.AppendLine(xmlNode.OuterXml);

                output.AppendLine();
            }

            this.FindSettings.Output.WriteLine(output);
        }
    }
}