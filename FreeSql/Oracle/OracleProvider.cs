﻿using FreeSql.Internal;
using FreeSql.Internal.CommonProvider;
using FreeSql.Oracle.Curd;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FreeSql.Oracle {

	class OracleProvider<TMark> : IFreeSql<TMark> {

		public ISelect<T1> Select<T1>() where T1 : class => new OracleSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
		public ISelect<T1> Select<T1>(object dywhere) where T1 : class => new OracleSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
		public IInsert<T1> Insert<T1>() where T1 : class => new OracleInsert<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression);
		public IInsert<T1> Insert<T1>(T1 source) where T1 : class => this.Insert<T1>().AppendData(source);
		public IInsert<T1> Insert<T1>(T1[] source) where T1 : class => this.Insert<T1>().AppendData(source);
		public IInsert<T1> Insert<T1>(IEnumerable<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
		public IUpdate<T1> Update<T1>() where T1 : class => new OracleUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
		public IUpdate<T1> Update<T1>(object dywhere) where T1 : class => new OracleUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
		public IDelete<T1> Delete<T1>() where T1 : class => new OracleDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
		public IDelete<T1> Delete<T1>(object dywhere) where T1 : class => new OracleDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);

		public IAdo Ado { get; }
		public IAop Aop { get; }
		public ICache Cache { get; }
		public ICodeFirst CodeFirst { get; }
		public IDbFirst DbFirst { get; }
		public OracleProvider(IDistributedCache cache, ILogger log, string masterConnectionString, string[] slaveConnectionString) {
			if (log == null) log = new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() }).CreateLogger("FreeSql.Oracle");

			this.InternalCommonUtils = new OracleUtils(this);
			this.InternalCommonExpression = new OracleExpression(this.InternalCommonUtils);

			this.Cache = new CacheProvider(cache, log);
			this.Ado = new OracleAdo(this.InternalCommonUtils, this.Cache, log, masterConnectionString, slaveConnectionString);
			this.Aop = new AopProvider();

			this.DbFirst = new OracleDbFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
			this.CodeFirst = new OracleCodeFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
		}

		internal CommonUtils InternalCommonUtils { get; }
		internal CommonExpression InternalCommonExpression { get; }

		public void Transaction(Action handler) => Ado.Transaction(handler);

		public void Transaction(Action handler, TimeSpan timeout) => Ado.Transaction(handler, timeout);
	}
}
