namespace ECDHAES256s
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Text;

    public class CNG
    {
        public CNG() { }
        public CNG (byte[] bKey)
        {
            bPublicKey = bKey;
        }
        public void Clean()
        {
            this.Alice = null;
            this.Bob = null;
            this.Iv = null;
            this.bPublicKey = null;
            this.EncryptedBytes = null;
            this.PlaintextBytes = null;
            this.Stream?.Dispose();
        }
        public CngKey CngKey;
        public ECDiffieHellmanCng Alice;
        public ECDiffieHellmanCng Bob;
        public Byte[] Key;
        public Byte[] Iv;
        public Byte[] PublicKey;
        public Byte[] bPublicKey;
        public Byte[] EncryptedBytes;
        public Byte[] PlaintextBytes;
        public Stream Stream;
    }
    public class AES
    {
        public static CNG Cng { get; set; }
        public static byte[] Key { get; private set; }
        public AES()
        {
            CNG c = new CNG();
            c.Key = Key = RijndaelManaged.Create().Key;
            Cng = c;
        }

        public static string Sha256(string input)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public static byte[] Sha256b(string input)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));
            return crypto;
        }
        public static Tuple<string, string> PassToKeyIV(string password)
        {
            var a = password;
            var mid = a.Length / 2;
            var take1 = a.Substring(0, mid);
            var take2 = a.Substring(mid, a.Length - mid);
            var h1 = Sha256(take1);
            var h2 = Sha256(take2);
            return new Tuple<string, string>(h1, h2);
        }
        public static Tuple<byte[], byte[]> PassToKeyIVb(string password)
        {
            var a = password;
            var mid = a.Length / 2;
            var take1 = a.Substring(0, mid);
            var take2 = a.Substring(mid, a.Length - mid);
            var h1 = Sha256b(take1);
            var h2 = Sha256b(take2);
            return new Tuple<byte[], byte[]>(h1, h2);
        }
        public static byte[] GetSalt(int maximumSaltLength = 32)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }

        public static CNG Encrypt(CNG c)
        {
            Encrypt(c.Key, c.PlaintextBytes, out c.EncryptedBytes, out c.Iv);
            c.PlaintextBytes = null;
            return c;
        }
        public static void Encrypt(Byte[] key, Byte[] plaintextMessage, out Byte[] encryptedMessage, out Byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                        cs.Close();
                        encryptedMessage = ciphertext.ToArray();
                    }
                }
            }
        }
        public static void Encrypt(Byte[] key, Stream plaintextMessage, out Byte[] encryptedMessage, out Byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        plaintextMessage.CopyTo(cs);
                        // cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                        cs.Close();
                        encryptedMessage = ciphertext.ToArray();
                    }
                }
            }
        }
        public static MemoryStream Encrypt(Byte[] key, Stream plaintextMessage, out Byte[] iv)
        {
            MemoryStream ms = new MemoryStream();

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        plaintextMessage.CopyTo(cs);
                        // cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                        cs.Close();
                        //ciphertext.CopyTo(ms);
                        var buf = ciphertext.ToArray();
                        ms.Write(buf, 0, buf.Length);
                    }

                }
            }

            return ms;

        }
        public static MemoryStream Encrypt(Byte[] key, Stream plaintextMessage, Byte[] iv)
        {
            MemoryStream ms = new MemoryStream();

            using (Aes aes = new AesCryptoServiceProvider())
            {
                var is1024 = aes.ValidKeySize(512);
                aes.Key = key;
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        plaintextMessage.CopyTo(cs);
                        // cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                        cs.Close();
                        //ciphertext.CopyTo(ms);
                        var buf = ciphertext.ToArray();
                        ms.Write(buf, 0, buf.Length);
                    }

                }
            }

            return ms;

        }
        public static byte[] Encrypt(Byte[] key, byte[] plaintextBytes, Byte[] iv)
        {
            byte[] encryptedMessage;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                var is1024 = aes.ValidKeySize(512);
                aes.Key = key;
                iv = aes.IV;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextBytes, 0, plaintextBytes.Length);
                        cs.Close();
                        encryptedMessage = ciphertext.ToArray();


                    }

                }
            }
            return encryptedMessage;


        }
        public static byte[] Encrypt(string password, byte[] plaintextBytes)
        {
            byte[] encryptedMessage;
            using (Aes aes = new AesCryptoServiceProvider())
            {

                var a = password;
                var mid = a.Length / 2;
                var take1 = a.Substring(0, mid);
                var take2 = a.Substring(mid, a.Length - mid);
                var ivHash = Sha256(take2);

                aes.Key = Sha256b(take1);
                aes.IV = Encoding.ASCII.GetBytes(ivHash.Substring(0, 16));
                aes.Padding = PaddingMode.ISO10126;

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextBytes, 0, plaintextBytes.Length);
                        cs.Close();
                        encryptedMessage = ciphertext.ToArray();


                    }

                }
            }
            return encryptedMessage;


        }
        public static Stream Encrypt(string password, Stream plaintextStream)
        {
            MemoryStream ms = new MemoryStream();
            using (Aes aes = new AesCryptoServiceProvider())
            {

                var a = password;
                var mid = a.Length / 2;
                var take1 = a.Substring(0, mid);
                var take2 = a.Substring(mid, a.Length - mid);
                var ivHash = Sha256(take2);

                aes.Key = Sha256b(take1);
                aes.IV = Encoding.ASCII.GetBytes(ivHash.Substring(0, 16));
                aes.Padding = PaddingMode.ISO10126;

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        plaintextStream.CopyTo(cs);
                        // cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                        cs.Close();
                        //ciphertext.CopyTo(ms);
                        var buf = ciphertext.ToArray();
                        ms.Write(buf, 0, buf.Length);
                    }

                }
            }
            return ms;


        }
        public static bool EncryptToFile(string source, string dest, string password)
        {
            if (password == "")
            {

                return false;
            }
            try
            {
                using (FileStream writer = new FileStream(dest, FileMode.Create))
                using (FileStream reader = new FileStream(source, FileMode.Open))
                {
                    var os = ECDHAES256s.AES.Encrypt(password, reader);
                    os.Position = 0;
                    os.CopyTo(writer);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool EncryptToFile(Stream source, string dest, string password)
        {
            if (password == "")
            {

                return false;
            }
            try
            {
                using (FileStream writer = new FileStream(dest, FileMode.Create))

                {
                    var os = ECDHAES256s.AES.Encrypt(password, source);
                    os.Position = 0;
                    os.CopyTo(writer);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static async Task<CNG> EncryptAsync(Byte[] key, Byte[] plaintextMessage)
        {
            CNG cng = new CNG();
            cng.Key = key;
            cng.PlaintextBytes = plaintextMessage;
            return await Task.Factory.StartNew(() =>
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    aes.Key = key;
                    cng.Iv = aes.IV;
                    aes.Padding = PaddingMode.PKCS7;
                    using (MemoryStream ciphertext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                            cs.Close();
                            cng.EncryptedBytes = ciphertext.ToArray();
                        }
                    }
                    return cng;
                }
            });
        }
        public static async Task<CNG> EncryptAsync(Byte[] key, Stream plaintextMessage)
        {
            CNG cng = new CNG();
            cng.Key = key;
            cng.Stream = plaintextMessage;
            return await Task.Factory.StartNew(() =>
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    aes.Key = key;
                    cng.Iv = aes.IV;
                    aes.Padding = PaddingMode.PKCS7;
                    using (MemoryStream ciphertext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            plaintextMessage.CopyTo(cs);
                            cs.Close();
                            cng.EncryptedBytes = ciphertext.ToArray();
                        }
                    }
                    return cng;
                }
            });
        }

        public static CNG Decrypt(CNG c)
        {
            Decrypt(out c.PlaintextBytes, c.EncryptedBytes, c.Iv, c.Key);
            c.EncryptedBytes = null;
            c.Iv = null;
            return c;
        }
        public static void Decrypt(out Byte[] plaintextBytes, byte[] encryptedBytes, byte[] iv, byte[] bkey)
        {

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bkey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cs.Close();
                        encryptedBytes = null;
                        plaintextBytes = plaintext.ToArray();
                    }
                }
            }
        }
        public static void Decrypt(out Byte[] plaintextBytes, Stream encryptedBytes, Byte[] iv, Byte[] bkey)
        {

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bkey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        encryptedBytes.CopyTo(cs);
                        cs.Close();
                        encryptedBytes = null;
                        plaintextBytes = plaintext.ToArray();
                    }
                }
            }
        }
        public static Stream Decrypt(Stream encryptedBytes, Byte[] iv, Byte[] bkey)
        {
            MemoryStream ms = new MemoryStream();
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bkey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        encryptedBytes.CopyTo(cs);
                        cs.Close();
                        encryptedBytes = null;
                        plaintext.CopyTo(ms);
                    }
                }
            }
            return ms;
        }
        public static byte[] Decrypt(byte[] encryptedBytes, byte[] iv, byte[] bkey)
        {
            byte[] plaintextBytes = null;
            try
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {

                    aes.Key = bkey;


                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;
                    // Decrypt the message
                    using (MemoryStream plaintext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                            cs.Close();
                            encryptedBytes = null;
                            plaintextBytes = plaintext.ToArray();
                        }
                    }

                }
            }
            catch { }

            return plaintextBytes;
        }
        public static byte[] Decrypt(byte[] encryptedBytes, string password)
        {
            byte[] plaintextBytes = null;
            try
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {

                    var a = password;
                    var mid = a.Length / 2;
                    var take1 = a.Substring(0, mid);
                    var take2 = a.Substring(mid, a.Length - mid);

                    aes.Key = Sha256b(take1);
                    var ivHash = Sha256(take2);
                    aes.IV = Encoding.ASCII.GetBytes(ivHash.Substring(0, 16));
                    aes.Padding = PaddingMode.ISO10126;
                    // Decrypt the message
                    using (MemoryStream plaintext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                            cs.Close();
                            encryptedBytes = null;
                            plaintextBytes = plaintext.ToArray();
                        }
                    }

                }
            }
            catch { }

            return plaintextBytes;
        }
        public static Stream Decrypt(Stream source, string password)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {

                    var a = password;
                    var mid = a.Length / 2;
                    var take1 = a.Substring(0, mid);
                    var take2 = a.Substring(mid, a.Length - mid);

                    aes.Key = Sha256b(take1);
                    var ivHash = Sha256(take2);
                    aes.IV = Encoding.ASCII.GetBytes(ivHash.Substring(0, 16));
                    aes.Padding = PaddingMode.ISO10126;
                    // Decrypt the message
                    using (MemoryStream plaintext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            source.CopyTo(cs);
                            cs.Close();

                            var buf = plaintext.ToArray();
                            ms.Write(buf, 0, buf.Length);
                        }
                    }

                }
            }
            catch { }

            return ms;
        }
        public static bool DecryptToFile(string source, string dest, string password)
        {
            if (password == "")
            {
                return false;
            }
            try
            {
                using (FileStream writer = new FileStream(dest, FileMode.Create))
                using (FileStream reader = new FileStream(source, FileMode.Open))
                {
                    var os = ECDHAES256s.AES.Decrypt(reader, password);
                    os.Position = 0;
                    os.CopyTo(writer);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static async Task<CNG> DecryptAsync(Byte[] encryptedBytes, Byte[] iv, Byte[] bkey)
        {
            CNG cng = new CNG();
            cng.Key = bkey;
            cng.Iv = iv;
            cng.EncryptedBytes = encryptedBytes;
            return await Task.Factory.StartNew(() =>
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    aes.Key = cng.Key;
                    aes.IV = cng.Iv;
                    aes.Padding = PaddingMode.PKCS7;
                    // Decrypt the message
                    using (MemoryStream plaintext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                            cs.Close();
                            encryptedBytes = null;
                            cng.PlaintextBytes = plaintext.ToArray();
                        }
                    }
                }
                return cng;
            });
        }
        public static async Task<CNG> DecryptAsync(Stream encryptedStream, Byte[] iv, Byte[] bkey)
        {
            CNG cng = new CNG();
            cng.Key = bkey;
            cng.Iv = iv;
            cng.Stream = encryptedStream;
            return await Task.Factory.StartNew(() =>
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    aes.Key = cng.Key;
                    aes.IV = cng.Iv;
                    aes.Padding = PaddingMode.PKCS7;
                    // Decrypt the message
                    using (MemoryStream plaintext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            encryptedStream.CopyTo(cs);
                            cs.Close();
                            cng.PlaintextBytes = plaintext.ToArray();
                        }
                    }
                }
                return cng;
            });
        }

    }
    public static class DH
    {        
        public static CNG A(CNG c)
        {
            if (c.Alice == null)
            {
                c.Alice = new ECDiffieHellmanCng();

                c.Alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                c.Alice.HashAlgorithm = CngAlgorithm.Sha256;
                c.PublicKey = c.Alice.PublicKey.ToByteArray();
                c.EncryptedBytes = null;
                c.Iv = null;
                c.PlaintextBytes = null;
                return c;

            }
            if (c.Alice != null)
            {
                try
                {
                    c.CngKey = CngKey.Import(c.bPublicKey, CngKeyBlobFormat.EccPublicBlob);
                    c.Key = c.Alice.DeriveKeyMaterial(CngKey.Import(c.bPublicKey, CngKeyBlobFormat.EccPublicBlob));
                    c.Iv = null;
                    //c.publicKey = null;
                    c.bPublicKey = null;
                    //c.bob = null;
                    c.EncryptedBytes = null;
                    c.PlaintextBytes = null;
                    //c.alice = null;
                    return c;
                }
                catch (Exception) { return c; }

            }

            c.Iv = null;
            //c.publicKey = null;
            c.bPublicKey = null;
            //c.bob = null;
            c.EncryptedBytes = null;
            c.PlaintextBytes = null;
            //c.alice = null;
            return c;
        }
        public static CNG B(CNG c)
        {
            if (c.Bob == null)
            {
                c.Bob = new ECDiffieHellmanCng();

                c.Bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                c.Bob.HashAlgorithm = CngAlgorithm.Sha256;
                c.PublicKey = c.Bob.PublicKey.ToByteArray();
                //c.cngkey = CngKey.Import(c.bpublicKey, CngKeyBlobFormat.EccPublicBlob);
                c.Key = c.Bob.DeriveKeyMaterial(CngKey.Import(c.bPublicKey, CngKeyBlobFormat.EccPublicBlob));
                c.Iv = null;
                c.bPublicKey = null;
                //c.bob = null;
                c.EncryptedBytes = null;
                c.PlaintextBytes = null;
                // c.alice = null;
                return c;

            }
            if (c.Bob != null)
            {

                c.CngKey = CngKey.Import(c.bPublicKey, CngKeyBlobFormat.EccPublicBlob);
                //c.bcngkey = c.cngkey;
                c.Key = c.Bob.DeriveKeyMaterial(CngKey.Import(c.bPublicKey, CngKeyBlobFormat.EccPublicBlob));


                c.EncryptedBytes = null;
                c.Iv = null;
                //c.publicKey = null;
                c.bPublicKey = null;
                //c.bob = null;
                c.PlaintextBytes = null;
                //c.alice = null;
                return c;

            }
            c.EncryptedBytes = null;
            c.Iv = null;
            //c.publicKey = null;
            c.bPublicKey = null;
            //c.bob = null;
            c.EncryptedBytes = null;
            c.PlaintextBytes = null;
            //c.alice = null;
            return c;
        }
   
    }
}
