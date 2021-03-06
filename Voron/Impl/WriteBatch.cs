﻿namespace Voron.Impl
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class WriteBatch
	{
		private readonly List<BatchOperation> _operations;

		public IReadOnlyCollection<BatchOperation> Operations
		{
			get
			{
				return _operations;
			}
		}

		public long Size
		{
			get
			{
				return _operations.Sum(x => x.Type == BatchOperationType.Add ? x.Value.Length + x.Key.Size : x.Key.Size);
			}
		}

		public WriteBatch()
		{
			_operations = new List<BatchOperation>();
		}

		public void Add(Slice key, Stream value, string treeName)
		{
			if (string.IsNullOrEmpty(treeName)) throw new ArgumentNullException("treeName");
			if (value == null) throw new ArgumentNullException("value");
			if (value.Length == 0)
				throw new ArgumentException("Cannot add empty value");
			if (value.Length > int.MaxValue)
				throw new ArgumentException("Cannot add a value that is over 2GB in size", "value");

			_operations.Add(new BatchOperation(key, value, treeName, BatchOperationType.Add));
		}

		public void Delete(Slice key, string treeName)
		{
			if (string.IsNullOrEmpty(treeName)) throw new ArgumentNullException("treeName");

			_operations.Add(new BatchOperation(key, null, treeName, BatchOperationType.Delete));
		}

		public class BatchOperation
		{
			public BatchOperation(Slice key, Stream value, string treeName, BatchOperationType type)
			{
				Key = key;
				Value = value;
				TreeName = treeName;
				Type = type;
			}

			public Slice Key { get; private set; }

			public Stream Value { get; private set; }

			public string TreeName { get; private set; }

			public BatchOperationType Type { get; private set; }
		}

		public enum BatchOperationType
		{
			Add,
			Delete
		}
	}
}