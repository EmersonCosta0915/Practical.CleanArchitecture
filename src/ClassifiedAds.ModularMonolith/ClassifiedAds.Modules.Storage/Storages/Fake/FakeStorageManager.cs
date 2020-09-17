﻿using ClassifiedAds.Modules.Storage.DTOs;
using System.IO;
using System.Text;

namespace ClassifiedAds.Modules.Storage.Storages.Fake
{
    public class FakeStorageManager : IFileStorageManager
    {
        public void Create(FileEntryDTO fileEntry, MemoryStream stream)
        {
            fileEntry.FileLocation = "Fake.txt";
        }

        public void Delete(FileEntryDTO fileEntry)
        {
            // do nothing
        }

        public byte[] Read(FileEntryDTO fileEntry)
        {
            return Encoding.UTF8.GetBytes("The content is generated by Fake Storage Manager.");
        }
    }
}
