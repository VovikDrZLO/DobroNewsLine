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
    /// 

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
            TitleTextBox.DataContext = newsItem;
            PhoneTextControl.DataContext = newsItem;
            CityRegionTextBlock.DataContext = newsItem;
            PriceTextBlock.DataContext = newsItem;
            BodyTextBlock.DataContext = newsItem;
            FavorCB.DataContext = newsItem;
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
            byte[] binaryData = Convert.FromBase64String(CurrentNewsItem.PictList[Index].Base64Data);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        private void DeleteImageByIndex(int Index)
        {
            CurrentNewsItem.PictList.RemoveAt(Index);
            PictsMaxCount--;
        }

        private void PrevButtom_Click(object sender, RoutedEventArgs e)
        {
            ShowPrevImage();
        }

        private void ShowPrevImage()
        {
            PictsCurrentindex--;
            BitmapImage bi = GetBIByIndex(PictsCurrentindex);
            AdvImage.Source = bi;
            SetPrevNextButtonsVisibility();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNextImage();
        }

        private void ShowNextImage()
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
            DefPictCB.IsChecked = CurrentNewsItem.PictList[PictsCurrentindex].UID == CurrentNewsItem.DefPictId;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            XMLUtils.SaveNewList(CurrentNewsItem);
            this.Close();
        }

        private void DelImgBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteImageByIndex(PictsCurrentindex);
            if (PictsCurrentindex > 0)
            {
                PictsCurrentindex++;
                ShowPrevImage();                
            }
            else if (PictsCurrentindex == 0 && PictsMaxCount > 0)
            {
                PictsCurrentindex--;
                ShowNextImage();
            }
            else
            {
                AdvImage.Source = null;
            }
            SetPrevNextButtonsVisibility();
        }
        private void DefPictCB_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox CBControl = sender as CheckBox;
            bool IsChecked = Convert.ToBoolean(CBControl.IsChecked);
            if (IsChecked)
            {
                PictObj CurrPictObj = CurrentNewsItem.PictList[PictsCurrentindex];
                CurrentNewsItem.DefPictId = CurrPictObj.UID;
            }
            
            
        }

    }
}
