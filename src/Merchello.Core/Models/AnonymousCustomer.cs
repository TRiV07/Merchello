namespace Merchello.Core.Models
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
        public AnonymousCustomer(int storeId)
            : this(storeId, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomer"/> class.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public AnonymousCustomer(int storeId, ExtendedDataCollection extendedData)
            : base(true, storeId, extendedData)
        {
        }
    }
}
