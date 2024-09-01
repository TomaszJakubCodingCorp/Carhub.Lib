namespace Carhub.Lib.SharedKernel.SharedKernel;

public abstract class AggregateRoot
{
    public AggregateId Id { get; }

    protected AggregateRoot(AggregateId id)
        => Id = id;
}

public abstract class AggregateRoot<T>
{
    public AggregateId<T> Id { get; }

    protected AggregateRoot(AggregateId<T> id)
        => Id = id;
}