using LiteDB;

namespace LearnNet_CartingService.Core.Interfaces
{
	public interface ILiteDbContext
	{
		LiteDatabase Database { get; }
	}
}
