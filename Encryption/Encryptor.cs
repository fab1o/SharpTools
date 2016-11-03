using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Fabio.SharpTools.Convertion;

namespace Fabio.SharpTools.Encryption
{
    /// <summary>
    /// Provides ways for encryption and decryption
    /// </summary>
    public sealed class Encryptor
    {
        private Encryptor() { }

        private const string DESKEY = "IKEYPASS"; //key for encryption
        private const string DESIV = "IVFORKEY"; //iv for encryption


        public static string DESEncrypt(object stringToEncrypt)
        {
            return DESEncrypt(Convert.ToString(stringToEncrypt));
        }
        /// <summary>
        /// Encrypt string using pre-defined key
        /// </summary>
        /// <param name="stringToEncrypt"></param>
        /// <returns></returns>
        public static string DESEncrypt(string stringToEncrypt)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
                return "";

            byte[] bKey = Convertor.ToByteArray(DESKEY);
            byte[] IV = Convertor.ToByteArray(DESIV);

            byte[] inputByteArray = Encoding.Unicode.GetBytes(stringToEncrypt);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            MemoryStream ms = new MemoryStream(); CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(bKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypt string using pre-defined key
        /// </summary>
        /// <param name="stringToDecrypt"></param>
        /// <returns></returns>
        public static string DESDecrypt(string stringToDecrypt)
        {
            if (string.IsNullOrEmpty(stringToDecrypt))
                return "";

            byte[] bKey = Convertor.ToByteArray(DESKEY);
            byte[] IV = Convertor.ToByteArray(DESIV);

            //int len = stringToDecrypt.Length;
            byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(bKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            Encoding encoding = Encoding.Unicode;

            return encoding.GetString(ms.ToArray());
        }

        public static string AESEncrypt(object stringToEncrypt)
        {
            return AESEncrypt(Convert.ToString(stringToEncrypt));
        }
        /// <summary>
        /// Use AES to encrypt data string.
        /// </summary>
        /// <param name="data">Clear string to encrypt.</param>
        /// <param name="password">Password used to encrypt the string.</param>
        /// <returns>Encrypted result as Base64 string.</returns>
        public static string AESEncrypt(string stringToEncrypt)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
                return "";

            byte[] encBytes;
            byte[] data = Encoding.UTF8.GetBytes(stringToEncrypt);

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(DESKEY, Encoding.UTF8.GetBytes(DESIV));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = PaddingMode.ISO10126;
            ICryptoTransform encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16));
            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                encStream.Write(data, 0, data.Length);
                encStream.FlushFinalBlock();
                encBytes = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encBytes);
        }

        /// <summary>
        /// Decrypt the data string to the original string.
        /// </summary>
        /// <returns>Decrypted string.</returns>
        public static string AESDecrypt(string stringToDecrypt)
        {
            if (string.IsNullOrEmpty(stringToDecrypt))
                return "";

            byte[] data = Convert.FromBase64String(stringToDecrypt);

            byte[] decBytes;

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(DESKEY, Encoding.UTF8.GetBytes(DESIV));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = PaddingMode.ISO10126;
            ICryptoTransform decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16));
            using (MemoryStream msDecrypt = new MemoryStream(data))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                // Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
                byte[] fromEncrypt = new byte[data.Length];
                // Read as many bytes as possible.
                int read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                if (read < fromEncrypt.Length)
                {
                    // Return a byte array of proper size.
                    byte[] clearBytes = new byte[read];
                    Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
                    decBytes = clearBytes;
                }
                else
                {
                    decBytes = fromEncrypt;
                }
            }

            return Encoding.UTF8.GetString(decBytes);
        }
    }
}
