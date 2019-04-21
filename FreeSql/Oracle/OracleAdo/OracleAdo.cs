﻿using FreeSql.Internal;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using SafeObjectPool;
using System;
using System.Collections;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace FreeSql.Oracle {
	class OracleAdo : FreeSql.Internal.CommonProvider.AdoProvider {
		public OracleAdo() : base(null, null, DataType.Oracle) { }
		public OracleAdo(CommonUtils util, ICache cache, ILogger log, string masterConnectionString, string[] slaveConnectionStrings) : base(cache, log, DataType.Oracle) {
			base._util = util;
			if (!string.IsNullOrEmpty(masterConnectionString))
				MasterPool = new OracleConnectionPool("主库", masterConnectionString, null, null);
			if (slaveConnectionStrings != null) {
				foreach (var slaveConnectionString in slaveConnectionStrings) {
					var slavePool = new OracleConnectionPool($"从库{SlavePools.Count + 1}", slaveConnectionString, () => Interlocked.Decrement(ref slaveUnavailables), () => Interlocked.Increment(ref slaveUnavailables));
					SlavePools.Add(slavePool);
				}
			}
		}
		static DateTime dt1970 = new DateTime(1970, 1, 1);
		public override object AddslashesProcessParam(object param) {
			if (param == null) return "NULL";
			if (param is bool || param is bool?)
				return (bool)param ? 1 : 0;
			else if (param is string || param is char)
				return string.Concat("'", param.ToString().Replace("'", "''"), "'");
			else if (param is Enum)
				return ((Enum)param).ToInt64();
			else if (decimal.TryParse(string.Concat(param), out var trydec))
				return param;
			else if (param is DateTime || param is DateTime?)
				return string.Concat("to_timestamp('", ((DateTime)param).ToString("yyyy-MM-dd HH:mm:ss.ffffff"), "','YYYY-MM-DD HH24:MI:SS.FF6')");
			else if (param is TimeSpan || param is TimeSpan?)
				return $"numtodsinterval({((TimeSpan)param).Ticks * 1.0 / 10000000},'second')";
			else if (param is IEnumerable) {
				var sb = new StringBuilder();
				var ie = param as IEnumerable;
				foreach (var z in ie) sb.Append(",").Append(AddslashesProcessParam(z));
				return sb.Length == 0 ? "(NULL)" : sb.Remove(0, 1).Insert(0, "(").Append(")").ToString();
			}
			return string.Concat("'", param.ToString().Replace("'", "''"), "'");
			//if (param is string) return string.Concat('N', nparms[a]);
		}

		protected override DbCommand CreateCommand() {
			return new OracleCommand();
		}

		protected override void ReturnConnection(ObjectPool<DbConnection> pool, Object<DbConnection> conn, Exception ex) {
			(pool as OracleConnectionPool).Return(conn, ex);
		}

		protected override DbParameter[] GetDbParamtersByObject(string sql, object obj) => _util.GetDbParamtersByObject(sql, obj);
	}
}
