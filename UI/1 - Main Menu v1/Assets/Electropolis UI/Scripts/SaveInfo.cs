using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class SaveInfo : MonoBehaviour
{
    public string name;
    public DateTime date;

    public DirectoryInfo save_directory;
    public FileInfo save_file;
    public FileInfo portrait;

    public SaveInfo(DirectoryInfo _save_directory, FileInfo _save_file, FileInfo _portrait)
    {
        save_directory =_save_directory;
        save_file = _save_file;
        portrait = _portrait;

        name = Path.GetFileNameWithoutExtension(save_file.Name);
        date = save_file.LastWriteTime;
    }

    /// This functions exist because we cannot replace directly one component in Unity (component = new_component),
    /// instead we need to change the values with a function AFAIK
    public void replace_values(SaveInfo other)
    {
        save_directory = other.save_directory;
        save_file = other.save_file;
        portrait = other.portrait;

        name = other.name;
        date = other.date;
    }

    public static SaveInfo[] get_stored_saves_info()
    {

        DirectoryInfo stored_saves_directory = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] saves = stored_saves_directory.GetDirectories();

        List<SaveInfo> stored_saves = new List<SaveInfo>();

        foreach(DirectoryInfo save_dir in saves)
        {
            FileInfo[] json_files = save_dir.GetFiles("*.json");
            if (json_files.Length == 0)
                continue;
            FileInfo json_file = json_files[0];

            FileInfo[] portrait_files = save_dir.GetFiles("portrait.*");
            FileInfo portrait_file = (portrait_files.Length != 0) ? portrait_files[0] : null;

            SaveInfo save_info = new SaveInfo(save_dir, json_file, portrait_file);

            stored_saves.Add(save_info);
        }

        stored_saves.OrderByDescending(save => save.date);
        return stored_saves.ToArray();
    }
}
