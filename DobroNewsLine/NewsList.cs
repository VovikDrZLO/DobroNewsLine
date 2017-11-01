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
                                            PictObj CurrPictObj = new PictObj { Base64Data = Current.GetAttribute("base64Data"), 
                                                                                UID = Convert.ToInt16(Current.GetAttribute("UID")) };
                                            PictList.Add(CurrPictObj);
                                        }
                                    }
                                    NewsClass.DefPictId = Convert.ToInt16(reader.GetAttribute("defPictId"));
                                    NewsClass.Body = reader.GetAttribute("body");
                                    NewsClass.UID = new Guid(reader.GetAttribute("UID"));
                                    NewsClass.Title = reader.GetAttribute("title");
                                    NewsClass.Phone = reader.GetAttribute("phone");
                                    NewsClass.Link = new Uri(reader.GetAttribute("link"));
                                    NewsClass.Date = reader.GetAttribute("date");
                                    NewsClass.CityRegion = reader.GetAttribute("cityRegion");
                                    NewsClass.Age = reader.GetAttribute("age");
                                    NewsClass.Price = Convert.ToDecimal(reader.GetAttribute("price"));
                                    string UIds = reader.GetAttribute("TegCollection");
                                    NewsClass.PictList = PictList;
                                    if (!string.IsNullOrEmpty(UIds))
                                    {
                                        NewsClass.Tegs = UIds.Split(':');
                                    }
                                    if (PictList.Count > 0)
                                    {                                        
                                        NewsClass.DefPict = GetBitmapImageFromBase64(GetDefBase64DataFromNewsItem(NewsClass));
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
                    return NewsList;
                }
            }
        }

        private BitmapImage GetBitmapImageFromBase64(string Base64Data)
        {
            byte[] imageBytes;
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
            return myBitmapImage;
        }

        private string GetDefBase64DataFromNewsItem(NewsItem NewsClass)
        {
            if (NewsClass.DefPictId == 0)
            {
                NewsClass.DefPictId = 1;
            }
            return NewsClass.PictList.Where(m => m.UID == NewsClass.DefPictId).Single().Base64Data;
        }
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
        public int DefPictId { get; set; }
    }

    public class PictObj
    {
        public string Base64Data { get; set; }

        public int UID { get; set; }
    }

    public class NewsTeg
    {
        public int UID {get; set;}
        public string Name {get; set;}
    }
}
