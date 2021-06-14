using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using TestNamespace;
using TestNamespace2;

namespace GGDBF.Generator.ManualTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var options = new DbContextOptionsBuilder<TestDBContext>()
				.UseInMemoryDatabase("Test")
				.Options;

			var writer = new DefaultFileGGDBFDataWriter(new DefaultGGDBFProtobufNetSerializer());

			await using TestDBContext context = new(options);
			ContextGenerator<TestContext> generator = new ContextGenerator<TestContext>(new EntityFrameworkGGDBFDataSource(context), writer);

			await generator.Generate();

			//Reload the data
			await TestContext.Initialize(new FileGGDBFDataSource(new DefaultGGDBFProtobufNetSerializer()));
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

		public TestDBContext(DbContextOptions options)
			: base(options)
		{
			
		}
	}
}
