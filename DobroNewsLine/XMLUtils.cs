using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DobroNewsLine
{
    enum StatusType{Starting, Importing, Finish }
    static class XMLUtils
    {
        private static XDocument _settingsXMLDoc { get; set; }
        public static XDocument SettingsXMLDoc //valid
        {
            get
            {                
                if (_settingsXMLDoc == null)
                {
                    string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
                    _settingsXMLDoc = XDocument.Load(DataFilePath);
                }
                return _settingsXMLDoc;
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
            //XElement Pictlement = new XElement("Pict",
              //  new XAttribute("base64Data", PictBase64String)
            //);
            //CurrentElement.Add(Pictlement);            
            AddPictToElement(PictBase64String, CurrentElement);        
        }

        public static void MergeXElements(NewsItem NewItem, XDocument CurrentDoc)
        {
            string Phone = NewItem.Phone;
            XElement OldItem = (from el in CurrentDoc.Root.Elements("advert")
                                    where (string)el.Attribute("phone") == Phone
                                    select el).Single<XElement>();
            if (NewItem.PictList != null)
            {
                foreach (PictObj CurrPictObj in NewItem.PictList)
                {
                    PictInAdvert(CurrPictObj.Base64Data, OldItem);
                }
            }
        }

        public static void SaveAdvertData(NewsItem AdvertItem)
        {
            XDocument CurrentDoc = SettingsXMLDoc;
            if (IsNewRecord(AdvertItem, CurrentDoc))
            {
                CurrentDoc.Root.Add(CreateAdverItemXElement(AdvertItem));
            }
            else
            {
                MergeXElements(AdvertItem, CurrentDoc);
            }
            string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
            CurrentDoc.Save(DataFilePath);
        }

        public static void SaveAdvertData(List<NewsItem> AdvertItemList)
        {
            XDocument CurrentDoc = SettingsXMLDoc;
            foreach (NewsItem AdvertItem in AdvertItemList)
            {
                if (IsNewRecord(AdvertItem, CurrentDoc))
                {
                    CurrentDoc.Root.Add(CreateAdverItemXElement(AdvertItem));
                }
                else
                {
                    MergeXElements(AdvertItem, CurrentDoc);
                }
            }
            string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
            CurrentDoc.Save(DataFilePath);
        }

        public static XElement CreateAdverItemXElement(NewsItem AdvertItem)
        {
            XElement AdvertItemXElement = new XElement("advert");
            if (AdvertItem.UID == Guid.Empty)
            { 
                AddAttribute(AdvertItemXElement, "UID",  Guid.NewGuid().ToString());
            }
            else
            {
                AddAttribute(AdvertItemXElement, "UID", AdvertItem.UID.ToString());
            }
            AddAttribute(AdvertItemXElement, "title", AdvertItem.Title);
            AddAttribute(AdvertItemXElement, "link", AdvertItem.Link.ToString());
            AddAttribute(AdvertItemXElement, "price", AdvertItem.Price.ToString());
            AddAttribute(AdvertItemXElement, "IsFavorite", AdvertItem.IsFavorite ? "1" : "0");
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
            if (AdvertItem.DefPictId != 0)
            {
                AddAttribute(AdvertItemXElement, "defPictId", AdvertItem.DefPictId.ToString());
            }

            if (AdvertItem.PictList != null)
            {
                int PictCnt = 1;
                foreach (PictObj CurrPictObj in AdvertItem.PictList)
                {
                    AddPictToElement(CurrPictObj.Base64Data, AdvertItemXElement, PictCnt);
                    PictCnt++;
                }
                AddAttribute(AdvertItemXElement, "lastPictId", PictCnt.ToString());
            }
            else
            {
                AddAttribute(AdvertItemXElement, "lastPictId", "0");
            }
            return AdvertItemXElement;
        }

        private static void AddPictToElement(string PictStr, XElement NewsItemXElement, int PictCnt)
        {
            XElement PictlEment = new XElement("Pict", new XAttribute("UID", PictCnt), new XAttribute("base64Data", PictStr));
            NewsItemXElement.Add(PictlEment);
        }

        private static void AddPictToElement(string PictStr, XElement NewsItemXElement)
        {
            string PictCnt = NewsItemXElement.Attribute("lastPictId").Value;
            int PictCntInt = Convert.ToInt16(PictCnt);
            XElement PictlEment = new XElement("Pict", new XAttribute("UID", PictCntInt++), new XAttribute("base64Data", PictStr));            
            NewsItemXElement.Add(PictlEment);
            NewsItemXElement.SetAttributeValue("lastPictId", PictCntInt);
        }

        private static void AddAttribute(XElement XMLElement, string AttributeName, string AttributeValue)
        {
            XAttribute attribute = new XAttribute(AttributeName, AttributeValue);
            XMLElement.Add(attribute);
        }

        public static bool IsNewRecord(NewsItem AdvertItem, XDocument CurrentSettingsXMLDoc)
        {
            string Phone = AdvertItem.Phone;
            if (string.IsNullOrEmpty(Phone)) return true;
            IEnumerable<XElement> AdvertLines = (from el in CurrentSettingsXMLDoc.Root.Elements("advert")
                                                 where (string)el.Attribute("phone") == Phone
                                                 select el);
            ICollection<XElement> AdvertLinesCollection = AdvertLines.ToList();
            return AdvertLinesCollection.Count == 0;
        }

        public static Dictionary<int, string> GetAllTegsCollection()
        {
            Dictionary<int, string> TagDict = new Dictionary<int, string>();
            XDocument settingsXMLDoc = SettingsXMLDoc;
            IEnumerable<XElement> TegsIEnum = (from el in settingsXMLDoc.Root.Element("TegList").Elements("Teg")
                                               select el);
            ICollection<XElement> TegsCollection = TegsIEnum.ToList();

            foreach (XElement TegElem in TegsCollection)
            {
                int TegUID = Convert.ToInt16(TegElem.Attribute("UID").Value);
                string TegName = TegElem.Attribute("Name").Value;
                TagDict.Add(TegUID, TegName);
            }

            return TagDict;
        }
        public static void SaveNewList(NewsItem newsItem)
        {
            XDocument CurrentDoc = SettingsXMLDoc;
            XElement CurrElement = CreateAdverItemXElement(newsItem);
            Guid Uid = newsItem.UID;
            if (Uid == Guid.Empty)
            {
                CurrElement = CreateAdverItemXElement(newsItem);
                CurrentDoc.Add(CurrElement);
            }
            else
            {                
                XElement UpdatedItem = (from el in CurrentDoc.Root.Elements("advert")
                                        where (string)el.Attribute("UID") == Uid.ToString()
                                    select el).Single<XElement>();
                if (UpdatedItem != null)
                {
                    UpdatedItem.ReplaceWith(CurrElement);
                }
                else
                {
                    CurrElement = CreateAdverItemXElement(newsItem);
                    CurrentDoc.Add(CurrElement);
                }
            }
            string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
            CurrentDoc.Save(DataFilePath);
            CurrentDoc = null;
        }
    }
}
