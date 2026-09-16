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

        public Task ShowDialogAsync<TView>(Action<FormBindableBase> setup = null) where TView : class
        {
            var tcs = new TaskCompletionSource<bool>();
            var parameters = new DialogParameters();
            if (setup != null)
            {
                parameters.Add("_SetupAction", setup);
            }
            _dialogService.ShowDialog(typeof(TView).Name, parameters, _ => tcs.TrySetResult(true));
            return tcs.Task;
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

        public async Task ShowMessageAsync(string message, string title = "提示")
        {
            var dialogParameters = new DialogParameters();
            dialogParameters.Add("message", message);

            _dialogService.ShowDialog("MessageDialog", dialogParameters, _ => { });
        }
    }
}