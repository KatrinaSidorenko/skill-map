namespace LearningPlatform.Core.IntegrationTests.Account;
internal sealed record AccountParameters(long Id, string Email, string UserName, string Password, string Role)
{
    internal static AccountParameters GetValid() => new(
        Id: 1,
        Email: "testuser@gmail.com",
        UserName: "testuser",
        Password: "Test@1234",
        Role: "User"
    );
}
