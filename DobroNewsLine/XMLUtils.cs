using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DobroNewsLine
{
    static class XMLUtils
    {
        public static XDocument SettingsXMLDoc //valid
        {
            get
            {
                XDocument XMLDoc = XDocument.Load(@"C:\Users\Volodimir\Documents\Visual Studio 2013\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml");
                return XMLDoc;
            }
        }

        public static void PictInAdvert(string PictBase64String, XElement CurrentElement)
        {
            bool AreEqual = true;
            foreach (XElement Pict in CurrentElement.Elements())
            {
                string ExistPict = Pict.Attribute("base64Data").Value;
                AreEqual = String.Equals(ExistPict, PictBase64String, StringComparison.Ordinal);
                if (AreEqual)
                {
                    return;
                }
            }            
            XElement Pictlement = new XElement("Pict",
                new XAttribute("base64Data", PictBase64String)
            );
            CurrentElement.Add(Pictlement);            
        }

        public static void MergeXElements(NewsItem AdvertItem, XDocument CurrentDoc)
        {
            string Phone = AdvertItem.Phone;
            XElement AdvertLines = (from el in CurrentDoc.Root.Elements("advert")
                                    where (string)el.Attribute("phone") == Phone
                                    select el).Single<XElement>();
            if (AdvertItem.PictList != null)
            {
                foreach (string PictBase64String in AdvertItem.PictList)
                {
                    PictInAdvert(PictBase64String, AdvertLines);
                }
            }
        }

        public static void SaveAdvertData(NewsItem AdvertItem)
        {
            XDocument CurrentDoc = SettingsXMLDoc;
            //if (IsNewRecord(AdvertItem))
            //{
                CurrentDoc.Root.Add(CreateAdverItemXElement(AdvertItem));
            //}
            //else
            //{
                //MergeXElements(AdvertItem, CurrentDoc);
            //}
            CurrentDoc.Save(@"C:\Users\Volodimir\Documents\Visual Studio 2013\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml");
        }

        public static XElement CreateAdverItemXElement(NewsItem AdvertItem)
        {
            XElement AdvertItemXElement = new XElement("advert");
            AddAttribute(AdvertItemXElement, "title", AdvertItem.Title);
            AddAttribute(AdvertItemXElement, "link", AdvertItem.Link.ToString());
            if (AdvertItem.Body != null)
            {
                AddAttribute(AdvertItemXElement, "body", AdvertItem.Body);
            }

            if (AdvertItem.CityRegion != null)
            {
                AddAttribute(AdvertItemXElement, "cityRegion", AdvertItem.CityRegion);
            }

            if (AdvertItem.Date != null)
            {
                AddAttribute(AdvertItemXElement, "date", AdvertItem.Date);
            }

            if (AdvertItem.Age != null)
            {
                AddAttribute(AdvertItemXElement, "age", AdvertItem.Age);
            }

            if (AdvertItem.Phone != null)
            {
                AddAttribute(AdvertItemXElement, "phone", AdvertItem.Phone);
            }

            if (AdvertItem.PictList != null)
            {
                foreach (string PictStr in AdvertItem.PictList)
                {
                    XElement PictlEment = new XElement("Pict",
                            new XAttribute("base64Data", PictStr)
                        );
                    AdvertItemXElement.Add(PictlEment);
                }
            }
            return AdvertItemXElement;
        }

        private static void AddAttribute(XElement XMLElement, string AttributeName, string AttributeValue)
        {
            XAttribute attribute = new XAttribute(AttributeName, AttributeValue);
            XMLElement.Add(attribute);
        }

        public static bool IsNewRecord(NewsItem AdvertItem)
        {
            string Phone = AdvertItem.Phone;
            IEnumerable<XElement> AdvertLines = (from el in SettingsXMLDoc.Root.Elements("advert")
                                                 where (string)el.Attribute("phone") == Phone
                                                 select el);
            ICollection<XElement> AdvertLinesCollection = AdvertLines.ToList();
            return AdvertLinesCollection.Count == 0;
        }
    }
}
