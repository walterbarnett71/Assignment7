using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBLibrary
{
	public class DataRecord
	{
		public ObjectId Id { get; set; }
		public string Value { get; set; }
	}

	public class MongoConnector : IDBConnector
	{
		private readonly IMongoDatabase _db;
		private readonly IMongoCollection<DataRecord> _collection;

		public MongoConnector(string connectionString)
		{
			var client = new MongoClient(connectionString);
			_db = client.GetDatabase("assignmentdb");
			_collection = _db.GetCollection<DataRecord>("records");
		}

		public bool Ping()
		{
			try
			{
				_db.RunCommand((Command<BsonDocument>)"{ ping: 1 }");
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void InsertData(List<string> values)
		{
			var docs = values.Select(v => new DataRecord { Value = v }).ToList();
			_collection.InsertMany(docs);
		}

		public string ReadData(int index)
		{
			var doc = _collection.Find(_ => true).Skip(index).Limit(1).FirstOrDefault();
			return doc?.Value;
		}
	}
}
