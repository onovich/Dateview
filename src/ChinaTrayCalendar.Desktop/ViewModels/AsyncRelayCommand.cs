using System.Windows.Input;

namespace ChinaTrayCalendar.Desktop.ViewModels;

public sealed class AsyncRelayCommand : ICommand
{
    private readonly Func<CancellationToken, Task> executeAsync;
    private readonly Func<bool>? canExecute;
    private bool isExecuting;

    public AsyncRelayCommand(Func<CancellationToken, Task> executeAsync, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(executeAsync);

        this.executeAsync = executeAsync;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !isExecuting && (canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        await ExecuteAsync(CancellationToken.None);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!CanExecute(parameter: null))
        {
            return;
        }

        try
        {
            isExecuting = true;
            RaiseCanExecuteChanged();
            await executeAsync(cancellationToken);
        }
        finally
        {
            isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
