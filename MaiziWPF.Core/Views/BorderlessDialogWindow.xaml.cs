namespace MaiziWPF.Core.Views;

public partial class BorderlessDialogWindow : System.Windows.Window, Prism.Dialogs.IDialogWindow
{
    public BorderlessDialogWindow()
    {
        InitializeComponent();
    }

    public Prism.Dialogs.IDialogResult Result { get; set; }
}