using System.Net;

namespace Idams.Core.Http;

public class ErrorCode
{
    public ErrorCode(string? code, string message)
    {
        Code = code;
        Message = message;
    }
    public ErrorCode(string? code, string message, HttpStatusCode statusCode)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
    }
    public string? Code { get; private set; }
    public string Message { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }
}
public static  class ErrorCodes 
{
    public static ErrorCode Forbidden
    {
        get
        {
            return new ErrorCode("U001", "Forbidden", HttpStatusCode.Forbidden);
        }
    }
    public static ErrorCode TestNotFound
    {
        get
        {
            return new ErrorCode("T001", "Test Not Found", HttpStatusCode.BadRequest);
        }
    }
    public static ErrorCode AnswerNotFound
    {
        get
        {
            return new ErrorCode("A001", "Answer Not Found", HttpStatusCode.BadRequest);
        }
    }

    public static ErrorCode QuestionNotFound
    {
        get
        {
            return new ErrorCode("Q001", "Question Not Found", HttpStatusCode.BadRequest);
        }
    }

    public static ErrorCode DurationTimeout
    {
        get
        {
            return new ErrorCode("D001", "Run out of time", HttpStatusCode.BadRequest);
        }
    }

    public static ErrorCode PartNotFound
    {
        get
        {
            return new ErrorCode("P001", "Part Not Found", HttpStatusCode.BadRequest);
        }
    }
    public static ErrorCode SectionNotFound
    {
        get
        {
            return new ErrorCode("S001", "Section Not Found", HttpStatusCode.BadRequest);
        }
    }
    public static ErrorCode InvalidToken
    {
        get
        {
            return new ErrorCode("U002", "Invalid Token", HttpStatusCode.Unauthorized);
        }
    }
    public static ErrorCode TestNotCompletedYet
    {
        get
        {
            return new ErrorCode("T005", "Test Not Completed Yet", HttpStatusCode.BadRequest);
        }
    }
    public static ErrorCode InvalidQuestionType
    {
        get
        {
            return new ErrorCode("Q003", "Invalid question type", HttpStatusCode.BadRequest);
        }
    }
    public static ErrorCode BadRequest
    {
        get
        {
            return new ErrorCode("400", "BadRequest", HttpStatusCode.BadRequest);
        }
    }
}