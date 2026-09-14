using Prism.Dialogs;
using Prism.Ioc;
using System;
using System.Threading.Tasks;

namespace MaiziWPF.Core
{
    public class DialogHostService : IDialogHostService
    {
        private readonly IDialogService _dialogService;
        private readonly IContainerProvider _containerProvider;

        public DialogHostService(IDialogService dialogService, IContainerProvider containerProvider)
        {
            _dialogService = dialogService;
            _containerProvider = containerProvider;
        }

        public async Task ShowDialogAsync<TView>(Action<TView> setup = null) where TView : class
        {
            var tcs = new TaskCompletionSource<bool>();
            var dialogName = typeof(TView).Name;

            _dialogService.ShowDialog(dialogName, null, result =>
            {
                tcs.SetResult(true);
            });

            await tcs.Task;
        }

        public async Task<bool> ConfirmAsync(string message, string title = "确认", string confirmButtonText = "确定", string cancelButtonText = "取消")
        {
            var tcs = new TaskCompletionSource<bool>();

            var dialogParameters = new DialogParameters();
            dialogParameters.Add("message", message);

            _dialogService.ShowDialog("ConfirmDialog", dialogParameters, result =>
            {
                tcs.SetResult(result.Result == ButtonResult.OK);
            });

            return await tcs.Task;
        }
    }
}