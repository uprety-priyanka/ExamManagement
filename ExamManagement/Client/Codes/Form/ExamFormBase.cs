using ExamManagement.Client.Data;
using ExamManagement.Shared.Constants;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protos;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace ExamManagement.Client.Codes.Form;

public class ExamFormBase : ComponentBase
{

    [Inject]
    public EMSAuthenticationStateProvider _authProvider { get; set; }
    public string userRole { get; set; }
    public List<FormResultMessage> formResultMessages = new();
    [Inject]
    public ExamFormGrpcService.ExamFormGrpcServiceClient _examFormGrpcServiceClient { get; set; }
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var auth = await _authProvider.GetAuthenticationStateAsync();
            userRole = auth.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;

            if (userRole == RoleConstant.STUDENT) 
            {
                formResultMessages = new List<FormResultMessage>();
                var stream = _examFormGrpcServiceClient.GetFormPerStudent(new Empty());

                while (await stream.ResponseStream.MoveNext()) 
                {
                    formResultMessages.Add(stream.ResponseStream.Current);
                }
            }
            else if (userRole == RoleConstant.DEPARTMENTADMIN)
            {
                formResultMessages = new List<FormResultMessage>();
                var stream = _examFormGrpcServiceClient.GetFormPerFaculty(new Empty());

                while (await stream.ResponseStream.MoveNext())
                {
                    formResultMessages.Add(stream.ResponseStream.Current);
                }
            }
        }
        catch (Exception) { }

    }

    public void NavigateToFillForm() 
    {
        Navigation.NavigateTo("/fillexamform");
    }

}
