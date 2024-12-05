using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Pages
{
    //allow anonymous access to index page
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {
        //private readonly ContactManager.Data.ApplicationDbContext _context;
        public IndexModel(
       ApplicationDbContext context,
       IAuthorizationService authorizationService,
       UserManager<IdentityUser> userManager)
       : base(context, authorizationService, userManager)
        {
        }
        //public IndexModel(ContactManager.Data.ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        public IList<contact> contact { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var contacts = from c in Context.contact
                           select c;

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                contacts = contacts.Where(c => c.Status == ContactStatus.Approved
                || c.OwnerID == currentUserId);
                // contact = await _context.contact.ToListAsync();
            }
            contact = await contacts.ToListAsync();
        }
    }
}
