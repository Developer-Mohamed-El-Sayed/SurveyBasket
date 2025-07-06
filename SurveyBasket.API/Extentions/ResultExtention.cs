namespace SurveyBasket.API.Extentions;

public static class ResultExtention
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("cannot convert success result to problem.");
        var problem = Results.Problem(statusCode: result.Error.StatusCode);
        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors",new[]{result.Error}
            }
        };
        return new ObjectResult(problemDetails);
    }
}
