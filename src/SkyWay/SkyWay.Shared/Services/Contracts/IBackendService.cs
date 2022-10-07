using System.Threading.Tasks;

namespace SkyWay
{
    public interface IBackendService 
    {
        Task<(bool IsSuccess, string Message)> AuthenticateUser(string userNameOrEmail, string password);

        Task<(bool IsSuccess, string Message)> SignupUser(string fullName, string userName, string email, string password);

        Task<(bool IsSuccess, string Message)> SubmitScore(double score);

        Task<QueryRecordResponse<GameProfile>> GetGameProfile();

        Task<QueryRecordsResponse<GameScore>> GetGameScores(int pageIndex, int pageSize);

        Task<QueryRecordsResponse<GameProfile>> GetGameProfiles(int pageIndex, int pageSize);

        Task<(bool IsSuccess, string Message)> GenerateSession();

        Task<(bool IsSuccess, string Message)> ValidateSession(Session session);
    }
}
