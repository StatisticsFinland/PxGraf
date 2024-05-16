using PxGraf.Models.SavedQueries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.TestDummies
{
    public class SqFileInterfaceDummy : ISqFileInterface
    {
        readonly IReadOnlyDictionary<string, SavedQuery> _queries;

        public SqFileInterfaceDummy(IReadOnlyDictionary<string, SavedQuery> availableQueries)
        {
            _queries = availableQueries;
        }

        public bool SavedQueryExists(string id, string savedQueryDirectory)
        {
            return _queries.ContainsKey(id);
        }

        public bool ArchiveCubeExists(string id, string archiveDirectory)
        {
            return _queries.ContainsKey(id);
        }

        public Task<SavedQuery> ReadSavedQueryFromFile(string id, string savedQueryDirectory)
        {
            var task = new Task<SavedQuery>(() => _queries[id]);
            task.Start();
            return task;
        }

        public Task<ArchiveCube> ReadArchiveCubeFromFile(string id, string archiveDirectory)
        {
            throw new NotImplementedException();
        }

        public async Task SerializeToFile(string fileName, string filePath, object input)
        {
            await Task.Factory.StartNew(() => { /* some delay to simulate writing */ Thread.Sleep(10); });
        }
    }
}
