using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Programc
    {
        public static void Mainc(string[] args)
        {
            string prix = "1333,565 EUR";
            var test = CultureInfo.CreateSpecificCulture("fr-CH");
            test.NumberFormat.CurrencyDecimalSeparator = ",";
            test.NumberFormat.CurrencyGroupSeparator = "";
            test.NumberFormat.CurrencySymbol = "FR";

            var value = decimal.Parse(prix);
            var toto = (value).ToString("C", test.NumberFormat);
            //Console.WriteLine( toto);
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-CH");
            NumberFormatInfo nfi = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            var clone = (NumberFormatInfo)nfi.Clone();
            clone.CurrencySymbol = "CHF";
            clone.CurrencyPositivePattern = 2;
            // Change the negative currency pattern to <code><space><sign><value>.     
            clone.CurrencyNegativePattern = 12;

            var p1 = 132.92m;
            var p2 = 133.00m;
            var p3 = -13.22m;
            var p4 = 133.47m;

            Console.WriteLine(LiteralDecimal(p1));
            Console.WriteLine(LiteralDecimal(p2));
            Console.WriteLine(LiteralDecimal(p3));
            Console.WriteLine(LiteralDecimal(p4));

        }

        public static string PriceFormat(string strPriceInput, string wrapperSup1, string wrapperSup2, string wrapperHT1, string wrapperHT2, string htmlCurrency = "", string isoCurrency = "CHF", string currencyDecimalSeparator = ".", bool isFnacPro = false, string country = "CH", bool currencyIsBeforePrice = true, bool currencyInTheMiddle = true)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(strPriceInput))
            {
                if (string.IsNullOrEmpty(wrapperSup1) && string.IsNullOrEmpty(wrapperSup2))
                {
                    wrapperSup1 = currencyDecimalSeparator;
                }

                strPriceInput = CleanPrice(strPriceInput, country, htmlCurrency, isoCurrency);

                //string pattern = "";
                //if (isFnacPro)
                //{
                //    pattern = @"(\d+)([,.])?(\d{0,2})(\" + htmlCurrency + ")((HT)|(TTC))";
                //}
                //else
                //{
                var pattern = @"(\d+)([,.])?(\d{0,2})" + (!string.IsNullOrEmpty(htmlCurrency) ? "(\\" + htmlCurrency + ")" : "") + (isFnacPro ? "((HT)|(TTC))" : "");
                //}

                bool isDecimal = (strPriceInput.Contains(".") || strPriceInput.Contains(","));


                if (!isDecimal)
                {
                    wrapperSup1 = wrapperSup2 = string.Empty;
                }
                if (!isFnacPro)
                {
                    wrapperHT1 = wrapperHT2 = string.Empty;
                }

                try
                {
                    if (currencyIsBeforePrice)
                    {
                        result = Regex.Replace(strPriceInput, pattern, (!string.IsNullOrEmpty(htmlCurrency) ? "$4" : "") + "$1" + (!currencyInTheMiddle ? currencyDecimalSeparator : "") + wrapperSup1 + "$3" + wrapperSup2 + wrapperHT1 + (isFnacPro ? "$5" : "") + wrapperHT2); //€ nn,nn ou € nn <wrapperSup1> nn <wrapperSup2> <em>HT</em>
                    }
                    else
                    {
                        if (wrapperSup1 == "." || wrapperSup1 == ",")
                            result = Regex.Replace(strPriceInput, pattern, "$1" + wrapperSup1 + "$3" + (!string.IsNullOrEmpty(htmlCurrency) ? "$4" : "") + wrapperHT1 + (isFnacPro ? "$5" : "") + wrapperHT2);// nn,nn €  <em>HT<em>
                        else
                            result = Regex.Replace(strPriceInput, pattern, "$1" + wrapperSup1 + (!string.IsNullOrEmpty(htmlCurrency) ? "$4" : "") + "$3" + wrapperSup2 + wrapperHT1 + (isFnacPro ? "$5" : "") + wrapperHT2);// nn € <sup> nn</sup> <em>HT</em>


                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }

            return result;
        }

        private static string CleanPrice(string strPriceInput, string country, string htmlCurrency, string isoCurrency)
        {
            string result;
            switch (country)
            {
                case "CH":
                    result = new string(strPriceInput.ToCharArray().Where(c => Char.IsDigit(c) || c == '.' || c == ',').ToArray());
                    break;
                default:
                    result = new string(strPriceInput.ToCharArray().Where(c => Char.IsDigit(c) || c == '.' || c == ',').ToArray());
                    if (string.IsNullOrEmpty(result))
                    {
                        result = strPriceInput;
                    }
                    else if (result.Contains('.'))
                    {
                        if (result.Contains(',')) // fix si 1.345,99 => devient 1345,99
                        {
                            int lastIndex = Math.Max(result.LastIndexOf(','), result.LastIndexOf('.'));
                            string toClean = result.Substring(0, lastIndex);
                            string toKeep = result.Substring(lastIndex, result.Length - lastIndex);
                            toClean = toClean.Replace(",", "").Replace(".", "");
                            result = toClean + toKeep;
                        }
                        else if (result.Count(s => s == '.') == 1)
                        {
                            int lastIndex = result.LastIndexOf('.');
                            string lastPart = result.Substring(lastIndex + 1, result.Length - (lastIndex + 1));
                            if (lastPart.Length >= 3)
                            {
                                result = result.Replace(".", ""); // fix 1.699  devient 1699 € 
                            }
                            else
                            {
                                result = result.Replace(".", ","); // fix 8.20 devient 8,20
                            }
                        }
                    }
                    break;
            }

            strPriceInput = strPriceInput.ToUpper();
            if (strPriceInput.Contains(htmlCurrency.ToUpper()) || strPriceInput.Contains(isoCurrency))
            {
                result += htmlCurrency;
            }
            if (strPriceInput.Contains("HT"))
            {
                result += "HT";
            }
            if (strPriceInput.Contains("TTC"))
            {
                result += "TTC";
            }
            if (strPriceInput.Contains("%"))
            {
                result += "%";
            }
            return result;
        }

        public static string DisplayCost(decimal cost)
        {
            var culture = CultureInfo.CreateSpecificCulture("fr-CH");
            NumberFormatInfo _customNumberFormat = culture.NumberFormat;
            _customNumberFormat.CurrencyDecimalSeparator = ".";
            _customNumberFormat.CurrencyGroupSeparator = ",";
            _customNumberFormat.CurrencySymbol = "";
            _customNumberFormat.CurrencyPositivePattern = 1;
            var _zeroCentsReplacement = '-';

            var result = cost.ToString("N" + _customNumberFormat.CurrencyDecimalDigits, _customNumberFormat);
            if (cost % 1 == 0 && _zeroCentsReplacement != '\0')
                result = result.Replace(_customNumberFormat.CurrencyDecimalSeparator + new string('0', _customNumberFormat.CurrencyDecimalDigits), _customNumberFormat.CurrencyDecimalSeparator + _zeroCentsReplacement);
            return result;
        }

        public static string LiteralDecimal(decimal number)
        {
            string result;
            string ZeroCentsReplacement = "-";
            string CurrencySeparatorDecimal = ".";

            if (number == 0)
            {
                result = "0";
            }
            else if (Math.Floor(number) == number)
            {
                result = number.ToString("#,###") + (!string.IsNullOrEmpty(ZeroCentsReplacement) ? CurrencySeparatorDecimal + ZeroCentsReplacement : "");
            }
            else
            {
                result = number.ToString("#,##0.00");
            }
            return result;
        }
    }

    public class PriceFormatter : IFormatProvider, ICustomFormatter
    {
        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Check whether this is an appropriate callback             
            if (!this.Equals(formatProvider))
                return null;

            switch (format)
            {
                case "SwissDefault":
                    CultureInfo swissCulture = CultureInfo.GetCultureInfo("fr-CH").Clone() as CultureInfo;
                    NumberFormatInfo nfi = swissCulture.NumberFormat;
                    nfi.CurrencySymbol = "";
                    nfi.CurrencyPositivePattern = 1;
                    return ((double)arg).ToString("C", nfi).Replace(".00", ".-");
                case "SwissWithCurrency":
                    return "CHF " + ((double)arg).ToString("0.00").Replace(".00", ".-");
                case "DefaultWithcurrency":
                    return ((double)arg).ToString("0,00") + "&euro;";
                case "Default":
                    return ((double)arg).ToString("0,00");
                default:
                    throw new FormatException(string.Format("The {0} format specifier is invalid.", format));
            }
        }

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }
    }
}
