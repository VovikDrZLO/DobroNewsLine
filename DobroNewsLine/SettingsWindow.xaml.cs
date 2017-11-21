﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net;
using AngleSharp.Parser.Html;
using System.Xml.Linq;
using AngleSharp.Dom.Html;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace DobroNewsLine
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            GetCategoryCollection();
            CategoryListCB.ItemsSource = CategorisDict;
        }

        public Dictionary<int, string> CategorisDict { get; set; }
        
        private void Get_DataButton_Click(object sender, RoutedEventArgs e)
        {
            string SubsectionId = CategoryListCB.SelectedValue.ToString(); //CategoryIdTextBox.Text;
            string PagesCount = PageCountTextBox.Text;
            string CityName = CityTextBox.Text;
            if (string.IsNullOrEmpty(CityName))
            {
                CityName = "kiev";
            }
            //Task.Run(() => ParthingSite(SubsectionId, PagesCount, CityName));            
            ParthingSite(SubsectionId, PagesCount, CityName);            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GetCategoryCollection()
        {
            CategorisDict = new Dictionary<int, string>();
            string WebSiteName = DobroNewsLine.Properties.Settings.Default.SitePath;
            Uri uri = new Uri(WebSiteName);
            string html = new WebClient().DownloadString(uri);
            var parser = new HtmlParser();
            IHtmlDocument document = parser.Parse(html);
            var mainСontentList = document.All.Where(m => m.LocalName == "a" && m.ClassList.Contains("subsection_link"));
            List<Category> CategoryList = new List<Category>();
            foreach (var Link in mainСontentList)
            {
                string FullLink = Link.Attributes[0].Value;
                if (FullLink.Contains(WebSiteName + "/view_subsection.php?id_subsection="))
                {
                    //Category CategoryItem = new Category();
                    //CategoryItem.Name = Link.InnerHtml;
                    int CategoryId;
                    if (int.TryParse(FullLink.Replace(WebSiteName + "/view_subsection.php?id_subsection=", ""), out CategoryId))
                    {
                        CategorisDict.Add(CategoryId, Link.InnerHtml);
                    }
                    //CategoryItem.Code = Convert.ToInt16(FullLink.Replace(WebSiteName + "/view_subsection.php?id_subsection=", ""));
                    //CategoryList.Add(CategoryItem);                    
                }
            }
        }

        //public async Task ParthingSite(string SubsectionId, string PagesCount, string CityName)
        public void ParthingSite(string SubsectionId, string PagesCount, string CityName)
        {
            int Counter = 0;
            int TotalCount = Convert.ToInt16(PagesCount) * 50;
            string DomainName = DobroNewsLine.Properties.Settings.Default.DomainName;            
            //UpdateWindow("Import Starts", StatusType.Starting);
            List<NewsItem> NewsItemList = new List<NewsItem>();
            for (int cnt = 1; cnt <= Convert.ToInt16(PagesCount); cnt++)
            {                
                Uri uri = new Uri("http://" + CityName + "." + DomainName + ".com/view_subsection.php?id_subsection=" + SubsectionId + "&search=&page=" + cnt);
                string html = new WebClient().DownloadString(uri);
                var parser = new HtmlParser();
                var document = parser.Parse(html);
                var mainСontentList = document.All.Where(m => m.LocalName == "div" && m.ClassList.Contains("main-content")).Single();
                var subDocument = parser.Parse(mainСontentList.InnerHtml);
                var TablsList = subDocument.All.Where(m => m.LocalName == "table");                
                foreach (IHtmlTableElement tab in TablsList)
                {                    
                    NewsItem newsItem = new NewsItem();
                    string Title = tab.Rows[0].Cells[0].InnerHtml;
                    var InnerDoc = parser.Parse(Title);                   
                    newsItem.Link = new Uri(InnerDoc.QuerySelector("a").Attributes[0].Value);
                    newsItem.Title = InnerDoc.QuerySelector("img").Attributes[0].Value;                    
                    AddPageData(newsItem);
                    NewsItemList.Add(newsItem);
                    Counter++;
                    //UpdateWindow("In progress " + Counter + "/" + TotalCount, StatusType.Importing);
                }                
            }
            XMLUtils.SaveAdvertData(NewsItemList);
            //UpdateWindow("Import Complite!!!", StatusType.Finish);
        }        

        public void AddPageData(NewsItem newsItem)
        {
            string SitePathCom = DobroNewsLine.Properties.Settings.Default.SitePathCom;   

            Uri uri = newsItem.Link;
            string html;
            try
            {
                html = new WebClient().DownloadString(uri);
            }
            catch (WebException e)
            {
                return;
            }

            var parser = new HtmlParser();
            var document = parser.Parse(html);
            string slyleF = "width: 90%; margin-top: 19px; margin-left: auto; margin-right: auto; padding-top: 10px; text-align: left;";
            var TablsList = document.All.Where(m => m.LocalName == "table" && m.GetAttribute("style") == slyleF);
            foreach (IHtmlTableElement tab in TablsList)
            {
                var PictHtml = tab.QuerySelectorAll("table[style='margin-top: 10px; border: 1px dotted #bbbbbb;']");
                if (PictHtml.Length > 0)
                {
                    var imgList = PictHtml[0].QuerySelectorAll("img[style='max-width: 120px; max-height: 120px; padding-bottom: 5px; margin-left: 5px;']");
                    for (int cnt = 0; cnt < imgList.Length; cnt++)
                    {
                        string ImgLinkRight = imgList[cnt].GetAttribute("src");
                        var ImgLink = SitePathCom + ImgLinkRight.Substring(1);
                        PictObj PictObjData = Utils.SaveImage(ImgLink);
                        if (!PictObjData.GUID.Equals(Guid.Empty))
                        {
                            if (newsItem.PictList == null)
                            {
                                newsItem.PictList = new List<PictObj>();
                            }
                            newsItem.PictList.Add(PictObjData);
                        }
                        /*string ImgBase64String = Utils.ConvertImage(ImgLink);
                        if (ImgBase64String.Length > 0)
                        {
                            if (newsItem.PictList == null)
                            {
                                newsItem.PictList = new List<PictObj>();
                            }
                            PictObj CurrPictObj = new PictObj { Base64Data = ImgBase64String, UID = cnt };
                            newsItem.PictList.Add(CurrPictObj);
                        }*/
                    }
                }
                ////////////////
                var Header = tab.QuerySelectorAll("h1[style='display: inline; font-size: 20px; font-weight: normal;']");
                var Params = tab.QuerySelectorAll("div[style='color: #242424; font-size: 12px; margin-top: 5px;']");
                var BodyHtml = tab.QuerySelectorAll("div[style='margin-top: 15px; text-align: left; width: 100%; color: #2a2a2a; font-size: 14px;']");
                if (Header.Length > 0)
                {
                    string Caption = Header[0].InnerHtml;
                }
                if (Params.Length > 0)
                {
                    string ParamsHtml = Params[0].InnerHtml;
                    ParamsHtml = ParamsHtml.Replace("<b>", "");
                    ParamsHtml = ParamsHtml.Replace("</b>", "");
                    ParamsHtml = ParamsHtml.Replace("<br>", "");
                    string[] separators = { "\n" };
                    string[] ParamsStrArray = ParamsHtml.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < ParamsStrArray.Length; i++)
                    {
                        string ParamString = ParamsStrArray[i];
                        if (ParamString.IndexOf("Дата подачи объявления:") >= 0)
                        {
                            string NewsDate = ParamString.Substring(ParamString.IndexOf(":") + 2);
                            //AddAttribute(NewsXElement, "date", NewsDate);
                            newsItem.Date = NewsDate;
                        }
                        else if (ParamString.IndexOf("Город:") >= 0)
                        {
                            string NewsRegion = ParamString.Substring(ParamString.IndexOf(":") + 2);
                            //AddAttribute(NewsXElement, "cityRegion", NewsRegion);
                            newsItem.CityRegion = NewsRegion;
                        }
                        else if (ParamString.IndexOf("Возраст:") >= 0)
                        {
                            string NewsAge = ParamString.Substring(ParamString.IndexOf(":") + 2);
                            //AddAttribute(NewsXElement, "age", NewsAge);
                            newsItem.Age = NewsAge;
                        }
                    }
                }
                if (BodyHtml.Length > 0)
                {
                    var ContactData = BodyHtml[0].QuerySelectorAll("div[class='post-contacts']");
                    if (ContactData.Length > 0)
                    {
                        string ContactDataStr = ContactData[0].InnerHtml;
                        if (ContactDataStr.IndexOf("Контактные телефоны:") >= 0)
                        {
                            string Phone = ContactDataStr.Substring(ContactDataStr.IndexOf(":") + 2);//<span>797810842</span>
                            Phone = Phone.Replace("<span>", "");
                            Phone = Phone.Replace("</span>", "");
                            newsItem.Phone = Phone;
                            //AddAttribute(NewsXElement, "phone", Phone);
                        }
                    }
                    string Body = BodyHtml[0].InnerHtml;
                    Body = Body.Replace("<br>", "");
                    if (Body.IndexOf("<") > 0)
                    {
                        Body = Body.Substring(0, Body.IndexOf("<"));
                    }
                    Body = Body.Trim();
                    //AddAttribute(NewsXElement, "body", Body);
                    newsItem.Body = Body; 
                }
            }
        }
        

        public XElement CreateNewsXElement(string title, string link)
        {
            XElement FilmElement = new XElement("advert",
                new XAttribute("title", title),
                new XAttribute("link", link)
            );
            return FilmElement;
        }

        public void AddAttribute(XElement XMLElement, string AttributeName, string AttributeValue)
        {
            XAttribute attribute = new XAttribute(AttributeName, AttributeValue);
            XMLElement.Add(attribute);
        }

        public string ConvertImage(string ImgUrl)
        {
            try
            {
                byte[] ImgBodyByteArray = new WebClient().DownloadData(ImgUrl);
                string ImgBase64 = Convert.ToBase64String(ImgBodyByteArray);
                return ImgBase64;
            }
            catch
            {
                return "";
            }
        }
        public bool ValidateXML(string XMLDoc) //Valid
        {
            XmlDocument ValidDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, "DobroNewsLine.xsd");
            settings.ValidationType = ValidationType.Schema;
            XmlReader TryReader = XmlReader.Create(XMLDoc, settings);
            try
            {
                ValidDoc.Load(TryReader);
            }
            catch (XmlSchemaValidationException e)
            {
                string Message = String.Format("Validating error; {1}, try enother file", e.Message);
                Utils.ShowErrorDialog(Message);
                return false;
            }
            finally
            {
                TryReader.Close();
                ValidDoc = null;
            }
            return true;
        }

        public IList<NewsItem> GetNewsItemFromXML()
        {
            XDocument myXDocument = new XDocument();
            string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
            myXDocument = XDocument.Load(DataFilePath); 
            //IEnumerable<XElement> NewsList = myXDocument.Root.XPathSelectElements("//prefix:DobroNewsLine/prefix:advert");            
            var NewsList = myXDocument.XPathSelectElements("//DobroNewsLine/advert");
            IList<NewsItem> NewsItemsList = new List<NewsItem>();
            foreach (XElement news in NewsList)
            {
                NewsItem NewsClass = ConvertXElementToNewsItem(news);
                NewsItemsList.Add(NewsClass);
            }
            myXDocument = null;
            return NewsItemsList;
        }

        public NewsItem ConvertXElementToNewsItem(XElement news) //valid
        {
            NewsItem NewsClass = new NewsItem();
            NewsClass.Body = (string)news.Attribute("body");
            NewsClass.Title = (string)news.Attribute("title");
            NewsClass.Phone = (string)news.Attribute("phone");
            NewsClass.Link = new Uri((string)news.Attribute("link"));
            NewsClass.Date = (string)news.Attribute("date");
            NewsClass.CityRegion = (string)news.Attribute("cityRegion");
            NewsClass.Age = (string)news.Attribute("age");
            return NewsClass;
        }

        public XDocument SettingsXMLDoc //valid
        {
            get
            {
                string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
                XDocument XMLDoc = XDocument.Load(DataFilePath);
                return XMLDoc;
            }
        }

        void UpdateWindow(string Text, StatusType TextStatusType)
        {
            Dispatcher.Invoke(() =>
            {
                StatusLB.Content = Text;
                switch (TextStatusType)
                {
                    case  StatusType.Starting:
                        StatusLB.Foreground = Brushes.Red;
                        break;
                    case StatusType.Importing:
                        StatusLB.Foreground = Brushes.YellowGreen;
                        break;
                    case StatusType.Finish:
                        StatusLB.Foreground = Brushes.Green;
                        break;
                }                
            });
        }
    }
}
