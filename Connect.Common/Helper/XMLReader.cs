using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json;
namespace Connect.Common.Helper
{
    public class XMLReader
    {
        public static string Path = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath) + @"\";
        public static string fileName = Path + @"\Setting\SettingView.xml";
        public static string fileListColumn = "\\ListColumn.xml";
        public static string fileFactory = "\\Factory.cs";

        public XMLReader()
        {

        }
        public XMLReader(string path)
        {
            Path = path;
        }

        public IList<T> ReadXml<T>(string fileName)
        {
            try
            {
                var path = Path + fileName;
                if (File.Exists(path))
                {
                    FileStream stream = File.OpenRead(path);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    object dezerializedList = serializer.Deserialize(stream);
                    stream.Close();
                    stream.Dispose();
                    return (List<T>)dezerializedList;
                }
                else
                {
                    return null;
                }



            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public T ReadXmlInfo<T>(string fileName) where T : new()
        {
            try
            {
                FileStream stream = File.OpenRead(Path + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T dezerializedList = (T)serializer.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                return dezerializedList;
            }
            catch (Exception)
            {

                return new T();
            }
        }

        public T ReadJsonInfo<T>(string fileName) where T : class, new()
        {
            try
            {
                string jsonData = File.ReadAllText(Path + fileName);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public T PReadJsonInfo<T>(string fileName) where T : class, new()
        {
            try
            {
                string jsonData = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void WriteXml(string content, string fileName)
        {
            try
            {
                if (File.Exists(Path + fileName))
                {
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, content);
                    stream.Close();
                    stream.Dispose();
                }
                else
                {
                    //File.Create(Path + fileName);
                    CreateXml(fileName);
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, content);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception)
            {

            }

        }

        public void WriteXml<T>(IList<T> list, string fileName) where T : new()
        {
            try
            {
                if (File.Exists(Path + fileName))
                {
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(stream, list);
                    stream.Close();
                    stream.Dispose();
                }
                else
                {
                    File.Create(Path + fileName);
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(stream, list);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {

            }

        }
        public void WriteXmlList<T>(List<T> list, string fileName) where T : new()
        {
            try
            {
                if (File.Exists(Path + fileName))
                {
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(stream, list);
                    stream.Close();
                    stream.Dispose();
                }
                else
                {
                    File.Create(Path + fileName);
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(stream, list);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void WriteXmlInfo<T>(T info, string fileName) where T : new()
        {
            try
            {
                File.WriteAllText(Path + fileName, String.Empty);
                FileStream stream = File.OpenWrite(Path + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, info);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception)
            {

            }
        }

        public void WriteXmlInfoPath<T>(T info, string path) where T : new()
        {
            try
            {
                File.WriteAllText(path, String.Empty);
                FileStream stream = File.OpenWrite(path);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, info);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception)
            {

            }
        }

        public void CreateXml(string fileName)
        {
            try
            {
                if (!File.Exists(Path + fileName))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(Path + fileName))
                    {
                        sw.WriteLine("<?xml version='1.0'?> \n");
                        sw.WriteLine("<ArrayOfTableInfo xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'> \n");
                        sw.WriteLine("</ArrayOfTableInfo>");

                    }
                }

            }
            catch (Exception)
            {

            }

        }

        public void WriteXmlPath(string content, string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.WriteAllText(fileName, String.Empty);
                    FileStream stream = File.OpenWrite(fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, content);
                    stream.Close();
                }
                else
                {
                    CreateXmlPath(fileName);
                    File.WriteAllText(fileName, String.Empty);
                    FileStream stream = File.OpenWrite(fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, content);
                    stream.Close();
                }
            }
            catch (Exception)
            {

            }

        }

        public void CreateXmlPath(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine("<?xml version='1.0'?> \n");
                        sw.WriteLine("<ArrayOfTableInfo xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'> \n");
                        sw.WriteLine("</ArrayOfTableInfo>");
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        public void DeleteXml(string fileName)
        {
            try
            {
                if (File.Exists(Path + fileName))
                {
                    File.Delete(Path + fileName);
                }
            }
            catch (Exception)
            {

            }

        }


        public void SaveFilePath(string _result, string fileName)
        {
            try
            {
                File.WriteAllText(fileName, _result);
            }
            catch (Exception)
            {

            }

        }

        public static string EncryptionKey = "Thanhtin01282252279";

        public static string Encrypt(string clearText)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public void FloderExists(string floder)
        {
            if (!Directory.Exists(Path + floder))
            {
                Directory.CreateDirectory(Path + floder);
            }
        }
        public bool WriteTextEncrypt<T>(IList<T> list, string fileName) where T : new()
        {
            try
            {
                var content = JsonHelper.ListInfoToJson(list);
                if (File.Exists(Path + fileName))
                {
                    File.WriteAllText(Path + fileName, String.Empty);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, Encrypt(content));
                    stream.Close();
                    stream.Dispose();
                    return true;
                }
                else
                {
                    File.Create(Path + fileName);
                    FileStream stream = File.OpenWrite(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    serializer.Serialize(stream, Encrypt(content));
                    stream.Close();
                    stream.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public IList<T> ReadTextDecrypt<T>(string fileName) where T : new()
        {
            try
            {
                if (File.Exists(Path + fileName))
                {
                    FileStream stream = File.OpenRead(Path + fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(string));
                    string dezerializedList = "" + serializer.Deserialize(stream);
                    var list = JsonHelper.JsonToListInfo<T>(Decrypt(dezerializedList));
                    stream.Close();
                    stream.Dispose();
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<T>();
            }
            return new List<T>();
        }
    }

    public class DialogInfo
    {
        public string FileName { get; set; }
        public string PathFile { get; set; }
        public string FullPath { get; set; }
        public string Pathtemp { get; set; }
        public FileInfo File { get; set; }
        public DialogInfo()
        {
            FileName = "";
            PathFile = "";
            Pathtemp = "";
            FullPath = "";
        }
    }
}
