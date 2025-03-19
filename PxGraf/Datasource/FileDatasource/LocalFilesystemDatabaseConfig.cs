﻿using System.Text;

namespace PxGraf.Datasource.FileDatasource
{
    public class LocalFilesystemDatabaseConfig(bool enabled, string databaseRootPath, Encoding encoding, string[] databaseWhitelist)
    {
        public bool Enabled { get; } = enabled;
        public string DatabaseRootPath { get; } = databaseRootPath;

        public Encoding Encoding { get; } = encoding;
        public string[] DatabaseWhitelist { get; } = databaseWhitelist;
    }
}
