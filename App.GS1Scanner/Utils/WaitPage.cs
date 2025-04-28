using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;

namespace App.GS1Scanner.Utils
{
    public sealed class WaitPage : ContentPage
    {
        public WaitPage(string text = "Пожалуйста, подождите…")
        {
            BackgroundColor = Colors.Black.MultiplyAlpha(0.6f);
            Content = new ActivityIndicator
            {
                IsRunning = true,
                Color = Colors.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            // подпись-подсказка
            var label = new Label
            {
                Text = text,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 80, 0, 0)
            };

            Content = new Grid
            {
                Children = { Content, label }
            };
        }
    }

}
