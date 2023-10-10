namespace Yarp.ControlPlant.UseCase.Commands;

public class ExecuteResult
{
    protected ExecuteResult(bool isSuccess)
    {
        this.IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; init; }

    public static ExecuteResult Success()
    {
        return new ExecuteResult(true);
    }
}

public class ExecuteResult<TExecuteResponse> : ExecuteResult
{
    private ExecuteResult(bool isSuccess, TExecuteResponse executeResponse)
        : base(isSuccess)
    {
        this.ExecuteResponse = executeResponse ?? default;
    }

    public TExecuteResponse? ExecuteResponse { get; init; }

    public static ExecuteResult<TExecuteResponse> Success(TExecuteResponse executeResponse)
    {
        return new ExecuteResult<TExecuteResponse>(true, executeResponse);
    }
}