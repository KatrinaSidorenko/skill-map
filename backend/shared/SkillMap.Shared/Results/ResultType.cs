namespace SkillMap.Shared.Results;

public class ResultType
{
    public static Result<T> FailedToSave<T>(string message) => Result.Failure<T>(ErrorCode.FAILED_TO_SAVE, message);
    public static Result<T> FailedToGet<T>(string message) => Result.Failure<T>(ErrorCode.FAILED_TO_GET, message);
    public static Result<T> FailedToCreate<T>(string message) => Result.Failure<T>(ErrorCode.FAILED_TO_CREATE, message);
    public static Result<T> FailedToDelete<T>(string message) => Result.Failure<T>(ErrorCode.FAILED_TO_DELETE, message);
    public static Result<T> FailedToUpdate<T>(string message) => Result.Failure<T>(ErrorCode.FAILED_TO_UPDATE, message);
    public static Result<T> NotFound<T>(string message) => Result.Failure<T>(ErrorCode.NOT_FOUND, message);
    public static Result<T> ValidationError<T>(IEnumerable<string> errors) =>
        Result.Failure<T>(ErrorCode.VALIDATION_ERROR, "Validation error: " + string.Join(" | ", errors));


    public static Result<T> FailedToGetRoadmap<T>(string roadmapId) => FailedToGet<T>($"Failed to get roadmap with id {roadmapId}");
    public static Result<T> UserWithSuchEmailAlreadyExists<T>(string email) =>
        Result.Failure<T>(ErrorCode.USER_ALREADY_EXISTS, $"User with email {email} already exists");
    public static Result<T> FailedToCreateUser<T>(string email) =>
        FailedToCreate<T>($"Failed to create user with email {email}");
    public static Result<T> UserNotFound<T>(string email) => NotFound<T>($"User with email {email} not found");
    public static Result<T> InvalidPassword<T>(string email) =>
        Result.Failure<T>(ErrorCode.INVALID_PASSWORD, $"Invalid password for user with email {email}");


    // USER ROADMAP
    public static Result<T> UserRoadmapNotFound<T>(long userId, string roadmapId) 
        => NotFound<T>($"User roadmap not found for user {userId} and roadmap {roadmapId}");
    public static Result<T> UserRoadmapNotFound<T>(long userId) =>
        NotFound<T>($"User roadmap not found for user {userId}");
    public static Result<T> FailedToAddRoadmap<T>(long userId, string roadmapId) =>
        FailedToCreate<T>($"Failed to add roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToRemoveRoadmap<T>(long userId, string roadmapId) =>
        FailedToDelete<T>($"Failed to remove roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToCreateUserRoadmap<T>(long userId, string message) =>
        FailedToCreate<T>($"Failed to create user roadmap for user {userId}. Details: {message}");
    public static Result<T> FailedToGetUserRoadmaps<T>(long userId) =>
        FailedToGet<T>($"Failed to get roadmaps for user {userId}");

    // ROADMAP
    public static Result<T> RoadmapNotFound<T>(string roadmapId) =>
        NotFound<T>($"Roadmap with id {roadmapId} not found");
    public static Result<T> FailedToGetRoadmaps<T>() =>
        FailedToGet<T>("Failed to get roadmaps");

    // CUSTOMIZED ROADMAP
    public static Result<T> FailedToDeleteRoadmap<T>(long userId, string roadmapId) =>
       FailedToDelete<T>($"Failed to delete roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToGetCustomizedRoadmap<T>(long userId, string roadmapId) =>
        FailedToGet<T>($"Failed to get customized roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToGetModifications<T>(long userId, string roadmapId) =>
        FailedToGet<T>($"Failed to get modifications for roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToApplyModifications<T>(long userId, string roadmapId) =>
        FailedToSave<T>($"Failed to apply modifications to customized roadmap {roadmapId} for user {userId}");

    public static Result<T> FailedToCreateRoadmap<T>(long userId, string roadmapId) =>
        FailedToCreate<T>($"Failed to create customized roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToSave<T>(long userId, string roadmapId) => 
        FailedToSave<T>($"Failed to save customized roadmap {roadmapId} for user {userId}");

    public static Result<T> FailedToSendEmail<T>(string email) =>
        Result.Failure<T>(ErrorCode.EMAIL_SENDING_FAILED, $"Failed to send email to {email}");
    public static Result<T> InvalidOrExpiredToken<T>() =>
        Result.Failure<T>(ErrorCode.INVALID_OR_EXPIRED_TOKEN, "The provided token is invalid or has expired");
    public static Result<T> FailedToUpdatePassword<T>(string email) =>
        FailedToUpdate<T>($"Failed to update password for user with email {email}");
}
