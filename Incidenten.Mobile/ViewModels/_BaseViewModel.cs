using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Incidenten.Mobile.ViewModels;

public abstract class _BaseViewModel : INotifyPropertyChanged
{
    protected string _error = String.Empty;

    public string Error
    {
        get => _error;
        set
        {
            _error = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}