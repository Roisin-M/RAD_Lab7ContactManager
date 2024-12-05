using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;

namespace ContactManager.Pages
{
    public class EditModel : DI_BasePageModel
    {
        //private readonly ContactManager.Data.ApplicationDbContext _context;
        public EditModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
        {
        }
        //public EditModel(ContactManager.Data.ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        [BindProperty]
        public contact contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var contact =  await _context.contact.FirstOrDefaultAsync(m => m.ContactId == id);
            contact? contact = await Context.contact.FirstOrDefaultAsync(
                                                        m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }
            contact = contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                  User, contact,
                                                  ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //_context.Attach(contact).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!contactExists(contact.ContactId))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            // Fetch Contact from DB to get OwnerID.
            var contact = await Context
                .contact.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, contact,
                                                     ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            contact.OwnerID = contact.OwnerID;

            Context.Attach(contact).State = EntityState.Modified;

            if (contact.Status == ContactStatus.Approved)
            {
                // If the contact is updated after approval, 
                // and the user cannot approve,
                // set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        contact,
                                        ContactOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    contact.Status = ContactStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        //private bool contactExists(int id)
        //{
        //    return _context.contact.Any(e => e.ContactId == id);
        //}
    }
}
