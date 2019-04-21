﻿using FreeSql.Internal;
using FreeSql.Internal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FreeSql.MySql.Curd {

	class MySqlSelect<T1> : FreeSql.Internal.CommonProvider.Select1Provider<T1> where T1 : class {

		internal static string ToSqlStatic(CommonUtils _commonUtils, string _select, bool _distinct, string field, StringBuilder _join, StringBuilder _where, string _groupby, string _having, string _orderby, int _skip, int _limit, List<SelectTableInfo> _tables, Func<Type, string, string> tableRuleInvoke, IFreeSql _orm) {
			if (_orm.CodeFirst.IsAutoSyncStructure)
				_orm.CodeFirst.SyncStructure(_tables.Select(a => a.Table.Type).ToArray());

			var sb = new StringBuilder();
			var sbnav = new StringBuilder();
			sb.Append(_select);
			if (_distinct) sb.Append("DISTINCT ");
			sb.Append(field).Append(" \r\nFROM ");
			var tbsjoin = _tables.Where(a => a.Type != SelectTableInfoType.From).ToArray();
			var tbsfrom = _tables.Where(a => a.Type == SelectTableInfoType.From).ToArray();
			for (var a = 0; a < tbsfrom.Length; a++) {
				sb.Append(_commonUtils.QuoteSqlName(tableRuleInvoke(tbsfrom[a].Table.Type, tbsfrom[a].Table.DbName))).Append(" ").Append(tbsfrom[a].Alias);
				if (tbsjoin.Length > 0) {
					//如果存在 join 查询，则处理 from t1, t2 改为 from t1 inner join t2 on 1 = 1
					for (var b = 1; b < tbsfrom.Length; b++) {
						sb.Append(" \r\nLEFT JOIN ").Append(_commonUtils.QuoteSqlName(tableRuleInvoke(tbsfrom[b].Table.Type, tbsfrom[b].Table.DbName))).Append(" ").Append(tbsfrom[b].Alias);
						if (string.IsNullOrEmpty(tbsfrom[b].NavigateCondition) && string.IsNullOrEmpty(tbsfrom[b].On)) sb.Append(" ON 1 = 1");
						else sb.Append(" ON ").Append(tbsfrom[b].NavigateCondition ?? tbsfrom[b].On);
					}
					break;
				} else {
					if (!string.IsNullOrEmpty(tbsfrom[a].NavigateCondition)) sbnav.Append(" AND (").Append(tbsfrom[a].NavigateCondition).Append(")");
					if (!string.IsNullOrEmpty(tbsfrom[a].On)) sbnav.Append(" AND (").Append(tbsfrom[a].On).Append(")");
				}
				if (a < tbsfrom.Length - 1) sb.Append(", ");
			}
			foreach (var tb in tbsjoin) {
				if (tb.Type == SelectTableInfoType.Parent) continue;
				switch (tb.Type) {
					case SelectTableInfoType.LeftJoin:
						sb.Append(" \r\nLEFT JOIN ");
						break;
					case SelectTableInfoType.InnerJoin:
						sb.Append(" \r\nINNER JOIN ");
						break;
					case SelectTableInfoType.RightJoin:
						sb.Append(" \r\nRIGHT JOIN ");
						break;
				}
				sb.Append(_commonUtils.QuoteSqlName(tableRuleInvoke(tb.Table.Type, tb.Table.DbName))).Append(" ").Append(tb.Alias).Append(" ON ").Append(tb.On ?? tb.NavigateCondition);
				if (!string.IsNullOrEmpty(tb.On) && !string.IsNullOrEmpty(tb.NavigateCondition)) sbnav.Append(" AND (").Append(tb.NavigateCondition).Append(")");
			}
			if (_join.Length > 0) sb.Append(_join);

			sbnav.Append(_where);
			foreach (var tb in _tables) {
				if (tb.Type == SelectTableInfoType.Parent) continue;
				if (string.IsNullOrEmpty(tb.Table.SelectFilter) == false)
					sbnav.Append(" AND (").Append(tb.Table.SelectFilter.Replace("a.", $"{tb.Alias}.")).Append(")");
			}
			if (sbnav.Length > 0) {
				sb.Append(" \r\nWHERE ").Append(sbnav.Remove(0, 5));
			}
			if (string.IsNullOrEmpty(_groupby) == false) {
				sb.Append(_groupby);
				if (string.IsNullOrEmpty(_having) == false)
					sb.Append(" \r\nHAVING ").Append(_having.Substring(5));
			}
			sb.Append(_orderby);
			if (_skip > 0 || _limit > 0)
				sb.Append(" \r\nlimit ").Append(Math.Max(0, _skip)).Append(",").Append(_limit > 0 ? _limit : -1);

			sbnav.Clear();
			return sb.ToString();
		}

		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override ISelect<T1, T2> From<T2>(Expression<Func<ISelectFromExpression<T1>, T2, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3> From<T2, T3>(Expression<Func<ISelectFromExpression<T1>, T2, T3, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4> From<T2, T3, T4>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5> From<T2, T3, T4, T5>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5, T6> From<T2, T3, T4, T5, T6>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, T6, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5, T6>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5, T6, T7> From<T2, T3, T4, T5, T6, T7>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, T6, T7, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5, T6, T7>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5, T6, T7, T8> From<T2, T3, T4, T5, T6, T7, T8>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, T6, T7, T8, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5, T6, T7, T8, T9> From<T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, T6, T7, T8, T9, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8, T9>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override ISelect<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> From<T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<ISelectFromExpression<T1>, T2, T3, T4, T5, T6, T7, T8, T9, T10, ISelectFromExpression<T1>>> exp) { this.InternalFrom(exp?.Body); var ret = new MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(_orm, _commonUtils, _commonExpression, null); MySqlSelect<T1>.CopyData(this, ret, exp?.Parameters); return ret; }
		public override string ToSql(string field = null) => ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2> : FreeSql.Internal.CommonProvider.Select2Provider<T1, T2> where T1 : class where T2 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3> : FreeSql.Internal.CommonProvider.Select3Provider<T1, T2, T3> where T1 : class where T2 : class where T3 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4> : FreeSql.Internal.CommonProvider.Select4Provider<T1, T2, T3, T4> where T1 : class where T2 : class where T3 : class where T4 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5> : FreeSql.Internal.CommonProvider.Select5Provider<T1, T2, T3, T4, T5> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5, T6> : FreeSql.Internal.CommonProvider.Select6Provider<T1, T2, T3, T4, T5, T6> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class where T6 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5, T6, T7> : FreeSql.Internal.CommonProvider.Select7Provider<T1, T2, T3, T4, T5, T6, T7> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class where T6 : class where T7 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8> : FreeSql.Internal.CommonProvider.Select8Provider<T1, T2, T3, T4, T5, T6, T7, T8> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class where T6 : class where T7 : class where T8 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8, T9> : FreeSql.Internal.CommonProvider.Select9Provider<T1, T2, T3, T4, T5, T6, T7, T8, T9> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class where T6 : class where T7 : class where T8 : class where T9 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
	class MySqlSelect<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : FreeSql.Internal.CommonProvider.Select10Provider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> where T1 : class where T2 : class where T3 : class where T4 : class where T5 : class where T6 : class where T7 : class where T8 : class where T9 : class where T10 : class {
		public MySqlSelect(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) { }
		public override string ToSql(string field = null) => MySqlSelect<T1>.ToSqlStatic(_commonUtils, _select, _distinct, field ?? this.GetAllFieldExpressionTree().Field, _join, _where, _groupby, _having, _orderby, _skip, _limit, _tables, TableRuleInvoke, _orm);
	}
}
