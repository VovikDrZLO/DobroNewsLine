using System;
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
        }

        private void Get_DataButton_Click(object sender, RoutedEventArgs e)
        {

            ParthingSite("44", "1");
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void ParthingSite(string SubsectionId, string PagesCount)
        {
            for (int cnt = 1; cnt <= Convert.ToInt16(PagesCount); cnt++)
            {
                Uri uri = new Uri("http://ukrgo.ua/view_subsection.php?id_subsection=" + SubsectionId + "&search=&page=" + cnt);
                string html = new WebClient().DownloadString(uri);
                var parser = new HtmlParser();
                var document = parser.Parse(html);
                var mainСontentList = document.All.Where(m => m.LocalName == "div" && m.ClassList.Contains("main-content")).Single();
                var subDocument = parser.Parse(mainСontentList.InnerHtml);
                var TablsList = subDocument.All.Where(m => m.LocalName == "table");
                XDocument NewsLineX = XMLUtils.SettingsXMLDoc; //XDocument.Load(@"C:\Users\cons_inspiron\Documents\Visual Studio 2012\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml");
                foreach (IHtmlTableElement tab in TablsList)
                {
                    NewsItem newsItem = new NewsItem();
                    string Title = tab.Rows[0].Cells[0].InnerHtml;
                    var InnerDoc = parser.Parse(Title);
                    //string link = InnerDoc.QuerySelector("a").Attributes[0].Value;
                    //string title = InnerDoc.QuerySelector("img").Attributes[0].Value;
                    newsItem.Link = new Uri(InnerDoc.QuerySelector("a").Attributes[0].Value);
                    newsItem.Title = InnerDoc.QuerySelector("img").Attributes[0].Value;
                    //XElement FilmXElement = CreateNewsXElement(title, link);
                    AddPageData(newsItem);
                    XMLUtils.SaveAdvertData(newsItem);
                }
                
            }
        }        

        public void AddPageData(NewsItem newsItem)
        {            
            Uri uri = newsItem.Link;
            string html = new WebClient().DownloadString(uri);
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
                        var ImgLink = "http://ukrgo.com" + ImgLinkRight.Substring(1);
                        string ImgBase64String = ConvertImage(ImgLink);
                        if (ImgBase64String.Length > 0)
                        {
                            if (newsItem.Picts == null)
                            {
                                newsItem.Picts = new List<string>();
                            }
                            newsItem.Picts.Add(ImgBase64String);
                            //XElement ImgElement = new XElement("Pict", new XAttribute("base64Data", ImgBase64String));                            
                            //NewsXElement.Add(ImgElement);
                        }
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
            myXDocument = XDocument.Load(@"C:\Users\cons_inspiron\Documents\Visual Studio 2012\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml"); 
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
                XDocument XMLDoc = XDocument.Load(@"C:\Users\cons_inspiron\Documents\Visual Studio 2012\Projects\DobroNewsLine\DobroNewsLine\DobroNewsLine.xml");
                return XMLDoc;
            }
        }

    }
}
