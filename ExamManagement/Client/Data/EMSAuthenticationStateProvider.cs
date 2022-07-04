using Google.Protobuf.WellKnownTypes;
using Grpc.Protos;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ExamManagement.Client.Data
{
    public class EMSAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AccountGrpcService.AccountGrpcServiceClient _grpcClient;

        public EMSAuthenticationStateProvider(AccountGrpcService.AccountGrpcServiceClient grpcClient) 
        {
            _grpcClient = grpcClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var getCurrentUser = await _grpcClient.GetCurrentUserAsync(new Empty());

            if (getCurrentUser.IsSuccess) 
            {
                var claims = new []
                {
                    new Claim(ClaimTypes.NameIdentifier, getCurrentUser.Id),
                    new Claim(ClaimTypes.GivenName, getCurrentUser.GivenName),
                    new Claim(ClaimTypes.Surname, getCurrentUser.SurName),
                    new Claim(ClaimTypes.Email, getCurrentUser.EmailAddress),
                    new Claim(ClaimTypes.Name, getCurrentUser.UserName),
                    new Claim(ClaimTypes.Role, getCurrentUser.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Authentication");

                return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task UpdateAuthenticationState() 
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
