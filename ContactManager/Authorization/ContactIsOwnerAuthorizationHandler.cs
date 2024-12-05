using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ContactManager.Authorization
{
    //The ContactIsOwnerAuthorizationHandler calls context.Succeed if the current
    //authenticated user is the contact owner. Authorization handlers generally:
    /*Call context.Succeed when the requirements are met.
     *Return Task.CompletedTask when requirements aren't met. Returning 
     *Task.CompletedTask without a prior call to context.Success or context.Fail, 
     *is not a success or failure, it allows other authorization handlers to run.
     */

    //The app allows contact owners to edit/delete/create their own data.
    //ContactIsOwnerAuthorizationHandler doesn't need to check the operation passed
    //in the requirement parameter.


    public class ContactIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, contact>
    {
        UserManager<IdentityUser> _userManager;

        public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            contact resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }
            //if nto asking for CRUD operations, return.

            if(requirement.Name != Constants.CreateOperationName &&
                 requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }
            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

    }

}
