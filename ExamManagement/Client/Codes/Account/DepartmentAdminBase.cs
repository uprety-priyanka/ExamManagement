using ExamManagement.Client.Data;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Account
{
    public class DepartmentAdminBase:ComponentBase
    {
        public List<DepartmentUserMessage> _messages;
        [Inject]
        public AccountGrpcService.AccountGrpcServiceClient _grpcClient { get; set; }
        [Inject]
        public EMSAuthenticationStateProvider _authProvider { get; set; }

        public string userRole = "";

        protected override async Task OnInitializedAsync()
        {

            var auth = await _authProvider.GetAuthenticationStateAsync();

            try
            {
                userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                await FetchData("");
            }
            catch (Exception) { }
        }

        public async Task FetchData(string s) 
        {
            _messages = new List<DepartmentUserMessage>();

            var result = _grpcClient.GetDepartmentUser(new UserSearchMessage
            {
                Search = s
            });

            while (await result.ResponseStream.MoveNext())
            {
                _messages.Add(new DepartmentUserMessage 
                {
                    Id = result.ResponseStream.Current.Id,
                    GivenName = result.ResponseStream.Current.GivenName,
                    SurName = result.ResponseStream.Current.SurName,
                    Department = result.ResponseStream.Current.Department,
                    EmailAddress = result.ResponseStream.Current.EmailAddress,
                    UserName = result.ResponseStream.Current.UserName
                });
            }
        }

        public TableGroupDefinition<DepartmentUserMessage> _groupDefinition = new()
        {
            GroupName = "Department",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.Department
        };

        public async Task OnSearch(string s) 
        {
            await FetchData(s);
        }
    }
}
