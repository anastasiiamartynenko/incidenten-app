using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Incidenten.Mobile.ViewModels;

public abstract class _BaseViewModel : INotifyPropertyChanged
{
    /**
     * A helper function to simplify the set property process.
     */
    protected bool SetProperty<T>(ref T backingField, T value,
        [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    protected string _error = String.Empty;
    public string Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}