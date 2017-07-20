﻿using System;
using System.Collections;
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

namespace DobroNewsLine
{
    /// <summary>
    /// Interaction logic for TegManager.xaml
    /// </summary>
    public partial class TegManager : Window
    {
        public TegManager()
        {
            InitializeComponent();
            TegCollection = new Dictionary<int, string>();
            CurrTegCollection = new Dictionary<int, string>();
            TegCollection = XMLUtils.GetTegsCollection();
            AllTegsLB.ItemsSource = TegCollection;
            CurrTegsLB.ItemsSource = CurrTegCollection;
        }

        public Dictionary<int, string> TegCollection { get; set; }

        public Dictionary<int, string> CurrTegCollection { get; set; }

        private void AddTegButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllTegsLB.SelectedItems.Count == 0)
            {
                return;
            }
            KeyValuePair<int, string> SelTeg = (KeyValuePair<int, string>)AllTegsLB.SelectedItem;
            //Dictionary<int, string> SelTegDict = new Dictionary<int, string> { { SelTeg.Key, SelTeg.Value } };

            //ListBoxItem itm = new ListBoxItem();
            //itm.Content = SelTeg.Value;
            //CurrTegsLB.Items.Add(itm);

            TegCollection.Remove(SelTeg.Key);
            //AllTegsLB.ItemsSource = TegCollection;
            AllTegsLB.Items.Refresh();
            CurrTegCollection.Add(SelTeg.Key, SelTeg.Value);
            CurrTegsLB.ItemsSource = CurrTegCollection;
            CurrTegsLB.Items.Refresh();
        }

        private void RemoveTegButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrTegsLB.SelectedItems.Count == 0)
            {
                return;
            }
            KeyValuePair<int, string> SelTeg = (KeyValuePair<int, string>)CurrTegsLB.SelectedItem;
            //Dictionary<int, string> SelTegDict = new Dictionary<int, string> { { SelTeg.Key, SelTeg.Value } };

            //ListBoxItem itm = new ListBoxItem();
            //itm.Content = SelTeg.Value;
            //CurrTegsLB.Items.Add(itm);

            CurrTegCollection.Remove(SelTeg.Key);
            //AllTegsLB.ItemsSource = TegCollection;
            CurrTegsLB.Items.Refresh();
            TegCollection.Add(SelTeg.Key, SelTeg.Value);
            AllTegsLB.ItemsSource = TegCollection;
            AllTegsLB.Items.Refresh();
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }

        }

        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                    if (element == source)
                    {
                        return null;
                    }
                }
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }
            return null;
        }

        ListBox dragSource = null;

        private void RemoveTeg_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            if (parent.Equals(dragSource))
            {
                return;
            }
            KeyValuePair<int, string> data = (KeyValuePair<int, string>)e.Data.GetData(typeof(KeyValuePair<int, string>));            
            CurrTegCollection.Remove(data.Key);
            TegCollection.Add(data.Key, data.Value);
            AllTegsLB.Items.Refresh();
            CurrTegsLB.Items.Refresh();            
        }

        private void AddTeg_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            if (parent.Equals(dragSource))
            {
                return;
            }            
            KeyValuePair<int, string> data = (KeyValuePair<int, string>)e.Data.GetData(typeof(KeyValuePair<int, string>));            
            TegCollection.Remove(data.Key);
            CurrTegCollection.Add(data.Key, data.Value);
            AllTegsLB.Items.Refresh();
            CurrTegsLB.Items.Refresh();
        }
    }
}
