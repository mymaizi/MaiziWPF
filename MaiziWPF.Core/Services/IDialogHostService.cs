using System;
using System.Threading.Tasks;

namespace MaiziWPF.Core
{
    public interface IDialogHostService
    {
        Task ShowDialogAsync<TView>(Action<FormBindableBase> setup = null) where TView : class;
        Task<bool> ConfirmAsync(string message, string title = "确认", string confirmButtonText = "确定", string cancelButtonText = "取消");
    }
}