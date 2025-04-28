using App.Repository.Services;
using App.Shared.Models;
using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using App.GS1Scanner.Platforms.Android;
using App.Utils;
using System.Threading.Tasks;
using App.GS1Scanner.Utils;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices;

namespace App.GS1Scanner
{
    public partial class MainPage : ContentPage
    {
        private readonly ILoginService loginService;
        private readonly ICodeSenderService codeSenderService;
        private string token;

        public MainPage(ILoginService loginService, ICodeSenderService codeSenderService)
        {
            InitializeComponent();
            this.loginService = loginService;
            this.codeSenderService = codeSenderService;
            successScanButton.IsEnabled = false;
            rejectScanButton.IsEnabled = false;
            logoutButton.IsVisible = false;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var login = loginEntry.Text;
            var password = passwordEntry.Text;

            if (login.IsNullorEmpty() || password.IsNullorEmpty())
            {
                resultLabel.Text = "Введите логин и пароль.";
                resultLabel.TextColor = Color.FromRgb(245, 2, 2);
                return;
            }

            var waitPage = new WaitPage("Идет обработка…");
            Answer<TokenModel> answer;
            try
            {
                await Navigation.PushModalAsync(waitPage);
                answer = await loginService.LoginAsync(new viUserLogin { Login = login, Password = password });
                if (answer.Code != 1)
                {
                    resultLabel.Text = "Ошибка авторизации.";
                    resultLabel.TextColor = Color.FromRgb(245, 2, 2);
                    return;
                }
            }
            finally
            {
                await Navigation.PopModalAsync();
            }

            token = answer.Data.Token;
            resultLabel.Text = "Авторизация успешна.";
            resultLabel.TextColor = Color.FromRgb(15, 189, 56);
            loginEntry.IsVisible = passwordEntry.IsVisible = loginButton.IsVisible = false;
            successScanButton.IsEnabled = true;
            rejectScanButton.IsEnabled = true;
            logoutButton.IsVisible = true;
        }

        private async void OnScanSuccessClicked(object sender, EventArgs e)
        {
            await StartScanAsync(status: 3);
        }

        private async void OnScanRejectClicked(object sender, EventArgs e)
        {
            await StartScanAsync(status: 7);
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            token = null;
            loginEntry.IsVisible = passwordEntry.IsVisible = loginButton.IsVisible = true;
            successScanButton.IsEnabled = false;
            rejectScanButton.IsEnabled = false;
            logoutButton.IsVisible = false;
            loginEntry.Text = string.Empty;
            passwordEntry.Text = string.Empty;
            resultLabel.Text = "Вы вышли.";
            ResetUi();
        }

        private async void OnCopyCodeClicked(object sender, EventArgs e)
        {
            if (codeLabel == null) return;

            var text = codeLabel.Text;
            if (string.IsNullOrWhiteSpace(text)) return;

            await Clipboard.SetTextAsync(text);
            await DisplayAlert("Копия", "Код скопирован в буфер обмена", "OK");
        }

        private async void OnCopyResultClicked(object sender, EventArgs e)
        {
            if (resultLabel == null) return;

            var text = resultLabel.Text;
            if (string.IsNullOrWhiteSpace(text)) return;

            await Clipboard.SetTextAsync(text);
            await DisplayAlert("Копия", "Результат скопирован", "OK");
        }

        // вызывается из OnScanSuccessClicked(status:3) & OnScanRejectClicked(status:7)
        private async Task StartScanAsync(int status)
        {
            var permission = await Permissions.RequestAsync<Permissions.Camera>();
            if (permission != PermissionStatus.Granted)
            {
                await DisplayAlert("Нет доступа", "Для сканирования нужна камера.", "OK");
                return;
            }

            successScanButton.IsEnabled = rejectScanButton.IsEnabled = false;

            codeLabel.Text = string.Empty;
            resultLabel.Text = string.Empty;
            copyCodeButton.IsVisible = false;
            copyResultButton.IsVisible = false;

            var scanner = new MlKitScanner();
            bool scanned = false;

            var modalPage = new ContentPage { Content = scanner };

            modalPage.Disappearing += (obj, ev) =>
            {
                if (!scanned)
                    successScanButton.IsEnabled = rejectScanButton.IsEnabled = true;
            };

            EventHandler<string>? handler = null;
            handler = async (_, code) =>
            {
                if (scanned) return;
                scanned = true;
                scanner.CodeDetected -= handler;

                try { Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100)); }
                catch { }

                codeLabel.Text = code;
                copyCodeButton.IsVisible = true;

                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();

                var waitPage = new WaitPage("Отправка кода…");
                await Navigation.PushModalAsync(waitPage);

                var message = "Сессия истекла. Авторизуйтесь.";
                if (token != null)
                {
                    var model = new CodeModel(code, status);
                    var answer = await codeSenderService.SendCodeAsync(token, model);
                    message = answer.message ?? "Нет ответа";
                }

                await Navigation.PopModalAsync();

                resultLabel.Text = message;
                copyResultButton.IsVisible = true;

                successScanButton.IsEnabled = rejectScanButton.IsEnabled = true;
            };

            scanner.CodeDetected += handler;
            await Navigation.PushModalAsync(modalPage);
        }

        private void ResetUi()
        {
            codeLabel.Text = string.Empty;
            resultLabel.Text = string.Empty;
            copyCodeButton.IsVisible = false;
            copyResultButton.IsVisible = false;
        }
    }
}
