using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEncryptionApp
{
    public interface IFileSavePicker
    {
        Task<string> PickSaveFileAsync(string defaultFileName);
    }
}
