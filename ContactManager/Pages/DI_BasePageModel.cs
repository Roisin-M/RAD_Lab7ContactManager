using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactManager.Pages
{
    //Create a base class that contains the services used in the contacts
    //Razor Pages. The base class puts the initialization code in one
    //location
    public class DI_BasePageModel:PageModel
    {   
        //adds the db context
        protected ApplicationDbContext Context { get; }
        //to access the authorization handlers
        protected IAuthorizationService AuthorizationService { get; }
        //adds tehidentity usermanager service
        protected UserManager<IdentityUser> UserManager { get; }

        public DI_BasePageModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }
    }
}
