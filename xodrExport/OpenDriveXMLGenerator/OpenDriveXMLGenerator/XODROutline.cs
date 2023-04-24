using System.Xml;

namespace OpenDriveXMLGenerator
{
    public class XODROutline : XODRBase
    {
        public XODROutline(XmlElement element) : base(element) { }
    }

    public static class XODROutlineExtentions
    {
         public static XODRCornerLocal AddCornerLocalElement(this XODROutline parent, string topLeftu = "3.8000000000000000e+0", string topLeftv = "1.5000000000000000e+0", string topLeftz = "-4.4000000000000000e-10")
        {
            var cornerLocal = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));
            var cornerLocal1 = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));
            var cornerLocal2 = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));
            var cornerLocal3 = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));
            var cornerLocal4 = new XODRCornerLocal(parent.OwnerDocument.CreateElement("cornerLocal"));

            cornerLocal.SetAttribute("u", topLeftu);
            cornerLocal.SetAttribute("v", topLeftv);
            cornerLocal.SetAttribute("z", topLeftz);

            // cornerLocal1.SetAttribute("u");
            // cornerLocal1.SetAttribute("v");
            // cornerLocal1.SetAttribute("z");

            // cornerLocal2.SetAttribute("u");
            // cornerLocal2.SetAttribute("v");
            // cornerLocal2.SetAttribute("z");

            // cornerLocal3.SetAttribute("u");
            // cornerLocal3.SetAttribute("v");
            // cornerLocal3.SetAttribute("z");

            // cornerLocal4.SetAttribute("u");
            // cornerLocal4.SetAttribute("v");
            // cornerLocal4.SetAttribute("z");




            parent.AppendChild(cornerLocal.XmlElement);

            return cornerLocal;
        }

    }


    public class XODRCornerLocal : XODRBase
    {
        public XODRCornerLocal(XmlElement element) : base(element) { }
    }

}