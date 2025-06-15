using System.Windows.Input;

namespace Incidenten.Mobile.Interfaces;

public interface IIncidentForm
{
    string FormTitle { get; }
    string SubmitIncidentFormLabel { get; }
    ICommand SubmitIncidentFormCommand { get; }
}