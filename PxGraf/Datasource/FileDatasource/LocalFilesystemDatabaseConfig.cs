﻿using System.Text;

namespace PxGraf.Datasource.DatabaseConnection
{
    public class LocalFilesystemDatabaseConfig(bool enabled, string databaseRootPath, Encoding encoding)
    {
        public bool Enabled { get; } = enabled;
        public string DatabaseRootPath { get; } = databaseRootPath;

        public Encoding Encoding { get; } = encoding;
    }
}