using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Popups;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace CTManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Menu_SwitchExecute_Click(object sender, RoutedEventArgs e)
        {
            Menu_SwitchExecute.IsEnabled = false;
            TextBox1.IsEnabled = false;
            ProgressRing1.IsActive = true;

            String t = TextBox1.Text;
            Task task = Task.Run(() => { t = new Handler().Switch(t); });
            await task;
            TextBox1.Text = t;

            Menu_SwitchExecute.IsEnabled = true;
            TextBox1.IsEnabled = true;
            ProgressRing1.IsActive = false;
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatisticChars();
        }

        private void TextBlock1_Loaded(object sender, RoutedEventArgs e)
        {
            StatisticChars();
        }

        private void StatisticChars()
        {
            TextBlock1.Text = TextBox1.Text.Length.ToString() + " 字符";
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private async void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AboutDialog();
            await dlg.ShowAsync();
        }
    }
}
