using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;

namespace WebAppEstimate.Areas.SiteBram.Pipeline;

public static class WinchAnchorExcelInitializer
{
    public static void SeedData(AppDbContextWinchAnchor context)
    {
        // Здесь потом:
            // 1. открыть Excel
                // 2. прочитать Series
                    // 3. прочитать Weight
                        // 4. прочитать Shaft
                            // 5. прочитать Chain
                                // 6. заполнить WinchAnchorDesign
                                    // 7. context.SaveChanges();
    }
}