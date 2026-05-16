namespace LearningPlatform.Core.IntegrationTests.Account.RegisterAccount;
internal sealed record RegisterUserRequestParameters(string Email, string UserName, string Password, string Role)
{
    internal static RegisterUserRequestParameters GetValid() => new(
        Email: "testuser@gmail.com",
        UserName: "testuser",
        Password: "Test@1234",
        Role: "User"
    );
}
