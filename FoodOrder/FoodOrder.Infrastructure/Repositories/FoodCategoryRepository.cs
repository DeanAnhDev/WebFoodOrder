using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class FoodCategoryRepository : Repository<FoodCategory>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<FoodCategory> GetFoodCategoriesWithFoods()
        {
            return _dbSet
                .AsNoTracking()
                .Select(fc => new FoodCategory
                {
                    FoodCategoryId = fc.FoodCategoryId,
                    CategoryName = fc.CategoryName,
                    Slug = fc.Slug,

                    Foods = fc.Foods
                        .Where(f => f.Status)
                        .Select(f => new Food
                        {
                            FoodId = f.FoodId,
                            FoodName = f.FoodName,
                            Slug = f.Slug,
                            Status = f.Status,
                            Price = f.Price,
                            Images = f.Images,
                            Quantity = f.Quantity
                        }).ToList(),

                    Combos = fc.Combos
                        .Where(c => c.Status)
                        .Select(c => new Combo
                        {
                            ComboId = c.ComboId,
                            ComboName = c.ComboName,
                            Slug = c.Slug,
                            Status = c.Status,
                            Price = c.Price,
                            Images = c.Images,
                            Quantity = c.Quantity
                        }).ToList()
                })
                .AsSplitQuery();
        }



        public IQueryable<FoodCategory> GetFoodsByCategorySlug(string categorySlug)
        {
             var result =  _dbSet
                .AsNoTracking()
                .Where(fc => fc.Slug == categorySlug)
               .Select(fc => new FoodCategory
               {
                   FoodCategoryId = fc.FoodCategoryId,
                   CategoryName = fc.CategoryName,
                   Slug = fc.Slug,

                   Foods = fc.Foods
                        .Where(f => f.Status)
                        .Select(f => new Food
                        {
                            FoodId = f.FoodId,
                            FoodName = f.FoodName,
                            Slug = f.Slug,
                            Status = f.Status,
                            Price = f.Price,
                            Images = f.Images,
                            Quantity = f.Quantity
                        }).ToList(),

                   Combos = fc.Combos
                        .Where(c => c.Status)
                        .Select(c => new Combo
                        {
                            ComboId = c.ComboId,
                            ComboName = c.ComboName,
                            Slug = c.Slug,
                            Status = c.Status,
                            Price = c.Price,
                            Images = c.Images,
                            Quantity = c.Quantity
                        }).ToList()
               })
                .AsSplitQuery();
            return result;
        }


        public async Task<IEnumerable<FoodCategory>> GetAllAsync()
        {
            var result = await _dbSet
                .Include(fc => fc.Images)
                .ToListAsync();

            return result;
        }

        public async Task<FoodCategory?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.FoodCategoryId == id);
        }

        public async Task<FoodCategory?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.Slug == slug);
        }
    }
}
