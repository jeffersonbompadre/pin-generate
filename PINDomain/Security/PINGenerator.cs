using PINDomain.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PINDomain.Security
{
    /// <summary>
    /// * O PIN é formado por:
    /// * AA  - Ano
    /// * MM  - Mês
    /// * DD  - Dia
    /// * HH  - Hora
    /// * MM  - Minuto
    /// * SS  - Segundo
    /// * N   - Randomico de 1 a 9
    /// * NNN - Valor da tabela, de acordo com o número Randomico
    /// * N   - Dígito verificador
    /// </summary>
    public class PINGenerator : IPINGenerator
    {
        readonly ISpreadSheet spreadSheet;
        readonly string key = "E546C8DF278CD5931069B522E695D4F2";

        public PINGenerator(ISpreadSheet spreadSheet)
        {
            this.spreadSheet = spreadSheet;
        }

        public string GetPIN()
        {
            var dateTime = DateTime.Now;
            var strResult = new StringBuilder();
            strResult.Append(GetFormatedDate(dateTime));
            strResult.Append(GetRandomValue(dateTime.Second));
            strResult.Append(CalcDigitVerificator(strResult.ToString()));
            return EncryptString(StringToHexa(strResult.ToString()), key);
        }

        public bool PINIsValid(string pin)
        {
            try
            {
                if (string.IsNullOrEmpty(pin))
                    return false;
                var decryptyPIN = HexaToString(DecryptString(pin, key));
                var isValid =
                    !IsExpired(decryptyPIN) &&
                    TableNumberIsValid(decryptyPIN) &&
                    DigitVerifiatorIsValid(decryptyPIN);
                return isValid;
            }
            catch
            {
                return false;
            }
        }

        #region Aux Methods

        string GetFormatedDate(DateTime dateTime) =>
            dateTime.ToString("yyMMddHHmmss");

        string GetRandomValue(int second)
        {
            var sequence = new Random().Next(1, 9);
            var valueTable = ((int)spreadSheet.GetValueSpreadSheet(sequence, second)).ToString("000");
            return $"{sequence}{valueTable}";
        }

        string CalcDigitVerificator(string strValue)
        {
            var intWeights = new int[] { 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            var intSum = 0;
            var intIdx = 0;
            for (int intPos = strValue.Length - 1; intPos >= 0; intPos--)
            {
                intSum += int.Parse(strValue[intPos].ToString()) * intWeights[intIdx];
                intIdx++;
            }
            intSum %= 10;
            intSum = 10 - intSum;
            if (intSum == 10)
                intSum = 0;
            return intSum.ToString();
        }

        string StringToHexa(string strValue) =>
            Convert.ToString(long.Parse(strValue), 16);

        string HexaToString(string strHexaValue) =>
            Convert.ToUInt64(strHexaValue, 16).ToString("00000000000000000");

        string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);
            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }
            var iv = aesAlg.IV;
            var decryptedContent = msEncrypt.ToArray();
            var result = new byte[iv.Length + decryptedContent.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
            return Convert.ToBase64String(result);
        }

        string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var iv = new byte[16];
            var cipher = new byte[16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);
            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }
            return result;
        }

        bool IsExpired(string pin)
        {
            // 012345678901 
            // AAMMDDHHMMSS
            var century = DateTime.Now.ToString("yyyy").Substring(0, 2);
            var year = int.Parse($"{century}{pin.Substring(0, 2)}");
            var month = int.Parse(pin.Substring(2, 2));
            var day = int.Parse(pin.Substring(4, 2));
            var hour = int.Parse(pin.Substring(6, 2));
            var minute = int.Parse(pin.Substring(8, 2));
            var second = int.Parse(pin.Substring(10, 2));
            var pinDate = new DateTime(year, month, day, hour, minute, second);
            return DateTime.Now > pinDate.AddSeconds(30);
        }

        bool TableNumberIsValid(string pin)
        {
            // 0123456789012345 
            // AAMMDDHHMMSSNNNN
            var second = int.Parse(pin.Substring(10, 2));
            var sequence = int.Parse(pin.Substring(12, 1));
            var valueTable = int.Parse(pin.Substring(13, 3));
            return valueTable == (int)spreadSheet.GetValueSpreadSheet(sequence, second);
        }

        bool DigitVerifiatorIsValid(string pin)
        {
            // 01234567890123456 
            // AAMMDDHHMMSSNNNNN
            var digit = pin.Substring(16, 1);
            return digit == CalcDigitVerificator(pin.Substring(0, 16));
        }

        #endregion
    }
}
