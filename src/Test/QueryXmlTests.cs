using Xunit;
using XmlToPox;

namespace Test
{
    /// <summary>
    /// Tests for <see cref="XmlTools.QueryXml"/>.
    /// </summary>
    public class QueryXmlTests
    {
        private const string Catalog =
            "<catalog>" +
                "<item><number>QWZ5671</number><price>39.95</price></item>" +
                "<item><number>RRX9856</number><price>42.50</price></item>" +
            "</catalog>";

        #region Positive

        [Fact]
        public void QueryXml_FirstItemLeaf_ReturnsValue()
        {
            Assert.Equal("QWZ5671", XmlTools.QueryXml(Catalog, "/catalog/item[1]/number"));
        }

        [Fact]
        public void QueryXml_SecondItemLeaf_ReturnsValue()
        {
            Assert.Equal("42.50", XmlTools.QueryXml(Catalog, "/catalog/item[2]/price"));
        }

        [Fact]
        public void QueryXml_QueryStripsNamespaces()
        {
            string xml = "<catalog xmlns:n=\"http://example.com/n\"><n:item><n:number>ABC123</n:number></n:item></catalog>";
            Assert.Equal("ABC123", XmlTools.QueryXml(xml, "/catalog/item[1]/number"));
        }

        [Fact]
        public void QueryXml_LeafSelectedDirectly_ReturnsValue()
        {
            string xml = "<root><value>hello world</value></root>";
            Assert.Equal("hello world", XmlTools.QueryXml(xml, "/root/value"));
        }

        #endregion

        #region Negative

        [Fact]
        public void QueryXml_NullXml_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(null, "/catalog/item[1]/number"));
        }

        [Fact]
        public void QueryXml_EmptyXml_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(string.Empty, "/catalog"));
        }

        [Fact]
        public void QueryXml_NullPath_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(Catalog, null));
        }

        [Fact]
        public void QueryXml_EmptyPath_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(Catalog, string.Empty));
        }

        [Fact]
        public void QueryXml_NonexistentPath_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(Catalog, "/catalog/item[1]/doesnotexist"));
        }

        [Fact]
        public void QueryXml_InvalidXPath_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(Catalog, "///"));
        }

        [Fact]
        public void QueryXml_MalformedXml_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml("<catalog><item></catalog>", "/catalog/item"));
        }

        [Fact]
        public void QueryXml_OutOfRangeIndex_ReturnsNull()
        {
            Assert.Null(XmlTools.QueryXml(Catalog, "/catalog/item[99]/price"));
        }

        #endregion
    }
}
