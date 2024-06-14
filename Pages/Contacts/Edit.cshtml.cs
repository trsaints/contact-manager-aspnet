using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Pages.Contacts
{
    public class EditModel : DI_BasePageModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EditModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
                return NotFound();
            

#pragma warning disable CS8601 // Possible null reference assignment.
            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
#pragma warning restore CS8601 // Possible null reference assignment.

            if (contact is null)
                return NotFound();

            Contact = contact;
            
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Update);
            
            if (!isAuthorized.Succeeded)
                return Forbid();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
                return Page();

            var contact = await Context.Contact.AsNoTracking().FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact is null)
                return NotFound();
            
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact, ContactOperations.Update);
            
            if (!isAuthorized.Succeeded)
                return Forbid();
            
            Contact.OwnerID = contact.OwnerID;
            Context.Attach(Contact).State = EntityState.Modified;

            if (Contact.Status is ContactStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Approve);
                
                if (!canApprove.Succeeded)
                    Contact.Status = ContactStatus.Submitted;
            }
            
            await Context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }

        private bool ContactExists(int id) => Context.Contact.Any(e => e.ContactId == id);
    }
}
