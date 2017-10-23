using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Net;
using System.Windows.Threading;
using System.Drawing;

namespace DobroNewsLine
{    
    static public class Utils
    {

        public static string ConvertImage(string ImgUrl)
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

        static public void ShowWarningDialog(string Message)
        {
            string messageBoxText = Message;
            string caption = "Warning";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        static public void ShowErrorDialog(string Message)
        {
            string messageBoxText = Message;
            string caption = "Error";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        static public string RenameFile(string FilePath, string NewName)
        {
            if (!Utils.IsFileExists(FilePath))
            {
                return FilePath;
            }
            string NewFilePath = GhangeFileNameInPath(FilePath, NewName);
            MoveFile(FilePath, NewFilePath);
            return NewFilePath;
        }

        static public bool ShowSimpleYesNoDialog(string Message)
        {
            string messageBoxText = Message;
            string caption = "Question";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            return (MessageBox.Show(messageBoxText, caption, button, icon) == MessageBoxResult.Yes);
        }

        static public string GhangeFileNameInPath(string FilePath, string NewName)
        {
            return Path.GetDirectoryName(FilePath) + @"\" + NewName + Path.GetExtension(FilePath);
        }

        static public BitmapImage DecodePhoto(byte[] value)
        {
            if (value == null) return null;
            byte[] byteme = value as byte[];
            if (byteme == null) return null;
            MemoryStream strmImg = new MemoryStream(byteme);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strmImg;
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            return myBitmapImage;
        }

        static public bool IsFileExists(string FilePath)
        {
            if (FilePath == null)
            {
                return false;
            }
            return File.Exists(FilePath);
        }

        static public void CreateDirectory(string DirName)
        {
            if (!Directory.Exists(DirName))
            {
                Directory.CreateDirectory(DirName);
            }
        }

        //static public BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap BitmapSource)
        //{
        //    var memoryStream = new MemoryStream();
        //    BitmapSource.Save(memoryStream, Imaging.ImageFormat.Png);
        //    memoryStream.Position = 0;

        //    var bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.StreamSource = memoryStream;
        //    bitmapImage.EndInit();

        //    return bitmapImage;
        //}

        static public void DeleteFile(string FilePath)
        {
            if (!IsFileExists(FilePath)) return;
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = string.Format("/c del \"{0}\"", FilePath);
            try
            {
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(string.Format("File {0} was not delleted because {1}", FilePath, ex.Message));
            }
        }

        static public void MoveFile(string FromFilePath, string ToFilePath)
        {
            if (!IsFileExists(FromFilePath) || ToFilePath == string.Empty) return;
            try
            {
                File.Move(FromFilePath, ToFilePath);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(string.Format("File {0} was not moved because {1}", FromFilePath, ex.Message));
            }
        }

        static public bool IsStringUriValid(string Adsress)
        {
            return Uri.IsWellFormedUriString(Adsress, UriKind.Absolute);
        }

        static public string GenerateTempFilePath(string FilePath)
        {
            return FilePath.Substring(0, FilePath.Length - 11);
        }

        static public string GenerateCryptFilePath(string FilePath)
        {
            return string.Concat(System.IO.Path.GetDirectoryName(FilePath), @"\", System.IO.Path.GetFileName(FilePath), "CrypDobFilm");
        }       

        public static Image GetImageByBase64Str(string Base64String)
        {
            if (Base64String == string.Empty)
            {
                return null;
            }
            Image img = new Image();
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(Base64String);
            }
            catch
            {
                imageBytes = new byte[0];
            }
            MemoryStream strmImg = new MemoryStream(imageBytes);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strmImg;
            myBitmapImage.DecodePixelWidth = 120; //Величина картинки.
            myBitmapImage.EndInit();
            img.Source = myBitmapImage;
            return img;

        }               
    }    
}
