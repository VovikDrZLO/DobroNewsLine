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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AngleSharp.Html;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System.Net;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml;
using System.Collections.ObjectModel;

namespace DobroNewsLine
{
    //TODO
    //1. Import photo merge +
    //2. Photo delete
    //3. Notes + fields edit
    //4. NewsItems delete
    //5. Teg filtration
    //6. Import whith filtration
    //7. Import Counter
    //8. Main Pict in grid
    //9. Get CategotyId List
    //10. Reload main grid
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Title");
            list.Add("Phone");
            list.Add("City");
            list.Add("Body");
            SearchColumnComboBox.ItemsSource = list;
            MainGridData.DataContext = new NewsList();           
        }                 


        public bool ValidateXML(string XMLDoc) //Valid
        {
            XmlDocument ValidDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, "DobrofilmData.xsd");
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
            XDocument myXDocument = SettingsXMLDoc;
            IEnumerable<XElement> NewsList = myXDocument.XPathSelectElements("//prefix:DobroNewsLine/prefix:advert");
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
            NewsClass.Link = new Uri ((string)news.Attribute("link"));
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


        public ListCollectionView NewsItems
        {
            get
            {
                IList<NewsItem> _news = GetNewsItemFromXML();
                var NewsItemsList = (ListCollectionView)CollectionViewSource.GetDefaultView(_news);
                return NewsItemsList;
            }
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchColumnComboBox.SelectedItem == null) return;
            NewsList NewsListnewsListList = new NewsList();
            SelectedFilterColums = SearchColumnComboBox.SelectedItem.ToString();
            SelectedFilterValue = FilterValueTextBox.Text;            
            var FilteredSource = GetFilmListByCategory();
            MainGridData.ItemsSource = FilteredSource;            
        }

        public ListCollectionView GetFilmListByCategory()
        {
            NewsList newsList = new NewsList();
            ListCollectionView FilteredFileList = newsList.NewsItems;
            FilteredFileList.Filter = new Predicate<object>(Contains);
            return FilteredFileList;
        }

        public string SelectedFilterColums { get; set; }
        public string SelectedFilterValue { get; set; }

        public bool Contains(object de)
        {
            NewsItem order = de as NewsItem;
            if (SelectedFilterValue == String.Empty)
            {
                return true;
            }
            if (SelectedFilterColums == "Title")
            {
                return (order.Title.IndexOf(SelectedFilterValue, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (SelectedFilterColums == "Phone")
            {
                return order.Phone == SelectedFilterValue;
            }
            else if (SelectedFilterColums == "City")
            {
                return (order.CityRegion.IndexOf(SelectedFilterValue, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (SelectedFilterColums == "Body")
            {
                return (order.Body.IndexOf(SelectedFilterValue, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            return false;
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterValueTextBox.Text = "";
        }

        private void OpenImportWnd_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

        }              

        private void MyClick(object sender, RoutedEventArgs e)
        {
            string link = (((TextBlock)sender).DataContext as NewsItem).Link.OriginalString;
            System.Diagnostics.Process.Start(link);
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {

            XDocument AdvXML = XMLUtils.SettingsXMLDoc;
            AdvXML.Root.RemoveAll();
            string DataFilePath = DobroNewsLine.Properties.Resources.DataFilePath;
            AdvXML.Save(DataFilePath);

        }

        private void MainGridData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            NewsItem newsItem = (NewsItem)MainGridData.SelectedItem;
            NewsLineItemWindow newsLineItemWindow = new NewsLineItemWindow(newsItem);
            newsLineItemWindow.ShowDialog();

        }

        private void TegManageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGridData.SelectedItem != null)
            {
                NewsItem Selected = MainGridData.SelectedItem as NewsItem;
                TegManager tegManagerWindow = new TegManager(Selected);
                tegManagerWindow.ShowDialog();
            }           

        }
    }
}
