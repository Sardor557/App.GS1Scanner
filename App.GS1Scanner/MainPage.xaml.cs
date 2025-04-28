using App.Repository.Services;
using App.Shared.Models;
using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using App.GS1Scanner.Platforms.Android;
using Microsoft.Maui.Dispatching;
using App.Utils;
using App.GS1Scanner.Utils;

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
            scanButton.IsEnabled = false;
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

            var answer = await loginService.LoginAsync(new viUserLogin { Login = login, Password = password });
            if (answer.Code != 1)
            {
                resultLabel.Text = "Ошибка авторизации.";
                resultLabel.TextColor = Color.FromRgb(245, 2, 2);
                return;
            }

            token = answer.Data.Token;
            resultLabel.Text = "Авторизация успешна.";
            resultLabel.TextColor = Color.FromRgb(15, 189, 56);
            loginEntry.IsVisible = passwordEntry.IsVisible = loginButton.IsVisible = false;
            scanButton.IsEnabled = true;
            logoutButton.IsVisible = true;
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Нет доступа", "Для сканирования нужна камера.", "OK");
                    return;
                }

                scanButton.IsEnabled = false;

                var scanner = new MlKitScanner();
                bool scanned = false;

                EventHandler<string> handler = null;
                handler = async (_, code) =>
                {
                    if (scanned) return;
                    scanned = true;
                    scanner.CodeDetected -= handler;

                    await Navigation.PopModalAsync();

                    var waitPage = new WaitPage("Отправка кода…");
                    await Navigation.PushModalAsync(waitPage);

                    try
                    {
                        var message = "Сессия истекла. Авторизуйтесь.";
                        if (token != null)
                        {
                            var res = await codeSenderService.SendCodeAsync(token, code);
                            if (res.code != 1)
                                resultLabel.TextColor = Color.FromRgb(245, 2, 2);
                            message = res.message ?? "Нет ответа";
                        }

                        await Navigation.PopModalAsync();
                        resultLabel.Text = $"Скан: {code}\nОтвет: {message}";
                    }
                    finally
                    {
                        scanButton.IsEnabled = true;
                    }
                };

                scanner.CodeDetected += handler;
                await Navigation.PushModalAsync(new ContentPage { Content = scanner });
            }
            catch (Exception ex)
            {
                resultLabel.TextColor = Color.FromRgb(245, 2, 2);
                resultLabel.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            token = null;
            loginEntry.IsVisible = passwordEntry.IsVisible = loginButton.IsVisible = true;
            scanButton.IsEnabled = false;
            logoutButton.IsVisible = false;
            loginEntry.Text = string.Empty;
            passwordEntry.Text = string.Empty;
            resultLabel.Text = "Вы вышли.";
        }
        private record AnswerBasic(int Status, string Message);
    }
}
