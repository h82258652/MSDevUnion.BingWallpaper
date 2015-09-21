using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
    [AVClassName("_User")]
    public class AVUser : AVObject
    {
        private static readonly object currentUserMutex = new object();
        private static readonly IDictionary<string, IAVAuthenticationProvider> authProviders = (IDictionary<string, IAVAuthenticationProvider>)new Dictionary<string, IAVAuthenticationProvider>();
        private static AVUser currentUser;
        private static bool currentUserMatchesDisk;
        private string sessionToken;
        private bool isCurrentUser;

        internal IDictionary<string, IDictionary<string, object>> AuthData
        {
            get
            {
                IDictionary<string, IDictionary<string, object>> result;
                if (this.TryGetValue<IDictionary<string, IDictionary<string, object>>>("authData", out result))
                    return result;
                else
                    return (IDictionary<string, IDictionary<string, object>>)null;
            }
            private set
            {
                this["authData"] = (object)value;
            }
        }

        public static string CurrentSessionToken
        {
            get
            {
                lock (AVUser.currentUserMutex)
                  return AVUser.CurrentUser == null ? (string)null : AVUser.CurrentUser.SessionToken;
            }
        }

        public static AVUser CurrentUser
        {
            get
            {
                AVUser avUser;
                lock (AVUser.currentUserMutex)
                {
                    if (AVUser.currentUser != null)
                        avUser = AVUser.currentUser;
                    else if (!AVUser.currentUserMatchesDisk)
                    {
                        object local_0;
                        AVClient.ApplicationSettings.TryGetValue("CurrentUser", out local_0);
                        string local_2 = local_0 as string;
                        if (local_2 == null)
                        {
                            AVUser.SaveCurrentUser((AVUser)null);
                            avUser = (AVUser)null;
                        }
                        else
                        {
                            IDictionary<string, object> local_3 = AVClient.DeserializeJsonString(local_2);
                            AVUser local_4 = AVObject.CreateWithoutData<AVUser>((string)null);
                            local_4.MergeAfterFetch(local_3);
                            AVUser.currentUserMatchesDisk = true;
                            AVUser.SaveCurrentUser(local_4);
                            avUser = local_4;
                        }
                    }
                    else
                        avUser = AVUser.currentUser;
                }
                return avUser;
            }
        }

        [AVFieldName("email")]
        public string Email
        {
            get
            {
                return this.GetProperty<string>((string)null, "Email");
            }
            set
            {
                this.SetProperty<string>(value, "Email");
            }
        }

        [AVFieldName("mobilePhoneVerified")]
        public bool MobilePhoneVerified
        {
            get
            {
                return this.GetProperty<bool>(false, "MobilePhoneVerified");
            }
            private set
            {
                lock (this.mutex)
                  this.SetProperty<bool>(value, "MobilePhoneVerified");
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                lock (this.mutex)
                  return this.sessionToken != null && AVUser.CurrentUser != null && AVUser.CurrentUser.ObjectId == this.ObjectId;
            }
        }

        internal bool IsCurrentUser
        {
            get
            {
                return this.isCurrentUser;
            }
        }

        [AVFieldName("password")]
        public string Password
        {
            private get
            {
                return this.GetProperty<string>((string)null, "Password");
            }
            set
            {
                this.SetProperty<string>(value, "Password");
            }
        }

        public static AVQuery<AVUser> Query
        {
            get
            {
                return new AVQuery<AVUser>();
            }
        }

        internal string SessionToken
        {
            get
            {
                lock (this.mutex)
                  return this.sessionToken;
            }
        }

        [AVFieldName("username")]
        public string Username
        {
            get
            {
                return this.GetProperty<string>((string)null, "Username");
            }
            set
            {
                this.SetProperty<string>(value, "Username");
            }
        }

        [AVFieldName("mobilePhoneNumber")]
        public string MobilePhoneNumber
        {
            get
            {
                return this.GetProperty<string>((string)null, "MobilePhoneNumber");
            }
            set
            {
                this.SetProperty<string>(value, "MobilePhoneNumber");
            }
        }

        public bool IsAnonymous
        {
            get
            {
                bool flag = false;
                if (this.AuthData != null)
                    flag = this.AuthData.Keys.Contains("anonymous");
                return flag;
            }
        }

        public AVUser()
        {
            this.isCurrentUser = false;
        }

        public Task<bool> Follow(string userObjectId)
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("POST", string.Format("/users/{0}/friendship/{1}", (object)this.ObjectId, (object)userObjectId), this.sessionToken, (IDictionary<string, object>)null, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public Task<bool> Follow(string userObjectId, IDictionary<string, object> data)
        {
            if (data != null)
                data = this.ToJSONObjectForSaving(data);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("POST", string.Format("/users/{0}/friendship/{1}", (object)this.ObjectId, (object)userObjectId), this.sessionToken, data, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public Task<bool> Unfollow(string userObjectId)
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("DELETE", string.Format("/users/{0}/friendship/{1}", (object)this.ObjectId, (object)userObjectId), this.sessionToken, (IDictionary<string, object>)null, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public Task<IEnumerable<AVUser>> GetFollowers()
        {
            return ((AVQueryBase<AVQuery<AVUser>, AVUser>)this.GetFollowerQuery()).FindAsync();
        }

        public Task<IEnumerable<AVUser>> GetFollowees()
        {
            return ((AVQueryBase<AVQuery<AVUser>, AVUser>)this.GetFolloweeQuery()).FindAsync();
        }

        public AVQuery<AVUser> GetFollowerQuery()
        {
            return new AVQuery<AVUser>()
            {
                RelativeUri = string.Format("/users/{0}/followers", (object)this.ObjectId)
            };
        }

        public AVQuery<AVUser> GetFolloweeQuery()
        {
            return new AVQuery<AVUser>()
            {
                RelativeUri = string.Format("/users/{0}/followees", (object)this.ObjectId)
            };
        }

        public AVQuery<AVUser> GetFollowersAndFolloweesQuery()
        {
            return new AVQuery<AVUser>()
            {
                RelativeUri = string.Format("/users/{0}/followersAndFollowees", (object)this.ObjectId)
            };
        }

        public static Task<AVUser> BecomeAsync(string sessionToken)
        {
            return AVUser.BecomeAsync(sessionToken, CancellationToken.None);
        }

        public static Task<AVUser> BecomeAsync(string sessionToken, CancellationToken cancellationToken)
        {
            AVUser avUser = AVObject.CreateWithoutData<AVUser>((string)null);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVUser>(AVClient.RequestAsync("GET", "/users/me", sessionToken, (IDictionary<string, object>)null, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, AVUser>)(t =>
            {
                avUser.MergeAfterFetch(t.Result.Item2);
                AVUser.SaveCurrentUser(avUser);
                return avUser;
            }));
        }

        private void CleanupAuthData()
        {
            lock (this.mutex)
            {
                if (!this.IsCurrentUser)
                    return;
                IDictionary<string, IDictionary<string, object>> local_0 = this.AuthData;
                if (local_0 == null)
                    return;
                foreach (KeyValuePair<string, IDictionary<string, object>> item_0 in new Dictionary<string, IDictionary<string, object>>(local_0))
                {
                    if (item_0.Value == null)
                        local_0.Remove(item_0.Key);
                }
            }
        }

        internal static void ClearInMemoryUser()
        {
            lock (AVUser.currentUserMutex)
            {
                AVUser.currentUserMatchesDisk = false;
                AVUser.currentUser = (AVUser)null;
            }
        }

        internal override Task<AVObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken)
        {
            return InternalExtensions.OnSuccess<AVObject, AVObject>(base.FetchAsyncInternal(toAwait, cancellationToken), (Func<Task<AVObject>, AVObject>)(t =>
            {
                if (AVUser.CurrentUser == this)
                    AVUser.SaveCurrentUser(this);
                return t.Result;
            }));
        }

        private static IAVAuthenticationProvider GetProvider(string providerName)
        {
            IAVAuthenticationProvider authenticationProvider;
            if (AVUser.authProviders.TryGetValue(providerName, out authenticationProvider))
                return authenticationProvider;
            else
                return (IAVAuthenticationProvider)null;
        }

        internal bool IsLinked(string authType)
        {
            lock (this.mutex)
              return this.AuthData != null && this.AuthData.ContainsKey(authType) && this.AuthData[authType] != null;
        }

        internal Task LinkWithAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            IDictionary<string, IDictionary<string, object>> dictionary1 = this.AuthData;
            if (dictionary1 == null)
            {
                Dictionary<string, IDictionary<string, object>> dictionary2 = new Dictionary<string, IDictionary<string, object>>();
                IDictionary<string, IDictionary<string, object>> dictionary3 = (IDictionary<string, IDictionary<string, object>>)dictionary2;
                this.AuthData = (IDictionary<string, IDictionary<string, object>>)dictionary2;
                dictionary1 = dictionary3;
            }
            dictionary1[authType] = data;
            return base.SaveAsync(cancellationToken);
        }

        internal Task LinkWithAsync(string authType, CancellationToken cancellationToken)
        {
            return TaskExtensions.Unwrap(InternalExtensions.OnSuccess<IDictionary<string, object>, Task>(AVUser.GetProvider(authType).AuthenticateAsync(cancellationToken), (Func<Task<IDictionary<string, object>>, Task>)(t => this.LinkWithAsync(authType, t.Result, cancellationToken))));
        }

        public static void BeginAuthenticate(string loginUrl, EventHandler<AVCompleteAuthorizationEventArgs> completeAuthorizationHandler)
        {
            AVClient.platformHooks.PopupWebBrowser(loginUrl, completeAuthorizationHandler);
        }

        public static void BeginAuthenticate(string OAuthClientID, string OAuthClientSecret, string AuthType, EventHandler<AVCompleteAuthorizationEventArgs> CompleteAuthorizationHandler)
        {
            string[] array = new string[3]
            {
        "weibo",
        "qq",
        "weixin"
            };
            string[] strArray = new string[3]
            {
        "https://open.weibo.cn/oauth2/authorize?client_id={0}&response_type=code&redirect_uri=http://avoscloudsnshelper.chinacloudsites.cn/weibo/{1}/{2}",
        "https://graph.qq.com/oauth2.0/authorize",
        "https://open.weixin.qq.com/connect/qrconnect?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=SCOPE&state=STATE#wechat_redirect"
            };
            string format = string.Empty;
            int index = Array.IndexOf<string>(array, AuthType);
            if (index > -1)
                format = strArray[index];
            string uri = string.Format(format, (object)OAuthClientID, (object)OAuthClientID, (object)OAuthClientSecret);
            AVClient.platformHooks.PopupWebBrowser(uri, CompleteAuthorizationHandler);
        }

        public static Task<AVUser> LogInWithAuthData(string snsType, string accessToken, string openId, long expiresIn)
        {
            return AVUser.LogInWithAuthData(snsType, accessToken, openId, expiresIn, CancellationToken.None);
        }

        public static Task<AVUser> LogInWithAuthData(string snsType, string accessToken, string openId, long expiresIn, CancellationToken cancellationToken)
        {
            IDictionary<string, object> data = AVUser.BuildAuthData(snsType, accessToken, openId, expiresIn);
            AVUser avUser = AVObject.CreateWithoutData<AVUser>((string)null);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVUser>(AVClient.RequestAsync("POST", "/users", (string)null, data, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, AVUser>)(t =>
            {
                avUser.MergeAfterFetch(t.Result.Item2);
                AVUser.SaveCurrentUser(avUser);
                return avUser;
            }));
        }

        public static Task<bool> AssociateWithAuthData(AVUser user, string snsType, string accessToken, string openId, long expiresIn)
        {
            return AVUser.AssociateWithAuthData(user, snsType, accessToken, openId, expiresIn, CancellationToken.None);
        }

        public static Task<bool> AssociateWithAuthData(AVUser user, string snsType, string accessToken, string openId, long expiresIn, CancellationToken cancellationToken)
        {
            AVUser.CheckAVUserForSNS(user);
            IDictionary<string, object> data = AVUser.BuildAuthData(snsType, accessToken, openId, expiresIn);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("PUT", "/users/" + user.ObjectId, user.SessionToken, data, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public static Task<bool> DissociateAuthData(AVUser user, string snsType)
        {
            return AVUser.DissociateAuthData(user, snsType, CancellationToken.None);
        }

        public static Task<bool> DissociateAuthData(AVUser user, string snsType, CancellationToken cancellationToken)
        {
            AVUser.CheckAVUserForSNS(user);
            IDictionary<string, object> data = AVUser.BuildAuthData(snsType, (string)null, (string)null, 0L);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("PUT", "/users/" + user.ObjectId, user.SessionToken, data, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        private static void CheckAVUserForSNS(AVUser user)
        {
            if (user.ObjectId == null)
                throw new AVException(AVException.ErrorCode.CloudNotFindUser, "Cloud not find user.", (Exception)null);
            if (user.SessionToken == null)
                throw new AVException(AVException.ErrorCode.SessionMissing, "The user cannot be altered by a client without the session.", (Exception)null);
        }

        private static IDictionary<string, object> BuildAuthData(string snsType, string accessToken, string openId, long expiresIn)
        {
            IDictionary<string, object> dictionary1 = (IDictionary<string, object>)new Dictionary<string, object>();
            IDictionary<string, object> dictionary2 = (IDictionary<string, object>)new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(openId))
                dictionary2["openid"] = (object)openId;
            if (!string.IsNullOrEmpty(accessToken))
                dictionary2["access_token"] = (object)accessToken;
            if (expiresIn > 0L)
                dictionary2["expires_in"] = (object)expiresIn;
            dictionary1[snsType] = (object)dictionary2;
            IDictionary<string, object> dictionary3 = (IDictionary<string, object>)new Dictionary<string, object>();
            dictionary3["authData"] = (object)dictionary1;
            return dictionary3;
        }

        public static Task<AVUser> LogInAsync(string username, string password)
        {
            return AVUser.LogInAsync(username, password, CancellationToken.None);
        }

        public static Task<AVUser> LogInByMobilePhoneNumberAsync(string mobilePhoneNumber, string password)
        {
            return AVUser.LogInByMobilePhoneNumberAsync(mobilePhoneNumber, password, CancellationToken.None);
        }

        public static Task<AVUser> LoginBySmsCodeAsync(string mobilePhoneNumber, string smsCode)
        {
            return AVUser.LoginBySmsCodeAsync(mobilePhoneNumber, smsCode, CancellationToken.None);
        }

        public static Task<AVUser> LogInAsync(string username, string password, CancellationToken cancellationToken)
        {
            return AVUser.LoginWithParametersAsync(new Dictionary<string, object>()
      {
        {
          "username",
          (object) username
        },
        {
          "password",
          (object) password
        }
      }, cancellationToken);
        }

        public static Task<AVUser> LogInByEmailAsync(string email, string password)
        {
            return AVUser.LoginWithParametersAsync(new Dictionary<string, object>()
      {
        {
          "username",
          (object) email
        },
        {
          "password",
          (object) password
        }
      }, CancellationToken.None);
        }

        public static Task<AVUser> LogInAsync()
        {
            IDictionary<string, object> data = (IDictionary<string, object>)new Dictionary<string, object>();
            IDictionary<string, object> authData = (IDictionary<string, object>)new Dictionary<string, object>();
            IDictionary<string, object> dictionary = (IDictionary<string, object>)new Dictionary<string, object>();
            dictionary.Add("id", (object)Guid.NewGuid().ToString());
            authData.Add("anonymous", (object)dictionary);
            data.Add("authData", (object)authData);
            AVUser avUser = AVObject.CreateWithoutData<AVUser>((string)null);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVUser>(AVClient.RequestAsync("POST", "/users", (string)null, data, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, AVUser>)(t =>
            {
                avUser.AuthData = (IDictionary<string, IDictionary<string, object>>)new Dictionary<string, IDictionary<string, object>>()
        {
          {
            "anonymous",
            authData
          }
        };
                avUser.MergeAfterFetch(t.Result.Item2);
                AVUser.SaveCurrentUser(avUser);
                return avUser;
            }));
        }

        public static Task<AVUser> LogInByMobilePhoneNumberAsync(string mobilePhoneNumber, string password, CancellationToken cancellationToken)
        {
            return AVUser.LoginWithParametersAsync(new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        },
        {
          "password",
          (object) password
        }
      }, cancellationToken);
        }

        public static Task<AVUser> LoginBySmsCodeAsync(string mobilePhoneNumber, string smsCode, CancellationToken cancellationToken)
        {
            return AVUser.LoginWithParametersAsync(new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        },
        {
          "smsCode",
          (object) smsCode
        }
      }, cancellationToken);
        }

        public static Task<AVUser> SignUpOrLoginByMobilePhone(string mobilePhoneNumber, string smsCode)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        },
        {
          "smsCode",
          (object) smsCode
        }
      };
            AVUser avUser = AVObject.CreateWithoutData<AVUser>((string)null);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVUser>(AVClient.RequestAsync("POST", "/usersByMobilePhone", (string)null, (IDictionary<string, object>)dictionary, CancellationToken.None), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, AVUser>)(t =>
            {
                avUser.MergeAfterFetch(t.Result.Item2);
                AVUser.SaveCurrentUser(avUser);
                return avUser;
            }));
        }

        internal static Task<AVUser> LoginWithParametersAsync(Dictionary<string, object> strs, CancellationToken cancellationToken)
        {
            AVUser avUser = AVObject.CreateWithoutData<AVUser>((string)null);
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVUser>(AVClient.RequestAsync("GET", string.Format("/login?{0}", (object)AVClient.BuildQueryString((IDictionary<string, object>)strs)), (string)null, (IDictionary<string, object>)null, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, AVUser>)(t =>
            {
                avUser.MergeAfterFetch(t.Result.Item2);
                AVUser.SaveCurrentUser(avUser);
                return avUser;
            }));
        }

        public static Task<bool> RequestLoginSmsCodeAsync(string mobilePhoneNumber)
        {
            return AVUser.RequestLoginSmsCodeAsync(mobilePhoneNumber, CancellationToken.None);
        }

        public static Task<bool> RequestLoginSmsCodeAsync(string mobilePhoneNumber, CancellationToken cancellationToken)
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("POST", "/requestLoginSmsCode", (string)null, (IDictionary<string, object>)new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        }
      }, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        internal static Task<AVUser> LogInWithAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            AVUser strs = AVObject.Create<AVUser>();
            strs.AuthData = (IDictionary<string, IDictionary<string, object>>)new Dictionary<string, IDictionary<string, object>>();
            strs.AuthData[authType] = data;
            return InternalExtensions.OnSuccess<AVUser>(strs.SignUpAsync(cancellationToken), (Func<Task, AVUser>)(_ =>
            {
                strs.SynchronizeAllAuthData();
                return strs;
            }));
        }

        internal static Task<AVUser> LogInWithAsync(string authType, CancellationToken cancellationToken)
        {
            return TaskExtensions.Unwrap<AVUser>(InternalExtensions.OnSuccess<IDictionary<string, object>, Task<AVUser>>(AVUser.GetProvider(authType).AuthenticateAsync(cancellationToken), (Func<Task<IDictionary<string, object>>, Task<AVUser>>)(authData => AVUser.LogInWithAsync(authType, authData.Result, cancellationToken))));
        }

        public static void LogOut()
        {
            AVUser.LogOutWithProviders();
            AVUser.SaveCurrentUser((AVUser)null);
        }

        private static void LogOutWithProviders()
        {
            foreach (IAVAuthenticationProvider authenticationProvider in (IEnumerable<IAVAuthenticationProvider>)AVUser.authProviders.Values)
                authenticationProvider.Deauthenticate();
        }

        internal override void MergeAfterFetch(IDictionary<string, object> result)
        {
            lock (this.mutex)
            {
                base.MergeAfterFetch(result);
                this.SynchronizeAllAuthData();
            }
        }

        internal override void MergeAfterSave(IDictionary<string, object> result)
        {
            lock (this.mutex)
            {
                base.MergeAfterSave(result);
                this.SynchronizeAllAuthData();
                this.CleanupAuthData();
                this.serverData.Remove("password");
                this.RebuildEstimatedData();
            }
        }

        internal override void MergeMagicFields(IDictionary<string, object> data)
        {
            lock (this.mutex)
            {
                if (data.ContainsKey("sessionToken"))
                {
                    this.sessionToken = data["sessionToken"] as string;
                    data.Remove("sessionToken");
                }
                base.MergeMagicFields(data);
            }
        }

        internal static void RegisterProvider(IAVAuthenticationProvider provider)
        {
            AVUser.authProviders[provider.AuthType] = provider;
            AVUser currentUser = AVUser.CurrentUser;
            if (currentUser == null)
                return;
            currentUser.SynchronizeAuthData(provider);
        }

        public override void Remove(string key)
        {
            if (key == "username")
                throw new ArgumentException("Cannot remove the username key.");
            base.Remove(key);
        }

        public static Task RequestPasswordResetAsync(string email)
        {
            return AVUser.RequestPasswordResetAsync(email, CancellationToken.None);
        }

        public static Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
        {
            return (Task)AVClient.RequestAsync("POST", "/requestPasswordReset", AVUser.CurrentSessionToken, (IDictionary<string, object>)new Dictionary<string, object>()
      {
        {
          "email",
          (object) email
        }
      }, cancellationToken);
        }

        public static Task RequestPasswordResetBySmsCode(string mobilePhoneNumber)
        {
            return AVUser.RequestPasswordResetBySmsCode(mobilePhoneNumber, CancellationToken.None);
        }

        public static Task RequestPasswordResetBySmsCode(string mobilePhoneNumber, CancellationToken cancellationToken)
        {
            return (Task)AVClient.RequestAsync("POST", "/requestPasswordResetBySmsCode", AVUser.CurrentSessionToken, (IDictionary<string, object>)new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        }
      }, cancellationToken);
        }

        public static Task<bool> ResetPasswordBySmsCode(string newPassword, string smsCode)
        {
            return AVUser.ResetPasswordBySmsCode(newPassword, smsCode, CancellationToken.None);
        }

        public static Task<bool> ResetPasswordBySmsCode(string newPassword, string smsCode, CancellationToken cancellationToken)
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "password",
          (object) newPassword
        }
      };
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("PUT", "/resetPasswordBySmsCode/" + smsCode, currentSessionToken, (IDictionary<string, object>)dictionary, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public static Task<bool> RequestMobilePhoneVerifyAsync(string mobilePhoneNumber)
        {
            return AVUser.RequestMobilePhoneVerifyAsync(mobilePhoneNumber, CancellationToken.None);
        }

        public static Task<bool> RequestMobilePhoneVerifyAsync(string mobilePhoneNumber, CancellationToken cancellationToken)
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("POST", "/requestMobilePhoneVerify", AVUser.CurrentSessionToken, (IDictionary<string, object>)new Dictionary<string, object>()
      {
        {
          "mobilePhoneNumber",
          (object) mobilePhoneNumber
        }
      }, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        public static Task<bool> VerifyMobilePhoneAsync(string code, string mobilePhoneNumber)
        {
            return AVUser.VerifyMobilePhoneAsync(code, mobilePhoneNumber, CancellationToken.None);
        }

        public static Task<bool> VerifyMobilePhoneAsync(string code, string mobilePhoneNumber, CancellationToken cancellationToken)
        {
            return InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, bool>(AVClient.RequestAsync("POST", "/verifyMobilePhone/" + code.Trim() + "?mobilePhoneNumber=" + mobilePhoneNumber.Trim(), (string)null, (IDictionary<string, object>)null, cancellationToken), (Func<Task<Tuple<HttpStatusCode, IDictionary<string, object>>>, bool>)(t => t.Result.Item1 == HttpStatusCode.OK));
        }

        internal override Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
        {
            lock (this.mutex)
            {
                if (this.ObjectId == null)
                    throw new InvalidOperationException("You must call SignUpAsync before calling SaveAsync.");
                else
                    return InternalExtensions.OnSuccess(base.SaveAsync(toAwait, cancellationToken), (Action<Task>)(_ =>
                    {
                        if (AVUser.CurrentUser != this)
                            return;
                        AVUser.SaveCurrentUser(this);
                    }));
            }
        }

        internal static void SaveCurrentUser(AVUser user)
        {
            AVUser avUser = AVUser.currentUser;
            if (avUser != null)
                Monitor.Enter(avUser.mutex);
            if (user != null)
                Monitor.Enter(user.mutex);
            try
            {
                lock (AVUser.currentUserMutex)
                {
                    if (AVUser.currentUser != null && AVUser.currentUser != user)
                        AVUser.currentUser.isCurrentUser = false;
                    AVUser.currentUser = user;
                    if (AVUser.currentUser == null)
                    {
                        AVClient.ApplicationSettings["CurrentUser"] = (object)null;
                    }
                    else
                    {
                        AVUser.currentUser.isCurrentUser = true;
                        IDictionary<string, object> local_1 = AVUser.currentUser.ServerDataToJSONObjectForSerialization();
                        local_1["sessionToken"] = (object)AVUser.currentUser.SessionToken;
                        local_1["objectId"] = (object)AVUser.currentUser.ObjectId;
                        DateTime local_2 = AVUser.currentUser.CreatedAt.Value;
                        local_1["createdAt"] = (object)local_2.ToString(AVClient.dateFormatString);
                        DateTime local_3 = AVUser.currentUser.UpdatedAt.Value;
                        local_1["updatedAt"] = (object)local_3.ToString(AVClient.dateFormatString);
                        AVClient.ApplicationSettings["CurrentUser"] = (object)AVClient.SerializeJsonString(local_1);
                    }
                    AVUser.currentUserMatchesDisk = true;
                }
            }
            finally
            {
                if (avUser != null)
                    Monitor.Exit(avUser.mutex);
                if (user != null)
                    Monitor.Exit(user.mutex);
            }
        }

        internal Task SignUpAsync(Task toAwait, CancellationToken cancellationToken)
        {
            lock (this.mutex)
            {
                if (this.AuthData == null)
                {
                    if (string.IsNullOrEmpty(this.Username))
                        throw new InvalidOperationException("Cannot sign up user with an empty name.");
                    if (string.IsNullOrEmpty(this.Password))
                        throw new InvalidOperationException("Cannot sign up user with an empty password.");
                }
                if (this.ObjectId != null && !this.IsAnonymous)
                    throw new InvalidOperationException("Cannot sign up a user that already exists.");
                else
                    return InternalExtensions.OnSuccess(base.SaveAsync(toAwait, cancellationToken), (Action<Task>)(_ =>
                    {
                        if (this.IsAnonymous)
                            this.AuthData.Remove("anonymous");
                        AVUser.SaveCurrentUser(this);
                    }));
            }
        }

        public Task SignUpAsync()
        {
            return this.SignUpAsync(CancellationToken.None);
        }

        public Task SignUpAsync(CancellationToken cancellationToken)
        {
            return this.taskQueue.Enqueue<Task>((Func<Task, Task>)(toAwait => this.SignUpAsync(toAwait, cancellationToken)), cancellationToken);
        }

        private void SynchronizeAllAuthData()
        {
            lock (this.mutex)
            {
                IDictionary<string, IDictionary<string, object>> local_0 = this.AuthData;
                if (local_0 == null)
                    return;
                foreach (KeyValuePair<string, IDictionary<string, object>> item_0 in (IEnumerable<KeyValuePair<string, IDictionary<string, object>>>)local_0)
                    this.SynchronizeAuthData(AVUser.GetProvider(item_0.Key));
            }
        }

        private void SynchronizeAuthData(IAVAuthenticationProvider provider)
        {
            bool flag = false;
            lock (this.mutex)
            {
                IDictionary<string, IDictionary<string, object>> local_2 = this.AuthData;
                if (!this.IsCurrentUser || local_2 == null || provider == null)
                    return;
                IDictionary<string, object> local_0;
                if (local_2.TryGetValue(provider.AuthType, out local_0))
                    flag = provider.RestoreAuthentication(local_0);
            }
            if (flag)
                return;
            this.UnlinkFromAsync(provider.AuthType, CancellationToken.None);
        }

        internal Task UnlinkFromAsync(string authType, CancellationToken cancellationToken)
        {
            return this.LinkWithAsync(authType, (IDictionary<string, object>)null, cancellationToken);
        }
    }
}