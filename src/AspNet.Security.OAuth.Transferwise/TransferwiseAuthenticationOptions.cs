/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.OAuth.Transferwise {
    /// <summary>
    /// Defines a set of options used by <see cref="TransferwiseAuthenticationHandler"/>.
    /// </summary>
    public class TransferwiseAuthenticationOptions : OAuthOptions {
        /// <summary>
        /// Defines TransferwiseAuthenticationOptions.
        /// </summary>
        public TransferwiseAuthenticationOptions() {
            AuthenticationScheme = TransferwiseAuthenticationDefaults.AuthenticationScheme;
            DisplayName = TransferwiseAuthenticationDefaults.DisplayName;
            ClaimsIssuer = TransferwiseAuthenticationDefaults.Issuer;

            CallbackPath = new PathString(TransferwiseAuthenticationDefaults.CallbackPath);

            AuthorizationEndpoint = TransferwiseAuthenticationDefaults.AuthorizationEndpoint;
            TokenEndpoint = TransferwiseAuthenticationDefaults.TokenEndpoint;
            UserInformationEndpoint = TransferwiseAuthenticationDefaults.UserInformationEndpoint;
        }
    }
}
