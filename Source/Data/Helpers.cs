using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VMS.Error;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;
using SoapSSO;
using static SoapSSO.SSOWSSoapClient;
using Newtonsoft.Json;
using VMS.Entities;

namespace VMS.Data
{
    public class Settings
    {
        private static IDictionary env;
                
        internal static string AppSettingValue(string name, string key, string defValue = "")
        {
            if (Environment.GetEnvironmentVariables() != null)
            {
                env = Environment.GetEnvironmentVariables();
            }

            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile($"appsettings.json")
                .Build();

            if (configuration.GetSection(name)[key] == null)
            {
                return defValue;
            }
            else
            {
                return configuration.GetSection(name)[key] ?? defValue;

            }
        }

        internal static T AppSettingValue<T>(string name, string key, T defValue = default)
        {
            if (Environment.GetEnvironmentVariables() != null)
            {
                env = Environment.GetEnvironmentVariables();
            }
                        
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(projectPath)
                    .AddJsonFile($"appsettings.json")
                    .Build();

                return configuration.GetSection($"{name}:{key}").Get<T>();
            }
            catch (Exception)
            {
                return defValue;
            }
        }
                
    }

    public static class ExtensionClass
    {        
        public static string ToEncryptString(this string? value)
        {
            if (value == null) return "";

            var Key = Settings.AppSettingValue("AppSettings", "Secret", null);

            byte[] bytesBuff = Encoding.Unicode.GetBytes(value);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    value = Convert.ToBase64String(mStream.ToArray());
                }
            }

            return value;
      
        }

        public static string ToDecryptString(this string? value)
        {
            if (value == null) return "";

            var Key = Settings.AppSettingValue("AppSettings", "Secret", null);
            byte[] cipherBytes = Convert.FromBase64String(value);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    value= Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return value;
        }

        public static string EncryptString(this string? value)
        {
            if (value == null) return "";

            var KeyString = Settings.AppSettingValue("AppSettings", "AES-256-CBC", null);
            var IVString = Others.RandomString(16);

            byte[] Key = ASCIIEncoding.UTF8.GetBytes(KeyString);
            byte[] IV = ASCIIEncoding.UTF8.GetBytes(IVString);

            string encrypted = null;
            RijndaelManaged rj = new RijndaelManaged
            {
                Key = Key,
                IV = IV,
                Mode = CipherMode.CBC
            };

            try
            {
                MemoryStream ms = new MemoryStream();

                using (CryptoStream cs = new CryptoStream(ms, rj.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(value);
                        sw.Close();
                    }
                    cs.Close();
                }
                byte[] encoded = ms.ToArray();
                encrypted = Convert.ToBase64String(encoded);

                ms.Close();
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("A file error occurred: {0}", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
            }
            finally
            {
                rj.Clear();
            }
            return encrypted + "." + IVString;
        }

        public static string DecryptString(this string? value)
        {
            if (value == null) return "";

            var KeyString = Settings.AppSettingValue("AppSettings", "AES-256-CBC", null);
            var IVString = value.Split(".")[1];
            value= value.Split(".")[0];

            byte[] key = ASCIIEncoding.UTF8.GetBytes(KeyString);
            byte[] iv = ASCIIEncoding.UTF8.GetBytes(IVString);

            try
            {
                using var rj =
                       new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC };
                using var memoryStream =
                       new MemoryStream(Convert.FromBase64String(value));
                using var cryptoStream =
                       new CryptoStream(memoryStream,
                           rj.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read);
                return new StreamReader(cryptoStream).ReadToEnd();
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        public static float ToFloat(this object value) 
        {
            return Convert.ToSingle(value);
        }

        public static string ToFormatIDStringDate(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToString(
                "dd MMMM yyyy HH:mm:ss");
        }
        public static string ToFormatIDStringDate(this DateTime value)
        {
            return value.ToString(
                "dd MMMM yyyy HH:mm:ss");
        }
        public static string ToFormatIDStringDateShort(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToString(
                "dd/MM/yyyy");
        }
        public static string ToFormatIDStringDateShort(this DateTime value)
        {
            return value.ToString(
                "dd/MM/yyyy");
        }
        public static string ToFormatUSStringDate(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToString(
                "yyyy/MM/dd HH:mm:ss",
                new System.Globalization.CultureInfo("es-ES"));
        }
        public static string ToFormatUSStringDate(this DateTime value)
        {
            return value.ToString(
                "yyyy/MM/dd HH:mm:ss",
                new System.Globalization.CultureInfo("es-ES"));
        }
        public static string ToFormatUSStringDateShort(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToString(
                "yyyy/MM/dd",
                new System.Globalization.CultureInfo("es-ES"));
        }
        public static string ToFormatUSStringDateShort(this DateTime value)
        {
            return value.ToString(
                "yyyy/MM/dd",
                new System.Globalization.CultureInfo("es-ES"));
        }

        public static GridLimit GridRequest(this string value) 
        {
            if (value == null) return null;
            return JsonConvert.DeserializeObject<GridLimit>(value);
            
        }
    }

    public class Others
    {
        private static readonly Random random = new Random();

        public static DateTime DateTimeConvertToZone(DateTime oriDateTime)
        {
            string TZFromCfg = Settings.AppSettingValue("AppSettings", "TimeZoneId", "SE Asia Standard Time");

            //TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById(TZFromCfg);
            TimeZoneInfo timeInfo = TZConvert.GetTimeZoneInfo(TZFromCfg);

            DateTime _datetimeZoned = TimeZoneInfo.ConvertTime(oriDateTime, TimeZoneInfo.Local, timeInfo);

            return _datetimeZoned;
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

    public class Requests
    {
        internal static IActionResult Response(ControllerBase Controller, ApiStatus statusCode, object dataValue, string msg)
        {
            var e = new ApiStatus(500);

            var _ = new
            {
                status = e.StatusCode,
                error = true,
                detail = "",
                message = e.StatusDescription,
                data = dataValue

            };

            if (statusCode.StatusCode != 200)
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = true,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue
                };
            }
            else
            {
                _ = new
                {
                    status = statusCode.StatusCode,
                    error = false,
                    detail = msg,
                    message = statusCode.StatusDescription,
                    data = dataValue

                };
            }

            if (statusCode.StatusCode >= 500 && statusCode.StatusCode < 600)
            {
                throw new Exception(msg);
            }

            return Controller.StatusCode(statusCode.StatusCode, _);
        }
    }

    public class StringHelpers
    {
        public static string PrepareJsonstring(object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public static bool IsDBNull(object str) => str == DBNull.Value;

        public static string ConvertString(object str)
        {
            try
            {
                if (!IsDBNull(str) && !string.IsNullOrEmpty(Convert.ToString(str)))
                    return Convert.ToString(str);
            }
            catch { }
            return string.Empty;
        }

        public static string ConvertString(object str, string replaceValueIfNull)
        {
            try
            {
                if (!IsDBNull(str) && !string.IsNullOrEmpty(Convert.ToString(str)))
                    return Convert.ToString(str);
            }
            catch { }
            return replaceValueIfNull;
        }

        public static string AddQuotedStr(string str)
            => string.Format("'{0}'", str.Replace("'", "''")).Trim();

        public static string NumberFormat(string input)
            => NumberFormat(Convert.ToDouble(string.IsNullOrEmpty(input) ? "0" : input));

        public static string NumberFormat(double input)
            => input == 0 ? "0" : input.ToString("#,#;(#,#)");

        public static string DecimalFormat(string input)
            => DecimalFormat(Convert.ToDouble(string.IsNullOrEmpty(input) ? "0" : input));

        public static string DecimalFormat(double input)
            => input == 0 ? "0" : input.ToString("#,#.00;(#,#.00)");

        public static string NumberCustomFormat(double input, string stringFormat)
            => input.ToString(stringFormat);

        public static string RemoveSpace(string input)
            => Regex.Replace(input, @"\s", "");

        public static string RemoveSpecialChar(string input)
            => Regex.Replace(input, @"[^0-9a-zA-Z]+", "");

        public static string ExtractValueByDelimiter(string input, string delimiterString, int index)
        {
            try
            {
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(delimiterString) && input.IndexOf(delimiterString) >= 0)
                {
                    string[] arrValues = input.Split(delimiterString.ToCharArray());

                    if (arrValues.Count() > index)
                    {
                        return Convert.ToString(arrValues.GetValue(index));
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public static string RemoveSpaceThenLower(string input)
            => Regex.Replace(input, @"\s", "").ToLower();

        public static string IntToRoman(int num)
        {
            string romanResult = string.Empty;
            string[] romanLetters = {
            "M",
            "CM",
            "D",
            "CD",
            "C",
            "XC",
            "L",
            "XL",
            "X",
            "IX",
            "V",
            "IV",
            "I"
            };
            int[] numbers = {
                1000,
                900,
                500,
                400,
                100,
                90,
                50,
                40,
                10,
                9,
                5,
                4,
                1
            };
            int i = 0;
            while (num != 0)
            {
                if (num >= numbers[i])
                {
                    num -= numbers[i];
                    romanResult += romanLetters[i];
                }
                else
                {
                    i++;
                }
            }
            return romanResult;
        }
    }

   
}
