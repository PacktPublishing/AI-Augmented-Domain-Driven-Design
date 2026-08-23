using BrewUp.Shared.ReadModel;

namespace BrewUp.Infrastructure.ReadModel;

public class LastEventPosition : DtoBase
{
    public ulong CommitPosition { get; set; }
    public ulong PreparePosition { get; set; }   
}