﻿namespace Merchello.Core
{
    using System.Globalization;

    /// <summary>
    /// The currency helper.
    /// </summary>
    public static class CurrencyHelper
    {
        /// <summary>
        /// Formats an amount based on Merchello store settings.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatCurrency(decimal amount, int storeId)
        {
            if (!CurrencyContext.HasCurrent) return amount.ToString(CultureInfo.InvariantCulture);
            return CurrencyContext.Current.FormatCurrency(amount, storeId);
        }
    }

    /// <summary>
    /// Currency formatting extension.
    /// </summary>
    public static class CurrencyFormattingExtension
    {
        /// <summary>
        /// The as formatted currency.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsFormattedCurrency(this decimal amount, int storeId)
        {
            return CurrencyHelper.FormatCurrency(amount, storeId);
        }
    }
}