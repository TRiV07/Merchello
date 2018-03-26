﻿namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The anonymous customer.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AnonymousCustomer : CustomerBase, IAnonymousCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomer"/> class.
        /// </summary>
        public AnonymousCustomer(int domainRootStructureID)
            : this(domainRootStructureID, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomer"/> class.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public AnonymousCustomer(int domainRootStructureID, ExtendedDataCollection extendedData)
            : base(true, domainRootStructureID, extendedData)
        {
        }
    }
}
