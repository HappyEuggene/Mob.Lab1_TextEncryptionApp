using Android.App;
using Android.Content;
using Android.Runtime;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using TextEncryptionApp.Platforms.Android;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(FileSavePicker))]
namespace TextEncryptionApp.Platforms.Android
{
    public class FileSavePicker : IFileSavePicker
    {
        public TaskCompletionSource<string> SaveFileTaskCompletionSource { get; set; }

        public Task<string> PickSaveFileAsync(string defaultFileName)
        {
            // Створюємо intent для збереження файлу
            Intent intent = new Intent(Intent.ActionCreateDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("*/*");
            intent.PutExtra(Intent.ExtraTitle, defaultFileName);

            // Запускаємо Activity для результату
            var activity = Platform.CurrentActivity;
            activity.StartActivityForResult(intent, 1001);

            // Очікуємо результат
            SaveFileTaskCompletionSource = new TaskCompletionSource<string>();
            return SaveFileTaskCompletionSource.Task;
        }
    }
}
