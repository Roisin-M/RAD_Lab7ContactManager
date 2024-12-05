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
    public class DetailsModel : DI_BasePageModel
    {
        //private readonly ContactManager.Data.ApplicationDbContext _context;

        //public DetailsModel(ContactManager.Data.ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        public DetailsModel(
       ApplicationDbContext context,
       IAuthorizationService authorizationService,
       UserManager<IdentityUser> userManager)
       : base(context, authorizationService, userManager)
        {
        }

        public contact contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            contact? _contact = await Context.contact.FirstOrDefaultAsync(m => m.ContactId == id);
            //if (id == null)
            //{
            //    return NotFound();
            //}
            if (_contact == null)
            {
                return NotFound();
            }
            contact = _contact;
            //var contact = await _context.contact.FirstOrDefaultAsync(m => m.ContactId == id);

            //if (contact == null)
            //{
            //    return NotFound();
            //}
            //else
            //{
            //    contact = contact;
            //}
            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                          User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != contact.OwnerID
                && contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }
            return Page();
        }
        //new task
        public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
        {
            var contact = await Context.contact.FirstOrDefaultAsync(
                                                      m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ContactStatus.Approved)
                                                       ? ContactOperations.Approve
                                                       : ContactOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            contact.Status = status;
            Context.contact.Update(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        //on get fro forbid and challenge
        //public async Task<IActionResult> OnGetAsync(int id)
        //{
        //    Contact? _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

        //    if (_contact == null)
        //    {
        //        return NotFound();
        //    }
        //    Contact = _contact;

        //    if (!User.Identity!.IsAuthenticated)
        //    {
        //        return Challenge();
        //    }

        //    var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
        //                       User.IsInRole(Constants.ContactAdministratorsRole);

        //    var currentUserId = UserManager.GetUserId(User);

        //    if (!isAuthorized
        //        && currentUserId != Contact.OwnerID
        //        && Contact.Status != ContactStatus.Approved)
        //    {
        //        return Forbid();
        //    }

        //    return Page();
        //}
    }
}
