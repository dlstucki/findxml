# findxml
It's like findstr but for searching XML files that match the given xpath

```
Searches for XML in files.
FINDXML [-S] [-N] [-M] xpath_query [[drive:][path]filename ...]

  -S            Searches for matching files in the current directory and all subdirectories.
  -M            Prints only the filename if a file contains a match.
  -N            Specifies that strict namespace handling be performed. To make composing queries
                simpler the default is to ignore XML namespaces.
  xpath_query   XPath query to find matching XML nodes. See Examples below.
  filename      Specifies a file or files to search.

XPath quick reference:
  /nodename                 Selects the root element 'nodename'.
  //nodename                Selects nodes named 'nodename' anywhere in the document.
  *                         Matches any element node.
  @*                        Matches any attribute node.
  .                         Refers to the current node
  ..                        Refers to the parent node
  local-name()              Returns a string representing the local name of the given node.
  namespace-uri()           Returns a string representing the namespace URI of the given node.
  upper-case()              Convert the given string value to upper case.
  matches(str, pat[, 'i'])  Returns true if 'str' matches the regular expression
                            supplied as 'pat'. If a third parameter, 'i', is provided
                            then the matching ignores case.
Examples:

  FINDXML -SM /TestContext *.xml
      Search current and all subdirectories for *.XML files with root elements
      named "TestContext" in any namespace. Display the matching file names only.

  FINDXML -S //Reference *.csproj
      Search current and all subdirectories for *.csproj files with elements
      named Reference in any namespace.

  FINDXML -S //Reference/@Include[string()='System.Xml']/.. *.csproj
      Search current and all subdirectories for *.csproj files with elements
      named Reference which have an attribute named 'Include' with the value of
      'System.Xml' (whole string, case sensitive). Use '/..' syntax to return
      the entire parent Reference Element.

  FINDXML -S //Reference/@Include[matches(string(),'system.xml','i')]/.. *.csproj
      Similar to the previous example but the Include attribute is matched case-insentive
      and according to the regex 'system.xml'. Any Include attributes containing that value
      will be matched. To get an exact match use the beginning and end anchors: '^system.xml$'

  FINDXML -SN "/*[local-name()='Project' and namespace-uri()='http://sample.com/2021']" *.xml
      Search current and all subdirectories for *.xml files with root elements
      named 'Project' in xmlns:'http://sample.com/2021'

  FINDXML -S //OutDirSuffix[matches(.,'^Build\\Test$')]/../..//Reference/@Include[matches(.,'^Newtonsoft\.Json$','i')]/../Private[matches(.,'true','i')]/.. *.csproj
      Search current and all subdirectories for *.csproj files with the following criteria:
        - Contains an element named 'OutDirSuffix' matching 'Build\Test' (whole string, case sensitive).
        - Also in the same file there is an element named 'Reference' which has:
            - An attribute named 'Include' with the value of 'Newtonsoft.json (whole string, ignore case).
            - A child element named 'Private' with the value of 'true' (partial string, ignore case).
```
