using System;
using System.Globalization;
using System.Text;

namespace EDTESP.Infrastructure.CC.Util
{
    public static class DecimalUtil
    {
        public static string ToStrValue(this decimal value, bool removeDecimals = true, bool leadWithChar = false, int countChar = 0, char chr = '0')
        {
            var strvlr = value.ToString("N2");

            if (removeDecimals)
                strvlr = strvlr.ClearNumber();

            if (leadWithChar)
                strvlr = strvlr.PadLeft(countChar, chr);
            
            return strvlr;
        }

        public static string GetFullNameValue(decimal decimalValue, bool isMoney = true)
        {
            var result = new StringBuilder();

            var valor = decimalValue.ToString("n2");

            var arrayData = valor.Split(',');

            var valueNumber = "";

            if (arrayData != null && arrayData.Length > 1)
                valueNumber = arrayData[0];
            else
                valueNumber = "0";

            if (valueNumber.Contains("."))
            {
                var arraySeparator = valueNumber.Split('.');

                var quantityVerify = arraySeparator.Length;

                var valueResult = "";

                var firstTime = true;

                for (var i = 0; i < arraySeparator.Length; i++)
                {
                    valueResult = GetCompleteNameByLength(arraySeparator[i]);

                    if (quantityVerify == 3 && !string.IsNullOrEmpty(valueResult))
                        result.Append(valueResult + " milhões");
                    else if (quantityVerify == 2 && !string.IsNullOrEmpty(valueResult))
                    {
                        if (firstTime)
                            result.Append(valueResult + " mil");
                        else
                            result.Append(" e " + valueResult + " mil");
                    }
                    else if (!string.IsNullOrEmpty(valueResult))
                    {
                        if (firstTime)
                            result.Append(valueResult);
                        else
                            result.Append(" e " + valueResult);
                    }

                    quantityVerify -= 1;

                    if (firstTime)
                        firstTime = false;
                }
            }
            else
                result.Append(GetCompleteNameByLength(valueNumber));

            if (isMoney)
            {
                if (result.Length > 0)
                    result.Append(" reais");

                if (arrayData != null && arrayData.Length > 1 && arrayData[1] != "00")
                {
                    var cents = GetCompleteNameByLength(arrayData[1]);

                    if (!string.IsNullOrEmpty(cents))
                        result.Append(" e " + cents + " centavos");
                }
            }

            return result.ToString();
        }

        private static string GetNameNumber(int number)
        {
            switch(number)
            {
                case 1:
                    return "um";
                case 2:
                    return "dois";
                case 3:
                    return "três";
                case 4:
                    return "quatro";
                case 5:
                    return "cinco";
                case 6:
                    return "seis";
                case 7:
                    return "sete";
                case 8:
                    return "oito";
                case 9:
                    return "nove";
                default:
                    return "zero";
            }
        }

        private static string GetNameTen(int number)
        {
            switch (number)
            {
                case 1:
                case 10:
                    return "dez";
                case 11:
                    return "onze";
                case 12:
                    return "doze";
                case 13:
                    return "treze";
                case 14:
                    return "quatorze";
                case 15:
                    return "quize";
                case 16:
                    return "dezesseis";
                case 17:
                    return "dezessete";
                case 18:
                    return "dezoito";
                case 19:
                    return "dezenove";
                case 20:
                    return "vinte";
                case 3:
                    return "trinta";
                case 4:
                    return "quarenta";
                case 5:
                    return "cinquenta";
                case 6:
                    return "sessenta";
                case 7:
                    return "setenta";
                case 8:
                    return "oitenta";
                case 9:
                    return "noventa";
                default:
                    return "zero";
            }
        }

        private static string GetNameHundred(int number, bool changeName)
        {
            switch (number)
            {
                case 1:
                    return changeName ? "cento" : "cem";
                case 2:
                    return "duzentos";
                case 3:
                    return "trezentos";
                case 4:
                    return "quatrocentos";
                case 5:
                    return "quinhentos";
                case 6:
                    return "seiscentos";
                case 7:
                    return "setecentos";
                case 8:
                    return "oitocentos";
                case 9:
                    return "novecentos";
                default:
                    return "zero";
            }
        }

        private static string GetCompleteNameByLength(string valueString)
        {
            var result = new StringBuilder();

            var lengthString = valueString.Length;

            if (lengthString == 3)
            {
                var changeNameOne = valueString.Substring(0, 1) == "1" && valueString.Substring(1, 1) != "0" && valueString.Substring(2, 1) != "0";

                result.Append(GetNameHundred(Convert.ToInt32(valueString.Substring(0, 1)), changeNameOne));

                if (valueString.Substring(1, 1) == "1")
                    result.Append(" e " + GetNameTen(Convert.ToInt32(valueString.Substring(1, 2))));
                else
                {
                    if (valueString.Substring(1, 1) != "0")
                        result.Append(" e " + GetNameTen(Convert.ToInt32(valueString.Substring(1, 1))));

                    if (valueString.Substring(2, 1) != "0")
                        result.Append(" e " + GetNameNumber(Convert.ToInt32(valueString.Substring(2, 1))));
                }
            }
            else if (lengthString == 2)
            {
                if (valueString.Substring(0, 1) == "1")
                    result.Append(GetNameTen(Convert.ToInt32(valueString.Substring(0, 2))));
                else
                {
                    result.Append(GetNameTen(Convert.ToInt32(valueString.Substring(0, 1))));

                    if (valueString.Substring(1, 1) != "0")
                        result.Append(" e " + GetNameNumber(Convert.ToInt32(valueString.Substring(1, 1))));
                }

            }
            else if (lengthString == 1)
            {
                if (valueString.Substring(0, 1) != "0")
                    result.Append(GetNameNumber(Convert.ToInt32(valueString.Substring(0, 1))));
            }

            return result.ToString();
        }
    }
}