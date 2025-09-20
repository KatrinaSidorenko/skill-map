namespace SkillMap.Shared.Results;

public class ResultTypes
{
    private const string FAILED_TO_SAVE = "failed_to_save";
    public static Result<T> FailedToSave<T>(string message) => Result.Failure<T>(FAILED_TO_SAVE, message);
    public static Result<T> FailedToGet<T>(string code, string message) => Result.Failure<T>(code, message);

    private const string FAILED_TO_GET_ROADMAP = "failed_to_get_roadmap";
    public static Result<T> FailedToGetRoadmap<T>(string message) => FailedToGet<T>(FAILED_TO_GET_ROADMAP, message);


    // GENERIC
    public static Result<T> FailedToGet<T>(string message) => Result.Failure<T>("failed_to_get", message);
    public static Result<T> NotFound<T>(string message) => Result.Failure<T>("not_found", message);
   

    // USER
    public static Result<T> UserWithSuchEmailAlreadyExists<T>(string email) =>
        Result.Failure<T>("user_with_such_email_already_exists", $"User with email {email} already exists");
    public static Result<T> FailedToCreateUser<T>(string email) =>
        Result.Failure<T>("failed_to_create_user", $"Failed to create user with email {email}");
    public static Result<T> UserNotFound<T>(string email) =>
        Result.Failure<T>("user_not_found", $"User with email {email} not found");
    public static Result<T> InvalidPassword<T>(string email) =>
        Result.Failure<T>("invalid_password", $"Invalid password for user with email {email}");


    // USER ROADMAP
    public static Result<T> UserRoadmapNotFound<T>(long userId, string roadmapId) =>
        Result.Failure<T>("user_roadmap_not_found", $"User roadmap not found for user {userId} and roadmap {roadmapId}");
    public static Result<T> UserRoadmapNotFound<T>(long userId) =>
        Result.Failure<T>("user_roadmap_not_found", $"User roadmap not found for user {userId}");
    public static Result<T> FailedToAddRoadmap<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_add_roadmap", $"Failed to add roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToRemoveRoadmap<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_remove_roadmap", $"Failed to remove roadmap {roadmapId} for user {userId}");


    // ROADMAP
    public static Result<T> RoadmapNotFound<T>(string roadmapId) =>
        Result.Failure<T>("roadmap_not_found", $"Roadmap with id {roadmapId} not found");
    public static Result<T> FailedToGetRoadmaps<T>() =>
        Result.Failure<T>("failed_to_get_roadmap", "Failed to get roadmaps");

    // CUSTOMIZED ROADMAP
    public static Result<T> FailedToDeleteRoadmap<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_delete_roadmap", $"Failed to delete roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToGetCustomizedRoadmap<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_get_customized_roadmap", $"Failed to get customized roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToGetModifications<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_get_modifications", $"Failed to get modifications for roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToCreateRoadmap<T>(long userId, string roadmapId) =>
        Result.Failure<T>("failed_to_create_roadmap", $"Failed to create roadmap {roadmapId} for user {userId}");
    public static Result<T> FailedToSave<T>(long userId, string roadmapId)
         => Result.Failure<T>("failed_to_save", $"Failed to save roadmap {roadmapId} for user {userId}");

}
