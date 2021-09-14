using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cassandra;
using Cassandra.Data.Linq;

namespace AkkaPlayground.Proto
{
    public class CassandraRepo<T>
    {
        private ISession _session;
        private Table<T> _master;

        public CassandraRepo()
        {
            ConnectOrCreateCluster();
            ConnectOrCreateTable();
        }

        private void ConnectOrCreateTable()
        {
            _master = new Table<T>(_session);
            _master.CreateIfNotExists();
        }

        private void ConnectOrCreateCluster()
        {
            var cluster =
                Cluster
                    .Builder()
                    .AddContactPoints("localhost")
                    .WithDefaultKeyspace("cPlusC")
                    .Build();
            _session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
        }

        public void Insert(IEnumerable<T> rows)
        {
            _session
                .CreateBatch()
                .Append(rows.Select(row => _master.Insert(row)))
                .Execute();
        }

        public IEnumerable<T> Load(Expression<Func<T, bool>> predicate)
        {
            return _master.Where(predicate).Execute();
        }
        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
