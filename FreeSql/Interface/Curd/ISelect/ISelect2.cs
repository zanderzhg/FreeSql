﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeSql {
	public interface ISelect<T1, T2> : ISelect0<ISelect<T1, T2>, T1> where T1 : class where T2 : class {

		bool Any(Expression<Func<T1, T2, bool>> exp);
		Task<bool> AnyAsync(Expression<Func<T1, T2, bool>> exp);

		DataTable ToDataTable<TReturn>(Expression<Func<T1, T2, TReturn>> select);
		Task<DataTable> ToDataTableAsync<TReturn>(Expression<Func<T1, T2, TReturn>> select);

		List<TReturn> ToList<TReturn>(Expression<Func<T1, T2, TReturn>> select);
		Task<List<TReturn>> ToListAsync<TReturn>(Expression<Func<T1, T2, TReturn>> select);
		string ToSql<TReturn>(Expression<Func<T1, T2, TReturn>> select);

		TReturn ToAggregate<TReturn>(Expression<Func<ISelectGroupingAggregate<T1>, ISelectGroupingAggregate<T2>, TReturn>> select);
		Task<TReturn> ToAggregateAsync<TReturn>(Expression<Func<ISelectGroupingAggregate<T1>, ISelectGroupingAggregate<T2>, TReturn>> select);

		TMember Sum<TMember>(Expression<Func<T1, T2, TMember>> column);
		Task<TMember> SumAsync<TMember>(Expression<Func<T1, T2, TMember>> column);
		TMember Min<TMember>(Expression<Func<T1, T2, TMember>> column);
		Task<TMember> MinAsync<TMember>(Expression<Func<T1, T2, TMember>> column);
		TMember Max<TMember>(Expression<Func<T1, T2, TMember>> column);
		Task<TMember> MaxAsync<TMember>(Expression<Func<T1, T2, TMember>> column);
		TMember Avg<TMember>(Expression<Func<T1, T2, TMember>> column);
		Task<TMember> AvgAsync<TMember>(Expression<Func<T1, T2, TMember>> column);

		ISelect<T1, T2> Where(Expression<Func<T1, T2, bool>> exp);
		ISelect<T1, T2> WhereIf(bool condition, Expression<Func<T1, T2, bool>> exp);

		ISelectGrouping<TKey> GroupBy<TKey>(Expression<Func<T1, T2, TKey>> exp);

		ISelect<T1, T2> OrderBy<TMember>(Expression<Func<T1, T2, TMember>> column);
		ISelect<T1, T2> OrderByDescending<TMember>(Expression<Func<T1, T2, TMember>> column);
	}
}