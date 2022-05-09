using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EDTESP.Infrastructure.CC.Util
{
    public static class StringUtil
    {
        public static string ClearNumber(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var rgx = new Regex("[^0-9]");
            return rgx.Replace(str, "");
        }

        public static string CryptData(this string value, string key)
        {
            try
            {
                var objcriptografaSenha = new TripleDESCryptoServiceProvider();
                var objcriptoMd5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = key;

                byteHash = objcriptoMd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objcriptoMd5 = null;
                objcriptografaSenha.Key = byteHash;
                objcriptografaSenha.Mode = CipherMode.ECB;

                byteBuff = Encoding.ASCII.GetBytes(value);
                return Convert.ToBase64String(objcriptografaSenha.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Digite os valores Corretamente." + ex.Message;
            }
        }

        public static string UnCryptData(this string cipher, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(cipher))
                    return null;

                var objdescriptografaSenha = new TripleDESCryptoServiceProvider();
                var objcriptoMd5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = key;

                byteHash = objcriptoMd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objcriptoMd5 = null;
                objdescriptografaSenha.Key = byteHash;
                objdescriptografaSenha.Mode = CipherMode.ECB;

                byteBuff = Convert.FromBase64String(cipher);
                string strDecrypted = Encoding.ASCII.GetString(objdescriptografaSenha.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objdescriptografaSenha = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Digite os valores Corretamente." + ex.Message;
            }
        }

        public static string RandomString(int length = 64)
        {
            var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = string.Empty;

            var rn = new Random();
            while (rnd.Length < length)
            {
                rnd += str[rn.Next(str.Length-1)];
            }

            return rnd;
        }

        public static bool IsCnpj(this string cnpj)
        {
            var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            var tempCnpj = cnpj.Substring(0, 12);

            var soma = 0;
            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(this string cpf)
        {
            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto;

            return cpf.EndsWith(digito);
        }

        public static string FormataDocumento(this string doc, string tipo)
        {
            if (string.IsNullOrEmpty(doc))
                return doc;

            doc = doc.ClearNumber();

            if (tipo == "J")
                return $"{doc.Substring(0,2)}.{doc.Substring(2, 3)}.{doc.Substring(5, 3)}/{doc.Substring(8, 4)}-{doc.Substring(12)}";

            if (tipo == "F")
                return $"{doc.Substring(0,3)}{doc.Substring(3, 3)}{doc.Substring(6, 3)}-{doc.Substring(9)}";

            return string.Empty;
        }

        public static string FormataTel(this string tel)
        {
            if (string.IsNullOrEmpty(tel))
                return tel;

            tel = tel.ClearNumber();
            var ddd = tel.Substring(0, 2);
            var pre = tel.Substring(2, 4);
            var fim = tel.Substring(6);

            return $"({ddd}) {pre}-{fim}";
        }

        public static string FormataCel(this string tel)
        {
            if (string.IsNullOrEmpty(tel))
                return tel;

            tel = tel.ClearNumber();
            var ddd = tel.Substring(0, 2);
            var pre = tel.Substring(2, 5);
            var fim = tel.Substring(7);

            return $"({ddd}) {pre}-{fim}";
        }

        public static string FormataCep(this string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return cep;

            cep = cep.ClearNumber();
            var pre = cep.Substring(0, 5);
            var fim = cep.Substring(5);

            return $"{pre}-{fim}";
        }

        public static string MaxLength(this string value, int max = 30, bool leadWith = false, char chr = ' ')
        {
            var str = value ?? string.Empty;

            if(str.Length > max)
                str = str.Substring(0, max);

            if (leadWith)
                str = str.PadLeft(max, chr);

            return str;
        }

        public static Decimal StrNumToDecimal(this string num)
        {
            if (string.IsNullOrEmpty(num))
                return 0m;

            var dnum = Convert.ToDecimal(num);

            return dnum / 100;
        }
    }
}