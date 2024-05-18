using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace ZipUtilityApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var zipUtility = new ZipUtility();
            zipUtility.ZipFiles("destination.zip", "password");
            zipUtility.UnZipFiles("output_directory");
        }
    }

    public class ZipUtility
    {
        public void ZipFiles(string destinationFileName, string password)
        {
            using (FileStream outputFileStream = new FileStream(destinationFileName, FileMode.Create))
            using (ZipOutputStream zipStream = new ZipOutputStream(outputFileStream))
            {
                bool isCrypted = !string.IsNullOrEmpty(password);
                if (isCrypted)
                {
                    zipStream.Password = password;
                }

                // Example of adding a single file
                string fileToAdd = "example.txt";
                using (FileStream inputStream = new FileStream(fileToAdd, FileMode.Open))
                {
                    var zipEntry = new ZipEntry(Path.GetFileName(fileToAdd))
                    {
                        IsCrypted = isCrypted,
                        CompressionMethod = CompressionMethod.Deflated
                    };
                    zipStream.PutNextEntry(zipEntry);
                    CopyStream(inputStream, zipStream);
                    zipStream.CloseEntry();
                }

                zipStream.Finish();
            }
        }

        public void UnZipFiles(string destinationDirectoryName)
        {
            using (ZipFile zipFile = new ZipFile("destination.zip"))
            {
                if (zipFile.Password != null)
                {
                    zipFile.Password = "password";
                }

                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (zipEntry.IsFile)
                    {
                        using (Stream inputStream = zipFile.GetInputStream(zipEntry))
                        using (FileStream fileStream = new FileStream(Path.Combine(destinationDirectoryName, zipEntry.Name), FileMode.Create))
                        {
                            CopyStream(inputStream, fileStream);
                        }
                    }
                }
            }
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
