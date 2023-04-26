using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODROutline : XODRBase
    {
        public XODROutline(XmlElement element) : base(element) { }
    }

    public static class XODROutlineExtentions
    {
         public static XODRCornerLocal AddCornerLocalElement(this XODROutline parent, string u, string v)
        {
            var cornerLocal = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));

            cornerLocal.SetAttribute("u", u);
            cornerLocal.SetAttribute("v", v);

            parent.AppendChild(cornerLocal.XmlElement);

            return cornerLocal;
        }

    }


    public class XODRCornerLocal : XODRBase
    {
        public XODRCornerLocal(XmlElement element) : base(element) { }
    }

}