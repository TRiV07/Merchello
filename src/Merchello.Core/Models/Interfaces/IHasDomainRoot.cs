using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Defines an entity that has a domain root structure ID
    /// </summary>
    public interface IHasDomainRoot
    {
        /// <summary>
        /// Gets the domain root structure ID
        /// </summary>
        [DataMember]
        int DomainRootStructureID { get; }
    }
}
