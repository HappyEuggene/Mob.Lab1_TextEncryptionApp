using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Storage;
using System.IO;

namespace TextEncryptionApp
{
    public partial class ResultPage : ContentPage
    {
        public ResultPage(string resultText)
        {
            InitializeComponent();
            resultEditor.Text = resultText;

            // Побудова графіка при завантаженні сторінки
            DrawFrequencyGraph(resultText);
        }

        private async void OnSaveToFileClicked(object sender, EventArgs e)
        {
            try
            {
#if IOS || MACCATALYST || ANDROID || TIZEN || WINDOWS
                var fileName = $"result_{DateTime.Now:yyyyMMddHHmmss}.txt";
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(resultEditor.Text));

                var fileResult = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

                if (fileResult != null)
                {
                    await DisplayAlert("Успіх", $"Файл збережено: {fileResult.FilePath}", "OK");
                }
                else
                {
                    await DisplayAlert("Скасовано", "Збереження файлу було скасовано.", "OK");
                }
#else
                await DisplayAlert("Помилка", "Функція збереження файлів не підтримується на цій платформі.", "OK");
#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося зберегти файл: {ex.Message}", "OK");
            }
        }

        private void DrawFrequencyGraph(string text)
        {
            var frequencies = new Dictionary<char, int>();

            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    if (frequencies.TryGetValue(c, out int count))
                    {
                        frequencies[c] = count + 1;
                    }
                    else
                    {
                        frequencies[c] = 1;
                    }
                }
            }

            graphLayout.Children.Clear();

            foreach (var item in frequencies.OrderBy(f => f.Key))
            {
                var barHeight = item.Value * 5; // Регулюйте коефіцієнт для відповідної висоти
                var bar = new BoxView
                {
                    HeightRequest = barHeight,
                    WidthRequest = 20,
                    Color = Colors.Blue,
                    VerticalOptions = LayoutOptions.End
                };

                var label = new Label
                {
                    Text = item.Key.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End
                };

                var stack = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = { bar, label }
                };

                graphLayout.Children.Add(stack);
            }
        }
    }
}
