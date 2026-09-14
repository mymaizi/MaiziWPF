using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;

namespace MaiziWPF.Core
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "确认";

        public DialogCloseListener RequestClose { get; set; }

        public bool CanCloseDialog() => true;

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public ConfirmDialogViewModel()
        {
            ConfirmCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });

            CancelCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}