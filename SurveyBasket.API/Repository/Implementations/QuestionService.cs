﻿namespace SurveyBasket.API.Repository.Implementations;

public class QuestionService(SurveyBasketDbContext context,
    HybridCache hybridCache,
    ILogger<QuestionService> logger
    ) : IQuestionService
{
    private readonly SurveyBasketDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly ILogger<QuestionService> _logger = logger;
    private const string _cachePrefix = "availableQuestions";

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

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
        return Result.Success(response.Adapt<QuestionResponse>());
    }

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExist)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);
        // 
        var query = _context.Questions
            .Where(x => x.PollId == pollId);
        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            query = query.Where(x => x.Content.Contains(filters.SearchValue)); // filter items
        }
        if (!string.IsNullOrEmpty(filters.SortColumn))
        {
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
        }

        var sourse = query
            .Include(a => a.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking();

        var response = await PaginatedList<QuestionResponse>.CreateAsync(sourse, filters.PageNumber, filters.PageSize, cancellationToken);
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

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);
        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DublicatedVote);
        var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{_cachePrefix}-{pollId}"; // key unique for cacheing 

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
            cacheKey,
            async cacheEntry => await _context.Questions
                .Where(x => x.PollId == pollId && x.IsActive)
                .Include(x => x.Answers)
                .Select(q => new QuestionResponse(
                    q.Id,
                    q.Content,
                    q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken)
        );

        return Result.Success(questions!);
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

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

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

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }
}
