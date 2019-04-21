using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FreeSql.Tests.MySql {

	public class MySqlSelectTest {

		ISelect<Topic> select => g.mysql.Select<Topic>();

		[Table(Name = "tb_topic")]
		public class Topic {
			[Column(IsIdentity = true, IsPrimary = true)]
			public int Id { get; set; }
			public int Clicks { get; set; }
			[Column(OldName = "TypeGuid")]
			public int TypeGuid { get; set; }
			public TestTypeInfo Type { get; set; }
			public string Title { get; set; }
			public DateTime CreateTime { get; set; }

			public virtual TopicFields Fields { get; set; }
		}
		public class TopicFields {
			[Column(IsPrimary = true)]
			public int TopicId { get; set; }
			public virtual Topic Topic { get; set; }
		}
		public class TestTypeInfo {
			[Column(IsIdentity = true)]
			public int Guid { get; set; }

			public int ParentId { get; set; }
			public virtual TestTypeParentInfo Parent { get; set; }

			public string Name { get; set; }

			public virtual ICollection<Topic> Topics { get; set; }
		}
		public class TestTypeParentInfo {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public string Name { get; set; }

			public List<TestTypeInfo> Types { get; set; }
		}

		public partial class Song {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public DateTime? Create_time { get; set; }
			public bool? Is_deleted { get; set; }
			public string Title { get; set; }
			public string Url { get; set; }

			public virtual ICollection<Tag> Tags { get; set; }
		}
		public partial class Song_tag {
			public int Song_id { get; set; }
			public virtual Song Song { get; set; }

			public int Tag_id { get; set; }
			public virtual Tag Tag { get; set; }
		}
		public partial class Tag {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public int? Parent_id { get; set; }
			public virtual Tag Parent { get; set; }

			public decimal? Ddd { get; set; }
			public string Name { get; set; }

			public virtual ICollection<Song> Songs { get; set; }
			public virtual ICollection<Tag> Tags { get; set; }
		}


		[Fact]
		public void AsSelect() {
			//OneToOne、ManyToOne
			var t0 = g.mysql.Select<Tag>().Where(a => a.Parent.Parent.Name == "粤语").ToSql();
			//SELECT a.`Id`, a.`Parent_id`, a__Parent.`Id` as3, a__Parent.`Parent_id` as4, a__Parent.`Ddd`, a__Parent.`Name`, a.`Ddd` as7, a.`Name` as8 
			//FROM `Tag` a 
			//LEFT JOIN `Tag` a__Parent ON a__Parent.`Id` = a.`Parent_id` 
			//LEFT JOIN `Tag` a__Parent__Parent ON a__Parent__Parent.`Id` = a__Parent.`Parent_id` 
			//WHERE (a__Parent__Parent.`Name` = '粤语')

			//OneToMany
			var t1 = g.mysql.Select<Tag>().Where(a => a.Tags.AsSelect().Any(t => t.Parent.Id == 10)).ToSql();
			//SELECT a.`Id`, a.`Parent_id`, a.`Ddd`, a.`Name` 
			//FROM `Tag` a 
			//WHERE (exists(SELECT 1 
			//	FROM `Tag` t 
			//	LEFT JOIN `Tag` t__Parent ON t__Parent.`Id` = t.`Parent_id` 
			//	WHERE (t__Parent.`Id` = 10) AND (t.`Parent_id` = a.`Id`) 
			//	limit 0,1))

			//ManyToMany
			var t2 = g.mysql.Select<Song>().Where(s => s.Tags.AsSelect().Any(t => t.Name == "国语")).ToSql();
			//SELECT a.`Id`, a.`Create_time`, a.`Is_deleted`, a.`Title`, a.`Url` 
			//FROM `Song` a
			//WHERE(exists(SELECT 1
			//	FROM `Song_tag` Mt_Ms
			//	WHERE(Mt_Ms.`Song_id` = a.`Id`) AND(exists(SELECT 1
			//		FROM `Tag` t
			//		WHERE(t.`Name` = '国语') AND(t.`Id` = Mt_Ms.`Tag_id`)
			//		limit 0, 1))
			//	limit 0, 1))
		}

		[Fact]
		public void Lazy() {
			var tags = g.mysql.Select<Tag>().Where(a => a.Parent.Name == "xxx")
				.LeftJoin(a => a.Parent_id == a.Parent.Id)
				.ToSql();

			var songs = g.mysql.Select<Song>().Limit(10).ToList();
		}

		[Fact]
		public void ToDataTable() {
			var items = new List<Topic>();
			for (var a = 0; a < 10; a++) items.Add(new Topic { Id = a + 1, Title = $"newtitle{a}", Clicks = a * 100, CreateTime = DateTime.Now });

			Assert.Equal(1, g.mysql.Insert<Topic>().AppendData(items.First()).ExecuteAffrows());
			Assert.Equal(10, g.mysql.Insert<Topic>().AppendData(items).ExecuteAffrows());

			//items = Enumerable.Range(0, 9989).Select(a => new Topic { Title = "newtitle" + a, CreateTime = DateTime.Now }).ToList();
			//Assert.Equal(9989, g.mysql.Insert<Topic>(items).ExecuteAffrows());

			var dt1 = select.Limit(10).ToDataTable();
			var dt2 = select.Limit(10).ToDataTable("id, 111222");
			var dt3 = select.Limit(10).ToDataTable(a => new { a.Id, a.Type.Name, now = DateTime.Now });
		}

		class TestDto {
			public int id { get; set; }
			public string name { get; set; } //这是join表的属性
			public int ParentId { get; set; } //这是join表的属性
		}
		[Fact]
		public void ToList() {

			var testDto1 = select.Limit(10).ToList(a => new TestDto { id = a.Id, name = a.Title });
			var testDto2 = select.Limit(10).ToList(a => new TestDto());
			var testDto3 = select.Limit(10).ToList(a => new TestDto { });
			var testDto4 = select.Limit(10).ToList(a => new TestDto() { });

			var testDto11 = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).Limit(10).ToList(a => new TestDto { id = a.Id, name = a.Title });
			var testDto22 = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).Limit(10).ToList(a => new TestDto());
			var testDto33 = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).Limit(10).ToList(a => new TestDto { });
			var testDto44 = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).Limit(10).ToList(a => new TestDto() { });

			var t0 = select.Limit(50).ToList();


			var t1 = g.mysql.Select<TestInfo>().Where("").Where(a => a.Id > 0).Skip(100).Limit(200).ToSql();
			var t2 = g.mysql.Select<TestInfo>().As("b").Where("").Where(a => a.Id > 0).Skip(100).Limit(200).ToSql();


			var sql1 = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).ToSql();
			var sql2 = select.LeftJoin<TestTypeInfo>((a, b) => a.TypeGuid == b.Guid && b.Name == "111").ToSql();
			var sql3 = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid").ToSql();

			//g.mysql.Select<TestInfo, TestTypeInfo, TestTypeParentInfo>().Join((a, b, c) => new Model.JoinResult3(
			//   Model.JoinType.LeftJoin, a.TypeGuid == b.Guid,
			//   Model.JoinType.InnerJoin, c.Id == b.ParentId && c.Name == "xxx")
			//);

			//var sql4 = select.From<TestTypeInfo, TestTypeParentInfo>((a, b, c) => new SelectFrom()
			//	.InnerJoin(a.TypeGuid == b.Guid)
			//	.LeftJoin(c.Id == b.ParentId)
			//	.Where(b.Name == "xxx"))
			//.Where(a => a.Id == 1).ToSql();

			var sql4 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.InnerJoin(a => a.TypeGuid == b.Guid)
				.LeftJoin(a => c.Id == b.ParentId)
				.Where(a => b.Name == "xxx")).ToSql();
			//.Where(a => a.Id == 1).ToSql();


			var list111 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.InnerJoin(a => a.TypeGuid == b.Guid)
				.LeftJoin(a => c.Id == b.ParentId)
				.Where(a => b.Name != "xxx"));
			var list111sql = list111.ToSql();
			var list111data = list111.ToList((a, b, c) => new {
				a.Id,
				title_substring = a.Title.Substring(0, 1),
				a.Type,
				ccc = new { a.Id, a.Title },
				tp = a.Type,
				tp2 = new {
					a.Id,
					tp2 = a.Type.Name
				},
				tp3 = new {
					a.Id,
					tp33 = new {
						a.Id
					}
				}
			});

			var ttt122 = g.mysql.Select<TestTypeParentInfo>().Where(a => a.Id > 0).ToSql();
			var sql5 = g.mysql.Select<TestInfo>().From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s).Where((a, b, c) => a.Id == b.ParentId).ToSql();
			var t11112 = g.mysql.Select<TestInfo>().ToList(a => new {
				a.Id,
				a.Title,
				a.Type,
				ccc = new { a.Id, a.Title },
				tp = a.Type,
				tp2 = new {
					a.Id,
					tp2 = a.Type.Name
				},
				tp3 = new {
					a.Id,
					tp33 = new {
						a.Id
					}
				}

			});

			var t100 = g.mysql.Select<TestInfo>().Where("").Where(a => a.Id > 0).Skip(100).Limit(200).Caching(50).ToList();
			var t101 = g.mysql.Select<TestInfo>().As("b").Where("").Where(a => a.Id > 0).Skip(100).Limit(200).Caching(50).ToList();


			var t1111 = g.mysql.Select<TestInfo>().ToList(a => new { a.Id, a.Title, a.Type });

			var t2222 = g.mysql.Select<TestInfo>().ToList(a => new { a.Id, a.Title, a.Type.Name });

			g.mysql.Insert<TestGuidIdToList>().AppendData(new TestGuidIdToList()).ExecuteAffrows();
			var testGuidId5 = g.mysql.Select<TestGuidIdToList>().ToList();
			var testGuidId6 = g.mysql.Select<TestGuidIdToList>().ToList(a => a.id);
		}
		class TestGuidIdToList {
			public Guid id { get; set; }
			public string title { get; set; } = Guid.NewGuid().ToString();
		}
		[Fact]
		public void ToOne() {
			var testnotfind = select.Where("1=2").First(a => a.CreateTime);
			Assert.Equal(default(DateTime), testnotfind);
		}
		[Fact]
		public void ToSql() {
			g.mysql.Insert<testenumWhere>().AppendData(new testenumWhere { type = testenumWhereType.Blaaa }).ExecuteAffrows();

			var sql1 = g.mysql.Select<testenumWhere>().Where(a => a.type == testenumWhereType.Blaaa).ToSql();
			var sql2 = g.mysql.Select<testenumWhere>().Where(a => testenumWhereType.Blaaa == a.type).ToSql();

			var sql3 = g.mysql.Select<testenumWhere>().Where(a => a.type.Equals(testenumWhereType.Blaaa)).ToSql();
			var tolist = g.mysql.Select<testenumWhere>().Where(a => a.type == testenumWhereType.Blaaa).ToList();
		}
		class testenumWhere {
			public Guid id { get; set; }
			public testenumWhereType type { get; set; }
		}
		public enum testenumWhereType { Menu, Class, Blaaa }

		[Fact]
		public void Any() {
			var count = select.Where(a => 1 == 1).Count();
			Assert.False(select.Where(a => 1 == 2).Any());
			Assert.Equal(count > 0, select.Where(a => 1 == 1).Any());

			var sql2222 = select.Where(a =>
				select.Where(b => b.Id == a.Id &&
					select.Where(c => c.Id == b.Id).Where(d => d.Id == a.Id).Where(e => e.Id == b.Id)
					.Offset(a.Id)
					.Any()
				).Any(c => c.Id == a.Id + 10)
			);
			var sql2222Tolist = sql2222.ToList();

			var collectionSelect = select.Where(a =>
				a.Type.Guid == a.TypeGuid &&
				a.Type.Parent.Id == a.Type.ParentId &&
				a.Type.Parent.Types.AsSelect().Where(b => b.Name == a.Title).Any(b => b.ParentId == a.Type.Parent.Id)
			);
			collectionSelect.ToList();
		}
		[Fact]
		public void Count() {
			var count = select.Where(a => 1 == 1).Count();
			select.Where(a => 1 == 1).Count(out var count2);
			Assert.Equal(count, count2);
			Assert.Equal(0, select.Where(a => 1 == 2).Count());
		}
		[Fact]
		public void Master() {
			Assert.StartsWith(" SELECT", select.Master().Where(a => 1 == 1).ToSql());
		}
		[Fact]
		public void Caching() {
			var result1 = select.Where(a => 1 == 1).Caching(20, "testcaching").ToList();
			var testcaching1 = g.mysql.Cache.Get("testcaching");
			Assert.NotNull(testcaching1);
			var result2 = select.Where(a => 1 == 1).Caching(20, "testcaching").ToList();
			var testcaching2 = g.mysql.Cache.Get("testcaching");
			Assert.NotNull(testcaching2);
			Assert.Equal(result1.Count, result1.Count);
		}
		[Fact]
		public void From() {
			var query2 = select.From<TestTypeInfo>((s, b) => s
				 .LeftJoin(a => a.TypeGuid == b.Guid)
				);
			var sql2 = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON a.`TypeGuid` = b.`Guid`", sql2);
			query2.ToList();

			var query3 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TypeGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id)
				);
			var sql3 = query3.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON a.`TypeGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql3);
			query3.ToList();
		}
		[Fact]
		public void LeftJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx' LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx' LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.LeftJoin(a => a.Type.Guid == a.TypeGuid)
				.LeftJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.LeftJoin<TestTypeInfo>((a, a__Type) => a__Type.Guid == a.TypeGuid)
				.LeftJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` c ON c.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TypeGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON a.`TypeGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN TestTypeInfo b on b.Guid = a.TypeGuid", sql);
			query.ToList();

			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", sql);
			query.ToList();
		}
		[Fact]
		public void InnerJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.InnerJoin(a => a.Type.Guid == a.TypeGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.InnerJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.InnerJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx' LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx' LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.InnerJoin(a => a.Type.Guid == a.TypeGuid)
				.InnerJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` INNER JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.InnerJoin<TestTypeInfo>((a, a__Type) => a__Type.Guid == a.TypeGuid)
				.InnerJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` INNER JOIN `TestTypeParentInfo` c ON c.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .InnerJoin(a => a.TypeGuid == b.Guid)
				 .InnerJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON a.`TypeGuid` = b.`Guid` INNER JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.InnerJoin("TestTypeInfo b on b.Guid = a.TypeGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN TestTypeInfo b on b.Guid = a.TypeGuid", sql);
			query.ToList();

			query = select.InnerJoin("TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", sql);
			query.ToList();

		}
		[Fact]
		public void RightJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.RightJoin(a => a.Type.Guid == a.TypeGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.RightJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.RightJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx' LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid`", sql);
			query.ToList();

			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx' LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.RightJoin(a => a.Type.Guid == a.TypeGuid)
				.RightJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` RIGHT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.RightJoin<TestTypeInfo>((a, a__Type) => a__Type.Guid == a.TypeGuid)
				.RightJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` RIGHT JOIN `TestTypeParentInfo` c ON c.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .RightJoin(a => a.TypeGuid == b.Guid)
				 .RightJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON a.`TypeGuid` = b.`Guid` RIGHT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.RightJoin("TestTypeInfo b on b.Guid = a.TypeGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN TestTypeInfo b on b.Guid = a.TypeGuid", sql);
			query.ToList();

			query = select.RightJoin("TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", sql);
			query.ToList();

		}
		[Fact]
		public void Where() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.Where(a => a.Id == 10);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10)", sql);
			query.ToList();

			query = select.Where(a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10 AND a.`Id` > 10 OR a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.Where(a => a.Id == 10).Where(a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10) AND (a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.Where(a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` WHERE (a__Type.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.Where(a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` WHERE (a__Type.`Name` = 'typeTitle' AND a__Type.`Guid` = a.`TypeGuid`)", sql);
			query.ToList();

			query = select.Where(a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Name` = 'tparent')", sql);
			query.ToList();

			//���û�е������ԣ��򵥶������
			query = select.Where<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b WHERE (b.`Guid` = a.`TypeGuid` AND b.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.Where<TestTypeInfo>((a, b) => b.Name == "typeTitle" && b.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b WHERE (b.`Name` = 'typeTitle' AND b.`Guid` = a.`TypeGuid`)", sql);
			query.ToList();

			query = select.Where<TestTypeInfo, TestTypeParentInfo>((a, b, c) => c.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeParentInfo` c WHERE (c.`Name` = 'tparent')", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.Where(a => a.Id == 10 && c.Name == "xxx")
				.Where(a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b, `TestTypeParentInfo` c WHERE (a.`Id` = 10 AND c.`Name` = 'xxx') AND (b.`ParentId` = 20)", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.Where("a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.clicks > 100 and a.id = ?id)", sql);
			query.ToList();
		}
		[Fact]
		public void WhereIf() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.WhereIf(true, a => a.Id == 10);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10 AND a.`Id` > 10 OR a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Id == 10).WhereIf(true, a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10) AND (a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` WHERE (a__Type.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` WHERE (a__Type.`Name` = 'typeTitle' AND a__Type.`Guid` = a.`TypeGuid`)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Name` = 'tparent')", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.WhereIf(true, a => a.Id == 10 && c.Name == "xxx")
				.WhereIf(true, a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b, `TestTypeParentInfo` c WHERE (a.`Id` = 10 AND c.`Name` = 'xxx') AND (b.`ParentId` = 20)", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.WhereIf(true, "a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.clicks > 100 and a.id = ?id)", sql);
			query.ToList();

			// ==========================================WhereIf(false)

			//����е�������a.Type��a.Type.Parent ���ǵ�������
			query = select.WhereIf(false, a => a.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Id == 10).WhereIf(false, a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TypeGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.WhereIf(false, a => a.Id == 10 && c.Name == "xxx")
				.WhereIf(false, a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b, `TestTypeParentInfo` c", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.WhereIf(false, "a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();
		}
		[Fact]
		public void WhereExists() {
			var sql2222 = select.Where(a => select.Where(b => b.Id == a.Id).Any()).ToList();

			sql2222 = select.Where(a =>
				select.Where(b => b.Id == a.Id && select.Where(c => c.Id == b.Id).Where(d => d.Id == a.Id).Where(e => e.Id == b.Id)

				.Offset(a.Id)

				.Any()
				).Any()
			).ToList();
		}
		[Fact]
		public void GroupBy() {
			var groupby = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.Where(a => a.Id == 1)
			)
			.GroupBy((a, b, c) => new { tt2 = a.Title.Substring(0, 2), mod4 = a.Id % 4 })
			.Having(a => a.Count() > 0 && a.Avg(a.Key.mod4) > 0 && a.Max(a.Key.mod4) > 0)
			.Having(a => a.Count() < 300 || a.Avg(a.Key.mod4) < 100)
			.OrderBy(a => a.Key.tt2)
			.OrderByDescending(a => a.Count())
			.Offset(10)
			.Limit(2)
			.ToList(a => new {
				a.Key.tt2,
				cou1 = a.Count(),
				arg1 = a.Avg(a.Key.mod4),
				ccc2 = a.Key.tt2 ?? "now()",
				//ccc = Convert.ToDateTime("now()"), partby = Convert.ToDecimal("sum(num) over(PARTITION BY server_id,os,rid,chn order by id desc)")
			});
		}
		[Fact]
		public void ToAggregate() {
			var sql = select.ToAggregate(a => new { sum = a.Sum(a.Key.Id + 11.11), avg = a.Avg(a.Key.Id), count = a.Count(), max = a.Max(a.Key.Id), min = a.Min(a.Key.Id) });
		}

		[Fact]
		public void OrderBy() {
			var sql = select.OrderBy(a => new Random().NextDouble()).ToList();
		}
		[Fact]
		public void Skip_Offset() {
			var sql = select.Offset(10).Limit(10).ToList();
		}
		[Fact]
		public void Take_Limit() {
			var sql = select.Limit(10).ToList();
		}
		[Fact]
		public void Page() {
			var sql1 = select.Page(1, 10).ToList();
			var sql2 = select.Page(2, 10).ToList();
			var sql3 = select.Page(3, 10).ToList();

			var sql11 = select.OrderBy(a => new Random().NextDouble()).Page(1, 10).ToList();
			var sql22 = select.OrderBy(a => new Random().NextDouble()).Page(2, 10).ToList();
			var sql33 = select.OrderBy(a => new Random().NextDouble()).Page(3, 10).ToList();
		}
		[Fact]
		public void Distinct() {
			var t1 = select.Distinct().ToList(a => a.Title);
			var t2 = select.Distinct().Limit(10).ToList(a => a.Title);
		}

		[Fact]
		public void Sum() {
		}
		[Fact]
		public void Min() {
		}
		[Fact]
		public void Max() {
		}
		[Fact]
		public void Avg() {
		}
		[Fact]
		public void As() {
		}

		[Fact]
		public void AsTable() {

			var listt = select.AsTable((a, b) => "(select * from tb_topic where clicks > 10)").Page(1, 10).ToList();

			Func<Type, string, string> tableRule = (type, oldname) => {
				if (type == typeof(Topic)) return oldname + "AsTable1";
				else if (type == typeof(TestTypeInfo)) return oldname + "AsTable2";
				return oldname + "AsTable";
			};

			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid).AsTable(tableRule);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid`", sql);

			query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx'", sql);

			query = select.LeftJoin(a => a.Type.Guid == a.TypeGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid` AND a__Type.`Name` = 'xxx' LEFT JOIN `TestTypeParentInfoAsTable` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);

			//���û�е�������
			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TypeGuid`", sql);

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx'", sql);

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TypeGuid` AND b.`Name` = 'xxx' LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfoAsTable` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId` WHERE (a__Type__Parent.`Id` = 10)", sql);

			//�������
			query = select
				.LeftJoin(a => a.Type.Guid == a.TypeGuid)
				.LeftJoin(a => a.Type.Parent.Id == a.Type.ParentId).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfoAsTable` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);

			query = select
				.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TypeGuid)
				.LeftJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TypeGuid` LEFT JOIN `TestTypeParentInfoAsTable` c ON c.`Id` = a__Type.`ParentId`", sql);

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TypeGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id)).AsTable(tableRule);
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON a.`TypeGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfoAsTable` c ON b.`ParentId` = c.`Id`", sql);

			//������϶����㲻��
			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN TestTypeInfo b on b.Guid = a.TypeGuid", sql);

			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", new { bname = "xxx" }).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TypeGuid`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN TestTypeInfo b on b.Guid = a.TypeGuid and b.Name = ?bname", sql);
		}
	}
}
