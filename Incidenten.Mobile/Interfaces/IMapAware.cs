using System.Windows.Input;

namespace Incidenten.Mobile.Interfaces;

public interface IMapAware
{
    event Action<Location?> LocationChanged;
    ICommand MapClickedCommand { get; }
}