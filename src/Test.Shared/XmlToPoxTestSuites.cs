namespace Test.Shared
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Touchstone.Core;
    using XmlToPox;

    /// <summary>
    /// Central source of truth for every XmlToPox test.  The suites defined here
    /// are consumed unchanged by the Touchstone CLI runner (Test.Automated), the
    /// xUnit adapter (Test.Xunit), and the NUnit adapter (Test.Nunit), so a case
    /// authored once executes identically under all three hosts.
    ///
    /// XmlToPox exposes exactly two public methods, and both are covered
    /// exhaustively in the positive and negative directions:
    ///   * <see cref="XmlTools.Convert(string)"/>  -> Convert suites
    ///   * <see cref="XmlTools.QueryXml(string, string)"/> -> QueryXml suites
    /// </summary>
    public static class XmlToPoxTestSuites
    {
        #region Public-Members

        /// <summary>
        /// All XmlToPox test suites.
        /// </summary>
        public static IReadOnlyList<TestSuiteDescriptor> All
        {
            get
            {
                return new List<TestSuiteDescriptor>
                {
                    BuildConvertPositiveSuite(),
                    BuildConvertNegativeSuite(),
                    BuildQueryXmlPositiveSuite(),
                    BuildQueryXmlNegativeSuite()
                };
            }
        }

        #endregion

        #region Private-Members

        private const string _Catalog =
            "<catalog>" +
                "<item><number>QWZ5671</number><price>39.95</price></item>" +
                "<item><number>RRX9856</number><price>42.50</price></item>" +
            "</catalog>";

        /// <summary>
        /// Collapse all whitespace so structural assertions are independent of the
        /// platform newline and indentation emitted by XElement.ToString().
        /// </summary>
        private static string Collapse(string val)
        {
            return Regex.Replace(val ?? string.Empty, @"\s+", string.Empty);
        }

        private static TestCaseDescriptor Case(string suiteId, string caseId, string displayName, System.Action body)
        {
            return new TestCaseDescriptor(
                suiteId,
                caseId,
                displayName,
                _ =>
                {
                    body();
                    return Task.CompletedTask;
                });
        }

        #endregion

        #region Convert-Positive

        private static TestSuiteDescriptor BuildConvertPositiveSuite()
        {
            const string suite = "convert.positive";

            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                Case(suite, "simple-structure", "Convert preserves a simple document structure", () =>
                {
                    string result = XmlTools.Convert("<a><b>hello</b></a>");
                    Expect.NotNull(result);
                    Expect.Equal("<a><b>hello</b></a>", Collapse(result));
                }),

                Case(suite, "removes-prefixed-namespace", "Convert strips a prefixed namespace", () =>
                {
                    string result = XmlTools.Convert("<a xmlns:x=\"http://example.com/x\"><x:b>hi</x:b></a>");
                    Expect.Equal("<a><b>hi</b></a>", Collapse(result));
                    Expect.DoesNotContain("xmlns", result);
                    Expect.DoesNotContain("x:", result);
                }),

                Case(suite, "removes-default-namespace", "Convert strips a default namespace", () =>
                {
                    string result = XmlTools.Convert("<a xmlns=\"http://example.com\"><b>hi</b></a>");
                    Expect.Equal("<a><b>hi</b></a>", Collapse(result));
                    Expect.DoesNotContain("xmlns", result);
                }),

                Case(suite, "removes-multiple-namespaces", "Convert strips multiple namespaces", () =>
                {
                    string result = XmlTools.Convert(
                        "<a xmlns:x=\"http://x\" xmlns:y=\"http://y\"><x:b>1</x:b><y:c>2</y:c></a>");
                    Expect.Equal("<a><b>1</b><c>2</c></a>", Collapse(result));
                    Expect.DoesNotContain("xmlns", result);
                }),

                Case(suite, "removes-empty-elements", "Convert removes empty elements", () =>
                {
                    string result = XmlTools.Convert("<a><b></b><c>keep</c></a>");
                    Expect.Equal("<a><c>keep</c></a>", Collapse(result));
                }),

                Case(suite, "removes-attributes", "Convert removes attributes", () =>
                {
                    string result = XmlTools.Convert("<a id=\"1\"><b attr=\"2\">v</b></a>");
                    Expect.Equal("<a><b>v</b></a>", Collapse(result));
                    Expect.DoesNotContain("id", result);
                    Expect.DoesNotContain("attr", result);
                }),

                Case(suite, "nested-preserved", "Convert preserves a nested document", () =>
                {
                    string result = XmlTools.Convert("<root><a><b><c>deep</c></b></a></root>");
                    Expect.Equal("<root><a><b><c>deep</c></b></a></root>", Collapse(result));
                }),

                Case(suite, "deeply-nested-preserved", "Convert preserves a deeply nested document", () =>
                {
                    string result = XmlTools.Convert("<l1><l2><l3><l4><l5>x</l5></l4></l3></l2></l1>");
                    Expect.Equal("<l1><l2><l3><l4><l5>x</l5></l4></l3></l2></l1>", Collapse(result));
                }),

                Case(suite, "repeated-elements", "Convert preserves repeated sibling elements", () =>
                {
                    string result = XmlTools.Convert("<list><item>1</item><item>2</item><item>3</item></list>");
                    Expect.Equal("<list><item>1</item><item>2</item><item>3</item></list>", Collapse(result));
                }),

                Case(suite, "mixed-empty-and-nonempty", "Convert keeps populated elements and drops empty ones", () =>
                {
                    string result = XmlTools.Convert("<r><a>1</a><b></b><c>3</c><d></d></r>");
                    Expect.Equal("<r><a>1</a><c>3</c></r>", Collapse(result));
                }),

                Case(suite, "single-root-leaf", "Convert preserves a single-element document", () =>
                {
                    string result = XmlTools.Convert("<root>value</root>");
                    Expect.Equal("<root>value</root>", Collapse(result));
                }),

                Case(suite, "numeric-content", "Convert preserves numeric content", () =>
                {
                    string result = XmlTools.Convert("<a><n>12345</n></a>");
                    Expect.Equal("<a><n>12345</n></a>", Collapse(result));
                }),

                Case(suite, "underscore-element-names", "Convert preserves element names containing underscores", () =>
                {
                    string result = XmlTools.Convert("<root_node><child_1>v</child_1></root_node>");
                    Expect.Equal("<root_node><child_1>v</child_1></root_node>", Collapse(result));
                }),

                Case(suite, "escapes-ampersand", "Convert re-escapes an ampersand entity", () =>
                {
                    string result = XmlTools.Convert("<a><b>a &amp; b</b></a>");
                    Expect.Equal("<a><b>a&amp;b</b></a>", Collapse(result));
                }),

                Case(suite, "escapes-less-than", "Convert re-escapes a less-than entity", () =>
                {
                    string result = XmlTools.Convert("<a><b>1 &lt; 2</b></a>");
                    Expect.Equal("<a><b>1&lt;2</b></a>", Collapse(result));
                }),

                Case(suite, "cdata-becomes-text", "Convert flattens a CDATA section to text", () =>
                {
                    string result = XmlTools.Convert("<a><b><![CDATA[hello]]></b></a>");
                    Expect.Equal("<a><b>hello</b></a>", Collapse(result));
                }),

                Case(suite, "returns-nonnull-for-valid", "Convert returns a non-null result for valid XML", () =>
                {
                    Expect.NotNull(XmlTools.Convert("<a>1</a>"));
                })
            };

            return new TestSuiteDescriptor(suite, "Convert - Positive", cases);
        }

        #endregion

        #region Convert-Negative

        private static TestSuiteDescriptor BuildConvertNegativeSuite()
        {
            const string suite = "convert.negative";

            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                Case(suite, "null-returns-null", "Convert(null) returns null", () =>
                {
                    Expect.Null(XmlTools.Convert(null));
                }),

                Case(suite, "empty-returns-null", "Convert(\"\") returns null", () =>
                {
                    Expect.Null(XmlTools.Convert(string.Empty));
                }),

                Case(suite, "whitespace-only-returns-null", "Convert of whitespace-only input returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("   "));
                }),

                Case(suite, "malformed-unclosed", "Convert of an unclosed nested tag returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<a><b></a>"));
                }),

                Case(suite, "mismatched-tags", "Convert of mismatched tags returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<a></b>"));
                }),

                Case(suite, "unclosed-single-tag", "Convert of a single unclosed tag returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<a>"));
                }),

                Case(suite, "non-xml-text", "Convert of plain text returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("just some text"));
                }),

                Case(suite, "multiple-roots", "Convert of multiple root elements returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<a>1</a><b>2</b>"));
                }),

                Case(suite, "declaration-only", "Convert of an XML declaration with no root returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<?xml version=\"1.0\"?>"));
                }),

                Case(suite, "bare-angle-bracket", "Convert of a bare angle bracket returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<"));
                }),

                Case(suite, "unescaped-ampersand", "Convert of an unescaped ampersand returns null", () =>
                {
                    Expect.Null(XmlTools.Convert("<a>a & b</a>"));
                })
            };

            return new TestSuiteDescriptor(suite, "Convert - Negative", cases);
        }

        #endregion

        #region QueryXml-Positive

        private static TestSuiteDescriptor BuildQueryXmlPositiveSuite()
        {
            const string suite = "query.positive";

            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                Case(suite, "first-item-number", "QueryXml returns the first item's leaf value", () =>
                {
                    Expect.Equal("QWZ5671", XmlTools.QueryXml(_Catalog, "/catalog/item[1]/number"));
                }),

                Case(suite, "first-item-price", "QueryXml returns the first item's price", () =>
                {
                    Expect.Equal("39.95", XmlTools.QueryXml(_Catalog, "/catalog/item[1]/price"));
                }),

                Case(suite, "second-item-price", "QueryXml returns the second item's price", () =>
                {
                    Expect.Equal("42.50", XmlTools.QueryXml(_Catalog, "/catalog/item[2]/price"));
                }),

                Case(suite, "second-item-number", "QueryXml returns the second item's number", () =>
                {
                    Expect.Equal("RRX9856", XmlTools.QueryXml(_Catalog, "/catalog/item[2]/number"));
                }),

                Case(suite, "strips-namespaces", "QueryXml resolves a path after stripping namespaces", () =>
                {
                    string xml = "<catalog xmlns:n=\"http://example.com/n\">" +
                                 "<n:item><n:number>ABC123</n:number></n:item></catalog>";
                    Expect.Equal("ABC123", XmlTools.QueryXml(xml, "/catalog/item[1]/number"));
                }),

                Case(suite, "leaf-selected-directly", "QueryXml returns a directly selected leaf value", () =>
                {
                    string xml = "<root><value>hello world</value></root>";
                    Expect.Equal("hello world", XmlTools.QueryXml(xml, "/root/value"));
                }),

                Case(suite, "deep-leaf", "QueryXml returns a deeply nested leaf value", () =>
                {
                    string xml = "<a><b><c><d>deep</d></c></b></a>";
                    Expect.Equal("deep", XmlTools.QueryXml(xml, "/a/b/c/d"));
                }),

                Case(suite, "numeric-leaf", "QueryXml returns a numeric leaf value", () =>
                {
                    Expect.Equal("42", XmlTools.QueryXml("<a><n>42</n></a>", "/a/n"));
                })
            };

            return new TestSuiteDescriptor(suite, "QueryXml - Positive", cases);
        }

        #endregion

        #region QueryXml-Negative

        private static TestSuiteDescriptor BuildQueryXmlNegativeSuite()
        {
            const string suite = "query.negative";

            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                Case(suite, "null-xml", "QueryXml(null, path) returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(null, "/catalog/item[1]/number"));
                }),

                Case(suite, "empty-xml", "QueryXml(\"\", path) returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(string.Empty, "/catalog"));
                }),

                Case(suite, "null-path", "QueryXml(xml, null) returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, null));
                }),

                Case(suite, "empty-path", "QueryXml(xml, \"\") returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, string.Empty));
                }),

                Case(suite, "whitespace-path", "QueryXml with a whitespace-only path returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, "   "));
                }),

                Case(suite, "nonexistent-leaf", "QueryXml of a nonexistent leaf returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, "/catalog/item[1]/doesnotexist"));
                }),

                Case(suite, "nonexistent-root", "QueryXml of a nonexistent root returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, "/nope"));
                }),

                Case(suite, "invalid-xpath", "QueryXml of an invalid XPath returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, "///"));
                }),

                Case(suite, "malformed-xml", "QueryXml of malformed XML returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml("<catalog><item></catalog>", "/catalog/item"));
                }),

                Case(suite, "out-of-range-index", "QueryXml of an out-of-range index returns null", () =>
                {
                    Expect.Null(XmlTools.QueryXml(_Catalog, "/catalog/item[99]/price"));
                })
            };

            return new TestSuiteDescriptor(suite, "QueryXml - Negative", cases);
        }

        #endregion
    }
}
