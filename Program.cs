using System.Security.Cryptography;
using System.Text;

namespace EversoulTBLDecryptor
{
    public class Program
    {
        private static readonly int tableVersion = 0;
        private static readonly string keyMagic = ""; // Find and put your magic key here

        public static void Main()
        {
            try
            {
                Console.WriteLine("Enter the table data directory: ");
                string tableDir = Console.ReadLine() ?? "";
                tableDir = tableDir.Trim('"');
                if (!Directory.Exists(tableDir))
                {
                    Console.WriteLine("Table data directory doesn't exist.");
                    Environment.Exit(0);
                }

                var rijDecryptor = GetRijDecryptor();
                string[] files = Directory.GetFiles(tableDir);
                foreach (string file in files)
                {
                    try
                    {
                        DecryptFile(file, rijDecryptor);
                        Console.WriteLine($"{file} decrypted");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to decrypt {file} because {e.Message}");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private static ICryptoTransform GetRijDecryptor()
        {
            string unhashKey = string.Concat(((int)(tableVersion ^ 0x80000000)).ToString(), keyMagic);
            byte[] unhashKeyBytes = Encoding.UTF8.GetBytes(unhashKey);
            byte[] hashKeyBytes = SHA256.HashData(unhashKeyBytes);
            byte[] key = new byte[16];
            Array.Copy(hashKeyBytes, key, 16);
#pragma warning disable SYSLIB0022 // Type or member is obsolete
            RijndaelManaged rijAlg = new()
            {
                KeySize = 128,
                Key = key,
                IV = key
            };
#pragma warning restore SYSLIB0022 // Type or member is obsolete
            return rijAlg.CreateDecryptor();
        }

        private static string ToHex(byte[] bytes)
        {
            StringBuilder result = new(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("x2"));
            return result.ToString();
        }

        private static string GetOutputFileName(string inputFile)
        {
            string? directory = Path.GetDirectoryName(inputFile);
            if (directory == null)
                throw new ArgumentNullException(nameof(inputFile));

            string outputDirectory = Path.Combine(directory, "decrypt");
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
            string fileExtension = ".bin";

            string outputFileName = $"{fileNameWithoutExtension}{fileExtension}";
            return Path.Combine(outputDirectory, outputFileName);
        }

        private static void DecryptFile(string inputFile, ICryptoTransform decryptor)
        {
            using FileStream inputFileStream = File.OpenRead(inputFile);
            using CryptoStream rijStream = new(inputFileStream, decryptor, CryptoStreamMode.Read);
            using FileStream outputFileStream = File.Open(GetOutputFileName(inputFile), FileMode.OpenOrCreate);

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = rijStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputFileStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}