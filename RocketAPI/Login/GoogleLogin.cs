﻿using System.Threading.Tasks;
using DankMemes.GPSOAuthSharp;
using RocketAPI.Exceptions;

namespace RocketAPI.Login
{
    public class GoogleLogin : ILoginType
    {
        private readonly string _password;
        private readonly string _email;

        public GoogleLogin(string email, string password)
        {
            this._email = email;
            this._password = password;
        }

#pragma warning disable 1998
        public async Task<string> GetAccessToken()
#pragma warning restore 1998
        {
            var client = new GPSOAuthClient(_email, _password);
            var response = client.PerformMasterLogin();

            if (response.ContainsKey("Error"))
                throw new GoogleException(response["Error"]);

            //Todo: captcha/2fa implementation

            if (!response.ContainsKey("Auth"))
                throw new GoogleOfflineException();

            var oauthResponse = client.PerformOAuth(response["Token"],
                "audience:server:client_id:848232511240-7so421jotr2609rmqakceuu1luuq0ptb.apps.googleusercontent.com",
                "com.nianticlabs.pokemongo",
                "321187995bc7cdc2b5fc91b11a96e2baa8602c62");

            if (!oauthResponse.ContainsKey("Auth"))
                throw new GoogleOfflineException();

            return oauthResponse["Auth"];
        }
    }
}