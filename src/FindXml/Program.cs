using System;
using System.Collections.Generic;
using System.IO;

namespace FindXml
{
    class Program
    {
        static void Main(string[] args)
        {
            var findSettings = new FindXmlSettings
            {
                IgnoreNamespaces = true,
                Output = Console.Out,
            };

            var filePatterns = new List<string>();
            for (int i = 0; args != null && i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("-") ||
                    (arg.StartsWith("/") && !arg.StartsWith("//")))
                {
                    string argToLower = arg.ToLowerInvariant();
                    for (int j = 0; j < argToLower.Length; j++)
                    {
                        char c = argToLower[j];
                        switch (c)
                        {
                            case 'n':
                                findSettings.IgnoreNamespaces = false;
                                break;
                            case 's':
                                findSettings.SearchSubtree = true;
                                break;
                            case 'm':
                                findSettings.ShowFileNamesOnly = true;
                                break;
                            case 'c':
                                // match quoted string which can contain spaces...
                                findSettings.XPathToMatch = arg.Split(':')[1]?.Replace("\"", "");
                                j = argToLower.Length;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(findSettings.XPathToMatch))
                {
                    findSettings.XPathToMatch = arg;
                }
                else
                {
                    filePatterns.Add(arg);
                }
            }

            var findXmlTool = new FindXmlTool(findSettings);
            findXmlTool.FindInFiles(filePatterns);
        }
    }
}
