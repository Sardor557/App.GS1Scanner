using Microsoft.Maui.Controls;
using System;

namespace App.GS1Scanner.Platforms.Android
{
    public class MlKitScanner : View
    {
        public event EventHandler<string> CodeDetected;
        internal void RaiseCode(string code) => CodeDetected?.Invoke(this, code);
    }
}
