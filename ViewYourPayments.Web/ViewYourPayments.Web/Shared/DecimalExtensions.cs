using System;

namespace ViewYourPayments.Web.Shared
{
    /// <summary>
    /// Extension class for decimal related modifications
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Extension method to convert a decimal into a currency formatted string
        /// </summary>
        /// <param name="decimalValue">The decimal value.</param>
        public static string ToCurrency(this decimal decimalValue)
        {
            IFormatProvider formatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-GB");
            return decimalValue.ToString("C", formatProvider);
        }

        /// <summary>
        /// Extension method to convert a decimal into a currency formatted string without a decimal place.
        /// </summary>
        /// <param name="decimalValue">The decimal value.</param>
        public static string ToCurrencyWithoutDecimalPlace(this decimal decimalValue)
        {
            IFormatProvider formatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-GB");
            return decimalValue.ToString("C0", formatProvider);
        }
    }
}
