using System.Text.RegularExpressions;
using Xunit;
using XmlToPox;

namespace Test
{
    /// <summary>
    /// Tests for <see cref="XmlTools.Convert"/>.
    /// </summary>
    public class ConvertTests
    {
        /// <summary>
        /// Collapses all whitespace so structural assertions are independent of
        /// the platform newline and indentation emitted by XElement.ToString().
        /// </summary>
        private static string Collapse(string val) => Regex.Replace(val, @"\s+", "");

        #region Positive

        [Fact]
        public void Convert_SimpleDocument_PreservesStructure()
        {
            string result = XmlTools.Convert("<a><b>hello</b></a>");
            Assert.Equal("<a><b>hello</b></a>", Collapse(result));
        }

        [Fact]
        public void Convert_RemovesNamespaces()
        {
            string result = XmlTools.Convert("<a xmlns:x=\"http://example.com/x\"><x:b>hi</x:b></a>");
            Assert.Equal("<a><b>hi</b></a>", Collapse(result));
            Assert.DoesNotContain("xmlns", result);
            Assert.DoesNotContain("x:", result);
        }

        [Fact]
        public void Convert_RemovesEmptyElements()
        {
            string result = XmlTools.Convert("<a><b></b><c>keep</c></a>");
            Assert.Equal("<a><c>keep</c></a>", Collapse(result));
        }

        [Fact]
        public void Convert_RemovesAttributes()
        {
            string result = XmlTools.Convert("<a id=\"1\"><b attr=\"2\">v</b></a>");
            Assert.Equal("<a><b>v</b></a>", Collapse(result));
            Assert.DoesNotContain("id", result);
            Assert.DoesNotContain("attr", result);
        }

        [Fact]
        public void Convert_NestedDocument_IsPreserved()
        {
            string result = XmlTools.Convert("<root><a><b><c>deep</c></b></a></root>");
            Assert.Equal("<root><a><b><c>deep</c></b></a></root>", Collapse(result));
        }

        [Fact]
        public void Convert_RepeatedElements_AreAllPreserved()
        {
            string result = XmlTools.Convert("<list><item>1</item><item>2</item><item>3</item></list>");
            Assert.Equal("<list><item>1</item><item>2</item><item>3</item></list>", Collapse(result));
        }

        #endregion

        #region Negative

        [Fact]
        public void Convert_Null_ReturnsNull()
        {
            Assert.Null(XmlTools.Convert(null));
        }

        [Fact]
        public void Convert_Empty_ReturnsNull()
        {
            Assert.Null(XmlTools.Convert(string.Empty));
        }

        [Fact]
        public void Convert_MalformedXml_ReturnsNull()
        {
            Assert.Null(XmlTools.Convert("<a><b></a>"));
        }

        [Fact]
        public void Convert_NonXmlText_ReturnsNull()
        {
            Assert.Null(XmlTools.Convert("just some text"));
        }

        #endregion
    }
}
