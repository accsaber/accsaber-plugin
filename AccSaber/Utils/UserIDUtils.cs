using System.Threading;
using System.Threading.Tasks;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Utils
{
    internal class UserIDUtils : IInitializable
    {
        private readonly IPlatformUserModel _userModel = null!;

        public UserInfo UserInfo;

        public UserIDUtils(IPlatformUserModel userModel)
        {
            _userModel = userModel;
        }

        public void Initialize()
        {
            GetUserID();
        }

        private async void GetUserID()
        {
            UserInfo = await _userModel.GetUserInfo();
        }
    }
}