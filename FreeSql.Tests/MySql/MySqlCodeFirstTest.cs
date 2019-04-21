using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FreeSql.Tests.MySql {
	public class MySqlCodeFirstTest {

		[Fact]
		public void AddField() {
			var sql = g.mysql.CodeFirst.GetComparisonDDLStatements<TopicAddField>();

			var id = g.mysql.Insert<TopicAddField>().AppendData(new TopicAddField { }).ExecuteIdentity();
		}

		[Table(Name = "TopicAddField", OldName = "xxxtb.TopicAddField")]
		public class TopicAddField {
			[Column(IsIdentity = true)]
			public int? Id { get; set; }

			public string name { get; set; }

			[Column(DbType = "varchar(200) not null", OldName = "title")]
			public string title222 { get; set; } = "10";

			[Column(IsIgnore = true)]
			public DateTime ct { get; set; } = DateTime.Now;
		}

		[Fact]
		public void GetComparisonDDLStatements() {

			var sql = g.mysql.CodeFirst.GetComparisonDDLStatements<TableAllType>();
			if (string.IsNullOrEmpty(sql) == false) {
				Assert.Equal(@"CREATE TABLE IF NOT EXISTS `cccddd`.`tb_alltype` ( 
  `Id` INT(11) NOT NULL AUTO_INCREMENT, 
  `testFieldBool` BIT(1) NOT NULL, 
  `testFieldSByte` TINYINT(3) NOT NULL, 
  `testFieldShort` SMALLINT(6) NOT NULL, 
  `testFieldInt` INT(11) NOT NULL, 
  `testFieldLong` BIGINT(20) NOT NULL, 
  `testFieldByte` TINYINT(3) UNSIGNED NOT NULL, 
  `testFieldUShort` SMALLINT(5) UNSIGNED NOT NULL, 
  `testFieldUInt` INT(10) UNSIGNED NOT NULL, 
  `testFieldULong` BIGINT(20) UNSIGNED NOT NULL, 
  `testFieldDouble` DOUBLE NOT NULL, 
  `testFieldFloat` FLOAT NOT NULL, 
  `testFieldDecimal` DECIMAL(10,2) NOT NULL, 
  `testFieldTimeSpan` TIME NOT NULL, 
  `testFieldDateTime` DATETIME(3) NOT NULL, 
  `testFieldBytes` VARBINARY(255), 
  `testFieldString` VARCHAR(255), 
  `testFieldGuid` VARCHAR(36), 
  `testFieldBoolNullable` BIT(1), 
  `testFieldSByteNullable` TINYINT(3), 
  `testFieldShortNullable` SMALLINT(6), 
  `testFieldIntNullable` INT(11), 
  `testFielLongNullable` BIGINT(20), 
  `testFieldByteNullable` TINYINT(3) UNSIGNED, 
  `testFieldUShortNullable` SMALLINT(5) UNSIGNED, 
  `testFieldUIntNullable` INT(10) UNSIGNED, 
  `testFieldULongNullable` BIGINT(20) UNSIGNED, 
  `testFieldDoubleNullable` DOUBLE, 
  `testFieldFloatNullable` FLOAT, 
  `testFieldDecimalNullable` DECIMAL(10,2), 
  `testFieldTimeSpanNullable` TIME, 
  `testFieldDateTimeNullable` DATETIME(3), 
  `testFieldGuidNullable` VARCHAR(36), 
  `testFieldPoint` POINT, 
  `testFieldLineString` LINESTRING, 
  `testFieldPolygon` POLYGON, 
  `testFieldMultiPoint` MULTIPOINT, 
  `testFieldMultiLineString` MULTILINESTRING, 
  `testFieldMultiPolygon` MULTIPOLYGON, 
  `testFieldEnum1` ENUM('E1','E2','E3') NOT NULL, 
  `testFieldEnum1Nullable` ENUM('E1','E2','E3'), 
  `testFieldEnum2` SET('F1','F2','F3') NOT NULL, 
  `testFieldEnum2Nullable` SET('F1','F2','F3'), 
  PRIMARY KEY (`Id`)
) Engine=InnoDB CHARACTER SET utf8;
", sql);
			}

			sql = g.mysql.CodeFirst.GetComparisonDDLStatements<Tb_alltype>();
		}

		IInsert<TableAllType> insert => g.mysql.Insert<TableAllType>();
		ISelect<TableAllType> select => g.mysql.Select<TableAllType>();

		[Fact]
		public void CurdAllField() {
			var item = new TableAllType { };
			item.Id = (int)insert.AppendData(item).ExecuteIdentity();

			var newitem = select.Where(a => a.Id == item.Id).ToOne();

			var item2 = new TableAllType {
				testFieldBool = true,
				testFieldBoolNullable = true,
				testFieldByte = 255,
				testFieldByteNullable = 127,
				testFieldBytes = Encoding.UTF8.GetBytes("�����й���"),
				testFieldDateTime = DateTime.Now,
				testFieldDateTimeNullable = DateTime.Now.AddHours(-1),
				testFieldDecimal = 99.99M,
				testFieldDecimalNullable = 99.98M,
				testFieldDouble = 999.99,
				testFieldDoubleNullable = 999.98,
				testFieldEnum1 = TableAllTypeEnumType1.e5,
				testFieldEnum1Nullable = TableAllTypeEnumType1.e3,
				testFieldEnum2 = TableAllTypeEnumType2.f2,
				testFieldEnum2Nullable = TableAllTypeEnumType2.f3,
				testFieldFloat = 19.99F,
				testFieldFloatNullable = 19.98F,
				testFieldGuid = Guid.NewGuid(),
				testFieldGuidNullable = Guid.NewGuid(),
				testFieldInt = int.MaxValue,
				testFieldIntNullable = int.MinValue,
				testFieldLineString = new MygisLineString(new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10) }),
				testFieldLong = long.MaxValue,
				testFieldMultiLineString = new MygisMultiLineString(new[] {
					new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10) },
					new[] { new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 100) } }),
				testFieldMultiPoint = new MygisMultiPoint(new[] { new MygisCoordinate2D(11, 11), new MygisCoordinate2D(51, 11) }),
				testFieldMultiPolygon = new MygisMultiPolygon(new[] {
					new MygisPolygon(new[] {
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) } }),
					new MygisPolygon(new[] {
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) } }) }),
				testFieldPoint = new MygisPoint(99, 99),
				testFieldPolygon = new MygisPolygon(new[] {
					new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) },
						new[] { new MygisCoordinate2D(10, 10), new MygisCoordinate2D(50, 10), new MygisCoordinate2D(10, 50), new MygisCoordinate2D(10, 10) } }),
				testFieldSByte = 100,
				testFieldSByteNullable = 99,
				testFieldShort = short.MaxValue,
				testFieldShortNullable = short.MinValue,
				testFieldString = "�����й���string",
				testFieldTimeSpan = TimeSpan.FromSeconds(999),
				testFieldTimeSpanNullable = TimeSpan.FromSeconds(60),
				testFieldUInt = uint.MaxValue,
				testFieldUIntNullable = uint.MinValue,
				testFieldULong = ulong.MaxValue,
				testFieldULongNullable = ulong.MinValue,
				testFieldUShort = ushort.MaxValue,
				testFieldUShortNullable = ushort.MinValue,
				testFielLongNullable = long.MinValue
			};
			item2.Id = (int)insert.AppendData(item2).ExecuteIdentity();
			var newitem2 = select.Where(a => a.Id == item2.Id).ToOne();

			var items = select.ToList();
		}


		[JsonObject(MemberSerialization.OptIn), Table(Name = "tb_alltype")]
		public partial class Tb_alltype {

			[JsonProperty, Column(Name = "Id", DbType = "int(11)", IsPrimary = true, IsIdentity = true)]
			public int Id { get; set; }


			[JsonProperty, Column(Name = "testFieldBool", DbType = "bit(1)")]
			public bool TestFieldBool { get; set; }


			[JsonProperty, Column(Name = "testFieldBoolNullable", DbType = "bit(1)", IsNullable = true)]
			public bool? TestFieldBoolNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldByte", DbType = "tinyint(3) unsigned")]
			public byte TestFieldByte { get; set; }


			[JsonProperty, Column(Name = "testFieldByteNullable", DbType = "tinyint(3) unsigned", IsNullable = true)]
			public byte? TestFieldByteNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldBytes", DbType = "varbinary(255)", IsNullable = true)]
			public byte[] TestFieldBytes { get; set; }


			[JsonProperty, Column(Name = "testFieldDateTime", DbType = "datetime")]
			public DateTime TestFieldDateTime { get; set; }


			[JsonProperty, Column(Name = "testFieldDateTimeNullable", DbType = "datetime", IsNullable = true)]
			public DateTime? TestFieldDateTimeNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldDecimal", DbType = "decimal(10,2)")]
			public decimal TestFieldDecimal { get; set; }


			[JsonProperty, Column(Name = "testFieldDecimalNullable", DbType = "decimal(10,2)", IsNullable = true)]
			public decimal? TestFieldDecimalNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldDouble", DbType = "double")]
			public double TestFieldDouble { get; set; }


			[JsonProperty, Column(Name = "testFieldDoubleNullable", DbType = "double", IsNullable = true)]
			public double? TestFieldDoubleNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldEnum1", DbType = "enum('E1','E2','E3','E5')")]
			public Tb_alltypeTESTFIELDENUM1 TestFieldEnum1 { get; set; }


			[JsonProperty, Column(Name = "testFieldEnum1Nullable", DbType = "enum('E1','E2','E3','E5')", IsNullable = true)]
			public Tb_alltypeTESTFIELDENUM1NULLABLE? TestFieldEnum1Nullable { get; set; }


			[JsonProperty, Column(Name = "testFieldEnum2", DbType = "set('F1','F2','F3')")]
			public Tb_alltypeTESTFIELDENUM2 TestFieldEnum2 { get; set; }


			[JsonProperty, Column(Name = "testFieldEnum2Nullable", DbType = "set('F1','F2','F3')", IsNullable = true)]
			public Tb_alltypeTESTFIELDENUM2NULLABLE? TestFieldEnum2Nullable { get; set; }


			[JsonProperty, Column(Name = "testFieldFloat", DbType = "float")]
			public float TestFieldFloat { get; set; }


			[JsonProperty, Column(Name = "testFieldFloatNullable", DbType = "float", IsNullable = true)]
			public float? TestFieldFloatNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldGuid", DbType = "char(36)")]
			public Guid TestFieldGuid { get; set; }


			[JsonProperty, Column(Name = "testFieldGuidNullable", DbType = "char(36)", IsNullable = true)]
			public Guid? TestFieldGuidNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldInt", DbType = "int(11)")]
			public int TestFieldInt { get; set; }


			[JsonProperty, Column(Name = "testFieldIntNullable", DbType = "int(11)", IsNullable = true)]
			public int? TestFieldIntNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldLineString", DbType = "linestring", IsNullable = true)]
			public MygisGeometry TestFieldLineString { get; set; }


			[JsonProperty, Column(Name = "testFieldLong", DbType = "bigint(20)")]
			public long TestFieldLong { get; set; }


			[JsonProperty, Column(Name = "testFieldMultiLineString", DbType = "multilinestring", IsNullable = true)]
			public MygisGeometry TestFieldMultiLineString { get; set; }


			[JsonProperty, Column(Name = "testFieldMultiPoint", DbType = "multipoint", IsNullable = true)]
			public MygisGeometry TestFieldMultiPoint { get; set; }


			[JsonProperty, Column(Name = "testFieldMultiPolygon", DbType = "multipolygon", IsNullable = true)]
			public MygisGeometry TestFieldMultiPolygon { get; set; }


			[JsonProperty, Column(Name = "testFieldPoint", DbType = "point", IsNullable = true)]
			public MygisGeometry TestFieldPoint { get; set; }


			[JsonProperty, Column(Name = "testFieldPolygon", DbType = "polygon", IsNullable = true)]
			public MygisGeometry TestFieldPolygon { get; set; }


			[JsonProperty, Column(Name = "testFieldSByte", DbType = "tinyint(3)")]
			public sbyte TestFieldSByte { get; set; }


			[JsonProperty, Column(Name = "testFieldSByteNullable", DbType = "tinyint(3)", IsNullable = true)]
			public sbyte? TestFieldSByteNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldShort", DbType = "smallint(6)")]
			public short TestFieldShort { get; set; }


			[JsonProperty, Column(Name = "testFieldShortNullable", DbType = "smallint(6)", IsNullable = true)]
			public short? TestFieldShortNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldString", DbType = "varchar(255)", IsNullable = true)]
			public string TestFieldString { get; set; }


			[JsonProperty, Column(Name = "testFieldTimeSpan", DbType = "time")]
			public TimeSpan TestFieldTimeSpan { get; set; }


			[JsonProperty, Column(Name = "testFieldTimeSpanNullable", DbType = "time", IsNullable = true)]
			public TimeSpan? TestFieldTimeSpanNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldUInt", DbType = "int(10) unsigned")]
			public uint TestFieldUInt { get; set; }


			[JsonProperty, Column(Name = "testFieldUIntNullable", DbType = "int(10) unsigned", IsNullable = true)]
			public uint? TestFieldUIntNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldULong", DbType = "bigint(20) unsigned")]
			public ulong TestFieldULong { get; set; }


			[JsonProperty, Column(Name = "testFieldULongNullable", DbType = "bigint(20) unsigned", IsNullable = true)]
			public ulong? TestFieldULongNullable { get; set; }


			[JsonProperty, Column(Name = "testFieldUShort", DbType = "smallint(5) unsigned")]
			public ushort TestFieldUShort { get; set; }


			[JsonProperty, Column(Name = "testFieldUShortNullable", DbType = "smallint(5) unsigned", IsNullable = true)]
			public ushort? TestFieldUShortNullable { get; set; }


			[JsonProperty, Column(Name = "testFielLongNullable", DbType = "bigint(20)", IsNullable = true)]
			public long? TestFielLongNullable { get; set; }

			internal static IFreeSql mysql => null;
			public static FreeSql.ISelect<Tb_alltype> Select => mysql.Select<Tb_alltype>();

			public static int ItemCacheTimeout = 180;
			public static Tb_alltype GetItem(int Id) => Select.Where(a => a.Id == Id).Caching(ItemCacheTimeout, string.Concat("test:tb_alltype:", Id)).ToOne();

			public static long Delete(int Id) {
				var affrows = mysql.Delete<Tb_alltype>().Where(a => a.Id == Id).ExecuteAffrows();
				if (ItemCacheTimeout > 0) RemoveCache(new Tb_alltype { Id = Id });
				return affrows;
			}

			internal static void RemoveCache(Tb_alltype item) => RemoveCache(item == null ? null : new[] { item });
			internal static void RemoveCache(IEnumerable<Tb_alltype> items) {
				if (ItemCacheTimeout <= 0 || items == null || items.Any() == false) return;
				var keys = new string[items.Count() * 1];
				var keysIdx = 0;
				foreach (var item in items) {
					keys[keysIdx++] = string.Concat("test:tb_alltype:", item.Id);
				}
				if (mysql.Ado.TransactionCurrentThread != null) mysql.Ado.TransactionPreRemoveCache(keys);
				else mysql.Cache.Remove(keys);
			}

			/// <summary>
			/// �������ӣ����������ֵ���� Update�����Ӱ�����Ϊ 0 ���� Insert
			/// </summary>
			public void Save() {
				if (this.Id != default(int)) {
					var affrows = mysql.Update<Tb_alltype>().Where(a => a.Id == Id).ExecuteAffrows();
					if (affrows > 0) return;
				}
				this.Id = (int)mysql.Insert<Tb_alltype>().AppendData(this).ExecuteIdentity();
			}

		}

		public enum Tb_alltypeTESTFIELDENUM1 {
			E1 = 1, E2, E3, E5
		}
		public enum Tb_alltypeTESTFIELDENUM1NULLABLE {
			E1 = 1, E2, E3, E5
		}
		[Flags]
		public enum Tb_alltypeTESTFIELDENUM2 : long {
			F1 = 1, F2 = 2, F3 = 4
		}
		[Flags]
		public enum Tb_alltypeTESTFIELDENUM2NULLABLE : long {
			F1 = 1, F2 = 2, F3 = 4
		}


		[Table(Name = "tb_alltype")]
		class TableAllType {
			[Column(IsIdentity = true, IsPrimary = true)]
			public int Id { get; set; }

			public bool testFieldBool { get; set; }
			public sbyte testFieldSByte { get; set; }
			public short testFieldShort { get; set; }
			public int testFieldInt { get; set; }
			public long testFieldLong { get; set; }
			public byte testFieldByte { get; set; }
			public ushort testFieldUShort { get; set; }
			public uint testFieldUInt { get; set; }
			public ulong testFieldULong { get; set; }
			public double testFieldDouble { get; set; }
			public float testFieldFloat { get; set; }
			public decimal testFieldDecimal { get; set; }
			public TimeSpan testFieldTimeSpan { get; set; }
			public DateTime testFieldDateTime { get; set; }
			public byte[] testFieldBytes { get; set; }
			public string testFieldString { get; set; }
			public Guid testFieldGuid { get; set; }

			public bool? testFieldBoolNullable { get; set; }
			public sbyte? testFieldSByteNullable { get; set; }
			public short? testFieldShortNullable { get; set; }
			public int? testFieldIntNullable { get; set; }
			public long? testFielLongNullable { get; set; }
			public byte? testFieldByteNullable { get; set; }
			public ushort? testFieldUShortNullable { get; set; }
			public uint? testFieldUIntNullable { get; set; }
			public ulong? testFieldULongNullable { get; set; }
			public double? testFieldDoubleNullable { get; set; }
			public float? testFieldFloatNullable { get; set; }
			public decimal? testFieldDecimalNullable { get; set; }
			public TimeSpan? testFieldTimeSpanNullable { get; set; }
			public DateTime? testFieldDateTimeNullable { get; set; }
			public Guid? testFieldGuidNullable { get; set; }

			public MygisPoint testFieldPoint { get; set; }
			public MygisLineString testFieldLineString { get; set; }
			public MygisPolygon testFieldPolygon { get; set; }
			public MygisMultiPoint testFieldMultiPoint { get; set; }
			public MygisMultiLineString testFieldMultiLineString { get; set; }
			public MygisMultiPolygon testFieldMultiPolygon { get; set; }

			public TableAllTypeEnumType1 testFieldEnum1 { get; set; }
			public TableAllTypeEnumType1? testFieldEnum1Nullable { get; set; }
			public TableAllTypeEnumType2 testFieldEnum2 { get; set; }
			public TableAllTypeEnumType2? testFieldEnum2Nullable { get; set; }
		}

		public enum TableAllTypeEnumType1 { e1, e2, e3, e5 }
		[Flags] public enum TableAllTypeEnumType2 { f1, f2, f3 }
	}
}
