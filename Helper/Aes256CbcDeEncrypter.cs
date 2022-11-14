using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Aes256CbcDeEncrypter
    {
        public string eKey = "";
        public string eIv = "";

        public static string Encrypt(string prm_text_to_encrypt)
        {
            var rj = new RijndaelManaged()
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256,
            };


            string prm_key = "npvTlXjzfey/F4U8/gjP1P2L8eym7tjpraTJ9y3OFuY=";
            string prm_iv = "3yXL/1X5suAENTsSsO7m/oslmrpedkAs6nvdOrI58dw=";

            var sToEncrypt = prm_text_to_encrypt;


            var key = Convert.FromBase64String(prm_key);
            var IV = Convert.FromBase64String(prm_iv);

            var encryptor = rj.CreateEncryptor(key, IV);

            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            var toEncrypt = Encoding.ASCII.GetBytes(sToEncrypt);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encrypted = msEncrypt.ToArray();

            return (Convert.ToBase64String(encrypted));
        }

        public static string Decrypt(string prm_text_to_decrypt)
        {
            try
            {
                string prm_key = "npvTlXjzfey/F4U8/gjP1P2L8eym7tjpraTJ9y3OFuY=";
                string prm_iv = "3yXL/1X5suAENTsSsO7m/oslmrpedkAs6nvdOrI58dw=";

                var sEncryptedString = prm_text_to_decrypt;

                var rj = new RijndaelManaged()
                {
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC,
                    KeySize = 256,
                    BlockSize = 256,
                };

                var key = Convert.FromBase64String(prm_key);
                var IV = Convert.FromBase64String(prm_iv);

                var decryptor = rj.CreateDecryptor(key, IV);

                var sEncrypted = Convert.FromBase64String(sEncryptedString);

                var fromEncrypt = new byte[sEncrypted.Length];

                var msDecrypt = new MemoryStream(sEncrypted);
                var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                return (Encoding.ASCII.GetString(fromEncrypt));
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
