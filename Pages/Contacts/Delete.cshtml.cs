using ContactManager.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Pages.Contacts
{
    public class DeleteModel : DI_BasePageModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DeleteModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            
            if (_contact is null)
                return NotFound();

            Contact = _contact;
            
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Delete);
            
            if (!isAuthorized.Succeeded)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            
            if (contact is null)
                return NotFound();

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Delete);
            
            if (!isAuthorized.Succeeded)
                return Forbid();

            Context.Contact.Remove(Contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
