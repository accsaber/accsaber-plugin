using System.Threading.Tasks;

namespace AccSaber.Utils
{
    public class UserIDUtils
    {
        private readonly IPlatformUserModel _userModel;

        public UserIDUtils(IPlatformUserModel userModel)
        {
            _userModel = userModel;
        }

        public async Task<string> GetUserID()
        {
            var userID = await _userModel.GetUserInfo();
            return userID.platformUserId;
        }
    }
}