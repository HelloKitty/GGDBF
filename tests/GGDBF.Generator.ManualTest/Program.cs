using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using TestNamespace;
using TestNamespace2;
using System.Linq;
using ProtoBuf;
using Fasterflect;
using TestNameSpace.Extended.Multiple.Words;

namespace GGDBF.Generator.ManualTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			//TestProtobufSerialization();

			var options = new DbContextOptionsBuilder<TestDBContext>()
				.UseInMemoryDatabase("Test")
				.Options;

			var writer = new DefaultFileGGDBFDataWriter(new DefaultGGDBFProtobufNetSerializer());

			await using TestDBContext context = new(options);
			await context.Database.EnsureCreatedAsync();

			await InitializeDatabase(context);

			ContextGenerator<TestContext> generator = new ContextGenerator<TestContext>(new EntityFrameworkGGDBFDataSource(context), writer);

			await generator.Generate();

			//Reload the data
			await TestContext.Initialize(new FileGGDBFDataSource(new DefaultGGDBFProtobufNetSerializer()));

			if (TestContext.Instance.TestDatas.Values.Count() != 3)
				throw new InvalidOperationException($"{nameof(TestModelType)} does not have expected entry count.");

			if (TestContext.Instance.Test3DatasWithFK.Values.First().ModelId == null)
				throw new InvalidOperationException($"{nameof(TestModelType3)} nav property key null.");

			if (TestContext.Instance.Test3DatasWithFK.Values.First().Model == null)
				throw new InvalidOperationException($"{nameof(TestModelType3)} nav property is null.");

			if (TestContext.Instance.Test4Datas.Values.First().ModelCollection.Count() != 3)
				throw new InvalidOperationException($"{nameof(TestModelType4)} collection nav property is invalid.");
		}

		private static async Task InitializeDatabase(TestDBContext context)
		{
			var models = await context.Test2Datas.ToArrayAsync();
			var models10 = await context.Test10Datas.ToArrayAsync();
			var model4 = new TestModelType4("2", new List<TestModelType2>(models));

			var model11 = new TestModelType11<int, short>(1, 2,models10);

			await context.Test4Datas.AddAsync(model4);
			await context.Tes11Datas.AddAsync(model11);
			await context.SaveChangesAsync();
		}
	}

	/*[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType3))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType5<int, TestModelType4, TestModelType, short>))]*/
	public class TestDBContext : DbContext
	{
		public DbSet<TestModelType> TestDatas { get; set; }
		public DbSet<TestModelType2> Test2Datas { get; set; }
		public DbSet<TestModelType3> Test3Datas { get; set; }
		public DbSet<TestModelType4> Test4Datas { get; set; }
		public DbSet<TestModelType5<int, TestModelType4, TestModelType, short>> Test5Datas { get; set; }

		public DbSet<TestModelTypeUnderscore> TestDatasWithUnderScore { get; set; }

		public DbSet<TestModelReservedNameTable> ReservedTable { get; set; }

		public DbSet<TestModelType7> Test7Datas { get; set; }

		public DbSet<TestModelType8> Test8Datas { get; set; }

		public DbSet<TestModelType9<int, string>> Test9Datas { get; set; }

		public DbSet<TestModelType10<int>> Test10Datas { get; set; }

		public DbSet<TestModelType11<Int32, Int16>> Tes11Datas { get; set; }

		public TestDBContext(DbContextOptions options)
			: base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TestModelType>().HasData(new List<TestModelType>()
			{
				new TestModelType(1),
				new TestModelType(69),
				new TestModelType(9001)
			});

			modelBuilder.Entity<TestModelType2>().HasData(new List<TestModelType2>()
			{
				new TestModelType2("1"),
				new TestModelType2("69"),
				new TestModelType2("9001")
			});

			modelBuilder.Entity<TestModelType3>().HasData(new List<TestModelType3>()
			{
				new TestModelType3("1", "1"),
				new TestModelType3("2", "69"),
				new TestModelType3("3", "9001")
			});

			modelBuilder.Entity<TestModelType7>().HasData(new List<TestModelType7>()
			{
				new TestModelType7(1, "test", "2"),
				new TestModelType7(2, "test", "2"),
				new TestModelType7(1, "derp", "2")
			});

			modelBuilder.Entity<TestModelType7>()
				.HasKey(m => new {m.Id1, m.Id2});


			modelBuilder.Entity<TestModelType9<int, string>>()
				.HasKey(m => new { m.Id1, m.Id2 });

			modelBuilder.Entity<TestModelType10<int>>()
				.HasKey(m => new {m.Id1, m.Id2});

			modelBuilder.Entity<TestModelType10<int>>()
				.HasData(new List<TestModelType10<int>>()
				{
					new TestModelType10<int>(1, 2),
					new TestModelType10<int>(2, 2),
					new TestModelType10<int>(69, 3)
				});

			modelBuilder.Entity<TestModelType11<Int32, Int16>>()
				.HasKey(m => new { m.Id1, m.Id2 });

			//Seeding is broken for collection props.
			/*modelBuilder.Entity<TestModelType4>().HasData(new List<TestModelType4>()
			{
				new TestModelType4("2", new List<TestModelType2>()
				{
					new TestModelType2("1"),
					new TestModelType2("69"),
					new TestModelType2("9001")
				})
			});*/
		}
	}
}
