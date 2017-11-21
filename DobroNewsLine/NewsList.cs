using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Xml;

namespace DobroNewsLine
{
    class NewsList
    {
        public ListCollectionView NewsItems
        {
            get
            {
                {
                    IList<NewsItem> _news = new List<NewsItem> { };
                    XmlDocument NewsXml = new XmlDocument();
                    string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
                    NewsXml.Load(DataFilePath);
                    XmlNodeReader reader = new XmlNodeReader(NewsXml);
                    NewsItem NewsClass = new NewsItem();
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "advert")
                                {                                    
                                    var Current = reader.ReadSubtree();
                                    List<PictObj> PictList = new List<PictObj>();                                    
                                    while (Current.Read()) 
                                    {
                                        if (Current.NodeType == XmlNodeType.Element && Current.Name == "Pict")
                                        {                                           
                                            PictObj CurrPictObj = new PictObj { GUID = new Guid(Current.GetAttribute("GUID")),
                                                                                FilePath = Current.GetAttribute("FilePath")
                                            };
                                            PictList.Add(CurrPictObj);                                            
                                        }
                                    }
                                    string DefPictId = reader.GetAttribute("defPictId");
                                    if (DefPictId != null)
                                    {
                                        NewsClass.DefPictId = new Guid(DefPictId);
                                    }                                    
                                    NewsClass.Body = reader.GetAttribute("body");
                                    NewsClass.UID = new Guid(reader.GetAttribute("UID"));
                                    NewsClass.Title = reader.GetAttribute("title");
                                    NewsClass.Phone = reader.GetAttribute("phone");
                                    NewsClass.Link = new Uri(reader.GetAttribute("link"));
                                    NewsClass.Date = reader.GetAttribute("date");
                                    NewsClass.CityRegion = reader.GetAttribute("cityRegion");
                                    NewsClass.Age = reader.GetAttribute("age");
                                    NewsClass.Price = Convert.ToDecimal(reader.GetAttribute("price"));
                                    NewsClass.IsFavorite = Convert.ToBoolean(Convert.ToInt16(reader.GetAttribute("IsFavorite")));
                                    string UIds = reader.GetAttribute("TegCollection");
                                    NewsClass.PictList = PictList;
                                    NewsClass.PictCount = PictList.Count;                                    
                                    if (!string.IsNullOrEmpty(UIds))
                                    {
                                        NewsClass.Tegs = UIds.Split(':');
                                    }
/*                                    if (PictListCount > 0) delete later
                                    {
                                        NewsClass.DefPict = GetBitmapImageFromBase64(GetDefBase64DataFromXmlDocument(NewsXml, NewsClass));
                                    }*/
                                    if (PictList.Count > 0)
                                    {
                                        NewsClass.DefPict = GetBitmapImageFromFilePath(GetDefPictFilePathFromNewsItem(NewsClass));
                                    }
                                    _news.Add(NewsClass);
                                    NewsClass = new NewsItem();
                                    
                                }
                                break;
                            case XmlNodeType.Text:                                
                                break;                          
                            case XmlNodeType.EndElement:
                                if (reader.Name == "advert")
                                {
                                    _news.Add(NewsClass);
                                    NewsClass = new NewsItem();
                                }
                                break;
                        }

                    }                    
                    var NewsList = (ListCollectionView)CollectionViewSource.GetDefaultView(_news);
                    NewsXml = null;
                    return NewsList;
                }
            }
        }

        private BitmapImage GetBitmapImageFromFilePath(string FilePath)
        {
            /*byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(Base64Data);
            }
            catch
            {
                imageBytes = new byte[0];
            }
            MemoryStream strmImg = new MemoryStream(imageBytes);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strmImg;
            myBitmapImage.DecodePixelWidth = 200; //Величина картинки.
            myBitmapImage.EndInit();
            return myBitmapImage;*/
            Uri FileUri = new Uri(Utils.GetAppImgDir() + FilePath);
            BitmapImage myBitmapImage = new BitmapImage(FileUri);
            return myBitmapImage;
        }

        private string GetDefPictFilePathFromNewsItem(NewsItem NewsClass)
        {
            if (NewsClass.DefPictId == Guid.Empty)
            {
                return NewsClass.PictList[0].FilePath;
            }
            return NewsClass.PictList.Where(m => m.GUID == NewsClass.DefPictId).Single().FilePath;
        }

        /*private string GetDefBase64DataFromXmlDocument(XmlDocument NewsXml, NewsItem NewsClass) //delete later
        {
            Guid AdvertId = NewsClass.UID;
            int DefPictId = NewsClass.DefPictId;
            if (DefPictId == 0)
            {
                DefPictId = 1;
            }            
            XmlNode PictNode = NewsXml.SelectSingleNode(("//advert[@UID='" + AdvertId + "']/Pict[@UID='" + DefPictId + "']"));
            return PictNode.Attributes["base64Data"].Value;
        }*/
    }



    public class Category
    {
        public string Name { get; set; }

        public int Code { get; set; }
    }

    public class NewsItem
    {

        public Guid UID { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public Uri Link { get; set; }        
        public string Date { get; set; }
        public string CityRegion { get; set; }
        public string Age { get; set; }
        public List<PictObj> PictList { get; set; }
        public string[] Tegs { get; set; }
        public decimal Price { get; set; }
        public BitmapImage DefPict { get; set; }
        public Guid DefPictId { get; set; }
        public int PictCount { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class PictObj
    {
        public string Base64Data { get; set; } // delete later

        public int UID { get; set; } // delete later

        public Guid GUID { get; set; }

        public string FilePath { get; set; }
    }

    public class NewsTeg
    {
        public int UID {get; set;}
        public string Name {get; set;}
    }
}
