namespace Merchello.Core.Models
{
    using Merchello.Core.Models.EntityBase;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a Merchello Carrier
    /// </summary>
    public interface ICarrier : ICustomerBase
    {
        /// <summary>
        /// Gets the full name of the Carrier
        /// </summary>
        [IgnoreDataMember]
        string FullName { get; }

        /// <summary>
        /// Gets or sets first name of the Carrier
        /// </summary>
        [DataMember]
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the Carrier
        /// </summary>
        [DataMember]
        string LastName { get; set; }

        /// <summary>
        /// Gets or sets email address of the Carrier
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// Gets the login name.
        /// </summary>
        [DataMember]
        string LoginName { get; }

        [DataMember]
        bool IsDisabled { get; set; }

        [DataMember]
        bool IsOnDuty { get; set; }

        [DataMember]
        int ActiveOrders { get; set; }
    }
}
