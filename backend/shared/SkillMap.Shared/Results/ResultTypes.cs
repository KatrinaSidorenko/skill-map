namespace SkillMap.Shared.Results;

public class ResultTypes
{
    private const string FAILED_TO_SAVE = "failed_to_save";
    private const string FAILED_TO_CREATE = "failed_to_create";
    private const string FAILED_TO_DELETE = "failed_to_delete";
    private const string FAILED_TO_UPDATE = "failed_to_update";
    private const string FAILED_TO_GET = "failed_to_get";
    private const string NOT_FOUND = "not_found";
    private const string INVALID_PASSWORD = "invalid_password";
    private const string USER_ALREADY_EXISTS = "user_with_such_email_already_exists";

    public static Result<T> FailedToSave<T>(string message) => Result.Failure<T>(FAILED_TO_SAVE, message);
    private static Result<T> FailedToGet<T>(string message) => Result.Failure<T>(FAILED_TO_GET, message);
    private static Result<T> FailedToCreate<T>(string message) => Result.Failure<T>(FAILED_TO_CREATE, message);
    private static Result<T> FailedToDelete<T>(string message) => Result.Failure<T>(FAILED_TO_DELETE, message);
    private static Result<T> FailedToUpdate<T>(string message) => Result.Failure<T>(FAILED_TO_UPDATE, message);
    private static Result<T> NotFound<T>(string message) => Result.Failure<T>(NOT_FOUND, message);


    public static Result<T> FailedToGetRoadmap<T>(string roadmapId) => FailedToGet<T>($"Failed to get roadmap with id {roadmapId}");
    public static Result<T> UserWithSuchEmailAlreadyExists<T>(string email) =>
        Result.Failure<T>(USER_ALREADY_EXISTS, $"User with email {email} already exists");
    public static Result<T> FailedToCreateUser<T>(string email) =>
        FailedToCreate<T>($"Failed to create user with email {email}");
    public static Result<T> UserNotFound<T>(string email) => NotFound<T>($"User with email {email} not found");
    public static Result<T> InvalidPassword<T>(string email) =>
        Result.Failure<T>(INVALID_PASSWORD, $"Invalid password for user with email {email}");


    // USER ROADMAP
    public static Result<T> UserRoadmapNotFound<T>(long userId, string roadmapId) 
        => NotFound<T>($"User roadmap not found for user {userId} and roadmap {roadmapId}");
    public static Result<T> UserRoadmapNotFound<T>(long userId) =>
        NotFound<T>($"User roadmap not found for user {userId}");
    public static Result<T> FailedToAddRoadmap<T>(long userId, string roadmapId) =>
        FailedToCreate<T>($"Failed to add roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToRemoveRoadmap<T>(long userId, string roadmapId) =>
        FailedToDelete<T>($"Failed to remove roadmap {roadmapId} for user {userId}");


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
    public static Result<T> FailedToCreateRoadmap<T>(long userId, string roadmapId) =>
        FailedToCreate<T>($"Failed to create customized roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToSave<T>(long userId, string roadmapId) => 
        FailedToSave<T>($"Failed to save customized roadmap {roadmapId} for user {userId}");

}
