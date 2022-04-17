using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Logging;
using SiraUtil.Web;
using Zenject;

namespace AccSaber.Downloaders
{
    public class UserInfoDownloader : Downloader
    {
        private const string API_URL = "https://api.accsaber.com/";
        private const string PLAYERS_ENDPOINT = "players/";

        private readonly SiraLog _siraLog;
        private UserIDUtils _userID;
        private CategoryUtils _category;

        public UserInfoDownloader(SiraLog siraLog) : base(siraLog)
        {
            _siraLog = siraLog;
        }

        public async Task<List<AccSaberUserModel>> GetUserInfoAsync(string userID, CancellationToken cancellationToken = default)
        {
            var url = API_URL + PLAYERS_ENDPOINT + userID + "/" + _category.GetCategoryString();
            _siraLog.Debug($"making request to: {url}");
            return await MakeJsonRequestAsync<List<AccSaberUserModel>>(url, cancellationToken);
        }
    }
}