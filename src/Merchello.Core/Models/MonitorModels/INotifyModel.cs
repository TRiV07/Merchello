using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Defines the NotifyModel
    /// </summary>
    public interface INotifyModel : IHasDomainRoot
    {
        string[] Contacts { get; set; }
    }
}