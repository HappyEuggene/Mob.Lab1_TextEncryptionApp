using System;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;

namespace TextEncryptionApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Встановлюємо значення за замовчуванням
            encryptionMethodPicker.SelectedIndex = 0;
        }

        private async void OnReadFromFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/plain" } },
                    { DevicePlatform.iOS, new[] { "public.plain-text" } },
                    { DevicePlatform.WinUI, new[] { ".txt" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                });

                var fileResult = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Оберіть файл для зчитування",
                    FileTypes = customFileType
                });

                if (fileResult != null)
                {
                    using var stream = await fileResult.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    string text = await reader.ReadToEndAsync();
                    inputText.Text = text;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося зчитати файл: {ex.Message}", "OK");
            }
        }

        private async void OnEncryptClicked(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                string method = encryptionMethodPicker.SelectedItem?.ToString() ?? string.Empty;
                string text = inputText.Text;
                string key = keyEntry.Text;

                string result = method switch
                {
                    "Цезаря" => CaesarEncrypt(text, int.Parse(key)),
                    "Віженера" => VigenereEncrypt(text, key),
                    _ => text
                };

                await Navigation.PushAsync(new ResultPage(result));
            }
        }

        private async void OnDecryptClicked(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                string method = encryptionMethodPicker.SelectedItem?.ToString() ?? string.Empty;
                string text = inputText.Text;
                string key = keyEntry.Text;

                string result = method switch
                {
                    "Цезаря" => CaesarDecrypt(text, int.Parse(key)),
                    "Віженера" => VigenereDecrypt(text, key),
                    _ => text
                };

                await Navigation.PushAsync(new ResultPage(result));
            }
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(inputText.Text))
            {
                DisplayAlert("Помилка", "Введіть текст.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(keyEntry.Text))
            {
                DisplayAlert("Помилка", "Введіть ключ.", "OK");
                return false;
            }

            // Додаткова перевірка для методу Цезаря
            if (encryptionMethodPicker.SelectedItem?.ToString() == "Цезаря" && !int.TryParse(keyEntry.Text, out _))
            {
                DisplayAlert("Помилка", "Ключ повинен бути числом для методу Цезаря.", "OK");
                return false;
            }

            return true;
        }

        // Методи шифрування та дешифрування
        private string CaesarEncrypt(string text, int key)
        {
            char[] buffer = text.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                char letter = buffer[i];
                if (char.IsLetter(letter))
                {
                    char offset = char.IsUpper(letter) ? 'A' : 'a';
                    letter = (char)(((letter + key - offset) % 26) + offset);
                    buffer[i] = letter;
                }
            }
            return new string(buffer);
        }

        private string CaesarDecrypt(string text, int key)
        {
            return CaesarEncrypt(text, 26 - (key % 26));
        }

        private string VigenereEncrypt(string text, string key)
        {
            string result = "";
            int keyIndex = 0;
            key = key.ToUpper();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    int k = key[keyIndex % key.Length] - 'A';
                    c = (char)(((c + k - offset) % 26) + offset);
                    keyIndex++;
                }
                result += c;
            }
            return result;
        }

        private string VigenereDecrypt(string text, string key)
        {
            string result = "";
            int keyIndex = 0;
            key = key.ToUpper();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    int k = key[keyIndex % key.Length] - 'A';
                    c = (char)(((c - k - offset + 26) % 26) + offset);
                    keyIndex++;
                }
                result += c;
            }
            return result;
        }
    }
}
