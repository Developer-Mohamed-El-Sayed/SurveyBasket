namespace SurveyBasket.API.Repository.Implementations;

public class QuestionService(SurveyBasketDbContext context) : IQuestionService
{
    private readonly SurveyBasketDbContext _context = context;

    public async Task<Result<QuestionResponse>> CreateAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
        var questionIsExists = await _context.Questions
            .AnyAsync(x => x.PollId ==  pollId && x.Content == request.Content, cancellationToken: cancellationToken);

        if(questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DublicatedQuestions);

        var response = request.Adapt<Question>();
        response.PollId = pollId;
        await _context.AddAsync(response, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(response.Adapt<QuestionResponse>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist =  await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Include(a => a.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        var response = questions.Adapt<IEnumerable<QuestionResponse>>();
        return Result.Success(response);
    }
}
