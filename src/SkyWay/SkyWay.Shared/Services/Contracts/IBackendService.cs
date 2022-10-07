using System.Threading.Tasks;

namespace SkyWay
{
    public interface IBackendService
    {
        Task<(bool IsSuccess, string Message)> AuthenticateUser(string userNameOrEmail, string password);

        Task<(bool IsSuccess, string Message)> SignupUser(string fullName, string userName, string email, string password);

        Task<(bool IsSuccess, string Message)> SubmitUserGameScore(double score);

        Task<(bool IsSuccess, string Message)> GenerateUserSession();

        Task<(bool IsSuccess, string Message)> ValidateUserSession(Session session);

        Task<QueryRecordResponse<GameProfile>> GetGameProfile();

        Task<QueryRecordsResponse<GameScore>> GetGameScores(int pageIndex, int pageSize);

        Task<QueryRecordsResponse<GameProfile>> GetGameProfiles(int pageIndex, int pageSize);
    }
}
