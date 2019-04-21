﻿using FreeSql.Internal.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeSql.Internal.CommonProvider {

	abstract class Select6Provider<T1, T2, T3, T4, T5, T6> : Select0Provider<ISelect<T1, T2, T3, T4, T5, T6>, T1>, ISelect<T1, T2, T3, T4, T5, T6>
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class {

		public Select6Provider(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere) : base(orm, commonUtils, commonExpression, dywhere) {
			if (_orm.CodeFirst.IsAutoSyncStructure) _orm.CodeFirst.SyncStructure(typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
			_tables.Add(new SelectTableInfo { Table = _commonUtils.GetTableByEntity(typeof(T2)), Alias = $"SP10b", On = null, Type = SelectTableInfoType.From });
			_tables.Add(new SelectTableInfo { Table = _commonUtils.GetTableByEntity(typeof(T3)), Alias = $"SP10c", On = null, Type = SelectTableInfoType.From });
			_tables.Add(new SelectTableInfo { Table = _commonUtils.GetTableByEntity(typeof(T4)), Alias = $"SP10d", On = null, Type = SelectTableInfoType.From });
			_tables.Add(new SelectTableInfo { Table = _commonUtils.GetTableByEntity(typeof(T5)), Alias = $"SP10e", On = null, Type = SelectTableInfoType.From });
			_tables.Add(new SelectTableInfo { Table = _commonUtils.GetTableByEntity(typeof(T6)), Alias = $"SP10f", On = null, Type = SelectTableInfoType.From });
		}

		TMember ISelect<T1, T2, T3, T4, T5, T6>.Avg<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return default(TMember);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalAvg<TMember>(column?.Body);
		}

		Task<TMember> ISelect<T1, T2, T3, T4, T5, T6>.AvgAsync<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return Task.FromResult(default(TMember));
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalAvgAsync<TMember>(column?.Body);
		}

		ISelectGrouping<TKey> ISelect<T1, T2, T3, T4, T5, T6>.GroupBy<TKey>(Expression<Func<T1, T2, T3, T4, T5, T6, TKey>> exp) {
			if (exp == null) return this.InternalGroupBy<TKey>(exp?.Body);
			for (var a = 0; a < exp.Parameters.Count; a++) _tables[a].Parameter = exp.Parameters[a];
			return this.InternalGroupBy<TKey>(exp?.Body);
		}

		TMember ISelect<T1, T2, T3, T4, T5, T6>.Max<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return default(TMember);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalMax<TMember>(column?.Body);
		}

		Task<TMember> ISelect<T1, T2, T3, T4, T5, T6>.MaxAsync<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return Task.FromResult(default(TMember));
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalMaxAsync<TMember>(column?.Body);
		}

		TMember ISelect<T1, T2, T3, T4, T5, T6>.Min<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return default(TMember);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalMin<TMember>(column?.Body);
		}

		Task<TMember> ISelect<T1, T2, T3, T4, T5, T6>.MinAsync<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) return Task.FromResult(default(TMember));
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalMinAsync<TMember>(column?.Body);
		}

		ISelect<T1, T2, T3, T4, T5, T6> ISelect<T1, T2, T3, T4, T5, T6>.OrderBy<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) this.InternalOrderBy(column?.Body);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalOrderBy(column?.Body);
		}

		ISelect<T1, T2, T3, T4, T5, T6> ISelect<T1, T2, T3, T4, T5, T6>.OrderByDescending<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) this.InternalOrderBy(column?.Body);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalOrderByDescending(column?.Body);
		}

		TMember ISelect<T1, T2, T3, T4, T5, T6>.Sum<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) this.InternalOrderBy(column?.Body);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalSum<TMember>(column?.Body);
		}

		Task<TMember> ISelect<T1, T2, T3, T4, T5, T6>.SumAsync<TMember>(Expression<Func<T1, T2, T3, T4, T5, T6, TMember>> column) {
			if (column == null) this.InternalOrderBy(column?.Body);
			for (var a = 0; a < column.Parameters.Count; a++) _tables[a].Parameter = column.Parameters[a];
			return this.InternalSumAsync<TMember>(column?.Body);
		}

		TReturn ISelect<T1, T2, T3, T4, T5, T6>.ToAggregate<TReturn>(Expression<Func<ISelectGroupingAggregate<T1>, ISelectGroupingAggregate<T2>, ISelectGroupingAggregate<T3>, ISelectGroupingAggregate<T4>, ISelectGroupingAggregate<T5>, ISelectGroupingAggregate<T6>, TReturn>> select) {
			if (select == null) return default(TReturn);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToAggregate<TReturn>(select?.Body);
		}

		Task<TReturn> ISelect<T1, T2, T3, T4, T5, T6>.ToAggregateAsync<TReturn>(Expression<Func<ISelectGroupingAggregate<T1>, ISelectGroupingAggregate<T2>, ISelectGroupingAggregate<T3>, ISelectGroupingAggregate<T4>, ISelectGroupingAggregate<T5>, ISelectGroupingAggregate<T6>, TReturn>> select) {
			if (select == null) return Task.FromResult(default(TReturn));
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToAggregateAsync<TReturn>(select?.Body);
		}

		List<TReturn> ISelect<T1, T2, T3, T4, T5, T6>.ToList<TReturn>(Expression<Func<T1, T2, T3, T4, T5, T6, TReturn>> select) {
			if (select == null) return this.InternalToList<TReturn>(select?.Body);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToList<TReturn>(select?.Body);
		}

		Task<List<TReturn>> ISelect<T1, T2, T3, T4, T5, T6>.ToListAsync<TReturn>(Expression<Func<T1, T2, T3, T4, T5, T6, TReturn>> select) {
			if (select == null) return this.InternalToListAsync<TReturn>(select?.Body);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToListAsync<TReturn>(select?.Body);
		}

		DataTable ISelect<T1, T2, T3, T4, T5, T6>.ToDataTable<TReturn>(Expression<Func<T1, T2, T3, T4, T5, T6, TReturn>> select) {
			if (select == null) return this.InternalToDataTable(select?.Body);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToDataTable(select?.Body);
		}

		Task<DataTable> ISelect<T1, T2, T3, T4, T5, T6>.ToDataTableAsync<TReturn>(Expression<Func<T1, T2, T3, T4, T5, T6, TReturn>> select) {
			if (select == null) return this.InternalToDataTableAsync(select?.Body);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToDataTableAsync(select?.Body);
		}

		string ISelect<T1, T2, T3, T4, T5, T6>.ToSql<TReturn>(Expression<Func<T1, T2, T3, T4, T5, T6, TReturn>> select) {
			if (select == null) return this.InternalToSql<TReturn>(select?.Body);
			for (var a = 0; a < select.Parameters.Count; a++) _tables[a].Parameter = select.Parameters[a];
			return this.InternalToSql<TReturn>(select?.Body);
		}

		ISelect<T1, T2, T3, T4, T5, T6> ISelect<T1, T2, T3, T4, T5, T6>.Where(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> exp) {
			if (exp == null) return this.Where(null);
			for (var a = 0; a < exp.Parameters.Count; a++) _tables[a].Parameter = exp.Parameters[a];
			return this.Where(_commonExpression.ExpressionWhereLambda(_tables, exp?.Body, null));
		}

		ISelect<T1, T2, T3, T4, T5, T6> ISelect<T1, T2, T3, T4, T5, T6>.WhereIf(bool condition, Expression<Func<T1, T2, T3, T4, T5, T6, bool>> exp) {
			if (condition == false || exp == null) return this;
			for (var a = 0; a < exp.Parameters.Count; a++) _tables[a].Parameter = exp.Parameters[a];
			return this.Where(_commonExpression.ExpressionWhereLambda(_tables, exp?.Body, null));
		}

		bool ISelect<T1, T2, T3, T4, T5, T6>.Any(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> exp) {
			if (exp == null) return this.Any();
			for (var a = 0; a < exp.Parameters.Count; a++) _tables[a].Parameter = exp.Parameters[a];
			return this.Where(_commonExpression.ExpressionWhereLambda(_tables, exp?.Body, null)).Any();
		}

		Task<bool> ISelect<T1, T2, T3, T4, T5, T6>.AnyAsync(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> exp) {
			if (exp == null) return this.AnyAsync();
			for (var a = 0; a < exp.Parameters.Count; a++) _tables[a].Parameter = exp.Parameters[a];
			return this.Where(_commonExpression.ExpressionWhereLambda(_tables, exp?.Body, null)).AnyAsync();
		}
	}
}