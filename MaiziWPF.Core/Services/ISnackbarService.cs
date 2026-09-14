namespace MaiziWPF.Core
{
    public interface ISnackbarService
    {
        void Enqueue(string message);
        void EnqueueInfo(string message);
        void EnqueueWarning(string message);
        void EnqueueError(string message);
        void EnqueueSuccess(string message);
    }
}