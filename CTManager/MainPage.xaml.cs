using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated; // 快捷键
        }
        // 快捷键
        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.Z:
                            Undo();
                            break;
                    }
                }
            }
        }

        private bool isTextSet = false;
        private String strBackup = null;

        private Handler.Options crtOption = new Handler.Options();
        private async void Menu_SwitchExecute_Click(object sender, RoutedEventArgs e)
        {
            Menu_SwitchExecute.IsEnabled = false;
            TextBox1.IsEnabled = false;
            ProgressRing1.IsActive = true;

            strBackup = TextBox1.Text;
            String t = TextBox1.Text;
            await Task.Run(() => { t = new Handler(crtOption).Switch(t); });
            TextBox1.Text = t;

            Menu_SwitchExecute.IsEnabled = true;
            TextBox1.IsEnabled = true;
            ProgressRing1.IsActive = false;

            isTextSet = true;
            Button1.IsEnabled = true;
            Button1.Visibility = Visibility.Visible;
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatisticChars();
            if (!isTextSet)
            {
                strBackup = null;
                Button1.IsEnabled = false;
                Button1.Visibility = Visibility.Collapsed;
            }

            isTextSet = false;
        }

        private void TextBlock1_Loaded(object sender, RoutedEventArgs e)
        {
            StatisticChars();
        }

        private void StatisticChars()
        {
            TextBlock1.Text = TextBox1.Text.Length.ToString() + " 字符";

            if (TextBox1.Text.Length > 1000)
                ToolTipService.SetToolTip(TextBlock1, "当字符过多时，可能会有明显的性能问题");
            else
                ToolTipService.SetToolTip(TextBlock1, null);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private async void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            await new AboutDialog().ShowAsync();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void Undo()
        {
            if (strBackup == null) return;
            TextBox1.Text = strBackup;
            strBackup = null;
            Button1.IsEnabled = false;
            Button1.Visibility = Visibility.Collapsed;
        }

        private async void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".txt");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                strBackup = null;
                TextBox1.Text = await Windows.Storage.FileIO.ReadTextAsync(file);
            }
        }

        private void PopupToast(string strMainContent, string strButtonContent)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = strMainContent
                                },
                            },
                        //AppLogoOverride = new ToastGenericAppLogo()
                        //{
                        //    Source = "https://unsplash.it/64?image=1005",
                        //    HintCrop = ToastGenericAppLogoCrop.Circle
                        //}
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                        {
                            new ToastButton(strButtonContent, "None")
                            {
                                ActivationType = ToastActivationType.Foreground
                            }
                        }
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private async void Menu_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.FileTypeChoices.Add("文本文档", new List<string>() { ".txt" });
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;

            Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                await FileIO.WriteTextAsync(file, TextBox1.Text);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    PopupToast("文件已保存", "朕知道了");
                }
                else
                {
                    PopupToast("文件保存失败", "朕知道了");
                }
            }
        }

        private async void Menu_SwitchOptions_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OptionDialog();
            dlg.Tag = crtOption;
            await dlg.ShowAsync();
            if (dlg.isOk)
            {
                crtOption = (Handler.Options)dlg.Tag;
                ApplicationDataContainer setting = ApplicationData.Current.RoamingSettings;
                setting.Values["option"] = crtOption.Code();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer setting = ApplicationData.Current.RoamingSettings;
            object obj = setting.Values["option"];
            if (obj != null)
                crtOption = new Handler.Options((string)obj);
        }
    }
}