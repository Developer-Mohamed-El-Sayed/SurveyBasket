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
            .AnyAsync(x => x.PollId == pollId && x.Content == request.Content, cancellationToken: cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DublicatedQuestions);

        var response = request.Adapt<Question>();
        response.PollId = pollId;
        await _context.AddAsync(response, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(response.Adapt<QuestionResponse>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
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

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExist = await _context.Questions
            .AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
        if (!questionIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        var question = await _context.Questions
            .Where(x => x.Id == id && x.IsActive && x.PollId == pollId)
            .Include(a => a.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
           .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExist = await _context.Questions
            .AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
        if (!questionIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        var question = await _context.Questions
            .FindAsync(id, cancellationToken);
        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExist = await _context.Questions
            .AnyAsync(x => x.Id != id
                    && x.PollId == pollId
                    && x.Content == request.Content
                 , cancellationToken: cancellationToken
            );
        if (questionIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.DublicatedQuestions);
        var question = await _context.Questions
            .Include(a => a.Answers)
            .SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        // current answers
        var currentAnswer = question.Answers.Select(x => x.Content).ToList(); // list<string>


        // add new answers 
        var newAnswers = request.Answers.Except(currentAnswer).ToList();
        newAnswers.ForEach(answers =>
        {
            question.Answers.Add(new Answer
            {
                Content = answers
            });
        });
        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        }); // if answer is active  true != false 
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
