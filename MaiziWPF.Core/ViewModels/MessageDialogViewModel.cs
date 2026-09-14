using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;

namespace MaiziWPF.Core
{
    public class MessageDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "提示";

        public DialogCloseListener RequestClose { get; set; }

        public bool CanCloseDialog() => true;

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DelegateCommand CloseCommand { get; }

        public MessageDialogViewModel()
        {
            CloseCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            });
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}