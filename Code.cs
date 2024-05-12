// Models
public enum QuestionType
{
    Paragraph,
    YesNo,
    Dropdown,
    MultipleChoice,
    Date,
    Number
}

public class QuestionModel
{
    public string Id { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    // Additional properties based on question type
}

public class ApplicationModel
{
    public string ApplicantId { get; set; }
    public Dictionary<string, string> Answers { get; set; } // QuestionId -> Answer
}

// Service Interface
public interface IQuestionService
{
    Task<QuestionModel> CreateQuestionAsync(QuestionModel question);
    Task<QuestionModel> UpdateQuestionAsync(string id, QuestionModel question);
    Task<List<QuestionModel>> GetAllQuestionsAsync();
}

public interface IApplicationService
{
    Task SubmitApplicationAsync(ApplicationModel application);
}

// Service Implementation
public class QuestionService : IQuestionService
{
    private readonly ICosmosDbService _cosmosDbService;

    public QuestionService(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public async Task<QuestionModel> CreateQuestionAsync(QuestionModel question)
    {
        return await _cosmosDbService.AddItemAsync(question);
    }

    public async Task<QuestionModel> UpdateQuestionAsync(string id, QuestionModel question)
    {
        return await _cosmosDbService.UpdateItemAsync(id, question);
    }

    public async Task<List<QuestionModel>> GetAllQuestionsAsync()
    {
        return await _cosmosDbService.GetItemsAsync<QuestionModel>();
    }
}

public class ApplicationService : IApplicationService
{
    private readonly ICosmosDbService _cosmosDbService;

    public ApplicationService(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public async Task SubmitApplicationAsync(ApplicationModel application)
    {
        // Store application in Cosmos DB or perform further processing
        await _cosmosDbService.AddItemAsync(application);
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(QuestionModel question)
    {
        var createdQuestion = await _questionService.CreateQuestionAsync(question);
        return Ok(createdQuestion);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(string id, QuestionModel question)
    {
        var updatedQuestion = await _questionService.UpdateQuestionAsync(id, question);
        return Ok(updatedQuestion);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions()
    {
        var questions = await _questionService.GetAllQuestionsAsync();
        return Ok(questions);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitApplication(ApplicationModel application)
    {
        await _applicationService.SubmitApplicationAsync(application);
        return Ok("Application submitted successfully");
    }
}
