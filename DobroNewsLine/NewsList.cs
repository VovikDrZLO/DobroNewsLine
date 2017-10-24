﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
                    NewsXml.Load(@"C:\Users\cons_inspiron\Documents\Visual Studio 2012\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml");
                    XmlNodeReader reader = new XmlNodeReader(NewsXml);
                    NewsItem NewsClass = new NewsItem();
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "advert")
                                {
                                    //try
                                    //{
                                    //    FilmFileClass.ID = new Guid(reader.GetAttribute("GUID"));
                                    //}
                                    //catch
                                    //{
                                    //    FilmFileClass.ID = Guid.NewGuid();
                                    //}
                                    
                                    var Current = reader.ReadSubtree();
                                    List<string> PictList = new List<string>();

                                    while (Current.Read()) 
                                    {
                                        switch (Current.NodeType)
                                        {
                                            case XmlNodeType.Element:
                                                if (Current.Name == "Pict")
                                                {
                                                    PictList.Add(Current.GetAttribute("base64Data"));
                                                }
                                                break;
                                        }
                                    }
                                    NewsClass.Body = reader.GetAttribute("body");
                                    NewsClass.UID = new Guid(reader.GetAttribute("UID"));
                                    NewsClass.Title = reader.GetAttribute("title");
                                    NewsClass.Phone = reader.GetAttribute("phone");
                                    NewsClass.Link = new Uri(reader.GetAttribute("link"));
                                    NewsClass.Date = reader.GetAttribute("date");
                                    NewsClass.CityRegion = reader.GetAttribute("cityRegion");
                                    NewsClass.Age = reader.GetAttribute("age");
                                    string UIds = reader.GetAttribute("TegCollection");

                                    NewsClass.PictList = PictList;
                                    if (!string.IsNullOrEmpty(UIds))
                                    {
                                        NewsClass.Tegs = UIds.Split(':');
                                    }
                                    if (reader.IsEmptyElement)
                                    {
                                        _news.Add(NewsClass);
                                        NewsClass = new NewsItem();
                                    }
                                }
                                break;
                            case XmlNodeType.Text:                                
                                break;
                            /*case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                            case XmlNodeType.Comment:*/
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
        public List<string> Picts { get; set; }
        public string[] Tegs { get; set; }
        public decimal Price { get; set; }
    }

    public class NewsTeg
    {
        public int UID {get; set;}
        public string Name {get; set;}
    }
}
