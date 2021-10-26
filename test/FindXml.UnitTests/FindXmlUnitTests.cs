namespace FindXml.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FindXmlUnitTests
    {
        [TestMethod]
        public void IgnoreNamespacesRoot()
        {
            const string Xml = "<Project xmlns=\"http://schemas.com/2021\"><Reference Include=\"System\" /></Project>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = true;
            findXmlTool.FindSettings.XPathToMatch = "/Project";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<Project>\r\n  <Reference Include=\"System\" />\r\n</Project>", results[0]);
            }
        }

        [TestMethod]
        public void IgnoreNamespacesChild()
        {
            const string Xml = "<Project xmlns=\"http://schemas.com/2021\"><Reference Include=\"System\" /></Project>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = true;
            findXmlTool.FindSettings.XPathToMatch = "//Reference";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<Reference Include=\"System\" />", results[0]);
            }
        }

        [TestMethod]
        public void IgnoreNamespacesWithAliasesRoot()
        {
            const string Xml = "<book:BOOK xmlns:book=\"http://www.contoso.com\" xmlns:t=\"http://www.fabrikam.com\">" + 
                                 "<t:title>My Wonderful Day</t:title>" +
                               "</book:BOOK>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = true;
            findXmlTool.FindSettings.XPathToMatch = "//BOOK";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<BOOK>\r\n  <title>My Wonderful Day</title>\r\n</BOOK>", results[0]);
            }
        }

        [TestMethod]
        public void IgnoreNamespacesWithAliasesChild()
        {
            const string Xml = "<book:BOOK xmlns:book=\"http://www.contoso.com\" xmlns:t=\"http://www.fabrikam.com\">" +
                                 "<t:title>My Wonderful Day</t:title>" +
                               "</book:BOOK>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = true;
            findXmlTool.FindSettings.XPathToMatch = "//title";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<title>My Wonderful Day</title>", results[0]);
            }
        }

        [TestMethod]
        public void IgnoreNamespacesWithAliasesMatchChildReturnParent()
        {
            const string Xml = "<book:BOOK xmlns:book=\"http://www.contoso.com\" xmlns:t=\"http://www.fabrikam.com\">" +
                                 "<t:title>My Wonderful Day</t:title>" +
                               "</book:BOOK>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = true;
            findXmlTool.FindSettings.XPathToMatch = "//title[matches(string(),'Wonderful')]/..";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<BOOK>\r\n  <title>My Wonderful Day</title>\r\n</BOOK>", results[0]);
            }
        }

        [TestMethod]
        public void PreserveNamespacesRoot()
        {
            const string Xml = "<Project xmlns=\"http://schemas.com/2021\"><Reference Include=\"System\" /></Project>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = false;
            findXmlTool.FindSettings.XPathToMatch = "/*[local-name()='Project']";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<Project xmlns=\"http://schemas.com/2021\">\r\n  <Reference Include=\"System\" />\r\n</Project>", results[0]);
            }
        }

        [TestMethod]
        public void PreserveNamespacesChild()
        {
            const string Xml = "<Project xmlns=\"http://schemas.com/2021\"><Reference Include=\"System\" /></Project>";
            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.IgnoreNamespaces = false;
            findXmlTool.FindSettings.XPathToMatch = "//*[local-name()='Reference']";
            using (var xmlReader = XmlReader.Create(new StringReader(Xml)))
            {
                XPathNodeIterator findResults = findXmlTool.FindXmlInReader(xmlReader);
                string[] results = LoadResults(findResults);
                Assert.AreEqual(1, results.Length, "Results.Length");
                Assert.AreEqual("<Reference Include=\"System\" xmlns=\"http://schemas.com/2021\" />", results[0]);
            }
        }

        [TestMethod]
        public void FilePatternUsingRelativePath()
        {
            string filePattern = "..\\..\\..\\..\\src\\FindXml\\findxml.csproj";

            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.XPathToMatch = "//*[local-name()='Reference']";
            findXmlTool.FindInFiles(new[] { filePattern });
        }

        [TestMethod]
        public void FilePatternUsingRelativePathAndWildcard()
        {
            string filePattern = "..\\..\\..\\..\\*.csproj";

            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.XPathToMatch = "//*[local-name()='Reference']";
            findXmlTool.FindSettings.SearchSubtree = true;
            findXmlTool.FindInFiles(new[] { filePattern });
        }

        [TestMethod]
        public void FilePatternUsingAbsolutePath()
        {
            string filePattern = "D:\\src\\GitHub\\dlstucki\\findxml\\src\\FindXml\\findxml.csproj";

            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.XPathToMatch = "//*[local-name()='Reference']";
            findXmlTool.FindInFiles(new[] { filePattern });
        }

        [TestMethod]
        public void FilePatternUsingAbsolutePathWithWildcard()
        {
            string filePattern = "D:\\src\\GitHub\\dlstucki\\findxml\\*.csproj";

            var findXmlTool = new FindXmlTool();
            findXmlTool.FindSettings.XPathToMatch = "//*[local-name()='Reference']";
            findXmlTool.FindSettings.SearchSubtree = true;
            findXmlTool.FindInFiles(new[] { filePattern });
        }

        string[] LoadResults(XPathNodeIterator nodeIterator)
        {
            var results = new List<string>();
            foreach (XPathNavigator matchNode in nodeIterator)
            {
                Trace.WriteLine("Result: \r\n" + matchNode.OuterXml);
                results.Add(matchNode.OuterXml);
            }

            return results.ToArray();
        }
    }
}
