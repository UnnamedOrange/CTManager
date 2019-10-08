using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace CTManager
{
    public sealed partial class OptionDialog : ContentDialog
    {
        public OptionDialog()
        {
            this.InitializeComponent();
        }
        private Handler.Options option;
        public bool isOk;
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Tag = option;
            isOk = true;
        }
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Tag = option;
            isOk = false;
        }
        private void CheckBox1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isSingleLine = (bool)CheckBox1.IsChecked;
        }
        private void CheckBox2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isRN = (bool)CheckBox2.IsChecked;
        }
        private void CheckBox3_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isNIgnored = (bool)CheckBox3.IsChecked;
        }
        private void CheckBox4_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isBackslashIgnored = (bool)CheckBox4.IsChecked;
        }
        private void CheckBox5_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isFormatIgnored = (bool)CheckBox5.IsChecked;
        }
        private void CheckBox6_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option.isQuoteIgnored = (bool)CheckBox6.IsChecked;
        }
        private void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            option = (Handler.Options)Tag;
            CheckBox1.IsChecked = option.isSingleLine;
            CheckBox2.IsChecked = option.isRN;
            CheckBox3.IsChecked = option.isNIgnored;
            CheckBox4.IsChecked = option.isBackslashIgnored;
            CheckBox5.IsChecked = option.isFormatIgnored;
            CheckBox6.IsChecked = option.isQuoteIgnored;
        }
    }
}
