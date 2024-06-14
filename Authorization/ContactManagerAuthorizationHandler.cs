using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ContactManager.Authorization;

public class ContactManagerAuthorizationHandler: AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, Contact resource)
    {
        if (context.User is null || resource is null)
            return Task.CompletedTask;
        
        if ((requirement.Name != Constants.ApproveOperationName) && 
            (requirement.Name != Constants.RejectOperationName))
            return Task.CompletedTask;
        
        if (context.User.IsInRole(Constants.ContactManagersRole))
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}
