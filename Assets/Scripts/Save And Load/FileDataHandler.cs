using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string codeWord = "alexdev";

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        this.dataDirPath = _dataDirPath;
        this.dataFileName = _dataFileName;
        this.encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataStore = JsonUtility.ToJson(_data, true);

            if(encryptData)
                dataStore = EncryptDecrypt(dataStore);

            using (FileStream stream = new FileStream(fullPath,FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataStore);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError("error on trying to save data to file : " + fullPath + "\n" + e);
        }

    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadData = null;

        if(File.Exists(fullPath))
        {
            try
            {
                string dataLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataLoad = reader.ReadToEnd();
                    }
                }

                if (encryptData)
                    dataLoad = EncryptDecrypt(dataLoad);

                loadData = JsonUtility.FromJson<GameData>(dataLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("error on trying to load data to file : " + fullPath + "\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if(File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedDate = "";

        for(int i = 0; i < _data.Length; i++)
        {
            modifiedDate += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedDate;
    }
}
