using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;

namespace ContactManager.Pages
{
    public class DeleteModel : DI_BasePageModel
    {
        //private readonly ContactManager.Data.ApplicationDbContext _context;

        //public DeleteModel(ContactManager.Data.ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        public DeleteModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public contact contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}
            contact? _contact = await Context.contact.FirstOrDefaultAsync(
                                             m => m.ContactId == id);


           // var contact = await _context.contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (_contact == null)
            {
                return NotFound();
            }
            contact= _contact;
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                User, contact,
                                                ContactOperations.Delete);
            //else
            //{
            //    contact = contact;
            //}
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}
            var contact = await Context
           .contact.AsNoTracking()
           .FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                User, contact,
                                                ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.contact.Remove(contact);
            await Context.SaveChangesAsync();

            //var contact = await _context.contact.FindAsync(id);
            //if (contact != null)
            //{
            //    contact = contact;
            //    _context.contact.Remove(contact);
            //    await _context.SaveChangesAsync();
            //}

            return RedirectToPage("./Index");
        }
    }
}
