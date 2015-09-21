using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
	public static class AVCloud
	{
		public static Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters)
		{
			return AVCloud.CallFunctionAsync<T>(name, parameters, CancellationToken.None);
		}

		public static Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, CancellationToken cancellationToken)
		{
            return AVClient.RequestAsync("POST", new Uri($"/functions/{Uri.EscapeUriString(name)}", UriKind.Relative), AVUser.CurrentSessionToken, (IDictionary<string, object>)AVClient.MaybeEncodeJSONObject(parameters, false), cancellationToken).OnSuccess(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                IDictionary<string, object> dictionary = AVClient.Decode(t.Result.Item2) as IDictionary<string, object>;
                if (!dictionary.ContainsKey("result"))
                {
                    return default(T);
                }
                return (T)((object)AVClient.ConvertTo<T>(dictionary["result"]));
            });
        }

		public static Task<DateTime> GetServerDateTime()
		{
            return AVClient.RequestAsync("GET", "/date", null, null, CancellationToken.None).OnSuccess(delegate (Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
            {
                DateTime result = DateTime.MinValue;
                if (t.Result.Item1 == HttpStatusCode.OK)
                {
                    object obj = AVClient.Decode(t.Result.Item2);
                    if (obj != null && obj is DateTime)
                    {
                        result = (DateTime)obj;
                    }
                }
                return result;
            });
        }

		public static Task<bool> RequestSMSCode(string mobilePhoneNumber, string name, string op, int ttl)
		{
			return AVCloud.RequestSMSCode(mobilePhoneNumber, name, op, ttl, CancellationToken.None);
		}

		public static Task<bool> RequestSMSCode(string mobilePhoneNumber, string name, string op, int ttl, CancellationToken cancellationToken)
		{
            if (string.IsNullOrEmpty(mobilePhoneNumber))
            {
                throw new AVException(AVException.ErrorCode.MobilePhoneInvalid, "Moblie Phone number is invalid.", null);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("mobilePhoneNumber", mobilePhoneNumber);
            Dictionary<string, object> dictionary2 = dictionary;
            if (!string.IsNullOrEmpty(name))
            {
                dictionary2.Add("name", name);
            }
            if (!string.IsNullOrEmpty(op))
            {
                dictionary2.Add("op", op);
            }
            if (ttl > 0)
            {
                dictionary2.Add("ttl", ttl);
            }
            return AVClient.RequestAsync("POST", "/requestSmsCode", null, dictionary2, cancellationToken).OnSuccess(t => t.Result.Item1 == HttpStatusCode.OK);
        }

		public static Task<bool> RequestSMSCode(string mobilePhoneNumber)
		{
			return AVCloud.RequestSMSCode(mobilePhoneNumber, CancellationToken.None);
		}

		public static Task<bool> RequestSMSCode(string mobilePhoneNumber, CancellationToken cancellationToken)
		{
			return AVCloud.RequestSMSCode(mobilePhoneNumber, null, null, 0, cancellationToken);
		}

		public static Task<bool> RequestSMSCode(string mobilePhoneNumber, string template, IDictionary<string, object> env)
		{
            if (string.IsNullOrEmpty(mobilePhoneNumber))
            {
                throw new AVException(AVException.ErrorCode.MobilePhoneInvalid, "Moblie Phone number is invalid.", null);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("mobilePhoneNumber", mobilePhoneNumber);
            Dictionary<string, object> dictionary2 = dictionary;
            dictionary2.Add("template", template);
		    foreach (var k in env.Keys)
		    {
                dictionary2.Add(k, env[k]);
            }
            return AVClient.RequestAsync("POST", "/requestSmsCode", null, dictionary2, CancellationToken.None).OnSuccess(t => t.Result.Item1 == HttpStatusCode.OK);
        }

		public static Task<bool> VerifySmsCode(string code, string mobilePhoneNumber)
		{
			return AVCloud.VerifySmsCode(code, mobilePhoneNumber, CancellationToken.None);
		}

		public static Task<bool> VerifySmsCode(string code, string mobilePhoneNumber, CancellationToken cancellationToken)
		{
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("code", code.Trim());
            dictionary.Add("mobilePhoneNumber", mobilePhoneNumber.Trim());
            return AVClient.RequestAsync("POST", "/verifySmsCode/" + code.Trim() + "?mobilePhoneNumber=" + mobilePhoneNumber.Trim(), null, null, cancellationToken).OnSuccess(t => t.Result.Item1 == HttpStatusCode.OK);
        }
	}
}