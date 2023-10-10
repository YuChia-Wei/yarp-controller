namespace Yarp.ControlPlant.UseCase.Queries;

public class QueryResult<TResponse>
{
    private QueryResult(bool isFounded, TResponse queryResponse)
    {
        this.IsFounded = isFounded;
        this.QueryResponse = queryResponse;
    }

    public bool IsFounded { get; }

    public TResponse QueryResponse { get; init; }

    public static QueryResult<TResponse> Found(TResponse queryResponse)
    {
        return new QueryResult<TResponse>(true, queryResponse);
    }

    public static QueryResult<TResponse> NotFound()
    {
        return new QueryResult<TResponse>(false, default!);
    }
}