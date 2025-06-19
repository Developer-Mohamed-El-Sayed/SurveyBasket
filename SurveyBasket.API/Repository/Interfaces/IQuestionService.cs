namespace SurveyBasket.API.Repository.Interfaces;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> CreateAsync(int pollId,QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,CancellationToken cancellationToken = default);
}
