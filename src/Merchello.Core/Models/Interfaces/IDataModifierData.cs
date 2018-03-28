namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Defines ModifiableData.
    /// </summary>
    public interface IDataModifierData : IHasDomainRoot
    {
        /// <summary>
        /// Gets or sets the modified data logs.
        /// </summary>
        IEnumerable<IDataModifierLog> ModifiedDataLogs { get; set; }
    }
}