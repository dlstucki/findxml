using System;
using System.Threading.Tasks;
using System.Xml;

namespace FindXml
{
    class NoNamespaceXmlReader : XmlReader, IXmlLineInfo
    {
        readonly XmlReader xmlReader;

        public NoNamespaceXmlReader(XmlReader xmlReader) => this.xmlReader = xmlReader;

        public override XmlNodeType NodeType => this.xmlReader.NodeType;

        public override string LocalName => this.xmlReader.LocalName;

        public override string NamespaceURI => ""; //this.xmlReader.NamespaceURI;

        public override string Prefix => "";

        public override string Value => this.xmlReader.Value;

        public override int Depth => this.xmlReader.Depth;

        public override string BaseURI => this.xmlReader.BaseURI;

        public override bool IsEmptyElement => this.xmlReader.IsEmptyElement;

        public override int AttributeCount => this.xmlReader.AttributeCount;

        public override bool EOF => this.xmlReader.EOF;

        public override ReadState ReadState => this.xmlReader.ReadState;

        public override XmlNameTable NameTable => this.xmlReader.NameTable;

        public override string GetAttribute(string name) => this.xmlReader.GetAttribute(name);
        public override string GetAttribute(string name, string namespaceURI) => this.xmlReader.GetAttribute(name, namespaceURI);
        public override string GetAttribute(int i) => this.xmlReader.GetAttribute(i);
        public override string LookupNamespace(string prefix) => this.xmlReader.LookupNamespace(prefix);
        public override bool MoveToAttribute(string name) => this.xmlReader.MoveToAttribute(name);
        public override bool MoveToAttribute(string name, string ns) => this.xmlReader.MoveToAttribute(name, ns);
        public override bool MoveToElement() => this.xmlReader.MoveToElement();
        public override bool MoveToFirstAttribute() => throw new NotImplementedException("Would need to filter out xmlns attributes like MoveToNextAttribute method does");

        public override bool MoveToNextAttribute()
        {
            bool result = this.xmlReader.MoveToNextAttribute();
            while (result &&
                this.xmlReader.NodeType == XmlNodeType.Attribute &&
                (this.xmlReader.LocalName == "xmlns" || this.xmlReader.Prefix == "xmlns"))
            {
                result = this.xmlReader.MoveToNextAttribute();
            }

            return result;
        }

        public override bool Read() => this.xmlReader.Read();

        public override bool ReadAttributeValue() => this.xmlReader.ReadAttributeValue();
        public override void ResolveEntity() => this.xmlReader.ResolveEntity();

        public override bool CanReadBinaryContent => this.xmlReader.CanReadBinaryContent;

        public override XmlNodeType MoveToContent() => this.xmlReader.MoveToContent();

        public override void MoveToAttribute(int i) => this.xmlReader.MoveToAttribute(i);

        public override bool CanReadValueChunk => this.xmlReader.CanReadValueChunk;

        public override bool CanResolveEntity => this.xmlReader.CanResolveEntity;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.xmlReader.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Close()
        {
            this.Dispose(true);
            base.Close();
        }

        public override Task<string> GetValueAsync() => this.xmlReader.GetValueAsync();

        public override bool HasAttributes => this.xmlReader.HasAttributes;
        public override bool HasValue => this.xmlReader.HasValue;

        public override bool IsStartElement() => this.xmlReader.IsStartElement();
        public override bool IsStartElement(string localname, string ns) => this.xmlReader.IsStartElement(localname, ns);
        public override bool IsStartElement(string name) => this.IsStartElement(name);
        public override bool IsDefault => this.xmlReader.IsDefault;

        bool IXmlLineInfo.HasLineInfo()
        {
            return this.xmlReader is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo();
        }

        int IXmlLineInfo.LineNumber => this.xmlReader is IXmlLineInfo xmlLineLInfo ? xmlLineLInfo.LineNumber : 0;

        int IXmlLineInfo.LinePosition => this.xmlReader is IXmlLineInfo xmlLineLInfo ? xmlLineLInfo.LinePosition : 0;
    }
}