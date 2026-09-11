using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Academy2.Data;

namespace Academy2.Pages.DisciplinesPages;

public class IndexModel : PageModel
{
    private readonly Academy2Context _context;

    public IndexModel(Academy2Context context)
    {
        _context = context;
    }

    public IList<Disciplines> Disciplines { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Disciplines = await _context.Disciplines.ToListAsync();
    }
}
