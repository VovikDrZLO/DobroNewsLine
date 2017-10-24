using System;
using System.Collections.Generic;
using System.IO;
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

namespace DobroNewsLine
{
    /// <summary>
    /// Логика взаимодействия для NewsLineItemWindow.xaml
    /// </summary>
    public partial class NewsLineItemWindow : Window
    {
        private int PictsMaxCount = 0;
        private int PictsCurrentindex = -1;
        public NewsItem CurrentNewsItem {get; set;}   
        public NewsLineItemWindow()
        {
            InitializeComponent();
        }
        public NewsLineItemWindow(NewsItem newsItem)
        {
            InitializeComponent();
            //CurrTitle = newsItem.Title;
            //TitleTextBox.Text = newsItem.Title;  
             //BodyTextBlock.Text = newsItem.Body;
            //PhoneTextControl.Text = newsItem.Phone;
            //CityRegionTextBlock.Text = newsItem.CityRegion;

            //TitleTextBox.DataContext = newsItem;
            //PhoneTextControl.DataContext = newsItem;
            //CityRegionTextBlock.DataContext = newsItem;
            //PriceTextBlock.DataContext = newsItem;
            //BodyTextBlock.DataContext = newsItem;
           
            PictsMaxCount = newsItem.PictList.Count;
            CurrentNewsItem = newsItem;
            if (newsItem.PictList.Count > 0)
            {
                PictsCurrentindex = 0;
                SetPrevNextButtonsVisibility();
                BitmapImage bi = GetBIByIndex(PictsCurrentindex);
                AdvImage.Source = bi;
            }
        }

        private BitmapImage GetBIByIndex(int Index )
        {
            byte[] binaryData = Convert.FromBase64String(CurrentNewsItem.PictList[Index]);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        private void PrevButtom_Click(object sender, RoutedEventArgs e)
        {
            PictsCurrentindex--;
            BitmapImage bi = GetBIByIndex(PictsCurrentindex);
            AdvImage.Source = bi;
            SetPrevNextButtonsVisibility();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PictsCurrentindex++;
            BitmapImage bi = GetBIByIndex(PictsCurrentindex);
            AdvImage.Source = bi;
            SetPrevNextButtonsVisibility();
        }

        private void SetPrevNextButtonsVisibility()
        {
            if (PictsMaxCount > PictsCurrentindex + 1 && PictsMaxCount > 0)
            {
                NextButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                NextButton.Visibility = System.Windows.Visibility.Hidden;
            }
            if (PictsCurrentindex > 0)
            {
                PrevButtom.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PrevButtom.Visibility = System.Windows.Visibility.Hidden;
            }

        }

    }
}
