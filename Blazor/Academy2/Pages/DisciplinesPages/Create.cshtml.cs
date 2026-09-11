using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Academy2.Data;

namespace Academy2.Pages.DisciplinesPages;

public class CreateModel : PageModel
{
    private readonly Academy2Context _context;

    public CreateModel(Academy2Context context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Disciplines Disciplines { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Disciplines.Add(Disciplines);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
