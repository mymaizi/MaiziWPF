using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace MaiziWPF.Core
{
    public class FormBindableBase : BindableBase, IDialogAware
    {
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; }
        private readonly ISnackbarService _snackbarService;
        public Action OnSaveSuccessCallback { get; set; }

        public string DialogTitle { get; set; }
        public DialogCloseListener RequestClose { get; private set; }

        public FormBindableBase(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;
            CancelCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        protected void ShowWarning(string message)
        {
            _snackbarService.EnqueueWarning(message);
        }

        protected void ShowError(string message)
        {
            _snackbarService.EnqueueError(message);
        }

        protected void ShowSuccess(string message)
        {
            _snackbarService.EnqueueSuccess(message);
        }

        protected void CloseDialog(){
             RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters != null && parameters.TryGetValue<Action<FormBindableBase>>("_SetupAction", out var setup))
            {
                setup(this);
            }
        }
    }
}