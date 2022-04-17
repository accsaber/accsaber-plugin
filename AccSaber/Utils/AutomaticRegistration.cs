// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using AccSaber.Models;
// using AccSaber.UI.Panel;
// using SiraUtil.Web;
// using Zenject;
//
// namespace AccSaber.Utils
// {
//     public class AutomaticRegistration : IInitializable
//     {
//         private IHttpService _httpService;
//         private AccSaberPanelViewController _panelViewController;
//         private IPlatformUserModel _platformUser;
//         private UserIDUtils _userID;
//         private AccSaberUserModel _userModel;
//
//         public AutomaticRegistration(AccSaberPanelViewController panelViewController, UserIDUtils userID, IPlatformUserModel platformUser, IHttpService httpService, AccSaberUserModel userModel)
//         {
//             _httpService = httpService;
//             _userModel = userModel;
//             _panelViewController = panelViewController;
//             _platformUser = platformUser;
//             _userID = userID;
//         }
//
//         public void Initialize() => _ = InitializeAsync();
//
//         private async Task InitializeAsync()
//         {
//             var userId = _userID.UserInfo;
//             if (userId == null)
//             {
//                 if (_userModel.Registered == false)
//                 {
//                     HandleRegistrationProgress();
//                     _userModel.Registered = true;
//                 }
//                 return;
//             }
//
//             var userInfo = await _platformUser.GetUserInfo();
//             if (userInfo == null)
//             {
//                 return;
//             }
//
//             var response = await _httpService.GetAsync($"https://scoresaber.com/api/player/{userInfo.platformUserId}/full").ConfigureAwait(false);
//             var scoreSaberUserInfo = await ResponseParser.ParseWebResponse<ScoreSaberUserInfo>(response);
//             if (scoreSaberUserInfo?.ErrorMessage == "Player not found")
//             {
//                 _panelViewController.PromptText = "<color=red>Please submit some scores from your ScoreSaber account.</color>";
//                 _panelViewController.LoadingActive = false;
//                 return;
//             }
//
//             var content = new Dictionary<string, string>
//             {
//                 { "url", $"https://scoresaber.com/u/{userInfo.platformUserId}" },
//                 { "playerName", $"{userInfo.userName}" },
//                 { "hmd", String.Empty}
//             };
//
//             response = await _httpService.PostAsync("https://api.accsaber.com/players", content);
//             var regEntry = await ResponseParser.ParseWebResponse<AccSaberUserModel>(response);
//             if (regEntry != null)
//             {
//                 HandleRegistrationProgress();
//             }
//             else
//             {
//                 _panelViewController.PromptText = "Please register for AccSaber on the website.";
//                 _panelViewController.LoadingActive = false;
//             }
//         }
//         
//
//         private void HandleRegistrationProgress()
//         {
//             _panelViewController.PromptText = "Registering AccSaber user, please wait..";
//             _panelViewController.LoadingActive = true;
//         }
//
//         
//     }
// }